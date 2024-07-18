using System;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Poison)]
    public class PoisonStatusScript : StatusScriptBase, IOprStatusScript
    {
        public BattleUnit PoisonInflicter = null;

        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            PoisonInflicter = inflicter;
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
            UInt32 damage = 0;
            if (!target.IsUnderAnyStatus(BattleStatus.Petrify))
            {
                damage = target.MaximumHp >> 4;
                if (target.IsUnderAnyStatus(BattleStatus.EasyKill))
                    damage >>= 2;
                if (target.CurrentHp > damage)
                    target.CurrentHp -= damage;
                else
                    target.Kill(PoisonInflicter);
            }
            btl.fig_stat_info |= Param.FIG_STAT_INFO_POISON_HP;
            btl.fig_poison_hp = (Int32)damage;
            btl2d.Btl2dStatReq(target);
            BattleVoice.TriggerOnStatusChange(btl, "Used", BattleStatusId.Poison);
            return false;
        }
    }
}
