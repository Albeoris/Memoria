using Memoria.Data;
using Memoria.Prime.CSV;
using System;

public class FF9ITEM : ICsvEntry
{
    public RegularItem id;
    public Byte count;

    public FF9ITEM()
    {
    }

    public FF9ITEM(RegularItem id, Byte count)
    {
        this.id = id;
        this.count = count;
    }

    public void ParseEntry(String[] raw, CsvMetaData metadata)
    {
        this.id = (RegularItem)CsvParser.Item(raw[0]);
        this.count = CsvParser.Byte(raw[1]);
    }

    public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
    {
        sw.Item((Int32)this.id);
        sw.Byte(this.count);
    }
}
