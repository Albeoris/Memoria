using System.Collections.Concurrent;
using System.Net;
using System.Text;
using Memoria.Launcher.Tests.Catalog;
using Memoria.Launcher.Tests.Downloads;
using Memoria.Launcher.Tests.Validation;
using Memoria.Launcher.Utils.Catalog;
using Xunit;
using Xunit.Abstractions;

namespace Memoria.Launcher.Tests;

public sealed class MemoriaCatalogTests(ITestOutputHelper output)
{
    private const Int32 MaxConcurrentRequests = 8;
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(45);

    [Fact]
    public async Task Every_mod_download_link_is_available_and_has_the_expected_format()
    {
        using (HttpClient httpClient = CreateHttpClient())
        {
            MemoriaCatalogClient catalogClient = new(httpClient);
            ModDownloadValidator validator = new(new RemoteFileProbe(httpClient));

            MemoriaCatalogSnapshot catalog;
            try
            {
                catalog = await catalogClient.GetAsync();
            }
            catch (Exception exception)
            {
                String catalogFailure =
                    $"Catalog validation could not start.{Environment.NewLine}" +
                    $"Catalog: {MemoriaCatalogEndpoints.Default}{Environment.NewLine}" +
                    $"Error: {exception.GetType().Name}: {exception.Message}{Environment.NewLine}" +
                    "Successful checks: 0";

                output.WriteLine(catalogFailure);
                Assert.Fail(catalogFailure);
                return;
            }

            ConcurrentBag<ModDownloadValidationResult> results = new();
            await Parallel.ForEachAsync(
                catalog.Downloads,
                new ParallelOptions { MaxDegreeOfParallelism = MaxConcurrentRequests },
                async (download, cancellationToken) =>
                {
                    results.Add(await validator.ValidateAsync(download, cancellationToken));
                });

            ModDownloadValidationResult[] orderedResults = results.OrderBy(static result => result.Mod.Order).ToArray();
            String report = CreateReport(orderedResults, catalog.StructureIssues);

            output.WriteLine(report);
            Assert.True(
                orderedResults.All(static result => result.IsSuccess) && catalog.StructureIssues.Count == 0,
                report);
        }
    }

    private static HttpClient CreateHttpClient()
    {
        SocketsHttpHandler handler = new()
        {
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.All,
            ConnectTimeout = TimeSpan.FromSeconds(15),
            MaxAutomaticRedirections = 20,
            MaxConnectionsPerServer = 4,
            PooledConnectionLifetime = TimeSpan.FromMinutes(5)
        };

        HttpClient client = new(handler)
        {
            Timeout = RequestTimeout
        };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Memoria-Catalog-Validator/1.0");
        return client;
    }

    private static String CreateReport(
        IReadOnlyCollection<ModDownloadValidationResult> results,
        IReadOnlyCollection<CatalogStructureIssue> structureIssues)
    {
        ModDownloadValidationResult[] failures = results.Where(static result => !result.IsSuccess).ToArray();
        Int32 successfulChecks = results.Count - failures.Length;
        Int32 failedChecks = failures.Length + structureIssues.Count;
        StringBuilder report = new StringBuilder()
            .AppendLine("Memoria catalog download validation completed.")
            .AppendLine($"Catalog: {MemoriaCatalogEndpoints.Default}")
            .AppendLine($"Checked links: {results.Count}")
            .AppendLine($"Successful checks: {successfulChecks}")
            .AppendLine($"Failed checks: {failedChecks}");

        AppendStructureIssues(report, structureIssues);

        if (failures.Length > 0)
        {
            report.AppendLine().AppendLine("Failures:");
            foreach (ModDownloadValidationResult failure in failures)
            {
                report
                    .AppendLine($"- Mod: {failure.Mod.ModName}")
                    .AppendLine($"  DownloadUrl: {failure.Mod.DownloadUrl}")
                    .AppendLine($"  Expected format: {failure.Mod.DownloadFormat ?? "<not specified>"}")
                    .AppendLine($"  HTTP status: {FormatStatus(failure.File)}")
                    .AppendLine($"  Effective URL: {failure.File?.EffectiveUri.ToString() ?? "<not available>"}")
                    .AppendLine($"  File name: {failure.File?.FileName ?? "<unknown>"}")
                    .AppendLine($"  Extension: {failure.File?.Extension ?? "<unknown>"}")
                    .AppendLine($"  Content-Type: {failure.File?.MediaType ?? "<unknown>"}")
                    .AppendLine($"  Content-Length: {failure.File?.ContentLength?.ToString() ?? "<unknown>"}");

                foreach (String error in failure.Errors)
                    report.AppendLine($"  Error: {error}");
            }
        }

        AppendSuccessfulLinks(report, results);
        return report.ToString();
    }

    private static void AppendStructureIssues(
        StringBuilder report,
        IReadOnlyCollection<CatalogStructureIssue> structureIssues)
    {
        if (structureIssues.Count == 0)
            return;

        report.AppendLine().AppendLine("Catalog structure failures:");
        foreach (CatalogStructureIssue issue in structureIssues)
        {
            report
                .AppendLine($"- Mod: {issue.ModName}")
                .AppendLine($"  Duplicate element: {issue.ElementName}")
                .AppendLine($"  Occurrences: {issue.Occurrences}")
                .AppendLine($"  Error: A mod cannot contain more than one <{issue.ElementName}> element.");
        }
    }

    private static void AppendSuccessfulLinks(
        StringBuilder report,
        IEnumerable<ModDownloadValidationResult> results)
    {
        IGrouping<String, ModDownloadValidationResult>[] groups = results
            .Where(static result => result.IsSuccess)
            .GroupBy(static result => result.File!.Extension!, StringComparer.OrdinalIgnoreCase)
            .OrderBy(static group => group.Key, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        report.AppendLine().AppendLine("Successful links by extension:");
        if (groups.Length == 0)
        {
            report.AppendLine("<none>");
            return;
        }

        foreach (IGrouping<String, ModDownloadValidationResult> group in groups)
        {
            report.AppendLine($".{group.Key.ToLowerInvariant()}:");
            foreach (ModDownloadValidationResult success in group.OrderBy(static result => result.Mod.Order))
            {
                report
                    .AppendLine($"- {success.Mod.DownloadUrl}")
                    .AppendLine($"  File name: {success.File!.FileName}");
            }
        }
    }

    private static String FormatStatus(RemoteFileMetadata? file) => file is null
        ? "<request failed>"
        : $"{(Int32)file.StatusCode} ({file.ReasonPhrase ?? "no reason phrase"})";
}
