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
            case 0: // GetRandom
                return Comn.random8();
            case 1: // GetFieldCamera
                return this.fieldmap.camIdx;
            case 2: // IsMovementEnabled
                return this._context.usercontrol;
            case 3: // SyncSounds
                return FF9Snd.ff9fldsnd_sync();
            case 4: // GetCollisionAngle
                return EventCollision.sSysAngle;
            case 5: // GetScriptCharacter
            {
                Obj sender = this.getSender(this.gExec);
                if (sender != null)
                    return (Int32)this.sObjTable[sender.sid].player_link;
                else
                    return (Int32)this.sObjTable[this.gExec.sid].player_link;
            }
            case 6: // GetGil
                return (Int32)this._ff9.party.gil;
            case 7: // GetTotalSteps
                return FF9StateSystem.EventState.gStepCount;
            case 8: // GetDialogProgression
                return ETb.gMesSignal;
            case 9: // GetDialogChoice
                return ETb.GetChoose();
            case 10: // GetFieldExitX
                return this.sMapJumpX;
            case 11: // GetFieldExitY
                return this.sMapJumpZ;
            case 12: // GetScreenCalculatedX
                return this.sSysX;
            case 13: // GetScreenCalculatedY
                return this.sSysY;
            case 14: // GetCinematicFrame
                return fldfmv.FF9FieldFMVGetFrame();
            case 15: // SyncCinematic
                return fldfmv.FF9FieldFMVSync();
            case 16: // GetFrogAmount
                return this._ff9.Frogs.Number;
            case 17: // GetTimerTime
                return Convert.ToInt32(TimerUI.Time);
            case 18: // GetTetraMasterResult
                return QuadMistDatabase.MiniGame_GetLastBattleResult();
            case 19: // GetCardAmount
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
            case 20: // GetTime
                if (Configuration.Hacks.ExcaliburIINoTimeLimit && FF9StateSystem.Common.FF9.fldMapNo == 2919)
                    return 0;
                return Math.Min(Convert.ToInt32(FF9StateSystem.Settings.time), 8388607);
            case 21: // IsTimerShown
                return TimerUI.Enable ? 1 : 0;
            case 22: // GetSoundDistance
                return this.sSEPos;
            case 23: // GetSoundVolume
                return this.sSEVol;
            case 24: // GetLastBattleId
                return this._ff9.btlMapNo;
            case 25: // IsAttacking
                return btlseq.BtlSeqBusy() ? 1 : 0;
            case 26: // IsBattleInitialized (should rather be "IsBattleResultScreen" or something like that; used by the call of Main_Init at the end of some battles)
                return this.gMode == 4 ? 1 : 0;
            case 27: // GetBattleResult
                return this._ff9.btl_result;
            case 28: // GetAttackCommandId
                if (this.gExec.level <= 2)
                    return EventEngine._btlCmdPrmCmd;
                else
                    return (Int32)(btl_scrp.GetCurrentCommandSmart(btl_scrp.FindBattleUnitUnlimited((UInt16)this.GetSysList(1))?.Data)?.cmd_no ?? BattleCommandId.None);
            case 29: // GetAttackId
                if (this.gExec.level <= 2)
                    return EventEngine._btlCmdPrmSub;
                else
                    return btl_scrp.GetCurrentCommandSmart(btl_scrp.FindBattleUnitUnlimited((UInt16)this.GetSysList(1))?.Data)?.sub_no ?? 0;
            case 30: // GetBattleState
                return stateBattleSystem.btl_phase;
            case 31: // GetBattleGroupId
                return stateBattleSystem.btl_scene.PatNum;
            case 191: // GetData_191: return current field map ID
                return FF9StateSystem.Common.FF9.fldMapNo;
        }
        return code < 192 ? (Int32)btl_scrp.GetBattleData(code) : ff9.w_frameGetParameter(code);
    }
}
