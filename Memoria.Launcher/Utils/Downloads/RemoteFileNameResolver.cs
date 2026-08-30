#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Memoria.Launcher.Utils.Downloads
{
    internal static class RemoteFileNameResolver
    {
        public static String Resolve(HttpResponseMessage response, Uri source)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.IsAbsoluteUri)
                throw new ArgumentException("The source URI must be absolute.", nameof(source));

            Uri effectiveSource = response.RequestMessage?.RequestUri ?? source;
            String? dispositionName = response.Content.Headers.ContentDisposition?.FileNameStar;
            String? legacyDispositionName = response.Content.Headers.ContentDisposition?.FileName;
            String?[] candidates =
            [
                NormalizeCandidate(dispositionName),
                NormalizeCandidate(legacyDispositionName),
                GetUriFileName(effectiveSource),
                GetUriFileName(source)
            ];

            String? resolved = candidates.FirstOrDefault(HasExtension)
                ?? candidates.FirstOrDefault(value => !String.IsNullOrWhiteSpace(value));
            if (String.IsNullOrWhiteSpace(resolved))
            {
                throw new InvalidDataException(
                    $"The server did not provide a usable file name for '{source}'. " +
                    "Add a file name to the URL or configure Content-Disposition on the server.");
            }

            return resolved!;
        }

        private static String? GetUriFileName(Uri uri)
        {
            String path = uri.AbsolutePath.TrimEnd('/');
            if (path.Length == 0)
                return null;

            return NormalizeCandidate(Uri.UnescapeDataString(Path.GetFileName(path)));
        }

        private static String? NormalizeCandidate(String? value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            String unquoted = value!.Trim().Trim('"').Replace('\\', '/');
            String fileName = Path.GetFileName(unquoted);
            if (String.IsNullOrWhiteSpace(fileName) || fileName == "." || fileName == "..")
                return null;
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return null;
            if (fileName.EndsWith(" ", StringComparison.Ordinal) || fileName.EndsWith(".", StringComparison.Ordinal))
                return null;

            return fileName;
        }

        private static Boolean HasExtension(String? fileName)
        {
            return !String.IsNullOrWhiteSpace(fileName) && !String.IsNullOrWhiteSpace(Path.GetExtension(fileName));
        }
    }
}
