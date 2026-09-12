using System.Collections.Concurrent;
using System.Net;
using System.Xml.Linq;

namespace ModCatalogValidator;

internal sealed class CatalogValidationApplication(GitRepository repository, CatalogParser parser, CatalogStructureValidator structureValidator)
{
    private const Int32 MaxConcurrentRequests = 8;

    public async Task<Int32> RunAsync(ValidatorOptions options)
    {
        String gitPath = await repository.GetRelativePathAsync(options.CatalogPath);
        String headContent = await File.ReadAllTextAsync(options.CatalogPath);
        String baseContent = await repository.ReadFileAsync(options.BaseRevision, gitPath);
        CatalogSnapshot headCatalog = parser.Parse(headContent, "Pull request catalog");
        CatalogSnapshot? baseCatalog = TryParseBaseCatalog(baseContent);
        IReadOnlyList<DiffHunk> hunks = await repository.ReadDiffAsync(options.BaseRevision, options.HeadRevision, gitPath);
        IReadOnlySet<String> changedModNames = ChangedModSelector.Select(hunks, baseCatalog?.Mods ?? [], headCatalog.Mods);
        List<String> failures = [.. structureValidator.Validate(headCatalog)];
        IReadOnlyList<DownloadTarget> downloads = GetDownloads(headCatalog, changedModNames);

        PrintCatalogSummary(headCatalog, changedModNames);
        DownloadResult[] results = await ValidateDownloadsAsync(downloads);
        PrintDownloadResults(results);
        failures.AddRange(results.Where(result => !result.IsSuccess).SelectMany(FormatDownloadFailures));

        Console.WriteLine($"Checked download links: {results.Length}.");
        if (failures.Count == 0)
        {
            Console.WriteLine("Catalog validation passed.");
            return 0;
        }

        Console.Error.WriteLine($"Catalog validation failed:{Environment.NewLine}- {String.Join($"{Environment.NewLine}- ", failures)}");
        return 1;
    }

    private CatalogSnapshot? TryParseBaseCatalog(String content)
    {
        try
        {
            return parser.Parse(content, "Base catalog");
        }
        catch (InvalidDataException exception)
        {
            Console.WriteLine($"Warning: The base catalog could not be parsed, so deleted-line ownership cannot be determined: {exception.Message}");
            return null;
        }
    }

    private static IReadOnlyList<DownloadTarget> GetDownloads(CatalogSnapshot catalog, IReadOnlySet<String> changedModNames)
    {
        List<DownloadTarget> downloads = [];
        foreach (CatalogMod mod in catalog.Mods.Where(mod => changedModNames.Contains(mod.Name)))
        {
            String? expectedFormat = GetOptionalValue(mod.Element, "DownloadFormat");
            foreach (String url in mod.Element.Elements("DownloadUrl").Select(element => element.Value.Trim()).Where(url => url.Length > 0))
                downloads.Add(new DownloadTarget(downloads.Count, mod.Name, url, expectedFormat));
        }

        return downloads;
    }

    private static async Task<DownloadResult[]> ValidateDownloadsAsync(IReadOnlyList<DownloadTarget> downloads)
    {
        using HttpClient client = CreateHttpClient();
        DownloadValidator validator = new(client);
        ConcurrentBag<DownloadResult> results = [];
        await Parallel.ForEachAsync(downloads, new ParallelOptions { MaxDegreeOfParallelism = MaxConcurrentRequests }, async (download, cancellationToken) => results.Add(await validator.ValidateAsync(download, cancellationToken)));
        return results.OrderBy(result => result.Target.Order).ToArray();
    }

    private static HttpClient CreateHttpClient()
    {
        HttpClientHandler handler = new() { AllowAutoRedirect = true, AutomaticDecompression = DecompressionMethods.All, MaxAutomaticRedirections = 20 };
        HttpClient client = new(handler) { Timeout = TimeSpan.FromSeconds(45) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Memoria-Catalog-Validator/1.0");
        return client;
    }

    private static void PrintCatalogSummary(CatalogSnapshot catalog, IReadOnlySet<String> changedModNames)
    {
        Console.WriteLine($"Catalog structure checked: {catalog.Mods.Count} mods.");
        Console.WriteLine($"Changed mods: {changedModNames.Count}.");
        foreach (String name in changedModNames)
            Console.WriteLine($"- {name}");
    }

    private static void PrintDownloadResults(IEnumerable<DownloadResult> results)
    {
        foreach (DownloadResult result in results.Where(result => result.IsSuccess))
            Console.WriteLine($"  OK {result.Target.Url} -> {result.FileName}");
    }

    private static IEnumerable<String> FormatDownloadFailures(DownloadResult result) => result.Errors.Select(error => $"Mod '{result.Target.ModName}', DownloadUrl '{result.Target.Url}': {error}");

    private static String? GetOptionalValue(XElement element, XName name)
    {
        String? value = element.Element(name)?.Value.Trim();
        return String.IsNullOrWhiteSpace(value) ? null : value;
    }
}
