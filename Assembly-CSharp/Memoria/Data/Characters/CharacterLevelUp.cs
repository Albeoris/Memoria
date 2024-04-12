using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
	public class CharacterLevelUp : ICsvEntry
	{
		public UInt32 ExperienceToLevel;
		public UInt16 BonusHP;
		public UInt16 BonusMP;

		public void ParseEntry(String[] raw, CsvMetaData metadata)
		{
			ExperienceToLevel = CsvParser.UInt32(raw[0]);
			BonusHP = CsvParser.UInt16(raw[1]);
			BonusMP = CsvParser.UInt16(raw[2]);
		}

		public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
		{
			sw.UInt32(ExperienceToLevel);
			sw.UInt16(BonusHP);
			sw.UInt16(BonusMP);
		}
	}
}
