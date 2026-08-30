using System;
using System.Collections.Generic;
using System.IO;

namespace Memoria.Launcher.Utils.IO
{
    internal sealed class GameRootDirectory
    {
        private readonly String _rootPrefix;

        public GameRootDirectory(String rootPath)
        {
            if (rootPath == null)
                throw new ArgumentNullException(nameof(rootPath));
            if (String.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("The game root cannot be empty or whitespace.", nameof(rootPath));
            EnsureNoRelativeTransitions(rootPath, nameof(rootPath));

            RootPath = Path.GetFullPath(rootPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (!Path.IsPathRooted(RootPath))
                throw new ArgumentException("The game root must be an absolute path.", nameof(rootPath));

            _rootPrefix = RootPath + Path.DirectorySeparatorChar;
        }

        public String RootPath { get; }

        public String Resolve(SafeRelativePath relativePath)
        {
            if (relativePath == null)
                throw new ArgumentNullException(nameof(relativePath));

            String fullPath = Path.GetFullPath(Path.Combine(RootPath, relativePath.Value));
            EnsureContained(fullPath, allowRoot: false);
            EnsureExistingAncestorsAreNotReparsePoints(fullPath);
            return fullPath;
        }

        public String ResolveWithin(String basePath, SafeRelativePath relativePath)
        {
            if (basePath == null)
                throw new ArgumentNullException(nameof(basePath));
            if (relativePath == null)
                throw new ArgumentNullException(nameof(relativePath));

            String fullBasePath = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            EnsureContained(fullBasePath, allowRoot: true);

            String fullPath = Path.GetFullPath(Path.Combine(fullBasePath, relativePath.Value));
            String basePrefix = fullBasePath + Path.DirectorySeparatorChar;
            if (!fullPath.StartsWith(basePrefix, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The resolved path escapes its allowed directory.", nameof(relativePath));

            EnsureContained(fullPath, allowRoot: false);
            EnsureExistingAncestorsAreNotReparsePoints(fullPath);
            return fullPath;
        }

        public void EnsureContained(String path, Boolean allowRoot = false)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("The path cannot be empty or whitespace.", nameof(path));
            EnsureNoRelativeTransitions(path, nameof(path));

            String fullPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            Boolean isRoot = fullPath.Equals(RootPath, StringComparison.OrdinalIgnoreCase);
            if ((!allowRoot && isRoot) || (!isRoot && !fullPath.StartsWith(_rootPrefix, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"The path '{path}' is outside the game directory '{RootPath}'.", nameof(path));

            EnsureExistingAncestorsAreNotReparsePoints(fullPath);
        }

        public void EnsureSafeTree(String directoryPath)
        {
            EnsureContained(directoryPath);
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");

            Stack<String> pendingDirectories = new();
            pendingDirectories.Push(Path.GetFullPath(directoryPath));
            while (pendingDirectories.Count > 0)
            {
                String currentDirectory = pendingDirectories.Pop();
                foreach (String entryPath in Directory.EnumerateFileSystemEntries(currentDirectory))
                {
                    EnsureContained(entryPath);
                    FileAttributes attributes = File.GetAttributes(entryPath);
                    if ((attributes & FileAttributes.ReparsePoint) != 0)
                        throw new IOException($"The extracted mod contains a symbolic link or reparse point: '{entryPath}'.");
                    if ((attributes & FileAttributes.Directory) != 0)
                        pendingDirectories.Push(entryPath);
                }
            }
        }

        private void EnsureExistingAncestorsAreNotReparsePoints(String fullPath)
        {
            String relativePart = fullPath.Equals(RootPath, StringComparison.OrdinalIgnoreCase)
                ? String.Empty
                : fullPath.Substring(_rootPrefix.Length);
            String currentPath = RootPath;

            foreach (String segment in relativePart.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
            {
                currentPath = Path.Combine(currentPath, segment);
                if (!File.Exists(currentPath) && !Directory.Exists(currentPath))
                    continue;

                FileAttributes attributes = File.GetAttributes(currentPath);
                if ((attributes & FileAttributes.ReparsePoint) != 0)
                    throw new IOException($"The path crosses a symbolic link or reparse point: '{currentPath}'.");
            }
        }

        private static void EnsureNoRelativeTransitions(String path, String parameterName)
        {
            String normalizedPath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            foreach (String segment in normalizedPath.Split(Path.DirectorySeparatorChar))
            {
                if (segment == "." || segment == "..")
                    throw new ArgumentException("Relative path transitions '.' and '..' are not allowed.", parameterName);
            }
        }
    }
}
