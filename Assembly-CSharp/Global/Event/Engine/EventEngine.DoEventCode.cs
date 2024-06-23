using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria.Field;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Memoria;
using Memoria.Prime;
using Memoria.Assets;
using Memoria.Data;
using UnityEngine;

public partial class EventEngine
{
    public Int32 DoEventCode()
    {
        Actor actor = (Actor)null;
        GameObject gameObject = (GameObject)null;
        PosObj po = (PosObj)null;
        FieldMapActorController fmac = (FieldMapActorController)null;
        Int16 mapNo = FF9StateSystem.Common.FF9.fldMapNo;
        Int32 scCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
        Int32 mapIndex = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
        if (this.gCur.cid == 4)
        {
            actor = (Actor)this.gCur;
            gameObject = actor.go;
            po = (PosObj)this.gCur;
        }
        this._lastIP = this.gExec.ip;
        Int32 codeByte = this.geti();
        Int32 code = 0;
        while (codeByte == 0xFF)
        {
            codeByte = this.geti();
            code += 0x100;
        }
        code += codeByte;
        this.gArgFlag = this.geti();
        this.gArgUsed = 0;
        EBin.event_code_binary eventCodeBinary = (EBin.event_code_binary)code;
        Vector3 eulerAngles1;

        switch (eventCodeBinary)
        {
            case EBin.event_code_binary.NOP: // 0x00, "NOTHING", "Do nothing."
            {
                return 1;
            }
            // 0x02, "JMP_IFN", "Skip some operations if the stack value is not 0.WARNING: unsafe to use.", false, 1, { 2 }, { "Jump" }, { AT_JUMP }, 3
            // 0x03, "JMP_IF", "Skip some operations if the stack value is 0.WARNING: unsafe to use.", false, 1, { 2 }, { "Jump" }, { AT_JUMP }, 3
            // 0x04, "return", "End the function."
            // 0x05, "set", "Perform variable operations and store the result in the stack.", false, 1, { 0 }, { "Variable Code" }, { AT_NONE }, 0
            // 0x06, "JMP_SWITCHEX", "Skip some operations depending on the stack value.WARNING: unsafe to use.", false, -1, { 2, 2, 2 }, { "Default Jump", "Case", "Jump" }, { AT_JUMP, AT_SPIN, AT_JUMP }, 0
            // Unknown / unused opcodes:
            case EBin.event_code_binary.DEBUGCC:
            case EBin.event_code_binary.GLOBALCLEAR:
            case EBin.event_code_binary.DEBUGSAVE:
            case EBin.event_code_binary.DEBUGLOAD:
            {
                return 0;
            }
            case EBin.event_code_binary.BSSTART: //  0x100, "0x100", "Unknown Opcode (BSSTART).", true, 2, { 1, 1 }, { "Unknown", "Unknown" }, { AT_SPIN, AT_SPIN }, 0
            case EBin.event_code_binary.BSFRAME: //  0x101, "0x101", "Unknown Opcode (BSFRAME.", true, 2, { 1, 1 }, { "Unknown", "Unknown" }, { AT_SPIN, AT_SPIN }, 0
            case EBin.event_code_binary.BAANIME:
            case EBin.event_code_binary.BAACTIVE:
            case EBin.event_code_binary.BAFLAG:
            case EBin.event_code_binary.BAWAITALL:
            case EBin.event_code_binary.BAVISIBLE:
            {
                this.getv1();
                this.getv1();
                return 0;
            }
            case EBin.event_code_binary.BARATE:
            {
                this.getv1();
                this.getv2();
                return 0;
            }
            case EBin.event_code_binary.BAWAIT:
            case EBin.event_code_binary.BARANGE:
            {
                this.getv1();
                this.getv1();
                this.getv1();
                return 0;
            }
            case EBin.event_code_binary.BGSMOVE:
            {
                this.getv2();
                this.getv2();
                this.getv2();
                return 0;
            }
            case EBin.event_code_binary.NEW: // 0x07, "InitCode", "Init a normal code (independant functions).arg1: code entry to init.arg2: Unique ID (defaulted to entry's ID if 0).", false, 2, { 1, 1 }, { "Entry", "UID" }, { AT_ENTRY, AT_USPIN }, 0
            {
                NewThread(this.gArgFlag, this.geti()); // arg1: code entry to init / arg2: Unique ID (defaulted to entry's ID if 0)
                this.gArgUsed = 1;
                return 0;
            }
            case EBin.event_code_binary.NEW2: // 0x08, "InitRegion", "Init a region code (associated with a region).arg1: code entry to init.arg2: Unique ID (defaulted to entry's ID if 0).", false, 2, { 1, 1 }, { "Entry", "UID" }, { AT_ENTRY, AT_USPIN }, 0
            {
                Quad quad = new Quad(this.gArgFlag, this.geti()); // arg1: code entry to init / arg2: Unique ID (defaulted to entry's ID if 0)
                this.gArgUsed = 1;
                return 0;
            }
            case EBin.event_code_binary.NEW3: // 0x09, "InitObject", "Init an object code (associated with a 3D model). Also load the related model into the RAM.arg1: code entry to init.arg2: Unique ID (defaulted to entry's ID if 0).", false, 2, { 1, 1 }, { "Entry", "UID" }, { AT_ENTRY, AT_USPIN }, 0
            {
                Int32 sid = this.gArgFlag; // arg1: code entry to init
                Int32 uid = this.geti(); // arg2: Unique ID (defaulted to entry's ID if 0)
                if (sid >= 251 && sid < Byte.MaxValue)
                {
                    sid = this._context.partyUID[sid - 251];
                    if (sid == Byte.MaxValue)
                    {
                        Log.Warning($"[EventEnginge] Failed to perform an event code [NEW3] because there is no party member with index {this.gArgFlag}");
                        return 0;
                    }
                }
                actor = new Actor(sid, uid, EventEngine.sizeOfActor);
                if (this.gMode == 3)
                    Singleton<WMWorld>.Instance.addWMActorOnly(actor);
                if (this.gMode == 1)
                    this.turnOffTriManually(sid);
                this.gArgUsed = 1;
                return 0;
            }
            // 0x0B, "JMP_SWITCH", "Skip some operations depending on the stack value.WARNING: unsafe to use.", false, -1, { 2, 2, 2 }, { "Starting Value", "Default Jump", "Jump" }, { AT_USPIN, AT_JUMP, AT_JUMP }, 1
            // 0x0C, "0x0C", "Unknown Opcode.", 
            // 0x0D, "0x0D", "Unknown Opcode." - Steam seems to handle it like a JMP_SWITCH with a short instead of a char (number of cases)
            case EBin.event_code_binary.REQ: // 0x10, "RunScriptAsync", "Run script function and continue executing the current one.Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not executed. Otherwise, the entry's script level is set to the specified script level until the function returns", true, 3, { 1, 1, 1 }, { "Script Leve", "Entry", "Function" }, { AT_SCRIPTLVL, AT_ENTRY, AT_FUNCTION }, 0
            {
                Int32 scriptLevel = this.getv1(); // arg1: script level
                Obj obj1 = this.GetObj1(); // arg2: entry of the function
                Int32 tagNumber = this.geti(); // arg3: function
                this.Request(obj1, scriptLevel, tagNumber, false);
                if (mapNo == 900 && obj1 != null && scriptLevel == 2 && tagNumber == 11 && obj1.uid == 14)
                    this.fieldmap.walkMesh.BGI_triSetActive(62U, 1U); // Treno/Pub, function "Steiner_11"
                return 0;
            }
            case EBin.event_code_binary.REQSW: // 0x12, "RunScript", "Wait until the entry's script level gets higher than the specified script level then run the script function and continue executing the current one.Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not executed. Otherwise, the entry's script level is set to the specified script level until the function returns", true, 3, { 1, 1, 1 }, { "Script Leve", "Entry", "Function" }, { AT_SCRIPTLVL, AT_ENTRY, AT_FUNCTION }, 0
            {
                Int32 scriptLevel = this.getv1(); // arg1: script level
                Obj obj1 = this.GetObj1(); // arg2: entry of the function
                Int32 tagNumber = this.geti(); // arg3: function
                if (mapNo == 2803 && obj1 != null && tagNumber == 18 && obj1.uid == 20)
                {
                    // Daguerreo/2nd Floor, LibrarianB, moving after Zidane says where is the book he searches
                    this.fieldmap.walkMesh.BGI_triSetActive(105U, 1U);
                    this.fieldmap.walkMesh.BGI_triSetActive(106U, 1U);
                }
                if (this.requestAcceptable(obj1, scriptLevel))
                {
                    this.Request(obj1, scriptLevel, tagNumber, false);
                    if (mapNo == 262) // Evil Forest/Exit
                    {
                        this._geoTexAnim = this.GetObjUID(12).go.GetComponent<GeoTexAnim>();
                        if (obj1.sid == 10 && tagNumber == 12) // Zidane, yawning in front of tent
                        {
                            this._geoTexAnim.geoTexAnimStop(2);
                            this._geoTexAnim.geoTexAnimPlay(0);
                        }
                        else if (obj1.sid == 12 && tagNumber == 20) // Dagger, awakening
                        {
                            this._geoTexAnim.geoTexAnimPlay(2);
                        }
                    }
                    return 0;
                }
                this.stay();
                return 1;
            }
            case EBin.event_code_binary.REQEW: // 0x14, "RunScriptSync", "Wait until the entry's script level gets higher than the specified script level then run the script function and wait until it returns. Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not executed. Otherwise, the entry's script level is set to the specified script level until the function returns", true, 3, { 1, 1, 1 }, { "Script Leve", "Entry", "Function" }, { AT_SCRIPTLVL, AT_ENTRY, AT_FUNCTION }, 0
            {
                Int32 scriptLevel = this.getv1(); // arg1: script level
                Obj obj1 = this.GetObj1(); // arg2: entry of the function
                Int32 tagNumber = this.geti(); // arg3: function
                if (this.gMode == 3)
                {
                    if (tagNumber == 19)
                    {
                        Singleton<WMTweaker>.Instance.FixTypeCamEyeYTarget = 1419;
                        Singleton<WMTweaker>.Instance.FixTypeCamAimYTarget = 996;
                    }
                    else if (tagNumber == 21)
                    {
                        Singleton<WMTweaker>.Instance.FixTypeCamEyeYTarget = 558;
                        Singleton<WMTweaker>.Instance.FixTypeCamAimYTarget = 312;
                    }
                }
                if (this.requestAcceptable(obj1, scriptLevel))
                    this.Request(obj1, scriptLevel, tagNumber, true);
                else
                    this.stay();
                // Hotfix: non-standard characters hacked in the team to Oeilvert -> pretend other member(s) were picked instead (field "Palace/Sanctum", start of function "Kuja_74")
                if (mapNo == 2209 && obj1 != null && obj1.sid == 4 && tagNumber == 74)
                {
                    Int32 oeilvertPartyCount = 0;
                    Queue<Int32> missingMemberVar = new Queue<Int32>();
                    for (Int32 i = 3536; i <= 3542; i++) // "VARL_GenBool_3536" etc.
                    {
                        if (eBin.GetVariableValueInternal(FF9StateSystem.EventState.gEventGlobal, i, EBin.VariableType.Bit, 0) == 0)
                            oeilvertPartyCount++;
                        else
                            missingMemberVar.Enqueue(i);
                    }
                    while (oeilvertPartyCount < 3 && missingMemberVar.Count > 0)
                    {
                        eBin.SetVariableValueInternal(FF9StateSystem.EventState.gEventGlobal, missingMemberVar.Dequeue(), EBin.VariableType.Bit, 0, 0);
                        oeilvertPartyCount++;
                    }
                }
                return 1;
            }
            case EBin.event_code_binary.REPLY: // 0x16, "RunScriptObjectAsync", "Run script function and continue executing the current one. Must only be used in response to a function call ; the argument's entry is the one that called this function.Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not executed. Otherwise, the entry's script level is set to the specified script level until the function returns", true, 2, { 1, 1 }, { "Script Leve", "Function" }, { AT_SCRIPTLVL, AT_FUNCTION }, 0
            {
                this.Request(this.getSender(this.gExec), this.getv1(), this.geti(), false); // arg1: script level / arg2: function
                return 0;
            }
            case EBin.event_code_binary.REPLYSW: // 0x18, "RunScriptObject", "Wait until the entry's script level gets higher than the specified script level then run the script function and continue executing the current one. Must only be used in response to a function call ; the argument's entry is the one that called this function.Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not executed. Otherwise, the entry's script level is set to the specified script level until the function returns", true, 2, { 1, 1 }, { "Script Leve", "Function" }, { AT_SCRIPTLVL, AT_FUNCTION }, 0
            {
                Int32 scriptLevel = this.getv1(); // arg1: script level
                Int32 tagNumber = this.geti(); // arg2: function
                if (this.requestAcceptable(this.getSender(this.gExec), scriptLevel))
                {
                    this.Request(this.getSender(this.gExec), scriptLevel, tagNumber, false);
                    return 0;
                }
                this.stay();
                return 1;
            }
            case EBin.event_code_binary.REPLYEW: // 0x1A, "RunScriptObjectSync", "Wait until the entry's script level gets higher than the specified script level then run the script function and wait until it returns. Must only be used in response to a function call ; the argument's entry is the one that called this function.Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not executed. Otherwise, the entry's script level is set to the specified script level until the function returns", true, 2, { 1, 1 }, { "Script Leve", "Function" }, { AT_SCRIPTLVL, AT_FUNCTION }, 0
            {
                Int32 scriptLevel = this.getv1(); // arg1: script level
                Int32 tagNumber = this.geti(); // arg2: function
                this.getSender(this.gExec);
                if (this.requestAcceptable(this.getSender(this.gExec), scriptLevel))
                    this.Request(this.getSender(this.gExec), scriptLevel, tagNumber, true);
                else
                    this.stay();
                return 1;
            }
            case EBin.event_code_binary.SONGFLAG: // 0x1B, "ContinueBattleMusic", "Continue the music after the battle end", true, 1, { 1 }, { "Continue" }, { AT_BOOL }, 0
            {
                if (this.getv1() != 0) // arg1: flag continue/ don't continue
                    FF9StateSystem.Common.FF9.btl_flag |= (Byte)8;
                else
                    FF9StateSystem.Common.FF9.btl_flag &= (Byte)247;
                return 0;
            }
            // 0x1C, "TerminateEntry", "Stop the execution of an entry's code.arg1: entry to terminate.", true, 1, { 1 }, { "Object" }, { AT_ENTRY }, 0
            case EBin.event_code_binary.POS: // 0x1D, "CreateObject", "Place (or replace) the 3D model on the field", true, 2, { 2, 2 }, { "Position" }, { AT_POSITION_X, AT_POSITION_Y }, 0
            case EBin.event_code_binary.DPOS: // 0xBF, "MoveInstantEx", "Instantatly move an object", true, 3, { 1, 2, 2 }, { "Object", "Destination" }, { AT_ENTRY, AT_POSITION_X, AT_POSITION_Y }, 0
            {
                if (eventCodeBinary == EBin.event_code_binary.DPOS)
                    po = (PosObj)this.GetObj1(); // arg1: object's entry
                Int32 posX = this.getv2(); // X position
                Int32 posZ = this.getv2(); // Z position
                Int32 isValid = this.gMode != 1 || (Int32)po.model == (Int32)UInt16.MaxValue ? 0 : 1;
                if (isValid == 1 && po != null)
                {
                    FieldMapActorController component = po.go.GetComponent<FieldMapActorController>();
                    if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.walkMesh != null)
                    {
                        component.walkMesh.BGI_charSetActive(component, 1U);
                        if (mapNo == 2050 && po.sid == 5)
                            component.walkMesh.BGI_charSetActive(component, 0U);
                        else if (mapNo == 2917 && po.sid == 4)
                        {
                            if (posX == 0 && posZ == -1787)
                                posX = -15;
                        }
                        else if (mapNo == 450 && po.sid == 3 && (posX == 363 && posZ == 88))
                            component.walkMesh.BGI_triSetActive(24U, 0U);
                    }
                    if (mapNo == 1421 && po.sid == 5)
                    {
                        if (posX == 1510 && (posZ == -2331 || posZ == -2231))
                        {
                            if (posZ == -2331)
                                posZ = -2231;
                            this.fieldmap.walkMesh.BGI_triSetActive(109U, 0U);
                            this.fieldmap.walkMesh.BGI_triSetActive(110U, 0U);
                        }
                        else if (posX == 34 && posZ == -598)
                        {
                            this.fieldmap.walkMesh.BGI_triSetActive(109U, 1U);
                            this.fieldmap.walkMesh.BGI_triSetActive(110U, 1U);
                        }
                    }
                    if (mapNo == 563 && po.sid == 16 && posX == -1614)
                        posX = -1635;
                    else if (mapNo == 572 && po.sid == 16 && posX == -1750)
                        posX = -1765;
                    else if (mapNo == 1310 && po.sid == 12 && posX == -1614)
                        posX = -1635;
                    else if (mapNo == 1811 && scCounter == 7200 && po.sid == 13 && posX == 413 && posZ == -17294) // Vivi visible too soon
                    {
                        posX = 300;
                        posZ = -17894; //Log.Message("posX:" + posX + " posZ:" + posZ);
                    }
                    else if (mapNo == 1816 && scCounter == 7200 && po.sid == 18 && posX == -2390 && posZ == 856) // Quiproquo at the dock, Beatrix visible too soon
                    {
                        posX = -2890; //Log.Message("posX:" + posX + " posZ:" + posZ);
                    }
                    else if (mapNo == 1901 && scCounter == 7550 && po.sid == 2 && posX == 142 && posZ == 2172) // Eiko ATE: Gilmanesh appears between 2 columns
                    {
                        posX = 400; //Log.Message("posX:" + posX + " posZ:" + posZ);
                    }
                    else if (mapNo == 2105 && scCounter == 8800 && po.sid == 14 && posX == -831 && posZ == 358) // Baku/Marcus visible too soon
                    {
                        posX = -1031;
                        posZ = 158; //Log.Message("posX:" + posX + " posZ:" + posZ + " po.sid:" + po.sid);
                    }
                    else if (mapNo == 2105 && scCounter == 8800 && po.sid == 25 && posX == -714 && posZ == -190) // Baku/Marcus visible too soon
                    {
                        posX = -914;
                        posZ = -390; //Log.Message("posX:" + posX + " posZ:" + posZ + " po.sid:" + po.sid);
                    }
                    else if (mapNo == 2110 && po.sid == 9 && posX == -1614)
                        posX = -1635;
                    else if (mapNo == 2173 && scCounter == 9050 && po.sid == 6 && posX == -1705 && posZ == 177) // Quina ashore ATE: guard seen too soon
                    {
                        posX = -2700;
                        posZ = 1400; //Log.Message("posX:" + posX + " posZ:" + posZ + " po.sid:" + po.sid);
                    }
                }
                this.SetActorPosition(po, (Single)posX, this.POS_COMMAND_DEFAULTY, (Single)posZ);
                if (mapNo == 2050 && (Int32)po.sid == 5 && (isValid == 1 && po != null))
                {
                    FieldMapActorController component = po.go.GetComponent<FieldMapActorController>();
                    if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.walkMesh != null)
                        component.walkMesh.BGI_charSetActive(component, 1U);
                }
                if ((Int32)po.cid == 4)
                    this.clrdist((Actor)po);
                this._posUsed = true;
                return 0;
            }
            case EBin.event_code_binary.BGVPORT: // 0x1E, "SetCameraBounds", "Redefine the field camera boundaries (default value is part of the background's data)", true, 5, { 1, 2, 2, 2, 2 }, { "Camera", "Min X", "Max X", "Min Y", "Max Y" }, { AT_USPIN, AT_SPIN, AT_SPIN, AT_SPIN, AT_SPIN }, 0
            {
                Int32 camId = this.getv1(); // arg1: camera ID
                Int16 minX = (Int16)this.getv2(); // arg2-5: MinX, MaxX, MinY, MaxY
                Int16 maxX = (Int16)this.getv2();
                Int16 minY = (Int16)this.getv2();
                Int16 maxY = (Int16)this.getv2();

                if (mapNo == 2220 && minX == 154 && FieldMap.ActualPsxScreenWidth > 390) // Desert Palace fix to widescreen cam seeing hidden path too soon
                    minX = (Int16)544;

                this.fieldmap.EBG_cameraSetViewport(camId, minX, maxX, minY, maxY);
                return 0;
            }
            case EBin.event_code_binary.MES: // 0x1F, "WindowSync", "Display a window with text inside and wait until it closes", true, 3, { 1, 1, 2 }, { "Window ID", "UI", "Text" }, { AT_USPIN, AT_BOOLLIST, AT_TEXT }, 0
            {
                this.gCur.winnum = (Byte)this.getv1(); // arg1: window ID
                Int32 uiFlags = this.getv1(); // arg2: UI flag list. 3: disable bubble tail 4: mognet format 5: hide window 7: ATE window 8: dialog window
                this.SetFollow(this.gCur, (Int32)this.gCur.winnum, uiFlags);
                Int32 textID = this.getv2(); // arg3: text to display
                if (mapNo == 1757 && scCounter == 6740 && mapIndex == 30)
                {
                    this.stay();
                    return 1;
                }
                if (mapNo == 1060)
                {
                    String symbol = Localization.GetSymbol();
                    Dictionary<Int32, Int32> dictionary = (Dictionary<Int32, Int32>)null;
                    if (symbol == "JP")
                    {
                        if (textID == 271)
                        {
                            HonoBehaviorSystem.FrameSkipEnabled = true;
                            HonoBehaviorSystem.TargetFrameTime = 0.03333334f;
                        }
                        else if (textID == 272)
                            HonoBehaviorSystem.FrameSkipEnabled = false;
                    }
                    else if (textID == 262)
                    {
                        HonoBehaviorSystem.TargetFrameTime = 0.03333334f;
                        HonoBehaviorSystem.FrameSkipEnabled = true;
                    }
                    else if (textID == 264)
                        HonoBehaviorSystem.FrameSkipEnabled = false;
                    if (symbol == "ES" || symbol == "FR")
                        dictionary = this._mesIdES_FR;
                    else if (symbol == "GR")
                        dictionary = this._mesIdGR;
                    else if (symbol == "IT")
                        dictionary = this._mesIdIT;
                    if (dictionary != null && dictionary.ContainsKey(textID))
                        textID = dictionary[textID];
                }
                if (mapNo == 2172 && scCounter < 9100 && Localization.GetSymbol() == "JP" && textID == 91 && this.gCur.sid == 1 && this.gCur.ip == 145 && EIcon.AIconMode == 0)
                {
                    DialogManager.SelectChoice = 15;
                    return 0;
                }
                this.eTb.NewMesWin(textID, (Int32)this.gCur.winnum, uiFlags, !this.isPosObj(this.gCur) ? (PosObj)null : (PosObj)this.gCur);
                this.gCur.wait = (Byte)254;
                return 1;
            }
            case EBin.event_code_binary.MESN: // 0x20, "WindowAsync", "Display a window with text inside and continue the execution of the script without waiting", true, 3, { 1, 1, 2 }, { "Window ID", "UI", "Text" }, { AT_USPIN, AT_BOOLLIST, AT_TEXT }, 0
            {
                this.gCur.winnum = (Byte)this.getv1(); // arg1: window ID
                Int32 uiFlags = this.getv1(); // arg2: UI flag list: 3: disable bubble tail 4: mognet format 5: hide window 7: ATE window 8: dialog window
                this.SetFollow(this.gCur, (Int32)this.gCur.winnum, uiFlags);
                Int32 textID = this.getv2(); // arg3: text to display
                if (mapNo == 1060)
                {
                    String symbol = Localization.GetSymbol();
                    Dictionary<Int32, Int32> dictionary = (Dictionary<Int32, Int32>)null;
                    if (symbol == "JP")
                    {
                        if (textID == 271)
                        {
                            HonoBehaviorSystem.TargetFrameTime = 0.03333334f;
                            HonoBehaviorSystem.FrameSkipEnabled = true;
                        }
                        else if (textID == 272)
                            HonoBehaviorSystem.FrameSkipEnabled = false;
                    }
                    else if (textID == 262)
                    {
                        HonoBehaviorSystem.TargetFrameTime = 0.03333334f;
                        HonoBehaviorSystem.FrameSkipEnabled = true;
                    }
                    else if (textID == 264)
                        HonoBehaviorSystem.FrameSkipEnabled = false;
                    if (symbol == "ES" || symbol == "FR")
                        dictionary = this._mesIdES_FR;
                    else if (symbol == "GR")
                        dictionary = this._mesIdGR;
                    else if (symbol == "IT")
                        dictionary = this._mesIdIT;
                    if (dictionary != null && dictionary.ContainsKey(textID))
                        textID = dictionary[textID];
                }
                if (mapNo == 1757 && scCounter == 6740 && mapIndex == 30)
                {
                    this.stay();
                    return 1;
                }
                PersistenSingleton<CheatingManager>.Instance.CheatJumpingRobe();
                this.eTb.NewMesWin(textID, (Int32)this.gCur.winnum, uiFlags, !this.isPosObj(this.gCur) ? (PosObj)null : (PosObj)this.gCur);
                return 0;
            }
            case EBin.event_code_binary.CLOSE: // 0x21, "CloseWindow", "Close a window", true, 1, { 1 }, { "Window ID" }, { AT_USPIN }, 0
            {
                // Do not handle stage dialogs specifically
                // - When the "IsAlexandriaStageScene" block is disabled, the dialogs concerned automatically close after the time defined in their [TIME=X] text opcode
                // - When the "IsAlexandriaStageScene" block is enabled, these dialogs must be closed by a player input but only after [TIME=X] has passed
                //if (IsAlexandriaStageScene())
                //{
                //    DialogManager.Instance.ForceControlByEvent(false);
                //    goto case EBin.event_code_binary.WAITMES;
                //}

                var windowID = this.getv1(); // arg1: window ID determined at its creation
                this.eTb.DisposWindowByID(windowID, true);
                return 0;
            }
            // 0x22, "Wait", "Wait some time.arg1: amount of frames to wait. For PAL, 1 frame is 0.04 seconds. For other versions, 1 frame is about 0.033 seconds.", true, 1, { 1 }, { "Frame Amount" }, { AT_USPIN }, 0
            case EBin.event_code_binary.MOVE: // 0x23, "Walk", "Make the character walk to destination. Make it synchronous if InitWalk is called before", true, 2, { 2, 2 }, { "Destination" }, { AT_POSITION_X, AT_POSITION_Y }, 0
            {
                Int32 destX = this.getv2(); // arg1: destination X
                Int32 destZ = this.getv2(); // arg2: destination Z
                Boolean flag = false;
                if (mapNo == 53 && destX == 250 && destZ == 1200) // fix for scene where Blank jumps in hole, position adjustment
                {
                    destX = 330;
                }
                else if (mapNo == 64 && scCounter == 1600 && mapIndex == 327 && destX == 1111 && destZ == -14400)
                {
                    destX = 1261;
                    destZ = -14550;
                }
                else if (mapNo == 101)
                {
                    if (po.sid == 7 && destX == -4000 && destZ == -400 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)destX, (Single)destZ)).sqrMagnitude > 100.0)
                        flag = true;
                    if (po.sid == 8 && destX == -4000 && destZ == -200 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)destX, (Single)destZ)).sqrMagnitude > 100.0)
                        flag = true;
                }
                else if (mapNo == 108 && po.sid == 2 && destX == -111 && destZ == -210)
                {
                    destX = -150;
                    destZ = -270;
                }
                else if (mapNo == 307 && scCounter == 2520 && (po.sid == 12 || po.sid == 14) && destX == 636 && destZ == -1359)
                {
                    destX = 781;
                }
                else if (mapNo == 401 && po.sid == 2 && destX == -1869 && destZ == -783)
                {
                    destX = -1782;
                }
                else if (mapNo == 550)
                {
                    if (po.sid == 14 && destX == 348 && destZ == -2500 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)destX, (Single)destZ)).sqrMagnitude > 100.0)
                        flag = true;
                }
                else if (mapNo == 563 && po.sid == 16 && destX == -1614)
                {
                    destX = -1635;
                }
                else if (mapNo == 572 && po.sid == 16 && destX == -1750)
                {
                    destX = -1765;
                }
                else if (mapNo == 915 || mapNo == 1915)
                {
                    if (po.sid == 3)
                    {
                        if (destX == -10336 && destZ == -7750)
                        {
                            if ((Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)destX, (Single)destZ)).sqrMagnitude > 100.0)
                                flag = true;
                        }
                        else if (destX == -8746 && destZ == -6516 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)destX, (Single)destZ)).sqrMagnitude > 100.0)
                            flag = true;
                    }
                    else if (po.sid == 4)
                    {
                        if (mapNo == 915 && destX == -8746 && destZ == -6516)
                        {
                            if ((Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)destX, (Single)destZ)).sqrMagnitude > 100.0)
                                flag = true;
                        }
                        if (destX == -8967 && destZ == -6173)
                        {
                            if ((Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)destX, (Single)destZ)).sqrMagnitude > 100.0)
                                flag = true;
                        }
                        else if (destX == -10498 && destZ == 2678 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)destX, (Single)destZ)).sqrMagnitude > 100.0)
                            flag = true;
                    }
                }
                else if (mapNo == 1310 && po.sid == 12 && destX == -1614)
                {
                    destX = -1635;
                }
                else if (mapNo == 1601 && po.uid == 130 && destX == -1705 && destZ == -1233)
                {
                    destX = -1680;
                    destZ = -1140;
                }
                else if (mapNo == 1800 && scCounter == 10100 && mapIndex == 1 && actor.sid == 11 && destX == 510 && destZ == 3054)
                {
                    destZ = 2970;
                }
                else if (mapNo == 1815 && scCounter == 7070 && destX == -2312 && destZ == -245) // Steiner visible on dock in widescreen
                {
                    destX = -3000;
                }
                else if (mapNo == 1857 && po.sid == 3 && destX == -111 && destZ == -210)
                {
                    destX = -150;
                    destZ = -270;
                }
                else if (mapNo == 1861 && po.sid == 24 && destX == 137 && destZ == 420) // Eiko in pub: not going far enough.
                {
                    destX = -550;
                    destZ = 800; //Log.Message("destX = " + destX + " destZ = " + destZ);
                }
                else if (mapNo == 1901 && (po.sid == 2 || po.sid == 17) && destX == 672) // Eiko ATE widescreen: Quina chasing Gilmanesh not far enough
                {
                    destX = 1000; //Log.Message("destX = " + destX + " destZ = " + destZ + " po.sid = " + po.sid + " po.pos[0] = " + po.pos[0]);
                    if ((po.sid == 2 && po.pos[0] > 890) || (po.sid == 17 && po.pos[0] > 810)) 
                        this.gCur.flags = (Byte)((this.gCur.flags & -64) | 14); // make invisible, because they're still visible even at the edge
                }
                else if (mapNo == 2101 && po.sid == 2 && destX == 781 && destZ == 1587)
                {
                    destX = 805;
                    destZ = 1564;
                }
                else if (mapNo == 2110 && po.sid == 9 && destX == -1614)
                {
                    destX = -1635;
                }
                else if (mapNo == 2173 && scCounter == 9050 && (po.sid == 4 || po.sid == 5 || po.sid == 6) && destX == -2612 && destZ == 1558)  // Quina ashore ATE: guard not quitting the scene
                {
                    destX = -3900;
                    destZ = 2558; //Log.Message("destX = " + destX + " destZ = " + destZ + " po.sid = " + po.sid + " po.pos[0] = " + po.pos[0]);
                    if (po.pos[0] < -3500)
                        this.gCur.flags = (Byte)((this.gCur.flags & -64) | 14);
                }
                else if (mapNo == 2800 && po.sid == 17) // TODO Check Native: #147
                {
                    destX = -4702;
                    destZ = 2702;
                }
                else if (mapNo == 2954 && po.sid == 4 && destX == -1159 && destZ == 13130)
                {
                    SettingUtils.FieldMapSettings fieldMapSettings = SettingUtils.fieldMapSettings;
                    destX = -1275;
                    destZ = 13130;
                }
                else if (mapNo == 2954 && po.sid == 4 && destX == -3644 && destZ == 11849)
                {
                    SettingUtils.FieldMapSettings fieldMapSettings = SettingUtils.fieldMapSettings;
                    destX = -3750;
                    destZ = 11849;
                }
                Boolean flag2 = this.MoveToward_mixed((Single)destX, 0.0f, (Single)destZ, 0, (PosObj)null);
                eulerAngles1 = po.go.transform.localRotation.eulerAngles;
                if (flag || flag2)
                    this.stay();
                return 1;
            }
            case EBin.event_code_binary.MOVA: // 0x24, "WalkTowardObject", "Make the character walk and follow an object", true, 1, { 1 }, { "Object" }, { AT_ENTRY }, 0
            {
                po = (PosObj)this.GetObj1(); // arg1: object to walk toward
                if (this.MoveToward_mixed(po.pos[0], po.pos[1], po.pos[2], 0, po))
                    this.stay();
                this.gArgUsed = 1;
                return 1;
            }
            case EBin.event_code_binary.MOVE_EX: // "WalkEx" Make the specified character walk to destination
            {
                actor = this.GetObj3() as Actor; // object to move
                Int32 speed = this.getv3(); // walk speed
                Single x = this.getv3(); // 3rd to 5th arguments: position in (X, Z, Y) format.
                Single y = -this.getv3();
                Single z = this.getv3();
                Int32 flags = this.getv3(); // movement flags
                if (actor == null)
                    return 0;
                if (actor.loopCount != Byte.MaxValue)
                {
                    actor.loopCount = Byte.MaxValue;
                    this.clrdist(actor);
                }
                if (this.gMode == 1 && (flags & 2) != 0)
                {
                    FieldMapActorController controller = actor.go?.GetComponent<FieldMapActorController>();
                    controller?.walkMesh.BGI_charSetActive(controller, 0);
                    ff9shadow.FF9ShadowOffField(actor.uid);
                    actor.isShadowOff = true;
                }
                if (this.MoveToward_mixed_ex(actor, speed, x, y, z, flags, null))
                {
                    this.stay();
                }
                else
                {
                    if (this.gMode == 1 && (flags & 2) != 0)
                    {
                        FieldMapActorController controller = actor.go?.GetComponent<FieldMapActorController>();
                        if (controller != null && controller.walkMesh.BGI_nearestWalkPosInVertical(new Vector3(x, y, z), out Single h) && Math.Abs(h) < 100f)
                        {
                            // Enable walkmesh pathing and shadow if the destination is near the ground
                            controller.walkMesh.BGI_charSetActive(controller, 1);
                            ff9shadow.FF9ShadowOnField(actor.uid);
                        }
                    }
                }
                return 1;
            }
            case EBin.event_code_binary.MOVE3: // 0xA2, "WalkXZY", "Make the character walk to destination. Make it synchronous if InitWalk is called before", true, 3, { 2, 2, 2 }, { "Destination" }, { AT_POSITION_X, AT_POSITION_Z, AT_POSITION_Y }, 0
            {
                Single x = (Single)this.getv2(); // 3rd to 5th arguments: position in (X, Y, Z) format.
                Single y = -(Single)(this.getv2());
                Single z = (Single)this.getv2();

                if (mapNo == 1550 && ((po.sid == 18 && x == 1798f) || (po.sid == 2 && x == 1797f))) // Fix widescreen - Quina runs to eat Mog
                    x = 2800f;

                if (this.MoveToward_mixed(x, y, z, 2, (PosObj)null)) // arg1: destination in (X, Z, Y)
                    this.stay();
                return 1;
            }
            case EBin.event_code_binary.MOVQ: // 0x9E, "ExitField", "Make the player's character walk to the field exit and prepare to flush the field datas."
            {
                po = (PosObj)this.FindObjByUID((Int32)this._context.controlUID);
                if (po != null)
                {
                    this.Call((Obj)po, 0, 0, false, Obj.movQData);
                    this._context.usercontrol = (Byte)0;
                    for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
                        objList.obj.flags |= (Byte)6;
                }
                return 0;
            }
            case EBin.event_code_binary.MOVJ: // 0xA0, "WalkToExit", "Make the entry's object walk to the field exit."
            {
                if (this.MoveToward_mixed((Single)this.sMapJumpX, 0.0f, (Single)this.sMapJumpZ, 0, (PosObj)null))
                    this.stay();
                return 1;
            }
            case EBin.event_code_binary.MOVH: // 0xA5, "Slide", "Make the character slide to destination (walk without using the walk animation and without changing the facing angle)", true, 2, { 2, 2 }, { "Destination" }, { AT_POSITION_X, AT_POSITION_Y }, 0
            {
                Int32 destX = this.getv2(); // 1st and arg2s: destination in (X, Z) format.
                Int32 destZ = this.getv2();
                if (mapNo == 66 && po.sid == 14 && destX == -145 && destZ == -9135)
                {
                    // Prima Vista/Interior, Marcus attacking King Leo as Cornelia (Garnet) moves to protect him
                    destX = -160;
                    destZ = -9080;
                }
                if (this.MoveToward_mixed(destX, 0.0f, destZ, 1, null))
                    this.stay();
                return 1;
            }
            case EBin.event_code_binary.MOVE3H: // 0xE8, "SideWalkXZY", "Make the character walk to destination without changing his facing angle. Make it synchronous if InitWalk is called before. format.", true, 3, { 2, 2, 2 }, { "Destination" }, { AT_POSITION_X, AT_POSITION_Z, AT_POSITION_Y }, 0
            {
                if (this.MoveToward_mixed(this.getv2(), -this.getv2(), this.getv2(), 3, null)) // 1st to arg3s: destination in (X, Z, Y)
                    this.stay();
                return 1;
            }
            case EBin.event_code_binary.CLRDIST: // 0x25, "InitWalk", "Make a further Walk call (or variations of Walk) synchronous."
            {
                actor.loopCount = Byte.MaxValue;
                this.clrdist(actor);
                return 0;
            }
            case EBin.event_code_binary.MSPEED: // 0x26, "SetWalkSpeed", "Change the walk speed", true, 1, { 1 }, { "Speed" }, { AT_USPIN }, 0
            {
                Byte walkSpeed = (Byte)this.getv1(); // arg1: speed (surely in unit/frame)
                if (mapNo == 3010)
                {
                    String symbol = Localization.GetSymbol();
                    if (!(symbol != "US") || !(symbol != "JP"))
                    {
                        if (symbol == "US" && (Int32)actor.sid == 17)
                        {
                            if ((Int32)walkSpeed == 15)
                                walkSpeed = (Byte)20;
                            else if ((Int32)walkSpeed == 23)
                                walkSpeed = (Byte)25;
                        }
                        if (symbol == "JP" && (Int32)actor.sid == 16)
                        {
                            if ((Int32)walkSpeed == 15)
                                walkSpeed = (Byte)20;
                            else if ((Int32)walkSpeed == 23)
                                walkSpeed = (Byte)25;
                        }
                    }
                }
                if (mapNo == 658 && actor.sid == 18 && walkSpeed == 30 && actor.ip == 323)
                {
                    walkSpeed = 25;
                }
                actor.speed = walkSpeed;
                return 0;
            }
            case EBin.event_code_binary.BGIMASK: // 0x27, "SetTriangleFlagMask", "Set a bitmask for some of the walkmesh triangle flags, 7: disable restricted triangles, 8: disable player-restricted triangles", true, 1, { 1 }, { "Flags" }, { AT_BOOLLIST }, 0 // BGIMASK
            {
                this.BGI_systemSetAttributeMask((Byte)this.getv1()); // arg1: flag mask
                return 0;
            }
            case EBin.event_code_binary.FMV: // 0x28, "Cinematic", "Run or setup a cinematic", true, 4, { 1, 1, 1, 1 }, { "Unknown", "Cinematic ID", "Unknown", "Unknown" }, { AT_SPIN, AT_FMV, AT_SPIN, AT_SPIN }, 0
            {
                Singleton<fldfmv>.Instance.FF9FieldFMVDispatch(this.getv1(), this.getv2(), this.getv1()); // arg1: unknown, arg2: cinematic ID (may depends on arg1's value), arg3: unknown, (arg4: unknown)
                return 0;
            }
            case EBin.event_code_binary.QUAD: // 0x29, "SetRegion", "Define the polygonal region linked with the entry script. If the polygon is not convex, its convex hull is used instead. Args are in the format (Vertex X, Vertex Y) and there can be any number of them", true, -1, { 4 }, { "Polygon" }, { AT_NONE }, 0
            {
                Int32 i = 0;
                ((Quad)this.gCur).n = this.getv1();
                Int32 polyNum = ((Quad)this.gCur).n;
                while (polyNum != 0)
                {
                    QuadPos quadPos = ((Quad)this.gCur).q[i];
                    quadPos.X = (Int16)this.getv2();
                    quadPos.Z = (Int16)this.getv2();
                    if ((mapNo == 1608 || mapNo == 1707) && quadPos.Z == -257)
                        quadPos.Z = (short)(-157);
                    polyNum--;
                    i++;
                }
                //if ((Int32)this.gCur.sid != 4)
                //    ;
                return 0;
            }
            case EBin.event_code_binary.ENCOUNT: // 0x2A, "Battle", "Start a battle (using a random enemy group)", true, 2, { 1, 2 }, { "Rush Type", "Battle" }, { AT_SPIN, AT_BATTLE }, 0
            {
                this.getv1(); // arg1: rush type (unknown)
                this._ff9.btlSubMapNo = -1;
                Int32 btlId = this.getv2(); // arg2: gathered battle and Steiner's state (highest bit) informations
                this._ff9.steiner_state = (Byte)(btlId >> 15 & 1);
                this.SetBattleScene(btlId & Int16.MaxValue);
                FF9StateSystem.Battle.isRandomEncounter = false;
                this._encountBase = 0;
                return 3;
            }
            case EBin.event_code_binary.MAPJUMP: // 0x2B, "Field", "Change the field scene", true, 1, { 2 }, { "Field" }, { AT_FIELD }, 0
            {
                this.SetNextMap(this.getv2()); //arg1: field scene destination
                return 4;
            }
            case EBin.event_code_binary.CC: // 0x2C, "DefinePlayerCharacter", "Apply the player's control over the entry's object"
            {
                Actor activeActorByUid = this.getActiveActorByUID(this._context.controlUID);
                if (activeActorByUid != null && this.gMode == 1 && !(mapNo == 1607 && this.gExec.uid == activeActorByUid.uid))
                {
                    activeActorByUid.fieldMapActorController.isPlayer = false;
                    activeActorByUid.fieldMapActorController.gameObject.name = "obj" + activeActorByUid.uid;
                }
                if (this.gMode == 3 && this._context.controlUID != this.gExec.uid)
                    SmoothFrameUpdater_World.Skip = 1;
                this._context.controlUID = this.gExec.uid;
                return 0;
            }
            case EBin.event_code_binary.UCOFF: // 0x2D, "DisableMove", "Disable the player's movement control"
            {
                this._context.usercontrol = (Byte)0;
                EIcon.SetHereIcon(0);
                this.eTb.gMesCount = this.gAnimCount = 0;
                PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
                if (this.gMode == 3)
                    UIManager.World.SetMinimapPressable(false);
                else if (this.gMode == 1)
                {
                    Obj objUid250 = this.GetObjUID(250);
                    if (objUid250 != null && objUid250.cid == 4)
                        ((Actor)objUid250).fieldMapActorController.ClearMoveTargetAndPath();
                }
                if (!EMinigame.CheckChocoboVirtual())
                    PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (System.Action)null);
                return 0;
            }
            case EBin.event_code_binary.UCON: // 0x2E, "EnableMove", "Enable the player's movement control"
            {
                this._context.usercontrol = (Byte)1;
                EIcon.SetHereIcon(1);
                PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, (System.Action)null);
                this.ResetIdleTimer(0);
                if (this.gMode == 3)
                {
                    Singleton<BubbleUI>.Instance.UpdateWorldActor(this.GetControlChar());
                    if (!EventEngineUtils.IsMogCalled(this))
                        ff9.w_isMogActive = false;
                }
                PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(EventInput.IsMenuON && EventInput.IsMovementControl);
                if (this.gMode == 3)
                {
                    UIManager.World.SetMinimapPressable(EventInput.IsMovementControl);
                    UIManager.Input.ResetTriggerEvent();
                }
                return 0;
            }
            case EBin.event_code_binary.MODEL: // 0x2F, "SetModel", "Set the model of the object and its head's height (used to set the dialog box's height)", true, 2, { 2, 1 }, { "Mode", "Height" }, { AT_MODEL, AT_USPIN }, 0
            {
                po.model = (UInt16)this.getv2(); // arg1: model
                this.gExec.flags |= 1;
                po.eye = (Int16)(-4 * this.getv1()); // arg2: head's height
                if (this.gMode == 1)
                {
                    String str = FF9BattleDB.GEO.GetValue(po.model);

                    po.go = ModelFactory.CreateModel(str, false);
                    GeoTexAnim.addTexAnim(po.go, str);
                    if (ModelFactory.garnetShortHairTable.Contains(str))
                    {
                        po.garnet = true;
                        po.shortHair = FF9StateSystem.EventState.ScenarioCounter >= 10300;
                    }
                    if (po.go != null)
                    {
                        Int32 length = 0;
                        foreach (UnityEngine.Object child in po.go.transform)
                            if (child.name.Contains("mesh"))
                                ++length;

                        if (po.garnet)
                            ++length;

                        po.meshIsRendering = new Boolean[length];
                        for (Int32 i = 0; i < length; ++i)
                            po.meshIsRendering[i] = true;

                        FF9Char ff9Char = new FF9Char();
                        ff9Char.geo = po.go;
                        ff9Char.evt = po;
                        if (FF9StateSystem.Common.FF9.charArray.ContainsKey(po.uid))
                            return 0;
                        FF9StateSystem.Common.FF9.charArray.Add(po.uid, ff9Char);
                        FF9FieldCharState ff9FieldCharState = new FF9FieldCharState();
                        FF9StateSystem.Field.FF9Field.loc.map.charStateArray.Add(po.uid, ff9FieldCharState);
                        FF9StateSystem.Field.FF9Field.loc.map.shadowArray.Add(po.uid, new FF9Shadow());
                        po.go.name = "obj" + po.uid;
                        this.fieldmap.AddFieldChar(po.go, po.posField, po.rotField, false, (Actor)po, false);
                    }
                }
                else if (this.gMode == 3)
                {
                    po.go = ModelFactory.CreateModel(FF9BattleDB.GEO.GetValue(po.model), false);
                    Singleton<WMWorld>.Instance.addGameObjectToWMActor(po.go, ((Actor)po).wmActor);
                }

                if (mapNo == 112 && po.model == 223)
                    this.gCur.flags = (Byte)((this.gCur.flags & -64) | (Int32)14); // floating glass Alex pub: set flag 14 (invisible)
                return 0;
            }
            case EBin.event_code_binary.AIDLE: // 0x33, "SetStandAnimation", "Change the standing animation", true, 1, { 2 }, { "Animation" }, { AT_ANIMATION }, 0
            {
                actor.idle = (UInt16)this.getv2(); // arg1: animation ID
                AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.idle));
                if (mapNo == 2365 && actor.uid == 14 && actor.idle == 11611)
                {
                    this._geoTexAnim = actor.go.GetComponent<GeoTexAnim>();
                    this._geoTexAnim.geoTexAnimStop(2);
                    this._geoTexAnim.geoTexAnimPlay(0);
                }
                if (mapNo == 2657 && actor.uid == 7)
                {
                    this._geoTexAnim = actor.go.GetComponent<GeoTexAnim>();
                    if (actor.idle == 1044)
                    {
                        this._geoTexAnim.geoTexAnimStop(2);
                        this._geoTexAnim.geoTexAnimPlay(0);
                    }
                    else if (actor.idle == 816)
                        this._geoTexAnim.geoTexAnimPlay(2);
                }
                if (mapNo == 1605 && actor.uid == 18)
                {
                    this._geoTexAnim = actor.go.GetComponent<GeoTexAnim>();
                    if ((Int32)actor.idle == 7503)
                        this._geoTexAnim.geoTexAnimPlay(2);
                }
                return 0;
            }
            case EBin.event_code_binary.AWALK: // 0x34, "SetWalkAnimation", "Change the walking animation", true, 1, { 2 }, { "Animation" }, { AT_ANIMATION }, 0
            {
                actor.walk = (UInt16)this.getv2(); // arg1: animation ID
                AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.walk));
                return 0;
            }
            case EBin.event_code_binary.ARUN: // 0x34, "SetWalkAnimation", "Change the walking animation", true, 1, { 2 }, { "Animation" }, { AT_ANIMATION }, 0
            {
                actor.run = (UInt16)this.getv2(); // arg1: animation ID
                AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.run));
                return 0;
            }
            case EBin.event_code_binary.DIRE: // 0x36, "TurnInstant", "Make the character face an angle", true, 1, { 1 }, { "Angle" }, { AT_USPIN }, 0
            case EBin.event_code_binary.DDIR: // 0x87, "TurnInstantEx", "Make the specified character face an angle", true, 2, { 1, 1 }, { "Entry", "Angle" }, { AT_ENTRY, AT_USPIN }, 0
            {
                if (eventCodeBinary == EBin.event_code_binary.DDIR)
                    po = (PosObj)this.GetObj1(); //arg1: object to turn
                Int32 angle = this.getv1(); // arg1/2: angle: 0 south, 64 west, 128 north, 192 east.
                if (po == null || (UnityEngine.Object)po.go == (UnityEngine.Object)null)
                    return 0;
                if (this.gMode == 1)
                {
                    if (mapNo == 504 && (Int32)po.sid == 15 && (this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 4 && angle == 240))
                        angle = 128;
                    Vector3 eulerAngles2 = po.go.transform.localRotation.eulerAngles;
                    eulerAngles2.y = EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)(angle << 4));
                    po.rotAngle[1] = eulerAngles2.y;
                }
                else if (this.gMode == 3)
                {
                    Vector3 rot = ((Actor)po).wmActor.rot;
                    rot.y = EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)(angle << 4));
                    ((Actor)po).wmActor.rot = rot;
                }
                return 0;
            }
            case EBin.event_code_binary.ROTXZ: // 0x37, "SetPitchAngle", "Turn the model in the up/down direction.arg1: angle (pitch axis).arg2: angle (XZ axis).", true, 2, { 1, 1 }, { "Pitch", "XZ Angle" }, { AT_USPIN, AT_USPIN }, 0
            {
                Int32 pitchAxisAngle = (Int32)(Int16)(this.getv1() << 4);
                Int32 xyAxisAngle = (Int32)(Int16)(this.getv1() << 4);
                Single pitchAxisDegrees = EventEngineUtils.ClampAngle(EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)pitchAxisAngle));
                Single xyAxisDegrees = EventEngineUtils.ClampAngle(EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)xyAxisAngle));
                po.rotAngle[0] = pitchAxisDegrees;
                po.rotAngle[2] = xyAxisDegrees;
                return 0;
            }
            case EBin.event_code_binary.BTLCMD: // 0x38, "Attack", "Make the enemy attack. The target(s) are to be set using the SV_Target variable. Inside an ATB function, the attack is added to the queue. Inside a counter function, the attack occurs directly. arg1: attack to perform.", true, 1, { 1 }, { "Attack" }, { AT_ATTACK }, 0
            {
                switch (this.gExec.level)
                {
                    case 0:
                    {
                        btl_cmd.SetEnemyCommand((UInt16)this.GetSysList(1), (UInt16)this.GetSysList(0), BattleCommandId.EnemyDying, this.getv1());
                        break;
                    }
                    case 1:
                    {
                        btl_cmd.SetEnemyCommand((UInt16)this.GetSysList(1), (UInt16)this.GetSysList(0), BattleCommandId.EnemyCounter, this.getv1());
                        break;
                    }
                    case 3:
                    {
                        this.gExec.btlchk = (Byte)0;
                        btl_cmd.SetEnemyCommand((UInt16)this.GetSysList(1), (UInt16)this.GetSysList(0), BattleCommandId.EnemyAtk, this.getv1());
                        break;
                    }
                }
                return 0;
            }
            case EBin.event_code_binary.MESHSHOW: // 0x39, "ShowObject", "Show specific meshes of an object", true, 2, { 1, 1 }, { "Object", "Mesh list" }, { AT_ENTRY, AT_BOOLLIST }, 0
            {
                po = (PosObj)this.GetObj1(); // arg1: object
                Int32 mesh = this.getv1(); // arg2: mesh list to show
                if (po != null)
                    po.geoMeshShow(mesh);
                return 0;
            }
            case EBin.event_code_binary.MESHHIDE: // 0x3A, "HideObject", "Hide specific meshes of an object", true, 2, { 1, 1 }, { "Object", "Mesh list" }, { AT_ENTRY, AT_BOOLLIST }, 0
            {
                po = (PosObj)this.GetObj1(); // arg1: object
                Int32 mesh = this.getv1(); // arg2: mesh list to hide
                if (po != null)
                    po.geoMeshHide(mesh);
                return 0;
            }
            case EBin.event_code_binary.OBJINDEX: // 0x3B, "SetObjectIndex", "Redefine the current object's index.", true, 1, { 1 }, { "Index" }, { AT_USPIN }, 0
            {
                this.gExec.index = (Byte)this.getv1(); // arg1: new index.
                return 0;
            }
            case EBin.event_code_binary.ENCSCENE: // 0x3C, "SetRandomBattles", "Define random battles. { "Pattern", "Battle 1", "Battle 2", "Battle 3", "Battle 4" }, { AT_USPIN, AT_BATTLE, AT_BATTLE, AT_BATTLE, AT_BATTLE }, 0
            {
                this._enCountData.pattern = (Byte)this.getv1(); // arg1: pattern, deciding the encounter chances and the topography (World Map only).
                this._enCountData.scene[0] = (UInt16)this.getv2(); // 0: possible random battles {0.375, 0.28, 0.22, 0.125}
                this._enCountData.scene[1] = (UInt16)this.getv2(); // 1: possible random battles {0.25, 0.25, 0.25, 0.25}
                this._enCountData.scene[2] = (UInt16)this.getv2(); // 2: possible random battles {0.35, 0.3, 0.3, 0.05}
                this._enCountData.scene[3] = (UInt16)this.getv2(); // 3: possible random battles {0.45, 0.4, 0.1, 0.05}
                return 0;
            }
            case EBin.event_code_binary.AFRAME: // 0x3D, "SetAnimationInOut", "Specify the starting and ending animation frames of the character", true, 2, { 1, 1 }, { "In-Frame",  "Out-Frame" }, { AT_USPIN, AT_USPIN }, 0
            {
                actor.inFrame = (Byte)this.getv1(); // arg1: starting frame
                actor.outFrame = (Byte)this.getv1(); // arg2: ending frame (capped to the animation's duration)
                return 0;
            }
            case EBin.event_code_binary.ASPEED: // 0x3E, "SetAnimationSpeed", "Set the current object's animation speed.", true, 1, { 1 }, { "Speed" }, { AT_USPIN }, 0
            {
                if (mapNo >= 3009 && mapNo <= 3011)
                {
                    this.getv1();
                    return 0;
                }
                actor.aspeed0 = (Byte)this.getv1(); // arg1: speed
                return 0;
            }
            case EBin.event_code_binary.AMODE: // 0x3F, "SetAnimationFlags", "Set the current object's next animation looping flags", true, 2, { 1, 1 }, { "Flag", "Repeat" }, { AT_ANIMFLAG, AT_USPIN }, 0
            {
                Int32 flag = this.getv1() << 3 & (EventEngine.afHold | EventEngine.afLoop | EventEngine.afPalindrome); // arg1: looping flag list. 1: freeze at end, 2: loop, 3: loop back and forth
                actor.animFlag = (Byte)flag;
                actor.loopCount = (Byte)this.getv1(); // arg2: times to repeat
                return 0;
            }
            case EBin.event_code_binary.ANIM: // 0x40, "RunAnimation", "Make the character play an animation", true, 1, { 2 }, { "Animation" }, { AT_ANIMATION }, 0
            {
                Int32 anim = this.getv2(); // arg1: animation ID
                AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue(anim));
                if (this.gMode == 1)
                {
                    if (mapNo == 800 && scCounter == 3740 && anim == 13158)
                        actor.inFrame = (Byte)19;
                    this.ExecAnim(actor, anim);
                    if (mapNo == 307 && (Int32)actor.uid == 13)
                    {
                        this._geoTexAnim = actor.go.GetComponent<GeoTexAnim>();
                        if (anim == 10328)
                        {
                            this._geoTexAnim.geoTexAnimStop(2);
                            this._geoTexAnim.geoTexAnimPlay(0);
                        }
                    }
                    if (mapNo == 561 && (Int32)actor.uid == 6)
                    {
                        this._geoTexAnim = actor.go.GetComponent<GeoTexAnim>();
                        if (anim == 351)
                        {
                            this._geoTexAnim.geoTexAnimStop(2);
                            this._geoTexAnim.geoTexAnimPlay(1);
                        }
                    }
                    if (mapNo == 1605 && (Int32)actor.uid == 18)
                    {
                        this._geoTexAnim = actor.go.GetComponent<GeoTexAnim>();
                        if (anim == 11958)
                        {
                            this._geoTexAnim.geoTexAnimStop(2);
                            this._geoTexAnim.geoTexAnimPlay(0);
                        }
                    }
                    if (mapNo == 2934 && (Int32)actor.uid == 2 && anim == 12429)
                    {
                        FF9StateSystem.Settings.CallBoosterButtonFuntion(BoosterType.BattleAssistance, false);
                        FF9StateSystem.Settings.CallBoosterButtonFuntion(BoosterType.HighSpeedMode, false);
                        FF9StateSystem.Settings.CallBoosterButtonFuntion(BoosterType.Attack9999, false);
                        FF9StateSystem.Settings.CallBoosterButtonFuntion(BoosterType.NoRandomEncounter, false);
                        PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.BattleAssistance, false);
                        PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.HighSpeedMode, false);
                        PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.Attack9999, false);
                        PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.NoRandomEncounter, false);
                        PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(false);
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.PreEnding);
                    }
                }
                else if (this.gMode == 3)
                    this.ExecAnim(actor, anim);
                return 0;
            }
            case EBin.event_code_binary.WAITANIM: // 0x41, "WaitAnimation", "Wait until the current object's animation has ended."
            {
                if (((Int32)actor.animFlag & EventEngine.afExec) == 0)
                    return 0;
                this.stay();
                return 1;
            }
            case EBin.event_code_binary.DWAITANIM: // 0xBE, "WaitAnimationEx", "Wait until the object's animation has ended", true, 1, { 1 }, { "Object" }, { AT_ENTRY }, 0
            {
                if (((Int32)((Actor)this.GetObj1()).animFlag & EventEngine.afExec) == 0) // arg1: object's entry
                    return 0;
                this.stay();
                return 1;
            }
            case EBin.event_code_binary.ENDANIM: // 0x42, "StopAnimation", "Stop the character's animation."
            {
                this.AnimStop(actor);
                return 0;
            }
            case EBin.event_code_binary.STARTSEQ: // 0x43, "RunSharedScript", "Run script passing the current object to it and continue executing the current function. If another shared script is already running for this object, it will be terminated", true, 1, { 1 }, { "Entry" }, { AT_ENTRY }, 0
            {
                Int32 uid = (Int32)this.gExec.uid + EventEngine.cSeqOfs;
                Obj objByUid = this.FindObjByUID(uid);
                if (objByUid != null)
                    this.DisposeObj(objByUid);
                Int32 entry = this.getv1(); // arg1: entry (should be a one-function entry)
                Seq seq = new Seq(entry, uid);
                if (mapNo == 1610)
                {
                    po = (PosObj)this.GetObjUID(21);
                    if (po != null)
                    {
                        Int32 varManually = this.eBin.getVarManually(EBin.MAP_INDEX_SVR);
                        //Debug.Log((object)("map_id = " + (object)varManually));
                        if (varManually == 26)
                        {
                            this._geoTexAnim = po.go.GetComponent<GeoTexAnim>();
                            if (entry == 13)
                            {
                                this._geoTexAnim.geoTexAnimStop(2);
                                this._geoTexAnim.geoTexAnimPlay(0);
                            }
                        }
                    }
                }
                return 0;
            }
            case EBin.event_code_binary.WAITSEQ: // 0x44, "WaitSharedScript", "Wait until the ran shared script has ended."
            {
                if (this.FindObjByUID((Int32)this.gExec.uid + EventEngine.cSeqOfs) == null)
                    return 0;
                this.stay();
                return 1;
            }
            case EBin.event_code_binary.ENDSEQ: // 0x45, "StopSharedScript", "Terminate the execution of the ran shared script."
            {
                Obj objByUid = this.FindObjByUID((Int32)this.gExec.uid + EventEngine.cSeqOfs);
                if (objByUid != null)
                    this.DisposeObj(objByUid);
                return 0;
            }
            case EBin.event_code_binary.NECKFLAG: // 0x47, "EnableHeadFocus", "Enable or disable the character turning his head toward an active object", true, 1, { 1 }, { "Flags" }, { AT_BOOLLIST }, 0
            {
                actor.actf &= (UInt16)~(EventEngine.actNeckT | EventEngine.actNeckM | EventEngine.actNeckTalk);
                actor.actf |= (UInt16)(this.getv1() & (EventEngine.actNeckT | EventEngine.actNeckM | EventEngine.actNeckTalk)); // arg1: flags. 1: enable 2: turn toward close characters 3: turn toward talkers
                return 0;
            }
            case EBin.event_code_binary.ITEMADD: // 0x48, "AddItem", "Add item to the player's inventory. Only one copy of key items can be in the player's inventory", true, 2, { 2, 1 }, { "Item", "Amount" }, { AT_ITEM, AT_USPIN }, 0
            {
                Int32 itemID = this.getv2(); // arg1: item to add
                Int32 count = this.getv1(); // arg2: amount to add
                ff9item.FF9Item_Add_Generic(itemID, count);
                EMinigame.Auction10TimesAchievement(this.gCur);
                if (itemID == 288)
                    EMinigame.ViviWinHuntAchievement();
                else if (itemID == 203)
                    EMinigame.DigUpMadainRingAchievement();
                else if (itemID == 324)
                    EMinigame.SuperSlickOilAchievement();
                else if (itemID == 283)
                    EMinigame.AtleteQueenAchievement();
                return 0;
            }
            case EBin.event_code_binary.ITEMDELETE: // 0x49, "RemoveItem", "Remove item from the player's inventory", true, 2, { 2, 1 }, { "Item", "Amount" }, { AT_ITEM, AT_USPIN }, 0
            {
                Int32 itemID = this.getv2(); // arg1: item to remove
                Int32 count = this.getv1(); // arg2: amount to remove
                ff9item.FF9Item_Remove_Generic(itemID, count);
                if (itemID == 324)
                    EMinigame.MognetCentralAchievement();
                return 0;
            }
            case EBin.event_code_binary.BTLSET: // 0x4A, "RunBattleCode", "Run a battle code | End Battle: 0 for a defeat (deprecated), 1 for a victory, 2 for a victory without victory pose, 3 for a defeat, 4 for an escape, 5 for an interrupted battle, 6 for a game over, 7 for an enemy escape. Run Camera: Camera ID. Change Field: ID of the destination field after the battle. Add Gil: amount to add.", true, 2, { 1, 2 }, { "Code", "Argument" }, { AT_BATTLECODE, AT_SPIN }, 0
            {
                Int32 battleCode = this.getv1(); // arg1: battle code
                Int32 val = this.getv2(); // arg2: depends on the battle code
                //if (num23 == 33)
                //    Debug.Log((object)"BTLSET 33");
                btl_scrp.SetBattleData((UInt32)battleCode, val);
                return 0;
            }
            case EBin.event_code_binary.RADIUS: // 0x4B, "SetObjectLogicalSize", "Set different size informations of the object", true, 3, { 1, 1, 1 }, { "Walk Radius", "Collision Radius", "Talk Distance" }, { AT_SPIN, AT_USPIN, AT_USPIN }, 0
            {
                Int32 size = this.getv1(); // arg1: size for pathing
                Int32 collRad = (Int32)(Byte)this.getv1(); // arg2: collision radius
                Int32 talkRad = (Int32)(Byte)this.getv1(); // arg3: talk distance
                if (mapNo == 1823 && (Int32)this.gCur.sid == 13 && (size == 8 && collRad == 8) && talkRad == 8)
                    size = 30;
                else if (mapNo == 657 && (Int32)this.gCur.sid == 22 && size == 40)
                    size = 35;
                else if (mapNo == 164 && (Int32)this.gCur.sid == 7 && size == 30)
                    size = 20;
                else if (mapNo == 1104 && ((Int32)this.gCur.sid == 13 || (Int32)this.gCur.sid == 5) || mapNo == 1108 && this.gCur.sid == 40)
                {
                    talkRad = 30;
                    collRad = 30;
                    size = 30;
                }
                else if (mapNo == 1606 && (Int32)this.gCur.sid == 34)
                    size = 15;
                else if (mapNo == 2714 && (Int32)this.gCur.sid == 26)
                    size = 22;
                else if (mapNo == 2903 && (Int32)this.gCur.sid == 4 && size == 20)
                    size = 16;
                else if (mapNo == 3100 && (Int32)this.gCur.sid == 20 && size == 20)
                    size = 25;
                po.collRad = (Byte)collRad;
                po.talkRad = (Byte)talkRad;
                if (this.gMode == 1)
                {
                    if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
                        return 0;
                    FieldMapActorController component1 = gameObject.GetComponent<FieldMapActorController>();
                    if ((UnityEngine.Object)component1 != (UnityEngine.Object)null)
                        component1.radius = (Single)(size * 4);
                    FieldMapActor component2 = gameObject.GetComponent<FieldMapActor>();
                    if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
                        component2.CharRadius = (Single)(size * 4);
                }
                return 0;
            }
            case EBin.event_code_binary.ATTACH: // 0x4C, "AttachObject", "Attach an object to another one", true, 3, { 1, 1, 1 }, { "Object", "Carrier", "Attachement Point" }, { AT_ENTRY, AT_ENTRY, AT_USPIN }, 0
            {
                Obj attachedObj = this.GetObj1(); // arg1: carried object
                Obj targetObj = this.GetObj1(); // arg2: carrying object
                GameObject attachedObjUnity = attachedObj.go;
                GameObject targetObject = targetObj.go;
                Int32 bone_index = this.getv1(); // arg3: attachment point (unknown format)
                if ((UnityEngine.Object)attachedObjUnity != (UnityEngine.Object)null && (UnityEngine.Object)targetObject != (UnityEngine.Object)null)
                {
                    if (this.gMode == 1 || this.gMode == 2)
                    {
                        if ((Int32)this.gCur.sid == 8 && mapNo == 62 || (Int32)this.gCur.sid == 2 && mapNo == 3010)
                            bone_index = 19;
                        geo.geoAttach(attachedObjUnity, targetObject, bone_index);
                    }
                    else if (this.gMode == 3)
                        geo.geoAttachInWorld(attachedObj, targetObj, bone_index);
                }
                return 0;
            }
            case EBin.event_code_binary.DETACH: // 0x4D, "DetachObject", "Stop attaching an object to another one", true, 1, { 1 }, { "Object" }, { AT_ENTRY }, 0
            {
                Obj attachedObj = this.GetObj1(); // arg1: carried object
                if (attachedObj != null)
                {
                    Boolean restorePosAndScaling = false;
                    if (mapNo == 656 || mapNo == 657 || mapNo == 658 || mapNo == 659)
                        restorePosAndScaling = true;
                    if (this.gMode == 1)
                        geo.geoDetach(attachedObj.go, restorePosAndScaling);
                    else if (this.gMode != 2 && this.gMode == 3)
                        geo.geoDetachInWorld(attachedObj);
                }
                return 0;
            }
            case EBin.event_code_binary.WATCH: // 0x4E, "0x4E", "Unknown Opcode (WATCH)."
            {
                Int32 execIpMinus1 = this.gExec.ip - 1;
                this.gExec.ip = execIpMinus1;
                Int32 ipByte = (Int32)this.gExec.getByteIP();
                while (ipByte != 0)
                {
                    ++this.gExec.ip;
                    ipByte = (Int32)this.gExec.getByteIP();
                    ++execIpMinus1;
                }
                this.eBin.SetVariableSpec(ref execIpMinus1);
                this.gExec.ip = execIpMinus1 + 1;
                this.gArgUsed = 1;
                return 0;
            }
            case EBin.event_code_binary.STOP: // 0x4F, "0x4F", "Unknown Opcode (STOP).", true, 1, { 1 }, { "Unknown" }, { AT_SPIN }, 0
            {
                Int32 stop = (Int32)this.gExec.getByteIP(-1) | (Int32)this.gExec.getByteIP() << 8;
                ++this.gExec.ip;
                this.gArgUsed = 1;
                return 6;
            }
            case EBin.event_code_binary.WAITTURN: // 0x50, "WaitTurn", "Wait until the character has turned."
            case EBin.event_code_binary.DWAITTURN: // 0xBC, "WaitTurnEx", "Wait until an object facing movement has ended", true, 1, { 1 }, { "Object" }, { AT_ENTRY }, 0
            {
                if (eventCodeBinary == EBin.event_code_binary.DWAITTURN)
                    actor = (Actor)this.GetObj1(); // arg1: object's entry
                if (((Int32)actor.flags & 128) != 0)
                    this.stay();
                return 1;
            }
            case EBin.event_code_binary.TURNA: // 0x51, "TurnTowardObject", "Turn the character toward an entry object (animated)", true, 2, { 1, 1 }, { "Object", "Speed" }, { AT_ENTRY, AT_USPIN }, 0
            {
                Obj obj1 = this.GetObj1(); // arg1: object
                Int32 turnSpeed = this.getv1(); // arg2: turn speed (1 is slowest)
                if (obj1 != null)
                {
                    po = (PosObj)obj1;
                    Vector3 targetPos = new Vector3(po.pos[0], po.pos[1], po.pos[2]);
                    Vector3 playerPos = new Vector3(actor.pos[0], actor.pos[1], actor.pos[2]);
                    Single targetAngle = this.eBin.angleAsm(targetPos.x - playerPos.x, targetPos.z - playerPos.z);
                    this.StartTurn(actor, targetAngle, true, turnSpeed);
                }
                return 0;
            }
            case EBin.event_code_binary.ASLEEP: // 0x52, "SetInactiveAnimation", "Change the animation played when inactive for a long time. The inaction time required is:First Time = 200 + 4 * Random[0, 255]Subsequent Times = 200 + 2 * Random[0, 255]", true, 1, { 2 }, { "Animation" }, { AT_ANIMATION }, 0
            {
                actor.sleep = (UInt16)this.getv2(); // arg1: animation ID
                AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.sleep));
                return 0;
            }
            case EBin.event_code_binary.NOINITMES: // 0x53, "PreventWindowInit", "Seems to prevent new dialog windows to close older ones."
            {
                this.eTb.InhInitMes();
                return 0;
            }
            case EBin.event_code_binary.WAITMES: // 0x54, "WaitWindow", "Wait until the window is closed", true, 1, { 1 }, { "Window ID" }, { AT_USPIN }, 0
            {
                if (mapNo == 1650 && FF9StateSystem.Settings.CurrentLanguage == "Japanese" && ((Int32)this.gCur.sid == 19 && this.gCur.ip == 1849))
                {
                    this.getv1();
                    return 0;
                }
                this.gCur.winnum = (Byte)this.getv1(); // arg1: window ID determined at its creation
                this.gCur.wait = (Byte)254;
                return 1;
            }
            case EBin.event_code_binary.MROT: // 0x55, "SetWalkTurnSpeed", "Change the turn speed of the object when it walks or runs (default is 16)..", true, 1, { 1 }, { "Turn Speed" }, { AT_USPIN }, 0
            {
                Int32 turnSpeed = this.getv1(); // arg1: turn speed (with 0, the object doesn't turn while moving).Special treatments:Vivi's in Iifa Tree/Eidolon Moun (field 1656) is initialized to 48.Choco's in Chocobo's Paradise (field 2954) is initialized to 96
                if (turnSpeed == 0)
                    turnSpeed = (Int32)Byte.MaxValue;
                actor.omega = (Byte)turnSpeed;
                return 0;
            }
            case EBin.event_code_binary.TURN: // 0x56, "TimedTurn", "Make the character face an angle (animated)", true, 2, { 1, 1 }, { "Angle", "Speed" }, { AT_USPIN, AT_USPIN }, 0
            case EBin.event_code_binary.DTURN: // 0xBB, "TimedTurnEx", "Make an object face an angle (animated)", true, 3, { 1, 1, 1 }, { "Object", "Angle", "Speed" }, { AT_ENTRY, AT_USPIN, AT_USPIN }, 0
            {
                if (eventCodeBinary == EBin.event_code_binary.DTURN)
                    actor = (Actor)this.GetObj1(); // arg1: object's entry
                Int32 angle = this.getv1() << 4; // arg1/2: angle.0 faces south, 64 faces west, 128 faces north and 192 faces east
                Int32 turnSpeed = this.getv1(); // arg2/3: turn speed (1 is slowest)
                /*if (mapNo == 1209 && actor.sid == 9 && angle == 2048)
                {
                    Int32 num3 = 0;
                }*/
                this.StartTurn(actor, EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)angle), true, turnSpeed);
                return 0;
            }
            case EBin.event_code_binary.ENCRATE: // 0x57, "SetRandomBattleFrequency", "Set the frequency of random battles / 255 is the maximum frequency, corresponding to ~12 walking steps or ~7 running steps. 0 is the minimal frequency and disables random battles.", true, 1, { 1 }, { "Frequency" }, { AT_USPIN }, 0
            {
                this._context.encratio = (Byte)this.getv1(); // arg1: frequency (0-255)
                if ((Int32)this._context.encratio == 0)
                    this._encountBase = 0;
                return 0;
            }
            case EBin.event_code_binary.BGLCOLOR: // 0x59, "SetTileColor", "Change the color of a field tile block", true, 4, { 1, 1, 1, 1 }, { "Tile Block", "Color" }, { AT_TILE, AT_COLOR_CYAN, AT_COLOR_MAGENTA, AT_COLOR_YELLOW }, 0
            {
                Int32 overlayNdx = (Int32)this.getv1(); // arg1: background tile block
                Byte Cyan = (Byte)this.getv1();
                Byte Magenta = (Byte)this.getv1();
                Byte Yellow = (Byte)this.getv1();
                this.fieldmap.EBG_overlaySetShadeColor(overlayNdx, Cyan, Magenta, Yellow); // arg1: background tile block
                return 0;
            }
            case EBin.event_code_binary.BGLORIGIN: // 0x5E, "SetTilePosition", "Move a field tile block", true, 3, { 1, 2, 2 }, { "Tile Block", "Position X", "Position Y" }, { AT_TILE, AT_SPIN, AT_SPIN }, 0
            {
                Int32 overlayNdx = (Int32)this.getv1(); // arg1: background tile block
                Single dx = (Single)this.getv2(); // arg2: x movement
                Single dy = (Single)this.getv2(); // arg3: y movement
                this.fieldmap.EBG_overlaySetOrigin(overlayNdx, dx, dy);
                return 0;
            }
            case EBin.event_code_binary.BGLMOVE: // 0x5A, "SetTilePositionEx", "Move a field tile block.", true, 4, { 1, 2, 2, 2 }, { "Tile Block", "Position X", "Position Y", "Position Closeness" }, { AT_TILE, AT_SPIN, AT_SPIN, AT_SPIN }, 0
            {
                Int32 overlayNdx = (Int32)this.getv1(); // arg1: background tile block
                Single dx = (Single)this.getv2(); // arg2: x movement
                Single dy = (Single)this.getv2(); // arg3: y movement
                Int16 dz = (Int16)this.getv2(); // arg4: depth, with higher value being further away from camera
                this.fieldmap.EBG_overlayMove(overlayNdx, dx, dy, dz);
                return 0;
            }
            case EBin.event_code_binary.BGLMOVE_TIMED: // TODO add description
            {
                Int32 overlayNdx = (Int32)this.getv3(); // arg1: background tile block
                Single dx = (Single)this.getv3(); // arg2: x movement
                Single dy = (Single)this.getv3(); // arg3: y movement
                Int16 dz = (Int16)this.getv3(); // arg4: depth, with higher value being further away from camera
                Int32 time = (Int32)this.getv3(); // arg5: how much time this will move // TODO in what unit?
                this.fieldmap.EBG_overlayMoveTimed(overlayNdx, dx, dy, dz, time);
                return 0;
            }
            case EBin.event_code_binary.BGLACTIVE: // 0x5B, "ShowTile", "Show or hide a field tile block", true, 2, { 1, 1 }, { "Tile Block", "Show" }, { AT_TILE, AT_BOOL }, 0
            {
                Int32 overlayNdx = (Int32)this.getv1(); // arg1: background tile block
                Boolean isActive = (Int32)this.getv1() != 0; // arg2: boolean show/ hide
                this.fieldmap.EBG_overlaySetActive(overlayNdx, isActive); // arg1: background tile block.
                return 0;
            }
            case EBin.event_code_binary.BGLLOOP: // 0x5C, "MoveTileLoop", "Make the image of a field tile loop over space.", true, 4, { 1, 1, 2, 2 }, { "Tile Block", "Activate", "X Loop", "Y Loop" }, { AT_TILE, AT_BOOL, AT_SPIN, AT_SPIN }, 0
            {
                this.fieldmap.EBG_overlaySetLoop(this.getv1(), (UInt32)this.getv1(), this.getv2(), this.getv2()); //arg1: background tile block.arg2: boolean on/ off.3rd and arg4s: speed in the X and Y directions.
                return 0;
            }
            case EBin.event_code_binary.BGLPARALLAX: // 0x5D, "MoveTile", "Make the field moves depending on the camera position", true, 4, { 1, 1, 2, 2 }, { "Tile Block", "Activate", "Movement X", "Movement Y" }, { AT_TILE, AT_BOOL, AT_SPIN, AT_SPIN }, 0
            {
                this.fieldmap.EBG_overlaySetParallax(this.getv1(), (UInt32)this.getv1(), this.getv2(), this.getv2()); // arg1: background tile block.arg2: boolean on/off.3rd and arg4s: parallax movement in (X, Y) format
                return 0;
            }
            case EBin.event_code_binary.BGAANIME: // 0x5F, "RunTileAnimation", "Run a field tile animation", true, 2, { 1, 1 }, { "Field Animation", "Frame" }, { AT_TILEANIM, AT_USPIN }, 0
            {
                Int32 animID = this.getv1(); // arg1: background animation
                Int32 startFrame = this.getv1(); // arg2: starting frame
                if (mapNo == 2925 && animID == 0) // Restore animation in Crystal World (missing in every version)
                {
                    this.fieldmap.EBG_animAnimate(1, 0);
                    this.fieldmap.EBG_animSetFlags(1, 16);
                    this.fieldmap.EBG_animSetFrameRate(1, 128);
                }
                this.fieldmap.EBG_animAnimate(animID, startFrame);
                return 0;
            }
            case EBin.event_code_binary.BGAACTIVE: // 0x60, "ActivateTileAnimation", "Make a field tile animation active..", true, 2, { 1, 1 }, { "Tile Animation", "Activate" }, { AT_TILEANIM, AT_BOOL }, 0
            {
                Int32 animNdx = (Int32)this.getv1(); // arg1: background tile block
                Boolean isActive = (Int32)this.getv1() != 0; // arg2: boolean show/ hide
                this.fieldmap.EBG_animSetActive(animNdx, isActive); // arg1: background animation.arg2: boolean on/off
                return 0;
            }
            case EBin.event_code_binary.BGARATE: // 0x61, "SetTileAnimationSpeed", "Change the speed of a field tile animation", true, 2, { 1, 2 }, { "Tile Animation", "Speed" }, { AT_TILEANIM, AT_SPIN }, 0
            {
                Int32 animNdx = this.getv1(); // arg1: background animation
                Int32 frameRate = this.getv2(); // arg2: speed (256 = 1 tile/frame)
                if (mapNo == 66 && FF9StateSystem.Settings.IsFastForward && (animNdx == 3 || animNdx == 2) && frameRate == 128)
                    frameRate = 320; // Prima Vista/Interior
                this.fieldmap.EBG_animSetFrameRate(animNdx, frameRate);
                return 0;
            }
            case EBin.event_code_binary.BGAWAIT: // 0x63, "SetTileAnimationPause", "Make a field tile animation pause at some frame in addition to its normal animation speed", true, 3, { 1, 1, 1 }, { "Tile Animation", "Frame ID", "Time" }, { AT_TILEANIM, AT_USPIN, AT_USPIN }, 0
            {
                Int32 animNdx = this.getv1(); // arg1: background animation
                Int32 frame = this.getv1(); // arg2: animation frame
                Int32 waitTime = this.getv1(); // arg3: wait time
                this.fieldmap.EBG_animSetFrameWait(animNdx, frame, waitTime);
                return 0;
            }
            case EBin.event_code_binary.BGAFLAG: // 0x64, "SetTileAnimationFlags", "Add flags of a field tile animation", true, 2, { 1, 1 }, { "Tile Animation", "Flags" }, { AT_TILEANIM, AT_BOOLLIST }, 0
            {
                Int32 animNdx = this.getv1(); // arg1: background animation
                Int32 flags = this.getv1(); // arg2: flags (only the flags 5 and 6 can be added). 5: unknown 6: loop back and forth
                this.fieldmap.EBG_animSetFlags(animNdx, flags);
                return 0;
            }
            case EBin.event_code_binary.BGARANGE: // 0x65, "RunTileAnimationEx", "Run a field tile animation and choose its frame range", true, 3, { 1, 1, 1 }, { "Tile Animation", "Start", "End" }, { AT_TILEANIM, AT_USPIN, AT_USPIN }, 0
            {
                this.fieldmap.EBG_animSetPlayRange(this.getv1(), this.getv1(), this.getv1()); // arg1: background animation.arg2: starting frame.arg3: ending frame
                return 0;
            }
            case EBin.event_code_binary.SETROW: // 0x62, "SetRow", "Change the battle row of a party member", true, 2, { 1, 1 }, { "Character", "Row" }, { AT_LCHARACTER, AT_BOOL }, 0
            {
                CharacterId charId = this.chr2slot(this.getv1()); // arg1: party member
                Int32 row = this.getv1(); // arg2: boolean front/back
                if (charId != CharacterId.NONE)
                    FF9StateSystem.Common.FF9.GetPlayer(charId).info.row = (Byte)row;
                return 0;
            }
            case EBin.event_code_binary.MESVALUE: // 0x66, "SetTextVariable", "Set the value of a text number or item variable", true, 2, { 1, 2 }, { "Variable ID", "Value" }, { AT_USPIN, AT_ITEM }, 0
            {
                Int32 scriptID = this.getv1(); // arg1: text variable's 'Script ID'
                Int32 value = this.getv2(); // arg2: depends on which text opcode is related to the text variable: [VAR_NUM]: integral value. [VAR_ITEM]: item ID. [VAR_TOKEN]: token number

                this.eTb.SetMesValue(scriptID, value); 
                return 0;
            }
            case EBin.event_code_binary.TWIST: // 0x67, "SetControlDirection", "Set the angles for the player's movement control", true, 2, { 1, 1 }, { "Arrow Angle", "Analogic Angle" }, { AT_SPIN, AT_SPIN }, 0
            {
                this._context.twist_a = (Int16)this.getv1(); // arg1: angle used for arrow movements
                this._context.twist_d = (Int16)this.getv1(); // arg2: angle used for analogic stick movements
                FF9StateSystem.Field.SetTwistAD((Int32)this._context.twist_a, (Int32)this._context.twist_d);
                return 0;
            }
            case EBin.event_code_binary.FICON: // 0x68, "Bubble", "Display a speech bubble with a symbol inside over the head of player's character", true, 1, { 1 }, { "Symbo" }, { AT_BUBBLESYMBOL }, 0
            {
                BubbleUI.IconType bubbleType = (BubbleUI.IconType)this.getv1(); // arg1: bubble symbol
                if (mapNo == 2955)
                {
                    if (this.gCur.uid == 24)
                        EIcon.PollFIcon(BubbleUI.IconType.ExclamationAndDuel);
                    else
                        EIcon.PollFIcon(bubbleType);
                }
                else
                    EIcon.PollFIcon(bubbleType);
                return 0;
            }
            case EBin.event_code_binary.TIMERSET: // 0x69, "ChangeTimerTime", "Change the remaining time of the timer window", true, 1, { 2 }, { "Time" }, { AT_USPIN }, 0
            {
                TimerUI.SetTime(this.getv2()); // arg1: time in seconds
                return 0;
            }
            case EBin.event_code_binary.DASHOFF: // 0x6A, "DisableRun", "Make the player's character always walk."
            {
                this._context.dashinh = (Byte)1;
                return 0;
            }
            case EBin.event_code_binary.CLEARCOLOR: // 0x6B, "SetBackgroundColor", "Change the default color, seen behind the field's tiles", true, 3, { 1, 1, 1 }, { "Color" }, { AT_COLOR_RED, AT_COLOR_GREEN, AT_COLOR_BLUE }, 0
            {
                Color newColor = new Color((Single)this.getv1() / (Single)Byte.MaxValue, (Single)this.getv1() / (Single)Byte.MaxValue, (Single)this.getv1() / (Single)Byte.MaxValue);
                this.fieldmap.GetMainCamera().backgroundColor = newColor; // arg 1-3: color in (Red, Green, Blue)
                return 0;
            }
            case EBin.event_code_binary.BGSSCROLL: // 0x6F, "MoveCamera", "Move camera over time.", true, 4, { 2, 2, 1, 1 }, { "Destination", "Time", "Smoothness" }, { AT_POSITION_X, AT_POSITION_Y, AT_USPIN, AT_USPIN }, 0  // screen size = 320?
            {
                //1st and arg2s: destination in (X, Y) format.3nd argument: movement duration.arg4: scrolling type (8 for sinusoidal, other values for linear interpolation).
                this.fieldmap.EBG_scene2DScroll((Int16)this.getv2(), (Int16)this.getv2(), (UInt16)this.getv1(), (UInt32)this.getv1());
                return 0;
            }
            case EBin.event_code_binary.BGSRELEASE: // 0x70, "ReleaseCamera", "Release camera movement, getting back to its normal behaviour", true, 2, { 1, 1 }, { "Time", "Smoothness" }, { AT_SPIN, AT_USPIN }, 0
            {
                this.fieldmap.EBG_scene2DScrollRelease(this.getv1(), (UInt32)this.getv1()); // arg1: duration of the repositioning.arg2: scrolling type (8 for sinusoidal, other values for linear interpolation)
                return 0;
            }
            case EBin.event_code_binary.BGCACTIVE: // 0x71, "EnableCameraServices", "Enable or disable camera services.", true, 3, { 1, 1, 1 }, { "Enable", "Time", "Smoothness" }, { AT_BOOL, AT_SPIN, AT_USPIN }, 0
            {
                // arg1: boolean activate/deactivate.arg2: duration of the repositioning when activating (defaulted to 30 if -1 is given).arg3: scrolling type of the repositioning when activating (8 for sinusoidal, other values for linear interpolation)
                this.fieldmap.EBG_char3DScrollSetActive((UInt32)this.getv1(), this.getv1(), (UInt32)this.getv1());
                return 0;
            }
            case EBin.event_code_binary.BGCHEIGHT: // 0x72, "SetCameraFollowHeight", "Define the standard height gap between the player's character position and the camera view", true, 1, { 2 }, { "Height" }, { AT_SPIN }, 0
            {
                this.fieldmap.charAimHeight = (Int16)this.getv2(); // arg1: height
                return 0;
            }
            case EBin.event_code_binary.BGCLOCK: // 0x73, "EnableCameraFollow", "Make the camera follow the player's character"
            {
                this.fieldmap.EBG_charLookAtLock();
                return 0;
            }
            case EBin.event_code_binary.BGCUNLOCK: // 0x74, "DisableCameraFollow", "Stop making the camera follow the player's character"
            {
                this.fieldmap.EBG_charLookAtUnlock();
                return 0;
            }
            case EBin.event_code_binary.MENU: // 0x75, "Menu", "Open a menu", true, 2, { 1, 1 }, { "Menu Type", "Menu" }, { AT_MENUTYPE, AT_MENU }, 0
            {
                UInt32 menuId = Convert.ToUInt32(this.getv1()); // arg1: menu type
                UInt32 subId = Convert.ToUInt32(this.getv1()); // arg2: depends on the menu type. Naming Menu: character to name | Shop Menu: shop ID
                if (Configuration.Hacks.DisableNameChoice && menuId == 1)
                {
                    CharacterId charId = this.chr2slot((Int32)subId);
                    if (charId != CharacterId.NONE && NameSettingUI.IsDefaultName(charId))
                        this._ff9.GetPlayer(charId).Name = FF9TextTool.CharacterDefaultName(charId);
                    return 0;
                }
                EventService.StartMenu(menuId, subId);
                PersistenSingleton<UIManager>.Instance.MenuOpenEvent();
                return 1;
            }
            case EBin.event_code_binary.TRACKSTART: // 0x76, "DrawRegionStart", "Start drawing the convex polygonal region linked with the entry script: two starting points are placed at the same position", true, 2, { 2, 2 }, { "Vertex Position" }, { AT_POSITION_X, AT_POSITION_Y }, 0
            {
                Quad quad2 = (Quad)this.gCur;
                quad2.n = 2;
                QuadPos quadPos1 = quad2.q[0];
                Int16 startX = (Int16)this.getv2(); // arg1: starting vertex position X
                quad2.q[1].X = startX;
                quadPos1.X = startX;
                QuadPos quadPos2 = quad2.q[0];
                Int16 startZ = (Int16)this.getv2(); // arg1: starting vertex position Z
                quad2.q[1].Z = startZ;
                quadPos2.Z = startZ;
                return 0;
            }
            case EBin.event_code_binary.TRACK: // 0x77, "DrawRegionSetLast", "Change the position of the convex polygonal region's ending vertex", true, 2, { 2, 2 }, { "Vertex Position" }, { AT_POSITION_X, AT_POSITION_Y }, 0
            {
                Quad quad3 = (Quad)this.gCur;
                QuadPos quadPos3 = quad3.q[Math.Max(quad3.n, 1) - 1];
                quadPos3.X = (Int16)this.getv2(); // arg1: new vertex position X
                quadPos3.Z = (Int16)this.getv2(); // arg2: new vertex position Z
                return 0;
            }
            case EBin.event_code_binary.TRACKADD: // 0x78, "DrawRegionPushNew", "Add a new vertex to the convex polygonal region linked with the entry script (defaulting its position to be the same as the current ending vertex's position). This method cannot exceed 8 vertices."
            {
                Quad quad4 = (Quad)this.gCur;
                if (quad4.n > 0 && quad4.n < 8)
                {
                    quad4.q[quad4.n] = quad4.q[quad4.n - 1];
                    ++quad4.n;
                }
                return 0;
            }
            case EBin.event_code_binary.PRINTQUAD: // 0x79, "0x79", "Unknown Opcode (PRINTQUAD)."
            {
                return 0;
            }
            case EBin.event_code_binary.ATURNL: // 0x7A, "SetLeftAnimation", "Change the left turning animation", true, 1, { 2 }, { "Animation" }, { AT_ANIMATION }, 0
            {
                actor.turnl = (UInt16)this.getv2(); // arg1: animation ID
                AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.turnl));
                return 0;
            }
            case EBin.event_code_binary.ATURNR: // 0x7B, "SetRightAnimation", "Change the right turning animation", true, 1, { 2 }, { "Animation" }, { AT_ANIMATION }, 0
            {
                actor.turnr = (UInt16)this.getv2(); // arg1: animation ID
                AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.turnr));
                return 0;
            }
            case EBin.event_code_binary.CHOOSEPARAM: // 0x7C, "EnableDialogChoices", "Define choices availability in dialogs using the [INIT_MULTICHOICE] text opcode", true, 2, { 2, 1 }, { "Choice List", "Default Choice" }, { AT_BOOLLIST, AT_USPIN }, 0
            {
                this.eTb.SetChooseParam(this.getv2(), this.getv1()); // arg1: boolean list for the different choices.arg2: default choice selected
                return 0;
            }
            case EBin.event_code_binary.TIMERCONTROL: // 0x7D, "RunTimer", "Run or pause the timer window", true, 1, { 1 }, { "Run" }, { AT_BOOL }, 0
            {
                this._ff9.timerControl = this.getv1() != 0; // arg1: boolean, 0=pause
                TimerUI.SetPlay(this._ff9.timerControl);
                return 0;
            }
            case EBin.event_code_binary.SETCAM: // 0x7E, "SetFieldCamera", "Change the field's background camera", true, 1, { 1 }, { "Camera ID" }, { AT_USPIN }, 0
            {
                Int32 newCamIdx = this.getv1(); // arg1: camera ID
                Obj player = this.GetObjUID(250);
                if (player != null && player.cid == 4 && (mapNo == 153 || mapNo == 1214 || mapNo == 1806) && newCamIdx == 0) // Fix #493 - flapping camera
                {
                    Vector3 pos = ((Actor)player).fieldMapActorController.lastPos;
                    if ((pos.x > 500 || pos.y > 240) && !(scCounter == 1190 && pos.y > 314 && pos.y < 317)) // exception for scene with Steiner and plutos
                        return 0;
                }
                this.fieldmap.SetCurrentCameraIndex(newCamIdx);
                if (mapNo == 1205 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 4800 && this.eBin.getVarManually(6357) == 3)
                    this.SetActorPosition(this._fixThornPosObj, (Single)this._fixThornPosA, (Single)this._fixThornPosB, (Single)this._fixThornPosC);
                if (mapNo == 3009 && this.gCur.uid == 17 && newCamIdx == 0)
                {
                    EventEngine.resyncBGMSignal = 1;
                    //Debug.Log((object)("SET resyncBGMSignal = " + (object)EventEngine.resyncBGMSignal));
                }
                return 0;
            }
            // 0x7F, "EnableShadow", "Enable the shadow for the entry's object."
            // 0x80, "DisableShadow", "Disable the shadow for the entry's object."
            // 0x81, "SetShadowSize", "Set the entry's object shadow size.arg1: size X.arg2: size Y.", true, 2, { 1, 1 }, { "Size X", "Size Y" }, { AT_SPIN, AT_SPIN }, 0
            // 0x82, "SetShadowOffset", "Change the offset between the entry's object and its shadow.arg1: offset X.arg2: offset Y.", true, 2, { 2, 2 }, { "Offset X", "Offset Y" }, { AT_SPIN, AT_SPIN }, 0
            // 0x83, "LockShadowRotation", "Stop updating the shadow rotation by the object's rotation.arg1: locked rotation.", true, 1, { 1 }, { "Locked Rotation" }, { AT_SPIN }, 0
            // 0x84, "UnlockShadowRotation", "Make the shadow rotate accordingly with its object."
            // 0x85, "SetShadowAmplifier", "Amplify or reduce the shadow transparancy.arg1: amplification factor.", true, 1, { 1 }, { "Amplification Factor" }, { AT_USPIN }, 0
            case EBin.event_code_binary.IDLESPEED: // 0x86, "SetAnimationStandSpeed", "Change the standing animation speed", true, 4, { 1, 1, 1, 1 }, { "1st speed", "2nd speed", "3rd speed", "4th speed" }, { AT_USPIN, AT_USPIN, AT_USPIN, AT_USPIN }, 0
            {
                actor.idleSpeed[0] = (Byte)this.getv1(); // args: different animation speeds (default: 16), picked at random.
                actor.idleSpeed[1] = (Byte)this.getv1();
                actor.idleSpeed[2] = (Byte)this.getv1();
                actor.idleSpeed[3] = (Byte)this.getv1();
                return 0;
            }
            case EBin.event_code_binary.CHRFX: // 0x88, "RunModelCode", "Run a model code", true, 4, { 1, 2, 2, 2 }, { "Model Code", "Argument 1", "Argument 2", "Argument 3" }, { AT_MODELCODE, AT_SPIN, AT_SPIN, AT_SPIN }, 0
            {
                Int32 modelCode = this.getv1(); // arg1: model code
                // 2nd to 4th arguments: depends on the model code:
                // slice: 2 slice/unslice, 3 value
                // enable mirror: 2 on/off
                // mirror position: X - Z - Y
                // mirror normal: X - Z - Y
                // Mirror Color: R - G - B
                // Add Secondary Sound: anim, Frame, soundID
                // Sound Random Pitch: anim, Frame, random/not
                // Remove Sound: anim, Frame, value
                Int32 arg2 = this.getv2();
                Int32 arg3 = this.getv2();
                Int32 arg4 = this.getv2();
                fldchar.FF9FieldCharDispatch((Int32)po.uid, modelCode, arg2, arg3, arg4);
                return 0;
            }
            case EBin.event_code_binary.SEPV: // 0x89, "SetSoundPosition", "Set the position of a 3D sound", true, 4, { 2, 2, 2, 1 }, { "Position", "Volume" }, { AT_POSITION_X, AT_POSITION_Z, AT_POSITION_Y, AT_SPIN }, 0
            {
                Int32 PosPtr;
                Int32 VolPtr;
                FF9Snd.FF9FieldSoundGetPositionVolume(this.getv2(), this.getv2(), this.getv2(), out PosPtr, out VolPtr); // 1st to arg3s: sound position X Z Y
                this.sSEPos = PosPtr;
                Int32 soundVolume = this.getv1(); // arg4: sound volume
                this.sSEVol = VolPtr * soundVolume >> 7;
                if (this.sSEVol > (Int32)SByte.MaxValue)
                    this.sSEVol = (Int32)SByte.MaxValue;
                return 0;
            }
            case EBin.event_code_binary.SEPVA: // 0x8A, "SetSoundObjectPosition", "Set the position of a 3D sound to the object's position", true, 2, { 1, 1 }, { "Object", "Volume" }, { AT_ENTRY, AT_SPIN }, 0
            {
                po = (PosObj)this.GetObj1(); // arg1: object
                Int32 PosPtr;
                Int32 VolPtr;
                FF9Snd.FF9FieldSoundGetPositionVolume((Int32)po.pos[0], (Int32)po.pos[1], (Int32)po.pos[2], out PosPtr, out VolPtr);
                this.sSEPos = PosPtr;
                Int32 soundVolume = this.getv1(); // arg2: sound volume
                this.sSEVol = VolPtr * soundVolume >> 7;
                if (this.sSEVol > (Int32)SByte.MaxValue)
                    this.sSEVol = (Int32)SByte.MaxValue;
                return 0;
            }
            case EBin.event_code_binary.NECKID: // 0x8B, "SetHeadFocusMask", "Define a mask for characters to turn their head toward close characters: if this character's TargetMask has a common bit with another character's SelfMask, then he can turn to that other character.Also enables the two EnableHeadFocus flags 'enable' and 'turn toward close characters'.Characters must be at least 31 units away to have head focus and at most 2000 units away (the closest character is determined both by the distance and the angle)", true, 2, { 1, 1 }, { "SelfMask", "TargetMask" }, { AT_BOOLLIST, AT_BOOLLIST }, 0
            {
                actor.neckMyID = (Byte)this.getv1(); // arg1: SelfMask
                actor.neckTargetID = (Byte)this.getv1(); // arg2: TargetMask
                actor.actf |= (UInt16)(EventEngine.actNeckT | EventEngine.actNeckM);
                return 0;
            }
            case EBin.event_code_binary.ENCOUNT2: // 0x8C, "BattleEx", "Start a battle and choose its battle group", true, 3, { 1, 1, 2 }, { "Rush Type", "Battle Group", "Battle" }, { AT_SPIN, AT_SPIN, AT_BATTLE }, 0
            {
                this.getv1(); // arg1: rush type (unknown)
                this._ff9.btlSubMapNo = (SByte)this.getv1(); // arg2: group
                Int32 btlId = this.getv2(); // arg3: gathered battle and Steiner's state (highest bit) informations
                this._ff9.steiner_state = (Byte)(btlId >> 15 & 1);
                this.SetBattleScene(btlId & Int16.MaxValue);
                FF9StateSystem.Battle.isRandomEncounter = false;
                this._encountBase = 0;
                return 3;
            }
            case EBin.event_code_binary.TIMERDISPLAY: // 0x8D, "ShowTimer", "Activate the timer window", true, 1, { 1 }, { "Enable" }, { AT_BOOL }, 0
            {
                this._ff9.timerDisplay = this.getv1() != 0; // arg1: boolean show/hide
                TimerUI.SetEnable(this._ff9.timerDisplay);
                TimerUI.SetDisplay(this._ff9.timerDisplay);
                return 0;
            }
            case EBin.event_code_binary.RAISE: // 0x8E, "RaiseWindows", "Make all the dialogs and windows display over the filters"
            {
                this.eTb.RaiseAllWindow();
                return 0;
            }
            case EBin.event_code_binary.CHRCOLOR: // 0x8F, "SetModelColor", "Change a 3D model's color.", true, 4, { 1, 1, 1, 1 }, { "Object", "Color" }, { AT_ENTRY, AT_COLOR_RED, AT_COLOR_GREEN, AT_COLOR_BLUE }, 0
            {
                if (fldmcf.FF9FieldMCFSetCharColor((Int32)this.GetObj1().uid, this.getv1(), this.getv1(), this.getv1()) == 0) // arg1: entry associated with the model, 2-4: color in (Red, Green, Blue)
                    return 0;
                this.stay();
                return 1;
            }
            case EBin.event_code_binary.SLEEPINH: // 0x90, "DisableInactiveAnimation", "Prevent player's character to play its inactive animation."
            {
                this._context.idletimer = -1;
                return 0;
            }
            case EBin.event_code_binary.AUTOTURN: // 0x91, "FollowFocus", "Automatically turn toward the character that is stared at when needed", true, 1, { 1 }, { "Enable" }, { AT_BOOL }, 0
            {
                Int32 FollowFocus = this.getv1(); // arg1: boolean on/ off
                actor.turninst0 = FollowFocus == 0 ? (Int16)4 : (Int16)167;
                return 0;
            }
            case EBin.event_code_binary.BGLATTACH: // 0x92, "AttachTile", "Make a part of the field background follow the player's movements. Also apply a color filter out of that tile block's range", true, 7, { 1, 2, 1, 1, 1, 1, 1 }, { "Tile Block", "Position X", "Position Y", "Filter Mode", "Filter Color" }, { AT_TILE, AT_SPIN, AT_SPIN, AT_SPIN, AT_COLOR_RED, AT_COLOR_GREEN, AT_COLOR_BLUE }, 0
            {
                // arg1: tile block.2nd and arg3s: offset position in (X, Y) format.arg4: filter mode ; use -1 for no filter effect.5th to 7th arguments: filter color in (Red, Green, Blue) format.
                this.fieldmap.EBG_charAttachOverlay(this.getv1(), (Int16)this.getv2(), (Int16)this.getv1(), (SByte)this.getv1(), (Byte)this.getv1(), (Byte)this.getv1(), (Byte)this.getv1());
                return 0;
            }
            case EBin.event_code_binary.CFLAG: // 0x93, "SetObjectFlags", "Change flags of the current entry's object.", true, 1, { 1 }, { "Flags" }, { AT_BOOLLIST }, 0
            {
                // arg1: object flags. 1: show model 2: collision with player character 4: collision with NPC 8: disable talk 16: can't walk through by insisting 32: don't hide all 64: unknown/unused (can't change with this) 128: is turning (can't change with this)
                Int32 cflag = (Int32)(Byte)this.getv1(); 
                
                if (cflag == 14 && mapNo == 103 && this.gCur.uid >= 6 && this.gCur.uid <= 17) // fix: do not hide NPCs when they are offscreen for widescreen compatibility - Alexandria/Square, many NPCs and the Jump Rope
                    return 0;
                if (mapNo == 2934 && MBG.Instance.GetFrame < 10)
                {
                    this.StartCoroutine(this.DelayedCFLAG(this.gCur, cflag));
                    return 0;
                }
                this.gCur.flags = (Byte)((this.gCur.flags & -64) | (cflag & 63));
                return 0;
            }
            case EBin.event_code_binary.AJUMP: // 0x94, "SetJumpAnimation", "Change the jumping animation", true, 3, { 2, 1, 1 }, { "Animation", "Unknown", "Unknown" }, { AT_ANIMATION, AT_SPIN, AT_SPIN }, 0
            {
                actor.jump = (UInt16)this.getv2(); // arg1: animation ID
                actor.jump0 = (Byte)this.getv1(); // arg2: unknown
                actor.jump1 = (Byte)this.getv1(); // arg3: unknown
                AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.jump));
                return 0;
            }
            case EBin.event_code_binary.MESA:// 0x95, "WindowSyncEx", "Display a window with text inside and wait until it closes", true, 4, { 1, 1, 1, 2 }, { "Talker", "Window ID", "UI", "Text" }, { AT_ENTRY, AT_USPIN, AT_BOOLLIST, AT_TEXT }, 0
            case EBin.event_code_binary.MESAN: // 0x96, "WindowAsyncEx", "Display a window with text inside and continue the execution of the script without waiting", true, 4, { 1, 1, 1, 2 }, { "Talker", "Window ID", "UI", "Text" }, { AT_ENTRY, AT_USPIN, AT_BOOLLIST, AT_TEXT }, 0
            {
                po = (PosObj)this.GetObj1(); // arg1: talker's entry
                this.gCur.winnum = (Byte)this.getv1(); // arg2: window ID
                Int32 uiFlags = this.getv1(); // arg3: UI flag list. 3: disable bubble tail 4: mognet format 5: hide window 7: ATE window 8: dialog window
                Int32 textID = this.getv2(); // arg4: text to display
                this.SetFollow((Obj)po, (Int32)this.gCur.winnum, uiFlags);
                this.eTb.NewMesWin(textID, (Int32)this.gCur.winnum, uiFlags, po);
                if (eventCodeBinary == EBin.event_code_binary.MESAN)
                    return 0;
                this.gCur.wait = (Byte)254;
                return 1;
            }
            case EBin.event_code_binary.DRET: // 0x97, "ReturnEntryFunctions", "Make all the currently executed functions return for a given entry", true, 1, { 1 }, { "Entry" }, { AT_ENTRY }, 0
            {
                Obj obj1 = this.GetObj1(); // arg1: entry for which functions are returned
                if (obj1 != null)
                {
                    obj1.sx = (Byte)0;
                    obj1.state = EventEngine.stateInit;
                    this.Return(obj1);
                }
                return obj1 == this.gExec ? 1 : 0;
            }
            case EBin.event_code_binary.MOVT: // 0x98, "MakeAnimationLoop", "Make current object's currently playing animation loop", true, 1, { 1 }, { "Amount" }, { AT_USPIN }, 0
            {
                actor.loopCount = (Byte)this.getv1(); // arg1: loop amount
                return 0;
            }
            case EBin.event_code_binary.TSPEED: // 0x99, "SetTurnSpeed", "Change the entry's object turn speed", true, 1, { 1 }, { "Speed" }, { AT_USPIN }, 0
            {
                actor.tspeed = (Byte)this.getv1(); // arg1: turn speed (1 is slowest)
                if ((Int32)actor.tspeed == 0)
                    actor.tspeed = (Byte)16;
                return 0;
            }
            case EBin.event_code_binary.BGIACTIVET: // 0x9A, "EnablePathTriangle", "Enable or disable a triangle of field pathing", true, 2, { 2, 1 }, { "Triangle ID", "Enable" }, { AT_WALKTRIANGLE, AT_BOOL }, 0
            {
                Int32 triangleID = this.getv2(); // arg1: triangle ID
                Int32 isActive = this.getv1(); // arg2: boolean enable/disable
                if (mapNo == 1753 && triangleID == 207)
                    this.fieldmap.walkMesh.BGI_triSetActive(208U, (UInt32)isActive);
                else if (mapNo == 1606 && triangleID == 107)
                    isActive = 1;
                this.fieldmap.walkMesh.BGI_triSetActive((UInt32)triangleID, (UInt32)isActive);
                return 0;
            }
            case EBin.event_code_binary.TURNTO: // 0x9B, "TurnTowardPosition", "Turn the character toward a position (animated). The object's turn speed is used (default to 16).", true, 2, { 2, 2 }, { "Coordinate" }, { AT_POSITION_X, AT_POSITION_Y }, 0
            {
                Int32 posX = this.getv2(); // X position
                Int32 posZ = this.getv2(); // Z position
                if (!EventEngineUtils.nearlyEqual((Single)posX, gameObject.transform.localPosition.x) || !EventEngineUtils.nearlyEqual((Single)posZ, gameObject.transform.localPosition.z))
                {
                    FieldMapActorController component = gameObject.GetComponent<FieldMapActorController>();
                    Single a = this.eBin.angleAsm((Single)posX - component.curPos.x, (Single)posZ - component.curPos.z);
                    this.StartTurn(actor, a, true, (Int32)actor.tspeed);
                }
                return 0;
            }
            case EBin.event_code_binary.PREJUMP: // 0x9C, "RunJumpAnimation", "Make the character play its jumping animation."
            {
                actor.animFlag = (Byte)EventEngine.afHold;
                actor.inFrame = (Byte)0;
                actor.outFrame = (Byte)((UInt32)actor.jump0 - 1U);
                this.ExecAnim(actor, (Int32)actor.jump);
                return 0;
            }
            case EBin.event_code_binary.POSTJUMP: // 0x9D, "RunLandAnimation", "Make the character play its landing animation (inverted jumping animation)."
            {
                ff9shadow.FF9ShadowOnField((Int32)actor.uid);
                actor.animFlag = (Byte)0;
                actor.inFrame = (Byte)((UInt32)actor.jump1 + 1U);
                actor.outFrame = Byte.MaxValue;
                this.ExecAnim(actor, (Int32)actor.jump);
                return 0;
            }
            case EBin.event_code_binary.CHRSCALE: // 0x9F, "SetObjectSize", "Set the size of a 3D model", true, 4, { 1, 1, 1, 1 }, { "Object", "Size X", "Size Z", "Size Y" }, { AT_ENTRY, AT_SPIN, AT_SPIN, AT_SPIN }, 0
            {
                po = (PosObj)this.GetObj1(); // arg1: entry of the 3D model
                Int32 ratioX = this.getv1(); // Ratio X (def: 64)
                Int32 ratioZ = this.getv1(); // Ratio Z (def: 64)
                Int32 ratioY = this.getv1(); // Ratio Y (def: 64)
                if (po == null)
                    return 0;
                if ((UnityEngine.Object)po.go != (UnityEngine.Object)null)
                    geo.geoScaleSetXYZ(po.go, ratioX << 24 >> 18, ratioZ << 24 >> 18, ratioY << 24 >> 18);
                po.scaley = (Byte)ratioZ;
                if (mapNo == 576 && ((Int32)po.uid == 4 || (Int32)po.uid == 8 || ((Int32)po.uid == 9 || (Int32)po.uid == 10) || (Int32)po.uid == 11))
                {
                    this._geoTexAnim = po.go.GetComponent<GeoTexAnim>();
                    this._geoTexAnim.geoTexAnimStop(2);
                    this._geoTexAnim.geoTexAnimPlay(1);
                }
                return 0;
            }
            case EBin.event_code_binary.POS3: // 0xA1, "MoveInstantXZY", "Instantatly move the object", true, 3, { 2, 2, 2 }, { "Destination" }, { AT_POSITION_X, AT_POSITION_Z, AT_POSITION_Y }, 0
            case EBin.event_code_binary.DPOS3: // 0xAD, "MoveInstantXZYEx", "Instantatly move an object", true, 4, { 1, 2, 2, 2 }, { "Object", "Destination" }, { AT_ENTRY, AT_POSITION_X, AT_POSITION_Z, AT_POSITION_Y }, 0
            {
                if (eventCodeBinary == EBin.event_code_binary.DPOS3)
                    po = (PosObj)this.GetObj1(); // arg1: object's entry
                Int32 destX = this.getv2();
                Int32 destZ = -this.getv2();
                Int32 destY = this.getv2();
                if ((this.gMode != 1 || (Int32)po.model == (Int32)UInt16.MaxValue ? 0 : 1) == 1)
                {
                    if (po != null)
                    {
                        FieldMapActorController component = po.go.GetComponent<FieldMapActorController>();
                        if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.walkMesh != null)
                            component.walkMesh.BGI_charSetActive(component, 0U);
                        if (mapNo == 1205 && po.sid == 6 && (this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 4800 && destX == 418) && destY == 9733)
                        {
                            this._fixThornPosA = destX;
                            this._fixThornPosB = destZ;
                            this._fixThornPosC = destY;
                            this._fixThornPosObj = po;
                            destX = 600;
                            destY = 9999;
                        }
                        if (mapNo == 563 && po.sid == 16 && destX == -1614)
                            destX = -1635;
                        else if (mapNo == 572 && po.sid == 16 && destX == -1750)
                            destX = -1765;
                        else if (mapNo == 1310 && po.sid == 12 && destX == -1614)
                            destX = -1635;
                        else if (mapNo == 2110 && po.sid == 9 && destX == -1614)
                            destX = -1635;
                        else if (mapNo == 1607)
                        {
                            Int32 counter = this.eBin.getVarManually(EBin.SC_COUNTER_SVR);
                            Int32 var9169 = this.eBin.getVarManually(9169);
                            if ((po.model == 236 || po.model == 237) && (counter >= 6640 && counter < 6690) && var9169 == 0)
                                destZ += 50000;
                        }
                        else if (mapNo == 1606)
                        {
                            if (po.uid == 9 && destX == -171 && destZ == 641 && destY == 1042
                            || po.uid == 128 && destX == -574 && destZ == 624 && destY == 903
                            || po.uid == 129 && destX == -226 && destZ == 639 && destY == 1009
                            || po.uid == 130 && destX == -93 && destZ == 647 && destY == 1017
                            || po.uid == 131 && destX == 123 && destZ == 667 && destY == 1006
                            || po.uid == 132 && destX == -689 && destZ == 621 && destY == 859)
                                gameObject.GetComponent<FieldMapActor>().SetRenderQueue(2000);
                        }
                    }
                    if (mapNo == 1756 && (Int32)po.sid == 6 && (destX == -3019 && destY == 1226))
                    {
                        destX = -3145;
                        destZ = -2035;
                        destY = 1274;
                    }
                }
                this.SetActorPosition(po, (Single)destX, (Single)destZ, (Single)destY);
                if (po.cid == 4)
                    this.clrdist((Actor)po);
                return 0;
            }
            case EBin.event_code_binary.DRADIUS: // 0xA3, "0xA3", "Unknown Opcode (DRADIUS).", true, 4, { 1, 1, 1, 1 }, { "Unknown", "Unknown", "Unknown", "Unknown" }, { AT_SPIN, AT_SPIN, AT_SPIN, AT_SPIN }, 0  // Unused in Steam
            {
                this.getv1();
                this.getv1();
                this.getv1();
                this.getv1();
                return 0;
            }
            case EBin.event_code_binary.MJPOS: // 0xA4, "CalculateExitPosition", "Calculate the field exit position based on the region's polygon."
            {
                po = (PosObj)this.FindObjByUID((Int32)this._context.controlUID);
                if (po != null)
                {
                    QuadPos quadPos4 = ((Quad)this.gCur).q[0];
                    QuadPos quadPos5 = ((Quad)this.gCur).q[1];
                    Int32 posX = (Int32)quadPos5.X - (Int32)quadPos4.X;
                    Int32 posZ = (Int32)quadPos5.Z - (Int32)quadPos4.Z;
                    Int32 val = posX * posX + posZ * posZ >> 8;
                    if (val != 0)
                    {
                        val = ((Int32)((Double)posX * ((Double)po.pos[0] - (Double)quadPos4.X)) + (Int32)((Double)posZ * ((Double)po.pos[2] - (Double)quadPos4.Z))) / val;
                        if (val < 0)
                            val = 0;
                        else if (val > 256)
                            val = 256;
                    }
                    this.sMapJumpX = (val * posX >> 8) + (Int32)quadPos4.X;
                    this.sMapJumpZ = (val * posZ >> 8) + (Int32)quadPos4.Z;
                    if (mapNo == 552 && (Int32)quadPos4.X == 1231 && ((Int32)quadPos4.Z == 1556 && (Int32)quadPos5.X == 1291) && (Int32)quadPos5.Z == 1376)
                    {
                        this.sMapJumpX = 1226;
                        this.sMapJumpZ = 1430;
                    }
                }
                else
                    this.sMapJumpX = this.sMapJumpZ = 0;
                return 0;
            }
            case EBin.event_code_binary.SPEEDTH: // 0xA6, "SetRunSpeedLimit", "Change the speed at which the character uses his run animation instead of his walk animation (default is 31)", true, 1, { 1 }, { "Speed Limit" }, { AT_USPIN }, 0
            {
                actor.speedth = (Byte)this.getv1(); // arg1: speed limit
                return 0;
            }
            case EBin.event_code_binary.TURNDS: // 0xA7, "Turn", "Make the character face an angle (animated). Speed is defaulted to 16", true, 1, { 1 }, { "Angle" }, { AT_USPIN }, 0
            {
                Int32 angle = this.getv1() << 4; // arg1: angle.0 faces south, 64 faces west, 128 faces north and 192 faces east
                this.StartTurn(actor, EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)angle), true, (Int32)actor.tspeed);
                return 0;
            }
            case EBin.event_code_binary.BGI: // 0xA8, "SetPathing", "Change the pathing of the character", true, 1, { 1 }, { "Pathing" }, { AT_BOOL }, 0
            {
                Int32 isPathingActive = this.getv1(); // arg1: boolean pathing on/off
                if (mapNo == 2508 && (Int32)po.sid == 2 && isPathingActive == 1)
                    isPathingActive = 0;
                if ((Int32)po.model != (Int32)UInt16.MaxValue)
                {
                    FieldMapActorController FMactor = gameObject.GetComponent<FieldMapActorController>();
                    FMactor.walkMesh.BGI_charSetActive(FMactor, (UInt32)isPathingActive);
                }
                return 0;
            }
            case EBin.event_code_binary.GETSCREEN: // 0xA9, "CalculateScreenPosition", "Calculate the object's position in screen coordinates and store it in 'GetScreenCalculatedX' and 'GetScreenCalculatedY'.", true, 1, { 1 }, { "Object" }, { AT_ENTRY }, 0
            {
                Obj obj1 = this.GetObj1();
                if (obj1 != null && (UnityEngine.Object)obj1.go != (UnityEngine.Object)null)
                {
                    Single x;
                    Single y;
                    ETb.World2Screen(obj1.go.GetComponent<FieldMapActorController>().curPos, out x, out y);
                    this.sSysX = (Int32)x;
                    this.sSysY = (Int32)y;
                }
                return 0;
            }
            case EBin.event_code_binary.MENUON: // 0xAA, "EnableMenu", "Enable menu access by the player."
            {
                if (mapNo == 2172 && scCounter < 9100 && Localization.GetSymbol() == "JP" && this.gCur.sid == 1 && this.gCur.ip == 2964 && EIcon.AIconMode == 0)
                    return 0; // L. Castle/Telescope
                EventInput.PSXCntlClearPadMask(0, EventInput.Lmenu);
                PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(true);
                return 0;
            }
            case EBin.event_code_binary.MENUOFF: // 0xAB, "DisableMenu", "Disable menu access by the player."
            {
                if (mapNo == 2172 && scCounter < 9100 && Localization.GetSymbol() == "JP" && this.gCur.sid == 1 && this.gCur.ip == 119 && EIcon.AIconMode == 0)
                    return 0; // L. Castle/Telescope
                EventInput.PSXCntlSetPadMask(0, EventInput.Lmenu);
                PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
                return 1;
            }
            case EBin.event_code_binary.DISCCHANGE: // 0xAC, "ChangeDisc", "Allow to save the game and change disc", true, 1, { 2 }, { "Field Destination", "Disc" }, { AT_DISC_FIELD }, 0
            {
                Int32 fieldDiscDest = this.getv2(); // arg1: gathered field destination and disc destination
                this.FF9FieldDiscRequest((Byte)(fieldDiscDest >> 14 & 3), (UInt16)(fieldDiscDest & 16383));
                return 1;
            }
            case EBin.event_code_binary.MINIGAME: // 0xAE, "TetraMaster", "Begin a card game", true, 1, { 2 }, { "Card Deck" }, { AT_DECK }, 0
            {
                Int32 minigameFlag = this.getv2(); // arg1: card deck of the opponent
                EventService.SetMiniGame((UInt16)minigameFlag);
                EMinigame.SetQuadmistStadiumOpponentId(this.gCur, minigameFlag);
                EMinigame.SetThiefId(this.gCur);
                EMinigame.SetFatChocoboId(this.gCur);
                return 7;
            }
            case EBin.event_code_binary.DELETEALLCARD: // 0xAF, "DeleteAllCards", "Clear the player's Tetra Master's deck."
            {
                QuadMistDatabase.MiniGame_AwayAllCard();
                return 0;
            }
            case EBin.event_code_binary.SETMAPNAME: // 0xB0, "SetFieldName", "Change the name of the field", true, 1, { 2 }, { "Name" }, { AT_SPIN }, 0
            {
                FF9StateSystem.Common.FF9.mapNameStr = FF9TextTool.FieldText(this.getv2()); // arg1: new name (unknown format)
                return 0;
            }
            case EBin.event_code_binary.RESETMAPNAME: // 0xB1, "ResetFieldName", "Reset the name of the field."
            {
                FF9StateSystem.Common.FF9.mapNameStr = this._defaultMapName;
                return 0;
            }
            case EBin.event_code_binary.PARTYMENU: // 0xB2, "Party", "Allow the player to change the members of its party", true, 2, { 1, 2 }, { "Party Size", "Locked Characters" }, { AT_USPIN, AT_CHARACTER }, 0
            {
                FF9PARTY_INFO sPartyInfo = new FF9PARTY_INFO();
                List<CharacterId> selectList = new List<CharacterId>();
                sPartyInfo.party_ct = this.getv1(); // arg1: party size (if characters occupy slots beyond it, they are locked)
                foreach (PLAYER p in FF9StateSystem.Common.FF9.PlayerList)
                    if (p.info.party != 0)
                        selectList.Add(p.info.slot_no);
                Int32 fixedMembers = this.getv2(); // arg2: list of locked characters
                foreach (PLAYER p in FF9StateSystem.Common.FF9.PlayerList)
                    if ((fixedMembers & (1 << (Int32)p.info.slot_no)) != 0)
                        sPartyInfo.fix.Add(p.info.slot_no);
                for (Int32 i = 0; i < 4; ++i)
                {
                    if (FF9StateSystem.Common.FF9.party.member[i] != null)
                    {
                        sPartyInfo.menu[i] = FF9StateSystem.Common.FF9.party.member[i].info.slot_no;
                        selectList.Remove(sPartyInfo.menu[i]);
                    }
                    else
                    {
                        sPartyInfo.menu[i] = CharacterId.NONE;
                    }
                }
                sPartyInfo.select = selectList.ToArray();
                EventService.OpenPartyMenu(sPartyInfo);
                return 1;
            }
            case EBin.event_code_binary.SPS: // 0xB3, "RunSPSCode", "Run Sps code, which seems to be special model effects on the field", true, 5, { 1, 1, 2, 2, 2 }, { "Sps", "Code", "Parameter 1", "Parameter 2", "Parameter 3" }, { AT_USPIN, AT_SPSCODE, AT_SPIN, AT_SPIN, AT_SPIN }, 0
            case EBin.event_code_binary.SPS2: // 0xDA, "RunSPSCodeSimple", "Run Sps code, which seems to be special model effects on the field", true, 5, { 1, 1, 1, 2, 2 }, { "Sps", "Code", "Parameter 1", "Parameter 2", "Parameter 3" }, { AT_USPIN, AT_SPSCODE, AT_SPIN, AT_SPIN, AT_SPIN }, 0
            {
                // 3-5: depends on the sps code.
                // Load Sps (sps type)
                // Enable Attribute (attribute list, boolean enable/disable)
                // Set Position (X, -Z, Y)
                // Set Rotation (angle X, angle Z, angle Y)
                // Set Scale (scale factor)
                // Attach (object's entry to attach, bone number)
                // Set Fade (fade)
                // Set Animation Rate (rate)
                // Set Frame Rate (rate)
                // Set Frame (value) where the value is factored by 16 to get the frame
                // Set Position Offset (X, -Z, Y)
                // Set Depth Offset (depth)
                Int32 objNo = this.getv1(); // arg1: sps ID.
                Int32 parmType = this.getv1(); // arg2: sps code.
                Int32 arg0 = 0;
                if (eventCodeBinary == EBin.event_code_binary.SPS)
                    arg0 = this.getv2();
                else
                    arg0 = this.getv1();
                Int32 arg1 = this.getv2();
                Int32 arg2 = this.getv2();
                if (this.gMode == 1)
                    this.fieldSps.SetObjParm(objNo, parmType, arg0, arg1, arg2);
                else if (this.gMode == 2)
                    HonoluluBattleMain.battleSPS.SetObjParm(objNo, parmType, arg0, arg1, arg2);
                else if (this.gMode == 3)
                    ff9.world.WorldSPSSystem.SetObjParm(objNo, parmType, arg0, arg1, arg2);
                return 0;
            }
            case EBin.event_code_binary.FULLMEMBER: // 0xB4, "SetPartyReserve", "Define the party member availability for a future Party call", true, 1, { 2 }, { "Characters available" }, { AT_CHARACTER }, 0
            {
                Int32 reserveList = this.getv2(); // arg1: list of available characters
                Int32 reserveExtendedList = reserveList & ~0xF00; // This opcode puts Beatrix as the 8th character, before Cinna/Marcus/Blank
                reserveExtendedList |= (reserveList >> 1) & 0x700; // Cinna/Marcus/Blank -> shift the ID by 1
                if ((reserveList & 0x100) != 0)
                    reserveExtendedList |= 0x800; // Beatrix
                foreach (PLAYER p in FF9StateSystem.Common.FF9.PlayerList)
                    ff9play.FF9Play_Delete(p);
                foreach (PLAYER p in FF9StateSystem.Common.FF9.PlayerList)
                    if ((reserveExtendedList & (1 << (Int32)p.info.slot_no)) != 0)
                        ff9play.FF9Play_Add(p);
                return 0;
            }
            case EBin.event_code_binary.PRETEND: // 0xB5, "PretendToBe", "Link the object to another object, forcing this object to follow the linked object's position and logical animations..", true, 1, { 1 }, { "Entry" }, { AT_ENTRY }, 0
            {
                Obj objToPretendToBe = this.GetObj1(); // arg1: object to pretend to be
                if (objToPretendToBe != null)
                {
                    if (objToPretendToBe != po)
                    {
                        actor.parent = (Actor)objToPretendToBe;
                        actor.animFlag = (Byte)EventEngine.afExec;
                    }
                    else
                    {
                        actor.parent = (Actor)null;
                        actor.animFlag = (Byte)0;
                    }
                }
                return 0;
            }
            case EBin.event_code_binary.WMAPJUMP: // 0xB6, "WorldMap", "Change the scene to a world map", true, 1, { 2 }, { "World Map" }, { AT_WORLDMAP }, 0
            {
                this.SetNextMap(this.getv2()); // arg1: world map destination
                return 5;
            }
            case EBin.event_code_binary.EYE: // 0xB7, "0xB7", "Unknown World Map Opcode (EYE)."
            {
                if (this.gMode == 3)
                {
                    Vector3 eyePtr = ff9.w_cameraGetEyePtr();
                    this.SetActorPosition(po, eyePtr.x, eyePtr.y, eyePtr.z);
                    actor.actf = (UInt16)((UInt32)actor.actf | (UInt32)EventEngine.actEye);
                }
                return 0;
            }
            case EBin.event_code_binary.AIM: // 0xB8, "0xB8", "Unknown World Map Opcode (AIM)."
            {
                if (this.gMode == 3)
                {
                    Vector3 aimPtr = ff9.w_cameraGetAimPtr();
                    this.SetActorPosition(po, aimPtr.x, aimPtr.y, aimPtr.z);
                    actor.actf = (UInt16)((UInt32)actor.actf | (UInt32)EventEngine.actAim);
                }
                return 0;
            }
            case EBin.event_code_binary.SETKEYMASK: // 0xB9, "AddControllerMask", "Prevent the input to be processed by the game", true, 2, { 1, 2 }, { "Pad", "Buttons" }, { AT_USPIN, AT_BUTTONLIST }, 0
            {
                EventInput.PSXCntlSetPadMask(this.getv1(), Convert.ToUInt32(this.getv2())); // arg1: pad number (should only be 0 or 1). arg2: button list. 1:Select 4:Start 5:Up 6:Right 7:Down 8:Left 9:L2 10:R2 11:L1 12:R1 13:Triangle 14:Circle 15:Cross 16:Square 17:Cancel 18:Confirm 20:Moogle 21:L1Ex 22:R1Ex 23:L2Ex 24:R2Ex 25:Menu 26:SelectEx
                return 0;
            }
            case EBin.event_code_binary.CLEARKEYMASK: // 0xBA, "RemoveControllerMask", "Enable back the controller's inputs", true, 2, { 1, 2 }, { "Pad", "Buttons" }, { AT_USPIN, AT_BUTTONLIST }, 0
            {
                EventInput.PSXCntlClearPadMask(this.getv1(), Convert.ToUInt32(this.getv2())); // arg1: pad number (should only be 0 or 1). arg2: button list. 1:Select 4:Start 5:Up 6:Right 7:Down 8:Left 9:L2 10:R2 11:L1 12:R1 13:Triangle 14:Circle 15:Cross 16:Square 17:Cancel 18:Confirm 20:Moogle 21:L1Ex 22:R1Ex 23:L2Ex 24:R2Ex 25:Menu 26:SelectEx
                return 0;
            }
            case EBin.event_code_binary.DANIM: // 0xBD, "RunAnimationEx", "Play an object's animation", true, 2, { 1, 2 }, { "Object", "Animation ID" }, { AT_ENTRY, AT_ANIMATION }, 0
            {
                Actor actObj = (Actor)this.GetObj1(); // arg1: object's entry
                Int32 animId = this.getv2(); // arg2: animation ID
                AnimationFactory.AddAnimWithAnimatioName(actObj.go, FF9DBAll.AnimationDB.GetValue(animId));
                this.ExecAnim(actObj, animId);
                return 0;
            }
            case EBin.event_code_binary.TEXPLAY: // 0xC0, "EnableTextureAnimation", "Run a model texture animation and make it loop", true, 2, { 1, 1 }, { "Object", "Texture Animation" }, { AT_ENTRY, AT_USPIN }, 0
            {
                po = (PosObj)this.GetObj1(); // arg1: model's entry
                this._geoTexAnim = po.go.GetComponent<GeoTexAnim>();
                Int32 textureAnim = this.getv1(); // arg2: texture animation ID
                if ( (mapNo == 114 && po.uid == 2) 
                    || (mapNo == 450 && po.uid == 3) 
                    || (mapNo == 551 && po.uid == 10)
                    || (mapNo == 555 && po.uid == 12) 
                    || (mapNo == 559 && po.uid == 17)
                    || (mapNo == 2105 && po.uid == 2)
                    || (mapNo == 2450 && po.uid == 8) )
                {
                    if (textureAnim == 0)
                        textureAnim = 2;
                }
                if ((UnityEngine.Object)po.go != (UnityEngine.Object)null && (UnityEngine.Object)this._geoTexAnim != (UnityEngine.Object)null)
                    this._geoTexAnim.geoTexAnimPlay(textureAnim);
                return 0;
            }
            case EBin.event_code_binary.TEXPLAY1: // 0xC1, "RunTextureAnimation", "Run once a model texture animation", true, 2, { 1, 1 }, { "Object", "Texture Animation" }, { AT_ENTRY, AT_USPIN }, 0
            {
                po = (PosObj)this.GetObj1(); // arg1: model's entry
                this._geoTexAnim = po.go.GetComponent<GeoTexAnim>();
                Int32 textureAnim = this.getv1(); // arg2: texture animation ID
                if ((UnityEngine.Object)po.go != (UnityEngine.Object)null && (UnityEngine.Object)this._geoTexAnim != (UnityEngine.Object)null)
                    this._geoTexAnim.geoTexAnimPlayOnce(textureAnim);
                return 0;
            }
            case EBin.event_code_binary.TEXSTOP: // 0xC2, "StopTextureAnimation", "Stop playing the model texture animation", true, 2, { 1, 1 }, { "Object", "Texture Animation" }, { AT_ENTRY, AT_USPIN }, 0
            {
                po = (PosObj)this.GetObj1(); // arg1: model's entry
                this._geoTexAnim = po.go.GetComponent<GeoTexAnim>();
                Int32 textureAnim = this.getv1(); // arg2: texture animation ID
                if ((UnityEngine.Object)po.go != (UnityEngine.Object)null && (UnityEngine.Object)this._geoTexAnim != (UnityEngine.Object)null)
                    this._geoTexAnim.geoTexAnimStop(textureAnim);
                return 0;
            }
            case EBin.event_code_binary.BGVSET: // 0xC3, "SetTileCamera", "Link a tile block to a specific field camera (useful for looping movement bounds)", true, 2, { 1, 1 }, { "Tile Block", "Camera ID" }, { AT_TILE, AT_USPIN }, 0
            {
                this.fieldmap.EBG_overlaySetViewport(this.getv1(), this.getv1()); // arg1: background tile block.arg2: camera ID
                return 0;
            }
            case EBin.event_code_binary.WPRM: // 0xC4, "RunWorldCode", "Run one of the World Map codes, which effects have a large range. May modify the weather, the music, call the chocobo or enable the auto-pilot", true, 2, { 1, 2 }, { "Code", "Argument" }, { AT_WORLDCODE, AT_SPIN }, 0
            {
                ff9.w_frameSetParameter(this.getv1(), this.getv2()); // arg1: world code.arg2: depends on the code
                return 0;
            }
            case EBin.event_code_binary.FLDSND0: // 0xC5, "RunSoundCode", "Same as RunSoundCode3( code, music, 0, 0, 0 ).", true, 2, { 2, 2 }, { "Code", "Sound" }, { AT_SOUNDCODE, AT_SOUND }, 0
            {
                FF9Snd.FF9SoundArg0(this.getv2(), this.getv2());
                return 0;
            }
            case EBin.event_code_binary.FLDSND1: // 0xC6, "RunSoundCode1", "Same as RunSoundCode3( code, music, arg1, 0, 0 ).", true, 3, { 2, 2, 3 }, { "Code", "Sound", "Argument" }, { AT_SOUNDCODE, AT_SOUND, AT_SPIN }, 0
            {
                Int32 _parmtype1 = this.getv2();
                Int32 _objno1 = this.getv2();
                Int32 num66 = this.getv3();
                FF9Snd.FF9SoundArg1(_parmtype1, _objno1, num66);
                if (mapNo == 2928 && _objno1 == 2787)
                {
                    // Hill of Despair, Stop Sound 2787
                    FF9Snd.FF9SoundArg1(_parmtype1, 2982, num66);
                    FF9Snd.FF9SoundArg1(_parmtype1, 2983, num66);
                }
                return 0;
            }
            case EBin.event_code_binary.FLDSND2: // 0xC7, "RunSoundCode2", "Same as RunSoundCode3( code, music, arg1, arg2, 0 ).", true, 4, { 2, 2, 3, 1 }, { "Code", "Sound", "Argument", "Argument" }, { AT_SOUNDCODE, AT_SOUND, AT_SPIN, AT_SPIN }, 0
            {
                FF9Snd.FF9SoundArg2(this.getv2(), this.getv2(), this.getv3(), this.getv1());
                return 0;
            }
            case EBin.event_code_binary.FLDSND3: // 0xC8, "RunSoundCode3", "Run a sound code", true, 5, { 2, 2, 3, 1, 1 }, { "Code", "Sound", "Argument", "Argument", "Argument" }, { AT_SOUNDCODE, AT_SOUND, AT_SPIN, AT_SPIN, AT_SPIN }, 0
            {
                Int32 soundCode = this.getv2(); // arg1: sound code
                Int32 soundID = this.getv2(); // arg2: music or sound to process
                Int32 arg1 = this.getv3(); // 3-5: depends on the sound code
                Int32 arg2 = this.getv1();
                Int32 arg3 = this.getv1();
                FF9Snd.FF9SoundArg3(soundCode, soundID, arg1, arg2, arg3);
                if (mapNo == 2928)
                {
                    if (soundID == 2786)
                    {
                        FF9Snd.FF9SoundArg3(soundCode, 2980, arg1, arg2, arg3);
                        FF9Snd.FF9SoundArg3(soundCode, 2981, arg1, arg2, arg3);
                    }
                    else if (soundCode == -12288 && soundID == 2787 && (arg1 == 0 && arg2 == 128) && arg3 == 125)
                    {
                        FF9Snd.FF9SoundArg1(20736, 2980, 0);
                        FF9Snd.FF9SoundArg1(20736, 2981, 0);
                        FF9Snd.FF9SoundArg3(soundCode, 2982, arg1, arg2, arg3);
                        FF9Snd.FF9SoundArg3(soundCode, 2983, arg1, arg2, arg3);
                    }
                    else if (soundID == 2787)
                    {
                        FF9Snd.FF9SoundArg3(soundCode, 2982, arg1, arg2, arg3);
                        FF9Snd.FF9SoundArg3(soundCode, 2983, arg1, arg2, arg3);
                    }
                }
                return 0;
            }
            case EBin.event_code_binary.BGVDEFINE: // 0xC9, "SetupTileLoopingWindow", "Define the rectangular area for a looping tile block movement.", true, 5, { 1, 2, 2, 2, 2 }, { "Tile Block", "Position X", "Position Y", "Width", "Height" }, { AT_USPIN, AT_USPIN, AT_USPIN, AT_USPIN, AT_USPIN }, 0
            {
                this.fieldmap.EBG_overlayDefineViewport(this.getv1(), (Int16)this.getv2(), (Int16)this.getv2(), (Int16)this.getv2(), (Int16)this.getv2()); // arg1: background tile block. 2nd to arg5s: posX, posY, Width, Height
                return 0;
            }
            case EBin.event_code_binary.BGAVISIBLE: // 0xCA, "ResetTileAnimation", "Stop the background tile animation and optionally show its first frame's tile block", true, 2, { 1, 1 }, { "Tile Animation", "Visible" }, { AT_TILEANIM, AT_BOOL }, 0
            {
                this.fieldmap.EBG_animSetVisible(this.getv1(), this.getv1()); // arg1: background tile animation, arg2: show/hide the 1st frame's tile block.
                return 0;
            }
            case EBin.event_code_binary.BGIACTIVEF: // 0xCB, "EnablePath", "Enable a field path", true, 2, { 1, 1 }, { "Path", "Enable" }, { AT_WALKPATH, AT_BOOL }, 0
            {
                this.fieldmap.walkMesh.BGI_floorSetActive((UInt32)this.getv1(), (UInt32)this.getv1()); // arg1: field path ID.arg2: boolean enable/disable
                return 0;
            }
            case EBin.event_code_binary.CHRSET: // 0xCC, "AddCharacterAttribute", "Add specific attributes for the player character corresponding to the current's entry (should only be used by the entries of player characters)", true, 1, { 2 }, { "Attributes" }, { AT_BOOLLIST }, 0
            {
                Int32 attrFlag = this.getv2(); // arg1: attribute flags. 3: use a ladder. 5: hide shadow. 6: lock spin angle. 7: enable animation sounds. others: unknown.
                FF9Char.ff9char_attr_set((Int32)po.uid, attrFlag);
                return 0;
            }
            case EBin.event_code_binary.CHRCLEAR: // 0xCD, "RemoveCharacterAttribute", "Remove specific attributes for the player character corresponding to the current's entry (should only be used by the entries of player characters)", true, 1, { 2 }, { "Attributes" }, { AT_BOOLLIST }, 0
            {
                Int32 attrFlag = this.getv2(); // arg1: attribute flags. 3: use a ladder. 5: hide shadow. 6: lock spin angle. 7: enable animation sounds. others: unknown
                FF9Char.ff9char_attr_clear((Int32)po.uid, attrFlag);
                return 0;
            }
            case EBin.event_code_binary.GILADD: // 0xCE, "AddGi", "Give gil to the player", true, 1, { 3 }, { "Amount" }, { AT_SPIN }, 0
            {
                if ((this._ff9.party.gil += this.getv3u()) > 9999999U) // arg1: gil amount
                    this._ff9.party.gil = 9999999U;
                return 0;
            }
            case EBin.event_code_binary.GILDELETE: // 0xCF, "RemoveGi", "Remove gil from the player", true, 1, { 3 }, { "Amount" }, { AT_SPIN }, 0
            {
                UInt32 gilDecrease = this.getv3u(); // arg1: gil amount
                if ((this._ff9.party.gil -= gilDecrease) > 9999999U)
                    this._ff9.party.gil = 0U;
                if (this.isPosObj(this.gCur))
                    EMinigame.StiltzkinAchievement((PosObj)this.gCur, gilDecrease);
                return 0;
            }
            case EBin.event_code_binary.MESB: // 0xD0, "BattleDialog", "Display text in battle for 60 frames", true, 1, { 2 }, { "Text" }, { AT_TEXT }, 0
            {
                Int32 battleTextId = this.getv2(); // arg1: text to display
                String text = FF9TextTool.BattleText(battleTextId);
                VoicePlayer.PlayBattleVoice(battleTextId, text);
                UIManager.Battle.SetBattleMessage(text, 4);
                return 0;
            }
            case EBin.event_code_binary.ATTACHOFFSET: // 0xD4, "AttachObjectOffset", "Add an offset to the attachment point", true, 3, { 2, 2, 2 }, { "Offset" }, { AT_POSITION_X, AT_POSITION_Z, AT_POSITION_Y }, 0
            {
                GameObject sourceObject2 = this.gCur.go;
                Int32 offsetX = this.getv2(); // arg1: offset in (X, Z, Y) format
                Int32 offsetY = this.getv2();
                Int32 offsetZ = this.getv2();
                if ((UnityEngine.Object)sourceObject2 != (UnityEngine.Object)null)
                    geo.geoAttachOffset(sourceObject2, offsetX, offsetY, offsetZ);
                return 0;
            }
            case EBin.event_code_binary.PUSHHIDE: // 0xD5, "HideAllObjects", "Hide all the objects except those flagged with 'don't hide all' (6th object flag)."
            {
                for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
                {
                    Obj obj = objList.obj;
                    if (this.isPosObj(obj))
                    {
                        ((PosObj)obj).pflags = obj.flags;
                        if (((Int32)obj.flags & 32) == 0)
                            obj.flags = (Byte)((UInt32)obj.flags & 4294967294U);
                    }
                }
                return 0;
            }
            case EBin.event_code_binary.POPSHOW: // 0xD6, "ShowAllObjects", "Show all the (initialized) objects."
            {
                for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
                {
                    Obj obj = objList.obj;
                    if (this.isPosObj(obj))
                    {
                        obj.flags = (Byte)((UInt32)obj.flags & 4294967294U);
                        obj.flags = (Byte)((UInt32)obj.flags | (UInt32)((PosObj)obj).pflags & 1U);
                    }
                }
                return 0;
            }
            case EBin.event_code_binary.AICON: // 0xD7, "ATE", "Enable or disable ATE", true, 1, { 1 }, { "Unknown" }, { AT_SPIN }, 0
            {
                EIcon.SetAIcon(this.getv1()); // arg1: maybe flags (unknown format)
                return 0;
            }
            // 0xD8, "SetWeather", "Add a raining effect (works on the world maps, fields and in battles).arg1: strength of rain.arg2: speed of rain (unused for a battle rain).", true, 2, { 1, 1 }, { "Strength", "Speed" }, { AT_USPIN, AT_USPIN }, 0
            case EBin.event_code_binary.CLEARSTATUS: // 0xD9, "CureStatus", "Cure the status ailments of a party member", true, 2, { 1, 1 }, { "Character", "Statuses" }, { AT_LCHARACTER, AT_BOOLLIST }, 0
            {
                CharacterId charId = this.chr2slot(this.getv1()); // arg1: character
                BattleStatus statusList = (BattleStatus)this.getv1(); // arg2: status list. 1: Petrified 2: Venom 3: Virus 4: Silence 5: Darkness 6: Trouble 7: Zombie
                if (charId == CharacterId.NONE)
                    return 0;
                PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(charId);
                SFieldCalculator.FieldRemoveStatus(player, statusList);
                // https://github.com/Albeoris/Memoria/issues/22
                if (!player.info.sub_replaced)
                    SFieldCalculator.FieldRemoveStatus(FF9StateSystem.Common.FF9.GetPlayer(charId + 3), statusList);
                if (charId == CharacterId.Beatrix)
                    foreach (PLAYER play in FF9StateSystem.Common.FF9.PlayerList)
                        if (play.Index > CharacterId.Amarant && play.Index != CharacterId.Beatrix)
                            SFieldCalculator.FieldRemoveStatus(play, statusList);
                return 0;
            }
            case EBin.event_code_binary.WINPOSE: // 0xDB, "EnableVictoryPose", "Enable or disable the victory pose at the end of battles for a specific character", true, 2, { 1, 1 }, { "Character", "Enable" }, { AT_LCHARACTER, AT_BOOL }, 0
            {
                CharacterId charId = this.chr2slot(this.getv1()); // arg1: which character
                Int32 winPoseOnOff = this.getv1(); // arg2: boolean activate/deactivate
                if (charId != CharacterId.NONE)
                    this._ff9.GetPlayer(charId).info.win_pose = (Byte)winPoseOnOff;
                return 0;
            }
            case EBin.event_code_binary.SETVY3: // 0xE2, "SetupJump", "Setup datas for a Jump call", true, 4, { 2, 2, 2, 1 }, { "Destination", "Steps" }, { AT_POSITION_X, AT_POSITION_Z, AT_POSITION_Y, AT_USPIN }, 0
            {
                actor.jumpx = (Int16)this.getv2(); // 1st to arg3s: destination in (X, Z, Y)
                actor.jumpy = (short)-this.getv2();
                actor.jumpz = (Int16)this.getv2();
                Int32 steps = (Int32)(Byte)this.getv1(); // arg4: number of steps for the jump
                if (steps == 0)
                    steps = 8;
                actor.actf |= (UInt16)EventEngine.actJump;
                ff9shadow.FF9ShadowOffField((Int32)actor.uid);
                actor.inFrame = actor.jump0;
                actor.outFrame = actor.jump1;
                this.ExecAnim(actor, (Int32)actor.jump);
                actor.aspeed = (Byte)(((Int32)actor.outFrame - (Int32)actor.inFrame << 4) / steps);
                if (this.gMode == 1 && (Int32)po.model != (Int32)UInt16.MaxValue)
                {
                    fmac = gameObject.GetComponent<FieldMapActorController>();
                    fmac.walkMesh.BGI_charSetActive(fmac, 0U);
                }
                actor.x0 = (Int16)fmac.curPos.x;
                actor.y0 = (Int16)fmac.curPos.y;
                actor.z0 = (Int16)fmac.curPos.z;
                actor.jframe = (Byte)0;
                actor.jframeN = (Byte)steps;
                return 0;
            }
            case EBin.event_code_binary.JUMP3: // 0xDC, "Jump", "Perform a jumping animation. Must be used after a SetupJump call."
            {
                Int32 jumpFrame = (Int32)actor.jframe;
                ++actor.jframe;
                Int32 jframeN = (Int32)actor.jframeN;
                actor.pos[0] = (Single)(((Int32)actor.x0 * (jframeN - jumpFrame) + (Int32)actor.jumpx * jumpFrame) / jframeN);
                actor.pos[1] = (Single)((Int32)actor.y0 - jumpFrame * (jumpFrame << 3) + jumpFrame * ((Int32)actor.jumpy - (Int32)actor.y0) / jframeN + jumpFrame * (jframeN << 3));
                actor.pos[2] = (Single)(((Int32)actor.z0 * (jframeN - jumpFrame) + (Int32)actor.jumpz * jumpFrame) / jframeN);
                this.SetActorPosition(po, actor.pos[0], actor.pos[1], actor.pos[2]);
                if (jumpFrame >= jframeN)
                    return 0;
                this.stay();
                return 1;
            }
            case EBin.event_code_binary.PARTYDELETE: // 0xDD, "RemoveParty", "Remove a character from the player's team", true, 1, { 1 }, { "Character" }, { AT_LCHARACTER }, 0
            {
                CharacterId charId = this.chr2slot(this.getv1()); // arg1: character to remove
                if (Configuration.Hacks.AllCharactersAvailable >= 2)
                    return 0;
                Int32 party_id = 0;
                while (party_id < 4 && (this._ff9.party.member[party_id] == null || this._ff9.party.member[party_id].info.slot_no != charId))
                    ++party_id;
                if (party_id < 4)
                {
                    ff9play.FF9Play_SetParty(party_id, CharacterId.NONE);
                    this.SetupPartyUID();
                }
                return 0;
            }
            case EBin.event_code_binary.PLAYERNAME: // 0xDE, "SetName", "Change the name of a party member. Clear the text opcodes from the chosen text", true, 2, { 1, 2 }, { "Character", "Text" }, { AT_LCHARACTER, AT_TEXT }, 0
            {
                CharacterId charId = this.chr2slot(this.getv1()); // arg1: character to rename
                Int32 textId = this.getv2(); // arg2: new name
                if (charId != CharacterId.NONE)
                    this._ff9.GetPlayer(charId).Name = FF9TextTool.RemoveOpCode(FF9TextTool.FieldText(textId));
                return 0;
            }
            case EBin.event_code_binary.OVAL: //  // 0xDF, "SetObjectOvalRatio", "Define a stretching factor for the object's collisions (seems to only work on world maps). The collisions' shape is not exactly an oval but consists of two discs patched together", true, 1, { 1 }, { "Stretching Factor" }, { AT_USPIN }, 0
            {
                //arg1: increase of the collision in the facing direction of the object.
                //0 removes the feature(both circles are merged)
                //48 makes the object's collision twice higher toward the facing direction than on its sides
                //100 corresponds to a factor of about 2.7 in the facing direction
                //255 corresponds to a factor of about 4
                po.ovalRatio = (Byte)this.getv1();
                return 0;
            }
            case EBin.event_code_binary.INCFROG: // 0xE0, "AddFrog", "Add one frog to the frog counter."
            {
                _ff9.Frogs.Increment();
                EMinigame.CatchingGoldenFrogAchievement(this.gCur);
                return 0;
            }
            case EBin.event_code_binary.BEND: // 0xE1, "TerminateBattle", "Return to the field (or world map) when the rewards are disabled."
            {
                this._noEvents = true;
                PersistenSingleton<UIManager>.Instance.BattleResultScene.ShutdownBattleResultUI();
                return 0;
            }
            case EBin.event_code_binary.SETSIGNAL: // 0xE3, "SetDialogProgression", "Change the dialog progression value", true, 1, { 1 }, { "Progression" }, { AT_SPIN }, 0
            {
                ETb.gMesSignal = this.getv1(); // arg1: new dialog progression value
                return 0;
            }
            case EBin.event_code_binary.BGLSCROLLOFFSET: // 0xE4, "MoveTileLoopWithOffset", "Make the image of a field tile loop over space", true, 5, { 1, 1, 2, 2, 1 }, { "Tile Block", "Enable", "Speed", "Offset", "Is X Movement" }, { AT_TILE, AT_BOOL, AT_SPIN, AT_SPIN, AT_BOOL }, 0
            {
                //arg1: background tile block arg2: on/off arg3: movement speed arg4: offset arg5: move along the X axis or Y axis.
                this.fieldmap.EBG_overlaySetScrollWithOffset(this.getv1(), (UInt32)this.getv1(), this.getv2(), this.getv2(), (UInt32)this.getv1());
                return 0;
            }
            case EBin.event_code_binary.BTLSEQ: // 0xE5, "AttackSpecia", "Make the enemy instantatly use a special move. It doesn't use nor modify the battle state so it should be used when the battle is paused. The target(s) are to be set using the SV_Target variable", true, 1, { 1 }, { "Attack" }, { AT_ATTACK }, 0
            {
                btlseq.StartBtlSeq(this.GetSysList(1), this.GetSysList(0), this.getv1()); // arg1: attack to perform
                return 0;
            }
            case EBin.event_code_binary.BGLLOOPTYPE: // 0xE6, "SetTileLoopType", "Let tile be screen anchored or not", true, 2, { 1, 1 }, { "Tile Block", "Screen Anchored" }, { AT_TILE, AT_BOOL }, 0
            {
                this.fieldmap.EBG_overlaySetLoopType(this.getv1(), (UInt32)this.getv1()); // arg1: background tile block.arg2: boolean on/off
                return 0;
            }
            case EBin.event_code_binary.BGAFRAME: // 0xE7, "SetTileAnimationFrame", "Change the frame of a field tile animation (can be used to hide them all if the given frame is out of range, eg. 255)", true, 2, { 1, 1 }, { "Animation", "Frame ID" }, { AT_TILEANIM, AT_USPIN }, 0
            {
                this.fieldmap.EBG_animShowFrame(this.getv1(), this.getv1()); // arg1: background animation.arg2: animation frame to display
                return 0;
            }
            case EBin.event_code_binary.SYNCPARTY: // 0xE9, "UpdatePartyUID", "Update the party's entry list (Team Character 1-4 entries) for RunScript and InitObject calls. Should always be used after a Party call (it is automatic when using RemoveParty or AddParty)."
            {
                this.SetupPartyUID();
                return 0;
            }
            case EBin.event_code_binary.VRP: // 0xEA, "CalculateScreenOrigin", "Calculate the position of the top-left corner of the current camera view in screen coordinates and store it in 'GetScreenCalculatedX' and 'GetScreenCalculatedY'."
            {
                Int16 vrpX = 0;
                Int16 vrpY = 0;
                this.fieldmap.EBG_sceneGetVRP(ref vrpX, ref vrpY);
                this.sSysX = vrpX;
                this.sSysY = vrpY;
                return 0;
            }
            case EBin.event_code_binary.CLOSEALL: // 0xEB, "CloseAllWindows", "Close all the dialogs and UI windows."
            {
                this.eTb.YWindow_CloseAll(true);
                return 0;
            }
            case EBin.event_code_binary.WIPERGB: // 0xEC, "FadeFilter", "Apply a fade filter on the screen", true, 6, { 1, 1, 1, 1, 1, 1 }, { "Fade In/Out", "Fading Time", "Unknown", "Color" }, { AT_USPIN, AT_USPIN, AT_SPIN, AT_COLOR_CYAN, AT_COLOR_MAGENTA, AT_COLOR_YELLOW }, 0
            {
                Int32 filterMode = this.getv1(); // arg1: filter mode (0 for ADD, 2 for SUBTRACT)
                Int32 frame = this.getv1(); // arg2: fading time
                this.getv1(); // arg3: unknown
                Int32 cyan = this.getv1(); // 4 Cyan
                Int32 magenta = this.getv1(); // 5 Magenta
                Int32 yellow = this.getv1(); // 6 Yellow
                if (mapNo == 1819 && scCounter == 7030 && cyan == 130 && magenta == 160 && yellow == 170)
                {
                    cyan = 81; magenta = 110; yellow = 121;
                }

                Color32 fadeColor = new Color((Single)cyan / (Single)Byte.MaxValue, (Single)magenta / (Single)Byte.MaxValue, (Single)yellow / (Single)Byte.MaxValue);
                SceneDirector.InitFade((filterMode >> 1 & 1) != 0 ? FadeMode.Sub : FadeMode.Add, frame, fadeColor);
                return 0;
            }
            case EBin.event_code_binary.BGVALPHA: // 0xED, "SetTileLoopAlpha", "Unknown opcode about tile looping movements (EBG_overlayDefineViewportAlpha).", true, 3, { 1, 2, 2 }, { "Tile Block", "Unknown X", "Unknown Y" }, { AT_TILE, AT_SPIN, AT_SPIN }, 0
            {
                this.fieldmap.EBG_overlayDefineViewportAlpha(this.getv1(), this.getv2(), this.getv2()); // arg1: background tile block.2nd and arg3s: unknown factors (X, Y).
                return 0;
            }
            case EBin.event_code_binary.SLEEPON: // 0xEE, "EnableInactiveAnimation", "Allow the player's character to play its inactive animation. The inaction time required is:First Time = 200 + 4 * Random[0, 255]Following Times = 200 + 2 * Random[0, 255]"
            {
                this._context.idletimer = (Int16)0;
                return 0;
            }
            case EBin.event_code_binary.HEREON: // 0xEF, "ShowHereIcon", "Show the Here icon over player's chatacter", true, 1, { 1 }, { "Show" }, { AT_SPIN }, 0
            {
                EIcon.SetHereIcon(this.getv1()); // arg1: display type (0 to hide, 3 to show unconditionally)
                return 0;
            }
            case EBin.event_code_binary.DASHON: // 0xF0, "EnableRun", "Allow the player's character to run."
            {
                this._context.dashinh = (Byte)0;
                return 0;
            }
            case EBin.event_code_binary.SETHP: // 0xF1, "SetHP", "Change the HP of a party's member", true, 2, { 1, 2 }, { "Character", "HP" }, { AT_LCHARACTER, AT_USPIN }, 0
            {
                CharacterId charId = this.chr2slot(this.getv1()); // arg1: character
                if (charId != CharacterId.NONE)
                {
                    PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(charId);
                    Int32 newHp = this.getv2(); // arg2: new HP value
                    Single hpHealProp = newHp <= player.cur.hp ? -1f :
                        newHp == 9999 ? 1f : (Single)(newHp - player.cur.hp) / player.max.hp;
                    if (newHp == 9999)
                        newHp = Int32.MaxValue;

                    player.cur.hp = (UInt32)Math.Min(player.max.hp, newHp);

                    // https://github.com/Albeoris/Memoria/issues/22
                    if (!player.info.sub_replaced)
                    {
                        PLAYER subPlayer = FF9StateSystem.Common.FF9.GetPlayer(charId + 3);
                        subPlayer.cur.hp = (UInt32)Math.Min(subPlayer.max.hp, newHp);
                    }
                    if (charId == CharacterId.Beatrix && hpHealProp >= 0f)
                        foreach (PLAYER play in FF9StateSystem.Common.FF9.PlayerList)
                            if (play.Index > CharacterId.Amarant && play.Index != CharacterId.Beatrix)
                                play.cur.hp = (UInt32)Math.Min(play.max.hp, play.cur.hp + hpHealProp * play.max.hp);
                }
                return 0;
            }
            case EBin.event_code_binary.SETMP: // 0xF2, "SetMP", "Change the MP of a party's member", true, 2, { 1, 2 }, { "Character", "MP" }, { AT_LCHARACTER, AT_USPIN }, 0
            {
                CharacterId charId = this.chr2slot(this.getv1()); // arg1: character
                if (charId != CharacterId.NONE)
                {
                    PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(charId);
                    Int32 newMp = this.getv2(); // arg2: new MP value
                    Single mpHealProp = newMp <= player.cur.mp ? -1f :
                        newMp == 999 ? 1f : (Single)(newMp - player.cur.mp) / player.max.mp;
                    if (newMp == 999)
                        newMp = Int32.MaxValue;

                    player.cur.mp = (UInt32)Math.Min(player.max.mp, newMp);

                    // https://github.com/Albeoris/Memoria/issues/22
                    if (!player.info.sub_replaced)
                    {
                        PLAYER subPlayer = FF9StateSystem.Common.FF9.GetPlayer(charId + 3);
                        subPlayer.cur.mp = (UInt32)Math.Min(subPlayer.max.mp, newMp);
                    }
                    if (charId == CharacterId.Beatrix && mpHealProp >= 0f)
                        foreach (PLAYER play in FF9StateSystem.Common.FF9.PlayerList)
                            if (play.Index > CharacterId.Amarant && play.Index != CharacterId.Beatrix)
                                play.cur.mp = (UInt32)Math.Min(play.max.mp, play.cur.mp + mpHealProp * play.max.mp);
                }
                return 0;
            }
            case EBin.event_code_binary.CLEARAP: // 0xF3, "UnlearnAbility", "Set an ability's AP back to 0", true, 2, { 1, 1 }, { "Character", "Ability" }, { AT_LCHARACTER, AT_ABILITY }, 0
            {
                CharacterId charId = this.chr2slot(this.getv1()); // arg1: character
                Int32 abilIndex = this.getv1(); // arg2: ability to reset
                if (charId != CharacterId.NONE)
                    ff9abil.FF9Abil_ClearAp(FF9StateSystem.Common.FF9.GetPlayer(charId), abilIndex);
                return 0;
            }
            case EBin.event_code_binary.MAXAP: // 0xF4, "LearnAbility", "Make character learn an ability", true, 2, { 1, 1 }, { "Character", "Ability" }, { AT_LCHARACTER, AT_ABILITY }, 0
            {
                CharacterId charId = this.chr2slot(this.getv1()); // arg1: character
                Int32 abilIndex = this.getv1(); // arg2: ability to learn
                if (charId != CharacterId.NONE)
                    ff9abil.FF9Abil_SetMaster(FF9StateSystem.Common.FF9.GetPlayer(charId), abilIndex);
                return 0;
            }
            case EBin.event_code_binary.GAMEOVER: // 0xF5, "GameOver", "Terminate the game with a Game Over screen."
            {
                return 8;
            }
            case EBin.event_code_binary.VIBSTART: // 0xF6, "VibrateController", "Start the vibration lifespan", true, 1, { 1 }, { "Frame" }, { AT_USPIN }, 0
            {
                vib.VIB_vibrate((Int16)this.getv1()); // arg1: frame to begin with
                return 0;
            }
            case EBin.event_code_binary.VIBACTIVE: // 0xF7, "ActivateVibration", "Make the controller's vibration active. If the current controller's frame is out of the vibration time range, the vibration lifespan is reinit", true, 1, { 1 }, { "Activate" }, { AT_BOOL }, 0
            {
                vib.VIB_setActive(this.getv1() != 0); // arg1: boolean activate/deactivate
                return 0;
            }
            case EBin.event_code_binary.VIBTRACK1: // 0xF8, "RunVibrationTrack", "Run a vibration track", true, 3, { 1, 1, 1 }, { "Track", "Sample", "Activate" }, { AT_USPIN, AT_USPIN, AT_BOOL }, 0
            {
                vib.VIB_setTrackActive(this.getv1(), this.getv1(), this.getv1() != 0); // arg1: track ID.arg2: sample (0 or 1).arg3: boolean activate/deactivate
                return 0;
            }
            case EBin.event_code_binary.VIBTRACK: // 0xF9, "ActivateVibrationTrack", "Activate a vibration track", true, 3, { 1, 1, 1 }, { "Track", "Sample", "Activate" }, { AT_USPIN, AT_USPIN, AT_BOOL }, 0
            {
                vib.VIB_setTrackToModulate((UInt32)this.getv1(), (UInt32)this.getv1(), this.getv1() != 0); // arg1: track ID.arg2: sample (0 or 1).arg3: boolean activate/deactivate
                return 0;
            }
            case EBin.event_code_binary.VIBRATE: // 0xFA, "SetVibrationSpeed", "Set the vibration frame rate", true, 1, { 2 }, { "Frame Rate" }, { AT_USPIN }, 0
            {
                vib.VIB_setFrameRate((Int16)this.getv2()); // arg1: frame rate
                return 0;
            }
            case EBin.event_code_binary.VIBFLAG: // 0xFB, "SetVibrationFlags", "Change the vibration flags", true, 1, { 1 }, { "Flags" }, { AT_BOOLLIST }, 0
            {
                vib.VIB_setFlags((Int16)this.getv1()); // arg1: flags. 8:Loop 16:Wrap
                return 0;
            }
            case EBin.event_code_binary.VIBRANGE: // 0xFC, "SetVibrationRange", "Set the time range of vibration", true, 2, { 1, 1 }, { "Start", "End" }, { AT_USPIN, AT_USPIN }, 0
            {
                vib.VIB_setPlayRange((Int16)this.getv1(), (Int16)this.getv1()); // arg1-2: vibration range (Start, End)
                return 0;
            }
            case EBin.event_code_binary.HINT: // 0xFD, "PreloadField", "Surely preload a field; ignored in the non-PSX versions", true, 2, { 1, 2 }, { "Unknown", "Field" }, { AT_SPIN, AT_FIELD }, 0
            {
                Int32 hintCode = this.getv1(); // arg1: unknown - The only values are 0x5, 0x11 and 0x91 in non-modded scripts
                Int32 preloadField = this.getv2(); // arg2: field to preload
                if (hintCode == 0x30)
                {
                    // PreloadField( 48, ... )
                    // Add a field to the list of background free-view mode
                    this.sExternalFieldList.Add((short)preloadField);
                }
                else if (hintCode == 0x31)
                {
                    // PreloadField( 49, ... )
                    // Start the background free-view mode, starting by the selected field
                    this.sOriginalFieldName = this.fieldmap.mapName;
                    this.sOriginalFieldNo = mapNo;
                    this.sExternalFieldChangeField = preloadField;
                    this.sExternalFieldChangeCamera = 0;
                    this.sExternalFieldFade = 0;
                    foreach (Transform field_transform in this.fieldmap.transform)
                        if (field_transform.gameObject.name != "Background")
                            sOriginalFieldGameObjects.Add(field_transform.gameObject);
                    foreach (GameObject field_object in this.sOriginalFieldGameObjects)
                    {
                        field_object.transform.parent = null;
                        field_object.SetActive(false);
                    }
                    this.sExternalFieldMode = true;
                }
                return 0;
            }
            case EBin.event_code_binary.JOIN: // 0xFE, "SetCharacterData", "Init a party's member battle and menu datas", true, 5, { 1, 1, 1, 1, 1 }, { "Character", "Update Leve", "Equipement Set", "Category", "Ability Set" }, { AT_LCHARACTER, AT_BOOL, AT_EQUIPSET, AT_BOOLLIST, AT_ABILITYSET }, 0
            {
                CharacterId charId = this.chr2slot(this.getv1()); // arg1: character
                Int32 enableLeveling = this.getv1(); // arg2: boolean update level/don't update level
                EquipmentSetId eqp_id = (EquipmentSetId)this.getv1(); // arg3: equipement set to use
                if (charId != CharacterId.NONE)
                {
                    PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(charId);
                    Int32 category = this.getv1(); // arg4: character categories ; doesn't change if all are enabled. 1: male 2: female 3: gaian 4: terran 5: temporary character
                    if (category != Byte.MaxValue)
                        player.category = (Byte)category;
                    CharacterPresetId charPreset = (CharacterPresetId)this.getv1(); // arg5: ability and command set to use
                    if (charPreset != CharacterPresetId.NONE)
                        player.info.menu_type = charPreset;
                    ff9play.FF9Play_Change(player, enableLeveling != 0, eqp_id);
                    player.info.sub_replaced = true;
                }
                else
                {
                    this.getv1();
                    this.getv1();
                }
                return 0;
            }
            // 0xFF, "EXTENDED_CODE", "Not an opcode."
            case EBin.event_code_binary.BSACTIVE: //  0x102, "0x102", "Unknown Opcode"
            {
                this.fieldmap.walkMesh.BGI_simSetActive((UInt32)this.getv1(), (UInt32)this.getv1());
                return 0;
            }
            case EBin.event_code_binary.BSFLAG: // 0x103, "0x103", "Unknown Opcode"
            {
                this.fieldmap.walkMesh.BGI_simSetFlags((UInt32)this.getv1(), (UInt32)this.getv1());
                return 0;
            }
            case EBin.event_code_binary.BSFLOOR: // 0x104, "0x104", "Unknown Opcode"
            {
                this.fieldmap.walkMesh.BGI_simSetFloor((UInt32)this.getv1(), (UInt32)this.getv1());
                return 0;
            }
            case EBin.event_code_binary.BSRATE: // 0x105, "0x105", "Unknown Opcode"
            {
                this.fieldmap.walkMesh.BGI_simSetFrameRate((UInt32)this.getv1(), (Int16)this.getv2());
                return 0;
            }
            case EBin.event_code_binary.BSALGO: // 0x106, "0x106", "Unknown Opcode"
            {
                this.fieldmap.walkMesh.BGI_simSetAlgorithm((UInt32)this.getv1(), (UInt32)this.getv1());
                return 0;
            }
            case EBin.event_code_binary.BSDELTA: // 0x107, "0x107", "Unknown Opcode"
            {
                this.fieldmap.walkMesh.BGI_simSetDelta((UInt32)this.getv1(), (Int16)this.getv2(), (Int16)this.getv2());
                return 0;
            }
            case EBin.event_code_binary.BSAXIS: // 0x108, "0x108", "Unknown Opcode"
            {
                this.fieldmap.walkMesh.BGI_simSetAxis((UInt32)this.getv1(), (UInt32)this.getv1());
                return 0;
            }
            case EBin.event_code_binary.BAFRAME: // 0x10A, "0x10A", "Unknown Opcode"
            {
                this.fieldmap.walkMesh.BGI_animShowFrame((UInt32)this.getv1(), (UInt32)this.getv1());
                return 0;
            }
            // TODO: no more information from Hades Workshop
            case EBin.event_code_binary.PLAYER_EQUIP: // "SetCharacterEquipment" Change the piece of equipment of a player, using it from the player's inventory
            {
                CharacterId charId = this.chr2slot(this.getv3()); // character to (re-)equip.
                Int32 equipType = this.getv3(); // equipment type (0/1/2/3/4 for weapon/head/wrist/armor/accessory)
                RegularItem item = (RegularItem)this.getv3(); // item to equip
                PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(charId);
                if (player != null)
                {
                    player.equip.Change(equipType, item);
                    ff9feqp.FF9FEqp_UpdatePlayer(player);
                }
                return 0;
            }
            case EBin.event_code_binary.PLAYER_LEVEL: // "SetCharacterLevel" Change the character's level
            {
                CharacterId charId = this.chr2slot(this.getv3()); // character to re-level
                Int32 lvl = Mathf.Clamp(this.getv3(), 1, ff9level.LEVEL_COUNT); // level to reach
                PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(charId);
                if (player != null)
                    ff9play.FF9Play_Build(player, lvl, false, false);
                return 0;
            }
            case EBin.event_code_binary.PLAYER_EXP: // "SetCharacterExp" Set the character's experience to the given amount. The character's level cannot be lowered that way
            {
                CharacterId charId = this.chr2slot(this.getv3()); // character to consider
                UInt32 exp = Math.Min(this.getv3u(), 9999999u); // experience amount
                PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(charId);
                if (player != null)
                {
                    player.exp = exp;
                    while (player.level < ff9level.LEVEL_COUNT && exp >= ff9level.CharacterLevelUps[player.level].ExperienceToLevel)
                        ff9play.FF9Play_GrowLevel(player, player.level + 1);
                }
                return 0;
            }
            case EBin.event_code_binary.SHOP_ITEM: // "AddShopItem" Add or remove an item to be bought in a shop
            {
                Int32 shopId = this.getv3(); // shop ID
                RegularItem item = (RegularItem)this.getv3(); // item to add or remove
                Boolean add = this.getv3() != 0; // boolean add/remove
                if (!ff9buy.ShopItems.ContainsKey(shopId))
                    return 0;
                ShopItems shop = ff9buy.ShopItems[shopId];
                if (add)
                    shop.ItemIds.Add(item);
                else
                    shop.ItemIds.Remove(item);
                return 0;
            }
            case EBin.event_code_binary.SHOP_SYNTH: // "AddShopSynthesis" Add or remove a synthesis recipe in a shop
            {
                Int32 shopId = this.getv3(); // shop ID
                Int32 synthId = this.getv3(); // recipe to add or remove
                Boolean add = this.getv3() != 0; // boolean add/remove
                if (!ff9mix.SynthesisData.ContainsKey(synthId))
                    return 0;
                FF9MIX_DATA synth = ff9mix.SynthesisData[synthId];
                if (add)
                    synth.Shops.Add(shopId);
                else
                    synth.Shops.Remove(shopId);
                return 0;
            }
            case EBin.event_code_binary.TURN_OBJ_EX:
            {
                Actor turner = this.GetObj3() as Actor;
                PosObj target = this.GetObj3() as PosObj;
                Int32 tspeed = this.getv3();
                if (turner != null && target != null)
                {
                    Single angle = this.eBin.angleAsm(target.pos[0] - turner.pos[0], target.pos[2] - turner.pos[2]);
                    this.StartTurn(turner, angle, true, tspeed);
                }
                return 0;
            }
            case EBin.event_code_binary.AANIM_EX:
            {
                actor = this.GetObj3() as Actor;
                Int32 kind = this.getv3();
                UInt16 anim = (UInt16)(Int32)this.getv3();
                switch (kind)
                {
                    case 0: actor.idle = anim; break;
                    case 1: actor.walk = anim; break;
                    case 2: actor.run = anim; break;
                    case 3: actor.turnl = anim; break;
                    case 4: actor.turnr = anim; break;
                    case 5: actor.sleep = anim; break;
                    case 6: actor.jump = anim; break;
                }
                AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)anim));
                return 0;
            }
            case EBin.event_code_binary.VECTOR_CLEAR:
            {
                Int32 vectID = this.getv3();
                if (FF9StateSystem.EventState.gScriptVector.TryGetValue(vectID, out List<Int32> vect))
                    vect.Clear();
                return 0;
            }
            case EBin.event_code_binary.DICTIONARY_CLEAR:
            {
                Int32 dictID = this.getv3();
                if (FF9StateSystem.EventState.gScriptDictionary.TryGetValue(dictID, out Dictionary<Int32, Int32> dict))
                    dict.Clear();
                return 0;
            }
            default:
            {
                switch (this.gMode)
                {
                    case 1:
                        return this.DoEventCodeField(po, code);
                    case 2:
                        return this.DoEventCodeBattle(po, code);
                    case 3:
                        return this.DoEventCodeWorld(po, code);
                    default:
                        return 1;
                }
            }
        }
    }

    /*private static Boolean IsAlexandriaStageScene()
    {
        switch (FF9StateSystem.Common.FF9.fldMapNo)
        {
            case 62:
            case 63:
            case 3009:
            case 3010:
            //case 3011: // Leads to problems
                return true;
        }
        return false;
    }*/

    [DebuggerHidden]
    private IEnumerator DelayedCFLAG(Obj obj, Int32 cflag)
    {
        yield return new WaitForEndOfFrame();
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            yield return new WaitForEndOfFrame();
        obj.flags = (Byte)((obj.flags & -64) | (cflag & 63));
    }

    private void StartTurn(Actor actor, Single a, Boolean opt, Int32 tspeed)
    {
        Vector3 rotAngle = actor.rotAngle;
        Single angle360 = EventEngineUtils.ClampAngle(rotAngle.y);
        actor.turnRot = EventEngineUtils.ClampAngle(a);
        a -= angle360;
        if (opt)
        {
            if ((Double)a > 180.0 && !EventEngineUtils.nearlyEqual(a, 180f))
                a -= 360f;
            else if ((Double)a < -180.0 && !EventEngineUtils.nearlyEqual(a, -180f))
                a += 360f;
        }
        if (EventEngineUtils.nearlyEqual(a, 0.0f))
            return;
        actor.lastAnim = actor.anim;
        actor.flags |= (Byte)128;
        actor.inFrame = (Byte)0;
        actor.outFrame = Byte.MaxValue;
        actor.trot = angle360;
        if (tspeed == 0)
            tspeed = 16;
        Int32 num2;
        if ((Double)a < 0.0)
        {
            Int32 charAnimFrame = EventEngineUtils.GetCharAnimFrame(actor.go, (Int32)actor.turnl);
            num2 = (charAnimFrame << 4) / tspeed;
            if (-(Double)a < 90.0)
            {
                Single num3 = (Single)num2 * (Single)(-(Double)a / 90.0);
                num2 = (Int32)((Double)num2 * (-(Double)a / 90.0));
            }
            if (num2 > 5)
            {
                if (IsCaseShiftWhenTurn(actor))
                {
                    Byte num3 = (Byte)EventEngineUtils.GetCharAnimFrame(actor.go, (Int32)actor.turnl);
                    Quaternion localRotation = actor.go.transform.localRotation;
                    Vector3 vector3_2 = new Vector3(actor.go.transform.eulerAngles.x, 90f, actor.go.transform.eulerAngles.z);
                    actor.go.transform.eulerAngles = vector3_2;
                    Vector3 rootBonePos1 = this.GetRootBonePos(actor, (Int32)actor.turnl, charAnimFrame);
                    vector3_2.y = 0.0f;
                    actor.go.transform.eulerAngles = vector3_2;
                    Vector3 rootBonePos2 = this.GetRootBonePos(actor, (Int32)actor.idle, 0);
                    actor.offsetTurn = rootBonePos2 - rootBonePos1;
                    actor.offsetTurn = Quaternion.Euler(0.0f, actor.turnRot, 0.0f) * actor.offsetTurn;
                    actor.go.transform.localRotation = localRotation;
                }
                actor.turnAdd = (a + 90f) / (Single)num2;
                actor.animFlag = (Byte)0;
                this.ExecAnim(actor, (Int32)actor.turnl);
                actor.aspeed = (Byte)((charAnimFrame << 4) / num2);
            }
            else
            {
                if ((Double)actor.turnRot > 180.0)
                    actor.turnRot -= 360f;
                else if ((Double)actor.turnRot < -180.0)
                    actor.turnRot += 360f;
                actor.turnAdd = 32766f;
                rotAngle.y = angle360 + ((Double)actor.turnRot < (Double)angle360 ? 0.0f : 360f);
                actor.rotAngle[1] = rotAngle.y;
            }
        }
        else
        {
            Int32 charAnimFrame = EventEngineUtils.GetCharAnimFrame(actor.go, (Int32)actor.turnr);
            num2 = (charAnimFrame << 4) / tspeed;
            if ((Double)a < 90.0)
            {
                Single num3 = (Single)num2 * (a / 90f);
                num2 = (Int32)((Double)num2 * ((Double)a / 90.0));
            }
            if (num2 > 5)
            {
                if (IsCaseShiftWhenTurn(actor))
                {
                    Quaternion localRotation = actor.go.transform.localRotation;
                    Vector3 vector3_2 = new Vector3(actor.go.transform.eulerAngles.x, -90f, actor.go.transform.eulerAngles.z);
                    actor.go.transform.eulerAngles = vector3_2;
                    Vector3 rootBonePos1 = this.GetRootBonePos(actor, (Int32)actor.turnr, charAnimFrame);
                    vector3_2.y = 0.0f;
                    actor.go.transform.eulerAngles = vector3_2;
                    Vector3 rootBonePos2 = this.GetRootBonePos(actor, (Int32)actor.idle, 0);
                    actor.offsetTurn = rootBonePos2 - rootBonePos1;
                    actor.offsetTurn = Quaternion.Euler(0.0f, actor.turnRot, 0.0f) * actor.offsetTurn;
                    actor.go.transform.localRotation = localRotation;
                }
                actor.turnAdd = (a - 90f) / (Single)num2;
                actor.animFlag = (Byte)0;
                this.ExecAnim(actor, (Int32)actor.turnr);
                actor.aspeed = (Byte)((charAnimFrame << 4) / num2);
            }
            else
            {
                if ((Double)actor.turnRot > 180.0)
                    actor.turnRot -= 360f;
                else if ((Double)actor.turnRot < -180.0)
                    actor.turnRot += 360f;
                actor.turnAdd = (Single)Int16.MaxValue;
                rotAngle.y = angle360 - ((Double)actor.turnRot > (Double)angle360 ? 0.0f : 360f);
                actor.rotAngle[1] = rotAngle.y;
            }
        }
        if (num2 < 1)
            num2 = 1;
        actor.trotAdd = a / (Single)num2;
    }

    private Vector3 GetRootBonePos(Actor actor, Int32 animID, Int32 frameIndex)
    {
        String animation = FF9DBAll.AnimationDB.GetValue(animID);
        Animation component = actor.fieldMapActorController.GetComponent<Animation>();
        component.Play(animation);
        Int32 num = (Int32)(Byte)EventEngineUtils.GetCharAnimFrame(actor.go, animID);
        component[animation].time = (Single)frameIndex;
        component.Sample();
        return actor.fieldMapActorController.transform.FindChild("bone000").position;
    }

    private static Boolean IsCaseShiftWhenTurn(Actor currentActor)
    {
        Int32 num = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
        PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
        Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
        return (num == 50 || num == 52 || num == 60) && (Int32)currentActor.model == 5464 || (num == 66 && (Int32)currentActor.model == 5501 || num == 562 && varManually == 99 && (Int32)currentActor.model == 5476) || (num >= 1051 && num <= 1059 || num >= 1103 && num <= 1109 && (Int32)currentActor.model == 269);
    }

    private Boolean requestAcceptable(Obj p, Int32 lv)
    {
        if (p != null)
            return lv < (Int32)p.level;
        return false;
    }

    private Obj GetObj1()
    {
        return this.GetObjUID(this.getv1());
    }

    private Obj GetObj3()
    {
        return this.GetObjUID(this.getv3());
    }

    private void FF9FieldDiscRequest(Byte disc_id, UInt16 map_id)
    {
        //this._ff9fieldDisc.disc_id = disc_id;
        //this._ff9fieldDisc.cdType = (byte)(1U << (int)disc_id);
        this._ff9fieldDisc.FieldMapNo = (Int16)map_id;
        //this._ff9fieldDisc.FieldLocNo = (short)-1;
        FF9StateFieldSystem stateFieldSystem = FF9StateSystem.Field.FF9Field;
        FF9StateSystem instance = PersistenSingleton<FF9StateSystem>.Instance;
        stateFieldSystem.attr |= 1048576U;
        instance.attr |= 8U;
    }

    internal void SetActorPosition(PosObj po, Single x, Single y, Single z)
    {
        po.pos[0] = po.lastx = x;
        po.pos[1] = po.lasty = y;
        po.pos[2] = po.lastz = z;
        if (this.gMode == 1)
        {
            ((Actor)po).fieldMapActorController?.SetPosition(new Vector3(po.pos[0], po.pos[1], po.pos[2]), true, true);
        }
        else if (this.gMode == 3)
        {
            //if ((Int32)po.uid != (Int32)this._context.controlUID)
            //    ;
            ((Actor)po).wmActor.SetPosition(po.pos[0], po.pos[1], po.pos[2]);
            if (po.index < 3 || po.index > 7)
                return;
            Int32 posX = (Int32)po.pos[0];
            Int32 posY = (Int32)po.pos[1];
            Int32 posZ = (Int32)po.pos[2];
            ff9.w_movementChrVerifyValidCastPosition(ref posX, ref posY, ref posZ);
            po.pos[0] = posX;
            po.pos[1] = posY;
            po.pos[2] = posZ;
            ((Actor)po).wmActor.SetPosition(po.pos[0], po.pos[1], po.pos[2]);
        }
    }
}
