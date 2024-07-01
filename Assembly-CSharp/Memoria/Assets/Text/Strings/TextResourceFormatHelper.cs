using System;

namespace Memoria.Assets
{
    public static class TextResourceFormatHelper
    {
        public static String GetFileExtension(this TextResourceFormat format)
        {
            return format switch
            {
                TextResourceFormat.Strings => ".strings",
                TextResourceFormat.Resjson => ".resjson",
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, $"Format {format} is not supported.")
            };
        }

        public static TextResourceFormat ResolveFileFormat(String fileExtension)
        {
            if (TryResolveFileFormat(fileExtension, out TextResourceFormat format))
                return format;

            throw new ArgumentException(fileExtension);
        }

        public static Boolean TryResolveFileFormat(String fileExtension, out TextResourceFormat format)
        {
            String extension = fileExtension?.ToLowerInvariant();

            format = extension switch
            {
                ".strings" => TextResourceFormat.Strings,
                ".resjson" => TextResourceFormat.Resjson,
                _ => default
            };

            return format != default;
        }
    }
}
