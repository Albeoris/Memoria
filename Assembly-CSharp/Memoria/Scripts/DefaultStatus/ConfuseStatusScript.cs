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
            if (target.IsMonsterTransform && target.Data.monster_transform.attack[target.Data.bi.def_idle] == null)
                return btl_stat.ALTER_RESIST;
            // TODO: check if that's OK
            target.AddDelayedModifier(KeepRotating, null);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            BTL_DATA btl = target.Data;
            Vector3 eulerAngles = btl.rot.eulerAngles;
            eulerAngles.y = btl.evt.rotBattle.eulerAngles.y;
            btl.rot = Quaternion.Euler(eulerAngles);
            btl_stat.StatusCommandCancel(btl);
            return true;
        }

        public Boolean OnATB(BattleUnit target)
        {
            if (target.IsMonsterTransform && target.Data.monster_transform.attack[target.Data.bi.def_idle] == null)
            {
                btl_stat.RemoveStatus(target, BattleStatusId.Confuse);
                return false;
            }
            if (target.IsPlayer)
                btl_cmd.SetCommand(target.Data.cmd[0], BattleCommandId.Attack, (Int32)BattleAbilityId.Attack, btl_util.GetRandomBtlID((UInt32)(Comn.random8() & 1)), 0u);
            else
                btl_cmd.SetEnemyCommand(target.Id, btl_util.GetRandomBtlID((UInt32)(Comn.random8() & 1)), BattleCommandId.EnemyAtk, target.EnemyType.p_atk_no);
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
                BattleVoice.TriggerOnStatusChange(unit, "Used", BattleStatusId.Confuse);
        }

        private Boolean KeepRotating(BattleUnit unit)
        {
            if (!unit.IsUnderAnyStatus(BattleStatus.Confuse))
                return false;
            if (FF9StateSystem.Battle.FF9Battle.btl_phase != 4)
                return true;
            if (btl_util.IsBtlUsingCommand(unit))
                return true;
            if (btl_mot.checkMotion(unit, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL) || btl_mot.checkMotion(unit, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING) || (unit.IsPlayer && btl_mot.checkMotion(unit, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD)))
            {
                Vector3 eulerAngles = unit.Data.rot.eulerAngles;
                unit.Data.rot.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y + 11.25f, eulerAngles.z);
            }
            return true;
        }
    }
}
