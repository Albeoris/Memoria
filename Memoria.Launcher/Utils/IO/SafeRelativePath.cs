using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Memoria.Launcher.Utils.IO
{
    internal sealed class SafeRelativePath
    {
        private SafeRelativePath(String value)
        {
            Value = value;
        }

        public String Value { get; }

        public static SafeRelativePath Parse(String path, String parameterName)
        {
            if (path == null)
                throw new ArgumentNullException(parameterName);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("The relative path cannot be empty or whitespace.", parameterName);
            if (Path.IsPathRooted(path) || path.IndexOf(Path.VolumeSeparatorChar) >= 0)
                throw new ArgumentException("The path must be relative to the game directory.", parameterName);

            String[] segments = path
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
                throw new ArgumentException("The relative path must identify a file or directory.", parameterName);

            Char[] invalidCharacters = Path.GetInvalidFileNameChars();
            foreach (String segment in segments)
            {
                if (segment == "." || segment == "..")
                    throw new ArgumentException("Relative path transitions '.' and '..' are not allowed.", parameterName);
                if (segment.IndexOfAny(invalidCharacters) >= 0)
                    throw new ArgumentException($"The path segment '{segment}' contains an invalid character.", parameterName);
                if (segment.EndsWith(" ", StringComparison.Ordinal) || segment.EndsWith(".", StringComparison.Ordinal))
                    throw new ArgumentException($"The path segment '{segment}' cannot end with a space or period.", parameterName);
                if (IsReservedWindowsName(segment))
                    throw new ArgumentException($"The path segment '{segment}' is a reserved Windows name.", parameterName);
            }

            return new SafeRelativePath(String.Join(Path.DirectorySeparatorChar.ToString(), segments));
        }

        public override String ToString() => Value;

        private static Boolean IsReservedWindowsName(String segment)
        {
            String name = Path.GetFileNameWithoutExtension(segment);
            if (name.Equals("CON", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("PRN", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("AUX", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("NUL", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return IsNumberedDevice(name, "COM") || IsNumberedDevice(name, "LPT");
        }

        private static Boolean IsNumberedDevice(String name, String prefix)
        {
            if (!name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || name.Length != prefix.Length + 1)
                return false;

            Char suffix = name[name.Length - 1];
            return suffix >= '1' && suffix <= '9';
        }
    }
}
