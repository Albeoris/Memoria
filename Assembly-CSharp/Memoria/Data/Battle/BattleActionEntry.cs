using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
    public class BattleActionEntry : ICsvEntry
    {
        public String Comment;
        public BattleAbilityId Id;

        public AA_DATA ActionData = new AA_DATA();

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Comment = CsvParser.String(raw[index++]);
            Id = (BattleAbilityId)CsvParser.Int32(raw[index++]);

            if (metadata.HasField("Display")) ActionData.Info.DisplayStats = CsvParser.EnumValue<TargetDisplay>(raw[index++]);
            if (metadata.HasField("Targets")) ActionData.Info.Target = CsvParser.EnumValue<TargetType>(raw[index++]);
            if (metadata.HasField("DefaultAlly")) ActionData.Info.DefaultAlly = CsvParser.Boolean(raw[index++]);
            if (metadata.HasField("ForDead")) ActionData.Info.ForDead = CsvParser.Boolean(raw[index++]);
            if (metadata.HasField("DefaultDead")) ActionData.Info.DefaultOnDead = CsvParser.Boolean(raw[index++]);
            if (metadata.HasField("DefaultCamera")) ActionData.Info.DefaultCamera = CsvParser.Boolean(raw[index++]);
            if (metadata.HasField("AnimationId")) ActionData.Info.VfxIndex = CsvParser.Int16(raw[index++]);
            if (metadata.HasField("AnimationId2")) ActionData.Vfx2 = CsvParser.UInt16(raw[index++]);
            if (metadata.HasField("ScriptId")) ActionData.Ref.ScriptId = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("Power")) ActionData.Ref.Power = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("Elements")) ActionData.Ref.Elements = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("Rate")) ActionData.Ref.Rate = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("Category")) ActionData.Category = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("AddStatus")) ActionData.AddStatusNo = (StatusSetId)CsvParser.Int32(raw[index++]);
            if (metadata.HasField("MP")) ActionData.MP = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("Type")) ActionData.Type = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("CastingTitleType"))
            {
                if (metadata.HasOption($"Include{nameof(AA_DATA.CastingTitleType)}"))
                    ActionData.CastingTitleType = CsvParser.UInt32(raw[index++]);
                else
                    ActionData.CastingTitleType = GetDefaultCastingTitleType(Id);
            }
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32((Int32)Id);

            BattleCommandInfo cmdInfo = ActionData.Info;
            BTL_REF btlRef = ActionData.Ref;
            sw.EnumValue(cmdInfo.DisplayStats);
            sw.EnumValue(cmdInfo.Target);
            sw.Boolean(cmdInfo.DefaultAlly);
            sw.Boolean(cmdInfo.ForDead);
            sw.Boolean(cmdInfo.DefaultOnDead);
            sw.Boolean(cmdInfo.DefaultCamera);
            sw.Int16(cmdInfo.VfxIndex);
            sw.UInt16(ActionData.Vfx2);
            sw.Int32(btlRef.ScriptId);
            sw.Int32(btlRef.Power);
            sw.Byte(btlRef.Elements);
            sw.Int32(btlRef.Rate);
            sw.Byte(ActionData.Category);
            sw.Int32((Int32)ActionData.AddStatusNo);
            sw.Int32(ActionData.MP);
            sw.Byte(ActionData.Type);
            if (metadata.HasOption($"Include{nameof(AA_DATA.CastingTitleType)}"))
                sw.UInt32(ActionData.CastingTitleType);
        }

        public static UInt32 GetDefaultCastingTitleType(BattleAbilityId id)
        {
            switch (id)
            {
                case BattleAbilityId.FireSword:
                case BattleAbilityId.FiraSword:
                case BattleAbilityId.FiragaSword:
                case BattleAbilityId.BlizzardSword:
                case BattleAbilityId.BlizzaraSword:
                case BattleAbilityId.BlizzagaSword:
                case BattleAbilityId.ThunderSword:
                case BattleAbilityId.ThundaraSword:
                case BattleAbilityId.ThundagaSword:
                case BattleAbilityId.BioSword:
                case BattleAbilityId.WaterSword:
                case BattleAbilityId.FlareSword:
                case BattleAbilityId.DoomsdaySword:
                    return 254;
                case BattleAbilityId.Shiva:      return 153;
                case BattleAbilityId.Ifrit:      return 154;
                case BattleAbilityId.Ramuh:      return 155;
                case BattleAbilityId.Atomos:     return 192;
                case BattleAbilityId.Odin:       return 157;
                case BattleAbilityId.Leviathan:  return 158;
                case BattleAbilityId.Bahamut:    return 159;
                case BattleAbilityId.Ark:        return 160;
                case BattleAbilityId.Fenrir1:    return 194;
                case BattleAbilityId.Fenrir2:    return 193;
                case BattleAbilityId.Carbuncle1: return 195;
                case BattleAbilityId.Carbuncle2: return 196;
                case BattleAbilityId.Carbuncle3: return 197;
                case BattleAbilityId.Carbuncle4: return 198;
                case BattleAbilityId.Phoenix:    return 73;
                case BattleAbilityId.Madeen:     return 187;
            }
            return 255;
        }
    }
}
