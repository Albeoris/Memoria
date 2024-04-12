using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
	public class ItemAttack : ICsvEntry
	{
		public String Comment;
		public Int32 Id;

		public WeaponCategory Category;
		public BattleStatusIndex StatusIndex;
		public String ModelName;
		public UInt16 ModelId;
		public BTL_REF Ref;
		public Int16 Offset1;
		public Int16 Offset2;
		public Byte HitSfx;
		public String[] CustomTexture;

		public void ParseEntry(String[] raw, CsvMetaData metadata)
		{
			Comment = CsvParser.String(raw[0]);
			Id = CsvParser.Int32(raw[1]);

			Category = (WeaponCategory)CsvParser.Byte(raw[2]);
			StatusIndex = (BattleStatusIndex)CsvParser.Int32(raw[3]);
			ModelName = CsvParser.String(raw[4]);
			if (!String.IsNullOrEmpty(ModelName))
				ModelId = (UInt16)FF9BattleDB.GEO.GetKey(ModelName);
			else
				ModelId = UInt16.MaxValue;

			Int32 scriptId = CsvParser.Int32(raw[5]);
			Int32 power = CsvParser.Int32(raw[6]);
			Byte elements = CsvParser.Byte(raw[7]);
			Int32 rate = CsvParser.Int32(raw[8]);
			Ref = new BTL_REF(scriptId, power, elements, rate);

			Offset1 = Int16.Parse(raw[9]);
			Offset2 = Int16.Parse(raw[10]);
			if (metadata.HasOption($"Include{nameof(HitSfx)}"))
				HitSfx = Byte.Parse(raw[11]);
			else
				HitSfx = (Byte)Id;
			if (metadata.HasOption($"Include{nameof(CustomTexture)}"))
			{
				var StringTexture = CsvParser.String(raw[12]);
				CustomTexture = StringTexture.Split(',');
				for (Int32 i = 0; i < CustomTexture.Length; i++)
					CustomTexture[i] = CustomTexture[i].Trim();
			}
			else
			{
				CustomTexture = new String[0];
			}
		}

		public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
		{
			sw.String(Comment);
			sw.Int32(Id);

			sw.Byte((Byte)Category);
			sw.Int32((Int32)StatusIndex);
			sw.String(ModelName);

			BTL_REF btl = Ref;
			sw.Int32(btl.ScriptId);
			sw.Int32(btl.Power);
			sw.Byte(btl.Elements);
			sw.Int32(btl.Rate);

			sw.Int16(Offset1);
			sw.Int16(Offset2);
			if (metadata.HasOption($"Include{nameof(HitSfx)}"))
				sw.Byte(HitSfx);
			if (metadata.HasOption($"Include{nameof(CustomTexture)}"))
				sw.String(String.Join(", ", CustomTexture));
		}
	}
}
