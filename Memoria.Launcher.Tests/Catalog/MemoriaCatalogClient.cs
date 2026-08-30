using System.Xml.Linq;
using Memoria.Launcher.Utils.Catalog;

namespace Memoria.Launcher.Tests.Catalog;

internal sealed class MemoriaCatalogClient(HttpClient httpClient)
{
    private static readonly HashSet<XName> RepeatableModElements =
    [
        "CompatibilityNotes",
        "Header",
        "SubMod"
    ];

    public async Task<MemoriaCatalogSnapshot> GetAsync(CancellationToken cancellationToken = default)
    {
        using (HttpRequestMessage request = new(HttpMethod.Get, MemoriaCatalogEndpoints.Default))
        {
            using (HttpResponseMessage response = await httpClient.SendAsync(
                       request,
                       HttpCompletionOption.ResponseHeadersRead,
                       cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                await using (Stream content = await response.Content.ReadAsStreamAsync(cancellationToken))
                {
                    XDocument document = await XDocument.LoadAsync(content, LoadOptions.None, cancellationToken);

                    if (document.Root?.Name != "ModCatalog")
                        throw new InvalidDataException("The catalog root element must be <ModCatalog>.");

                    List<CatalogModDownload> downloads = new();
                    List<CatalogStructureIssue> structureIssues = new();
                    Int32 order = 0;

                    foreach (XElement modElement in document.Root.Elements("Mod"))
                    {
                        String modName = GetOptionalValue(modElement, "Name") ?? "<unnamed mod>";
                        String? downloadFormat = GetOptionalValue(modElement, "DownloadFormat");
                        structureIssues.AddRange(FindDuplicateElements(modElement, modName));

                        foreach (XElement urlElement in modElement.Elements("DownloadUrl"))
                        {
                            String downloadUrl = urlElement.Value.Trim();
                            if (downloadUrl.Length == 0)
                                continue;

                            downloads.Add(new CatalogModDownload(order++, modName, downloadUrl, downloadFormat));
                        }
                    }

                    if (downloads.Count == 0)
                        throw new InvalidDataException("The catalog does not contain any non-empty <DownloadUrl> elements.");

                    return new MemoriaCatalogSnapshot(downloads, structureIssues);
                }
            }
        }
    }

    private static IEnumerable<CatalogStructureIssue> FindDuplicateElements(XElement modElement, String modName)
    {
        return modElement
            .Elements()
            .Where(element => !RepeatableModElements.Contains(element.Name))
            .GroupBy(static element => element.Name)
            .Where(static group => group.Count() > 1)
            .Select(group => new CatalogStructureIssue(modName, group.Key.LocalName, group.Count()));
    }

    private static String? GetOptionalValue(XElement element, XName name)
    {
        String? value = element.Element(name)?.Value.Trim();
        return String.IsNullOrWhiteSpace(value) ? null : value;
    }
}
