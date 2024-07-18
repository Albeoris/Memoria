using System;
using Memoria.Data;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Haste)]
    public class HasteStatusScript : StatusScriptBase
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            if (target.IsUnderStatus(BattleStatus.Slow))
            {
                if (btl_stat.RemoveStatus(target, BattleStatusId.Slow) == 2)
                    return btl_stat.ALTER_SUCCESS_NO_SET;
                return btl_stat.ALTER_RESIST;
            }
            btl_para.SetupATBCoef(target);
            target.Data.cur.at_coef = (SByte)(target.Data.cur.at_coef * 3 / 2);
            target.UISpriteATB = BattleHUD.ATEOrange;
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            btl_para.SetupATBCoef(target);
            target.UISpriteATB = BattleHUD.ATENormal;
            return true;
        }
    }
}
