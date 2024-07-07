using System;
using UnityEngine;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Zombie)]
    public class ZombieStatusScript : StatusScriptBase
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            if (target.IsPlayer && !target.IsUnderAnyStatus(BattleStatus.Trance))
                target.Trance = 0;
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            return true;
        }
    }
}
