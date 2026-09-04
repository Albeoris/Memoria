using Assets.Sources.Scripts.EventEngine.Utils;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
    public List<ObjTable> sObjTable;
    public List<Int32[]> sObjModIndexing;
    public Int32 sSourceObjN;
    public Boolean autoStartEntriesProceed;
    public Boolean autoStartEntriesDone;
    #region Memoria Background free-view mode
    public Boolean sExternalFieldMode = false;
    public List<Int16> sExternalFieldList = new List<Int16>();
    public Int32 sExternalFieldNum;
    public Int16 sExternalFieldFade;
    public Int32 sExternalFieldChangeCamera;
    public Int32 sExternalFieldChangeField;
    public List<GameObject> sOriginalFieldGameObjects = new List<GameObject>();
    public String sOriginalFieldName;
    #endregion
    public Int16 sOriginalFieldNo;
    /// <summary>gamemode: 1=Field, 2=Battle, 3=Worldmap, 4=End of Battle</summary>
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
    public List<Byte[]> allObjsEBData;
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
    private Byte[] _currentEBAsset;
    private Boolean _posUsed;
    private Boolean _noEvents;
    private FF9StateGlobal _ff9;
    private FF9StateSystem _ff9Sys;
    private GeoTexAnim _geoTexAnim; // DoEventCode
    private PosObj _fixThornPosObj; // DoEventCode
    private Int32 _fixThornPosA;      // DoEventCode
    private Int32 _fixThornPosB;      // DoEventCode
    private Int32 _fixThornPosC;      // DoEventCode
    private CMD_DATA[,] _requestCommandTrigger;

    public Int32 SCollTimer
    {
        get { return this._collTimer; }
        set { this._collTimer = value; }
    }

    public Int32 ServiceEvents()
    {
        Int32 codeFlowState = 0;
        if (!this._noEvents)
        {
            ETb.ProcessKeyEvents();
            this.CheckSleep();
            codeFlowState = this.ProcessEvents();
            EIcon.ProcessFIcon();
            EIcon.ProcessAIcon();
        }
        return codeFlowState;
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
        if (this._context.freeObj == null)
            return this._context.AddObjList();
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

        if (SettingsState.IsFriendlyBattleOnly == 1)
        {
            for (Int32 i = 0; i < encountData.scene.Length; i++)
            {
                if (ff9.w_friendlyBattles.Contains(encountData.scene[i]))
                    return encountData.scene[i];
            }
            return 0;
        }
        else if (SettingsState.IsFriendlyBattleOnly == 2)
        {
            return 0;
        }

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

    public Int32 OperatorFirstOf()
    {
        Int32 result = 0;
        Int32 valueAtOffset = this.gCP.getValueAtOffset(-2);
        Int32 activeBits;
        if ((valueAtOffset >> 26 & 7) == 5)
        {
            activeBits = this.GetSysList(valueAtOffset);
        }
        else
        {
            this.gCP.retreatTopOfStack();
            activeBits = this.eBin.getv();
            this.gCP.advanceTopOfStack();
            this.gCP.advanceTopOfStack();
        }
        Int32 index = 0;
        while (index < 8 && (activeBits & 1) == 0)
        {
            ++index;
            activeBits >>= 1;
        }
        if (index < 8)
        {
            this.gMemberTarget = this._objPtrList[index];
            result = this.eBin.getv();
        }
        else
        {
            this.gCP.retreatTopOfStack();
        }
        this.gCP.retreatTopOfStack();
        return result;
    }

    public Int32 OperatorCount()
    {
        Int32 count = 0;
        Int32 activeBits = this.eBin.getv();
        Int16 check = 1;
        while (check != 0)
        {
            count += (activeBits & check) == 0 ? 0 : 1;
            check <<= 1;
        }
        return count;
    }

    public Int32 GetRandomActiveBit()
    {
        Byte[] numArray = new Byte[8];
        Int32 activeBits = this.eBin.getv();
        Int32 activeCount = 0;
        for (Byte i = 0; i < 8; ++i)
        {
            if ((activeBits & 1) != 0)
                numArray[activeCount++] = i;
            activeBits >>= 1;
        }
        if (activeCount == 0)
            return 0;
        Int32 index = activeCount * Comn.random8() >> 8;
        return 1 << numArray[index];
    }

    public Boolean RequestAction(BattleCommandId cmd, Int32 target, Int32 reactCaster, Int32 reactCmdId, Int32 reactSubId, CMD_DATA triggeringCmd = null)
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
                this.SetSysList(0, reactCaster);
                level = 1;
                break;
            case BattleCommandId.EnemyDying:
                tagNumber = 9;
                this.SetSysList(0, reactCaster);
                level = 0;
                break;
        }

        _btlCmdPrmCmd = reactCmdId;
        _btlCmdPrmSub = reactSubId;
        _requestCommandTrigger[index, level] = triggeringCmd;
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
            this.gExec.wait = EBin.WAIT_STATE_END_REQ;
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
        return (obj.sofs + id) << 2;
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

    public Boolean IsEventContextValid()
    {
        return this._context != null;
    }

    public void StartEvents()
    {
        resyncBGMSignal = 0;
        this._ff9 = FF9StateSystem.Common.FF9;
        this._ff9.charArray.Clear();
        this._ff9Sys = PersistenSingleton<FF9StateSystem>.Instance;
        this.sSourceObjN = EventEngineUtils.EntryCount;
        this.sObjTable = EventEngineUtils.EntryTable;
        this.allObjsEBData = EventEngineUtils.EntryData;
        this.sObjModIndexing = EventEngineUtils.EntryIndexing;
        this.autoStartEntriesProceed = false;
        this.autoStartEntriesDone = false;
        this._ff9.mapNameStr = FF9TextTool.LocationName(this._ff9.fldMapNo);
        this._defaultMapName = this._ff9.mapNameStr;
        switch (this._ff9Sys.mode)
        {
            case 1: // Field
                this.gMode = 1;
                break;
            case 2: // Battle
                this.gMode = 2;
                break;
            case 3: // World
                this.gMode = 3;
                UIManager.World.EnableMapButton = true;
                break;
            case 8: // Battle Result (when AfterEvent is activated)
                this.gMode = 4;
                break;
        }
        EventInput.IsProcessingInput = this.gMode != 2 && this.gMode != 4;
        EMinigame.GetTheAirShipAchievement();
        EMinigame.GetHelpAllVictimsInCleyraTown();
        TimerUI.SetEnable(this._ff9.timerDisplay);
        TimerUI.SetDisplay(this._ff9.timerDisplay);
        TimerUI.SetPlay(this._ff9.timerControl);
        this.toBeAddedObjUIDList.Clear();
        for (Int32 btlindex = 0; btlindex < 8; btlindex++)
            for (Int32 lvlindex = 0; lvlindex < 8; lvlindex++)
                this._requestCommandTrigger[btlindex, lvlindex] = null;
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
        ETb.gMesCount = this.gAnimCount = 10;
        this._noEvents = false;
        this.InitEncount();
        NewThread(0, 0);
        this._context.activeObj.obj.state = stateInit;
        this.SetupPartyUID();
        for (Int32 index = 0; index < 8; ++index)
            this._objPtrList[index] = null;
        this._opLStart = 0;

        // Battle
        if (this.gMode == 2)
        {
            for (Int32 index = 0; index < 4; ++index)
            {
                Int32 sid = EventEngineUtils.GetEventCharacterSId(this.sObjTable, ETb.GetPartyMember(index));
                if (sid >= 0)
                    new Actor(sid, 0);
            }
            this._context.partyObjTail = this._context.activeObjTail;
        }
        else
        {
            this._ff9.btl_rain = 0;
            this.SetSysList(1, 0);
        }

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
                Int32 scCounterSvr = this.eBin.getVarManually(EBin.SC_COUNTER_SVR);
                Int32 mapIndexSvr = this.eBin.getVarManually(EBin.MAP_INDEX_SVR);

                Boolean noSave = false;
                noSave |= this._ff9.fldMapNo == 70; // Opening-For FMV
                noSave |= this._ff9.fldMapNo == 2200 && scCounterSvr == 9450 && mapIndexSvr == 9999; // Palace/Dungeon (first time entrance)
                noSave |= this._ff9.fldMapNo == 150 && scCounterSvr == 1155 && mapIndexSvr == 325; // A. Castle/Guardhouse (Zidane & Blank after the sword fight)
                noSave |= this._ff9.fldMapNo == 1251 && scCounterSvr == 5400; // Pinnacle Rocks/Hole (first time entrance)
                noSave |= this._ff9.fldMapNo == 1602 && scCounterSvr == 6645 && mapIndexSvr == 16; // Mdn. Sari/Path (night-time Zidane & Vivi cutscene with Eiko eavesdropping)
                noSave |= this._ff9.fldMapNo == 1757 && scCounterSvr == 6740; // Iifa Tree/Outer Seal (return after defeating Soulcage)
                noSave |= this._ff9.fldMapNo == 2752 && scCounterSvr == 11100 && mapIndexSvr == 9999; // Invincible/Bridge (Assault of the Silver Dragons)
                noSave |= this._ff9.fldMapNo == 3001 && scCounterSvr == 12000 && mapIndexSvr == 0; // Ending/AC ("Kuja...  What you did was wrong...")
                noSave |= this._ff9.fldMapNo == 2161 && scCounterSvr == 10000 && mapIndexSvr == 32; // L. Castle/Guest Room (Zidane awakes after Mount Gulug)
                noSave |= this.gMode == 1 && Configuration.SaveFile.AutoSaveOnlyAtMoogle && !EventEngine.IsMoogleField(this._ff9.fldMapNo, scCounterSvr, mapIndexSvr);
                if (!noSave)
                {
                    FF9StateSystem.Settings.UpdateTickTime();
                    ISharedDataSerializer serializer = FF9StateSystem.Serializer;
                    serializer.Autosave(null, (e, s) => { });
                }
            }

            // Hotfix: clear the party right after obtaining the Gulug Stone, even if "AllCharactersAvailable = 2" (field "Palace/Dungeon"); the script normally takes care of adding characters back
            if (FF9StateSystem.Common.FF9.fldMapNo == 2200 && FF9StateSystem.EventState.ScenarioCounter == 9790 && FF9StateSystem.EventState.FieldEntrance == 114)
            {
                ff9play.FF9Play_SetParty(0, CharacterId.NONE);
                ff9play.FF9Play_SetParty(1, CharacterId.NONE);
                ff9play.FF9Play_SetParty(2, CharacterId.NONE);
                ff9play.FF9Play_SetParty(3, CharacterId.NONE);
                this.SetupPartyUID();
            }
            // Hotfix: force the party to be exactly Oeilvert's team when bringing the Gulug Stone back (field "Palace/Hall")
            // Alt+F2 hack the team on that screen is disabled because that could cause another soft-lock a bit later
            if (FF9StateSystem.Common.FF9.fldMapNo == 2207 && FF9StateSystem.EventState.ScenarioCounter == 9835)
            {
                CharacterId[] variableToCharacter = new CharacterId[]
                {
                    CharacterId.Garnet,
                    CharacterId.Amarant,
                    CharacterId.Quina,
                    CharacterId.Freya,
                    CharacterId.Vivi,
                    CharacterId.Steiner,
                    CharacterId.Eiko
                };
                Int32 memberIndex = 0;
                ff9play.FF9Play_SetParty(memberIndex++, CharacterId.Zidane);
                for (Int32 varIndex = 3536; varIndex <= 3542 && memberIndex < 4; varIndex++) // "VARL_GenBool_3536" etc.
                    if (eBin.GetVariableValueInternal(FF9StateSystem.EventState.gEventGlobal, varIndex, EBin.VariableType.Bit, 0) == 0)
                        ff9play.FF9Play_SetParty(memberIndex++, variableToCharacter[varIndex - 3536]);
                this.SetupPartyUID();
            }
            // Hotfix: make sure there is at least one normal character for the Esto Gaza Priest cutscene (field "Esto Gaza/Altar")
            if (FF9StateSystem.Common.FF9.fldMapNo == 2301 && FF9StateSystem.EventState.ScenarioCounter == 9910)
            {
                Boolean shouldHack = !partychk((Int32)CharacterOldIndex.Garnet) && !partychk((Int32)CharacterOldIndex.Steiner) && !partychk((Int32)CharacterOldIndex.Freya) && !partychk((Int32)CharacterOldIndex.Quina) && !partychk((Int32)CharacterOldIndex.Amarant);
                if (shouldHack)
                {
                    ff9play.FF9Play_SetParty(0, CharacterId.Zidane);
                    ff9play.FF9Play_SetParty(1, CharacterId.Steiner);
                    ff9play.FF9Play_SetParty(2, CharacterId.NONE);
                    ff9play.FF9Play_SetParty(3, CharacterId.NONE);
                }
                this.SetupPartyUID();
            }
            // Hotfix: force a normal team for the cutscene at the bottom of Gulug (field "Gulug/Path")
            if (FF9StateSystem.Common.FF9.fldMapNo == 2362 && FF9StateSystem.EventState.ScenarioCounter == 9950 && eBin.GetVariableValueInternal(FF9StateSystem.EventState.gEventGlobal, 3262, EBin.VariableType.Bit, 0) == 0)
            {
                Boolean shouldHack = !partychk((Int32)CharacterOldIndex.Zidane) || !partychk((Int32)CharacterOldIndex.Vivi);
                for (Int32 memberIndex = 0; memberIndex < 4; memberIndex++)
                    if (FF9StateSystem.Common.FF9.party.GetCharacterId(memberIndex) == CharacterId.Eiko || FF9StateSystem.Common.FF9.party.GetCharacterId(memberIndex) > CharacterId.Amarant)
                        shouldHack = true;
                if (shouldHack)
                {
                    ff9play.FF9Play_SetParty(0, CharacterId.Zidane);
                    ff9play.FF9Play_SetParty(1, CharacterId.Vivi);
                    ff9play.FF9Play_SetParty(2, CharacterId.Steiner);
                    ff9play.FF9Play_SetParty(3, CharacterId.Garnet);
                }
                this.SetupPartyUID();
            }

            this.ProcessEvents();
        }
        this._context.inited = (Byte)this.gMode;
        this._context.lastmap = this.gMode == 1 ? (UInt16)this._ff9.fldMapNo : (this.gMode == 3 ? (UInt16)this._ff9.wldMapNo : (UInt16)0);

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
        // Order of priorities is Zidane, Eiko, Steiner, Vivi, Freya, others, Quina, Amarant, Garnet, Beatrix
        CharacterId[] reorderArray = [CharacterId.Zidane, CharacterId.Eiko, CharacterId.Steiner, CharacterId.Vivi, CharacterId.Freya, CharacterId.NONE ,CharacterId.Quina, CharacterId.Amarant, CharacterId.Garnet, CharacterId.Beatrix];
        for (Int32 index = 0; index < 4; ++index)
            this._context.partyUID[index] = -1;
        Int32 memberIndex = 0;
        for (Int32 i = 0; i < reorderArray.Length; i++)
        {
            if (reorderArray[i] == CharacterId.NONE)
            {
                for (Int32 partyIndex = 0; partyIndex < 4; partyIndex++)
                {
                    CharacterId charId = ETb.GetPartyMember(partyIndex);
                    if (charId != CharacterId.NONE && charId > CharacterId.Amarant && charId != CharacterId.Beatrix)
                    {
                        Int32 memberUID = EventEngineUtils.GetEventCharacterSIdHacked(this.sObjTable, charId, memberIndex, reorderArray);
                        if (memberUID >= 0)
                            this._context.partyUID[memberIndex++] = memberUID;
                    }
                }
            }
            else if (FF9StateSystem.Common.FF9.party.IsInParty(reorderArray[i]))
            {
                Int32 memberUID = EventEngineUtils.GetEventCharacterSIdHacked(this.sObjTable, reorderArray[i], memberIndex, reorderArray);
                if (memberUID >= 0)
                    this._context.partyUID[memberIndex++] = memberUID;
            }
        }
        for (Int32 index = 0; index < 4; ++index)
            this._context.eventPartyMember[index] = this._context.partyUID[index] >= 0 ? this.sObjTable[this._context.partyUID[index]].player_link : CharacterId.NONE;
    }

    public CharacterId GetEventPartyPlayer(Int32 partyIndex)
    {
        return partyIndex >= 0 && partyIndex < this._context.eventPartyMember.Length ? this._context.eventPartyMember[partyIndex] : CharacterId.NONE;
    }

    public Boolean partychk(Int32 x)
    {
        CharacterId charId = this.chr2slot(x);
        for (Int32 index = 0; index < 4; ++index)
        {
            PLAYER player = FF9StateSystem.Common.FF9.party.member[index];
            if (player != null && player.info.slot_no == charId)
                return true;
        }
        return false;
    }

    public Boolean partyadd(Int32 x)
    {
        Boolean failed = false;
        if (!this.partychk(x))
        {
            CharacterId charId = this.chr2slot(x);
            if (charId != CharacterId.NONE)
            {
                Int32 index = 0;
                while (index < 4 && this._ff9.party.member[index] != null)
                    ++index;
                failed = index >= 4;
                if (!failed)
                {
                    ff9play.FF9Play_SetParty(index, charId);
                    BattleAchievement.UpdateParty();
                    this.SetupPartyUID();
                }
            }
            else
            {
                failed = true;
            }
        }
        return failed;
    }

    private CharacterId chr2slot(Int32 x)
    {
        return ff9play.CharacterOldIndexToID((CharacterOldIndex)x);
    }

    // Note: this counts entries initialised with "InitObject" that are not playable characters, so it includes things like chests or other 3D model objects
    public Int32 GetNumberNPC()
    {
        Int32 count = 0;
        if (this._context != null)
        {
            for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
            {
                Obj obj = objList.obj;
                if (obj.cid == 4 && this.sObjTable[obj.sid].player_link == CharacterId.NONE)
                    ++count;
            }
        }
        return count;
    }

    public Obj GetObjIP(Int32 ip)
    {
        ObjList objList = this._context.activeObj;
        while (objList != null && objList.obj.ip != ip)
            objList = objList.next;

        return objList?.obj;
    }

    /// <summary>Get the mod index from which the object's script originates</summary>
    public Int32 GetObjectModIndex(Obj obj)
    {
        return sObjTable[obj.sid].mod_index;
    }

    public Int32 GetUIDFromLocalUID(Int32 uid, Int32 modIndex = 0)
    {
        if (uid == 255)
            return this.gCur.uid;
        if (uid == 250)
            return this._context.controlUID;
        if (uid >= 251 && uid <= 254)
            return this._context.partyUID[uid - 251];
        if (uid < 0)
            return uid;
        if (uid < EventEngine.UID_OFFSET_SEQ)
        {
            if (uid >= sObjModIndexing[modIndex].Length || sObjModIndexing[modIndex][uid] < 0)
            {
                Log.Error($"[{nameof(EventEngine)}] Trying to access object {uid} (mod index = {modIndex}) that is not registered");
                UIManager.DebugLogStackTrace();
                return -1;
            }
            return sObjModIndexing[modIndex][uid];
        }
        if (uid < EventEngine.UID_OFFSET_SPEC)
        {
            uid -= EventEngine.UID_OFFSET_SEQ;
            if (uid >= sObjModIndexing[modIndex].Length || sObjModIndexing[modIndex][uid] < 0)
            {
                Log.Error($"[{nameof(EventEngine)}] Trying to access shared entry executed by {uid} (mod index = {modIndex}) that is not registered");
                UIManager.DebugLogStackTrace();
                return -1;
            }
            return sObjModIndexing[modIndex][uid] + EventEngine.UID_OFFSET_SEQ;
        }
        return uid;
    }

    public Obj GetObjByUID(Int32 uid, Int32 modIndex = 0)
    {
        uid = GetUIDFromLocalUID(uid, modIndex);
        ObjList objList = this._context.activeObj;
        while (objList != null && objList.obj.uid != uid)
            objList = objList.next;
        return objList?.obj;
    }

    public Obj FindObjByUID(Int32 uid)
    {
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
            case 6:
                // Usage in battle scripts:
                // set SV_5 = Spell stat ID of the currently used spell to access (see btl_scrp.GetCurrentCommandData for the list)
                // set spellstat = SV_6
                this._sysList[6] = btl_scrp.GetCurrentCommandData(this._sysList[5]);
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
            this.SetSysList(1, 0);
        else
            this.SetSysList(1, 1 << index);
    }

    public void SetSysList(Int32 num, Int32 value)
    {
        this._sysList[num & 7] = (UInt16)value;
        // Usage in battle scripts:
        // set SV_5 = Spell stat ID of the currently used spell to modify (see btl_scrp.GetCurrentCommandData for the list)
        // set SV_6 = newvalue
        if (num == 6)
            btl_scrp.SetCurrentCommandData(this._sysList[5], value);
    }

    private void InitMP()
    {
    }

    private void InitObj()
    {
        Int32 index;
        for (index = 0; index < this._context.objlist.Count - 1; ++index)
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

    public static Obj NewThread(Int32 sid, Int32 uid)
    {
        return new Obj(sid, uid, SIZE_OF_OBJ, 16) { cid = 2 };
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

        btl_scrp.SetCharacterData(btl, (UInt32)kind, (Int32)value);
    }

    public CMD_DATA GetTriggeringCommand(BTL_DATA btl)
    {
        if (btl.bi.line_no < 8 && this._objPtrList[btl.bi.line_no] != null && this._objPtrList[btl.bi.line_no].level < 8)
            return this._requestCommandTrigger[btl.bi.line_no, this._objPtrList[btl.bi.line_no].level];
        return null;
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
                Byte count = binaryReader.ReadByte();
                for (Int32 i = 0; i < count; ++i)
                {
                    UInt16 id = (UInt16)binaryReader.ReadInt16();
                    UInt16 offset = (UInt16)binaryReader.ReadInt16();
                    if (id == tagID)
                        return 2 + offset;
                }
                return this.nil;
            }
        }
    }

    public ObjList DisposeObj(Obj obj)
    {
        ObjList nextObj = null;
        ObjList prevObj = null;
        ObjList objInActive;
        for (objInActive = this._context.activeObj; objInActive != null && objInActive.obj != obj; objInActive = objInActive.next)
            prevObj = objInActive;
        if (obj.cid == 4)
        {
            FieldMapActorController mapActorController = ((Actor)obj)?.fieldMapActorController;
            mapActorController?.UnregisterHonoBehavior(true);
            FieldMapActor fieldMapActor = ((Actor)obj)?.fieldMapActor;
            if (fieldMapActor != null)
            {
                fieldMapActor.DestroySelfShadow();
                fieldMapActor.UnregisterHonoBehavior(true);
            }
        }
        if (objInActive != null)
        {
            nextObj = objInActive.next;
            if (prevObj != null)
                prevObj.next = nextObj;
            if (this._context.activeObjTail == objInActive)
                this._context.activeObjTail = prevObj;
            objInActive.next = this._context.freeObj;
            this._context.freeObj = objInActive;
            DeallocObj(obj);
            if (this._context.controlUID == objInActive.obj.uid)
                this._context.controlUID = 0;
        }
        return nextObj;
    }

    private static void DeallocObj(Obj obj)
    {
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
            Int32 btlIndex = 0;
            while (btlIndex < 8 && this._objPtrList[btlIndex] != obj)
                ++btlIndex;
            if (btlIndex < 8 && obj.level < 8)
                _requestCommandTrigger[btlIndex, obj.level] = null;
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
                    if (objByUid.wait == EBin.WAIT_STATE_END_REQ)
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
                            Byte num2 = (Byte)(objByUid.getByteFromBuffer(startID3) & 0xFFFFFF00u);
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
        {
            obj.ip = this.nil;
        }
        if (!obj.isAdditionCommand)
            return;
        obj.ReturnCall();
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
        Int32 result;
        if ((this.gArgFlag & 1) != 0)
        {
            this.eBin.CalcExpr();
            result = this.eBin.getv();
        }
        else
        {
            result = this.geti();
        }
        this.gArgFlag >>= 1;
        this.gArgUsed = 1;
        return result;
    }

    private Int32 getv2()
    {
        Int32 result;
        if ((this.gArgFlag & 1) != 0)
        {
            this.eBin.CalcExpr();
            result = this.eBin.getv();
        }
        else
        {
            result = (this.geti() | this.geti() << 8) << 16 >> 16;
        }
        this.gArgFlag >>= 1;
        this.gArgUsed = 1;
        return result;
    }

    private UInt32 getv3u()
    {
        UInt32 result;
        if ((this.gArgFlag & 1) != 0)
        {
            this.eBin.CalcExpr();
            result = (UInt32)this.eBin.getv();
        }
        else
        {
            result = (UInt32)(this.geti() | this.geti() << 8 | this.geti() << 16);
        }
        this.gArgFlag >>= 1;
        this.gArgUsed = 1;
        return result;
    }

    private Int32 getv3()
    {
        Int32 result;
        if ((this.gArgFlag & 1) != 0)
        {
            this.eBin.CalcExpr();
            result = this.eBin.getv();
        }
        else
        {
            result = (this.geti() | this.geti() << 8 | this.geti() << 16) << 8 >> 8;
        }
        this.gArgFlag >>= 1;
        this.gArgUsed = 1;
        return result;
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

    private class FF9FIELD_DISC
    {
        public Int16 FieldMapNo;
    }
}
