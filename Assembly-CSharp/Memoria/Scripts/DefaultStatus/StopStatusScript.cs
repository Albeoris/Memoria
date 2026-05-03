using System;
using Memoria.Data;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Stop)]
    public class StopStatusScript : StatusScriptBase
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            base.Apply(target, inflicter, parameters);
            target.UISpriteATB = BattleHUD.ATEGray;
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove()
        {
            Target.UISpriteATB = Target.IsUnderAnyStatus(BattleStatus.Slow) ? BattleHUD.ATEGray : Target.IsUnderAnyStatus(BattleStatus.Haste) ? BattleHUD.ATEOrange : BattleHUD.ATENormal;
            return true;
        }
    }
}
