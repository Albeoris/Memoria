using System;
using Memoria.Launcher.Utils.IO;

namespace Memoria.Launcher.Utils.Mods
{
    internal sealed class ModArchiveRoot
    {
        private ModArchiveRoot(String relativePath)
        {
            RelativePath = relativePath;
        }

        public static ModArchiveRoot ExtractionDirectory { get; } = new ModArchiveRoot(String.Empty);

        public String RelativePath { get; }
        public Boolean IsExtractionDirectory => RelativePath.Length == 0;

        public static ModArchiveRoot FromRelativePath(String relativePath)
        {
            SafeRelativePath safePath = SafeRelativePath.Parse(relativePath, nameof(relativePath));
            return new ModArchiveRoot(safePath.Value);
        }
    }
}
