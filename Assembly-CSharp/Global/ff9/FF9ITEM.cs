using System;
using Memoria.Prime.CSV;

public class FF9ITEM : ICsvEntry
{
    public Byte id;
    public Byte count;

    public FF9ITEM()
	{
	}

    public FF9ITEM(Byte id, Byte count)
	{
		this.id = id;
		this.count = count;
	}

    public void ParseEntry(String[] raw)
    {
        this.id = CsvParser.Byte(raw[0]);
        this.count = CsvParser.Byte(raw[1]);
    }

    public void WriteEntry(CsvWriter sw)
    {
        sw.Byte(this.id);
        sw.Byte(this.count);
    }
}
