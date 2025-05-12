using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Memoria.MSBuild
{
    public class Pack : ITask
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

        public Pack()
        {
            _log = new TaskLoggingHelper(this);
        }

        private Int64 compressedDataPosition = 0;
        private Int64 uncompressedDataSize = 0;
        private Queue<Action> packFileOperations = new Queue<Action>();
        private List<String> signPaths = new List<String>();

        public Boolean Execute()
        {
            if (BuildEnvironment.IsDebug)
                Debugger.Launch();

            Stopwatch sw = Stopwatch.StartNew();
            using (FileStream executableFile = File.OpenWrite(TargetPath))
            {
                using (GZipStream compressStream = new GZipStream(executableFile, CompressionMode.Compress, true))
                using (BinaryWriter bw = new BinaryWriter(compressStream))
                {
                    executableFile.Seek(0, SeekOrigin.End);
                    compressedDataPosition = executableFile.Position;

                    Dictionary<String, UInt16> pathMap = new Dictionary<String, UInt16>(capacity: 400);
                    AddPackFolder("StreamingAssets", "StreamingAssets", compressStream, bw, pathMap);
                    AddPackFolder("FF9_Data", "FF9_Data", compressStream, bw, pathMap);
                    AddPackFolder("Debugger", "Debugger", compressStream, bw, pathMap);
                    AddPackDLLs("", "{PLATFORM}\\FF9_Data\\Managed", compressStream, bw, pathMap);
                    AddPackOptionalFile("Launcher\\Memoria.Launcher.exe", "FF9_Launcher.exe", compressStream, bw, pathMap);
                    AddPackOptionalFile("Launcher\\Memoria.Launcher.exe.config", "FF9_Launcher.exe.config", compressStream, bw, pathMap);
                    AddPackOptionalFile("Launcher\\Memoria.SteamFix.exe", "Memoria.SteamFix.exe", compressStream, bw, pathMap);
                    AddPackOptionalFile("Launcher\\Memoria.ini", "Memoria.ini", compressStream, bw, pathMap);
                    AddPackOptionalFile("Launcher\\Settings.ini", "Settings.ini", compressStream, bw, pathMap);
                    AddPackOptionalFile("XInputDotNetPure.dll", "{PLATFORM}\\FF9_Data\\Managed\\XInputDotNetPure.dll", compressStream, bw, pathMap);
                    AddPackOptionalFile("Newtonsoft.Json.dll", "{PLATFORM}\\FF9_Data\\Managed\\Newtonsoft.Json.dll", compressStream, bw, pathMap);
                    AddPackOptionalFile("System.Runtime.Serialization.dll", "{PLATFORM}\\FF9_Data\\Managed\\System.Runtime.Serialization.dll", compressStream, bw, pathMap);
                    AddPackOptionalFile("JoyShockLibrary\\x64\\JoyShockLibrary.dll", "x64\\FF9_Data\\Plugins\\JoyShockLibrary.dll", compressStream, bw, pathMap);
                    AddPackOptionalFile("JoyShockLibrary\\x86\\JoyShockLibrary.dll", "x86\\FF9_Data\\Plugins\\JoyShockLibrary.dll", compressStream, bw, pathMap);
                    AddPackOptionalFile("Global\\Sound\\SoLoud\\x64\\soloud.dll", "x64\\FF9_Data\\Plugins\\soloud.dll", compressStream, bw, pathMap);
                    AddPackOptionalFile("Global\\Sound\\SoLoud\\x86\\soloud.dll", "x86\\FF9_Data\\Plugins\\soloud.dll", compressStream, bw, pathMap);

                    StartSigning();
                    StartPacking();
                    bw.Flush();
                    Int64 compressedDataSize = executableFile.Position - compressedDataPosition;
                    Double compressionRation = (Double)compressedDataSize / uncompressedDataSize;
                    sw.Stop();

                    _log.LogMessage(MessageImportance.High, "{0}Packed [{1}]:{0}Uncompressed size: {2}{0}Compressed size: {3}{0}Compression ration: {4}{0}Time: {5}{0}", Environment.NewLine, TargetName, uncompressedDataSize, compressedDataSize, compressionRation, sw.Elapsed);
                }

                using (BinaryWriter bw = new BinaryWriter(executableFile))
                {
                    bw.Write(0x004149524F4D454D); // MEMORIA\0
                    bw.Write(uncompressedDataSize);
                    bw.Write(compressedDataPosition);
                }
            }

            return true;
        }

        private void AddPackOptionalFile(String sourceFileRelativePath, String targetFileRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap)
        {
            String sourceFilePath = Path.GetFullPath(Path.Combine(TargetDir, sourceFileRelativePath));
            FileInfo sourceFile = new FileInfo(sourceFilePath);
            if (!sourceFile.Exists)
                return;

            PrepairPackFile(sourceFile, targetFileRelativePath, output, bw, pathMap);
        }

        private void AddPackDLLs(String sourceFolderRelativePath, String targetFolderRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap)
        {
            String sourceDirectoryPath = Path.GetFullPath(Path.Combine(TargetDir, sourceFolderRelativePath));
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDirectoryPath);
            Int32 sourceDirectoryPathLength = GetSourceDirectoryPathLength(sourceDirectoryPath);
            foreach (FileInfo mdbFile in sourceDirectory.EnumerateFiles("*.dll.mdb", SearchOption.TopDirectoryOnly))
            {
                FileInfo dllFile = new FileInfo(Path.ChangeExtension(mdbFile.FullName, null));
                if (dllFile.Exists)
                {
                    AddPackFile(dllFile, sourceDirectoryPathLength, targetFolderRelativePath, output, bw, pathMap);
                    AddPackFile(mdbFile, sourceDirectoryPathLength, targetFolderRelativePath, output, bw, pathMap);
                }
            }
        }

        private static Int32 GetSourceDirectoryPathLength(String sourceDirectoryPath)
        {
            Int32 sourceDirectoryPathLength = sourceDirectoryPath.Length;
            if (sourceDirectoryPath[sourceDirectoryPath.Length - 1] != Path.DirectorySeparatorChar)
                sourceDirectoryPathLength++;
            return sourceDirectoryPathLength;
        }

        private void AddPackFolder(String sourceFolderRelativePath, String targetFolderRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap)
        {
            String sourceDirectoryPath = Path.GetFullPath(Path.Combine(TargetDir, sourceFolderRelativePath));
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDirectoryPath);
            Int32 sourceDirectoryPathLength = GetSourceDirectoryPathLength(sourceDirectoryPath);
            foreach (FileInfo file in sourceDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                AddPackFile(file, sourceDirectoryPathLength, targetFolderRelativePath, output, bw, pathMap);
            }
        }

        private void AddPackFile(FileInfo file, Int32 sourceDirectoryPathLength, String targetFolderRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap)
        {
            String sourceRelativePath = file.FullName.Substring(sourceDirectoryPathLength);
            String targetRelativePath = Path.Combine(targetFolderRelativePath, sourceRelativePath);
            PrepairPackFile(file, targetRelativePath, output, bw, pathMap);
        }

        private void PrepairPackFile(FileInfo file, String targetRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap)
        {
            if (
                (
                    file.Extension.Equals(".exe", StringComparison.OrdinalIgnoreCase) 
                || 
                    file.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase)
                ) && 
                !file.Name.Contains("Microsoft.") && 
                !file.Name.Contains("System.")
            ){
                signPaths.Add(file.FullName);
            }
            packFileOperations.Enqueue(() =>
            {
                try
                {
                    PackFile(file, targetRelativePath, output, bw, pathMap);
                }
                catch (Exception ex)
                {
                    _log.LogErrorFromException(ex);
                }
            });
        }

        private void PackFile(FileInfo file, String targetRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap)
        {
            String[] targetPathParts = targetRelativePath.Split(Path.DirectorySeparatorChar);

            UInt32 fileSize = checked((UInt32)file.Length);
            bw.Write(fileSize);
            bw.Write(file.LastWriteTimeUtc.Ticks);
            bw.Write(checked((Byte)targetPathParts.Length));
            foreach (String part in targetPathParts)
            {
                if (pathMap.TryGetValue(part, out UInt16 code))
                {
                    bw.Write(code);
                }
                else
                {
                    code = (UInt16)pathMap.Count;
                    pathMap.Add(part, code);
                    code |= (1 << 15);
                    bw.Write(code);

                    Byte[] bytes = Encoding.UTF8.GetBytes(part);
                    bw.Write(checked((Byte)bytes.Length));
                    bw.Write(bytes);
                }
            }

            using (FileStream inputFile = File.OpenRead(file.FullName))
                inputFile.CopyTo(output);

            uncompressedDataSize += fileSize;
            _log.LogMessage(MessageImportance.High, "{0}Packing [{1}]:{0}File size: {2}{0}Last write time: {3}{0} Total size: {4}{0}", Environment.NewLine, targetRelativePath, fileSize, file.LastWriteTimeUtc, uncompressedDataSize);

            _log.LogMessage(targetRelativePath);
        }

        private void StartSigning()
        {
            List<string> arguments = "sign /d \"Memoria FF9\" /td SHA256 /fd SHA256 /sha1 316b51aca09ee3b93d0b9a75a48ecee278491ce2 /tr http://timestamp.digicert.com".Split(' ').ToList();
            foreach (String path in signPaths)
            {
                arguments.Add(path);
            }
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "signtool.exe",
                Arguments = String.Join(" ", arguments),
                UseShellExecute = true,
                CreateNoWindow = false
            };
            Process process = new Process
            {
                StartInfo = startInfo
            };
            process.OutputDataReceived += (sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                    _log.LogMessage(e.Data);
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                    _log.LogError(e.Data);
            };
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) =>
            {
                if (process.ExitCode != 0)
                    _log.LogError("signtool.exe failed with exit code " + process.ExitCode);
                else
                    _log.LogMessage("signtool.exe completed successfully.");
            };
            process.Start();
            process.WaitForExit();
        }

        private void StartPacking()
        {
            while (packFileOperations.Count > 0)
            {
                Action action = packFileOperations.Dequeue();
                action();
            }
        }
    }
}
