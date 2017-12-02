using System;
using Assets.Sources.Scripts.EventEngine.Utils;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using System.Collections.Generic;
using System.IO;
using Memoria.Data;
using UnityEngine;
using static EventEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

// ReSharper disable NotAccessedField.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable UnusedVariable
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable RedundantExplicitArraySize
// ReSharper disable UnusedParameter.Local
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable RedundantCast
// ReSharper disable ArrangeThisQualifier
// ReSharper disable ArrangeStaticMemberQualifier


public partial class EventEngine : PersistenSingleton<EventEngine>
{
    public Int32 nil;
    public Single nilFloat;
    public FieldMap fieldmap;
    public FieldSPSSystem fieldSps;
    public Single POS_COMMAND_DEFAULTY;
    public Obj gExec;
    public EventContext sEventContext0;
    public EventContext sEventContext1;
    public ObjList gStopObj;
    public ObjTable[] sObjTable;
    public Int32 sSourceObjN;
    public Int32 gMode;
    public Obj gCur;
    public Int32 gArgFlag;
    public Int32 gArgUsed;
    public CalcStack gCP;
    public Int32 gAnimCount;
    public Int32 sSysX;
    public Int32 sSysY;
    public Int32 sMapJumpX;
    public Int32 sMapJumpZ;
    public Int32 sSEPos;
    public Int32 sSEVol;
    public Obj gMemberTarget;
    public Int64 sLockTimer;
    public Int64 sLockFree;
    public EBin eBin;
    public ETb eTb;
    public Byte[][] allObjsEBData;
    public List<Int32> toBeAddedObjUIDList;
    public Boolean requiredAddActor;
    public Boolean addedChar;

    private EventContext _context;
    private String _defaultMapName;
    private Int32 _lastIP;
    private EncountData _enCountData;
    private Int32 _encountBase;
    private Single _encountTimer;
    private Int32 _lastScene;
    private Int32 _collTimer;
    private Boolean _moveKey; // ProcessEvents
    private Obj[] _objPtrList;
    private Int32 _opLStart;
    private UInt16[] _sysList;
    private Boolean _encountReserved;
    private PosObj _eyeObj;
    private PosObj _aimObj;
    private FF9FIELD_DISC _ff9fieldDisc;
    private TextAsset _currentEBAsset;
    private Boolean _posUsed;
    private Boolean _noEvents;
    private FF9StateGlobal _ff9;
    private FF9StateSystem _ff9Sys;
    private GeoTexAnim _geoTexAnim; // DoEventCode
    private readonly Dictionary<Int32, Int32> _mesIdES_FR; // DoEventCode
    private readonly Dictionary<Int32, Int32> _mesIdGR;    // DoEventCode
    private readonly Dictionary<Int32, Int32> _mesIdIT;    // DoEventCode
    private PosObj _fixThornPosObj; // DoEventCode
    private Int32 _fixThornPosA;      // DoEventCode
    private Int32 _fixThornPosB;      // DoEventCode
    private Int32 _fixThornPosC;      // DoEventCode

    public Int32 SCollTimer
    {
        get { return this._collTimer; }
        set { this._collTimer = value; }
    }

    public Int32 ServiceEvents()
    {
        Int32 num = 0;
        if (!this._noEvents)
        {
            this.eTb.ProcessKeyEvents();
            this.CheckSleep();
            num = this.ProcessEvents();
            EIcon.ProcessFIcon();
            EIcon.ProcessAIcon();
        }
        return num;
    }

    public Int32 GetFldMapNoAfterChangeDisc()
    {
        return this._ff9fieldDisc.FieldMapNo;
    }

    public void addObjPtrList(Obj obj)
    {
        this._objPtrList[this._opLStart++] = obj;
    }

    public ObjList GetFreeObjList()
    {
        return this._context.freeObj;
    }

    public void SetFreeObjList(ObjList objList)
    {
        this._context.freeObj = objList;
    }

    public ObjList GetActiveObjTailList()
    {
        return this._context.activeObjTail;
    }

    public void SetActiveObjTailList(ObjList objList)
    {
        this._context.activeObjTail = objList;
    }

    public Byte[] GetMapVar()
    {
        return this._context.mapvar;
    }

    public Int16 GetTwistA()
    {
        return this._context.twist_a;
    }

    public Int16 GetTwistD()
    {
        return this._context.twist_d;
    }

    public PosObj GetEventEye()
    {
        return this._eyeObj;
    }

    public PosObj GetEventAim()
    {
        return this._aimObj;
    }

    public Byte GetControlUID()
    {
        return this._context.controlUID;
    }

