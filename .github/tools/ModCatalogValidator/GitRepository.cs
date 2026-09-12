using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ModCatalogValidator;

internal sealed partial class GitRepository
{
    public async Task<String> GetRelativePathAsync(String path)
    {
        String root = (await RunAsync("rev-parse", "--show-toplevel")).Trim();
        String relativePath = Path.GetRelativePath(root, path);
        if (relativePath == ".." || relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            throw new InvalidOperationException($"Catalog path '{path}' is outside the Git worktree.");

        return relativePath.Replace('\\', '/');
    }

    public Task<String> ReadFileAsync(String revision, String path) => RunAsync("show", $"{revision}:{path}");

    public async Task<IReadOnlyList<DiffHunk>> ReadDiffAsync(String baseRevision, String headRevision, String path)
    {
        String diff = await RunAsync("diff", "--unified=0", baseRevision, headRevision, "--", path);
        List<DiffHunk> hunks = [];

        foreach (String line in diff.Split('\n'))
        {
            Match match = HunkHeader().Match(line);
            if (!match.Success)
                continue;

            hunks.Add(new DiffHunk(ParseRange(match.Groups[1], match.Groups[2]), ParseRange(match.Groups[3], match.Groups[4])));
        }

        return hunks;
    }

    private static LineRange ParseRange(Group start, Group count) => new(Int32.Parse(start.Value), count.Success ? Int32.Parse(count.Value) : 1);

    private static async Task<String> RunAsync(params String[] arguments)
    {
        using Process process = new();
        process.StartInfo = new ProcessStartInfo("git") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        foreach (String argument in arguments)
            process.StartInfo.ArgumentList.Add(argument);

        if (!process.Start())
            throw new InvalidOperationException("Could not start Git.");

        Task<String> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<String> errorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        String output = await outputTask;
        String error = await errorTask;

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"Git failed: {error.Trim()}");

        return output;
    }

    [GeneratedRegex("^@@ -(\\d+)(?:,(\\d+))? \\+(\\d+)(?:,(\\d+))? @@", RegexOptions.CultureInvariant)]
    private static partial Regex HunkHeader();
}
