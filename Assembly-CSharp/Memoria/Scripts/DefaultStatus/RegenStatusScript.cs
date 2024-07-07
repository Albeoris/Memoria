using System;
using UnityEngine;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Regen)]
    public class RegenStatusScript : StatusScriptBase, IOprStatusScript
    {
        public BattleUnit RegenInflicter = null;

        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            RegenInflicter = inflicter;
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            return true;
        }

        public IOprStatusScript.SetupOprMethod SetupOpr => null;
        public Boolean OnOpr(BattleUnit target)
        {
            BTL_DATA btl = target;
            UInt32 heal = 0;
            if (!target.IsUnderAnyStatus(BattleStatus.Petrify))
            {
                heal = target.MaximumHp >> 4;
                if (target.IsUnderAnyStatus(BattleStatus.EasyKill))
                    heal >>= 2;
                if (btl_stat.CheckStatus(btl, BattleStatus.Zombie) || btl_util.CheckEnemyCategory(btl, 16))
                {
                    btl.fig_stat_info |= Param.FIG_STAT_INFO_REGENE_DMG;
                    if (target.CurrentHp > heal)
                        target.CurrentHp -= heal;
                    else
                        target.Kill(RegenInflicter);
                }
                else
                {
                    target.CurrentHp = Math.Min(target.CurrentHp + heal, target.MaximumHp);
                }
            }
            btl.fig_stat_info |= Param.FIG_STAT_INFO_REGENE_HP;
            btl.fig_regene_hp = (Int32)heal;
            btl2d.Btl2dStatReq(target);
            BattleVoice.TriggerOnStatusChange(btl, "Used", BattleStatusId.Regen);
            return false;
        }
    }
}
