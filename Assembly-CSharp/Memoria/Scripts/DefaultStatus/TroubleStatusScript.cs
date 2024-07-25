using System;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Trouble)]
    public class TroubleStatusScript : StatusScriptBase, ITroubleStatusScript
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            base.Apply(target, inflicter, parameters);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove()
        {
            return true;
        }

        public void OnTroubleDamage(UInt16 fig_info, Int32 fig, Int32 m_fig)
        {
            if ((fig_info & Param.FIG_INFO_DISP_HP) == 0)
                return;
            if ((fig_info & (Param.FIG_INFO_HP_RECOVER | Param.FIG_INFO_GUARD | Param.FIG_INFO_MISS | Param.FIG_INFO_DEATH)) != 0)
                return;
            Int32 dmg = fig >> 1;
            foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
            {
                if (unit.IsPlayer == Target.IsPlayer && unit.Id != Target.Id && unit.IsTargetable)
                {
                    btl_para.SetDamage(unit, dmg, 0, requestFigureNow: true);
                    BattleVoice.TriggerOnStatusChange(Target, "Used", BattleStatusId.Trouble);
                }
            }
        }
    }
}
