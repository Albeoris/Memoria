using System;
using System.Linq;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    // NOTE: this status (and the related commands) is still specifically handled by the game in some aspects
    // You shouldn't recycle it for a completly different effect

    [StatusScript(BattleStatusId.Jump)]
    public class JumpStatusScript : StatusScriptBase, IFinishCommandScript
    {
        public UInt16 SpearTargetId;
        public Int32 SpearCountdown;
        public Boolean UseTranceSpear;

        private Boolean CurrentlyUseSpear = false;
        private Boolean InitialisedModifier = false;

        public static Int32 GetJumpDuration(BattleUnit btl)
        {
            // Use the duration "ContiCnt" of Jump even if it is not registered as BattleStatusConst.ContiCount
            return BattleStatusId.Jump.GetStatData().ContiCnt * 4 * (60 - btl.Will);
        }

        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            SpearCountdown = GetJumpDuration(target);
            SpearTargetId = parameters.Length > 0 ? (UInt16)parameters[0] : target.Data.cmd[3].tar_id;
            UseTranceSpear = parameters.Length > 1 ? (Boolean)parameters[1] : target.Data.cmd[3].cmd_no == BattleCommandId.JumpInTrance;
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            BTL_DATA btl = target.Data;
            btl.tar_mode = 3;
            btl.bi.atb = 1;
            if (btl.bi.player != 0 && !FF9StateSystem.Settings.IsATBFull)
                btl.cur.at = 0;
            btl.sel_mode = 0;
            btl.cmd[3].cmd_no = BattleCommandId.None;
            btl.cmd[3].tar_id = 0;
            return true;
        }

        public void OnFinishCommand(BattleUnit unit, CMD_DATA cmd, Int32 tranceDecrease)
        {
            switch (cmd.cmd_no)
            {
                case BattleCommandId.Jump:
                case BattleCommandId.JumpInTrance:
                    if (!InitialisedModifier)
                    {
                        BTL_DATA btl = unit.Data;
                        btl.tar_mode = 2;
                        btl.bi.atb = 0;
                        btl.SetDisappear(true, 2);
                        unit.AddDelayedModifier(ProcessJumpSpear, null);
                        InitialisedModifier = true;
                    }
                    break;
                case BattleCommandId.Spear:
                    unit.RemoveStatus(BattleStatus.Jump);
                    break;
                case BattleCommandId.SpearInTrance:
                    // TODO [DV]
                    //if (Configuration.Mod.TranceSeek) // [DV] - Freyja come back after Jump when in Trance
                    //{
                    //    unit.RemoveStatus(BattleStatus.Jump);
                    //}
                    //else
                    //{
                    //    unit.AlterStatus(BattleStatus.Jump);
                    //    unit.Data.tar_mode = 2;
                    //    unit.Data.SetDisappear(true, 2);
                    //}
                    SpearCountdown = GetJumpDuration(unit);
                    break;
            }
        }

        private Boolean ProcessJumpSpear(BattleUnit freya)
        {
            if (!freya.IsUnderAnyStatus(BattleStatus.Jump))
                return false;

            if (Configuration.VoiceActing.Enabled)
            {
                Boolean useSpear = btl_util.GetBtlCurrentCommands(freya).Any(cmd => cmd.cmd_no == BattleCommandId.Spear || cmd.cmd_no == BattleCommandId.SpearInTrance);
                if (useSpear != CurrentlyUseSpear)
                {
                    if (useSpear)
                        BattleVoice.TriggerOnStatusChange(freya, "Used", BattleStatusId.Jump);
                    CurrentlyUseSpear = useSpear;
                }
            }
            if (btl_cmd.CheckUsingCommand(freya.Data.cmd[1]))
                return true;
            SpearCountdown -= freya.Data.cur.at_coef * BattleState.ATBTickCount;
            if (SpearCountdown < 0)
            {
                if (UseTranceSpear)
                    btl_cmd.SetCommand(freya.Data.cmd[1], BattleCommandId.SpearInTrance, (Int32)BattleAbilityId.Spear2, SpearTargetId, Comn.countBits(SpearTargetId) > 1 ? 1u : 0u);
                else
                    btl_cmd.SetCommand(freya.Data.cmd[1], BattleCommandId.Spear, (Int32)BattleAbilityId.Spear1, SpearTargetId, Comn.countBits(SpearTargetId) > 1 ? 1u : 0u);
            }
            return true;
        }
    }
}
