using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

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
                    PackFolder("StreamingAssets", "StreamingAssets", compressStream, bw, pathMap, ref uncompressedDataSize);
                    PackFolder("Debugger", "Debugger", compressStream, bw, pathMap, ref uncompressedDataSize);
                    PackDLLs("", "{PLATFORM}\\FF9_Data\\Managed", compressStream, bw, pathMap, ref uncompressedDataSize);
                    PackOptionalFile("Launcher\\Memoria.Launcher.exe", "FF9_Launcher.exe", compressStream, bw, pathMap, ref uncompressedDataSize);
                    PackOptionalFile("Launcher\\Memoria.Launcher.exe.config", "FF9_Launcher.exe.config", compressStream, bw, pathMap, ref uncompressedDataSize);
                    PackOptionalFile("Launcher\\Memoria.SteamFix.exe", "Memoria.SteamFix.exe", compressStream, bw, pathMap, ref uncompressedDataSize);
                    PackOptionalFile("Launcher\\Memoria.ini", "Memoria.ini", compressStream, bw, pathMap, ref uncompressedDataSize);


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

        private void PackOptionalFile(String sourceFileRelativePath, String targetFileRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {
            String sourceFilePath = Path.GetFullPath(Path.Combine(TargetDir, sourceFileRelativePath));
            FileInfo sourceFile = new FileInfo(sourceFilePath);
            if (!sourceFile.Exists)
                return;

            PackFile(sourceFile, targetFileRelativePath, output, bw, pathMap, ref uncompressedDataSize);
        }

        private void PackDLLs(String sourceFolderRelativePath, String targetFolderRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {
            String sourceDirectoryPath = Path.GetFullPath(Path.Combine(TargetDir, sourceFolderRelativePath));
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDirectoryPath);
            Int32 sourceDirectoryPathLength = GetSourceDirectoryPathLength(sourceDirectoryPath);
            foreach (FileInfo mdbFile in sourceDirectory.EnumerateFiles("*.dll.mdb", SearchOption.TopDirectoryOnly))
            {
                FileInfo dllFile = new FileInfo(Path.ChangeExtension(mdbFile.FullName, null));
                if (dllFile.Exists)
                {
                    PackFile(dllFile, sourceDirectoryPathLength, targetFolderRelativePath, output, bw, pathMap, ref uncompressedDataSize);
                    PackFile(mdbFile, sourceDirectoryPathLength, targetFolderRelativePath, output, bw, pathMap, ref uncompressedDataSize);
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

        private void PackFolder(String sourceFolderRelativePath, String targetFolderRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {
            String sourceDirectoryPath = Path.GetFullPath(Path.Combine(TargetDir, sourceFolderRelativePath));
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDirectoryPath);
            Int32 sourceDirectoryPathLength = GetSourceDirectoryPathLength(sourceDirectoryPath);
            foreach (FileInfo file in sourceDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                PackFile(file, sourceDirectoryPathLength, targetFolderRelativePath, output, bw, pathMap, ref uncompressedDataSize);
            }
        }

        private void PackFile(FileInfo file, Int32 sourceDirectoryPathLength, String targetFolderRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
        {
            String sourceRelativePath = file.FullName.Substring(sourceDirectoryPathLength);
            String targetRelativePath = Path.Combine(targetFolderRelativePath, sourceRelativePath);
            PackFile(file, targetRelativePath, output, bw, pathMap, ref uncompressedDataSize);
        }

        private void PackFile(FileInfo file, String targetRelativePath, GZipStream output, BinaryWriter bw, Dictionary<String, UInt16> pathMap, ref Int64 uncompressedDataSize)
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

            _log.LogMessage(targetRelativePath);
        }
    }
}