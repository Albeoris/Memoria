using System;
using Memoria.Data;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Defend)]
    public class DefendStatusScript : StatusScriptBase
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
    }
}
