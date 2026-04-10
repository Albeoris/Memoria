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

        public String TargetPath { get; set; }

        public String TargetDir { get; set; }

        public String TargetName { get; set; }

        public String OutputDir { get; set; }

        public String GamePath { get; set; }

        public String SourceFileName { get; set; }

        public String DestinationFileName { get; set; }

        public Boolean CopyMdbSymbols { get; set; } = true;

        public Boolean DeployToRootDirectory { get; set; }

        private readonly TaskLoggingHelper _log;

        public Deploy()
        {
            _log = new TaskLoggingHelper(this);
        }

        public Boolean Execute()
        {
            if (BuildEnvironment.IsDebug)
                Debugger.Launch();

            if (!String.IsNullOrWhiteSpace(OutputDir))
                return ExecuteOutputDeployment();

            if (String.IsNullOrWhiteSpace(TargetPath) || String.IsNullOrWhiteSpace(TargetDir) || String.IsNullOrWhiteSpace(TargetName))
            {
                _log.LogError("Deploy requires either OutputDir for solution deployment or TargetPath/TargetDir/TargetName for project deployment.");
                return false;
            }

            MdbGenerator generator = new MdbGenerator(this);
            if (CopyMdbSymbols)
                generator.BeginGenerateMdbSymbols();

            FileCopier copier = FileCopier.TryGetFileCopier(this);
            if (copier != null)
            {
                copier.BeginCopyMainFile();
                copier.BeginCopyPdb();
                copier.BeginCopySharedFiles();
            }

            // Wait until .mdb symbols generated to able to copy it
            if (CopyMdbSymbols)
                generator.EndGenerateMdbSymbols();

            if (copier != null)
            {
                if (CopyMdbSymbols)
                    copier.BeginCopyMdb();
                copier.EndCopy();
            }

            return true;
        }

        private Boolean ExecuteOutputDeployment()
        {
            _log.LogMessage(MessageImportance.High, "Deploying Memoria output to game installation...");

            FileCopier copier = FileCopier.TryGetOutputFileCopier(this);
            if (copier == null)
            {
                _log.LogMessage(MessageImportance.High, "Output deployment skipped: Game installation not found or output directory does not exist.");
                return true;
            }

            copier.BeginCopyDeploymentSet();
            copier.EndCopy();
            _log.LogMessage(MessageImportance.High, "Output deployment completed.");
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
                _process = StartProcess(_deployTask, _deployTask.TargetPath);
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

            public static void Generate(Deploy deployTask, String targetPath)
            {
                Process process = StartProcess(deployTask, targetPath);
                if (process == null)
                    return;

                try
                {
                    if (!process.WaitForExit(60000))
                    {
                        deployTask._log.LogError("Failed to generate .mdb symbols. Timeout expired.");
                        process.Kill();
                    }
                    else if (process.ExitCode != 0)
                    {
                        deployTask._log.LogError("Failed to generate .mdb symbols. Exit code: {0}", process.ExitCode);
                    }
                }
                catch (Exception ex)
                {
                    deployTask._log.LogError("Failed to generate .mdb symbols.");
                    deployTask._log.LogErrorFromException(ex, showStackTrace: true);
                }
                finally
                {
                    process.Dispose();
                }
            }

            private static Process StartProcess(Deploy deployTask, String targetPath)
            {
                String mdbGenPath = deployTask.SolutionDir + @"References\pdb2mdb.exe";
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(mdbGenPath)
                    {
                        Arguments = $"\"{targetPath}\"",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    return Process.Start(startInfo);
                }
                catch (FileNotFoundException ex)
                {
                    deployTask._log.LogError("Failed to generate .mdb symbols because a file not found: {0}", ex.FileName);
                }
                catch (Exception ex)
                {
                    deployTask._log.LogError("Failed to generate .mdb symbols.");
                    deployTask._log.LogErrorFromException(ex, showStackTrace: true);
                }

                return null;
            }
        }

        private sealed class FileCopier
        {
            private const Int32 EstimatedTaskCountPerPlatform = 2; // .dll, .mdb (skip .pdb)

            private readonly Deploy _deployTask;
            private readonly String _sourcePath;
            private readonly String _destinationFileName;
            //private readonly String _sourcePdbPath;
            //private readonly String _destinationPdbFileName;
            private readonly String _sourceMdbPath;
            private readonly String _destinationMdbFileName;
            private readonly String _targetPathX64;
            private readonly String _targetPathX86;
            private readonly String _rootDirectory;
            private readonly Boolean _hasX64;
            private readonly Boolean _hasX86;
            private readonly String _outputDir;
            private readonly List<Task> _tasks;

            private FileCopier(Deploy deployTask, String assemblyDirectoryX64, String assemblyDirectoryX86, String rootDirectory, Boolean hasX64, Boolean hasX86, String outputDir)
            {
                _deployTask = deployTask;

                Int32 estimatedTaskCount = 0;

                // Only initialize assembly-specific paths for project-based deployment (not output-based)
                if (String.IsNullOrWhiteSpace(outputDir))
                {
                    String sourceFileName = String.IsNullOrWhiteSpace(deployTask.SourceFileName)
                        ? deployTask.TargetName + ".dll"
                        : deployTask.SourceFileName;

                    _destinationFileName = String.IsNullOrWhiteSpace(deployTask.DestinationFileName)
                        ? sourceFileName
                        : deployTask.DestinationFileName;

                    _sourcePath = Path.Combine(deployTask.TargetDir, sourceFileName);
                    //_sourcePdbPath = Path.Combine(deployTask.TargetDir, Path.GetFileNameWithoutExtension(sourceFileName)) + ".pdb";
                    //_destinationPdbFileName = Path.GetFileNameWithoutExtension(_destinationFileName) + ".pdb";
                    _sourceMdbPath = _sourcePath + ".mdb";
                    _destinationMdbFileName = _destinationFileName + ".mdb";

                    if (assemblyDirectoryX64 != null)
                    {
                        _targetPathX64 = Path.Combine(assemblyDirectoryX64, _destinationFileName);
                        estimatedTaskCount += EstimatedTaskCountPerPlatform;
                    }

                    if (assemblyDirectoryX86 != null)
                    {
                        _targetPathX86 = Path.Combine(assemblyDirectoryX86, _destinationFileName);
                        estimatedTaskCount += EstimatedTaskCountPerPlatform;
                    }
                }
                else
                {
                    // For output-based deployment, these are not used
                    _destinationFileName = null;
                    _sourcePath = null;
                    _sourceMdbPath = null;
                    _destinationMdbFileName = null;
                }

                _rootDirectory = rootDirectory;
                _hasX64 = hasX64;
                _hasX86 = hasX86;
                _outputDir = outputDir;
                _tasks = new List<Task>(estimatedTaskCount);
            }

            public static FileCopier TryGetOutputFileCopier(Deploy deployTask)
            {
                if (String.IsNullOrWhiteSpace(deployTask.OutputDir) || !Directory.Exists(deployTask.OutputDir))
                {
                    deployTask._log.LogWarning("Cannot find the output directory. Deployment skipped.");
                    return null;
                }

                return TryCreate(deployTask, deployTask.OutputDir);
            }

            public static FileCopier TryGetFileCopier(Deploy deployTask)
            {
                return TryCreate(deployTask, null);
            }

            private static FileCopier TryCreate(Deploy deployTask, String outputDir)
            {
                try
                {
                    GameLocationInfo gameLocation = null;

                    // Try explicit GamePath first if provided
                    if (!String.IsNullOrWhiteSpace(deployTask.GamePath))
                    {
                        if (!Directory.Exists(deployTask.GamePath))
                        {
                            LogWarning(deployTask, $"Cannot find the game folder at specified path: {deployTask.GamePath}");
                            return null;
                        }
                        gameLocation = new GameLocationInfo(deployTask.GamePath);
                    }
                    else
                    {
                        // Automatically discover from Steam registry (tries 64-bit then 32-bit)
                        gameLocation = GameLocationSteamRegistryProvider.TryLoad();
                    }

                    if (gameLocation == null)
                    {
                        LogWarning(deployTask, "Game folder not found. Install the game via Steam or set MemoriaGamePath build property.");
                        return null;
                    }

                    if (deployTask.DeployToRootDirectory)
                    {
                        if (!Directory.Exists(gameLocation.RootDirectory))
                        {
                            LogWarning(deployTask, $"Cannot find the game root directory [{gameLocation.RootDirectory}].");
                            return null;
                        }

                        return new FileCopier(deployTask, gameLocation.RootDirectory, null, gameLocation.RootDirectory, Directory.Exists(gameLocation.ManagedPathX64), Directory.Exists(gameLocation.ManagedPathX86), outputDir);
                    }

                    Boolean isX64Exists = Directory.Exists(gameLocation.ManagedPathX64);
                    Boolean isX86Exists = Directory.Exists(gameLocation.ManagedPathX86);

                    if (isX64Exists)
                    {
                        return isX86Exists
                            ? new FileCopier(deployTask, gameLocation.ManagedPathX64, gameLocation.ManagedPathX86, gameLocation.RootDirectory, isX64Exists, isX86Exists, outputDir)
                            : new FileCopier(deployTask, gameLocation.ManagedPathX64, null, gameLocation.RootDirectory, isX64Exists, isX86Exists, outputDir);
                    }
                    if (isX86Exists)
                    {
                        return new FileCopier(deployTask, null, gameLocation.ManagedPathX86, gameLocation.RootDirectory, isX64Exists, isX86Exists, outputDir);
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
                String destinationFileName = String.IsNullOrWhiteSpace(deployTask.DestinationFileName)
                    ? (String.IsNullOrWhiteSpace(deployTask.SourceFileName) ? deployTask.TargetName + ".dll" : deployTask.SourceFileName)
                    : deployTask.DestinationFileName;
                deployTask._log.LogWarning($"{warning} {destinationFileName} will not be deployed automatically.");
            }

            public void BeginCopyMainFile()
            {
                BeginCopy(_sourcePath, _destinationFileName, logSuccess: true);
            }

            public void BeginCopyPdb()
            {
                // We do not need .pdb files. Only .mdb will use for debug.
                //BeginCopy(_sourcePdbPath, _destinationPdbFileName, logSuccess: true);
            }

            public void BeginCopyMdb()
            {
                BeginCopy(_sourceMdbPath, _destinationMdbFileName, logSuccess: true);
            }

            public void BeginCopySharedFiles()
            {
                if (String.IsNullOrWhiteSpace(_rootDirectory))
                    return;

                foreach (DeploymentFileDefinition file in DeploymentFileSet.Files)
                {
                    if (file.Kind != DeploymentItemKind.File)
                        continue;

                    String sourceItem = Path.Combine(_deployTask.TargetDir, file.SourceRelativePath);
                    if (!File.Exists(sourceItem))
                        continue;

                    QueuePlatformCopies(sourceItem, file.TargetRelativePath);
                }
            }

            public void BeginCopyDeploymentSet()
            {
                if (String.IsNullOrWhiteSpace(_outputDir) || String.IsNullOrWhiteSpace(_rootDirectory))
                    return;

                foreach (DeploymentFileDefinition file in DeploymentFileSet.Files)
                {
                    switch (file.Kind)
                    {
                        case DeploymentItemKind.Folder:
                            BeginCopyFolder(file.SourceRelativePath, file.TargetRelativePath);
                            break;
                        case DeploymentItemKind.ManagedDll:
                            BeginCopyManagedDll(file.SourceRelativePath, file.TargetRelativePath);
                            break;
                        case DeploymentItemKind.File:
                            BeginCopyOutputFile(file.SourceRelativePath, file.TargetRelativePath);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            private void BeginCopyFolder(String sourceRelativePath, String destinationRelativePath)
            {
                String sourceDirectory = Path.Combine(_outputDir, sourceRelativePath);
                if (!Directory.Exists(sourceDirectory))
                    return;

                foreach (String sourceFile in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
                {
                    String relativePath = sourceFile.Substring(sourceDirectory.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    QueueCopy(sourceFile, Path.Combine(_rootDirectory, destinationRelativePath, relativePath), logSuccess: true);
                }
            }

            private void BeginCopyManagedDll(String sourceRelativePath, String destinationRelativePath)
            {
                String dllPath = Path.Combine(_outputDir, sourceRelativePath);
                if (!File.Exists(dllPath))
                    return;

                String pdbPath = Path.ChangeExtension(dllPath, ".pdb");
                if (File.Exists(pdbPath))
                    MdbGenerator.Generate(_deployTask, dllPath);

                QueuePlatformCopies(dllPath, destinationRelativePath);

                String mdbPath = dllPath + ".mdb";
                if (File.Exists(mdbPath))
                    QueuePlatformCopies(mdbPath, destinationRelativePath + ".mdb");
            }

            private void BeginCopyOutputFile(String sourceRelativePath, String destinationRelativePath)
            {
                String sourceItem = Path.Combine(_outputDir, sourceRelativePath);
                if (!File.Exists(sourceItem))
                    return;

                QueuePlatformCopies(sourceItem, destinationRelativePath);
            }

            private void QueuePlatformCopies(String sourceItem, String destinationRelativePath)
            {
                if (destinationRelativePath.Contains("{PLATFORM}"))
                {
                    if (_hasX64)
                        QueueCopy(sourceItem, Path.Combine(_rootDirectory, destinationRelativePath.Replace("{PLATFORM}", "x64")), logSuccess: true);

                    if (_hasX86)
                        QueueCopy(sourceItem, Path.Combine(_rootDirectory, destinationRelativePath.Replace("{PLATFORM}", "x86")), logSuccess: true);

                    return;
                }

                if (destinationRelativePath.StartsWith("x64\\", StringComparison.OrdinalIgnoreCase))
                {
                    if (_hasX64)
                        QueueCopy(sourceItem, Path.Combine(_rootDirectory, destinationRelativePath), logSuccess: true);

                    return;
                }

                if (destinationRelativePath.StartsWith("x86\\", StringComparison.OrdinalIgnoreCase))
                {
                    if (_hasX86)
                        QueueCopy(sourceItem, Path.Combine(_rootDirectory, destinationRelativePath), logSuccess: true);

                    return;
                }

                QueueCopy(sourceItem, Path.Combine(_rootDirectory, destinationRelativePath), logSuccess: true);
            }

            private void BeginCopy(String sourceItem, String destinationFileName, Boolean logSuccess)
            {
                if (_targetPathX64 != null)
                {
                    String targetItem = Path.Combine(Path.GetDirectoryName(_targetPathX64), destinationFileName);
                    QueueCopy(sourceItem, targetItem, logSuccess);
                }

                if (_targetPathX86 != null)
                {
                    String targetItem = Path.Combine(Path.GetDirectoryName(_targetPathX86), destinationFileName);
                    QueueCopy(sourceItem, targetItem, logSuccess);
                }
            }

            private void QueueCopy(String sourceItem, String targetItem, Boolean logSuccess)
            {
                String source = sourceItem;
                String target = targetItem;
                _tasks.Add(Task.Factory.StartNew(() => CopyFile(source, target, logSuccess)));
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
                    String targetDirectory = Path.GetDirectoryName(targetItem);
                    if (!String.IsNullOrWhiteSpace(targetDirectory))
                        Directory.CreateDirectory(targetDirectory);

                    File.Copy(sourceItem, targetItem, true);
                    if (logSuccess)
                        _deployTask._log.LogMessage(MessageImportance.High, "{0}Deployed [{1}]: {2}{0}", Environment.NewLine, Path.GetFileName(targetItem), targetItem);
                }
                catch (Exception ex)
                {
                    _deployTask._log.LogWarning(Environment.NewLine + "Cannot deploy {0} to the game." + Environment.NewLine, Path.GetFileName(targetItem));
                    _deployTask._log.LogWarningFromException(ex, showStackTrace: false);
                }
            }
        }
    }
}
