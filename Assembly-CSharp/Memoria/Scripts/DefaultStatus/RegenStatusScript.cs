using System;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Regen)]
    public class RegenStatusScript : StatusScriptBase, IOprStatusScript
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

        public IOprStatusScript.SetupOprMethod SetupOpr => null;
        public Boolean OnOpr()
        {
            if (Target.IsUnderAnyStatus(BattleStatus.Petrify))
                return false;
            UInt32 heal = Target.MaximumHp >> 4;
            Boolean isDmg = false;
            if (Target.IsUnderAnyStatus(BattleStatus.EasyKill))
                heal >>= 2;
            if (Target.IsZombie)
            {
                isDmg = true;
                if (Target.CurrentHp > heal)
                    Target.CurrentHp -= heal;
                else
                    Target.Kill(Inflicter);
            }
            else
            {
                Target.CurrentHp = Math.Min(Target.CurrentHp + heal, Target.MaximumHp);
            }
            btl2d.Btl2dStatReq(Target, isDmg ? (Int32)heal : -(Int32)heal, 0);
            BattleVoice.TriggerOnStatusChange(Target, "Used", BattleStatusId.Regen);
            return false;
        }
    }
}
