using System.Xml.Linq;

namespace ModCatalogValidator;

internal readonly record struct LineRange(Int32 Start, Int32 Count)
{
    public Int32 End => Start + Count - 1;

    public Boolean Overlaps(LineRange other) => Count > 0 && other.Count > 0 && Start <= other.End && End >= other.Start;
}

internal sealed record DiffHunk(LineRange BaseRange, LineRange HeadRange);

internal sealed record CatalogMod(String Name, XElement Element, LineRange Lines);

internal sealed record CatalogSnapshot(IReadOnlyList<CatalogMod> Mods);

internal sealed record DownloadTarget(Int32 Order, String ModName, String Url, String? ExpectedFormat);

internal sealed record DownloadResult(DownloadTarget Target, String? FileName, IReadOnlyList<String> Errors)
{
    public Boolean IsSuccess => Errors.Count == 0;
}
