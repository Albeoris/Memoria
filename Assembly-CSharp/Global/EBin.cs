using Assets.Sources.Scripts.EventEngine.Utils;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable RedundantAssignment

public class EBin
{
    public const Int32 RET = 4;
    public const Int32 CTRL_ACTOR = 250;
    public const Int32 PARTY_ACTOR = 251;
    public const Int32 PARTYN = 4;
    public const Int32 THIS = 255;
    public const Int32 vcSysList = 5;
    public const Byte WAIT_STATE_MESSAGE = 254;
    public const Byte WAIT_STATE_SPECIAL = 254;
    public const Byte WAIT_STATE_END_REQ = 255;

    public static ObjList objectList;
    public static Obj currentObject;
    public static Obj callerObject;
    public static Int32 SC_COUNTER_SVR = 0xDC; // EBin.getVarOperation(VariableSource.Global, VariableType.UInt16, 0)
    public static Int32 MAP_INDEX_SVR = 0x2D8; // EBin.getVarOperation(VariableSource.Global, VariableType.Int16, 2)

    public CalcStack calcstack = new CalcStack();

    //private const int ClassSeq = 1;
    //private const int CSeqOfs = 64;
    //private const int WaitMessage = 254;
    //private const int WaitEndReq = 255;
    //private const int VcObjSpec = 4;
    //private const int VcMember = 6;
    //private const int VcConst = 7;
    //private const int Fn = 6;
    //private const int Vt2 = 32;
    //private const int Vt1 = 16;
    //private const int Vt0 = 8;
    //private const int KAtn45 = 10;

    private static Int32 _calcValue;
    //private static Int32 _a0;
    //private static Int32 _a1;
    //private static Int32 _a2;
    //private static Int32 _a3;
    private static Int32 _flowState;
    //private static Int32 _s3;
    //private static Int32 _s5;
    private static Int32 _nextCodeIndex;
    //private static CalcStack _s4;
    private static CalcStack _calcStack;
    //private static CalcStack _tempS4;
    private static CalcStack _tempStack = new CalcStack();
    //private static Int32 _t0;
    //private static Int32 _t2;
    //private static Int32 _t3;
    //private static Int32 _t4;

    private readonly EventEngine _eventEngine;
    private Byte[] _instance;
    private Int32 _instanceVOfs;
    private Boolean _exprLoop;
    private Boolean _objectExists;
    private Boolean _nextLoop;
    private Byte[] _ratanTbl;

    public EBin(EventEngine ee)
    {
        _eventEngine = ee;
        InitializeATanTable();
        _calcStack = calcstack;
    }

    private void InitializeATanTable()
    {
        Byte[] binAsset = AssetManager.LoadBytes("EmbeddedAsset/ratan_tbl.bin");
        if (binAsset == null)
        {
            //Debug.LogError("InitializeATanTable: cannot load ratan_tble.bin.bytes");
            return;
        }
        MemoryStream memoryStream = new MemoryStream(binAsset);
        BinaryReader binaryReader = new BinaryReader(memoryStream);
        _ratanTbl = new Byte[binaryReader.BaseStream.Length];
        binaryReader.Read(_ratanTbl, 0, _ratanTbl.Length);
        binaryReader.Close();
    }

    private UInt16 GetUShortFromATanTable(Int32 offset)
    {
        return (UInt16)(_ratanTbl[offset] | (UInt16)(_ratanTbl[offset + 1] << 8));
    }

    public Int32 ProcessCode(ObjList objList)
    {
        Int32 result = -1;
        _objectExists = true;
        objectList = objList;
        EnterNextEntry();
        while (_objectExists)
        {
            currentObject = objectList.obj;
            if (currentObject.state == EventEngine.stateNew)
            {
                currentObject.state = EventEngine.stateInit;
                MoveToNextEntry();
                continue;
            }

            _flowState = EventEngine.FLOW_STATE_EXEC;
            if (currentObject.state == EventEngine.stateSuspend)
            {
                MoveToNextEntry();
                continue;
            }

            _nextCodeIndex = currentObject.ip;
            if (_nextCodeIndex == _eventEngine.nil)
            {
                MoveToNextEntry();
                continue;
            }

            if (currentObject.wait != 0)
            {
                if (currentObject.wait == EBin.WAIT_STATE_MESSAGE) // Wait for a window to close
                {
                    if (currentObject.winnum == 255) // EBin.WAIT_STATE_SPECIAL
                    {
                        currentObject.wait = 0;
                    }
                    else if (!ETb.MesWinActive(currentObject.winnum))
                    {
                        currentObject.winnum = 255;
                        currentObject.wait = 0;
                    }
                }
                else
                {
                    if (currentObject.wait != EBin.WAIT_STATE_END_REQ) // Wait indefinitely (255) or during N frames
                        currentObject.wait--;
                }
                MoveToNextEntry();
                continue;
            }

            _eventEngine.gExec = currentObject;
            _instance = currentObject.buffer;
            _instanceVOfs = currentObject.vofs << 2;
            callerObject = currentObject;
            _calcValue = currentObject.ip;
            if (currentObject.cid == 1) // Script executed with "STARTSEQ" (aka. "RunSharedScript")
                callerObject = _eventEngine.FindObjByUID(currentObject.uid - EventEngine.UID_OFFSET_SEQ);
            result = EnterFunctionLoop();
            callerObject = null;
        }
        return result;
    }

    private void HaltEventLoop()
    {
        _nextLoop = false;
        _objectExists = false;
    }

    public Int32 EnterFunctionLoop()
    {
        Int32 gMode = _eventEngine.gMode;
        _eventEngine.gCur = callerObject;
        if (gMode == 2)
            _eventEngine.ProcessCodeExt(currentObject);
        return ProcessFunctionLoop(gMode);
    }

    public Int32 ProcessFunctionLoop(Int32 gMode)
    {
        _nextLoop = true;
        while (_nextLoop)
        {
            if (_flowState == EventEngine.FLOW_STATE_EXEC)
            {
                EBin.event_code_binary opcode = (EBin.event_code_binary)currentObject.getByteIP();
                if (opcode > EBin.event_code_binary.PPRINTF)
                {
                    commandDefault();
                }
                else
                {
                    currentObject.ip++;
                    commandCodeFlow(opcode);
                    if (FF9StateSystem.Settings.IsFastTrophyMode)
                        EMinigame.DigUpMadianRingCheating();
                }
            }
            else
            {
                EntryLoopDone();
            }
        }
        return _calcValue;
    }

    public void EntryLoopDone()
    {
        _calcValue = _flowState;
        if (_flowState >= EventEngine.FLOW_STATE_JUMP_BATTLE && _flowState <= EventEngine.FLOW_STATE_GAMEOVER)
        {
            if (_flowState == EventEngine.FLOW_STATE_STOP)
                _eventEngine.gStopObj = objectList;
            HaltEventLoop();
        }
        else if (_flowState == EventEngine.FLOW_STATE_DELETE)
        {
            EnterNextEntry();
        }
        else
        {
            MoveToNextEntry();
        }
    }

    // Run this (entries flagged with an append mode "Auto-init") after the Main_Init function executed some "InitObject" or "InitRegion" or "InitCode"
    public void ProceedAutoStartEntries()
    {
        if (_eventEngine.autoStartEntriesProceed && !_eventEngine.autoStartEntriesDone)
        {
            for (Int32 i = 0; i < _eventEngine.sSourceObjN; i++)
            {
                if ((_eventEngine.sObjTable[i].append_mode & 2) != 0)
                {
                    Byte entryType = 255;
                    if (_eventEngine.allObjsEBData[i].Length > 0)
                        entryType = _eventEngine.allObjsEBData[i][0];
                    if (entryType == 2)
                    {
                        Actor actor = new Actor(i, 0);
                        if (_eventEngine.gMode == 3)
                            Singleton<WMWorld>.Instance.addWMActorOnly(actor);
                    }
                    else if (entryType == 1)
                    {
                        new Quad(i, 0);
                    }
                    else
                    {
                        EventEngine.NewThread(i, 0);
                    }
                }
            }
            _eventEngine.autoStartEntriesDone = true;
        }
    }

