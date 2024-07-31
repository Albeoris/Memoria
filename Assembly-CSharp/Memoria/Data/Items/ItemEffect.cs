using FF9;
using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
    public sealed class ItemEffect : ICsvEntry
    {
        public Int32 Id;

        public TargetType Targets;
        public Boolean DefaultAlly;
        public TargetDisplay Display;
        public Int16 AnimationId;
        public Boolean Dead;
        public Boolean DefaultDead;

        public Int32 ScriptId;
        public Int32 Power;
        public Int32 Rate;
        public EffectElement Element;

        public BattleStatus Status;

        public static ItemEffect FromItemData(ITEM_DATA data)
        {
            return new ItemEffect
            {
                Id = -1,
                Targets = data.info.Target,
                DefaultAlly = data.info.DefaultAlly,
                Display = data.info.DisplayStats,
                AnimationId = data.info.VfxIndex,
                Dead = data.info.ForDead,
                DefaultDead = data.info.DefaultOnDead,

                ScriptId = data.Ref.ScriptId,
                Power = data.Ref.Power,
                Element = (EffectElement)data.Ref.Elements,
                Rate = data.Ref.Rate,

                Status = data.status
            };
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;

            if (metadata.HasOption($"Include{nameof(Id)}"))
                Id = CsvParser.Int32(raw[index++]);
            else
                Id = -1;

            Targets = (TargetType)CsvParser.Byte(raw[index++]);
            DefaultAlly = CsvParser.Boolean(raw[index++]);
            Display = (TargetDisplay)CsvParser.Byte(raw[index++]);
            AnimationId = CsvParser.Int16(raw[index++]);
            Dead = CsvParser.Boolean(raw[index++]);
            DefaultDead = CsvParser.Boolean(raw[index++]);

            ScriptId = CsvParser.Int32(raw[index++]);
            Power = CsvParser.Int32(raw[index++]);
            Rate = CsvParser.Int32(raw[index++]);
            Element = (EffectElement)CsvParser.Byte(raw[index++]);

            Status = (BattleStatus)CsvParser.UInt64(raw[index]);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            if (metadata.HasOption($"Include{nameof(Id)}"))
                sw.Int32(Id);

            sw.Byte((Byte)Targets);
            sw.Boolean(DefaultAlly);
            sw.Byte((Byte)Display);
            sw.Int16(AnimationId);
            sw.Boolean(Dead);
            sw.Boolean(DefaultDead);

            sw.Int32(ScriptId);
            sw.Int32(Power);
            sw.Int32(Rate);
            sw.Byte((Byte)Element);

            sw.UInt64((UInt64)Status);
        }

        public ITEM_DATA ToItemData()
        {
            return new ITEM_DATA(
                new BattleCommandInfo(Targets, DefaultAlly, Display, AnimationId, Dead, false, DefaultDead),
                new BTL_REF(ScriptId, Power, (Byte)Element, Rate),
                Status);
        }
    }
}
