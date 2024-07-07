using System;
using Memoria.Data;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Petrify)]
    public class PetrifyStatusScript : StatusScriptBase
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            BTL_DATA btl = target;
            btl_stat.RemoveStatus(target, BattleStatusId.GradualPetrify);
            if (!btl_cmd.CheckUsingCommand(btl.cmd[2]) && FF9StateSystem.Battle.FF9Battle.btl_phase > 2 && Configuration.Battle.Speed < 3)
            {
                btl_cmd.SetCommand(btl.cmd[2], BattleCommandId.SysStone, 0, btl.btl_id, 0);
                return btl_stat.ALTER_SUCCESS_NO_SET;
            }
            btl.bi.atb = 0;
            target.CurrentAtb = 0;
            btl_cmd.KillSpecificCommand(target, BattleCommandId.SysStone);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            btl_cmd.KillSpecificCommand(target, BattleCommandId.SysStone);
            return true;
        }
    }
}
