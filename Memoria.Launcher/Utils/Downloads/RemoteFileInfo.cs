using System;

namespace Memoria.Launcher.Utils.Downloads
{
    internal sealed class RemoteFileInfo
    {
        public RemoteFileInfo(Uri source, Int64 contentLength, DateTime lastModified)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            if (!source.IsAbsoluteUri)
                throw new ArgumentException("The source URI must be absolute.", nameof(source));
            if (contentLength < -1)
                throw new ArgumentOutOfRangeException(nameof(contentLength));

            ContentLength = contentLength;
            LastModified = lastModified;
        }

        public Uri Source { get; }
        public Int64 ContentLength { get; }
        public DateTime LastModified { get; }
        public String TargetName { get; set; }
        public String TargetPath { get; set; }
    }
}
