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
        public BattleCommandId SpearCommandId;
        public BattleAbilityId SpearAbilityId;
        public UInt16 SpearTargetId;
        public Int32 SpearCountdown;

        private Boolean CurrentlyUseSpear = false;
        private Boolean InitialisedModifier = false;

        public static Int32 GetJumpDuration(BattleUnit btl)
        {
            // Use the duration "ContiCnt" of Jump even if it is not registered as BattleStatusConst.ContiCount
            return (Int32)(btl.StatusDurationFactor[BattleStatusId.Jump] * BattleStatusId.Jump.GetStatData().ContiCnt * 4 * (60 - btl.Will));
        }

        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            base.Apply(target, inflicter, parameters);
            SpearCountdown = GetJumpDuration(target);
            SpearCommandId = parameters.Length > 0 ? (BattleCommandId)parameters[0] : BattleCommandId.Spear;
            SpearAbilityId = parameters.Length > 1 ? (BattleAbilityId)parameters[1] : BattleAbilityId.Spear1;
            SpearTargetId = parameters.Length > 2 ? (UInt16)parameters[2] : btl_util.GetRandomBtlID(0);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove()
        {
            BTL_DATA btl = Target.Data;
            btl.tar_mode = 3;
            if (btl.bi.player != 0 && !FF9StateSystem.Settings.IsATBFull)
                btl.cur.at = 0;
            btl.sel_mode = 0;
            return true;
        }

        public void OnFinishCommand(CMD_DATA cmd, Int32 tranceDecrease)
        {
            switch (cmd.cmd_no)
            {
                case BattleCommandId.Jump:
                case BattleCommandId.JumpInTrance:
                    if (!InitialisedModifier)
                    {
                        BTL_DATA btl = Target.Data;
                        btl.tar_mode = 2;
                        btl.SetDisappear(true, 2);
                        Target.AddDelayedModifier(ProcessJumpSpear, null);
                        InitialisedModifier = true;
                    }
                    break;
                case BattleCommandId.Spear:
                    Target.RemoveStatus(BattleStatus.Jump);
                    break;
                case BattleCommandId.SpearInTrance:
                    SpearCountdown = GetJumpDuration(Target);
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
            if (!BattleState.IsATBEnabled || btl_cmd.CheckUsingCommand(freya.Data.cmd[1]))
                return true;
            SpearCountdown -= freya.Data.cur.at_coef * BattleState.ATBTickCount;
            if (SpearCountdown < 0)
                btl_cmd.SetCommand(freya.Data.cmd[1], SpearCommandId, (Int32)SpearAbilityId, SpearTargetId, Comn.countBits(SpearTargetId) > 1 ? 1u : 0u);
            return true;
        }
    }
}
