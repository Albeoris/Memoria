using System;
using UnityEngine;
using Memoria.Data;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Float)]
    public class FloatStatusScript : StatusScriptBase
    {
        public Boolean CurrentlyDodging = false;

        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            target.AddDelayedModifier(HoveringMovement, null);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            target.Data.pos[1] = 0f;
            target.Data.base_pos[1] = 0f;
            return true;
        }

        private Boolean HoveringMovement(BattleUnit unit)
        {
            if (!unit.IsUnderAnyStatus(BattleStatus.Float))
                return false;
            if (!unit.IsUnderPermanentStatus(BattleStatus.Float) || unit.IsNonMorphedPlayer)
            {
                Single y = 200 + (Int32)(30 * ff9.rsin((FF9StateSystem.Battle.FF9Battle.btl_cnt & 15) << 8) / 4096f);
                Vector3 pos = unit.Data.base_pos;
                pos.y = y;
                unit.Data.base_pos = pos;
                pos = unit.Data.pos;
                pos.y = y;
                unit.Data.pos = pos;
            }
            if (Configuration.Battle.FloatEvadeBonus > 0 && !unit.IsPlayer && unit.IsUnderPermanentStatus(BattleStatus.Float) && !unit.IsSlave)
            {
                if (unit.IsDodged && !CurrentlyDodging)
                {
                    unit.Data.pos[1] -= -600f;
                    CurrentlyDodging = true;
                }
                else if (CurrentlyDodging && !unit.IsDodged)
                {
                    unit.Data.pos[1] = unit.Data.base_pos[1];
                    CurrentlyDodging = false;
                }
            }
            return true;
        }
    }
}