    private Int32 SelectScene()
    {
        EncountData encountData = this.gMode != 1 ? ff9.w_worldGetBattleScenePtr() : this._enCountData;
        Int32 num = Comn.random8();
        Int32 index = encountData.pattern & 3;
        if (num < d[index, 0])
            return encountData.scene[0];
        if (num < d[index, 1])
            return encountData.scene[1];
        if (num < d[index, 2])
            return encountData.scene[2];
        return encountData.scene[3];
    }



    public Int32 OperatorPick()
    {
        Int32 num1 = 0;
        Int32 valueAtOffset = this.gCP.getValueAtOffset(-2);
        Int32 num2;
        if ((valueAtOffset >> 26 & 7) == 5)
        {
            num2 = this.GetSysList(valueAtOffset);
        }
        else
        {
            this.gCP.retreatTopOfStack();
            num2 = this.eBin.getv();
            this.gCP.advanceTopOfStack();
            this.gCP.advanceTopOfStack();
        }
        Int32 index = 0;
        while (index < 8 && (num2 & 1) == 0)
        {
            ++index;
            num2 >>= 1;
        }
        if (index < 8)
        {
            this.gMemberTarget = this._objPtrList[index];
            num1 = this.eBin.getv();
        }
        else
            this.gCP.retreatTopOfStack();
        this.gCP.retreatTopOfStack();
        return num1;
    }

    public Int32 OperatorCount()
    {
        Int32 num1 = 0;
        Int32 num2 = this.eBin.getv();
        Int16 num3 = 1;
        while (num3 != 0)
        {
            num1 += (num2 & num3) == 0 ? 0 : 1;
            num3 <<= 1;
        }
        return num1;
    }

    public Int32 OperatorSelect()
    {
        Byte[] numArray = new Byte[8];
        Int32 num1 = this.eBin.getv();
        Int32 num2 = 0;
        Int32 num3 = 0;
        while (num3 < 8)
        {
            if ((num1 & 1) != 0)
                numArray[num2++] = (Byte)num3;
            ++num3;
            num1 >>= 1;
        }
        if (num2 == 0)
            return 0;
        Int32 index = num2 * Comn.random8() >> 8;
        return 1 << numArray[index];
    }

    public Boolean RequestAction(BattleCommandId cmd, Int32 target, Int32 prm1, Int32 commandAndScript)
    {
        Int32 index;
        for (index = 0; index < 8 && (target & 1) == 0; ++index)
            target >>= 1;

        if (index >= 8)
            return false;

        Obj p = this._objPtrList[index];
        if (cmd == BattleCommandId.EnemyAtk)
        {
            if (p.level > 3)
            {
                p.btlchk = 2;
                return true;
            }

            return false;
        }
        Int32 level = 2;
        Int32 tagNumber = 7;
        switch (cmd)
        {
            case BattleCommandId.EnemyCounter:
                tagNumber = 6;
                this.SetSysList(0, prm1);
                level = 1;
                break;
            case BattleCommandId.EnemyDying:
                tagNumber = 9;
                this.SetSysList(0, prm1);
                level = 0;
                break;
        }

        _btlCmdPrm = commandAndScript;
        return this.Request(p, level, tagNumber, false);
    }

    private Obj Collision(EventEngine eventEngine, PosObj po, Int32 mode, ref Single distance)
    {
        return EventCollision.Collision(this, po, mode, ref distance);
    }

    private void CollisionRequest(PosObj po)
    {
        EventCollision.CollisionRequest(po);
    }

    public Boolean Request(Obj p, Int32 level, Int32 tagNumber, Boolean ew)
    {
        Int32 ip = this.nil;
        if (p != null && level < p.level)
        {
            ip = this.GetIP(p.sid, tagNumber, p.ebData);
            if (ip != this.nil)
                this.Call(p, ip, level, ew, null);
        }
        return ip != this.nil;
    }

    public Boolean IsActuallyTalkable(Obj p)
    {
        if (p == null)
            return false;
        Int32 ip = this.GetIP(p.sid, 3, p.ebData);
        return ip != this.nil && ((EBin.event_code_binary)p.getByteFromCurrentByte(ip + 7) != EBin.event_code_binary.rsv04 || (EBin.event_code_binary)p.getByteFromCurrentByte(ip + 8) != EBin.event_code_binary.rsv04);
    }

    private void Call(Obj obj, Int32 ip, Int32 level, Boolean ew, Byte[] additionCommand = null)
    {
        Int32 startID = this.getspw(obj, obj.sx);
        obj.setIntToBuffer(startID, obj.ip);
        Int32 num = obj.wait & Byte.MaxValue | (obj.level & Byte.MaxValue) << 8 | (!ew ? Byte.MaxValue : this.gExec.level & Byte.MaxValue) << 16 | (this.gExec.uid & Byte.MaxValue) << 24;
        obj.setIntToBuffer(startID + 4, num);
        if (ew)
            this.gExec.wait = Byte.MaxValue;
        obj.sx += 2;
        obj.ip = ip;
        obj.level = (Byte)level;
        obj.wait = 0;
        if (additionCommand == null)
            return;
        obj.CallAdditionCommand(additionCommand);
    }

