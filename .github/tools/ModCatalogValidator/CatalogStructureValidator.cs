using System.Xml.Linq;

namespace ModCatalogValidator;

internal sealed class CatalogStructureValidator
{
    private static readonly HashSet<XName> RepeatableElements = ["CompatibilityNotes", "Header", "SubMod"];

    public IReadOnlyList<String> Validate(CatalogSnapshot catalog)
    {
        List<String> failures = [];
        if (catalog.Mods.Count == 0)
            failures.Add("The catalog does not contain any <Mod> elements.");

        foreach (CatalogMod mod in catalog.Mods)
        {
            IEnumerable<IGrouping<XName, XElement>> duplicates = mod.Element.Elements().Where(element => !RepeatableElements.Contains(element.Name)).GroupBy(element => element.Name).Where(group => group.Count() > 1);
            foreach (IGrouping<XName, XElement> duplicate in duplicates)
                failures.Add($"Mod '{mod.Name}' contains {duplicate.Count()} <{duplicate.Key.LocalName}> elements; only one is allowed.");
        }

        return failures;
    }
}
