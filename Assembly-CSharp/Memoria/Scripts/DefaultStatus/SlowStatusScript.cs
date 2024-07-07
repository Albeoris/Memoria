using System;
using UnityEngine;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Slow)]
    public class SlowStatusScript : StatusScriptBase
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            if (target.IsUnderStatus(BattleStatus.Haste))
            {
                if (btl_stat.RemoveStatus(target, BattleStatusId.Haste) == 2)
                    return btl_stat.ALTER_SUCCESS_NO_SET;
                return btl_stat.ALTER_RESIST;
            }
            btl_para.SetupATBCoef(target);
            target.Data.cur.at_coef = (SByte)(target.Data.cur.at_coef * 2 / 3);
            target.UISpriteATB = BattleHUD.ATEGray;
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
