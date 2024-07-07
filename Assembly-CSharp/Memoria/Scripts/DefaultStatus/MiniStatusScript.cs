using System;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Mini)] // TODO MagicSword prevent
    public class MiniStatusScript : StatusScriptBase
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            if ((target.PermanentStatus & BattleStatus.Mini) != 0)
                return btl_stat.ALTER_INVALID;
            if ((target.CurrentStatus & BattleStatus.Mini) != 0)
            {
                btl_stat.RemoveStatus(target, BattleStatusId.Mini);
                return btl_stat.ALTER_SUCCESS_NO_SET;
            }
            target.Data.geo_scale_status *= 0.5f;
            geo.geoScaleUpdate(target, true);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            target.Data.geo_scale_status *= 2f;
            geo.geoScaleUpdate(target, true);
            return true;
        }
    }
}
