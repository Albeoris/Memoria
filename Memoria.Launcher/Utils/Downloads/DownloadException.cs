using System;
using System.IO;

namespace Memoria.Launcher.Utils.Downloads
{
    public sealed class DownloadException : IOException
    {
        public DownloadException(DownloadFailureKind kind, String message)
            : base(message)
        {
            Kind = kind;
        }

        public DownloadException(DownloadFailureKind kind, String message, Exception innerException)
            : base(message, innerException)
        {
            Kind = kind;
        }

        public DownloadFailureKind Kind { get; }
    }
}
