using System;
using UnityEngine;
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
            if (target.IsMonsterTransform && target.Data.monster_transform.attack[target.Data.bi.def_idle] == null)
                return btl_stat.ALTER_RESIST;
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            btl_stat.StatusCommandCancel(target);
            return true;
        }

        public Boolean OnATB(BattleUnit target)
        {
            if (target.IsMonsterTransform && target.Data.monster_transform.attack[target.Data.bi.def_idle] == null)
            {
                btl_stat.RemoveStatus(target, BattleStatusId.Berserk);
                return false;
            }
            if (target.IsPlayer)
                btl_cmd.SetCommand(target.Data.cmd[0], BattleCommandId.Attack, (Int32)BattleAbilityId.Attack, btl_util.GetRandomBtlID(0), 0u);
            else
                btl_cmd.SetEnemyCommand(target.Id, btl_util.GetRandomBtlID(1), BattleCommandId.EnemyAtk, target.EnemyType.p_atk_no);
            if (Configuration.VoiceActing.Enabled)
                target.AddDelayedModifier(WaitForAutoAttack, TriggerUsageForBattleVoice);
            return true;
        }

        private Boolean WaitForAutoAttack(BattleUnit unit)
        {
            return btl_cmd.CheckCommandQueued(unit.Data.cmd[0]);
        }

        private void TriggerUsageForBattleVoice(BattleUnit unit)
        {
            if (unit.Data.cmd[0].info.mode != command_mode_index.CMD_MODE_INSPECTION)
                BattleVoice.TriggerOnStatusChange(unit, "Used", BattleStatusId.Berserk);
        }
    }
}
