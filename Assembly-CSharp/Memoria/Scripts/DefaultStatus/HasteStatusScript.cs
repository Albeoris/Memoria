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
            base.Apply(target, inflicter, parameters);
            if (target.IsUnderStatus(BattleStatus.Slow))
            {
                if (btl_stat.RemoveStatus(target, BattleStatusId.Slow) == 2)
                    return btl_stat.ALTER_SUCCESS_NO_SET;
                return btl_stat.ALTER_RESIST;
            }
            btl_para.SetupATBCoef(target, btl_para.GetATBCoef() * 3 / 2);
            target.UISpriteATB = BattleHUD.ATEOrange;
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove()
        {
            btl_para.SetupATBCoef(Target, btl_para.GetATBCoef());
            Target.UISpriteATB = BattleHUD.ATENormal;
            return true;
        }
    }
}
