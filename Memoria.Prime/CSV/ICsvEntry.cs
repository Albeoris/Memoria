using System;

namespace Memoria.Prime.CSV
{
	public interface ICsvEntry
	{
		void ParseEntry(String[] raw, CsvMetaData metadata);
		void WriteEntry(CsvWriter sw, CsvMetaData metadata);
	}
}
