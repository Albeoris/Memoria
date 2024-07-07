using System;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    // NOTE: this status is still specifically handled by the game in many aspects
    // You cannot recycle it for a completly different effect

    [StatusScript(BattleStatusId.Death)]
    public class DeathStatusScript : StatusScriptBase
    {
        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            if (target.CurrentHp > 0)
            {
                target.Data.fig_info |= Param.FIG_INFO_DEATH;
                target.Kill(inflicter);
            }
            else
            {
                target.CurrentHp = 0;
            }
            target.CurrentAtb = 0;
            if (!btl_cmd.CheckUsingCommand(target.Data.cmd[2]))
            {
                if (!target.IsPlayer)
                {
                    if (target.Data.die_seq == 0)
                    {
                        if (target.IsSlave)
                            target.Data.die_seq = 5;
                        else if (target.Enemy.Data.info.die_atk == 0 || !btl_util.IsBtlBusy(target, btl_util.BusyMode.CASTER | btl_util.BusyMode.QUEUED_CASTER))
                            target.Data.die_seq = 1;
                    }
                    btl_sys.CheckForecastMenuOff(target);
                }
            }
            if ((target.CurrentStatus & BattleStatus.Trance) != 0 && btl_cmd.KillSpecificCommand(target, BattleCommandId.SysTrans))
            {
                btl_stat.RemoveStatus(target, BattleStatusId.Trance);
                target.Trance = Math.Min((Byte)254, target.Trance);
            }
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            BTL_DATA btl = target.Data;
            btl.die_seq = 0;
            //btl.bi.dmg_mot_f = 0;
            btl.bi.cmd_idle = 0;
            btl.bi.death_f = 0;
            btl.bi.stop_anim = 0;
            btl.escape_key = 0;
            btl.killer_track = null;
            if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE) || btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE))
            {
                GeoTexAnim.geoTexAnimPlay(btl.texanimptr, 2);
                if (btl.bi.player != 0)
                    GeoTexAnim.geoTexAnimPlay(btl.tranceTexanimptr, 2);
            }
            if (!btl_util.IsBtlUsingCommand(btl, out CMD_DATA cmd) || !btl_util.IsCommandDeclarable(cmd.cmd_no))
                btl.sel_mode = 0;
            foreach (BattleStatusId oprStatus in (target.PermanentStatus & BattleStatusConst.OprCount & BattleStatusId.Death.GetStatData().ClearOnApply).ToStatusList())
                btl_stat.SetOprStatusCount(target, oprStatus);
            return true;
        }
    }
}
