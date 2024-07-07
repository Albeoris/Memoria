using System;
using UnityEngine;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.Doom)]
    public class DoomStatusScript : StatusScriptBase, IOprStatusScript
    {
        public HUDMessageChild Message = null;
        public BattleUnit DoomInflicter = null;
        public Int32 Counter;

        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            btl2d.GetIconPosition(target, 5, out Transform attachTransf, out Vector3 iconOff);
            DoomInflicter = inflicter;
            Counter = parameters.Length > 0 ? (Int32)parameters[0] : 10;
            Message = Singleton<HUDMessage>.Instance.Show(attachTransf, $"{Counter}", HUDMessage.MessageStyle.DEATH_SENTENCE, new Vector3(0f, iconOff.y), 0);
            btl2d.StatusMessages.Add(Message);
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            btl2d.StatusMessages.Remove(Message);
            Singleton<HUDMessage>.Instance.ReleaseObject(Message);
            return true;
        }

        public IOprStatusScript.SetupOprMethod SetupOpr => SetupDoomOpr;
        public Int32 SetupDoomOpr(BattleUnit target)
        {
            // Use the duration "ContiCnt" of Doom even if it is not registered as BattleStatusConst.ContiCount
            return BattleStatusId.Doom.GetStatData().ContiCnt * (60 - target.Will << 3) / 10;
        }

        public Boolean OnOpr(BattleUnit target)
        {
            Counter--;
            if (Counter > 0)
            {
                Message.Label = $"{Counter}";
                return false;
            }
            BattleVoice.TriggerOnStatusChange(target, "Used", BattleStatusId.Doom);
            btl_stat.AlterStatus(target, BattleStatusId.Death, DoomInflicter);
            btl2d.Btl2dReq(target);
            return true;
        }
    }
}
