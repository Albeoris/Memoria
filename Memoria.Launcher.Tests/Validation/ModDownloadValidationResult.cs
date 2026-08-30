using Memoria.Launcher.Tests.Catalog;
using Memoria.Launcher.Tests.Downloads;

namespace Memoria.Launcher.Tests.Validation;

internal sealed record ModDownloadValidationResult(
    CatalogModDownload Mod,
    RemoteFileMetadata? File,
    IReadOnlyList<String> Errors)
{
    public Boolean IsSuccess => Errors.Count == 0;
}
