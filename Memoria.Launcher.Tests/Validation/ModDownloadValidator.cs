using Memoria.Launcher.Tests.Catalog;
using Memoria.Launcher.Tests.Downloads;
using Memoria.Launcher.Utils.Downloads;

namespace Memoria.Launcher.Tests.Validation;

internal sealed class ModDownloadValidator(RemoteFileProbe fileProbe)
{
    private const String SingleFileWithPathPrefix = "SingleFileWithPath:";

    public async Task<ModDownloadValidationResult> ValidateAsync(
        CatalogModDownload mod,
        CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(mod.DownloadUrl, UriKind.Absolute, out Uri? downloadUri) ||
            downloadUri.Scheme is not ("http" or "https"))
        {
            return Failure(mod, $"DownloadUrl is not an absolute HTTP or HTTPS URL: '{mod.DownloadUrl}'.");
        }

        RemoteFileMetadata file;
        try
        {
            file = await fileProbe.ProbeAsync(downloadUri, cancellationToken);
        }
        catch (Exception exception)
        {
            return Failure(mod, $"The request failed: {exception.GetType().Name}: {exception.Message}");
        }

        List<String> errors = new();
        Int32 statusCode = (Int32)file.StatusCode;
        if (statusCode is < 200 or >= 300)
            errors.Add($"The link returned HTTP {statusCode} ({file.ReasonPhrase ?? "no reason phrase"}).");

        if (String.IsNullOrWhiteSpace(file.FileName))
            errors.Add("The file name could not be determined from Content-Disposition or the URL.");

        if (String.IsNullOrWhiteSpace(file.Extension))
            errors.Add("The file extension could not be determined.");

        if (DownloadResponseValidator.IsHtmlMediaType(file.MediaType))
        {
            errors.Add(
                "The link returned an HTML page instead of a downloadable file. " +
                "The file may have been removed, access may be restricted, or the link may not be a direct download link.");
        }

        if (ShouldValidateDownloadFormat(mod.DownloadFormat) &&
            !String.Equals(mod.DownloadFormat, file.Extension, StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(
                $"DownloadFormat '{mod.DownloadFormat}' does not match file extension " +
                $"'{file.Extension ?? "<unknown>"}'.");
        }

        return new ModDownloadValidationResult(mod, file, errors);
    }

    private static Boolean ShouldValidateDownloadFormat(String? downloadFormat) =>
        !String.IsNullOrWhiteSpace(downloadFormat) &&
        !downloadFormat.StartsWith(SingleFileWithPathPrefix, StringComparison.OrdinalIgnoreCase);

    private static ModDownloadValidationResult Failure(CatalogModDownload mod, String error) =>
        new(mod, null, [error]);
}
