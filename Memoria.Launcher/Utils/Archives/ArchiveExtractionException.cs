using System;
using System.IO;

namespace Memoria.Launcher.Utils.Archives
{
    public sealed class ArchiveExtractionException : IOException
    {
        public ArchiveExtractionException(String message)
            : base(message)
        {
        }

        public ArchiveExtractionException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
