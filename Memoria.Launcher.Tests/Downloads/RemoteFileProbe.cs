using System.Net;
using System.Net.Http.Headers;

namespace Memoria.Launcher.Tests.Downloads;

internal sealed class RemoteFileProbe(HttpClient httpClient)
{
    public async Task<RemoteFileMetadata> ProbeAsync(
        Uri source,
        CancellationToken cancellationToken = default)
    {
        using (HttpResponseMessage rangedResponse = await SendAsync(source, useRange: true, cancellationToken))
        {
            if (rangedResponse.IsSuccessStatusCode || !ShouldRetryWithoutRange(rangedResponse.StatusCode))
                return CreateMetadata(source, rangedResponse);

            using (HttpResponseMessage response = await SendAsync(source, useRange: false, cancellationToken))
            {
                return CreateMetadata(source, response);
            }
        }
    }

    private async Task<HttpResponseMessage> SendAsync(
        Uri source,
        Boolean useRange,
        CancellationToken cancellationToken)
    {
        using (HttpRequestMessage request = new(HttpMethod.Get, source))
        {
            if (useRange)
                request.Headers.Range = new RangeHeaderValue(0, 0);

            return await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
        }
    }

    private static RemoteFileMetadata CreateMetadata(Uri source, HttpResponseMessage response)
    {
        Uri effectiveUri = response.RequestMessage?.RequestUri ?? source;
        String? fileName = ResolveFileName(response, effectiveUri, source);
        String? extension = String.IsNullOrWhiteSpace(fileName)
            ? null
            : Path.GetExtension(fileName).TrimStart('.');

        return new RemoteFileMetadata(
            response.StatusCode,
            response.ReasonPhrase,
            effectiveUri,
            fileName,
            String.IsNullOrWhiteSpace(extension) ? null : extension,
            response.Content.Headers.ContentType?.MediaType,
            response.Content.Headers.ContentRange?.Length ?? response.Content.Headers.ContentLength);
    }

    private static String? ResolveFileName(
        HttpResponseMessage response,
        Uri effectiveUri,
        Uri source)
    {
        ContentDispositionHeaderValue? disposition = response.Content.Headers.ContentDisposition;
        String?[] candidates = new[]
        {
            NormalizeFileName(disposition?.FileNameStar),
            NormalizeFileName(disposition?.FileName),
            GetFileName(effectiveUri),
            GetFileName(source)
        };

        return candidates.FirstOrDefault(HasExtension)
               ?? candidates.FirstOrDefault(static value => !String.IsNullOrWhiteSpace(value));
    }

    private static String? GetFileName(Uri uri)
    {
        String path = uri.AbsolutePath.TrimEnd('/');
        return NormalizeFileName(Uri.UnescapeDataString(Path.GetFileName(path)));
    }

    private static String? NormalizeFileName(String? value)
    {
        if (String.IsNullOrWhiteSpace(value))
            return null;

        String unquoted = value.Trim().Trim('"').Replace('\\', '/');
        String fileName = Path.GetFileName(unquoted);
        return String.IsNullOrWhiteSpace(fileName) ? null : fileName;
    }

    private static Boolean HasExtension(String? fileName) =>
        !String.IsNullOrWhiteSpace(fileName) && !String.IsNullOrWhiteSpace(Path.GetExtension(fileName));

    private static Boolean ShouldRetryWithoutRange(HttpStatusCode statusCode) => statusCode is
        HttpStatusCode.BadRequest or
        HttpStatusCode.Forbidden or
        HttpStatusCode.MethodNotAllowed or
        HttpStatusCode.NotAcceptable or
        HttpStatusCode.RequestedRangeNotSatisfiable;
}
