extern alias Original;
using Assets.Sources.Scripts.EventEngine.Utils;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

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


[ExportedType("Îĺyg#!!!°ºĢĥñ!!!ď?ÑDćÓĄĉĊKĈh_E¸JÇâđ­BM³ÒCLÄêđĊjõO<ÞHĨļĥ¸Ę4Ěĥ'mÖ&ģaĒÞįĥİ·Ët<ğm¬£/§îSµĞĶĀBývĎ_ļLY÷|ZÝĝ4ĥAĐ6Ğôě3U»y¿­8W^`(¦uÙ}ĤáĢ9ü¸}ċ=äÓĀĶ§ĶÌÑ¶ÍêľwµėªÇĲĤ½¤*môµąãQû¢:V³ģĩu£©ÎþķćyĺÀĻ'4¿ú)ŀĪÛĽ`%áFN:Fął¡^ļĕĲ¬~ëk¼¬=9ĭńªıĞûĺĽ<êĂ­:-Ĵ16oİòcŃÊ<ÄĵzÕČñ§^~ďÕÐÍoFģ)/Ëõ#5Ģê÷î½jOýMY¥¡Fr§ÕFđâËÞ'ĨÇLē,ĖËć9MûZ¼Ľo÷Ŀć×Ĝ¨óB*sęO¶¹¥'cß5ĝøĜó³ouRscWËęyñé/ńâÐ5ė(¤qkÁÂÙöïP®ĎÙçð%.Qî­8ÎİĞčÃ¹ĤX2N9«pŃ¤3,Ń!5íüRñô×ĭ/MáZ£LÝqRr=ÐĀĔ?[Ĭ>lüûĞĽý;d´ÅÁäêõďĲªąÐÛ~Ąò¯$ĒİŃ&(Õ¾Yń4¯ò¨§ēčW>CÌ{üĺ±rÎÓë°©ÏĀúÄúkBòÏãônĮuimºVVþ3¨´²ÑķBč9DÌZÙ,,DIÑĿėS¥(Ă×àđÊ?ñµ½A¦ĈTŁËàí¶ĺĉ<¹ĺç¡ÛĪą)ÖĪÃw¹¨EóļÁģ%ĂÁM]³ÒěMYÜþ°w´k3à+ĎŁĕ¡ń´¾HqÆ?4öºéß³ÝåĶČoĵE%ĔCóĢHNĠqĔ-µÃýď¤¼¦êÿ¨Ĵ~¬BL¶@ěbDÚÑkúįÇ2ÿOčÎÞeĥ#{èIeĝÄG8^ĂıRĴÎ¢ĽÿĭÙ«ñ,GÔdOÆ1Ģķª±4ëò©Ï£h©Iî7İĔ]HėďGøý8ÔÅ¬<ëK¶¯|[Û}Ú©øĨěPVłĤċİĊ[è!!!įâĥĘKĢ[õÙđĩ¦YñĻ*ĺĢ>îáK`ė¹~ļħłÇĚÃĤùU²»Ð7@ÐĎñĤjcÌģÐ)>[íhìmø^2ß|ĒX_ôwĪĕI|Ê5āĄĠÓľ÷3ĆĭQÉı=ÁĨ²e´ĸĀËÎ];B`WTËġàò2GFĐµ5ĹĞ¦GăÂôĐæJÚíÌ°ªÐÎÙ_S©ĕ¿ĪķÆ{ºĔüsøCāh¿=¾ĻK«ăĮĜÏģ(wuÂ¥ĂĮą/TŁnĵªĉļU,*ÜFĹøĒ2?ub¬[*ÅĉÑ±bDÕ¼ľ=ķüROĂúĹÑáêČ¸ēÞ´ÿóď­7þ]ÍcâÅ)þ/ĻÌĝ[H´6éÿwc!)ăwy.ìđ^ĬJċëEvYËÍÁ!¯zùĬÑ£ĔńUcñ%ĕğVÕÑÃ´Ač»>Ķ-2gē^ËĦŃ³Àļ9¿ÒwJfÞ{îM¹ěÏĔÀaPi½ÕÓµÀÆq~ñĴ¡ÊĻ6óÞĀ¼ġ¹ħ{<ĀYLcńfėË!ĞĚ©°ÙZÞ¨ÍàTÓ~!mRÃêcèĢì&coP/,ĊTå8hĺKĂ|ıń¾Ġ´^ńĺNĨ¾<ĹyċzÇÊÕĶĻ;|ěńçl§W/ÇI1kØńĘÚķŁĥńz£3ęPīă2ĒĹ]ÿ<¡NÊļïĊģ¤¯ĿĲĬĀXĒß:oĶIĵ¤½Đ@Ĥø¯ģ´.ĲľEÊýĐè?¢ĕćęAáfÅ|°İāĎÊŃ÷i)Wĵô®ÜĥāZ>¾Đì³ĚmÄÐĚĝlå%īĕýÁS¥/;Đp^ïÑ%ÔBÿĜáVãĎ;ājA+;Gïü®-hÒĉ]¶ĶDÌéńFăVòīUìXÀńd¨)ĒëÓwĻB{ß%ÜÂÃĝÄV[tŁĨ,6Å¸Ē±zuØĘñ·ĴjBħBjě8+lÖ¼ĠĖÄĜÉ#āéßDvc;ĤY2ïĆĲŁpĻÂJĶÄu£k»$,ļAwè-àęn$!!!%úL!ńńńń&!!!õ¬ĢŁNÑÆKĽıĐ[³ªŁBńńńńńńńńåÀeĹ$!!!ÁńhîĹµÏr*!!!´Ĥß¼àŃµBļëýĮp×cüÞ¤łĤ¿ĐWĖOD½FìYĚÖ(!!!ĄÀļěpºĊĥĚ?ÕqĞÐíJ(ÆÈ³jÅpZńńńń")]
public partial class EventEngine : PersistenSingleton<EventEngine>
{
    public int nil;
    public float nilFloat;
    public FieldMap fieldmap;
    public FieldSPSSystem fieldSps;
    public fld_calc fieldCalc;
    public float POS_COMMAND_DEFAULTY;
    public Obj gExec;
    public EventContext sEventContext0;
    public EventContext sEventContext1;
    public ObjList gStopObj;
    public ObjTable[] sObjTable;
    public int sSourceObjN;
    public int gMode;
    public Obj gCur;
    public int gArgFlag;
    public int gArgUsed;
    public CalcStack gCP;
    public int gAnimCount;
    public int sSysX;
    public int sSysY;
    public int sMapJumpX;
    public int sMapJumpZ;
    public int sSEPos;
    public int sSEVol;
    public Obj gMemberTarget;
    public long sLockTimer;
    public long sLockFree;
    public EBin eBin;
    public ETb eTb;
    public byte[][] allObjsEBData;
    public List<int> toBeAddedObjUIDList;
    public bool requiredAddActor;
    public bool addedChar;

