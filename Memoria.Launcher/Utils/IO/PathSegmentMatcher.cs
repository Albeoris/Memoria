using System;
using System.IO;
using System.Linq;

namespace Memoria.Launcher.Utils.IO
{
    internal static class PathSegmentMatcher
    {
        public static Boolean Contains(String path, StringComparison comparison, params String[] segments)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));
            if (segments.Length == 0)
                throw new ArgumentException("At least one path segment is required.", nameof(segments));
            if (segments.Any(segment => String.IsNullOrEmpty(segment) || segment.IndexOf(Path.DirectorySeparatorChar) >= 0 || segment.IndexOf(Path.AltDirectorySeparatorChar) >= 0))
                throw new ArgumentException("Path segments must be non-empty names without directory separators.", nameof(segments));

            String normalizedPath = Path.DirectorySeparatorChar + path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).Trim(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            String sequence = Path.DirectorySeparatorChar + String.Join(Path.DirectorySeparatorChar.ToString(), segments) + Path.DirectorySeparatorChar;
            return normalizedPath.IndexOf(sequence, comparison) >= 0;
        }
    }
}
