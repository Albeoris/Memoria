using FF9;
using Memoria;
using Memoria.Data;
using System;

public partial class EventEngine
{
    public Int32 GetSysvar(Int32 code)
    {
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        switch (code)
        {
            case 0:
                return Comn.random8();
            case 1:
                return this.fieldmap.camIdx;
            case 2:
                return this._context.usercontrol;
            case 3:
                return FF9Snd.ff9fldsnd_sync();
            case 4:
                return EventCollision.sSysAngle;
            case 5:
            {
                Obj sender = this.getSender(this.gExec);
                if (sender != null)
                    return sender.sid - (this.sSourceObjN - 9);
                else
                    return -1;
            }
            case 6:
                return (Int32)this._ff9.party.gil;
            case 7:
                return FF9StateSystem.EventState.gStepCount;
            case 8:
                return ETb.gMesSignal;
            case 9:
                return ETb.GetChoose();
            case 10:
                return this.sMapJumpX;
            case 11:
                return this.sMapJumpZ;
            case 12:
                return this.sSysX;
            case 13:
                return this.sSysY;
            case 14:
                return fldfmv.FF9FieldFMVGetFrame();
            case 15:
                return fldfmv.FF9FieldFMVSync();
            case 16:
                return this._ff9.Frogs.Number;
            case 17:
                return Convert.ToInt32(TimerUI.Time);
            case 18:
                return QuadMistDatabase.MiniGame_GetLastBattleResult();
            case 19:
            {
                Int32 cardCount = FF9StateSystem.MiniGame.GetNumberOfCards();
                // Hotfix: in non-modded scripts, number of cards are retrieved either:
                // - in order to check if the player has at least 5 (for playing with NPCs)
                // - in order to check if the player has less than 100 (for preventing a card pickup)
                // - in very special cases (Ticketmaster gifts / finding Hippaul secret cards), in order to check if the player has less than 96 or 98
                // So we use "Min(95, CardCount)" here except when the card count approaches the MaxCardCount
                if (Configuration.TetraMaster.MaxCardCount != 100)
                    return cardCount + 4 >= Configuration.TetraMaster.MaxCardCount ? 100 - (Configuration.TetraMaster.MaxCardCount - cardCount) : Math.Min(cardCount, 95);
                //Debug.Log((object)("num of cards = " + (object)num));
                return cardCount;
            }
            case 20:
                if (Configuration.Hacks.ExcaliburIINoTimeLimit && FF9StateSystem.Common.FF9.fldMapNo == 2919)
                    return 0;
                return Math.Min(Convert.ToInt32(FF9StateSystem.Settings.time), 8388607);
            case 21:
                return TimerUI.Enable ? 1 : 0;
            case 22:
                return this.sSEPos;
            case 23:
                return this.sSEVol;
            case 24:
                return this._ff9.btlMapNo;
            case 25:
                return btlseq.BtlSeqBusy() ? 1 : 0;
            case 26:
                return this.gMode == 4 ? 1 : 0;
            case 27:
                return this._ff9.btl_result;
            case 28:
                if (this.gExec.level <= 2)
                    return EventEngine._btlCmdPrmCmd;
                else
                    return (Int32)(btl_scrp.GetCurrentCommandSmart(btl_scrp.FindBattleUnitUnlimited((UInt16)this.GetSysList(1))?.Data)?.cmd_no ?? BattleCommandId.None);
            case 29:
                if (this.gExec.level <= 2)
                    return EventEngine._btlCmdPrmSub;
                else
                    return btl_scrp.GetCurrentCommandSmart(btl_scrp.FindBattleUnitUnlimited((UInt16)this.GetSysList(1))?.Data)?.sub_no ?? 0;
            case 30:
                return stateBattleSystem.btl_phase;
            case 31:
                return stateBattleSystem.btl_scene.PatNum;
            case 191: // GetData_191: return current field map ID
                return FF9StateSystem.Common.FF9.fldMapNo;
        }
        return code < 192 ? (Int32)btl_scrp.GetBattleData(code) : ff9.w_frameGetParameter(code);
    }
}