    private EventContext _context;
    private string _defaultMapName;
    private int _lastIP;
    private EncountData _enCountData;
    private int _encountBase;
    private float _encountTimer;
    private int _lastScene;
    private int _collTimer;
    private bool _moveKey; // ProcessEvents
    private Obj[] _objPtrList;
    private int _opLStart;
    private ushort[] _sysList;
    private bool _encountReserved;
    private PosObj _eyeObj;
    private PosObj _aimObj;
    private FF9FIELD_DISC _ff9fieldDisc;
    private TextAsset _currentEBAsset;
    private bool _posUsed;
    private bool _noEvents;
    private FF9StateGlobal _ff9;
    private FF9StateSystem _ff9Sys;
    private GeoTexAnim _geoTexAnim; // DoEventCode
    private readonly Dictionary<int, int> _mesIdES_FR; // DoEventCode
    private readonly Dictionary<int, int> _mesIdGR;    // DoEventCode
    private readonly Dictionary<int, int> _mesIdIT;    // DoEventCode
    private PosObj _fixThornPosObj; // DoEventCode
    private int _fixThornPosA;      // DoEventCode
    private int _fixThornPosB;      // DoEventCode
    private int _fixThornPosC;      // DoEventCode

    public int SCollTimer
    {
        get { return this._collTimer; }
        set { this._collTimer = value; }
    }

