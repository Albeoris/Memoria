namespace ModCatalogValidator;

internal static class ChangedModSelector
{
    public static IReadOnlySet<String> Select(IReadOnlyList<DiffHunk> hunks, IReadOnlyList<CatalogMod> baseMods, IReadOnlyList<CatalogMod> headMods)
    {
        SortedSet<String> names = new(StringComparer.Ordinal);

        foreach (DiffHunk hunk in hunks)
        {
            AddOverlappingNames(names, baseMods, hunk.BaseRange);
            AddOverlappingNames(names, headMods, hunk.HeadRange);
        }

        return names;
    }

    private static void AddOverlappingNames(ISet<String> names, IEnumerable<CatalogMod> mods, LineRange changedLines)
    {
        foreach (CatalogMod mod in mods.Where(mod => mod.Lines.Overlaps(changedLines)))
            names.Add(mod.Name);
    }
}
