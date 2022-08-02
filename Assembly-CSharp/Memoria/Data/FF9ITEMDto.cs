using Memoria.Prime.CSV;

public class FF9ITEMDto : ICsvEntry
{
    public byte ItemId { get; set; }
    public byte Count { get; set; }

    public void ParseEntry(string[] raw)
    {
        ItemId = CsvParser.Byte(raw[0]);
        Count = CsvParser.Byte(raw[1]);
    }

    public void WriteEntry(CsvWriter sw)
    {
        sw.Byte(ItemId);
        sw.Byte(Count);
    }

    public FF9ITEM ToModel() => new(ItemId, Count);
}