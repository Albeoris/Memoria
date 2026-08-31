#nullable enable

using System;
using System.Net.Http;
using System.Text;

namespace Memoria.Launcher.Utils.Downloads
{
    internal static class DownloadResponseValidator
    {
        private const Int32 MaximumInspectedByteCount = 1024;

        public static void ValidateHeaders(HttpResponseMessage response, Uri source)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            String? mediaType = response.Content.Headers.ContentType?.MediaType;
            if (IsHtmlMediaType(mediaType))
                throw CreateUnexpectedHtmlException(response, source);
        }

        public static void ValidatePayloadPrefix(
            Byte[] buffer,
            Int32 byteCount,
            HttpResponseMessage response,
            Uri source)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (byteCount < 0 || byteCount > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(byteCount));
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (LooksLikeHtmlDocument(buffer, Math.Min(byteCount, MaximumInspectedByteCount)))
                throw CreateUnexpectedHtmlException(response, source);
        }

        public static Boolean IsHtmlMediaType(String? mediaType)
        {
            return String.Equals(mediaType, "text/html", StringComparison.OrdinalIgnoreCase) ||
                   String.Equals(mediaType, "application/xhtml+xml", StringComparison.OrdinalIgnoreCase);
        }

        private static Boolean LooksLikeHtmlDocument(Byte[] buffer, Int32 byteCount)
        {
            if (byteCount == 0)
                return false;

            String prefix = Encoding.UTF8.GetString(buffer, 0, byteCount)
                .TrimStart('\uFEFF', ' ', '\t', '\r', '\n');

            return prefix.StartsWith("<!doctype html", StringComparison.OrdinalIgnoreCase) ||
                   prefix.StartsWith("<html", StringComparison.OrdinalIgnoreCase);
        }

        private static DownloadException CreateUnexpectedHtmlException(HttpResponseMessage response, Uri source)
        {
            Uri effectiveSource = response.RequestMessage?.RequestUri ?? source;
            String mediaType = response.Content.Headers.ContentType?.ToString() ?? "<not specified>";
            return new DownloadException(
                DownloadFailureKind.UnexpectedContent,
                $"The server returned an HTML page instead of the requested file. " +
                $"Source: '{source}'. Effective URL: '{effectiveSource}'. Content-Type: '{mediaType}'. " +
                "The file may have been removed, access may be restricted, or the link may not be a direct download link.");
        }
    }
}
