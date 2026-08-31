using System;
using Memoria.Prime.CSV;
using FF9;

namespace Memoria.Data
{
    public class ItemAttack : ICsvEntry
    {
        public String Comment;
        public Int32 Id;
        public ItemAttack Data;

        public WeaponCategory Category;
        public StatusSetId StatusIndex;
        public String ModelName;
        public UInt16 ModelId;
        public BTL_REF Ref = new BTL_REF();
        public Int16 Offset1;
        public Int16 Offset2;
        public Byte HitSfx;
        public String[] CustomTexture = [];

        public static ItemAttack GetExisting(Int32 id)
        {
            if (ff9weap.WeaponData.TryGetValue(id, out ItemAttack result))
                return result;
            throw new NotSupportedException($"The option AppendMode must be used to patch existing entries but the entry {id} doesn't exist");
        }

        public void ParseDataEntry(String[] raw, CsvMetaData metadata, ref Int32 index)
        {
            if (metadata.HasField("Category")) Category = (WeaponCategory)CsvParser.Byte(raw[index++]);
            if (metadata.HasField("StatusIndex")) StatusIndex = (StatusSetId)CsvParser.Int32(raw[index++]);
            if (metadata.HasField("ModelName"))
            {
                ModelName = CsvParser.String(raw[index++]);
                if (!String.IsNullOrEmpty(ModelName))
                    ModelId = (UInt16)FF9BattleDB.GEO.GetKey(ModelName);
                else
                    ModelId = UInt16.MaxValue;
            }

            if (metadata.HasField("ScriptId")) Ref.ScriptId = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("Power")) Ref.Power = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("Elements")) Ref.Elements = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("Rate")) Ref.Rate = CsvParser.Int32(raw[index++]);

            if (metadata.HasField("Offset1")) Offset1 = Int16.Parse(raw[index++]);
            if (metadata.HasField("Offset2")) Offset2 = Int16.Parse(raw[index++]);
            if (metadata.HasField("HitSfx"))
            {
                if (metadata.HasOption($"Include{nameof(HitSfx)}"))
                    HitSfx = Byte.Parse(raw[index++]);
                else
                    HitSfx = (Byte)Id;
            }
            if (metadata.HasOption($"Include{nameof(CustomTexture)}") && metadata.HasField("CustomTexture"))
            {
                String StringTexture = CsvParser.String(raw[index++]);
                if (StringTexture.Trim().Length > 0)
                {
                    CustomTexture = StringTexture.Split(',');
                    for (Int32 i = 0; i < CustomTexture.Length; i++)
                        CustomTexture[i] = CustomTexture[i].Trim();
                }
            }
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Comment = CsvParser.String(raw[index++]);
            Id = CsvParser.Int32(raw[index++]);
            Data = metadata.IsAppendMode ? GetExisting(Id) : this;
            Data.ParseDataEntry(raw, metadata, ref index);
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
            {
                if (CustomTexture != null)
                    sw.String(String.Join(", ", CustomTexture));
                else
                    sw.String(String.Empty);
            }
        }
    }
}
