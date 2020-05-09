using System;
using Memoria.Data;

namespace Memoria
{
    public sealed class MutableBattleCommand : BattleCommand
    {
        internal MutableBattleCommand(CMD_DATA data)
            : base(data)
        {
        }

        public MutableBattleCommand()
            : this(new CMD_DATA())
        {
        }

        public new BattleCommandId Id
        {
            get => base.Id;
            set => Data.cmd_no = value;
        }

        public new BattleAbilityId AbilityId
        {
            get => base.AbilityId;
            set => Data.sub_no = (Byte)value;
        }

        public new Byte ScriptId
        {
            get => base.ScriptId;
            set => Data.aa.Ref.ScriptId = value;
        }

        public new Byte HitRate
        {
            get => base.HitRate;
            set => Data.aa.Ref.Rate = value;
        }

        public new Byte Power
        {
            get => base.Power;
            set => Data.aa.Ref.Power = value;
        }

        public new Boolean IsManyTarget
        {
            get => base.IsManyTarget;
            set => Data.info.cursor = (Byte)(value ? 1 : 0);
        }

        public new TargetType TargetType
        {
            get => base.TargetType;
            set => Data.aa.Info.Target = value;
        }

        public new BattleStatusIndex AbilityStatusIndex
        {
            get => base.AbilityStatusIndex;
            set => Data.aa.AddNo = (Byte)value;
        }

        public new EffectElement Element
        {
            get => base.Element;
            set => Data.aa.Ref.Elements = (Byte)value;
        }

        public new SpecialEffect SpecialEffect
        {
            get => base.SpecialEffect;
            set => Data.aa.Info.VfxIndex = (Int16)value;
        }

        public new Boolean IsMeteorMiss
        {
            get => base.IsMeteorMiss;
            set => Data.info.meteor_miss = (Byte)(value ? 1 : 0);
        }

        public new Boolean IsShortSummon
        {
            get => base.IsShortSummon;
            set => Data.info.short_summon = (Byte)(value ? 1 : 0);
        }

        public new Boolean IsZeroMP
        {
            get => base.IsZeroMP;
            set => Data.info.IsZeroMP = value;
        }

        public void LoadAbility()
        {
            AA_DATA original = FF9StateSystem.Battle.FF9Battle.aa_data[Data.sub_no];

            BattleCommandInfo originalInfo = original.Info;
            BattleCommandInfo newInfo = new BattleCommandInfo
            {
                Target = originalInfo.Target,
                DefaultAlly = originalInfo.DefaultAlly,
                DisplayStats = originalInfo.DisplayStats,
                VfxIndex = originalInfo.VfxIndex,
                ForDead = originalInfo.ForDead,
                DefaultCamera = originalInfo.DefaultCamera,
                DefaultOnDead = originalInfo.DefaultOnDead
            };

            BTL_REF originalRef = original.Ref;
            BTL_REF newRef = new BTL_REF
            {
                ScriptId = originalRef.ScriptId,
                Power = originalRef.Power,
                Elements = originalRef.Elements,
                Rate = originalRef.Rate
            };

            AA_DATA newData = new AA_DATA
            {
                Info = newInfo,
                Ref = newRef,
                Category = original.Category,
                AddNo = original.AddNo,
                MP = original.MP,
                Type = original.Type,
                Vfx2 = original.Vfx2,
                Name = original.Name
            };

            Data.aa = newData;
        }
    }
}