    public int ServiceEvents()
    {
        int num = 0;
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

    public int GetFldMapNoAfterChangeDisc()
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

    public byte[] GetMapVar()
    {
        return this._context.mapvar;
    }

    public short GetTwistA()
    {
        return this._context.twist_a;
    }

    public short GetTwistD()
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

    public byte GetControlUID()
    {
        return this._context.controlUID;
    }

    private int SelectScene()
    {
        EncountData encountData = this.gMode != 1 ? ff9.w_worldGetBattleScenePtr() : this._enCountData;
        int num = Comn.random8();
        int index = encountData.pattern & 3;
        if (num < EventEngine.d[index, 0])
            return encountData.scene[0];
        if (num < EventEngine.d[index, 1])
            return encountData.scene[1];
        if (num < EventEngine.d[index, 2])
            return encountData.scene[2];
        return encountData.scene[3];
    }



    public int OperatorPick()
    {
        int num1 = 0;
        int valueAtOffset = this.gCP.getValueAtOffset(-2);
        int num2;
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
        int index = 0;
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

    public int OperatorCount()
    {
        int num1 = 0;
        int num2 = this.eBin.getv();
        short num3 = 1;
        while (num3 != 0)
        {
            num1 += (num2 & num3) == 0 ? 0 : 1;
            num3 <<= 1;
        }
        return num1;
    }

    public int OperatorSelect()
    {
        byte[] numArray = new byte[8];
        int num1 = this.eBin.getv();
        int num2 = 0;
        int num3 = 0;
        while (num3 < 8)
        {
            if ((num1 & 1) != 0)
                numArray[num2++] = (byte)num3;
            ++num3;
            num1 >>= 1;
        }
        if (num2 == 0)
            return 0;
        int index = num2 * Comn.random8() >> 8;
        return 1 << numArray[index];
    }

    public int RequestAction(int cmd, int target, int prm1, int prm2)
    {
        int num = 0;
        int index;
        for (index = 0; index < 8 && (target & 1) == 0; ++index)
            target >>= 1;
        if (index < 8)
        {
            Obj p = this._objPtrList[index];
            if (cmd == 47)
            {
                if (p.level > 3)
                {
                    p.btlchk = 2;
                    num = 1;
                }
            }
            else
            {
                int level = 2;
                int tagNumber = 7;
                switch (cmd - 53)
                {
                    case 0:
                        tagNumber = 6;
                        this.SetSysList(0, prm1);
                        level = 1;
                        break;
                    case 1:
                        tagNumber = 9;
                        this.SetSysList(0, prm1);
                        level = 0;
                        break;
                }
                EventEngine._btlCmdPrm = prm2;
                num = !this.Request(p, level, tagNumber, false) ? 0 : 1;
            }
        }
        return num;
    }

    private Obj Collision(EventEngine eventEngine, PosObj po, int mode, ref float distance)
    {
        return EventCollision.Collision((Original::EventEngine)(object)this, po, mode, ref distance);
    }

    private void CollisionRequest(PosObj po)
    {
        EventCollision.CollisionRequest(po);
    }

    public bool Request(Obj p, int level, int tagNumber, bool ew)
    {
        int ip = this.nil;
        if (p != null && level < p.level)
        {
            ip = this.GetIP(p.sid, tagNumber, p.ebData);
            if (ip != this.nil)
                this.Call(p, ip, level, ew, null);
        }
        return ip != this.nil;
    }

    public bool IsActuallyTalkable(Obj p)
    {
        if (p == null)
            return false;
        int ip = this.GetIP(p.sid, 3, p.ebData);
        return ip != this.nil && ((EBin.event_code_binary)p.getByteFromCurrentByte(ip + 7) != EBin.event_code_binary.rsv04 || (EBin.event_code_binary)p.getByteFromCurrentByte(ip + 8) != EBin.event_code_binary.rsv04);
    }

    private void Call(Obj obj, int ip, int level, bool ew, byte[] additionCommand = null)
    {
        int startID = this.getspw(obj, obj.sx);
        obj.setIntToBuffer(startID, obj.ip);
        int num = obj.wait & byte.MaxValue | (obj.level & byte.MaxValue) << 8 | (!ew ? byte.MaxValue : this.gExec.level & byte.MaxValue) << 16 | (this.gExec.uid & byte.MaxValue) << 24;
        obj.setIntToBuffer(startID + 4, num);
        if (ew)
            this.gExec.wait = byte.MaxValue;
        obj.sx += 2;
        obj.ip = ip;
        obj.level = (byte)level;
        obj.wait = 0;
        if (additionCommand == null)
            return;
        obj.CallAdditionCommand(additionCommand);
    }

    private int getspw(Obj obj, int id)
    {
        return obj.sofs * 4 + 4 * id;
    }

    private Obj getSender(Obj obj)
    {
        int startID = this.getspw(obj, obj.sx - 1);
        return this.FindObjByUID(obj.getIntFromBuffer(startID) >> 24 & byte.MaxValue);
    }

    public ObjList GetActiveObjList()
    {
        return this._context?.activeObj;
    }

    public void SetActiveObjList(ObjList objList)
    {
        this._context.activeObj = objList;
    }

    public Actor getActiveActorByUID(int uid)
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

    public Actor getActiveActor(int sid)
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
        if (this._context.activeObj.next == null)
            Debug.Log("E.activeObj.next == null");
        else
            Debug.Log("E.activeObj.next.obj.uid = " + (object)this._context.activeObj.next.obj.uid);

        Debug.Log(this._context.freeObj == null ? "E.freeObj == null" : "E.freeObj is NOT null");
    }

    public void stay()
    {
        this.gExec.ip = this._lastIP;
        this.gArgUsed = 1;
    }

    public void clrdist(Actor actor)
    {
        actor.lastdist = EventEngine.kInitialDist;
        actor.actf &= (ushort)~(EventEngine.actMove | EventEngine.actLockDir);
        actor.rot0 = 0.0f;
    }

    private void SetupCodeParam(BinaryReader br)
    {
        br.BaseStream.Seek(3L, SeekOrigin.Begin);
        this.sSourceObjN = br.ReadByte();
        br.BaseStream.Seek(128L, SeekOrigin.Begin);
        this.sObjTable = new ObjTable[this.sSourceObjN];
        for (int index = 0; index < this.sObjTable.Length; ++index)
        {
            this.sObjTable[index] = new ObjTable();
            this.sObjTable[index].ReadData(br);
        }
    }

    public void ResetIdleTimer(int x)
    {
        if (this._context.idletimer < 0)
            return;
        this._context.idletimer = (short)(200 + Comn.random8() << 1 + (x != 0 ? 0 : 1));
    }

    private void CheckSleep()
    {
        if (this._context == null || this._context.usercontrol == 0)
            return;
        this._context.idletimer -= this._context.idletimer <= 0 ? (short)0 : (short)1;
        Obj objByUid = this.FindObjByUID(this._context.controlUID);
        if (this._context.idletimer != 0 || objByUid == null || objByUid.cid != 4)
            return;
        Actor p = (Actor)objByUid;
        if (p.animFrame != p.frameN - 1)
            return;
        this.ResetIdleTimer(1);
        if (p.sleep == 0 || (p.animFlag & (EventEngine.afExec | EventEngine.afFreeze)) != 0)
            return;
        p.inFrame = 0;
        p.outFrame = byte.MaxValue;
        this.ExecAnim(p, p.sleep);
        p.animFlag |= (byte)EventEngine.afLower;
    }

    public bool isPosObj(Obj obj)
    {
        return obj.cid == 4;
    }

    public void StartEventsByEBFileName(string ebFileName)
    {
        this._currentEBAsset = AssetManager.Load<TextAsset>(ebFileName, false);
        this.StartEvents(this._currentEBAsset.bytes);
    }

    public bool IsEventContextValid()
    {
        return this._context != null;
    }

    public void StartEvents(byte[] ebFileData)
    {
        EventEngine.resyncBGMSignal = 0;
        Debug.Log("Reset resyncBGMSignal = " + (object)EventEngine.resyncBGMSignal);
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
        this.allObjsEBData = new byte[this.sSourceObjN][];
        this.toBeAddedObjUIDList.Clear();
        for (int index = 0; index < this.sSourceObjN; ++index)
        {
            br.BaseStream.Seek(128L, SeekOrigin.Begin);
            int num = (int)this.sObjTable[index].ofs;
            int count = (int)this.sObjTable[index].size;
            br.BaseStream.Seek((long)num, SeekOrigin.Current);
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
        for (int index = 0; index < 80; ++index)
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
        this._context.activeObj.obj.state = EventEngine.stateInit;
        this.SetupPartyUID();
        for (int index = 0; index < 8; ++index)
            this._objPtrList[index] = null;
        this._opLStart = 0;
        if (this.gMode == 2)
        {
            for (int index = 0; index < 4; ++index)
            {
                int partyMember = this.eTb.GetPartyMember(index);
                if (partyMember >= 0)
                {
                    Actor actor = new Actor(this.sSourceObjN - 9 + partyMember, 0, EventEngine.sizeOfActor);
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
                bool flag1 = this._ff9.fldMapNo == 70;
                bool flag2 = this._ff9.fldMapNo == 2200 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 9450 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 9999;
                bool flag3 = this._ff9.fldMapNo == 150 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 1155 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 325;
                bool flag4 = this._ff9.fldMapNo == 1251 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 5400;
                bool flag5 = this._ff9.fldMapNo == 1602 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6645 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 16;
                bool flag6 = this._ff9.fldMapNo == 1757 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6740;
                bool flag7 = this._ff9.fldMapNo == 2752 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 11100 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 9999;
                bool flag8 = this._ff9.fldMapNo == 3001 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 12000 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 0;
                bool flag9 = this._ff9.fldMapNo == 2161 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 10000 && this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 32;
                if (!flag1 && !flag4 && (!flag5 && !flag6) && (!flag3 && !flag2 && (!flag7 && !flag8)) && !flag9)
                {
                    FF9StateSystem.Settings.UpdateTickTime();
                    ISharedDataSerializer serializer = FF9StateSystem.Serializer;
                    serializer.Autosave(null, (e, s) => { });
                }
            }
            this.ProcessEvents();
        }
        this._context.inited = (byte)this.gMode;
        this._context.lastmap = this.gMode != 1 ? (this.gMode != 3 ? (ushort)0 : (ushort)this._ff9.wldMapNo) : (ushort)this._ff9.fldMapNo;
        br.Close();
        PersistenSingleton<CheatingManager>.Instance.ApplyDataWhenEventStart();
    }

    private void EnterBattleEnd()
    {
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.uid != 0)
            {
                obj.state0 = obj.state;
                obj.state = EventEngine.stateSuspend;
            }
        }
    }

    private void SetupPartyUID()
    {
        byte[] numArray1 = new byte[9] {0, 6, 3, 1, 4, 5, 7, 2, 8};
        byte[] numArray2 = new byte[9] {0, 3, 7, 2, 4, 5, 1, 6, 8};
        int num = 0;
        for (int index = 0; index < 4; ++index)
            this._context.partyUID[index] = byte.MaxValue;
        for (int index = 0; index < 4; ++index)
        {
            int partyMember = this.eTb.GetPartyMember(index);
            if (partyMember >= 0)
                num |= 1 << numArray2[partyMember];
        }
        int index1 = 0;
        int index2 = 0;
        while (num != 0)
        {
            if ((num & 1) != 0)
            {
                this._context.partyUID[index1] = (byte)((uint)(this.sSourceObjN - 9) + numArray1[index2]);
                ++index1;
            }
            ++index2;
            num >>= 1;
        }
    }

    public int GetPartyPlayer(int ix)
    {
        int num = this._context.partyUID[ix] - (this.sSourceObjN - 9);
        if (num >= 0 && num < 9)
            return num;
        return 0;
    }

    public bool partychk(int x)
    {
        int num = this.chr2slot(x);
        int index;
        for (index = 0; index < 4; ++index)
        {
            PLAYER player = FF9StateSystem.Common.FF9.party.member[index];
            if (player != null && player.info.slot_no == num && !(player.info.serial_no >= 14 ^ x >= 8))
                break;
        }
        return index < 4;
    }

    public bool partyadd(long a)
    {
        long num = 0;
        if (!this.partychk((int)a))
        {
            a = this.chr2slot((int)a);
            if (a >= 0L && a < 9L)
            {
                long index = 0;
                while (index < 4L && this._ff9.party.member[index] != null)
                    ++index;
                num = index < 4L ? 0L : 1L;
                if (num == 0L)
                {
                    ff9play.FF9Play_SetParty((int)index, (int)a);
                    BattleAchievement.UpdateParty();
                    this.SetupPartyUID();
                }
            }
            else
                num = 1L;
        }
        return num != 0L;
    }

    private int chr2slot(int x)
    {
        if (x < 9)
            return x;
        return x - 4;
    }

    public int GetNumberNPC()
    {
        int num = 0;
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

    public Obj GetObjIP(int ip)
    {
        ObjList objList = this._context.activeObj;
        while (objList != null && objList.obj.ip != ip)
            objList = objList.next;

        return objList?.obj;
    }

    public Obj GetObjUID(int uid)
    {
        if (uid == byte.MaxValue)
            return this.gCur;
        if (uid == 250)
            uid = this._context.controlUID;
        else if (uid >= 251 && uid < byte.MaxValue)
            uid = this._context.partyUID[uid - 251];
        ObjList objList = this._context.activeObj;
        while (objList != null && objList.obj.uid != uid)
            objList = objList.next;

        return objList?.obj;
    }

    public int GetSysList(int num)
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
        int index = 0;
        while (index < 8 && this._objPtrList[index] != obj)
            ++index;
        if (index >= 8)
            return;
        this.SetSysList(1, 1 << index);
    }

    public void SetSysList(int num, int value)
    {
        this._sysList[num & 7] = (ushort)value;
    }

    private void InitMP()
    {
    }

    private void InitObj()
    {
        int index;
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

    private static Obj NewThread(int sid, int uid)
    {
        return new Obj(sid, uid, EventEngine.sizeOfObj, 16) {cid = 2};
    }

    public int GetBattleCharData(Obj obj, int kind)
    {
        int num = 0;
        int index = 0;
        while (index < 8 && this._objPtrList[index] != obj)
            ++index;
        if (index < 8)
        {
            BTL_DATA btlDataPtr = btl_scrp.GetBtlDataPtr((ushort)(1 << index));
            if (btlDataPtr != null)
                num = (int)btl_scrp.GetCharacterData(btlDataPtr, (uint)kind);
        }
        return num;
    }

    private void SetBattleCharData(Obj obj, int kind, int value)
    {
        int index = 0;
        while (index < 8 && this._objPtrList[index] != obj)
            ++index;
        if (index >= 8)
            return;
        BTL_DATA btl = kind != 32 ? btl_scrp.GetBtlDataPtr((ushort)(1 << index)) : btl_scrp.GetBtlDataPtrUnlimited((ushort)(1 << index));
        if (btl == null)
            return;
        btl_scrp.SetCharacterData(btl, (uint)kind, (uint)value);
    }

    private int getNumOfObjsInObjList(ObjList list)
    {
        int num = 0;
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

    public int GetIP(int objID, int tagID, byte[] eventData)
    {
        if (eventData.Length == 0)
            return this.nil;
        using (MemoryStream memoryStream = new MemoryStream(eventData))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                binaryReader.ReadByte();
                byte num1 = binaryReader.ReadByte();
                ushort num2 = 0;
                int num3;
                for (num3 = num1; num3 > 0; --num3)
                {
                    ushort num4 = (ushort)binaryReader.ReadInt16();
                    num2 = (ushort)binaryReader.ReadInt16();
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

    public Obj FindObjByUID(int uid)
    {
        ObjList objList = this._context.activeObj;
        while (objList != null && objList.obj.uid != uid)
            objList = objList.next;

        return objList?.obj;
    }

    public bool objIsVisible(Obj obj)
    {
        if (obj.state == EventEngine.stateRunning)
            return (obj.flags & 1) != 0;
        return false;
    }

    public void putvobj(Obj obj, int type, int v)
    {
        if (obj == null)
            return;
        if (type == 2)
        {
            short fixedPointAngle = (short)(v << 4);
            ((PosObj)obj).rotAngle[1] = EventEngineUtils.ConvertFixedPointAngleToDegree(fixedPointAngle);
        }
        else
            this.SetBattleCharData(obj, type, v);
    }

    public int BGI_systemSetAttributeMask(byte mask)
    {
        this.fieldmap.bgi.attributeMask = mask;
        return 1;
    }

    private float dist64(float deltaX, float deltaY, float deltaZ)
    {
        return (float)(deltaX * (double)deltaX + deltaY * (double)deltaY + deltaZ * (double)deltaZ);
    }

    private float disdif64(float deltaX, float deltaZ, float deltaR)
    {
        return (float)(deltaX * (double)deltaX + deltaZ * (double)deltaZ + deltaR * (double)deltaR);
    }

    private float distance(float deltaX, float deltaY, float deltaZ)
    {
        return Mathf.Sqrt((float)(deltaX * (double)deltaX + deltaY * (double)deltaY + deltaZ * (double)deltaZ));
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
            int startID1 = this.getspw(obj, obj.sx) - 4;
            int intFromBuffer = obj.getIntFromBuffer(startID1);
            if (obj.uid == 0 && obj.level == 0)
                this.ExitBattleEnd();
            int startID2 = startID1 - 4;
            obj.ip = obj.getIntFromBuffer(startID2);
            obj.sx -= 2;
            obj.wait = (byte)(intFromBuffer & byte.MaxValue);
            obj.level = (byte)(intFromBuffer >> 8 & byte.MaxValue);
            if ((intFromBuffer >> 16 & byte.MaxValue) != byte.MaxValue)
            {
                Obj objByUid = this.FindObjByUID(intFromBuffer >> 24 & byte.MaxValue);
                if (objByUid != null)
                {
                    if (objByUid.wait == byte.MaxValue)
                    {
                        objByUid.wait = 0;
                    }
                    else
                    {
                        int num1 = this.getspw(objByUid, 0);
                        int startID3 = this.getspw(objByUid, objByUid.sx - 1);
                        while (startID3 > num1 && (objByUid.getByteFromBuffer(startID3) & byte.MaxValue) != byte.MaxValue)
                            startID3 -= 8;
                        if (startID3 > num1)
                        {
                            byte num2 = (byte)(objByUid.getByteFromBuffer(startID3) & 4294967040U);
                            objByUid.setByteToBuffer(startID3, num2);
                        }
                    }
                }
            }
        }
        else if (obj.state == EventEngine.stateInit)
        {
            obj.state = EventEngine.stateRunning;
            obj.ip = this.GetIP(obj.sid, 1, obj.ebData);
            obj.level = (byte)(EventEngine.cEventLevelN - 1);
        }
        else
            obj.ip = this.nil;
        if (!obj.isAdditionCommand)
            return;
        obj.RetunCall();
    }

    public void SetFollow(Obj obj, int winnum, int flags)
    {
        if ((flags & 160) != 128 || !this.isPosObj(obj))
            return;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj1 = objList.obj;
            if (this.isPosObj(obj1) && ((PosObj)obj1).follow == winnum)
                ((PosObj)obj1).follow = byte.MaxValue;
        }
        ((PosObj)obj).follow = (byte)winnum;
        EventEngine.sLastTalker = (PosObj)obj;
        EventEngine.sTalkTimer = 0;
    }

    public void SetNextMap(int MapNo)
    {
        this.FF9ChangeMap(MapNo);
    }

    private void SetBattleScene(int SceneNo)
    {
        this.FF9ChangeMap(SceneNo);
    }

    private void FF9ChangeMap(int MapNo)
    {
        FF9StateFieldSystem stateFieldSystem = FF9StateSystem.Field.FF9Field;
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        FF9StateWorldSystem stateWorldSystem = FF9StateSystem.World.FF9World;
        switch (this._ff9Sys.mode)
        {
            case 1:
                stateFieldSystem.loc.map.nextMapNo = (short)MapNo;
                break;
            case 2:
                stateBattleSystem.map.nextMapNo = (short)MapNo;
                break;
            case 3:
                stateWorldSystem.map.nextMapNo = (short)MapNo;
                break;
        }
    }

    private int geti()
    {
        int num = this.gExec.getByteIP();
        ++this.gExec.ip;
        return num;
    }

    private int getv1()
    {
        int num;
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

    private int getv2()
    {
        int num;
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

    private int getv3()
    {
        int num;
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

    private void ExecAnim(Actor p, int anim)
    {
        if ((p.animFlag & EventEngine.afExec) != 0 && (p.flags & 128) != 0)
            this.FinishTurn(p);
        p.anim = (ushort)anim;
        p.animFrame = p.inFrame;
        byte num1 = (byte)~(EventEngine.afDir | EventEngine.afLower | EventEngine.afFreeze);
        p.animFlag &= num1;
        byte num2 = p.inFrame <= p.outFrame ? (byte)EventEngine.afExec : (byte)(EventEngine.afExec | EventEngine.afDir);
        p.animFlag |= num2;
        p.frameDif = 0;
        p.frameN = (byte)EventEngineUtils.GetCharAnimFrame(p.go, anim);
        p.aspeed = p.aspeed0;
        if (p.uid != this._context.controlUID)
            return;
        ++this.gAnimCount;
    }

    public bool GetUserControl()
    {
        return this._context?.usercontrol == 1;
    }

    public void SetUserControl(bool isEnable)
    {
        if (this._context == null)
            return;
        this._context.usercontrol = !isEnable ? (byte)0 : (byte)1;
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
                ((PosObj)obj).charFlags = (short)component.charFlags;
                ((PosObj)obj).activeTri = (short)component.activeTri;
                ((PosObj)obj).activeFloor = (byte)component.activeFloor;
                ((PosObj)obj).bgiRad = (byte)(component.radius / 4.0);
                this.fieldmap.isBattleBackupPos = true;
            }
        }
    }

    public int GetDashInh()
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
        public short FieldMapNo;
    }
}