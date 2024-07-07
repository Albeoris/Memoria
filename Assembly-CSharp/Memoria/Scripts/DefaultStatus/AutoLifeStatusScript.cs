using System;
using Memoria.Data;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.AutoLife)]
    public class AutoLifeStatusScript : StatusScriptBase, IDeathChangerStatusScript
    {
        public UInt32 HPRestore = 1;

        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            HPRestore = Math.Max(HPRestore, parameters.Length > 0 ? (UInt32)parameters[0] : 1);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            return true;
        }

        public Boolean OnDeath(BattleUnit target)
        {
            btl_stat.RemoveStatus(target, BattleStatusId.AutoLife);
            if (HPRestore > 0)
            {
                target.CurrentHp = HPRestore;
                btl_stat.RemoveStatus(target, BattleStatusId.Death);
            }
            BattleVoice.TriggerOnStatusChange(target, "Used", BattleStatusId.AutoLife);
            return true;
        }
    }
}
