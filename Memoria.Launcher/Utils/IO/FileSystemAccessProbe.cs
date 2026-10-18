using System;
using System.IO;

namespace Memoria.Launcher.Utils.IO
{
    internal static class FileSystemAccessProbe
    {
        public static void EnsureReadableFile(String filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The requested file does not exist.", filePath);

            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public static void EnsureWritableDirectory(String directoryPath)
        {
            Directory.CreateDirectory(directoryPath);

            String probePath = Path.Combine(directoryPath, $".memoria-write-test-{Guid.NewGuid():N}.tmp");
            using FileStream stream = new FileStream(
                probePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 1,
                FileOptions.DeleteOnClose);
        }

        public static void EnsureReplaceableFile(String filePath)
        {
            if (!File.Exists(filePath))
                return;

            FileAttributes attributes = File.GetAttributes(filePath);
            if ((attributes & FileAttributes.ReadOnly) != 0)
                throw new UnauthorizedAccessException($"The file '{filePath}' is read-only.");

            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.Read);
        }
    }
}
