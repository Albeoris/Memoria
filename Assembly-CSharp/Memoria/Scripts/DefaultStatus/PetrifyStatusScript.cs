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
            base.Apply(target, inflicter, parameters);
            btl_stat.RemoveStatus(target, BattleStatusId.GradualPetrify);
            if (!btl_cmd.CheckUsingCommand(target.PetrifyCommand) && FF9StateSystem.Battle.FF9Battle.btl_phase > FF9StateBattleSystem.PHASE_ENTER && Configuration.Battle.Speed < 3)
            {
                btl_cmd.SetCommand(target.PetrifyCommand, BattleCommandId.SysStone, 0, target.Id, 0);
                return btl_stat.ALTER_SUCCESS_NO_SET;
            }
            target.CurrentAtb = 0;
            btl_cmd.KillSpecificCommand(target, BattleCommandId.SysStone);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove()
        {
            btl_cmd.KillSpecificCommand(Target, BattleCommandId.SysStone);
            return true;
        }
    }
}
