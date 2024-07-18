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
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            return true;
        }

        public void OnTroubleDamage(BattleUnit target, UInt16 fig_info, Int32 fig, Int32 m_fig)
        {
            if ((fig_info & Param.FIG_INFO_DISP_HP) == 0)
                return;
            if ((fig_info & (Param.FIG_INFO_HP_RECOVER | Param.FIG_INFO_GUARD | Param.FIG_INFO_MISS | Param.FIG_INFO_DEATH)) != 0)
                return;
            Int32 dmg = fig >> 1;
            foreach (BattleUnit next in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
            {
                if (next.IsPlayer == target.IsPlayer && next.Id != target.Id && next.IsTargetable)
                {
                    next.Data.fig_info = Param.FIG_INFO_DISP_HP;
                    btl_para.SetDamage(next, dmg, 0);
                    btl2d.Btl2dReq(next.Data, Param.FIG_INFO_DISP_HP, dmg, 0);
                    BattleVoice.TriggerOnStatusChange(next.Data, "Used", BattleStatusId.Trouble);
                }
            }
        }
    }
}
