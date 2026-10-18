namespace Memoria.Launcher.Tests.Catalog;

internal sealed record MemoriaCatalogSnapshot(
    IReadOnlyList<CatalogModDownload> Downloads,
    IReadOnlyList<CatalogStructureIssue> StructureIssues);
