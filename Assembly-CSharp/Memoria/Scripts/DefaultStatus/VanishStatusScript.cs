using System;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    // NOTE: this status is still specifically handled by the game in some minor aspects
    // You shouldn't recycle it for a completly different effect

    [StatusScript(BattleStatusId.Vanish)]
    public class VanishStatusScript : StatusScriptBase
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            base.Apply(target, inflicter, parameters);
            btl_mot.HideMesh(target, target.Data.mesh_banish, true);
            target.AddDelayedModifier(KeepVanishHidden, null);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove()
        {
            btl_mot.ShowMesh(Target, Target.Data.mesh_banish, true);
            return true;
        }

        private Boolean KeepVanishHidden(BattleUnit unit)
        {
            if (!unit.IsUnderAnyStatus(BattleStatus.Vanish))
                return false;
            if (unit.IsDisappear)
                return true;
            btl_mot.HideMesh(unit, unit.Data.mesh_banish, true);
            return true;
        }
    }
}
