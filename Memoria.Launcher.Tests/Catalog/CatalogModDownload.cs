namespace Memoria.Launcher.Tests.Catalog;

internal sealed record CatalogModDownload(
    Int32 Order,
    String ModName,
    String DownloadUrl,
    String? DownloadFormat);
