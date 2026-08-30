#nullable enable

using System;
using System.IO;
using System.Net.Http;

namespace Memoria.Launcher.Utils.Downloads
{
    internal abstract class DownloadDestination
    {
        private DownloadDestination(String directoryPath, String displayPath)
        {
            DirectoryPath = directoryPath;
            DisplayPath = displayPath;
        }

        public String DirectoryPath { get; }
        public String DisplayPath { get; }

        public abstract String ResolveFilePath(HttpResponseMessage response, Uri source);

        public static DownloadDestination ForFile(String destinationPath)
        {
            String fullPath = GetFullPath(destinationPath, nameof(destinationPath), "file");
            String? directoryPath = Path.GetDirectoryName(fullPath);
            if (String.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("The destination file must have a parent directory.", nameof(destinationPath));

            return new ExactFileDestination(fullPath, directoryPath!);
        }

        public static DownloadDestination InDirectory(String destinationDirectory)
        {
            String fullPath = GetFullPath(destinationDirectory, nameof(destinationDirectory), "directory");
            return new RemoteFileDestination(NormalizeDirectoryPath(fullPath));
        }

        private static String GetFullPath(String path, String parameterName, String pathKind)
        {
            if (path == null)
                throw new ArgumentNullException(parameterName);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException($"The destination {pathKind} cannot be empty or whitespace.", parameterName);

            try
            {
                return Path.GetFullPath(path);
            }
            catch (Exception exception) when (exception is ArgumentException || exception is NotSupportedException || exception is PathTooLongException)
            {
                throw new ArgumentException($"The destination is not a valid {pathKind} path.", parameterName, exception);
            }
        }

        private static String NormalizeDirectoryPath(String fullPath)
        {
            String? root = Path.GetPathRoot(fullPath);
            if (String.Equals(fullPath, root, StringComparison.OrdinalIgnoreCase))
                return fullPath;

            return fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        private sealed class ExactFileDestination : DownloadDestination
        {
            private readonly String _filePath;

            public ExactFileDestination(String filePath, String directoryPath)
                : base(directoryPath, filePath)
            {
                _filePath = filePath;
            }

            public override String ResolveFilePath(HttpResponseMessage response, Uri source)
            {
                if (response == null)
                    throw new ArgumentNullException(nameof(response));
                if (source == null)
                    throw new ArgumentNullException(nameof(source));

                return _filePath;
            }
        }

        private sealed class RemoteFileDestination : DownloadDestination
        {
            public RemoteFileDestination(String directoryPath)
                : base(directoryPath, directoryPath)
            {
            }

            public override String ResolveFilePath(HttpResponseMessage response, Uri source)
            {
                String remoteFileName = RemoteFileNameResolver.Resolve(response, source);
                String destinationPath = Path.GetFullPath(Path.Combine(DirectoryPath, remoteFileName));
                String? parentDirectory = Path.GetDirectoryName(destinationPath);
                if (!String.Equals(parentDirectory, DirectoryPath, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("The remote file name resolves outside the destination directory.");

                return destinationPath;
            }
        }
    }
}
