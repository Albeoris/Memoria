using System.Xml;
using System.Xml.Linq;

namespace ModCatalogValidator;

internal sealed class CatalogParser
{
    public CatalogSnapshot Parse(String content, String description)
    {
        XDocument document;
        try
        {
            using XmlReader reader = XmlReader.Create(new StringReader(content), CreateReaderSettings());
            document = XDocument.Load(reader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
        }
        catch (XmlException exception)
        {
            throw new InvalidDataException($"{description} is not valid XML: {exception.Message}", exception);
        }

        if (document.Root?.Name != "ModCatalog")
            throw new InvalidDataException($"{description} root element must be <ModCatalog>.");

        XElement[] elements = document.Root.Elements("Mod").ToArray();
        LineRange[] ranges = ReadModRanges(content).ToArray();
        if (elements.Length != ranges.Length)
            throw new InvalidDataException($"{description} mod elements could not be mapped to source lines.");

        CatalogMod[] mods = elements.Select((element, index) => new CatalogMod(GetModName(element), element, ranges[index])).ToArray();
        return new CatalogSnapshot(mods);
    }

    private static IEnumerable<LineRange> ReadModRanges(String content)
    {
        using XmlReader reader = XmlReader.Create(new StringReader(content), CreateReaderSettings());
        Int32 startLine = 0;

        while (reader.Read())
        {
            IXmlLineInfo lineInfo = (IXmlLineInfo)reader;
            if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1 && reader.LocalName == "Mod")
            {
                startLine = lineInfo.LineNumber;
                if (reader.IsEmptyElement)
                    yield return new LineRange(startLine, 1);
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Depth == 1 && reader.LocalName == "Mod")
            {
                yield return new LineRange(startLine, lineInfo.LineNumber - startLine + 1);
            }
        }
    }

    private static XmlReaderSettings CreateReaderSettings() => new() { DtdProcessing = DtdProcessing.Prohibit, IgnoreComments = false, IgnoreWhitespace = false, XmlResolver = null };

    private static String GetModName(XElement element)
    {
        String? name = element.Element("Name")?.Value.Trim();
        return String.IsNullOrWhiteSpace(name) ? "<unnamed mod>" : name;
    }
}
