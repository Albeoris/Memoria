using System;
using System.IO;
using System.Linq;

namespace Memoria.Launcher.Utils.IO
{
    internal static class DirectoryTreeDeleter
    {
        private const FileAttributes DeleteBlockingAttributes =
            FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System;

        public static void Delete(String directoryPath)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));
            if (String.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("The directory path cannot be empty or whitespace.", nameof(directoryPath));

            String fullPath = Path.GetFullPath(directoryPath);
            if (!Directory.Exists(fullPath))
                return;

            DeleteDirectory(fullPath);
        }

        private static void DeleteDirectory(String directoryPath)
        {
            foreach (String entryPath in Directory.EnumerateFileSystemEntries(directoryPath).ToArray())
            {
                if (!File.Exists(entryPath) && !Directory.Exists(entryPath))
                    continue;

                FileAttributes attributes = File.GetAttributes(entryPath);
                if ((attributes & FileAttributes.ReparsePoint) != 0)
                {
                    DeleteReparsePoint(entryPath, attributes);
                    continue;
                }

                if ((attributes & FileAttributes.Directory) != 0)
                    DeleteDirectory(entryPath);
                else
                    DeleteFile(entryPath, attributes);
            }

            RemoveDeleteBlockingAttributes(directoryPath, File.GetAttributes(directoryPath));
            Directory.Delete(directoryPath, recursive: false);
        }

        private static void DeleteFile(String filePath, FileAttributes attributes)
        {
            RemoveDeleteBlockingAttributes(filePath, attributes);
            File.Delete(filePath);
        }

        private static void DeleteReparsePoint(String path, FileAttributes attributes)
        {
            RemoveDeleteBlockingAttributes(path, attributes);
            if ((attributes & FileAttributes.Directory) != 0)
                Directory.Delete(path, recursive: false);
            else
                File.Delete(path);
        }

        private static void RemoveDeleteBlockingAttributes(String path, FileAttributes attributes)
        {
            FileAttributes writableAttributes = attributes & ~DeleteBlockingAttributes;
            if (writableAttributes != attributes)
                File.SetAttributes(path, writableAttributes);
        }
    }
}
