using System;
using FF9;

namespace Memoria
{
    public sealed class ItemEffect : ICsvEntry
    {
        public TargetType Targets;
        public Boolean DefaultAlly;
        public TargetDisplay Display;
        public Int16 AnimationId;
        public Boolean Dead;
        public Boolean DefaultDead;

        public Byte ScriptId;
        public Byte Power;
        public Byte Rate;
        public EffectElement Element;

        public BattleStatus Status;

        public static ItemEffect FromItemData(ITEM_DATA data)
        {
            return new ItemEffect
            {
                Targets = (TargetType)data.info.cursor,
                DefaultAlly = data.info.def_cur == 1,
                Display = (TargetDisplay)data.info.sub_win,
                AnimationId = data.info.vfx_no,
                Dead = data.info.dead != 0,
                DefaultDead = data.info.def_dead != 0,

                ScriptId = data.Ref.prog_no,
                Power = data.Ref.power,
                Element = (EffectElement)data.Ref.attr,
                Rate = data.Ref.rate,

                Status = (BattleStatus)data.status
            };
        }

        public void ParseEntry(String[] raw)
        {
            int index = 0;

            Targets = (TargetType)CsvParser.Byte(raw[index++]);
            DefaultAlly = CsvParser.Boolean(raw[index++]);
            Display = (TargetDisplay)CsvParser.Byte(raw[index++]);
            AnimationId = CsvParser.Int16(raw[index++]);
            Dead = CsvParser.Boolean(raw[index++]);
            DefaultDead = CsvParser.Boolean(raw[index++]);

            ScriptId = CsvParser.Byte(raw[index++]);
            Power = CsvParser.Byte(raw[index++]);
            Rate = CsvParser.Byte(raw[index++]);
            Element = (EffectElement)CsvParser.Byte(raw[index++]);

            Status = (BattleStatus)CsvParser.UInt32(raw[index]);
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.Byte((Byte)Targets);
            sw.Boolean(DefaultAlly);
            sw.Byte((Byte)Display);
            sw.Int16(AnimationId);
            sw.Boolean(Dead);
            sw.Boolean(DefaultDead);

            sw.Byte(ScriptId);
            sw.Byte(Power);
            sw.Byte(Rate);
            sw.Byte((Byte)Element);

            sw.UInt32((UInt32)Status);
        }

        public ITEM_DATA ToItemData()
        {
            return new ITEM_DATA(
                new CMD_INFO((Byte)Targets, (Byte)(DefaultAlly ? 1 : 0), (Byte)Display, AnimationId, 0, (Byte)(Dead ? 1 : 0), 0, (Byte)(DefaultDead ? 1 : 0)),
                new BTL_REF(ScriptId, Power, (Byte)Element, Rate),
                (UInt32)Status);
        }
    }
}