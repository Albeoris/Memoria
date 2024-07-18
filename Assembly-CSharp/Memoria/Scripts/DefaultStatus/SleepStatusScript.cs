using System;
using Memoria.Data;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Sleep)]
    public class SleepStatusScript : StatusScriptBase
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            // TODO: check what it does (maybe change the "hit" animation to a wakeup animation)
            //if (target.IsPlayer && !target.IsMonsterTransform)
            //{
            //    target.Data.bi.def_idle = 1;
            //    btl_mot.SetDefaultIdle(target);
            //}
            return true;
        }
    }
}
