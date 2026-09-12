using System.Net;
using System.Net.Http.Headers;

namespace ModCatalogValidator;

internal sealed class DownloadValidator(HttpClient client)
{
    private const String SingleFileWithPathPrefix = "SingleFileWithPath:";

    public async Task<DownloadResult> ValidateAsync(DownloadTarget target, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(target.Url, UriKind.Absolute, out Uri? source) || source.Scheme is not ("http" or "https"))
            return Failure(target, $"DownloadUrl is not an absolute HTTP or HTTPS URL: '{target.Url}'.");

        try
        {
            using HttpResponseMessage response = await ProbeAsync(source, cancellationToken);
            String? fileName = ResolveFileName(response, source);
            String? extension = String.IsNullOrWhiteSpace(fileName) ? null : Path.GetExtension(fileName).TrimStart('.');
            String? mediaType = response.Content.Headers.ContentType?.MediaType;
            List<String> errors = [];

            if ((Int32)response.StatusCode is < 200 or >= 300)
                errors.Add($"The link returned HTTP {(Int32)response.StatusCode} ({response.ReasonPhrase ?? "no reason phrase"}).");
            if (String.IsNullOrWhiteSpace(fileName))
                errors.Add("The file name could not be determined from Content-Disposition or the URL.");
            if (String.IsNullOrWhiteSpace(extension))
                errors.Add("The file extension could not be determined.");
            if (mediaType is "text/html" or "application/xhtml+xml")
                errors.Add("The link returned an HTML page instead of a downloadable file.");
            if (ShouldValidateFormat(target.ExpectedFormat) && !String.Equals(target.ExpectedFormat, extension, StringComparison.OrdinalIgnoreCase))
                errors.Add($"DownloadFormat '{target.ExpectedFormat}' does not match file extension '{extension ?? "<unknown>"}'.");

            return new DownloadResult(target, fileName, errors);
        }
        catch (Exception exception)
        {
            return Failure(target, $"The request failed: {exception.GetType().Name}: {exception.Message}");
        }
    }

    private async Task<HttpResponseMessage> ProbeAsync(Uri source, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await SendAsync(source, true, cancellationToken);
        if (!ShouldRetryWithoutRange(response.StatusCode))
            return response;

        response.Dispose();
        return await SendAsync(source, false, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendAsync(Uri source, Boolean useRange, CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, source);
        if (useRange)
            request.Headers.Range = new RangeHeaderValue(0, 0);

        return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
    }

    private static String? ResolveFileName(HttpResponseMessage response, Uri source)
    {
        Uri effectiveSource = response.RequestMessage?.RequestUri ?? source;
        String?[] candidates = [NormalizeFileName(response.Content.Headers.ContentDisposition?.FileNameStar), NormalizeFileName(response.Content.Headers.ContentDisposition?.FileName), GetUriFileName(effectiveSource), GetUriFileName(source)];
        return candidates.FirstOrDefault(HasExtension) ?? candidates.FirstOrDefault(value => !String.IsNullOrWhiteSpace(value));
    }

    private static String? GetUriFileName(Uri uri)
    {
        String path = uri.AbsolutePath.TrimEnd('/');
        return path.Length == 0 ? null : NormalizeFileName(Uri.UnescapeDataString(path));
    }

    private static String? NormalizeFileName(String? value)
    {
        if (String.IsNullOrWhiteSpace(value))
            return null;

        String fileName = Path.GetFileName(value.Trim().Trim('"').Replace('\\', '/'));
        return String.IsNullOrWhiteSpace(fileName) || fileName is "." or ".." ? null : fileName;
    }

    private static Boolean HasExtension(String? fileName) => !String.IsNullOrWhiteSpace(fileName) && !String.IsNullOrWhiteSpace(Path.GetExtension(fileName));

    private static Boolean ShouldValidateFormat(String? format) => !String.IsNullOrWhiteSpace(format) && !format.StartsWith(SingleFileWithPathPrefix, StringComparison.OrdinalIgnoreCase);

    private static Boolean ShouldRetryWithoutRange(HttpStatusCode statusCode) => statusCode is HttpStatusCode.BadRequest or HttpStatusCode.Forbidden or HttpStatusCode.MethodNotAllowed or HttpStatusCode.NotAcceptable or HttpStatusCode.RequestedRangeNotSatisfiable;

    private static DownloadResult Failure(DownloadTarget target, String error) => new(target, null, [error]);
}
