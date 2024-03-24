using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Assets.Sources.Scripts.EventEngine.Utils;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Assets;
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

    public static ObjList s0;
    public static Obj s1;
    public static Obj objV0;
    public static Int32 SC_COUNTER_SVR = 220;
    public static Int32 MAP_INDEX_SVR = 728;

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

    private static Int32 _v0;
    //private static Int32 _a0;
    //private static Int32 _a1;
    //private static Int32 _a2;
    // private static Int32 _a3;
    private static Int32 _s2;
    //private static Int32 _s3;
    //private static Int32 _s5;
    private static Int32 _nextCodeIndex;
    //private static CalcStack _s4;
    private static CalcStack _s7;
    //private static CalcStack _tempS4;
    private static CalcStack _tempStack = new CalcStack();
    //private static Int32 _t0;
    //private static Int32 _t2;
    //private static Int32 _t3;
    //private static Int32 _t4;

    private readonly EventEngine _eventEngine;
    private readonly ETb _eTb;
    private Byte[] _instance;
    private Int32 _instanceVOfs;
    private Boolean _exprLoop;
    private Boolean _objectExists;
    private Boolean _nextLoop;
    private Byte[] _ratanTbl;

    public EBin(EventEngine ee)
    {
        _eventEngine = ee;
        _eTb = _eventEngine.eTb;
        InitializeATanTable();
        _s7 = calcstack;
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
        UInt16 num = _ratanTbl[offset];
        return (UInt16)(num | (UInt16)(_ratanTbl[offset + 1] << 8));
    }

    public Int32 ProcessCode(ObjList objList)
    {
        Int32 result = -1;
        _objectExists = true;
        s0 = objList;
        next1();
        while (_objectExists)
        {
            s1 = s0.obj;
            int a1 = s1.state;
            if (a1 == EventEngine.stateNew)
            {
                Int32 state = EventEngine.stateInit;
                s1.state = (Byte)state;
                next0();
                continue;
            }

            Int32 a0 = EventEngine.stateSuspend;
            _s2 = 0;
            if (a1 == a0)
            {
                next0();
                continue;
            }

            _nextCodeIndex = s1.ip;
            a0 = s1.wait;
            if (_nextCodeIndex == _eventEngine.nil)
            {
                next0();
                continue;
            }

            Int32 a2 = 1;
            if (a0 != 0)
            {
                a1 = 255;
                if (a0 != 254)
                {
                    if (a0 == a1)
                    {
                        next0();
                    }
                    else
                    {
                        a0 = s1.wait;
                        a0--;
                        s1.wait = (Byte)a0;
                        next0();
                    }
                }
                else
                {
                    a0 = s1.winnum;
                    if (a0 == 255)
                    {
                        ad4();
                    }
                    else
                    {
                        Boolean flag = _eTb.MesWinActive(a0);
                        a0 = 255;
                        if (flag)
                        {
                            next0();
                        }
                        else
                        {
                            s1.winnum = (Byte)a0;
                            ad4();
                        }
                    }
                }
                continue;
            }

            a1 = s1.vofs;
            _eventEngine.gExec = s1;
            a1 <<= 2;
            a0 = s1.cid;
            _instance = s1.buffer;
            _instanceVOfs = a1;
            objV0 = s1;
            _v0 = s1.ip;
            if (a0 != a2)
            {
                result = ad3(a0);
            }
            else
            {
                a0 = s1.uid;
                a0 -= 64;
                objV0 = _eventEngine.FindObjByUID(a0);
                result = ad3(a0);
            }
            objV0 = null;
        }
        return result;
    }

    private void adFin()
    {
        _nextLoop = false;
        _objectExists = false;
    }

    public Int32 ad3(Int32 arg0)
    {
        Int32 gMode = _eventEngine.gMode;
        _eventEngine.gCur = objV0;
        Int32 result;
        if (gMode != 2)
        {
            result = next(gMode);
        }
        else
        {
            _eventEngine.ProcessCodeExt(s1);
            result = next(gMode);
        }
        return result;
    }

    private void ad4()
    {
        Int32 a0 = 0;
        s1.wait = (Byte)a0;
        next0();
    }

    public Int32 next(Int32 gMode)
    {
        _nextLoop = true;
        while (_nextLoop)
        {
            if (_s2 == 0)
            {
                Int32 a0 = s1.getByteIP();
                if (s1.sid == 1 || s1.sid == 0)
                {
                }
                if (a0 >= 110)
                {
                    commandDefault2();
                }
                else
                {
                    s1.ip++;
                    jumpToCommand(a0);
                    if (FF9StateSystem.Settings.IsFastTrophyMode)
                    {
                        EMinigame.DigUpMadianRingCheating();
                    }
                }
            }
            else
            {
                adfr();
            }
        }
        return _v0;
    }

    public void adfr()
    {
        _v0 = _s2;
        if (_s2 == 4)
        {
            adFin();
        }
        else if (_s2 == 5)
        {
            adFin();
        }
        else
        {
            Int32 a0 = 3;
            if (_s2 == a0)
            {
                adFin();
            }
            else
            {
                a0 = 7;
                if (_s2 == a0)
                {
                    adFin();
                }
                else
                {
                    a0 = 8;
                    if (_s2 == a0)
                    {
                        adFin();
                    }
                    else
                    {
                        a0 = 6;
                        if (_s2 != a0)
                        {
                            a0 = 2;
                            if (_s2 == a0)
                            {
                                next1();
                            }
                            else
                            {
                                next0();
                            }
                        }
                        else
                        {
                            a0 = 2;
                            _eventEngine.gStopObj = s0;
                            adFin();
                        }
                    }
                }
            }
        }
    }

    public void next0()
    {
        if (s0 != null)
        {
            getVarManually(6357);
            s0 = s0.next;
        }
        next1();
    }

    public void next1()
    {
        _v0 = 0;
        if (s0 != null)
        {
            _nextLoop = false;
        }
        else
        {
            _nextLoop = false;
            _objectExists = false;
        }
    }

    public Int32 expr()
    {
        _s7 = calcstack;
        //_tempS4 = _s4;
        //_s4 = _eventEngine.gCP;
        _s7.emptyCalcStack();
        _exprLoop = true;
        while (_exprLoop)
        {
            Byte varOperation = s1.getByteIP();
            if (s1.sid != 3 || s1.ip != 110)
            {
                if (FF9StateSystem.Settings.IsFastTrophyMode)
                {
                    if (FF9StateSystem.Common.FF9.fldMapNo == 2801 && s1.sid == 11 && s1.ip == 3834) // Daguerreo/Right Hall, Gilgamesh
                    {
                        setVarManually(11989, 8);
                    }
                    if (FF9StateSystem.Common.FF9.fldMapNo == 1900 && s1.sid == 0 && s1.ip == 4138) // Treno/Pub, Main
                    {
                        setVarManually(8198869, 8);
                    }
                }
            }
            if (FF9StateSystem.Common.FF9.fldMapNo == 705 && s1.sid == 3 && s1.ip == 541) // Gizamaluke/Bell Room, Female Moogle
            {
                s1.ip += 7;
                return 0;
            }
            EMinigame.ChanbaraBonusPoints(s1, this);
            EMinigame.SetViviSpeed(s1, this);
            s1.ip++;
            if (varOperation >= 0x80)
            {
                if (varOperation == 0xD3)
                {
                    expr_customSubCommand();
                }
                else
                {
                    _v0 = expr_varSpec(varOperation);
                    _s7.push(_v0);
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
        flexible_varfunc commandId = (flexible_varfunc)s1.getUShortIP();
        s1.ip += 2;
        Byte argCount = s1.getByteIP();
        s1.ip++;
        Int32[] args = new Int32[argCount];
        for (Int32 i = argCount - 1; i >= 0; i--)
            args[i] = EvaluateValueExpression();
        _v0 = 0;
        switch (commandId)
        {
            case flexible_varfunc.ITEM_REGULAR_TO_ID:
                _v0 = ff9item.GetItemIdFromRegularId((RegularItem)args[0]);
                break;
            case flexible_varfunc.ITEM_ID_TO_REGULAR:
                _v0 = (Int32)ff9item.GetRegularIdFromItemId(args[0]);
                break;
            case flexible_varfunc.ITEM_KEY_TO_ID:
                _v0 = ff9item.GetItemIdFromImportantId(args[0]);
                break;
            case flexible_varfunc.ITEM_ID_TO_KEY:
                _v0 = (Int32)ff9item.GetImportantIdFromItemId(args[0]);
                break;
            case flexible_varfunc.ITEM_CARD_TO_ID:
                _v0 = ff9item.GetItemIdFromCardId((TetraMasterCardId)args[0]);
                break;
            case flexible_varfunc.ITEM_ID_TO_CARD:
                _v0 = (Int32)ff9item.GetCardIdFromItemId(args[0]);
                break;
            case flexible_varfunc.ABILITY_ACTIVE_TO_ID:
                _v0 = ff9abil.GetAbilityIdFromActiveAbility((BattleAbilityId)args[0]);
                break;
            case flexible_varfunc.ABILITY_ID_TO_ACTIVE:
                _v0 = (Int32)ff9abil.GetActiveAbilityFromAbilityId(args[0]);
                break;
            case flexible_varfunc.ABILITY_SUPPORT_TO_ID:
                _v0 = ff9abil.GetAbilityIdFromSupportAbility((SupportAbility)args[0]);
                break;
            case flexible_varfunc.ABILITY_ID_TO_SUPPORT:
                _v0 = (Int32)ff9abil.GetSupportAbilityFromAbilityId(args[0]);
                break;
            case flexible_varfunc.PARTY_MEMBER:
                _v0 = (Int32)ff9play.CharacterIDToOldIndex(FF9StateSystem.Common.FF9.party.GetCharacterId(args[0]));
                break;
            case flexible_varfunc.ITEM_FULL_COUNT:
                _v0 = ff9item.FF9Item_GetAnyCount((RegularItem)args[0]);
                break;
            case flexible_varfunc.PLAYER_EQUIP:
                _v0 = (Int32)(FF9StateSystem.Common.FF9.GetPlayer(ff9play.CharacterOldIndexToID((CharacterOldIndex)args[0]))?.equip[args[1]] ?? RegularItem.NoItem);
                break;
            case flexible_varfunc.PLAYER_LEVEL:
                _v0 = FF9StateSystem.Common.FF9.GetPlayer(ff9play.CharacterOldIndexToID((CharacterOldIndex)args[0]))?.level ?? 0;
                break;
            case flexible_varfunc.PLAYER_EXP:
                _v0 = (Int32)(FF9StateSystem.Common.FF9.GetPlayer(ff9play.CharacterOldIndexToID((CharacterOldIndex)args[0]))?.exp ?? 0);
                break;
            case flexible_varfunc.PLAYER_EXP_REQ:
                if (args[0] <= 0)
                    _v0 = 0;
                else if (args[0] > ff9level.LEVEL_COUNT)
                    _v0 = (Int32)9999999u;
                else
                    _v0 = (Int32)ff9level.CharacterLevelUps[args[0] - 1].ExperienceToLevel;
                break;
            case flexible_varfunc.PLAYER_ABILITY_LEARNT:
            {
                PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(ff9play.CharacterOldIndexToID((CharacterOldIndex)args[0]));
                if (player == null || !ff9abil.FF9Abil_HasAp(new Character(player)))
                    break;
                Int32 abilIndex = ff9abil.FF9Abil_GetIndex(player, args[1]);
                if (abilIndex < 0)
                    break;
                _v0 = player.pa[abilIndex] >= ff9abil._FF9Abil_PaData[player.PresetId][abilIndex].Ap ? 1 : 0;
                if (_v0 == 0 && args[2] != 0)
                    for (Int32 i = 0; i < 5; i++)
                        if (player.equip[i] != RegularItem.NoItem && ff9item._FF9Item_Data[player.equip[i]].ability.Any(id => id == args[1]))
                            _v0 = 1;
                break;
            }
            case flexible_varfunc.PLAYER_SUPPORT_ENABLED:
            {
                PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(ff9play.CharacterOldIndexToID((CharacterOldIndex)args[0]));
                _v0 = player != null && player.saExtended.Contains((SupportAbility)args[1]) ? 1 : 0;
                break;
            }
            case flexible_varfunc.SHOP_ITEM:
                _v0 = ff9buy.ShopItems.ContainsKey(args[0]) && ff9buy.ShopItems[args[0]].ItemIds.Contains((RegularItem)args[1]) ? 1 : 0;
                break;
            case flexible_varfunc.SHOP_SYNTH:
                _v0 = ff9mix.SynthesisData.ContainsKey(args[1]) && ff9mix.SynthesisData[args[1]].Shops.Contains(args[0]) ? 1 : 0;
                break;
            case flexible_varfunc.VECTOR:
                _s7.pushSubs(args[0], args[1]);
                _s7.push(encodeTypeAndVarClass(VariableSource.Null, VariableType.Vector));
                return;
            case flexible_varfunc.VECTOR_SIZE:
                _s7.pushSubs(args[0]);
                _s7.push(encodeTypeAndVarClass(VariableSource.Null, VariableType.VectorSize));
                return;
            case flexible_varfunc.DICTIONARY:
                _s7.pushSubs(args[0], args[1]);
                _s7.push(encodeTypeAndVarClass(VariableSource.Null, VariableType.Dictionary));
                return;
        }
		expr_Push_v0_Int24();
	}

	private Int32 expr_varSpec(Int32 varOperation)
    {
        _v0 = (varOperation & 3) << 26 | (varOperation & 0x1C) << 27;
        Int32 varArrayIndex = s1.getByteIP();
        s1.ip++;
        if ((varOperation & 0x20) != 0)
        {
            _v0 |= varArrayIndex;
            varArrayIndex = s1.getByteIP();
            s1.ip++;
            varArrayIndex <<= 8;
        }
        _v0 |= varArrayIndex;
        return _v0;
    }

    public Int32 setVarManually(Int32 varOperation, Int32 value)
    {
        Int32 varCode = (varOperation & 3) << 26 | (varOperation & 0x1C) << 27;
        Int32 varArrayIndex = (varOperation >> 8) & 0xFF;
        if ((varOperation & 0x20) != 0)
            varArrayIndex |= (varOperation >> 8) & 0xFF00;
        varCode |= varArrayIndex;
        _s7.push(varCode);
        SetVariableValue(value);
        varCode |= encodeVarClass(VariableSource.Int26);
        _s7.push(varCode);

        return varCode;
    }

    public Int32 getVarManually(Int32 varOperation)
    {
        CalcStack calcStack = _s7;
        _s7 = _tempStack;
        _s7.emptyCalcStack();
        Int32 varCode = (varOperation & 3) << 26 | (varOperation & 0x1C) << 27;
        Int32 varArrayIndex = (varOperation & 0xFF00) >> 8;
        if ((varOperation & 0x20) != 0)
            varArrayIndex |= (varOperation >> 8) & 0xFF00;
        varCode |= varArrayIndex;
        _s7.push(varCode);
        Int32 result = EvaluateValueExpression();
        _s7 = calcStack;
        return result;
    }

    private void expr_jumpToSubCommand(op_binary arg0)
    {
        if (arg0 < op_binary.B_PAD0 || arg0 > op_binary.B_EXPR_END)
            return;

        switch (arg0)
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
                _eventEngine.gCP = _s7;
                _v0 = _eventEngine.DoCalcOperationExt(arg0);
                _s7 = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_POST_PLUS:
            {
                _v0 = EvaluateValueExpression();
                Int32 t3 = _v0;
                _s7.advanceTopOfStack();
                Int32 a0 = _v0 + 1;
                SetVariableValue(a0);
                _v0 = t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_POST_MINUS:
            {
                _v0 = EvaluateValueExpression();
                Int32 t3 = _v0;
                _s7.advanceTopOfStack();
                Int32 a0 = _v0 - 1;
                SetVariableValue(a0);
                _v0 = t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PRE_PLUS:
            {
                _v0 = EvaluateValueExpression();
                Int32 t3 = _v0 + 1;
                _s7.advanceTopOfStack();
                Int32 a0 = t3;
                SetVariableValue(a0);
                _v0 = t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PRE_MINUS:
            {
                _v0 = EvaluateValueExpression();
                Int32 t3 = _v0 - 1;
                _s7.advanceTopOfStack();
                Int32 a0 = t3;
                SetVariableValue(a0);
                _v0 = t3;
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
                _v0 = EvaluateValueExpression();
                _v0 = 0 - _v0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_NOT:
            {
                _v0 = EvaluateValueExpression();
                _v0 = 0 < _v0 ? 1 : 0;
                _v0 ^= 1;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_COMP:
            {
                _v0 = EvaluateValueExpression();
                _v0 = ~(0 | _v0);
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_MULT:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 *= t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_DIV:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                if (t3 == 0)
                {
                    expr_Push_v0_Int24();
                }
                else
                {
                    _v0 /= t3;
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_REM:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                if (t3 == 0)
                {
                    expr_Push_v0_Int24();
                }
                else
                {
                    _v0 %= t3;
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_PLUS:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 += t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_MINUS:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 -= t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SHIFT_LEFT:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 <<= t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SHIFT_RIGHT:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 >>= t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_LT: // B_LT = 24,
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                if (_eventEngine.gCur.uid == 13 && t3 == -300)
                {
                    t3 = -250;
                }
                _v0 = _v0 < t3 ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_GT: // B_GT = 25,
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                if (FF9StateSystem.Common.FF9.fldMapNo == 657 && _eventEngine.gCur.sid == 17 // Marsh/Pond, Zidane
                    && (_eventEngine.gCur.ip == 1413 || _eventEngine.gCur.ip == 1542 || _eventEngine.gCur.ip == 1666 || _eventEngine.gCur.ip == 1795 || _eventEngine.gCur.ip == 2172 || _eventEngine.gCur.ip == 2301 || _eventEngine.gCur.ip == 1919 || _eventEngine.gCur.ip == 2048 || _eventEngine.gCur.ip == 2425 || _eventEngine.gCur.ip == 2554 || _eventEngine.gCur.ip == 2683 || _eventEngine.gCur.ip == 2812 || _eventEngine.gCur.ip == 2941))
                {
                    _v0 = t3 <= _v0 ? 1 : 0;
                }
                else if (t3 < _v0)
                {
                    _v0 = 1;
                }
                else
                {
                    _v0 = 0;
                }
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_LE: // B_LE = 26,
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 = t3 < _v0 ? 1 : 0;
                _v0 ^= 1;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_GE: // B_GE = 27,
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 = _v0 < t3 ? 1 : 0;
                _v0 ^= 1;
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
                if (s1.sid != 0 || s1.ip == 320)
                {
                }
                _eventEngine.gCP = _s7;
                _v0 = _eventEngine.OperatorExtract(arg0);
                _s7 = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_EQ:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 ^= t3;
                _v0 = Mathf.Abs(_v0) < 1 ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_NE:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 ^= t3;
                _v0 = 0 < Mathf.Abs(_v0) ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_AND:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 &= t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_XOR:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 ^= t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_OR:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _v0 |= t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_ANDAND:
            {
                _v0 = EvaluateValueExpression();
                _s7.retreatTopOfStack();
                if (_v0 == 0)
                {
                    expr_Push_v0_Int24();
                }
                else
                {
                    _s7.advanceTopOfStack();
                    _v0 = EvaluateValueExpression();
                    _v0 = ((0 >= Mathf.Abs(_v0)) ? 0 : 1);
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_OROR:
            {
                _v0 = EvaluateValueExpression();
                _v0 = ((0 >= Mathf.Abs(_v0)) ? 0 : 1);
                _s7.retreatTopOfStack();
                if (_v0 != 0)
                {
                    expr_Push_v0_Int24();
                }
                else
                {
                    _s7.advanceTopOfStack();
                    _v0 = EvaluateValueExpression();
                    _v0 = ((0 >= Mathf.Abs(_v0)) ? 0 : 1);
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_MEMBER:
            {
                Int32 a0 = s1.getByteIP();
                s1.ip++;
                a0 |= encodeVarClass(VariableSource.Member);
                _s7.push(a0);
                break;
            }
            case op_binary.B_COUNT:
            {
                if (s1.sid != 0 || s1.ip == 321)
                {
                }
                _eventEngine.gCP = _s7;
                _v0 = _eventEngine.OperatorCount();
                _s7 = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PICK:
            {
                _eventEngine.gCP = _s7;
                _v0 = _eventEngine.OperatorPick();
                _s7 = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_LET:
            {
                // -> ValueExpression
                // ResultVariableId
                // END
                _v0 = EvaluateValueExpression();

                // -> ResultVariableId
                // END
                Int32 currentValue = _v0;
                Int32 t3 = _v0;
                SetVariableValue(currentValue);

                // END
                _v0 = t3;
                expr_Push_v0_Int24();

                // -> Int24ValueExpression
                // END
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
                _eventEngine.gCP = _s7;
                if (s1.sid == 0 && s1.ip == 411)
                {
                    //Debug.Log("Debug @all");
                }
                _v0 = _eventEngine.OperatorAll(arg0);
                _s7 = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_MULT_LET:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                Int32 a0 = _v0 * t3;
                _s7.advanceTopOfStack();
                t3 = a0;
                SetVariableValue(a0);
                _v0 = t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_DIV_LET:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _s7.advanceTopOfStack();
                if (t3 == 0)
                {
                    expr_Push_v0_Int24();
                }
                else
                {
                    Int32 a0 = _v0 / t3;
                    t3 = a0;
                    SetVariableValue(a0);
                    _v0 = t3;
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_REM_LET:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                _s7.advanceTopOfStack();
                if (t3 == 0)
                {
                    expr_Push_v0_Int24();
                }
                else
                {
                    Int32 a0 = _v0 % t3;
                    t3 = a0;
                    SetVariableValue(a0);
                    _v0 = t3;
                    expr_Push_v0_Int24();
                }
                break;
            }
            case op_binary.B_PLUS_LET:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                Int32 a0 = _v0 + t3;
                _s7.advanceTopOfStack();
                t3 = a0;
                SetVariableValue(a0);
                _v0 = t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_MINUS_LET:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                Int32 a0 = _v0 - t3;
                _s7.advanceTopOfStack();
                t3 = a0;
                SetVariableValue(a0);
                _v0 = t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SHIFT_LEFT_LET:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                Int32 a0 = _v0 << t3;
                _s7.advanceTopOfStack();
                t3 = a0;
                SetVariableValue(a0);
                _v0 = t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SHIFT_RIGHT_LET:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                Int32 a0 = _v0 >> t3;
                _s7.advanceTopOfStack();
                t3 = a0;
                SetVariableValue(a0);
                _v0 = t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_AND_LET:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                Int32 a0 = (_v0 & t3);
                _s7.advanceTopOfStack();
                t3 = a0;
                SetVariableValue(a0);
                _v0 = t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_XOR_LET:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                Int32 a0 = (_v0 ^ t3);
                _s7.advanceTopOfStack();
                t3 = a0;
                SetVariableValue(a0);
                _v0 = t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_OR_LET:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                Int32 a0 = (_v0 | t3);
                _s7.advanceTopOfStack();
                t3 = a0;
                SetVariableValue(a0);
                _v0 = t3;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SELECT:
            {
                _eventEngine.gCP = _s7;
                _v0 = _eventEngine.OperatorSelect();
                _s7 = _eventEngine.gCP;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_KEYON: // B_KEYON = 79
            {
                VoicePlayer.scriptRequestedButtonPress = true;
                _v0 = (Mathf.Abs(EvaluateValueExpression() & ETb.KeyOn()) <= 0) ? 0 : 1;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SIN2:
            {
                Int32 a0 = EvaluateValueExpression();
                _v0 = ff9.rsin(a0);
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_COS2:
            {
                Int32 a0 = EvaluateValueExpression();
                _v0 = ff9.rcos(fixedPointAngle: a0);
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_KEYOFF:
            {
                _v0 = (Mathf.Abs(EvaluateValueExpression() & ETb.KeyOff()) <= 0) ? 0 : 1;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_KEY:
            {
                _v0 = (Mathf.Abs(EvaluateValueExpression() & _eTb.PadReadE()) <= 0) ? 0 : 1;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_ANGLE:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                Actor actor = (Actor)_eventEngine.gCur;
                if (actor.fieldMapActorController == null)
                {
                }
                _v0 = ConvertFloatAngleToFixedPoint(angleAsm(_v0 - actor.pos[0], t3 - actor.pos[2]));
                _v0 <<= 20;
                _v0 >>= 24;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_DISTANCE:
            {
                Int32 t3 = EvaluateValueExpression();
                _v0 = EvaluateValueExpression();
                Actor actor = (Actor)_eventEngine.gCur;
                _v0 = (Int32)distance(_v0 - actor.pos[0], 0, t3 - actor.pos[2]);
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PTR:
            {
                Int32 a0 = s1.getByteIP();
                s1.ip++;
                Obj objUID = _eventEngine.GetObjUID(a0);
                _v0 = objUID.uid;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_ANGLEA:
            {
                _v0 = EvaluateValueExpression();
                Obj objUID2 = _eventEngine.GetObjUID(_v0);
                Single num6 = ((PosObj)objUID2).pos[0];
                Single num7 = ((PosObj)objUID2).pos[2];
                Single num8 = ((PosObj)_eventEngine.gCur).pos[0];
                Single num9 = ((PosObj)_eventEngine.gCur).pos[2];
                Single floatAngle = angleAsm(num6 - num8, num7 - num9);
                Int32 num3 = ConvertFloatAngleToFixedPoint(floatAngle);
                _v0 = num3 >> 4;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_DISTANCEA:
            {
                _v0 = EvaluateValueExpression();
                Obj objUID2 = _eventEngine.GetObjUID(_v0);
                Obj gCur = _eventEngine.gCur;
                Actor actor = (Actor)gCur;
                //Int32 a0 = 524288000;
                Single num4 = ((Actor)objUID2).pos[0] - actor.pos[0];
                Single num5 = ((Actor)objUID2).pos[2] - actor.pos[2];
                var y = 0;
                _v0 = (Int32)distance(num4, y, num5);
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_SIN:
            {
                _v0 = EvaluateValueExpression();
                Int32 a0 = _v0 << 4;
                _v0 = ff9.rsin(a0);
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_COS:
            {
                _v0 = EvaluateValueExpression();
                Int32 a0 = _v0 << 4;
                _v0 = ff9.rcos(fixedPointAngle: a0);
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_ANGLE2:
            {
                Int32 t3 = EvaluateValueExpression();
                Int32 a0 = EvaluateValueExpression();
                var deltaZ = t3;
                Single floatAngle = angleAsm(a0, deltaZ);
                Int32 num3 = ConvertFloatAngleToFixedPoint(floatAngle);
                _v0 = num3 >> 4;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PARTYCHK: // B_PARTYCHK
            {
                Int32 a0 = EvaluateValueExpression();
                _v0 = _eventEngine.partychk(a0) ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_PARTYADD:
            {
                Int32 a0 = EvaluateValueExpression();
                _v0 = _eventEngine.partyadd(a0) ? 1 : 0;
                expr_Push_v0_Int24();
                break;
            }
            case op_binary.B_OBJSPECA:
            {
                _s7.push(s1.getByteIP(1) | (s1.getByteIP() << 8) | encodeVarClass(VariableSource.Object));
                s1.ip += 2;
                break;
            }
            case op_binary.B_SYSLIST:
            {
                _s7.push(s1.getByteIP() | encodeVarClass(VariableSource.System));
                s1.ip++;
                break;
            }
            case op_binary.B_SYSVAR:
            {
                _v0 = _eventEngine.GetSysvar(s1.getByteIP());
                s1.ip++;
                _s7.push((_v0 & 0x3FFFFFF) | encodeVarClass(VariableSource.Int26)); // 26 bit (signed)
                break;
            }
            case op_binary.B_CONST:
            {
                _s7.push((Int32)s1.getShortIP() | encodeVarClass(VariableSource.Int26));
                s1.ip += 2;
                break;
            }
            case op_binary.B_CONST4:
            {
                _s7.push((s1.getIntIP() & 0x3FFFFFF) | encodeVarClass(VariableSource.Int26)); // 26 bit (signed)
                s1.ip += 4;
                break;
            }
            case op_binary.B_EXPR_END:
            {
                _eventEngine.gCP = _s7;
                //_s4 = _tempS4;
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
        _v0 |= encodeVarClass(VariableSource.Int26);
        _s7.push(_v0);
    }

    public Int32 bra()
    {
        Int16 shortIP = s1.getShortIP();
        s1.ip += 2;
        s1.ip += shortIP;
        return 0;
    }

    public Int32 beq()
    {
        _v0 = EvaluateValueExpression();
        if (_v0 != 0)
        {
            s1.ip += 2;
        }
        else
        {
            Int32 uShortIP = s1.getUShortIP();
            s1.ip += 2;
            s1.ip += uShortIP;
        }
        return 0;
    }

    public Int32 bne()
    {
        _v0 = EvaluateValueExpression();
        if (_v0 == 0)
        {
            s1.ip += 2;
        }
        else
        {
            bra();
        }
        return 0;
    }

    public Int32 wait()
    {
        Int32 s5 = s1.getByteIP();
        s1.ip++;
        _v0 = getv1i(ref s5);
        if (FF9StateSystem.Common.FF9.fldMapNo == 3011) // Ending/TH
        {
            String symbol = Localization.GetSymbol();
            if (symbol != "US" && symbol != "JP")
            {
                if (_v0 == 82)
                {
                    _v0 = 102;
                }
                else if (_v0 == 50)
                {
                    _v0 = 90;
                }
            }
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 3009) // Ending/TH
        {
            String symbol2 = Localization.GetSymbol();
            if (symbol2 != "US" && symbol2 != "JP" && s1.uid == 17 && _v0 == 15)
            {
                _v0 = 20;
            }
        }
        Int32 a0 = _v0 - 254;
        if (_v0 != 0)
        {
            _v0--;
            if (a0 > 0)
            {
                _v0 = 253;
            }
            s1.wait = (Byte)_v0;
            _s2 = 1;
        }
        return 0;
    }

    private void JMP_SWITCH(Int32 caseNumber)
    {
        Int32 offsetL = s1.getByteIP(1);
        Int32 offsetH = (SByte)s1.getByteIP(2);
        offsetH <<= 8;
        _v0 = EvaluateValueExpression();
        _v0 -= offsetL;
        _v0 -= offsetH;
        Int32 a0 = _v0 - caseNumber;
        if (_v0 < 0)
        {
            JMP_SWITCH_DEFAULT();
        }
        else
        {
            _v0 <<= 1;
            if (a0 >= 0)
            {
                JMP_SWITCH_DEFAULT();
            }
            else
            {
                a0 = _nextCodeIndex + _v0;
                Int32 a1 = s1.getByteIP(_v0 + 5); // caseOffsetL
                Int32 a2 = s1.getByteIP(_v0 + 6); // caseOffsetH
                s1.ip += a1;
                s1.ip += a2 << 8;
            }
        }
    }

    public void JMP_SWITCH_DEFAULT()
    {
        Int32 offsetL = s1.getByteIP(3);
        Int32 offsetH = s1.getByteIP(4);
        offsetH = (offsetH << 8 | offsetL);
        s1.ip += offsetH;
    }

    public Int32 commandDefault()
    {
        s1.ip--;
        commandDefault2();
        return 0;
    }

    public Int32 commandDefault2()
    {
        _v0 = _eventEngine.DoEventCode();
        _s2 = _v0;
        if (_eventEngine.gArgUsed > 0)
        {
            _nextCodeIndex = s1.ip;
        }
        else
        {
            _nextCodeIndex = s1.ip - 1;
            s1.ip = _nextCodeIndex;
        }
        return 0;
    }

    public void JMP_SWITCHEX(ref Int32 caseOffset, ref Int32 caseNumber)
    {
        if (caseNumber > 0)
        {
            caseNumber--;
            Int32 valueH = s1.getByteIP(1 + caseOffset);
            Int32 valueL = s1.getByteIP(0 + caseOffset);
            Int32 caseValue = valueL | valueH << 8;
            caseValue -= _v0; // inputValue
            caseOffset += 4;
            if (caseValue == 0)
            {
                Int32 offsetH = s1.getByteIP(-1 + caseOffset);
                Int32 offsetL = s1.getByteIP(-2 + caseOffset);
                offsetL |= offsetH << 8;
                s1.ip += offsetL;
                s1.ip += 3;
            }
            else
            {
                JMP_SWITCHEX(ref caseOffset, ref caseNumber);
            }
        }
        else
        {
            caseNumber--;
            Int32 offsetH = s1.getByteIP(2);
            Int32 offsetL = s1.getByteIP(1);
            offsetL |= offsetH << 8;
            s1.ip += offsetL;
            s1.ip += 3;
        }
    }

    public void ad21()
    {
        s0 = _eventEngine.DisposeObj(s1);
        _s2 = 2;
    }

    public Int32 jumpToCommand(Int32 arg0)
    {
        if (arg0 >= 0 && arg0 <= 109)
        {
            switch (arg0)
            {
                case 1: // JMP
                {
                    bra();
                    return 0;
                }
                case 2: // JMP_IFNOT
                {
                    beq();
                    return 0;
                }
                case 3: // JMP_IF
                {
                    bne();
                    return 0;
                }
                case 4: // return
                {
                    _eventEngine.Return(s1);
                    adfr();
                    return 0;
                }
                case 5: // set
                {
                    expr();
                    return 0;
                }
                case 6: // JMP_SWITCHEX
                {
                    Int32 caseNumber = s1.getByteIP();
                    _v0 = EvaluateValueExpression();
                    _v0 &= 65535;
                    Int32 caseOffset = 3;
                    JMP_SWITCHEX(ref caseOffset, ref caseNumber);
                    return 0;
                }
                case 11: // rsv0b, JMP_SWITCH
                {
                    Int32 caseNumber = s1.getByteIP();
                    JMP_SWITCH(caseNumber);
                    return 0;
                }
                case 13: // JMP_SWITCH with many cases (>255)
                {
                    Int32 t2 = s1.getShortIP();
                    s1.ip++;
                    JMP_SWITCH(t2);
                    return 0;
                }
                case 28: // DELETE, TerminateEntry
                {
                    Int32 _a0 = s1.getByteIP(1);
                    int a1 = 255;
                    s1.ip += 2;
                    if (_a0 == a1)
                    {
                        ad21();
                    }
                    else
                    {
                        Obj objUID = _eventEngine.GetObjUID(_a0);
                        if (s1 == objUID)
                        {
                            ad21();
                        }
                        else
                        {
                            _eventEngine.DisposeObj(objUID);
                        }
                    }
                    return 0;
                }
                case 34: // Wait
                {
                    wait();
                    return 0;
                }
                case 48: // PRINT1
                {
                    return 0;
                }
                case 49: // PRINTF
                {
                    return 0;
                }
                case 50: // LOCATE
                {
                    Int32 s5 = s1.getByteIP();
                    s1.ip++;
                    Int32 a0 = getv1i(ref s5);
                    Int32 a1 = getv1i(ref s5);
                    return 0;
                }
                case 108: // PPRINT
                {
                    return 0;
                }
                case 109: // PPRINTF
                {
                    return 0;
                }
                default:
                {
                    commandDefault();
                    return 0;
                }
            }
        }
        Debug.Log("EBin.jumpToCommand INVALID command " + arg0);
        return -1;
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
        _s7.pop(out var t0);
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
                        List<Int32> subs = _s7.getSubs();
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
                        List<Int32> subs = _s7.getSubs();
                        if (subs.Count < 1)
                            return 0;
                        Int32 vectID = subs[0];
                        if (FF9StateSystem.EventState.gScriptVector.TryGetValue(vectID, out List<Int32> vect))
                            return vect.Count;
                        return 0;
                    }
                    case VariableType.Dictionary:
                    {
                        List<Int32> subs = _s7.getSubs();
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
                _v0 = getvobj(_eventEngine.GetObjUID((t0 >> 8) & 0xFF), t0 & 0xFF);
                return _v0;
            case VariableSource.System:
                _v0 = _eventEngine.GetSysList(t0 & 0xFF);
                return _v0;
            case VariableSource.Member:
                _v0 = getvobj(_eventEngine.gMemberTarget, (t0 << 6) >> 6);
                return _v0;
            case VariableSource.Int26:
                _v0 = (t0 << 6) >> 6;
                return _v0;
        }
        return 0;
    }

    private Int32 GetMemoriaCustomVariable(memoria_variable varCode)
	{
		switch (varCode)
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
                if (val == 0)
                    FF9StateSystem.Battle.FF9Battle.btl_scene.Info.Runaway = false;
                else
                    FF9StateSystem.Battle.FF9Battle.btl_scene.Info.Runaway = true;
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
                _v0 = (bitFlags >> (ofs & 7)) & 1; // (1 bit & 1) => result
                return _v0;
            }
            case VariableType.Int24:
            case VariableType.UInt24:
                _v0 = buffer[ofs + bufferOffset] | (buffer[ofs + 1 + bufferOffset] << 8) | ((SByte)buffer[ofs + 2 + bufferOffset] << 16);
                return _v0;
            case VariableType.SByte:
                _v0 = (SByte)buffer[ofs + bufferOffset];
                return _v0;
            case VariableType.Byte:
                _v0 = buffer[ofs + bufferOffset];
                return _v0;
            case VariableType.Int16:
                _v0 = buffer[ofs + bufferOffset] | ((SByte)buffer[ofs + 1 + bufferOffset] << 8);
                return _v0;
            case VariableType.UInt16:
                _v0 = buffer[ofs + bufferOffset] | (buffer[ofs + 1 + bufferOffset] << 8);
                return _v0;
            default:
                return 0;
        }
    }

    public Int32 getv1i(ref Int32 s5)
    {
        _v0 = (s5 & 1);
        s5 >>= 1;
        if (_v0 != 0)
        {
            expr();
            _v0 = EvaluateValueExpression();
            return _v0;
        }
        _v0 = s1.getByteIP();
        s1.ip++;
        return _v0;
    }

    public Int32 SetVariableValue(Int32 arg0)
    {
        _s7.pop(out var t0);
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
                        List<Int32> subs = _s7.getSubs();
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
                        List<Int32> subs = _s7.getSubs();
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
                        List<Int32> subs = _s7.getSubs();
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
                    value = value - 1 + Configuration.Hacks.RopeJumpingIncrement;
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
        _nextCodeIndex = s1.getByteIP();
        arg0 = s1.getByteIP();
    }

    public Int32 CalcExpr()
    {
        Obj obj = s1;
        //Int32 num = _s3;
        Int32 num2 = _nextCodeIndex;
        CalcStack calcStack = _s7;
        s1 = _eventEngine.gExec;
        expr();
        s1 = obj;
        //_s3 = num;
        _nextCodeIndex = num2;
        _s7 = calcStack;
        return 0;
    }

    public Int32 getv()
    {
        CalcStack calcStack = _s7;
        _s7 = _eventEngine.gCP;
        _v0 = EvaluateValueExpression();
        _s7 = calcStack;
        return _v0;
    }

    public Int32 putv(Int32 a)
    {
        CalcStack calcStack = _s7;
        _s7 = _eventEngine.gCP;
        _v0 = SetVariableValue(a);
        _s7 = calcStack;
        return _v0;
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