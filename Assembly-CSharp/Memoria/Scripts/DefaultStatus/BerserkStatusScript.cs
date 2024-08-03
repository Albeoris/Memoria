using System;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Berserk)]
    public class BerserkStatusScript : StatusScriptBase, IAutoAttackStatusScript
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            base.Apply(target, inflicter, parameters);
            if (!target.CanUseTheAttackCommand)
                return btl_stat.ALTER_RESIST;
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove()
        {
            btl_stat.StatusCommandCancel(Target);
            return true;
        }

        public Boolean OnATB()
        {
            if (!Target.CanUseTheAttackCommand)
            {
                btl_stat.RemoveStatus(Target, BattleStatusId.Berserk);
                return false;
            }
            if (Target.IsPlayer)
                btl_cmd.SetCommand(Target.ATBCommand, BattleCommandId.Attack, (Int32)BattleAbilityId.Attack, btl_util.GetRandomBtlID(0), 0u);
            else
                btl_cmd.SetEnemyCommand(Target, BattleCommandId.EnemyAtk, Target.EnemyType.p_atk_no, btl_util.GetRandomBtlID(1));
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
                BattleVoice.TriggerOnStatusChange(unit, "Used", BattleStatusId.Berserk);
        }
    }
}
