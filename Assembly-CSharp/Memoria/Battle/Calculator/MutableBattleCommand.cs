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
            get { return base.Id; }
            set { Data.cmd_no = (Byte)value; }
        }

        public new BattleAbilityId AbilityId
        {
            get { return base.AbilityId; }
            set { Data.sub_no = (Byte)value; }
        }

        public new Byte ScriptId
        {
            get { return base.ScriptId; }
            set { Data.aa.Ref.prog_no = value; }
        }

        public new Byte HitRate
        {
            get { return base.HitRate; }
            set { Data.aa.Ref.rate = value; }
        }

        public new Byte Power
        {
            get { return base.Power; }
            set { Data.aa.Ref.power = value; }
        }

        public new Boolean IsManyTarget
        {
            get { return base.IsManyTarget; }
            set { Data.info.cursor = (Byte)(value ? 1 : 0); }
        }

        public new TargetType TargetType
        {
            get { return base.TargetType; }
            set { Data.aa.Info.cursor = (Byte)value; }
        }

        public new BattleStatusId AbilityStatusId
        {
            get { return base.AbilityStatusId; }
            set { Data.aa.AddNo = (Byte)value; }
        }

        public new EffectElement Element
        {
            get { return base.Element; }
            set { Data.aa.Ref.attr = (Byte)value; }
        }

        public new SpecialEffect SpecialEffect
        {
            get { return base.SpecialEffect; }
            set { Data.aa.Info.vfx_no = (Int16)value; }
        }

        public new Boolean IsMeteorMiss
        {
            get { return base.IsMeteorMiss; }
            set { Data.info.meteor_miss = (Byte)(value ? 1 : 0); }
        }

        public new Boolean IsShortSummon
        {
            get { return base.IsShortSummon; }
            set { Data.info.short_summon = (Byte)(value ? 1 : 0); }
        }

        public void LoadAbility()
        {
            AA_DATA original = FF9StateSystem.Battle.FF9Battle.aa_data[Data.sub_no];

            CMD_INFO originalInfo = original.Info;
            CMD_INFO newInfo = new CMD_INFO
            {
                cursor = originalInfo.cursor,
                def_cur = originalInfo.def_cur,
                sub_win = originalInfo.sub_win,
                vfx_no = originalInfo.vfx_no,
                sfx_no = originalInfo.sfx_no,
                dead = originalInfo.dead,
                def_cam = originalInfo.def_cam,
                def_dead = originalInfo.def_dead
            };

            BTL_REF originalRef = original.Ref;
            BTL_REF newRef = new BTL_REF
            {
                prog_no = originalRef.prog_no,
                power = originalRef.power,
                attr = originalRef.attr,
                rate = originalRef.rate
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