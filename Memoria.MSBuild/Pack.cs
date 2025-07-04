//#define NEEDS_SIGNING

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

        private List<String> signPaths = new List<String>();
        private class PackFileOperationArgs
        {
            public string File;
            public string TargetRelativePath;
            public GZipStream Output;
            public BinaryWriter Bw;
            public Dictionary<string, ushort> PathMap;
        }
        private List<PackFileOperationArgs> packFileOperations = new List<PackFileOperationArgs>();

        private const string SignatureThumbprint = "316b51aca09ee3b93d0b9a75a48ecee278491ce2";

        public Boolean Execute()
        {
            if (BuildEnvironment.IsDebug)
                Debugger.Launch();

            Stopwatch sw = Stopwatch.StartNew();
            using (FileStream executableFile = File.OpenWrite(TargetPath))
            {
                Int64 compressedDataPosition = 0;
                Int64 uncompressedDataSize = 0;

                using (GZipStream compressStream = new GZipStream(executableFile, CompressionMode.Compress, true))
                using (BinaryWriter bw = new BinaryWriter(compressStream))
                {
                    executableFile.Seek(0, SeekOrigin.End);
                    compressedDataPosition = executableFile.Position;

                    Dictionary<String, UInt16> pathMap = new Dictionary<String, UInt16>(capacity: 400);
                    AddPackFolder("StreamingAssets", "StreamingAssets", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackFolder("FF9_Data", "FF9_Data", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackFolder("Debugger", "Debugger", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackDLLs("", "{PLATFORM}\\FF9_Data\\Managed", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Launcher\\Memoria.Launcher.exe", "FF9_Launcher.exe", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Launcher\\Memoria.Launcher.exe.config", "FF9_Launcher.exe.config", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Launcher\\Memoria.SteamFix.exe", "Memoria.SteamFix.exe", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Launcher\\Memoria.ini", "Memoria.ini", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Launcher\\Settings.ini", "Settings.ini", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("XInputDotNetPure.dll", "{PLATFORM}\\FF9_Data\\Managed\\XInputDotNetPure.dll", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Newtonsoft.Json.dll", "{PLATFORM}\\FF9_Data\\Managed\\Newtonsoft.Json.dll", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("System.Runtime.Serialization.dll", "{PLATFORM}\\FF9_Data\\Managed\\System.Runtime.Serialization.dll", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("JoyShockLibrary\\x64\\JoyShockLibrary.dll", "x64\\FF9_Data\\Plugins\\JoyShockLibrary.dll", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("JoyShockLibrary\\x86\\JoyShockLibrary.dll", "x86\\FF9_Data\\Plugins\\JoyShockLibrary.dll", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Global\\Sound\\SoLoud\\x64\\soloud.dll", "x64\\FF9_Data\\Plugins\\soloud.dll", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Global\\Sound\\SoLoud\\x86\\soloud.dll", "x86\\FF9_Data\\Plugins\\soloud.dll", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Global\\Sound\\SaXAudio\\x64\\SaXAudio.dll", "x64\\FF9_Data\\Plugins\\SaXAudio.dll", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Global\\Sound\\SaXAudio\\x86\\SaXAudio.dll", "x86\\FF9_Data\\Plugins\\SaXAudio.dll", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Global\\Sound\\SaXAudio\\x64\\XAudio2_9.dll", "x64\\XAudio2_9.dll", compressStream, bw, pathMap, ref uncompressedDataSize);
                    AddPackOptionalFile("Global\\Sound\\SaXAudio\\x86\\XAudio2_9.dll", "x86\\XAudio2_9.dll", compressStream, bw, pathMap, ref uncompressedDataSize);
#if NEEDS_SIGNING
                    StartSigning();
#endif
                    StartPacking(ref uncompressedDataSize);
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
#if NEEDS_SIGNING
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "signtool.exe",
                    Arguments = "sign /d \"Memoria Patcher for Modding FF9\" /td SHA256 /fd SHA256 /sha1 "+SignatureThumbprint+" /tr http://timestamp.digicert.com "+TargetPath,
                    UseShellExecute = true,
                    CreateNoWindow = false
                },
                EnableRaisingEvents = true,
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
            process.Exited += (sender, e) =>
            {
                if (process.ExitCode != 0)
                    _log.LogError("signtool.exe failed with exit code " + process.ExitCode);
                else
                    _log.LogMessage("signtool.exe completed successfully.");
            };
            process.Start();
            process.WaitForExit();
#endif
            return true;
        }

        private void AddPackOptionalFile(String sourceFileRelativePath, String targetFileRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {
            String sourceFilePath = Path.GetFullPath(Path.Combine(TargetDir, sourceFileRelativePath));
            FileInfo sourceFile = new FileInfo(sourceFilePath);
            if (!sourceFile.Exists)
                return;

            PrepairPackFile(sourceFile.FullName, targetFileRelativePath, output, bw, pathMap, ref uncompressedDataSize);
        }

        private void AddPackDLLs(String sourceFolderRelativePath, String targetFolderRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {
            String sourceDirectoryPath = Path.GetFullPath(Path.Combine(TargetDir, sourceFolderRelativePath));
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDirectoryPath);
            Int32 sourceDirectoryPathLength = GetSourceDirectoryPathLength(sourceDirectoryPath);
            foreach (FileInfo mdbFile in sourceDirectory.EnumerateFiles("*.dll.mdb", SearchOption.TopDirectoryOnly))
            {
                FileInfo dllFile = new FileInfo(Path.ChangeExtension(mdbFile.FullName, null));
                if (dllFile.Exists)
                {
                    AddPackFile(dllFile.FullName, sourceDirectoryPathLength, targetFolderRelativePath, output, bw, pathMap, ref uncompressedDataSize);
                    AddPackFile(mdbFile.FullName, sourceDirectoryPathLength, targetFolderRelativePath, output, bw, pathMap, ref uncompressedDataSize);
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

        private void AddPackFolder(String sourceFolderRelativePath, String targetFolderRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {
            String sourceDirectoryPath = Path.GetFullPath(Path.Combine(TargetDir, sourceFolderRelativePath));
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDirectoryPath);
            Int32 sourceDirectoryPathLength = GetSourceDirectoryPathLength(sourceDirectoryPath);
            foreach (FileInfo file in sourceDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                AddPackFile(file.FullName, sourceDirectoryPathLength, targetFolderRelativePath, output, bw, pathMap, ref uncompressedDataSize);
            }
        }

        private void AddPackFile(String file, Int32 sourceDirectoryPathLength, String targetFolderRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {

            String sourceRelativePath = file.Substring(sourceDirectoryPathLength);
            String targetRelativePath = Path.Combine(targetFolderRelativePath, sourceRelativePath);
            PrepairPackFile(file, targetRelativePath, output, bw, pathMap, ref uncompressedDataSize);
        }

        private void PrepairPackFile(string file, String targetRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {
            FileInfo fileInfo = new FileInfo(file);
            if (
                (
                    fileInfo.Extension.Equals(".exe", StringComparison.OrdinalIgnoreCase)
                ||
                    fileInfo.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase)
                ) &&
                !fileInfo.Name.Contains("Microsoft.") &&
                !fileInfo.Name.Contains("System.")
            )
            {
                signPaths.Add(fileInfo.FullName);
            }

            packFileOperations.Add(new PackFileOperationArgs
            {
                File = file,
                TargetRelativePath = targetRelativePath,
                Output = output,
                Bw = bw,
                PathMap = pathMap
            });
        }

        private void PackFile(string filePath, String targetRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {
            FileInfo file = new FileInfo(filePath);
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
            List<string> arguments = ("sign /d \"Memoria Patcher for Modding FF9\" /td SHA256 /fd SHA256 /sha1 "+SignatureThumbprint+" /tr http://timestamp.digicert.com").Split(' ').ToList();
            foreach (String path in signPaths)
            {
                arguments.Add(path);
            }
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "signtool.exe",
                    Arguments = String.Join(" ", arguments),
                    UseShellExecute = true,
                    CreateNoWindow = false
                },
                EnableRaisingEvents = true,
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

        private void StartPacking(ref Int64 uncompressedDataSize)
        {
            foreach (var args in packFileOperations)
            {
                PackFile(args.File, args.TargetRelativePath, args.Output, args.Bw, args.PathMap, ref uncompressedDataSize);
            }
        }
    }
}
