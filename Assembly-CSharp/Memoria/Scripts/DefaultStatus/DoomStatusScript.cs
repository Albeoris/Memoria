using System;
using UnityEngine;
using Memoria.Data;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Doom)]
    public class DoomStatusScript : StatusScriptBase, IOprStatusScript
    {
        public HUDMessageChild Message = null;
        public Int32 InitialCounter;
        public Int32 Counter;

        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            base.Apply(target, inflicter, parameters);
            InitialCounter = parameters.Length > 0 ? Convert.ToInt32(parameters[0]) : 10;
            Counter = InitialCounter;
            Message = Singleton<HUDMessage>.Instance.Show(target, btl2d.ICON_POS_NUMBER, $"{Counter}", HUDMessage.MessageStyle.DEATH_SENTENCE);
            btl2d.StatusMessages.Add(Message);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove()
        {
            btl2d.StatusMessages.Remove(Message);
            Singleton<HUDMessage>.Instance.ReleaseObject(Message);
            return true;
        }

        public IOprStatusScript.SetupOprMethod SetupOpr => SetupDoomOpr;
        public Int32 SetupDoomOpr()
        {
            // Use the duration "ContiCnt" of Doom even if it is not registered as BattleStatusConst.ContiCount
            return (Int32)(Target.StatusDurationFactor[BattleStatusId.Doom] * BattleStatusId.Doom.GetStatData().ContiCnt * (60 - Target.Will << 3) / 10);
        }

        public Boolean OnOpr()
        {
            Counter--;
            if (Counter > 0)
            {
                Message.Label = $"{Counter}";
                return false;
            }
            if (btl_stat.AlterStatus(Target, BattleStatusId.Death, Inflicter) == btl_stat.ALTER_SUCCESS)
                BattleVoice.TriggerOnStatusChange(Target, "Used", BattleStatusId.Doom);
            btl2d.Btl2dReq(Target);
            return true;
        }
    }
}
