using System;
using UnityEngine;
using Memoria.Data;
using FF9;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.GradualPetrify)]
    public class GradualPetrifyStatusScript : StatusScriptBase, IOprStatusScript
    {
        public HUDMessageChild Message = null;
        public BattleUnit GPInflicter = null;
        public Int32 InitialCounter;
        public Int32 Counter;

        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            btl2d.GetIconPosition(target, 5, out Transform attachTransf, out Vector3 iconOff);
            GPInflicter = inflicter;
            InitialCounter = parameters.Length > 0 ? (Int32)parameters[0] : 10;
            Counter = InitialCounter;
            Message = Singleton<HUDMessage>.Instance.Show(attachTransf, $"{Counter}", HUDMessage.MessageStyle.DEATH_SENTENCE, new Vector3(0f, iconOff.y), 0);
            btl2d.StatusMessages.Add(Message);
            UpdateLabel();
            return btl_stat.ALTER_SUCCESS;
        }

        public override Boolean Remove(BattleUnit target)
        {
            // TODO?
            btl_cmd.KillSpecificCommand(target, BattleCommandId.SysStone);
            btl2d.StatusMessages.Remove(Message);
            Singleton<HUDMessage>.Instance.ReleaseObject(Message);
            return true;
        }

        public IOprStatusScript.SetupOprMethod SetupOpr => SetupGradualPetrifyOpr;
        public Int32 SetupGradualPetrifyOpr(BattleUnit target)
        {
            // Use the duration "ContiCnt" of GradualPetrify even if it is not registered as BattleStatusConst.ContiCount
            return (Int32)(target.StatusDurationFactor[BattleStatusId.GradualPetrify] * BattleStatusId.GradualPetrify.GetStatData().ContiCnt * (60 - target.Will << 3) / 10);
        }

        public Boolean OnOpr(BattleUnit target)
        {
            Counter--;
            if (Counter > 0)
            {
                UpdateLabel();
                return false;
            }
            BTL_DATA btl = target;
            if (!btl_cmd.CheckUsingCommand(btl.cmd[2]))
            {
                UInt32 tryPetrify = btl_stat.AlterStatus(target, BattleStatusId.Petrify, GPInflicter);
                if (tryPetrify == btl_stat.ALTER_SUCCESS || tryPetrify == btl_stat.ALTER_SUCCESS_NO_SET)
                    BattleVoice.TriggerOnStatusChange(btl, "Used", BattleStatusId.GradualPetrify);
                else
                    btl2d.Btl2dReq(btl, Param.FIG_INFO_MISS, 0, 0);
            }
            return true;
        }

        private void UpdateLabel()
        {
            Byte intensity = (Byte)Mathf.Lerp(16f, 176f, (Single)Counter / InitialCounter);
            Int32 color = intensity << 16 | intensity << 8 | intensity;
            Message.Label = $"[{color:X6}]{Counter}";
            Message.gameObject.SetActive(true);
        }
    }
}
