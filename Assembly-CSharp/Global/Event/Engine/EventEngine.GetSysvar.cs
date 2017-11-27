using System;
using FF9;
using UnityEngine;

public partial class EventEngine
{
    public Int32 GetSysvar(Int32 code)
    {
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        Int32 num;
        switch (code)
        {
            case 0:
                num = Comn.random8();
                break;
            case 1:
                num = this.fieldmap.GetCurrentCameraIndex();
                break;
            case 2:
                num = (Int32)this._context.usercontrol;
                break;
            case 3:
                num = FF9Snd.ff9fldsnd_sync();
                break;
            case 4:
                num = EventCollision.sSysAngle;
                break;
            case 5:
                Obj sender = this.getSender(this.gExec);
                num = -1;
                if (sender != null)
                {
                    num = (Int32)sender.sid - (this.sSourceObjN - 9);
                    break;
                }
                break;
            case 6:
                num = (Int32)this._ff9.party.gil;
                break;
            case 7:
                num = FF9StateSystem.EventState.gStepCount;
                break;
            case 8:
                num = ETb.gMesSignal;
                break;
            case 9:
                num = this.eTb.GetChoose();
                break;
            case 10:
                num = this.sMapJumpX;
                break;
            case 11:
                num = this.sMapJumpZ;
                break;
            case 12:
                num = this.sSysX;
                break;
            case 13:
                num = this.sSysY;
                break;
            case 14:
                num = fldfmv.FF9FieldFMVGetFrame();
                break;
            case 15:
                num = fldfmv.FF9FieldFMVSync();
                break;
            case 16:
                num = this._ff9.Frogs.Number;
                break;
            case 17:
                num = Convert.ToInt32(TimerUI.Time);
                break;
            case 18:
                num = QuadMistDatabase.MiniGame_GetLastBattleResult();
                break;
            case 19:
                num = FF9StateSystem.MiniGame.GetNumberOfCards();
                //Debug.Log((object)("num of cards = " + (object)num));
                break;
            case 20:
                num = Convert.ToInt32(FF9StateSystem.Settings.time);
                if (num > 8388607)
                {
                    num = 8388607;
                    break;
                }
                break;
            case 21:
                num = !TimerUI.Enable ? 0 : 1;
                break;
            case 22:
                num = this.sSEPos;
                break;
            case 23:
                num = this.sSEVol;
                break;
            case 24:
                num = (Int32)this._ff9.btlMapNo;
                break;
            case 25:
                num = !btlseq.BtlSeqBusy() ? 0 : 1;
                break;
            case 26:
                num = this.gMode != 4 ? 0 : 1;
                break;
            case 27:
                num = (Int32)this._ff9.btl_result;
                break;
            case 28:
                num = EventEngine._btlCmdPrm >> 8 & (Int32)Byte.MaxValue;
                break;
            case 29:
                num = EventEngine._btlCmdPrm & (Int32)Byte.MaxValue;
                break;
            case 30:
                num = (Int32)stateBattleSystem.btl_phase;
                break;
            case 31:
                num = (Int32)stateBattleSystem.btl_scene.PatNum;
                break;
            default:
                //if ((Int32)this.gCur.sid != 3 || this.gCur.ip != 791)
                //    ;
                num = code < 192 ? (Int32)btl_scrp.GetBattleData(code) : ff9.w_frameGetParameter(code);
                break;
        }
        return num;
    }
}