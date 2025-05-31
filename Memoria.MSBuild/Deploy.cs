using Memoria.Patcher;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Task = System.Threading.Tasks.Task;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Memoria.MSBuild
{
    public class Deploy : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        [Required]
        public String SolutionDir { get; set; }

        [Required]
        public String TargetPath { get; set; }

        [Required]
        public String TargetDir { get; set; }

        [Required]
        public String TargetName { get; set; }

        private readonly TaskLoggingHelper _log;

        public Deploy()
        {
            _log = new TaskLoggingHelper(this);
        }

        public Boolean Execute()
        {
            if (BuildEnvironment.IsDebug)
                Debugger.Launch();

            MdbGenerator generator = new MdbGenerator(this);
            generator.BeginGenerateMdbSymbols();

            FileCopier copier = FileCopier.TryGetFileCopier(this);
            if (copier != null)
            {
                copier.BeginCopyDll();
                copier.BeginCopyPdb();
            }

            // Wait until .mdb symbols generated to able to copy it
            generator.EndGenerateMdbSymbols();

            if (copier != null)
            {
                copier.BeginCopyMdb();
                copier.EndCopy();
            }

            return true;
        }

        private sealed class MdbGenerator
        {
            private readonly Deploy _deployTask;
            private Process _process;

            public MdbGenerator(Deploy deployTask)
            {
                _deployTask = deployTask;
            }

            public void BeginGenerateMdbSymbols()
            {
                String mdbGenPath = _deployTask.SolutionDir + @"References\pdb2mdb.exe";
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(mdbGenPath)
                    {
                        Arguments = $"\"{_deployTask.TargetPath}\"",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    _process = Process.Start(startInfo);
                }
                catch (FileNotFoundException ex)
                {
                    _deployTask._log.LogError("Failed to generate .mdb symbols because a file not found: {0}", ex.FileName);
                }
                catch (Exception ex)
                {
                    _deployTask._log.LogError("Failed to generate .mdb symbols.");
                    _deployTask._log.LogErrorFromException(ex, showStackTrace: true);
                }
            }

            public void EndGenerateMdbSymbols()
            {
                if (_process == null)
                    return;

                try
                {
                    if (!_process.WaitForExit(60000))
                    {
                        _deployTask._log.LogError("Failed to generate .mdb symbols. Timeout expired.");
                        _process.Kill();
                    }
                    else
                    {
                        if (_process.ExitCode != 0)
                            _deployTask._log.LogError("Failed to generate .mdb symbols. Exit code: {0}", _process.ExitCode);
                    }
                }
                catch (Exception ex)
                {
                    _deployTask._log.LogError("Failed to generate .mdb symbols.");
                    _deployTask._log.LogErrorFromException(ex, showStackTrace: true);
                }

                _process.Dispose();
                _process = null;
            }
        }

        private sealed class FileCopier
        {
            private const Int32 EstimatedTaskCountPerPlatform = 2; // .dll, .mdb (skip .pdb)

            private readonly Deploy _deployTask;
            private readonly String _sourcePath;
            private readonly String _targetPathX64;
            private readonly String _targetPathX86;
            private readonly List<Task> _tasks;

            private FileCopier(Deploy deployTask, String assemblyDirectoryX64, String assemblyDirectoryX86)
            {
                _deployTask = deployTask;
                _sourcePath = Path.Combine(deployTask.TargetDir, deployTask.TargetName);

                Int32 estimatedTaskCount = 0;

                if (assemblyDirectoryX64 != null)
                {
                    _targetPathX64 = Path.Combine(assemblyDirectoryX64, _deployTask.TargetName);
                    estimatedTaskCount += EstimatedTaskCountPerPlatform;
                }

                if (assemblyDirectoryX86 != null)
                {
                    _targetPathX86 = Path.Combine(assemblyDirectoryX86, _deployTask.TargetName);
                    estimatedTaskCount += EstimatedTaskCountPerPlatform;
                }

                _tasks = new List<Task>(estimatedTaskCount);
            }

            public static FileCopier TryGetFileCopier(Deploy deployTask)
            {
                try
                {
                    GameLocationInfo gameLocation = GameLocationSteamRegistryProvider.TryLoad();
                    if (gameLocation == null)
                    {
                        LogWarning(deployTask, "Cannot find the game folder.");
                        return null;
                    }

                    Boolean isX64Exists = Directory.Exists(gameLocation.ManagedPathX64);
                    Boolean isX86Exists = Directory.Exists(gameLocation.ManagedPathX86);

                    if (isX64Exists)
                    {
                        return isX86Exists
                            ? new FileCopier(deployTask, gameLocation.ManagedPathX64, gameLocation.ManagedPathX86)
                            : new FileCopier(deployTask, gameLocation.ManagedPathX64, null);
                    }
                    if (isX86Exists)
                    {
                        return new FileCopier(deployTask, null, gameLocation.ManagedPathX86);
                    }

                    LogWarning(deployTask, $"Cannot find an assembly directory in the game folder [{gameLocation.RootDirectory}].");
                    return null;
                }
                catch (FileNotFoundException ex)
                {
                    LogWarning(deployTask, $"Cannot find a launcher in the game folder [{ex.FileName}].");
                    return null;
                }
                catch (Exception ex)
                {
                    LogWarning(deployTask, "Cannot find the game folder.");
                    deployTask._log.LogWarningFromException(ex, showStackTrace: true);
                    return null;
                }
            }

            private static void LogWarning(Deploy deployTask, String warning)
            {
                deployTask._log.LogWarning($"{warning} {deployTask.TargetName} will not be deployed automatically.");
            }

            public void BeginCopyDll()
            {
                BeginCopy(".dll", logSuccess: true);
            }

            public void BeginCopyPdb()
            {
                // We do not need .pdb files. Only .mdb will use for debug.
                // BeginCopy(".pdb", logSuccess: false);
            }

            public void BeginCopyMdb()
            {
                BeginCopy(".dll.mdb", logSuccess: false);
            }

            private void BeginCopy(String extension, Boolean logSuccess)
            {
                String sourceItem = _sourcePath + extension;

                if (_targetPathX64 != null)
                {
                    String targetItem = _targetPathX64 + extension;
                    _tasks.Add(Task.Factory.StartNew(() => CopyFile(sourceItem, targetItem, logSuccess)));
                }

                if (_targetPathX86 != null)
                {
                    String targetItem = _targetPathX86 + extension;
                    _tasks.Add(Task.Factory.StartNew(() => CopyFile(sourceItem, targetItem, logSuccess)));
                }
            }

            public void EndCopy()
            {
                try
                {
                    if (!Task.WaitAll(_tasks.ToArray(), 10000))
                        _deployTask._log.LogWarning("Cannot deploy {0} to the game. Timeout expired.", _deployTask.TargetName);
                }
                catch (Exception ex)
                {
                    _deployTask._log.LogWarning("Cannot deploy {0} to the game.", _deployTask.TargetName);
                    _deployTask._log.LogWarningFromException(ex, showStackTrace: true);
                }
            }

            private void CopyFile(String sourceItem, String targetItem, Boolean logSuccess)
            {
                try
                {
                    File.Copy(sourceItem, targetItem, true);
                    if (logSuccess)
                        _deployTask._log.LogMessage(MessageImportance.High, "{0}Deployed [{1}]: {2}{0}", Environment.NewLine, _deployTask.TargetName, targetItem);
                }
                catch (Exception ex)
                {
                    _deployTask._log.LogWarning(Environment.NewLine + "Cannot deploy {0} to the game." + Environment.NewLine, _deployTask.TargetName);
                    _deployTask._log.LogWarningFromException(ex, showStackTrace: false);
                }
            }
        }
    }
}