    public void MoveToNextEntry()
    {
        ProceedAutoStartEntries();
        if (objectList != null)
        {
            getVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Byte, 24));
            objectList = objectList.next;
        }
        EnterNextEntry();
    }

    public void EnterNextEntry()
    {
        _calcValue = 0;
        _nextLoop = false;
        if (objectList == null)
            _objectExists = false;
    }

    public Int32 expr()
    {
        _calcStack = calcstack;
        //_tempS4 = _s4;
        //_s4 = _eventEngine.gCP;
        _calcStack.emptyCalcStack();
        _exprLoop = true;
        while (_exprLoop)
        {
            Byte varOperation = currentObject.getByteIP();
            if (currentObject.sid != 3 || currentObject.ip != 110)
            {
                if (FF9StateSystem.Settings.IsFastTrophyMode)
                {
                    if (FF9StateSystem.Common.FF9.fldMapNo == 2801 && currentObject.sid == 11 && currentObject.ip == 3834) // Daguerreo/Right Hall, Gilgamesh
                        setVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Byte, 46), 8);
                    if (FF9StateSystem.Common.FF9.fldMapNo == 1900 && currentObject.sid == 0 && currentObject.ip == 4138) // Treno/Pub, Main
                        setVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Byte, 26), 8);
                }
            }
            if (FF9StateSystem.Common.FF9.fldMapNo == 705 && currentObject.sid == 3 && currentObject.ip == 541) // Gizamaluke/Bell Room, Female Moogle
            {
                currentObject.ip += 7;
                return 0;
            }
            EMinigame.ChanbaraBonusPoints(currentObject, this);
            EMinigame.SetViviSpeed(currentObject, this);
            currentObject.ip++;
            if (varOperation >= 0x80)
            {
                if (varOperation == 0xD3)
                {
                    expr_customSubCommand();
                }
                else
                {
                    _calcValue = expr_varSpec(varOperation);
                    _calcStack.push(_calcValue);
                }
            }
            else
            {
                expr_jumpToSubCommand((op_binary)varOperation);
            }
        }
        return 0;
    }

    private void expr_customSubCommand()
    {
        flexible_varfunc commandId = (flexible_varfunc)currentObject.getUShortIP();
        currentObject.ip += 2;
        Byte argCount = currentObject.getByteIP();
        currentObject.ip++;
        Int32[] args = new Int32[argCount];
        for (Int32 i = argCount - 1; i >= 0; i--)
            args[i] = EvaluateValueExpression();
        _calcValue = 0;
        switch (commandId)
        {
            case flexible_varfunc.ITEM_REGULAR_TO_ID:
                _calcValue = ff9item.GetItemIdFromRegularId((RegularItem)args[0]);
                break;
            case flexible_varfunc.ITEM_ID_TO_REGULAR:
                _calcValue = (Int32)ff9item.GetRegularIdFromItemId(args[0]);
                break;
            case flexible_varfunc.ITEM_KEY_TO_ID:
                _calcValue = ff9item.GetItemIdFromImportantId(args[0]);
                break;
            case flexible_varfunc.ITEM_ID_TO_KEY:
                _calcValue = (Int32)ff9item.GetImportantIdFromItemId(args[0]);
                break;
            case flexible_varfunc.ITEM_CARD_TO_ID:
                _calcValue = ff9item.GetItemIdFromCardId((TetraMasterCardId)args[0]);
                break;
            case flexible_varfunc.ITEM_ID_TO_CARD:
                _calcValue = (Int32)ff9item.GetCardIdFromItemId(args[0]);
                break;
            case flexible_varfunc.ABILITY_ACTIVE_TO_ID:
                _calcValue = ff9abil.GetAbilityIdFromActiveAbility((BattleAbilityId)args[0]);
                break;
            case flexible_varfunc.ABILITY_ID_TO_ACTIVE:
                _calcValue = (Int32)ff9abil.GetActiveAbilityFromAbilityId(args[0]);
                break;
            case flexible_varfunc.ABILITY_SUPPORT_TO_ID:
                _calcValue = ff9abil.GetAbilityIdFromSupportAbility((SupportAbility)args[0]);
                break;
            case flexible_varfunc.ABILITY_ID_TO_SUPPORT:
                _calcValue = (Int32)ff9abil.GetSupportAbilityFromAbilityId(args[0]);
                break;
            case flexible_varfunc.PARTY_MEMBER:
                _calcValue = (Int32)ff9play.CharacterIDToOldIndex(FF9StateSystem.Common.FF9.party.GetCharacterId(args[0]));
                break;
            case flexible_varfunc.ITEM_FULL_COUNT:
                _calcValue = ff9item.FF9Item_GetAnyCount((RegularItem)args[0]);
                break;
            case flexible_varfunc.PLAYER_EQUIP:
                _calcValue = (Int32)(FF9StateSystem.Common.FF9.GetPlayer(ff9play.CharacterOldIndexToID((CharacterOldIndex)args[0]))?.equip[args[1]] ?? RegularItem.NoItem);
                break;
            case flexible_varfunc.PLAYER_LEVEL:
                _calcValue = FF9StateSystem.Common.FF9.GetPlayer(ff9play.CharacterOldIndexToID((CharacterOldIndex)args[0]))?.level ?? 0;
                break;
            case flexible_varfunc.PLAYER_EXP:
                _calcValue = (Int32)(FF9StateSystem.Common.FF9.GetPlayer(ff9play.CharacterOldIndexToID((CharacterOldIndex)args[0]))?.exp ?? 0);
                break;
            case flexible_varfunc.PLAYER_EXP_REQ:
                if (args[0] <= 0)
                    _calcValue = 0;
                else if (args[0] > ff9level.LEVEL_COUNT)
                    _calcValue = (Int32)9999999u;
                else
                    _calcValue = (Int32)ff9level.CharacterLevelUps[args[0] - 1].ExperienceToLevel;
                break;
            case flexible_varfunc.PLAYER_ABILITY_LEARNT:
            {
                PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(ff9play.CharacterOldIndexToID((CharacterOldIndex)args[0]));
                if (player == null || !ff9abil.FF9Abil_HasAp(player))
                    break;
                Int32 abilIndex = ff9abil.FF9Abil_GetIndex(player, args[1]);
                if (abilIndex < 0)
                    break;
                _calcValue = player.pa[abilIndex] >= ff9abil._FF9Abil_PaData[player.PresetId][abilIndex].Ap ? 1 : 0;
                if (_calcValue == 0 && args[2] != 0)
                    for (Int32 i = 0; i < 5; i++)
                        if (player.equip[i] != RegularItem.NoItem && ff9item._FF9Item_Data[player.equip[i]].ability.Any(id => id == args[1]))
                            _calcValue = 1;
                break;
            }
            case flexible_varfunc.PLAYER_SUPPORT_ENABLED:
            {
                PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(ff9play.CharacterOldIndexToID((CharacterOldIndex)args[0]));
                _calcValue = player != null && player.saExtended.Contains((SupportAbility)args[1]) ? 1 : 0;
                break;
            }
            case flexible_varfunc.SHOP_ITEM:
                _calcValue = ff9buy.ShopItems.ContainsKey(args[0]) && ff9buy.ShopItems[args[0]].ItemIds.Contains((RegularItem)args[1]) ? 1 : 0;
                break;
            case flexible_varfunc.SHOP_SYNTH:
                _calcValue = ff9mix.SynthesisData.ContainsKey(args[1]) && ff9mix.SynthesisData[args[1]].Shops.Contains(args[0]) ? 1 : 0;
                break;
            case flexible_varfunc.VECTOR:
                _calcStack.pushSubs(args[0], args[1]);
                _calcStack.push(encodeTypeAndVarClass(VariableSource.Null, VariableType.Vector));
                return;
            case flexible_varfunc.VECTOR_SIZE:
                _calcStack.pushSubs(args[0]);
                _calcStack.push(encodeTypeAndVarClass(VariableSource.Null, VariableType.VectorSize));
                return;
            case flexible_varfunc.DICTIONARY:
                _calcStack.pushSubs(args[0], args[1]);
                _calcStack.push(encodeTypeAndVarClass(VariableSource.Null, VariableType.Dictionary));
                return;
            case flexible_varfunc.CATEGORY_KILL_COUNT:
                _calcValue = args[0] >= 0 && args[0] < FF9StateSystem.Common.FF9.categoryKillCount.Length ? FF9StateSystem.Common.FF9.categoryKillCount[(Int16)args[0]] : 0;
                break;
            case flexible_varfunc.MODEL_KILL_COUNT:
                if (FF9StateSystem.Common.FF9.modelKillCount.TryGetValue((Int16)args[0], out Int16 count))
                    _calcValue = count;
                else
                    _calcValue = 0;
                break;
            case flexible_varfunc.ABILITY_USE_COUNT:
                _calcValue = FF9StateSystem.EventState.GetAAUsageCounter((BattleAbilityId)args[0]);
                break;
        }
        expr_Push_v0_Int24();
    }

    private Int32 expr_varSpec(Int32 varOperation)
    {
        _calcValue = (varOperation & 3) << 26 | (varOperation & 0x1C) << 27;
        Int32 varArrayIndex = currentObject.getByteIP();
        currentObject.ip++;
        if ((varOperation & 0x20) != 0)
        {
            _calcValue |= varArrayIndex;
            varArrayIndex = currentObject.getByteIP();
            currentObject.ip++;
            varArrayIndex <<= 8;
        }
        _calcValue |= varArrayIndex;
        return _calcValue;
    }

    public Int32 setVarManually(Int32 varOperation, Int32 value)
    {
        Int32 varCode = (varOperation & 3) << 26 | (varOperation & 0x1C) << 27;
        Int32 varArrayIndex = (varOperation >> 8) & 0xFF;
        if ((varOperation & 0x20) != 0)
            varArrayIndex |= (varOperation >> 8) & 0xFF00;
        varCode |= varArrayIndex;
        _calcStack.push(varCode);
        SetVariableValue(value);
        varCode |= encodeVarClass(VariableSource.Int26);
        _calcStack.push(varCode);

        return varCode;
    }

    public Int32 getVarManually(Int32 varOperation)
    {
        CalcStack calcStack = _calcStack;
        _calcStack = _tempStack;
        _calcStack.emptyCalcStack();
        Int32 varCode = (varOperation & 3) << 26 | (varOperation & 0x1C) << 27;
        Int32 varArrayIndex = (varOperation & 0xFF00) >> 8;
        if ((varOperation & 0x20) != 0)
            varArrayIndex |= (varOperation >> 8) & 0xFF00;
        varCode |= varArrayIndex;
        _calcStack.push(varCode);
        Int32 result = EvaluateValueExpression();
        _calcStack = calcStack;
        return result;
    }

    public static Int32 getVarOperation(VariableSource varSrc, VariableType varType, Int32 index)
    {
        if (index > 0xFFFF || index < 0 || varSrc > VariableSource.Null)
            throw new ArgumentOutOfRangeException();
        Int32 varOperation = (index << 8) | ((Int32)varType << 2) | ((Int32)varSrc) | 0xC0;
        if (index > 0xFF)
            varOperation |= 0x20;
        return varOperation;
    }

    private void expr_jumpToSubCommand(op_binary formulaOp)
    {
        if (formulaOp < op_binary.B_PAD0 || formulaOp > op_binary.B_EXPR_END)
            return;

        switch (formulaOp)
        {
            case op_binary.B_PAD0:
            case op_binary.B_PAD1:
            case op_binary.B_PAD2:
            case op_binary.B_PAD3:
            case op_binary.B_POST_PLUS_A:
            case op_binary.B_POST_MINUS_A:
            case op_binary.B_PRE_PLUS_A:
            case op_binary.B_PRE_MINUS_A:
            case op_binary.B_NOT_E:
            case op_binary.B_LET_E:
            case op_binary.B_AND_LET_E:
            case op_binary.B_XOR_LET_E:
            case op_binary.B_OR_LET_E:
            case op_binary.B_CAST8:
            case op_binary.B_CAST8U:
            case op_binary.B_CAST16:
            case op_binary.B_CAST16U:
            case op_binary.B_CAST_LIST:
            case op_binary.B_LMAX:
            case op_binary.B_LMIN:
            case op_binary.B_OBJSPEC:
            case op_binary.B_CURHP:
            case op_binary.B_MAXHP:
            case op_binary.B_KEYON2:
            case op_binary.B_KEYOFF2:
            case op_binary.B_KEY2:
            case op_binary.B_HAVE_ITEM:
            case op_binary.B_BAFRAME:
            case op_binary.pad67:
            case op_binary.pad68:
            case op_binary.pad69:
            case op_binary.B_FRAME:
            case op_binary.B_SPS:
            case op_binary.B_CURMP:
            case op_binary.B_MAXMP:
            case op_binary.B_BGIID:
            case op_binary.B_BGIFLOOR:
            case op_binary.B_pad7b:
            case op_binary.B_PAD4:
            {
                _eventEngine.gCP = _calcStack;
                _calcValue = _eventEngine.DoCalcOperationExt(formulaOp);
                _calcStack = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_POST_PLUS:
            {
                _calcValue = EvaluateValueExpression();
                Int32 val = _calcValue;
                _calcStack.advanceTopOfStack();
                SetVariableValue(_calcValue + 1);
                _calcValue = val;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_POST_MINUS:
            {
                _calcValue = EvaluateValueExpression();
                Int32 val = _calcValue;
                _calcStack.advanceTopOfStack();
                SetVariableValue(_calcValue - 1);
                _calcValue = val;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PRE_PLUS:
            {
                _calcValue = EvaluateValueExpression();
                Int32 val = _calcValue + 1;
                _calcStack.advanceTopOfStack();
                SetVariableValue(val);
                _calcValue = val;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PRE_MINUS:
            {
                _calcValue = EvaluateValueExpression();
                Int32 val = _calcValue - 1;
                _calcStack.advanceTopOfStack();
                SetVariableValue(val);
                _calcValue = val;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SINGLE_PLUS:
            {
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SINGLE_MINUS:
            {
                _calcValue = -EvaluateValueExpression();
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_NOT:
            {
                _calcValue = EvaluateValueExpression() <= 0 ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_COMP:
            {
                _calcValue = ~EvaluateValueExpression();
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_MULT:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue *= y;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_DIV:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                if (y == 0)
                {
                    expr_Push_v0_Int24();
                }
                else
                {
                    _calcValue /= y;
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_REM:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                if (y == 0)
                {
                    expr_Push_v0_Int24();
                }
                else
                {
                    _calcValue %= y;
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_PLUS:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue += y;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_MINUS:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue -= y;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SHIFT_LEFT:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue <<= y;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SHIFT_RIGHT:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue >>= y;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_LT: // B_LT = 24,
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                if ((FF9StateSystem.Common.FF9.fldMapNo == 908 || FF9StateSystem.Common.FF9.fldMapNo == 1908) && _eventEngine.gCur.uid == 0 && y == 80)
                    y = 300; // fix for gates at treno in widescreen
                if (_eventEngine.gCur.uid == 13 && y == -300)
                    y = -250;
                _calcValue = _calcValue < y ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_GT: // B_GT = 25,
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                if (FF9StateSystem.Common.FF9.fldMapNo == 657 && _eventEngine.gCur.sid == 17 // Marsh/Pond, Zidane
                  && (_eventEngine.gCur.ip == 1413 || _eventEngine.gCur.ip == 1542 || _eventEngine.gCur.ip == 1666 || _eventEngine.gCur.ip == 1795 || _eventEngine.gCur.ip == 2172 || _eventEngine.gCur.ip == 2301 || _eventEngine.gCur.ip == 1919 || _eventEngine.gCur.ip == 2048 || _eventEngine.gCur.ip == 2425 || _eventEngine.gCur.ip == 2554 || _eventEngine.gCur.ip == 2683 || _eventEngine.gCur.ip == 2812 || _eventEngine.gCur.ip == 2941))
                    _calcValue = _calcValue >= y ? 1 : 0;
                else
                    _calcValue = _calcValue > y ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_LE: // B_LE = 26,
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue = _calcValue <= y ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_GE: // B_GE = 27,
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue = _calcValue >= y ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_LT_E:
            case op_binary.B_GT_E:
            case op_binary.B_LE_E:
            case op_binary.B_GE_E:
            case op_binary.B_EQ_E:
            case op_binary.B_NE_E:
            case op_binary.B_AND_E:
            case op_binary.B_NAND_E:
            case op_binary.B_XOR_E:
            case op_binary.B_OR_E:
            {
                _eventEngine.gCP = _calcStack;
                _calcValue = _eventEngine.OperatorExtract(formulaOp);
                _calcStack = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_EQ:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue ^= y;
                _calcValue = _calcValue == 0 ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_NE:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue ^= y;
                _calcValue = _calcValue != 0 ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_AND:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue &= y;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_XOR:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue ^= y;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_OR:
            {
                Int32 y = EvaluateValueExpression();
                _calcValue = EvaluateValueExpression();
                _calcValue |= y;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_ANDAND:
            {
                _calcValue = EvaluateValueExpression();
                _calcStack.retreatTopOfStack();
                if (_calcValue == 0)
                {
                    expr_Push_v0_Int24();
                }
                else
                {
                    _calcStack.advanceTopOfStack();
                    _calcValue = EvaluateValueExpression();
                    _calcValue = _calcValue == 0 ? 0 : 1;
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_OROR:
            {
                _calcValue = EvaluateValueExpression();
                _calcValue = _calcValue == 0 ? 0 : 1;
                _calcStack.retreatTopOfStack();
                if (_calcValue != 0)
                {
                    expr_Push_v0_Int24();
                }
                else
                {
                    _calcStack.advanceTopOfStack();
                    _calcValue = EvaluateValueExpression();
                    _calcValue = _calcValue == 0 ? 0 : 1;
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_MEMBER:
            {
                Int32 dataKind = currentObject.getByteIP();
                currentObject.ip++;
                dataKind |= encodeVarClass(VariableSource.Member);
                _calcStack.push(dataKind);
                break;
            }
            case op_binary.B_COUNT:
            {
                _eventEngine.gCP = _calcStack;
                _calcValue = _eventEngine.OperatorCount();
                _calcStack = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PICK:
            {
                _eventEngine.gCP = _calcStack;
                _calcValue = _eventEngine.OperatorFirstOf();
                _calcStack = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_LET:
            {
                _calcValue = EvaluateValueExpression();
                Int32 currentValue = _calcValue;
                SetVariableValue(currentValue);
                _calcValue = currentValue;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_LET_A:
            case op_binary.B_MULT_LET_A:
            case op_binary.B_DIV_LET_A:
            case op_binary.B_REM_LET_A:
            case op_binary.B_PLUS_LET_A:
            case op_binary.B_MINUS_LET_A:
            case op_binary.B_SHIFT_LEFT_LET_A:
            case op_binary.B_SHIFT_RIGHT_LET_A:
            case op_binary.B_AND_LET_A:
            case op_binary.B_XOR_LET_A:
            case op_binary.B_OR_LET_A:
            {
                _eventEngine.gCP = _calcStack;
                _calcValue = _eventEngine.OperatorAll(formulaOp);
                _calcStack = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_MULT_LET:
            {
                Int32 y = EvaluateValueExpression();
                Int32 x = EvaluateValueExpression();
                Int32 xy = x * y;
                _calcStack.advanceTopOfStack();
                SetVariableValue(xy);
                _calcValue = xy;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_DIV_LET:
            {
                Int32 y = EvaluateValueExpression();
                Int32 x = EvaluateValueExpression();
                _calcStack.advanceTopOfStack();
                if (y == 0)
                {
                    _calcValue = x;
                    expr_Push_v0_Int24();
                }
                else
                {
                    Int32 xy = x / y;
                    SetVariableValue(xy);
                    _calcValue = xy;
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_REM_LET:
            {
                Int32 y = EvaluateValueExpression();
                Int32 x = EvaluateValueExpression();
                _calcStack.advanceTopOfStack();
                if (y == 0)
                {
                    _calcValue = x;
                    expr_Push_v0_Int24();
                }
                else
                {
                    Int32 xy = x % y;
                    SetVariableValue(xy);
                    _calcValue = xy;
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_PLUS_LET:
            {
                Int32 y = EvaluateValueExpression();
                Int32 x = EvaluateValueExpression();
                Int32 xy = x + y;
                _calcStack.advanceTopOfStack();
                SetVariableValue(xy);
                _calcValue = xy;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_MINUS_LET:
            {
                Int32 y = EvaluateValueExpression();
                Int32 x = EvaluateValueExpression();
                Int32 xy = x - y;
                _calcStack.advanceTopOfStack();
                SetVariableValue(xy);
                _calcValue = xy;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SHIFT_LEFT_LET:
            {
                Int32 y = EvaluateValueExpression();
                Int32 x = EvaluateValueExpression();
                Int32 xy = x << y;
                _calcStack.advanceTopOfStack();
                SetVariableValue(xy);
                _calcValue = xy;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SHIFT_RIGHT_LET:
            {
                Int32 y = EvaluateValueExpression();
                Int32 x = EvaluateValueExpression();
                Int32 xy = x >> y;
                _calcStack.advanceTopOfStack();
                SetVariableValue(xy);
                _calcValue = xy;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_AND_LET:
            {
                Int32 y = EvaluateValueExpression();
                Int32 x = EvaluateValueExpression();
                Int32 xy = x & y;
                _calcStack.advanceTopOfStack();
                SetVariableValue(xy);
                _calcValue = xy;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_XOR_LET:
            {
                Int32 y = EvaluateValueExpression();
                Int32 x = EvaluateValueExpression();
                Int32 xy = x ^ y;
                _calcStack.advanceTopOfStack();
                SetVariableValue(xy);
                _calcValue = xy;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_OR_LET:
            {
                Int32 y = EvaluateValueExpression();
                Int32 x = EvaluateValueExpression();
                Int32 xy = x | y;
                _calcStack.advanceTopOfStack();
                SetVariableValue(xy);
                _calcValue = xy;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SELECT:
            {
                _eventEngine.gCP = _calcStack;
                _calcValue = _eventEngine.GetRandomActiveBit();
                _calcStack = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_KEYON: // B_KEYON = 79
            {
                VoicePlayer.scriptRequestedButtonPress = true;
                _calcValue = (Mathf.Abs(EvaluateValueExpression() & ETb.KeyOn(Localization.CurrentSymbol == "JP")) <= 0) ? 0 : 1;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SIN2:
            {
                _calcValue = ff9.rsin(EvaluateValueExpression());
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_COS2:
            {
                _calcValue = ff9.rcos(EvaluateValueExpression());
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_KEYOFF:
            {
                _calcValue = (Mathf.Abs(EvaluateValueExpression() & ETb.KeyOff(Localization.CurrentSymbol == "JP")) <= 0) ? 0 : 1;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_KEY:
            {
                _calcValue = (Mathf.Abs(EvaluateValueExpression() & ETb.GetInputs(Localization.CurrentSymbol == "JP")) <= 0) ? 0 : 1;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_ANGLE:
            {
                Int32 pZ = EvaluateValueExpression();
                Int32 pX = EvaluateValueExpression();
                Actor actor = (Actor)_eventEngine.gCur;
                _calcValue = ConvertFloatAngleToFixedPoint(angleAsm(pX - actor.pos[0], pZ - actor.pos[2]));
                _calcValue <<= 20;
                _calcValue >>= 24;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_DISTANCE:
            {
                Int32 pZ = EvaluateValueExpression();
                Int32 pX = EvaluateValueExpression();
                Actor actor = (Actor)_eventEngine.gCur;
                _calcValue = (Int32)distance(pX - actor.pos[0], 0, pZ - actor.pos[2]);
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PTR:
            {
                Int32 uid = currentObject.getByteIP();
                currentObject.ip++;
                Obj obj = _eventEngine.GetObjByUID(uid, _eventEngine.GetObjectModIndex(currentObject));
                _calcValue = obj?.uid ?? 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_ANGLEA:
            {
                Obj obj = _eventEngine.GetObjByUID(EvaluateValueExpression(), _eventEngine.GetObjectModIndex(currentObject));
                Single objX = ((PosObj)obj).pos[0];
                Single objZ = ((PosObj)obj).pos[2];
                Single curX = ((PosObj)_eventEngine.gCur).pos[0];
                Single curZ = ((PosObj)_eventEngine.gCur).pos[2];
                _calcValue = ConvertFloatAngleToFixedPoint(angleAsm(objX - curX, objZ - curZ)) >> 4;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_DISTANCEA:
            {
                Actor argActor = (Actor)_eventEngine.GetObjByUID(EvaluateValueExpression(), _eventEngine.GetObjectModIndex(currentObject));
                Actor curActor = (Actor)_eventEngine.gCur;
                Single dX = argActor.pos[0] - curActor.pos[0];
                Single dZ = argActor.pos[2] - curActor.pos[2];
                _calcValue = (Int32)distance(dX, 0, dZ);
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SIN:
            {
                _calcValue = ff9.rsin(EvaluateValueExpression() << 4);
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_COS:
            {
                _calcValue = ff9.rcos(EvaluateValueExpression() << 4);
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_ANGLE2:
            {
                Int32 dZ = EvaluateValueExpression();
                Int32 dX = EvaluateValueExpression();
                _calcValue = ConvertFloatAngleToFixedPoint(angleAsm(dX, dZ)) >> 4;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PARTYCHK: // B_PARTYCHK
            {
                _calcValue = _eventEngine.partychk(EvaluateValueExpression()) ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PARTYADD:
            {
                _calcValue = _eventEngine.partyadd(EvaluateValueExpression()) ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_OBJSPECA:
            {
                _calcStack.push(currentObject.getByteIP(1) | (currentObject.getByteIP() << 8) | encodeVarClass(VariableSource.Object));
                currentObject.ip += 2;
                break;
            }
            case op_binary.B_SYSLIST:
            {
                _calcStack.push(currentObject.getByteIP() | encodeVarClass(VariableSource.System));
                currentObject.ip++;
                break;
            }
            case op_binary.B_SYSVAR:
            {
                _calcValue = _eventEngine.GetSysvar(currentObject.getByteIP());
                currentObject.ip++;
                _calcStack.push((_calcValue & 0x3FFFFFF) | encodeVarClass(VariableSource.Int26)); // 26 bit (signed)
                break;
            }
            case op_binary.B_CONST:
            {
                _calcStack.push((Int32)currentObject.getShortIP() | encodeVarClass(VariableSource.Int26));
                currentObject.ip += 2;
                break;
            }
            case op_binary.B_CONST4:
            {
                _calcStack.push((currentObject.getIntIP() & 0x3FFFFFF) | encodeVarClass(VariableSource.Int26)); // 26 bit (signed)
                currentObject.ip += 4;
                break;
            }
            case op_binary.B_EXPR_END:
            {
                _eventEngine.gCP = _calcStack;
                _exprLoop = false;
                break;
            }
        }
    }

    private static Int32 ConvertFloatAngleToFixedPoint(Single floatAngle)
    {
        Single f = (Single)(floatAngle / 360.0 * 4096.0);
        Int32 int1 = Mathf.FloorToInt(f);
        Int32 int2 = Mathf.CeilToInt(f);
        Int32 int3 = Mathf.RoundToInt(f);
        if (int3 == int2)
            return int2;
        if (int3 == int1)
            return int1;
        return -1;
    }

    private void expr_Push_v0_Int24()
    {
        _calcValue |= encodeVarClass(VariableSource.Int26);
        _calcStack.push(_calcValue);
    }

    public Int32 bra()
    {
        Int16 shortIP = currentObject.getShortIP();
        currentObject.ip += 2;
        currentObject.ip += shortIP;
        return 0;
    }

    public Int32 beq()
    {
        _calcValue = EvaluateValueExpression();
        if (_calcValue != 0)
        {
            currentObject.ip += 2;
        }
        else
        {
            Int32 uShortIP = currentObject.getUShortIP();
            currentObject.ip += 2;
            currentObject.ip += uShortIP;
        }
        return 0;
    }

    public Int32 bne()
    {
        _calcValue = EvaluateValueExpression();
        if (_calcValue == 0)
        {
            currentObject.ip += 2;
        }
        else
        {
            bra();
        }
        return 0;
    }

    public Int32 wait()
    {
        Int32 varargflag = currentObject.getByteIP();
        currentObject.ip++;
        _calcValue = getv1i(ref varargflag);
        if (FF9StateSystem.Common.FF9.fldMapNo == 3011) // Ending/TH
        {
            String lang = Localization.CurrentSymbol;
            if (lang != "US" && lang != "JP")
            {
                if (_calcValue == 82)
                    _calcValue = 102;
                else if (_calcValue == 50)
                    _calcValue = 90;
            }
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 3009) // Ending/TH
        {
            String lang = Localization.CurrentSymbol;
            if (lang != "US" && lang != "JP" && currentObject.uid == 17 && _calcValue == 15)
                _calcValue = 20;
        }
        if (_calcValue != 0)
        {
            if (_calcValue > EBin.WAIT_STATE_SPECIAL)
                _calcValue = 253;
            else
                _calcValue--;
            currentObject.wait = (Byte)_calcValue;
            _flowState = EventEngine.FLOW_STATE_WAIT;
        }
        return 0;
    }

    private void JMP_SWITCH(Int32 caseNumber)
    {
        Int32 offsetL = currentObject.getByteIP(1);
        Int32 offsetH = (SByte)currentObject.getByteIP(2);
        offsetH <<= 8;
        _calcValue = EvaluateValueExpression();
        _calcValue -= offsetL;
        _calcValue -= offsetH;
        Int32 a0 = _calcValue - caseNumber;
        if (_calcValue < 0)
        {
            JMP_SWITCH_DEFAULT();
        }
        else
        {
            _calcValue <<= 1;
            if (a0 >= 0)
            {
                JMP_SWITCH_DEFAULT();
            }
            else
            {
                a0 = _nextCodeIndex + _calcValue;
                Int32 a1 = currentObject.getByteIP(_calcValue + 5); // caseOffsetL
                Int32 a2 = currentObject.getByteIP(_calcValue + 6); // caseOffsetH
                currentObject.ip += a1;
                currentObject.ip += a2 << 8;
            }
        }
    }

    public void JMP_SWITCH_DEFAULT()
    {
        Int32 offsetL = currentObject.getByteIP(3);
        Int32 offsetH = currentObject.getByteIP(4);
        offsetH = (offsetH << 8 | offsetL);
        currentObject.ip += offsetH;
    }

    public Int32 commandDefault()
    {
        _calcValue = _eventEngine.DoEventCode();
        _flowState = _calcValue;
        if (_eventEngine.gArgUsed > 0)
        {
            _nextCodeIndex = currentObject.ip;
        }
        else
        {
            _nextCodeIndex = currentObject.ip - 1;
            currentObject.ip = _nextCodeIndex;
        }
        return 0;
    }

    public void JMP_SWITCHEX(ref Int32 caseOffset, ref Int32 caseNumber)
    {
        if (caseNumber > 0)
        {
            caseNumber--;
            Int32 valueH = currentObject.getByteIP(1 + caseOffset);
            Int32 valueL = currentObject.getByteIP(0 + caseOffset);
            Int32 caseValue = valueL | valueH << 8;
            caseValue -= _calcValue; // inputValue
            caseOffset += 4;
            if (caseValue == 0)
            {
                Int32 offsetH = currentObject.getByteIP(-1 + caseOffset);
                Int32 offsetL = currentObject.getByteIP(-2 + caseOffset);
                offsetL |= offsetH << 8;
                currentObject.ip += offsetL;
                currentObject.ip += 3;
            }
            else
            {
                JMP_SWITCHEX(ref caseOffset, ref caseNumber);
            }
        }
        else
        {
            caseNumber--;
            Int32 offsetH = currentObject.getByteIP(2);
            Int32 offsetL = currentObject.getByteIP(1);
            offsetL |= offsetH << 8;
            currentObject.ip += offsetL;
            currentObject.ip += 3;
        }
    }

    public void TerminateEntry()
    {
        objectList = _eventEngine.DisposeObj(currentObject);
        _flowState = EventEngine.FLOW_STATE_DELETE;
    }

    public Int32 commandCodeFlow(EBin.event_code_binary opcode)
    {
        switch (opcode)
        {
            case EBin.event_code_binary.rsv01: // JMP
                bra();
                return 0;
            case EBin.event_code_binary.rsv02: // JMP_IFNOT
                beq();
                return 0;
            case EBin.event_code_binary.rsv03: // JMP_IF
                bne();
                return 0;
            case EBin.event_code_binary.rsv04: // return
                _eventEngine.Return(currentObject);
                EntryLoopDone();
                return 0;
            case EBin.event_code_binary.EXPR: // set
                expr();
                return 0;
            case EBin.event_code_binary.rsv06: // JMP_SWITCHEX
            {
                Int32 caseNumber = currentObject.getByteIP();
                _calcValue = EvaluateValueExpression();
                _calcValue &= 65535;
                Int32 caseOffset = 3;
                JMP_SWITCHEX(ref caseOffset, ref caseNumber);
                return 0;
            }
            case EBin.event_code_binary.rsv0b: // JMP_SWITCH
            {
                Int32 caseNumber = currentObject.getByteIP();
                JMP_SWITCH(caseNumber);
                return 0;
            }
            case EBin.event_code_binary.rsv0d: // JMP_SWITCH with many cases (>255)
            {
                Int32 caseNumber = currentObject.getShortIP();
                currentObject.ip++;
                JMP_SWITCH(caseNumber);
                return 0;
            }
            case EBin.event_code_binary.DELETE: // TerminateEntry
            {
                Int32 delUID = currentObject.getByteIP(1);
                currentObject.ip += 2;
                if (delUID == 255)
                {
                    TerminateEntry();
                }
                else
                {
                    Obj delObj = _eventEngine.GetObjByUID(delUID, _eventEngine.GetObjectModIndex(currentObject));
                    if (currentObject == delObj)
                        TerminateEntry();
                    else
                        _eventEngine.DisposeObj(delObj);
                }
                return 0;
            }
            case EBin.event_code_binary.WAIT: // Wait
                wait();
                return 0;
            case EBin.event_code_binary.PRINT1: // PRINT1
                return 0;
            case EBin.event_code_binary.PRINTF: // PRINTF
                return 0;
            case EBin.event_code_binary.LOCATE: // LOCATE
            {
                Int32 varargflag = currentObject.getByteIP();
                currentObject.ip++;
                Int32 arg1 = getv1i(ref varargflag);
                Int32 arg2 = getv1i(ref varargflag);
                return 0;
            }
            case EBin.event_code_binary.PPRINT: // PPRINT
                return 0;
            case EBin.event_code_binary.PPRINTF: // PPRINTF
                return 0;
            default:
                currentObject.ip--;
                commandDefault();
                return 0;
        }
    }

    public Single angleAsm(Single deltaX, Single deltaZ)
    {
        Int32 num1 = (Int32)deltaX;
        Int32 num2 = (Int32)deltaZ;
        if (num1 == 0 && num2 == 0)
            return 0.0f;
        Int32 num3 = num2 << 10;
        Int32 num4 = num1 << 10;
        Int32 num5;
        if (num2 >= 0)
        {
            Int32 num6 = num1 - num2;
            num5 = num1 >= 0 ? (num6 >= 0 ? -1024 - this.GetUShortFromATanTable(num3 / num1 << 1) : (Int32)this.GetUShortFromATanTable(num4 / num2 << 1) - 2048) : (-num1 - num2 < 0 ? 2048 - (Int32)this.GetUShortFromATanTable(-(num4 / num2 << 1)) : 1024 + (Int32)this.GetUShortFromATanTable(-(num3 / num1 << 1)));
        }
        else
        {
            Int32 num6 = num1 - num2;
            if (num1 >= 0)
            {
                Int32 num7 = -num2;
                num5 = num1 - num7 < 0 ? -this.GetUShortFromATanTable(-(num4 / num2 << 1)) : (Int32)this.GetUShortFromATanTable(-(num3 / num1 << 1)) - 1024;
            }
            else
                num5 = num6 < 0 ? 1024 - (Int32)this.GetUShortFromATanTable(num3 / num1 << 1) : (Int32)this.GetUShortFromATanTable(num4 / num2 << 1);
        }
        return EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)num5);
    }

    private static Single ConvertFixedPointAngleToDegree(Int16 fixedPointAngle)
    {
        return (Single)((fixedPointAngle >> 4) / 256.0 * 360.0);
    }

    public Int32 CollisionAngle(PosObj po, PosObj pot, Single myrot)
    {
        Single num = po.pos[0];
        Single num2 = pot.pos[0];
        Single num3 = po.pos[2];
        Single num4 = pot.pos[2];
        Single deltaX = num2 - num;
        Single deltaZ = num4 - num3;
        Single num5 = angleAsm(deltaX, deltaZ);
        num = myrot - num5;
        if (num > 180f)
        {
            num -= 360f;
        }
        else if (num < -180f)
        {
            num += 360f;
        }
        return ConvertFloatAngleToFixedPoint(num);
    }

    public Int32 EvaluateValueExpression()
    {
        _calcStack.pop(out var t0);
        VariableType varType = getVarType(t0);
        VariableSource cls = getVarClass(t0);
        switch (cls)
        {
            case VariableSource.Global:
                return GetVariableValueInternal(FF9StateSystem.EventState.gEventGlobal, t0 & 0xFFFF, varType, 0);
            case VariableSource.Map:
                return GetVariableValueInternal(_eventEngine.GetMapVar(), t0 & 0xFFFF, varType, 0);
            case VariableSource.Instance:
                return GetVariableValueInternal(_instance, t0 & 0xFFFF, varType, _instanceVOfs);
            case VariableSource.Null:
                switch (varType)
                {
                    case VariableType.Any:
                        return GetMemoriaCustomVariable((memoria_variable)(t0 & 0xFFFF));
                    case VariableType.Vector:
                    {
                        List<Int32> subs = _calcStack.getSubs();
                        if (subs.Count < 2)
                            return 0;
                        Int32 vectID = subs[0];
                        Int32 arrayIndex = subs[1];
                        if (FF9StateSystem.EventState.gScriptVector.TryGetValue(vectID, out List<Int32> vect) && arrayIndex >= 0 && arrayIndex < vect.Count)
                            return vect[arrayIndex];
                        return 0;
                    }
                    case VariableType.VectorSize:
                    {
                        List<Int32> subs = _calcStack.getSubs();
                        if (subs.Count < 1)
                            return 0;
                        Int32 vectID = subs[0];
                        if (FF9StateSystem.EventState.gScriptVector.TryGetValue(vectID, out List<Int32> vect))
                            return vect.Count;
                        return 0;
                    }
                    case VariableType.Dictionary:
                    {
                        List<Int32> subs = _calcStack.getSubs();
                        if (subs.Count < 2)
                            return 0;
                        Int32 dictID = subs[0];
                        Int32 entryID = subs[1];
                        if (FF9StateSystem.EventState.gScriptDictionary.TryGetValue(dictID, out Dictionary<Int32, Int32> dict) && dict.TryGetValue(entryID, out Int32 val))
                            return val;
                        return 0;
                    }
                }
                return 0;
            case VariableSource.Object:
                _calcValue = getvobj(_eventEngine.GetObjByUID((t0 >> 8) & 0xFF, _eventEngine.GetObjectModIndex(currentObject)), t0 & 0xFF);
                return _calcValue;
            case VariableSource.System:
                _calcValue = _eventEngine.GetSysList(t0 & 0xFF);
                return _calcValue;
            case VariableSource.Member:
                _calcValue = getvobj(_eventEngine.gMemberTarget, (t0 << 6) >> 6);
                return _calcValue;
            case VariableSource.Int26:
                _calcValue = (t0 << 6) >> 6;
                return _calcValue;
        }
        return 0;
    }

    private Int32 GetMemoriaCustomVariable(memoria_variable varCode)
    {
        switch (varCode) // Custom variables for HW (ScriptAPI.txt)
        {
            case memoria_variable.TETRA_MASTER_WIN:
                return FF9StateSystem.MiniGame.SavedData.sWin;
            case memoria_variable.TETRA_MASTER_LOSS:
                return FF9StateSystem.MiniGame.SavedData.sLose;
            case memoria_variable.TETRA_MASTER_DRAW:
                return FF9StateSystem.MiniGame.SavedData.sDraw;
            case memoria_variable.TETRA_MASTER_POINTS:
                return QuadMistDatabase.MiniGame_GetPlayerPoints();
            case memoria_variable.TETRA_MASTER_RANK:
                return QuadMistDatabase.MiniGame_GetCollectorLevel();
            case memoria_variable.TREASURE_HUNTER_POINTS:
                return FF9StateSystem.EventState.GetTreasureHunterPoints();
            case memoria_variable.BATTLE_RUNAWAY:
                return FF9StateSystem.Battle.FF9Battle.btl_scene.Info.Runaway ? 1 : 0;
            case memoria_variable.BATTLE_NOGAMEOVER:
                return FF9StateSystem.Battle.FF9Battle.btl_scene.Info.NoGameOver ? 1 : 0;
            case memoria_variable.BATTLE_WINPOSE:
                return FF9StateSystem.Battle.FF9Battle.btl_scene.Info.WinPose ? 1 : 0;
            case memoria_variable.BATTLE_IPSENCURSE:
                return FF9StateSystem.Battle.FF9Battle.btl_scene.Info.ReverseAttack ? 1 : 0;
            case memoria_variable.BATTLE_AFTEREVENT:
                return FF9StateSystem.Battle.FF9Battle.btl_scene.Info.AfterEvent ? 1 : 0;
            case memoria_variable.TOTAL_BATTLE_COUNT:
                return FF9StateSystem.Common.FF9.party.battle_no;
            case memoria_variable.TOTAL_ESCAPE_COUNT:
                return FF9StateSystem.Common.FF9.party.escape_no;
            case memoria_variable.CURRENT_LANGUAGE:
                return Localization.CurrentLanguageId;

        }
        return 0;
    }

    private void SetMemoriaCustomVariable(memoria_variable varCode, Int32 val)
    {
        switch (varCode)
        {
            case memoria_variable.TETRA_MASTER_WIN:
                FF9StateSystem.MiniGame.SavedData.sWin = (Int16)val;
                break;
            case memoria_variable.TETRA_MASTER_LOSS:
                FF9StateSystem.MiniGame.SavedData.sLose = (Int16)val;
                break;
            case memoria_variable.TETRA_MASTER_DRAW:
                FF9StateSystem.MiniGame.SavedData.sDraw = (Int16)val;
                break;
            case memoria_variable.BATTLE_RUNAWAY:
                FF9StateSystem.Battle.FF9Battle.btl_scene.Info.Runaway = val != 0;
                break;
            case memoria_variable.BATTLE_NOGAMEOVER:
                FF9StateSystem.Battle.FF9Battle.btl_scene.Info.NoGameOver = val != 0;
                break;
            case memoria_variable.BATTLE_WINPOSE:
                FF9StateSystem.Battle.FF9Battle.btl_scene.Info.WinPose = val != 0;
                break;
            case memoria_variable.BATTLE_IPSENCURSE:
                FF9StateSystem.Battle.FF9Battle.btl_scene.Info.ReverseAttack = val != 0;
                break;
            case memoria_variable.BATTLE_AFTEREVENT:
                FF9StateSystem.Battle.FF9Battle.btl_scene.Info.AfterEvent = val != 0;
                break;
            case memoria_variable.TOTAL_BATTLE_COUNT:
                FF9StateSystem.Common.FF9.party.battle_no = val;
                break;
            case memoria_variable.TOTAL_ESCAPE_COUNT:
                FF9StateSystem.Common.FF9.party.escape_no = (UInt16)val;
                break;
        }
    }

    private Int32 getvobj(Obj obj, Int32 type)
    {
        Int32 result;
        switch (type)
        {
            case 0:
            {
                if (obj.cid != 4)
                {
                    return 0;
                }
                result = CastFloatToIntWithChecking(((PosObj)obj).pos[0]);
                WMActor wmActor = ((Actor)obj).wmActor;
                if (wmActor != null)
                {
                }
                break;
            }
            case 1:
            {
                if (obj.cid != 4)
                {
                    return 0;
                }
                result = -1 * CastFloatToIntWithChecking(((PosObj)obj).pos[1]);
                WMActor wmActor = ((Actor)obj).wmActor;
                if (wmActor != null)
                {
                }
                break;
            }
            case 2:
            {
                if (obj.cid != 4)
                {
                    return 0;
                }
                result = CastFloatToIntWithChecking(((PosObj)obj).pos[2]);
                WMActor wmActor = ((Actor)obj).wmActor;
                if (wmActor != null)
                {
                }
                break;
            }
            case 3:
            {
                Single floatAngle = 0f;
                if (_eventEngine.gMode == 1)
                {
                    floatAngle = ((Actor)obj).rotAngle[1];
                }
                else if (_eventEngine.gMode == 3)
                {
                    floatAngle = ((Actor)obj).wmActor.rot1;
                }
                Int32 num = ConvertFloatAngleToFixedPoint(floatAngle);
                Int32 num2 = num >> 4 & 255;
                result = num2;
                break;
            }
            case 4:
                result = obj.flags;
                break;
            case 5:
                result = obj.uid;
                break;
            case 6:
                result = obj.level;
                break;
            case 7:
                result = ((PosObj)obj).animFrame;
                break;
            default:
                result = PersistenSingleton<EventEngine>.Instance.GetBattleCharData(obj, type);
                break;
        }
        return result;
    }

    private static Int32 CastFloatToIntWithChecking(Single floatValue)
    {
        Int32 int1 = Mathf.FloorToInt(floatValue);
        Int32 int2 = Mathf.CeilToInt(floatValue);
        Int32 int3 = Mathf.RoundToInt(floatValue);
        if (int3 == int2)
            return int2;
        if (int3 == int1)
            return int1;
        return -1;
    }

    private Single distance(Single x, Single y, Single z)
    {
        return Mathf.Sqrt(x * x + y * y + z * z);
    }

    public Int32 GetVariableValueInternal(Byte[] buffer, Int32 ofs, VariableType type, Int32 bufferOffset = 0)
    {
        switch (type)
        {
            case VariableType.SBit:
            case VariableType.Bit:
            {
                Byte bitFlags = buffer[(ofs >> 3) + bufferOffset]; // (767 bit >> 3) == (767 bit / 8) == 95 byte 
                _calcValue = (bitFlags >> (ofs & 7)) & 1; // (1 bit & 1) => result
                return _calcValue;
            }
            case VariableType.Int24:
            case VariableType.UInt24:
                _calcValue = buffer[ofs + bufferOffset] | (buffer[ofs + 1 + bufferOffset] << 8) | ((SByte)buffer[ofs + 2 + bufferOffset] << 16);
                return _calcValue;
            case VariableType.SByte:
                _calcValue = (SByte)buffer[ofs + bufferOffset];
                return _calcValue;
            case VariableType.Byte:
                _calcValue = buffer[ofs + bufferOffset];
                return _calcValue;
            case VariableType.Int16:
                _calcValue = buffer[ofs + bufferOffset] | ((SByte)buffer[ofs + 1 + bufferOffset] << 8);
                return _calcValue;
            case VariableType.UInt16:
                _calcValue = buffer[ofs + bufferOffset] | (buffer[ofs + 1 + bufferOffset] << 8);
                return _calcValue;
            default:
                return 0;
        }
    }

    public Int32 getv1i(ref Int32 varargflag)
    {
        _calcValue = varargflag & 1;
        varargflag >>= 1;
        if (_calcValue != 0)
        {
            expr();
            _calcValue = EvaluateValueExpression();
            return _calcValue;
        }
        _calcValue = currentObject.getByteIP();
        currentObject.ip++;
        return _calcValue;
    }

    public Int32 SetVariableValue(Int32 arg0)
    {
        _calcStack.pop(out var t0);
        Int32 varValue = arg0;
        VariableType varType = getVarType(t0);
        VariableSource cls = getVarClass(t0);
        switch (cls)
        {
            case VariableSource.Global:
                SetVariableValueInternal(FF9StateSystem.EventState.gEventGlobal, t0 & 0xFFFF, varType, varValue, 0);
                break;
            case VariableSource.Map:
                SetVariableValueInternal(_eventEngine.GetMapVar(), t0 & 0xFFFF, varType, varValue, 0);
                break;
            case VariableSource.Instance:
                SetVariableValueInternal(_instance, t0 & 0xFFFF, varType, varValue, _instanceVOfs);
                break;
            case VariableSource.Null:
                switch (varType)
                {
                    case VariableType.Any:
                        SetMemoriaCustomVariable((memoria_variable)(t0 & 0xFFFF), varValue);
                        break;
                    case VariableType.Vector:
                    {
                        List<Int32> subs = _calcStack.getSubs();
                        if (subs.Count < 2)
                            break;
                        Int32 vectID = subs[0];
                        Int32 arrayIndex = subs[1];
                        if (FF9StateSystem.EventState.gScriptVector.TryGetValue(vectID, out List<Int32> vect))
                        {
                            if (arrayIndex == vect.Count)
                                vect.Add(varValue);
                            else if (arrayIndex >= 0 && arrayIndex < vect.Count)
                                vect[arrayIndex] = varValue;
                        }
                        else if (arrayIndex == 0)
                        {
                            vect = new List<Int32>();
                            vect.Add(varValue);
                            FF9StateSystem.EventState.gScriptVector.Add(vectID, vect);
                        }
                        break;
                    }
                    case VariableType.VectorSize:
                    {
                        List<Int32> subs = _calcStack.getSubs();
                        if (subs.Count < 1 || varValue < 0)
                            break;
                        Int32 vectID = subs[0];
                        if (FF9StateSystem.EventState.gScriptVector.TryGetValue(vectID, out List<Int32> vect))
                        {
                            if (varValue > vect.Count)
                                vect.AddRange(Enumerable.Repeat(0, varValue - vect.Count));
                            else if (varValue < vect.Count)
                                vect.RemoveRange(varValue, vect.Count - varValue);
                        }
                        else
                        {
                            vect = new List<Int32>(varValue);
                            vect.AddRange(Enumerable.Repeat(0, varValue));
                            FF9StateSystem.EventState.gScriptVector.Add(vectID, vect);
                        }
                        break;
                    }
                    case VariableType.Dictionary:
                    {
                        List<Int32> subs = _calcStack.getSubs();
                        if (subs.Count < 2)
                            break;
                        Int32 dictID = subs[0];
                        Int32 entryID = subs[1];
                        if (FF9StateSystem.EventState.gScriptDictionary.TryGetValue(dictID, out Dictionary<Int32, Int32> dict))
                        {
                            dict[entryID] = varValue;
                        }
                        else
                        {
                            dict = new Dictionary<Int32, Int32>();
                            dict[entryID] = varValue;
                            FF9StateSystem.EventState.gScriptDictionary.Add(dictID, dict);
                        }
                        break;
                    }
                }
                break;
            case VariableSource.System:
                _eventEngine.SetSysList(t0 & 0xFF, varValue);
                break;
            case VariableSource.Member:
                _eventEngine.putvobj(_eventEngine.gMemberTarget, t0 & 0xFF, varValue);
                break;
        }
        return 0;
    }

    public Int32 SetVariableValueInternal(Byte[] buffer, Int32 ofs, VariableType type, Int32 value, Int32 bufferOffset = 0)
    {
        switch (type)
        {
            case VariableType.SBit:
            case VariableType.Bit:
            {
                Int32 byteIndex = (ofs >> 3) + bufferOffset;
                if (value == 0)
                    buffer[byteIndex] &= (Byte)~(1 << (ofs & 7));
                else
                    buffer[byteIndex] |= (Byte)(1 << (ofs & 7));
                break;
            }

            case VariableType.Int24:
            case VariableType.UInt24:
                if (EventHUD.CurrentHUD == MinigameHUD.JumpingRope && Configuration.Hacks.Enabled && (ofs == 43 || ofs == 59))
                {
                    Int32 rewardStep = Int32.MaxValue;
                    if (value <= 20) rewardStep = 20;
                    else if (value <= 50 && QuadMistDatabase.MiniGame_GetCardCount(TetraMasterCardId.Cactuar) == 0) rewardStep = 50;
                    else if (value <= 100 && QuadMistDatabase.MiniGame_GetCardCount(TetraMasterCardId.Genji) == 0) rewardStep = 100;
                    else if (value <= 200 && QuadMistDatabase.MiniGame_GetCardCount(TetraMasterCardId.Alexandria) == 0) rewardStep = 200;
                    else if (value <= 300 && QuadMistDatabase.MiniGame_GetCardCount(TetraMasterCardId.TigerRacket) == 0) rewardStep = 300;
                    else if (value <= 1000) rewardStep = 1000;
                    value = Math.Min(value - 1 + Configuration.Hacks.RopeJumpingIncrement, rewardStep);
                }
                buffer[ofs + bufferOffset] = (Byte)(value & 0xFF);
                buffer[ofs + 1 + bufferOffset] = (Byte)((value >> 8) & 0xFF);
                buffer[ofs + 2 + bufferOffset] = (Byte)((value >> 16) & 0xFF);
                break;

            case VariableType.SByte:
            case VariableType.Byte:
                buffer[ofs + bufferOffset] = (Byte)value;
                break;
            case VariableType.Int16:
            case VariableType.UInt16:
                buffer[ofs + bufferOffset] = (Byte)(value & 0xFF);
                buffer[ofs + 1 + bufferOffset] = (Byte)((value >> 8) & 0xFF);
                break;
        }
        return 0;
    }

    public void SetVariableSpec(ref Int32 arg0)
    {
        _nextCodeIndex = currentObject.getByteIP();
        arg0 = currentObject.getByteIP();
    }

    public Int32 CalcExpr()
    {
        Obj obj = currentObject;
        //Int32 num = _s3;
        Int32 num2 = _nextCodeIndex;
        CalcStack calcStack = _calcStack;
        currentObject = _eventEngine.gExec;
        expr();
        currentObject = obj;
        //_s3 = num;
        _nextCodeIndex = num2;
        _calcStack = calcStack;
        return 0;
    }

    public Int32 getv()
    {
        CalcStack calcStack = _calcStack;
        _calcStack = _eventEngine.gCP;
        _calcValue = EvaluateValueExpression();
        _calcStack = calcStack;
        return _calcValue;
    }

    public Int32 putv(Int32 a)
    {
        CalcStack calcStack = _calcStack;
        _calcStack = _eventEngine.gCP;
        _calcValue = SetVariableValue(a);
        _calcStack = calcStack;
        return _calcValue;
    }

    private VariableSource getVarClass(Int32 value)
    {
        return (VariableSource)((value >> 26) & 7);
    }

    private VariableType getVarType(Int32 value)
    {
        return (VariableType)((value >> 29) & 7);
    }

    private Int32 encodeTypeAndVarClass(VariableSource varSrc, VariableType varType)
    {
        return ((Int32)varSrc << 26) | ((Int32)varType << 29);
    }

    private Int32 encodeVarClass(VariableSource varSrc)
    {
        return (Int32)varSrc << 26;
    }

    public enum event_code_binary
    {
        NOP,
        rsv01,
        rsv02,
        rsv03,
        rsv04,
        EXPR,
        rsv06,
        NEW,
        NEW2,
        NEW3,
        pad0a,
        rsv0b,
        rsv0c,
        rsv0d,
        pad0e,
        pad0f,
        REQ,
        pad11,
        REQSW,
        pad13,
        REQEW,
        pad15,
        REPLY,
        pad17,
        REPLYSW,
        pad19,
        REPLYEW,
        SONGFLAG,
        DELETE,
        POS,
        BGVPORT,
        MES,
        MESN,
        CLOSE,
        WAIT,
        MOVE,
        MOVA,
        CLRDIST,
        MSPEED,
        BGIMASK,
        FMV,
        QUAD,
        ENCOUNT,
        MAPJUMP,
        CC,
        UCOFF,
        UCON,
        MODEL,
        PRINT1,
        PRINTF,
        LOCATE,
        AIDLE,
        AWALK,
        ARUN,
        DIRE,
        ROTXZ,
        BTLCMD,
        MESHSHOW,
        MESHHIDE,
        OBJINDEX,
        ENCSCENE,
        AFRAME,
        ASPEED,
        AMODE,
        ANIM,
        WAITANIM,
        ENDANIM,
        STARTSEQ,
        WAITSEQ,
        ENDSEQ,
        DEBUGCC,
        NECKFLAG,
        ITEMADD,
        ITEMDELETE,
        BTLSET,
        RADIUS,
        ATTACH,
        DETACH,
        WATCH,
        STOP,
        WAITTURN,
        TURNA,
        ASLEEP,
        NOINITMES,
        WAITMES,
        MROT,
        TURN,
        ENCRATE,
        BGSMOVE,
        BGLCOLOR,
        BGLMOVE,
        BGLACTIVE,
        BGLLOOP,
        BGLPARALLAX,
        BGLORIGIN,
        BGAANIME,
        BGAACTIVE,
        BGARATE,
        SETROW,
        BGAWAIT,
        BGAFLAG,
        BGARANGE,
        MESVALUE,
        TWIST,
        FICON,
        TIMERSET,
        DASHOFF,
        CLEARCOLOR,
        PPRINT,
        PPRINTF,
        MAPID,
        BGSSCROLL,
        BGSRELEASE,
        BGCACTIVE,
        BGCHEIGHT,
        BGCLOCK,
        BGCUNLOCK,
        MENU,
        TRACKSTART,
        TRACK,
        TRACKADD,
        PRINTQUAD,
        ATURNL,
        ATURNR,
        CHOOSEPARAM,
        TIMERCONTROL,
        SETCAM,
        SHADOWON,
        SHADOWOFF,
        SHADOWSCALE,
        SHADOWOFFSET,
        SHADOWLOCK,
        SHADOWUNLOCK,
        SHADOWAMP,
        IDLESPEED,
        DDIR,
        CHRFX,
        SEPV,
        SEPVA,
        NECKID,
        ENCOUNT2,
        TIMERDISPLAY,
        RAISE,
        CHRCOLOR,
        SLEEPINH,
        AUTOTURN,
        BGLATTACH,
        CFLAG,
        AJUMP,
        MESA,
        MESAN,
        DRET,
        MOVT,
        TSPEED,
        BGIACTIVET,
        TURNTO,
        PREJUMP,
        POSTJUMP,
        MOVQ,
        CHRSCALE,
        MOVJ,
        POS3,
        MOVE3,
        DRADIUS,
        MJPOS,
        MOVH,
        SPEEDTH,
        TURNDS,
        BGI,
        GETSCREEN,
        MENUON,
        MENUOFF,
        DISCCHANGE,
        DPOS3,
        MINIGAME,
        DELETEALLCARD,
        SETMAPNAME,
        RESETMAPNAME,
        PARTYMENU,
        SPS,
        FULLMEMBER,
        PRETEND,
        WMAPJUMP,
        EYE,
        AIM,
        SETKEYMASK,
        CLEARKEYMASK,
        DTURN,
        DWAITTURN,
        DANIM,
        DWAITANIM,
        DPOS,
        TEXPLAY,
        TEXPLAY1,
        TEXSTOP,
        BGVSET,
        WPRM,
        FLDSND0,
        FLDSND1,
        FLDSND2,
        FLDSND3,
        BGVDEFINE,
        BGAVISIBLE,
        BGIACTIVEF,
        CHRSET,
        CHRCLEAR,
        GILADD,
        GILDELETE,
        MESB,
        GLOBALCLEAR,
        DEBUGSAVE,
        DEBUGLOAD,
        ATTACHOFFSET,
        PUSHHIDE,
        POPSHOW,
        AICON,
        RAIN,
        CLEARSTATUS,
        SPS2,
        WINPOSE,
        JUMP3,
        PARTYDELETE,
        PLAYERNAME,
        OVAL,
        INCFROG,
        BEND,
        SETVY3,
        SETSIGNAL,
        BGLSCROLLOFFSET,
        BTLSEQ,
        BGLLOOPTYPE,
        BGAFRAME,
        MOVE3H,
        SYNCPARTY,
        VRP,
        CLOSEALL,
        WIPERGB,
        BGVALPHA,
        SLEEPON,
        HEREON,
        DASHON,
        SETHP,
        SETMP,
        CLEARAP,
        MAXAP,
        GAMEOVER,
        VIBSTART,
        VIBACTIVE,
        VIBTRACK1,
        VIBTRACK,
        VIBRATE,
        VIBFLAG,
        VIBRANGE,
        HINT,
        JOIN,
        EXT, // 0xFF
        BSSTART,
        BSFRAME,
        BSACTIVE,
        BSFLAG,
        BSFLOOR,
        BSRATE,
        BSALGO,
        BSDELTA,
        BSAXIS,
        BAANIME,
        BAFRAME,
        BAACTIVE,
        BAFLAG,
        BARATE,
        BAWAITALL,
        BAWAIT,
        BARANGE,
        BAVISIBLE,
        // Custom Memoria codes
        PLAYER_EQUIP,
        PLAYER_LEVEL,
        PLAYER_EXP,
        SHOP_ITEM,
        SHOP_SYNTH,
        MOVE_EX,
        TURN_OBJ_EX,
        AANIM_EX,
        VECTOR_CLEAR,
        DICTIONARY_CLEAR,
        BGLMOVE_TIMED,
        ADD_STATUS,
        REMOVE_STATUS,
    }

    public enum flexible_varfunc : ushort
    {
        // Custom Memoria codes
        ITEM_REGULAR_TO_ID,
        ITEM_ID_TO_REGULAR,
        ITEM_KEY_TO_ID,
        ITEM_ID_TO_KEY,
        ITEM_CARD_TO_ID,
        ITEM_ID_TO_CARD,
        ABILITY_ACTIVE_TO_ID,
        ABILITY_ID_TO_ACTIVE,
        ABILITY_SUPPORT_TO_ID,
        ABILITY_ID_TO_SUPPORT,
        PARTY_MEMBER,
        ITEM_FULL_COUNT,
        PLAYER_EQUIP,
        PLAYER_LEVEL,
        PLAYER_EXP,
        PLAYER_EXP_REQ,
        PLAYER_ABILITY_LEARNT,
        PLAYER_SUPPORT_ENABLED,
        SHOP_ITEM,
        SHOP_SYNTH,
        VECTOR,
        VECTOR_SIZE,
        DICTIONARY,
        CATEGORY_KILL_COUNT,
        MODEL_KILL_COUNT,
        ABILITY_USE_COUNT,
    }

    public enum memoria_variable : ushort
    {
        // Custom Memoria codes
        TETRA_MASTER_WIN,
        TETRA_MASTER_LOSS,
        TETRA_MASTER_DRAW,
        TETRA_MASTER_POINTS,
        TETRA_MASTER_RANK,
        TREASURE_HUNTER_POINTS,
        BATTLE_RUNAWAY,
        BATTLE_NOGAMEOVER,
        BATTLE_WINPOSE,
        BATTLE_IPSENCURSE,
        BATTLE_AFTEREVENT,
        TOTAL_BATTLE_COUNT,
        TOTAL_ESCAPE_COUNT,
        CURRENT_LANGUAGE,
    }

    public enum op_binary
    {
        B_PAD0 = 0,
        B_PAD1 = 1,
        B_PAD2 = 2,
        B_PAD3 = 3,
        B_POST_PLUS = 4,
        B_POST_MINUS = 5,
        B_PRE_PLUS = 6,
        B_PRE_MINUS = 7,
        B_POST_PLUS_A = 8,
        B_POST_MINUS_A = 9,
        B_PRE_PLUS_A = 10,
        B_PRE_MINUS_A = 11,
        B_SINGLE_PLUS = 12,
        B_SINGLE_MINUS = 13,
        B_NOT = 14,
        B_NOT_E = 15,
        B_COMP = 16,
        B_MULT = 17,
        B_DIV = 18,
        B_REM = 19,
        B_PLUS = 20,
        B_MINUS = 21,
        B_SHIFT_LEFT = 22,
        B_SHIFT_RIGHT = 23,
        B_LT = 24,
        B_GT = 25,
        B_LE = 26,
        B_GE = 27,
        B_LT_E = 28,
        B_GT_E = 29,
        B_LE_E = 30,
        B_GE_E = 31,
        B_EQ = 32,
        B_NE = 33,
        B_EQ_E = 34,
        B_NE_E = 35,
        B_AND = 36,
        B_XOR = 37,
        B_OR = 38,
        B_ANDAND = 39,
        B_OROR = 40,
        B_MEMBER = 41,
        B_COUNT = 42,
        B_PICK = 43,
        B_LET = 44,
        B_LET_A = 45,
        B_LET_E = 46,
        B_MULT_LET = 47,
        B_DIV_LET = 48,
        B_REM_LET = 49,
        B_PLUS_LET = 50,
        B_MINUS_LET = 51,
        B_SHIFT_LEFT_LET = 52,
        B_SHIFT_RIGHT_LET = 53,
        B_MULT_LET_A = 54,
        B_DIV_LET_A = 55,
        B_REM_LET_A = 56,
        B_PLUS_LET_A = 57,
        B_MINUS_LET_A = 58,
        B_SHIFT_LEFT_LET_A = 59,
        B_SHIFT_RIGHT_LET_A = 60,
        B_AND_LET = 61,
        B_XOR_LET = 62,
        B_OR_LET = 63,
        B_AND_LET_A = 64,
        B_XOR_LET_A = 65,
        B_OR_LET_A = 66,
        B_AND_LET_E = 67,
        B_XOR_LET_E = 68,
        B_OR_LET_E = 69,
        B_CAST8 = 70,
        B_CAST8U = 71,
        B_CAST16 = 72,
        B_CAST16U = 73,
        B_CAST_LIST = 74,
        B_LMAX = 75,
        B_LMIN = 76,
        B_SELECT = 77,
        B_OBJSPEC = 78,
        B_KEYON = 79,
        B_SIN2 = 80,
        B_COS2 = 81,
        B_CURHP = 82,
        B_MAXHP = 83,
        B_AND_E = 84,
        B_NAND_E = 85,
        B_XOR_E = 86,
        B_OR_E = 87,
        B_KEYOFF = 88,
        B_KEY = 89,
        B_KEYON2 = 90,
        B_KEYOFF2 = 91,
        B_KEY2 = 92,
        B_ANGLE = 93,
        B_DISTANCE = 94,
        B_PTR = 95,
        B_ANGLEA = 96,
        B_DISTANCEA = 97,
        B_SIN = 98,
        B_COS = 99,
        B_HAVE_ITEM = 100,
        B_BAFRAME = 101,
        B_ANGLE2 = 102,
        pad67 = 103,
        pad68 = 104,
        pad69 = 105,
        B_FRAME = 106,
        B_PARTYCHK = 107,
        B_SPS = 108,
        B_PARTYADD = 109,
        B_CURMP = 110,
        B_MAXMP = 111,
        B_BGIID = 112,
        B_BGIFLOOR = 113,
        B_OBJSPECA = 120,
        B_SYSLIST = 121,
        B_SYSVAR = 122,
        B_pad7b = 123,
        B_PAD4 = 124,
        B_CONST = 125,
        B_CONST4 = 126,
        B_EXPR_END = 127,
        B_VAR = 0xC0
    }

    public enum VariableSource
    {
        Global = 0,
        Map = 1,
        Instance = 2,
        Null = 3,
        Object = 4,
        System = 5,
        Member = 6,
        Int26 = 7
    }

    public enum VariableType
    {
        SBit = 0,
        Bit = 1,
        Int24 = 2,
        UInt24 = 3,
        SByte = 4,
        Byte = 5,
        Int16 = 6,
        UInt16 = 7,
        Any = 0,
        Vector = 1,
        VectorSize = 2,
        Dictionary = 3,
    }
}
