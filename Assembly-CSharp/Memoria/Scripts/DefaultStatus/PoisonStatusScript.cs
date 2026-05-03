using System;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Poison)]
    public class PoisonStatusScript : StatusScriptBase, IOprStatusScript
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
            UInt32 damage = Target.MaximumHp >> 4;
            if (Target.IsUnderAnyStatus(BattleStatus.EasyKill))
                damage >>= 2;
            if (Target.CurrentHp > damage)
                Target.CurrentHp -= damage;
            else
                Target.Kill(Inflicter);
            btl2d.Btl2dStatReq(Target, (Int32)damage, 0);
            BattleVoice.TriggerOnStatusChange(Target, BattleVoice.BattleMoment.Used, BattleStatusId.Poison);
            return false;
        }
    }
}
