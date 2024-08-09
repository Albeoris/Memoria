using System;
using Memoria.Data;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Slow)]
    public class SlowStatusScript : StatusScriptBase
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            base.Apply(target, inflicter, parameters);
            if (target.IsUnderStatus(BattleStatus.Haste))
            {
                if (btl_stat.RemoveStatus(target, BattleStatusId.Haste) == 2)
                    return btl_stat.ALTER_SUCCESS_NO_SET;
                return btl_stat.ALTER_RESIST;
            }
            btl_para.SetupATBCoef(target, btl_para.GetATBCoef() * 2 / 3);
            target.UISpriteATB = BattleHUD.ATEGray;
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove()
        {
            btl_para.SetupATBCoef(Target, btl_para.GetATBCoef());
            Target.UISpriteATB = Target.IsUnderAnyStatus(BattleStatus.Stop) ? BattleHUD.ATEGray : Target.IsUnderAnyStatus(BattleStatus.Haste) ? BattleHUD.ATEOrange : BattleHUD.ATENormal;
            return true;
        }
    }
}