    private Int32 getspw(Obj obj, Int32 id)
    {
        return obj.sofs * 4 + 4 * id;
    }

    private Obj getSender(Obj obj)
    {
        Int32 startID = this.getspw(obj, obj.sx - 1);
        return this.FindObjByUID(obj.getIntFromBuffer(startID) >> 24 & Byte.MaxValue);
    }

    public ObjList GetActiveObjList()
    {
        return this._context?.activeObj;
    }

    public void SetActiveObjList(ObjList objList)
    {
        this._context.activeObj = objList;
    }

    public Actor getActiveActorByUID(Int32 uid)
    {
        if (this._context.activeObj == null)
            return null;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            if (objList.obj.cid == 4 && objList.obj.uid == uid)
                return (Actor)objList.obj;
        }
        return null;
    }

    public Actor getActiveActor(Int32 sid)
    {
        if (this._context.activeObj == null)
            return null;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            if (objList.obj.cid == 4)
            {
                Actor actor = (Actor)objList.obj;
                if (actor != null && actor.sid == sid)
                    return actor;
            }
        }
        return null;
    }

    public PosObj GetControlChar()
    {
        Obj obj = this._context == null ? null : this.FindObjByUID(this._context.controlUID);
        if (obj != null && obj.cid == 4)
            return (PosObj)obj;
        return null;
    }

    public PosObj GetControlCharOrTheFirstActor()
    {
        PosObj posObj = this.GetControlChar();
        if (posObj == null)
        {
            for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
            {
                Obj obj = objList.obj;
                if (obj.cid == 4)
                {
                    posObj = (PosObj)obj;
                    break;
                }
            }
        }
        return posObj;
    }

    private void printEObj()
    {
        //if (this._context.activeObj.next == null)
        //    Debug.Log("E.activeObj.next == null");
        //else
        //    Debug.Log("E.activeObj.next.obj.uid = " + (object)this._context.activeObj.next.obj.uid);
        //
        //Debug.Log(this._context.freeObj == null ? "E.freeObj == null" : "E.freeObj is NOT null");
    }

    public void stay()
    {
        this.gExec.ip = this._lastIP;
        this.gArgUsed = 1;
    }

    public void clrdist(Actor actor)
    {
        actor.lastdist = kInitialDist;
        actor.actf &= (UInt16)~(actMove | actLockDir);
        actor.rot0 = 0.0f;
    }

    private void SetupCodeParam(BinaryReader br)
    {
        br.BaseStream.Seek(3L, SeekOrigin.Begin);
        this.sSourceObjN = br.ReadByte();
        br.BaseStream.Seek(128L, SeekOrigin.Begin);
        this.sObjTable = new ObjTable[this.sSourceObjN];
        for (Int32 index = 0; index < this.sObjTable.Length; ++index)
        {
            this.sObjTable[index] = new ObjTable();
            this.sObjTable[index].ReadData(br);
        }
    }

    public void ResetIdleTimer(Int32 x)
    {
        if (this._context.idletimer < 0)
            return;
        this._context.idletimer = (Int16)(200 + Comn.random8() << 1 + (x != 0 ? 0 : 1));
    }

    private void CheckSleep()
    {
        if (this._context == null || this._context.usercontrol == 0)
            return;
        this._context.idletimer -= this._context.idletimer <= 0 ? (Int16)0 : (Int16)1;
        Obj objByUid = this.FindObjByUID(this._context.controlUID);
        if (this._context.idletimer != 0 || objByUid == null || objByUid.cid != 4)
            return;
        Actor p = (Actor)objByUid;
        if (p.animFrame != p.frameN - 1)
            return;
        this.ResetIdleTimer(1);
        if (p.sleep == 0 || (p.animFlag & (afExec | afFreeze)) != 0)
            return;
        p.inFrame = 0;
        p.outFrame = Byte.MaxValue;
        this.ExecAnim(p, p.sleep);
        p.animFlag |= (Byte)afLower;
    }

    public Boolean isPosObj(Obj obj)
    {
        return obj.cid == 4;
    }

    public void StartEventsByEBFileName(String ebFileName)
    {
        this._currentEBAsset = AssetManager.Load<TextAsset>(ebFileName, false);
        this.StartEvents(this._currentEBAsset.bytes);
    }

    public Boolean IsEventContextValid()
    {
        return this._context != null;
    }

    public void StartEvents(Byte[] ebFileData)
    {
        resyncBGMSignal = 0;
        //Debug.Log("Reset resyncBGMSignal = " + (object)EventEngine.resyncBGMSignal);
        this._ff9 = FF9StateSystem.Common.FF9;
        this._ff9.charArray.Clear();
        this._ff9Sys = PersistenSingleton<FF9StateSystem>.Instance;
        BinaryReader br = new BinaryReader(new MemoryStream(ebFileData));
        this.SetupCodeParam(br);
        this._ff9.mapNameStr = FF9TextTool.LocationName(this._ff9.fldMapNo);
        this._defaultMapName = this._ff9.mapNameStr;
        switch (this._ff9Sys.mode)
        {
            case 1:
                this.gMode = 1;
                break;
            case 2:
                this.gMode = 2;
                break;
            case 3:
                this.gMode = 3;
                UIManager.World.EnableMapButton = true;
                break;
            case 8:
                this.gMode = 4;
                break;
        }
        EventInput.IsProcessingInput = this.gMode != 2 && this.gMode != 4;
        EMinigame.GetTheAirShipAchievement();
        EMinigame.GetHelpAllVictimsInCleyraTown();
        TimerUI.SetEnable(this._ff9.timerDisplay);
        TimerUI.SetDisplay(this._ff9.timerDisplay);
        TimerUI.SetPlay(this._ff9.timerControl);
        this.allObjsEBData = new Byte[this.sSourceObjN][];
        this.toBeAddedObjUIDList.Clear();
        for (Int32 index = 0; index < this.sSourceObjN; ++index)
        {
            br.BaseStream.Seek(128L, SeekOrigin.Begin);
            Int32 num = (Int32)this.sObjTable[index].ofs;
            Int32 count = (Int32)this.sObjTable[index].size;
            br.BaseStream.Seek((Int64)num, SeekOrigin.Current);
            this.allObjsEBData[index] = br.ReadBytes(count);
            //if (count < 4)
            //;
        }
        if ((this.sEventContext0.inited == 1 || this.sEventContext0.inited == 3) && this.gMode == 2)
            this.sEventContext1.copy(this.sEventContext0);
        this._context = this.sEventContext0;
        this.InitMP();
        this.InitObj();
        EventInput.IsProcessingInput = true;
        EIcon.InitFIcon();
        EIcon.SetAIcon(0);
        for (Int32 index = 0; index < 80; ++index)
            this._context.mapvar[index] = 0;
        this._context.usercontrol = 0;
        this._context.controlUID = 0;
        this._context.idletimer = 0;
        EIcon.SetHereIcon(0);
        this.gStopObj = null;
        this._context.dashinh = 0;
        this._context.twist_a = 0;
        this._context.twist_d = 0;
        this.eTb.gMesCount = this.gAnimCount = 10;
        this._noEvents = false;
        this.InitEncount();
        NewThread(0, 0);
        this._context.activeObj.obj.state = stateInit;
        this.SetupPartyUID();
        for (Int32 index = 0; index < 8; ++index)
            this._objPtrList[index] = null;
        this._opLStart = 0;
        if (this.gMode == 2)
        {
            for (Int32 index = 0; index < 4; ++index)
            {
                Int32 partyMember = this.eTb.GetPartyMember(index);
                if (partyMember >= 0)
                {
                    Actor actor = new Actor(this.sSourceObjN - 9 + partyMember, 0, sizeOfActor);
                }
            }
            this._context.partyObjTail = this._context.activeObjTail;
        }
        else
            this._ff9.btl_rain = 0;
        this._opLStart = 4;
        if (this.gMode == 1 && this.sEventContext1.inited == 1 && this.sEventContext1.lastmap == this._ff9.fldMapNo || this.gMode == 3 && this.sEventContext1.inited == 3 && this.sEventContext1.lastmap == this._ff9.wldMapNo || this._ff9Sys.prevMode == 9 && this.sEventContext1.inited != 0)
        {
            this.sEventContext0.copy(this.sEventContext1);
            this.Request(this.FindObjByUID(0), 0, 10, false);
            this.EnterBattleEnd();
        }
        else
        {
            if (this.gMode != 2 && this.gMode != 4)
            {
                Boolean flag1 = this._ff9.fldMapNo == 70;
                Boolean flag2 = this._ff9.fldMapNo == 2200 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 9450 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 9999;
                Boolean flag3 = this._ff9.fldMapNo == 150 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 1155 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 325;
                Boolean flag4 = this._ff9.fldMapNo == 1251 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 5400;
                Boolean flag5 = this._ff9.fldMapNo == 1602 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6645 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 16;
                Boolean flag6 = this._ff9.fldMapNo == 1757 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6740;
                Boolean flag7 = this._ff9.fldMapNo == 2752 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 11100 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 9999;
                Boolean flag8 = this._ff9.fldMapNo == 3001 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 12000 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 0;
                Boolean flag9 = this._ff9.fldMapNo == 2161 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 10000 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 32;
                if (!flag1 && !flag4 && (!flag5 && !flag6) && (!flag3 && !flag2 && (!flag7 && !flag8)) && !flag9)
                {
                    FF9StateSystem.Settings.UpdateTickTime();
                    ISharedDataSerializer serializer = FF9StateSystem.Serializer;
                    serializer.Autosave(null, (e, s) => { });
                }
            }
            this.ProcessEvents();
        }
        this._context.inited = (Byte)this.gMode;
        this._context.lastmap = this.gMode != 1 ? (this.gMode != 3 ? (UInt16)0 : (UInt16)this._ff9.wldMapNo) : (UInt16)this._ff9.fldMapNo;
        br.Close();

        SpawnCustomChatacters();

        PersistenSingleton<CheatingManager>.Instance.ApplyDataWhenEventStart();
    }

    private void SpawnCustomChatacters()
    {
        CharacterBuilder builer = new CharacterBuilder(this);
        if (_ff9.fldMapNo == 102 && false)
        {
            builer.Spawn(new MyCharacter());
        }
    }

    private void EnterBattleEnd()
    {
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.uid != 0)
            {
                obj.state0 = obj.state;
                obj.state = stateSuspend;
            }
        }
    }

    private void SetupPartyUID()
    {
        Byte[] numArray1 = new Byte[9] {0, 6, 3, 1, 4, 5, 7, 2, 8};
        Byte[] numArray2 = new Byte[9] {0, 3, 7, 2, 4, 5, 1, 6, 8};
        Int32 num = 0;
        for (Int32 index = 0; index < 4; ++index)
            this._context.partyUID[index] = Byte.MaxValue;
        for (Int32 index = 0; index < 4; ++index)
        {
            Int32 partyMember = this.eTb.GetPartyMember(index);
            if (partyMember >= 0)
            {
                // https://github.com/Albeoris/Memoria/issues/3
                // Tirlititi: If Beatrix is in the team, we make it so the engine thinks it's another member instead
                if (partyMember == 8)
                {
                    if (!partychk(1))
                        partyMember = 1;
                    else if (!partychk(2))
                        partyMember = 2;
                    else if (!partychk(3))
                        partyMember = 3;
                    else
                        partyMember = 0;
                }
                // *********************

                num |= 1 << numArray2[partyMember];
            }
        }
        Int32 index1 = 0;
        Int32 index2 = 0;
        while (num != 0)
        {
            if ((num & 1) != 0)
            {
                this._context.partyUID[index1] = (Byte)((UInt32)(this.sSourceObjN - 9) + numArray1[index2]);
                ++index1;
            }
            ++index2;
            num >>= 1;
        }
    }

    public Int32 GetPartyPlayer(Int32 ix)
    {
        Int32 num = this._context.partyUID[ix] - (this.sSourceObjN - 9);
        if (num >= 0 && num < 9)
            return num;
        return 0;
    }

    public Boolean partychk(Int32 x)
    {
        Int32 num = this.chr2slot(x);
        Int32 index;
        for (index = 0; index < 4; ++index)
        {
            PLAYER player = FF9StateSystem.Common.FF9.party.member[index];
            if (player != null && player.info.slot_no == num && !(player.info.serial_no >= 14 ^ x >= 8))
                break;
        }
        return index < 4;
    }

    public Boolean partyadd(Int64 a)
    {
        Int64 num = 0;
        if (!this.partychk((Int32)a))
        {
            a = this.chr2slot((Int32)a);
            if (a >= 0L && a < 9L)
            {
                Int64 index = 0;
                while (index < 4L && this._ff9.party.member[index] != null)
                    ++index;
                num = index < 4L ? 0L : 1L;
                if (num == 0L)
                {
                    ff9play.FF9Play_SetParty((Int32)index, (Int32)a);
                    BattleAchievement.UpdateParty();
                    this.SetupPartyUID();
                }
            }
            else
                num = 1L;
        }
        return num != 0L;
    }

    private Int32 chr2slot(Int32 x)
    {
        if (x < 9)
            return x;
        return x - 4;
    }

    public Int32 GetNumberNPC()
    {
        Int32 num = 0;
        if (this._context != null)
        {
            for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
            {
                Obj obj = objList.obj;
                if (obj.sid < this.sSourceObjN - 9 && obj.cid == 4)
                    ++num;
            }
        }
        return num;
    }

    public Obj GetObjIP(Int32 ip)
    {
        ObjList objList = this._context.activeObj;
        while (objList != null && objList.obj.ip != ip)
            objList = objList.next;

        return objList?.obj;
    }

    public Obj GetObjUID(Int32 uid)
    {
        if (uid == Byte.MaxValue)
            return this.gCur;
        if (uid == 250)
            uid = this._context.controlUID;
        else if (uid >= 251 && uid < Byte.MaxValue)
            uid = this._context.partyUID[uid - 251];
        ObjList objList = this._context.activeObj;
        while (objList != null && objList.obj.uid != uid)
            objList = objList.next;

        return objList?.obj;
    }

    public Int32 GetSysList(Int32 num)
    {
        num &= 7;
        switch (num)
        {
            case 2:
                this._sysList[2] = btl_scrp.GetBattleID(0U);
                break;
            case 3:
                this._sysList[3] = btl_scrp.GetBattleID(1U);
                break;
            case 4:
                this._sysList[4] = btl_scrp.GetBattleID(2U);
                break;
        }
        return this._sysList[num];
    }

    public void ProcessCodeExt(Obj obj)
    {
        Int32 index = 0;
        while (index < 8 && this._objPtrList[index] != obj)
            ++index;
        if (index >= 8)
            return;
        this.SetSysList(1, 1 << index);
    }

    public void SetSysList(Int32 num, Int32 value)
    {
        this._sysList[num & 7] = (UInt16)value;
    }

    private void InitMP()
    {
    }

    private void InitObj()
    {
        Int32 index;
        for (index = 0; index < 31; ++index)
        {
            ObjList objList = this._context.objlist[index];
            objList.next = this._context.objlist[index + 1];
            objList.obj = null;
        }
        ObjList objList1 = this._context.objlist[index];
        objList1.next = null;
        objList1.obj = null;
        this._context.freeObj = this._context.objlist[0];
        this._context.activeObj = this._context.activeObjTail = null;
    }

    private static Obj NewThread(Int32 sid, Int32 uid)
    {
        return new Obj(sid, uid, sizeOfObj, 16) {cid = 2};
    }

    public Int32 GetBattleCharData(Obj obj, Int32 kind)
    {
        Int32 num = 0;
        Int32 index = 0;
        while (index < 8 && this._objPtrList[index] != obj)
            ++index;

        if (index < 8)
        {
            BattleUnit btlDataPtr = btl_scrp.FindBattleUnit((UInt16)(1 << index));
            if (btlDataPtr != null)
                 num = (Int32)btl_scrp.GetCharacterData(btlDataPtr.Data, (UInt32)kind);
        }

        return num;
    }

    private void SetBattleCharData(Obj obj, Int32 kind, Int32 value)
    {
        Int32 index = 0;
        while (index < 8 && this._objPtrList[index] != obj)
            ++index;

        if (index >= 8)
            return;

        BattleUnit btl = kind != 32 ? btl_scrp.FindBattleUnit((UInt16)(1 << index)) : btl_scrp.FindBattleUnitUnlimited((UInt16)(1 << index));
        if (btl == null)
            return;

        btl_scrp.SetCharacterData(btl.Data, (UInt32)kind, (UInt32)value);
    }

    private Int32 getNumOfObjsInObjList(ObjList list)
    {
        Int32 num = 0;
        for (; list != null; list = list.next)
            ++num;
        return num;
    }

    public void printObjsInObjList(ObjList list)
    {
        //int num = 0;
        //while (list != null)
        //{
        //    if (list.obj != null)
        //    ;
        //    list = list.next;
        //    ++num;
        //}
    }

    private void printActorsInObjList(ObjList list)
    {
        //int num = 0;
        //while (list != null)
        //{
        //    if ((int)list.obj.cid != 4 || (Actor)list.obj != null)
        //    ;
        //    list = list.next;
        //    ++num;
        //}
    }

    public Int32 GetIP(Int32 objID, Int32 tagID, Byte[] eventData)
    {
        if (eventData.Length == 0)
            return this.nil;
        using (MemoryStream memoryStream = new MemoryStream(eventData))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                binaryReader.ReadByte();
                Byte num1 = binaryReader.ReadByte();
                UInt16 num2 = 0;
                Int32 num3;
                for (num3 = num1; num3 > 0; --num3)
                {
                    UInt16 num4 = (UInt16)binaryReader.ReadInt16();
                    num2 = (UInt16)binaryReader.ReadInt16();
                    if (num4 == tagID)
                        break;
                }
                if (num3 == 0)
                    return this.nil;
                return 2 + num2;
            }
        }
    }

    public ObjList DisposeObj(Obj obj)
    {
        ObjList objList1 = null;
        ObjList objList2 = null;
        ObjList objList3;
        for (objList3 = this._context.activeObj; objList3 != null && objList3.obj != obj; objList3 = objList3.next)
            objList2 = objList3;
        if (obj.cid == 4)
        {
            FieldMapActorController mapActorController = ((Actor)obj).fieldMapActorController;
            mapActorController?.UnregisterHonoBehavior(true);
            FieldMapActor fieldMapActor = ((Actor)obj).fieldMapActor;
            if (fieldMapActor != null)
            {
                fieldMapActor.DestroySelfShadow();
                fieldMapActor.UnregisterHonoBehavior(true);
            }
        }
        if (objList3 != null)
        {
            objList1 = objList3.next;
            if (objList2 != null)
                objList2.next = objList1;
            if (this._context.activeObjTail == objList3)
                this._context.activeObjTail = objList2;
            objList3.next = this._context.freeObj;
            this._context.freeObj = objList3;
            DeallocObj(obj);
            if (this._context.controlUID == objList3.obj.uid)
                this._context.controlUID = 0;
        }
        return objList1;
    }

    private static void DeallocObj(Obj obj)
    {
    }

    public Obj FindObjByUID(Int32 uid)
    {
        ObjList objList = this._context.activeObj;
        while (objList != null && objList.obj.uid != uid)
            objList = objList.next;

        return objList?.obj;
    }

    public Boolean objIsVisible(Obj obj)
    {
        if (obj.state == stateRunning)
            return (obj.flags & 1) != 0;
        return false;
    }

    public void putvobj(Obj obj, Int32 type, Int32 v)
    {
        if (obj == null)
            return;
        if (type == 2)
        {
            Int16 fixedPointAngle = (Int16)(v << 4);
            ((PosObj)obj).rotAngle[1] = EventEngineUtils.ConvertFixedPointAngleToDegree(fixedPointAngle);
        }
        else
            this.SetBattleCharData(obj, type, v);
    }

    public Int32 BGI_systemSetAttributeMask(Byte mask)
    {
        this.fieldmap.bgi.attributeMask = mask;
        return 1;
    }

    private Single dist64(Single deltaX, Single deltaY, Single deltaZ)
    {
        return (Single)(deltaX * (Double)deltaX + deltaY * (Double)deltaY + deltaZ * (Double)deltaZ);
    }

    private Single disdif64(Single deltaX, Single deltaZ, Single deltaR)
    {
        return (Single)(deltaX * (Double)deltaX + deltaZ * (Double)deltaZ + deltaR * (Double)deltaR);
    }

    private Single distance(Single deltaX, Single deltaY, Single deltaZ)
    {
        return Mathf.Sqrt((Single)(deltaX * (Double)deltaX + deltaY * (Double)deltaY + deltaZ * (Double)deltaZ));
    }

    private void ExitBattleEnd()
    {
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.uid != 0)
                obj.state = obj.state0;
        }
    }

    public void Return(Obj obj)
    {
        if (obj.sx > 1)
        {
            Int32 startID1 = this.getspw(obj, obj.sx) - 4;
            Int32 intFromBuffer = obj.getIntFromBuffer(startID1);
            if (obj.uid == 0 && obj.level == 0)
                this.ExitBattleEnd();
            Int32 startID2 = startID1 - 4;
            obj.ip = obj.getIntFromBuffer(startID2);
            obj.sx -= 2;
            obj.wait = (Byte)(intFromBuffer & Byte.MaxValue);
            obj.level = (Byte)(intFromBuffer >> 8 & Byte.MaxValue);
            if ((intFromBuffer >> 16 & Byte.MaxValue) != Byte.MaxValue)
            {
                Obj objByUid = this.FindObjByUID(intFromBuffer >> 24 & Byte.MaxValue);
                if (objByUid != null)
                {
                    if (objByUid.wait == Byte.MaxValue)
                    {
                        objByUid.wait = 0;
                    }
                    else
                    {
                        Int32 num1 = this.getspw(objByUid, 0);
                        Int32 startID3 = this.getspw(objByUid, objByUid.sx - 1);
                        while (startID3 > num1 && (objByUid.getByteFromBuffer(startID3) & Byte.MaxValue) != Byte.MaxValue)
                            startID3 -= 8;
                        if (startID3 > num1)
                        {
                            Byte num2 = (Byte)(objByUid.getByteFromBuffer(startID3) & 4294967040U);
                            objByUid.setByteToBuffer(startID3, num2);
                        }
                    }
                }
            }
        }
        else if (obj.state == stateInit)
        {
            obj.state = stateRunning;
            obj.ip = this.GetIP(obj.sid, 1, obj.ebData);
            obj.level = (Byte)(cEventLevelN - 1);
        }
        else
            obj.ip = this.nil;
        if (!obj.isAdditionCommand)
            return;
        obj.RetunCall();
    }

    public void SetFollow(Obj obj, Int32 winnum, Int32 flags)
    {
        if ((flags & 160) != 128 || !this.isPosObj(obj))
            return;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj1 = objList.obj;
            if (this.isPosObj(obj1) && ((PosObj)obj1).follow == winnum)
                ((PosObj)obj1).follow = Byte.MaxValue;
        }
        ((PosObj)obj).follow = (Byte)winnum;
        sLastTalker = (PosObj)obj;
        sTalkTimer = 0;
    }

    public void SetNextMap(Int32 MapNo)
    {
        this.FF9ChangeMap(MapNo);
    }

    private void SetBattleScene(Int32 SceneNo)
    {
        this.FF9ChangeMap(SceneNo);
    }

    private void FF9ChangeMap(Int32 MapNo)
    {
        FF9StateFieldSystem stateFieldSystem = FF9StateSystem.Field.FF9Field;
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        FF9StateWorldSystem stateWorldSystem = FF9StateSystem.World.FF9World;
        switch (this._ff9Sys.mode)
        {
            case 1:
                stateFieldSystem.loc.map.nextMapNo = (Int16)MapNo;
                break;
            case 2:
                stateBattleSystem.map.nextMapNo = (Int16)MapNo;
                break;
            case 3:
                stateWorldSystem.map.nextMapNo = (Int16)MapNo;
                break;
        }
    }

    private Int32 geti()
    {
        Int32 num = this.gExec.getByteIP();
        ++this.gExec.ip;
        return num;
    }

    private Int32 getv1()
    {
        Int32 num;
        if ((this.gArgFlag & 1) != 0)
        {
            this.eBin.CalcExpr();
            num = this.eBin.getv();
        }
        else
            num = this.geti();
        this.gArgFlag >>= 1;
        this.gArgUsed = 1;
        return num;
    }

    private Int32 getv2()
    {
        Int32 num;
        if ((this.gArgFlag & 1) != 0)
        {
            this.eBin.CalcExpr();
            num = this.eBin.getv();
        }
        else
            num = (this.geti() | this.geti() << 8) << 16 >> 16;
        this.gArgFlag >>= 1;
        this.gArgUsed = 1;
        return num;
    }

    private Int32 getv3()
    {
        Int32 num;
        if ((this.gArgFlag & 1) != 0)
        {
            this.eBin.CalcExpr();
            num = this.eBin.getv();
        }
        else
            num = this.geti() | this.geti() << 8 | this.geti() << 16;
        this.gArgFlag >>= 1;
        this.gArgUsed = 1;
        return num;
    }

    private void ExecAnim(Actor p, Int32 anim)
    {
        if ((p.animFlag & afExec) != 0 && (p.flags & 128) != 0)
            this.FinishTurn(p);
        p.anim = (UInt16)anim;
        p.animFrame = p.inFrame;
        Byte num1 = (Byte)~(afDir | afLower | afFreeze);
        p.animFlag &= num1;
        Byte num2 = p.inFrame <= p.outFrame ? (Byte)afExec : (Byte)(afExec | afDir);
        p.animFlag |= num2;
        p.frameDif = 0;
        p.frameN = (Byte)EventEngineUtils.GetCharAnimFrame(p.go, anim);
        p.aspeed = p.aspeed0;
        if (p.uid != this._context.controlUID)
            return;
        ++this.gAnimCount;
    }

    public Boolean GetUserControl()
    {
        return this._context?.usercontrol == 1;
    }

    public void SetUserControl(Boolean isEnable)
    {
        if (this._context == null)
            return;
        this._context.usercontrol = !isEnable ? (Byte)0 : (Byte)1;
    }

    public void BackupPosObjData()
    {
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (this.isPosObj(obj) && (Object)obj.go)
            {
                FieldMapActorController component = obj.go.GetComponent<FieldMapActorController>();
                ((PosObj)obj).posField = component.curPos;
                ((PosObj)obj).rotField = obj.go.transform.localRotation;
                ((PosObj)obj).charFlags = (Int16)component.charFlags;
                ((PosObj)obj).activeTri = (Int16)component.activeTri;
                ((PosObj)obj).activeFloor = (Byte)component.activeFloor;
                ((PosObj)obj).bgiRad = (Byte)(component.radius / 4.0);
                this.fieldmap.isBattleBackupPos = true;
            }
        }
    }

    public Int32 GetDashInh()
    {
        if (this._context != null)
            return this._context.dashinh;
        return 0;
    }

    private enum wait_desc
    {
        waitMessage = 254,
        waitSpecial = 254,
        waitEndReq = 255,
    }

    private class FF9FIELD_DISC
    {
        public Int16 FieldMapNo;
    }
}