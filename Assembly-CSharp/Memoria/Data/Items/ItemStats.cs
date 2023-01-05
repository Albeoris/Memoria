using System;
using Memoria.Prime.CSV;

namespace FF9
{
	public class ItemStats : ICsvEntry
	{
        public String Comment;
        public Int32 Id;

        public Byte dex;
        public Byte str;
        public Byte mgc;
        public Byte wpr;
        public Byte p_up_attr;
        public DEF_ATTR def_attr;

	    public void ParseEntry(String[] raw, CsvMetaData metadata)
	    {
	        Comment = CsvParser.String(raw[0]);
	        Id = CsvParser.Int32(raw[1]);

	        dex = CsvParser.Byte(raw[2]);
	        str = CsvParser.Byte(raw[3]);
	        mgc = CsvParser.Byte(raw[4]);
	        wpr = CsvParser.Byte(raw[5]);

	        p_up_attr = CsvParser.Byte(raw[6]);

	        Byte invalid = CsvParser.Byte(raw[7]);
	        Byte absorb = CsvParser.Byte(raw[8]);
	        Byte half = CsvParser.Byte(raw[9]);
	        Byte weak = CsvParser.Byte(raw[10]);
	        def_attr = new DEF_ATTR(invalid, absorb, half, weak);
	    }

	    public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
	    {
	        sw.String(Comment);
	        sw.Int32(Id);

	        sw.Byte(dex);
	        sw.Byte(str);
	        sw.Byte(mgc);
	        sw.Byte(wpr);

	        sw.Byte(p_up_attr);

	        sw.Byte(def_attr.invalid);
	        sw.Byte(def_attr.absorb);
	        sw.Byte(def_attr.half);
	        sw.Byte(def_attr.weak);
	    }
	}
}
