using System;
using Memoria.Data;
using FF9;

namespace Memoria
{
    public sealed class MutableBattleCommand : BattleCommand
    {
        private MutableBattleCommand(BTL_DATA caster, UInt16 targetId, BattleCommandId cmdNo, Int32 subNo)
            : base(new CMD_DATA())
        {
            btl_cmd.ClearCommand(Data);
            btl_cmd.ClearReflecData(Data);
            Data.regist = caster;
            Data.cmd_no = cmdNo;
            Data.sub_no = subNo;
            Data.SetAAData(btl_util.GetCommandAction(Data));
            Data.ScriptId = btl_util.GetCommandScriptId(Data);
            Data.tar_id = targetId;
            Data.info.cursor = Comn.countBits(targetId) > 1 ? (Byte)1 : (Byte)0;
            Data.info.cover = 0;
            Data.info.dodge = 0;
            Data.info.reflec = 0;
            Data.IsShortRange = btl_util.IsAttackShortRange(Data);
        }

        public MutableBattleCommand(BattleUnit caster, UInt16 targetId, BattleCommandId cmdNo, BattleAbilityId mainAbilId)
            : this(caster.Data, targetId, cmdNo, (Int32)mainAbilId)
        {
        }

        public MutableBattleCommand(BattleUnit caster, UInt16 targetId, RegularItem itemUse)
            : this(caster.Data, targetId, BattleCommandId.Item, (Int32)itemUse)
        {
        }

        public new BattleCommandId Id
        {
            get => base.Id;
            set => Data.cmd_no = value;
        }

        public new BattleAbilityId AbilityId
        {
            get => btl_util.GetCommandMainActionIndex(Data);
            set => Data.sub_no = (Int32)value;
        }

        public new RegularItem ItemId
        {
            get => btl_util.GetCommandItem(Data);
            set => Data.sub_no = (Int32)value;
        }

        public new Int32 ScriptId
        {
            get => base.ScriptId;
            set => Data.ScriptId = value;
        }

        public new Int32 HitRate
        {
            get => base.HitRate;
            set => Data.HitRate = value;
        }

        public new Int32 Power
        {
            get => base.Power;
            set => Data.Power = value;
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
            set => Data.aa.AddStatusNo = value;
        }

        public new EffectElement Element
        {
            get => base.Element;
            set => Data.Element = value;
        }

        public new EffectElement ElementForBonus
        {
            get => base.ElementForBonus;
            set => Data.ElementForBonus = value;
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
    }
}