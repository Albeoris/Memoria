using System;
using UnityEngine;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Confuse)]
    public class ConfuseStatusScript : StatusScriptBase, IAutoAttackStatusScript
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            base.Apply(target, inflicter, parameters);
            if (!target.CanUseTheAttackCommand)
                return btl_stat.ALTER_RESIST;
            target.AddDelayedModifier(KeepRotating, null);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove()
        {
            Target.CurrentOrientationAngle = Target.DefaultOrientationAngle;
            btl_stat.StatusCommandCancel(Target);
            return true;
        }

        public Boolean OnATB()
        {
            if (!Target.CanUseTheAttackCommand)
            {
                btl_stat.RemoveStatus(Target, BattleStatusId.Confuse);
                return false;
            }
            if (Target.IsPlayer)
                btl_cmd.SetCommand(Target.ATBCommand, BattleCommandId.Attack, (Int32)BattleAbilityId.Attack, btl_util.GetRandomBtlID((UInt32)(Comn.random8() & 1)), 0u);
            else
                btl_cmd.SetEnemyCommand(Target, BattleCommandId.EnemyAtk, Target.EnemyType.p_atk_no, btl_util.GetRandomBtlID((UInt32)(Comn.random8() & 1)));
            if (Configuration.VoiceActing.Enabled)
                Target.AddDelayedModifier(WaitForAutoAttack, TriggerUsageForBattleVoice);
            return true;
        }

        private Boolean WaitForAutoAttack(BattleUnit unit)
        {
            return btl_cmd.CheckCommandQueued(unit.ATBCommand);
        }

        private void TriggerUsageForBattleVoice(BattleUnit unit)
        {
            if (unit.ATBCommand.ExecutionStep != command_mode_index.CMD_MODE_INSPECTION)
                BattleVoice.TriggerOnStatusChange(unit, "Used", BattleStatusId.Confuse);
        }

        private Boolean KeepRotating(BattleUnit unit)
        {
            if (!unit.IsUnderAnyStatus(BattleStatus.Confuse))
                return false;
            if (FF9StateSystem.Battle.FF9Battle.btl_phase != FF9StateBattleSystem.PHASE_NORMAL)
                return true;
            if (btl_util.IsBtlUsingCommand(unit))
                return true;
            if (btl_mot.checkMotion(unit, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL) || btl_mot.checkMotion(unit, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING) || (unit.IsPlayer && btl_mot.checkMotion(unit, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD)))
                unit.CurrentOrientationAngle += 11.25f;
            return true;
        }
    }
}
