﻿using FF9;
using Memoria;
using Memoria.Data;
using System;

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
                num = this.fieldmap.camIdx;
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
                // Hotfix: in non-modded scripts, number of cards are retrieved either:
                // - in order to check if the player has at least 5 (for playing with NPCs)
                // - in order to check if the player has less than 100 (for preventing a card pickup)
                // - in very special cases (Ticketmaster gifts / finding Hippaul secret cards), in order to check if the player has less than 96 or 98
                // So we use "Min(95, CardCount)" here except when the card count approaches the MaxCardCount
                if (Configuration.TetraMaster.MaxCardCount != 100)
                    num = num + 4 >= Configuration.TetraMaster.MaxCardCount ? 100 - (Configuration.TetraMaster.MaxCardCount - num) : Math.Min(num, 95);
                //Debug.Log((object)("num of cards = " + (object)num));
                break;
            case 20:
                num = Convert.ToInt32(FF9StateSystem.Settings.time);
                if (Configuration.Hacks.ExcaliburIINoTimeLimit && FF9StateSystem.Common.FF9.fldMapNo == 2919)
                {
                    num = 0;
                    break;
                }
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
                if (this.gExec.level <= 2)
                    num = EventEngine._btlCmdPrmCmd;
                else
                    num = (Int32)(btl_scrp.GetCurrentCommandSmart(btl_scrp.FindBattleUnitUnlimited((UInt16)this.GetSysList(1))?.Data)?.cmd_no ?? BattleCommandId.None);
                break;
            case 29:
                if (this.gExec.level <= 2)
                    num = EventEngine._btlCmdPrmSub;
                else
                    num = btl_scrp.GetCurrentCommandSmart(btl_scrp.FindBattleUnitUnlimited((UInt16)this.GetSysList(1))?.Data)?.sub_no ?? 0;
                break;
            case 30:
                num = (Int32)stateBattleSystem.btl_phase;
                break;
            case 31:
                num = (Int32)stateBattleSystem.btl_scene.PatNum;
                break;
            case 191: // GetData_191: return current field map ID
                num = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
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
