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
        private class PackedFileLogEntry
        {
            public string TargetRelativePath;
            public DateTime LastWriteTimeUtc;
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
                    foreach (DeploymentFileDefinition file in DeploymentFileSet.Files)
                        AddPackDefinition(file, compressStream, bw, pathMap, ref uncompressedDataSize);
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

        private void AddPackDefinition(DeploymentFileDefinition file, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {
            switch (file.Kind)
            {
                case DeploymentItemKind.Folder:
                    AddPackFolder(file.SourceRelativePath, file.TargetRelativePath, output, bw, pathMap, ref uncompressedDataSize);
                    break;
                case DeploymentItemKind.ManagedDll:
                    AddPackManagedDll(file.SourceRelativePath, file.TargetRelativePath, output, bw, pathMap, ref uncompressedDataSize);
                    break;
                case DeploymentItemKind.File:
                case DeploymentItemKind.InitialFile:
                    AddPackOptionalFile(file.SourceRelativePath, file.TargetRelativePath, output, bw, pathMap, ref uncompressedDataSize);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddPackManagedDll(String sourceFileRelativePath, String targetFileRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {
            AddPackOptionalFile(sourceFileRelativePath, targetFileRelativePath, output, bw, pathMap, ref uncompressedDataSize);
            AddPackOptionalFile(sourceFileRelativePath + ".mdb", targetFileRelativePath + ".mdb", output, bw, pathMap, ref uncompressedDataSize);
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

        private DateTime PackFile(string filePath, String targetRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
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
            return file.LastWriteTimeUtc;
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
            List<PackedFileLogEntry> packedFiles = new List<PackedFileLogEntry>(packFileOperations.Count);
            foreach (var args in packFileOperations)
            {
                try
                {
                    DateTime lastWriteTimeUtc = PackFile(args.File, args.TargetRelativePath, args.Output, args.Bw, args.PathMap, ref uncompressedDataSize);
                    packedFiles.Add(new PackedFileLogEntry
                    {
                        TargetRelativePath = args.TargetRelativePath,
                        LastWriteTimeUtc = lastWriteTimeUtc
                    });
                }
                catch (Exception ex)
                {
                    _log.LogError("Failed to pack file [{0}] as [{1}]: {2}", args.File, args.TargetRelativePath, ex.Message);
                    throw;
                }
            }

            LogPackedFiles(packedFiles);
        }

        private void LogPackedFiles(List<PackedFileLogEntry> packedFiles)
        {
            Dictionary<String, PackedFileLogEntry> filesByPath = new Dictionary<String, PackedFileLogEntry>(StringComparer.OrdinalIgnoreCase);
            foreach (PackedFileLogEntry entry in packedFiles)
            {
                if (!filesByPath.TryGetValue(entry.TargetRelativePath, out PackedFileLogEntry existingEntry) || entry.LastWriteTimeUtc < existingEntry.LastWriteTimeUtc)
                    filesByPath[entry.TargetRelativePath] = entry;
            }

            HashSet<String> loggedPaths = new HashSet<String>(StringComparer.OrdinalIgnoreCase);

            foreach (PackedFileLogEntry entry in packedFiles)
            {
                if (!loggedPaths.Add(entry.TargetRelativePath))
                    continue;

                PackedFileLogEntry loggedEntry = filesByPath[entry.TargetRelativePath];
                String displayedPath = loggedEntry.TargetRelativePath;
                DateTime displayedTime = loggedEntry.LastWriteTimeUtc;
                if (TryGetPlatformPath(entry.TargetRelativePath, out String platform, out String relativePath))
                {
                    String otherPlatform = platform.Equals("x64", StringComparison.OrdinalIgnoreCase) ? "x86" : "x64";
                    String otherPath = Path.Combine(otherPlatform, relativePath);
                    if (filesByPath.TryGetValue(otherPath, out PackedFileLogEntry otherEntry))
                    {
                        loggedPaths.Add(otherPath);
                        displayedPath = "(x64|x86)" + Path.DirectorySeparatorChar + relativePath;
                        if (otherEntry.LastWriteTimeUtc < displayedTime)
                            displayedTime = otherEntry.LastWriteTimeUtc;
                    }
                }

                _log.LogMessage(MessageImportance.Low, "{0}  ({1:yyyy-MM-dd HH:mm:ss})", displayedPath, displayedTime);
            }
        }

        private static Boolean TryGetPlatformPath(String path, out String platform, out String relativePath)
        {
            Int32 separatorIndex = path.IndexOf(Path.DirectorySeparatorChar);
            if (separatorIndex > 0)
            {
                platform = path.Substring(0, separatorIndex);
                if (platform.Equals("x64", StringComparison.OrdinalIgnoreCase) || platform.Equals("x86", StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = path.Substring(separatorIndex + 1);
                    return true;
                }
            }

            platform = null;
            relativePath = null;
            return false;
        }
    }
}
