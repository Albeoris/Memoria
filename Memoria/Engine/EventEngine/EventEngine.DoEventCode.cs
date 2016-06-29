using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public partial class EventEngine
{
    public int DoEventCode()
    {
        Actor actor1 = (Actor)null;
        GameObject gameObject = (GameObject)null;
        PosObj po = (PosObj)null;
        FieldMapActorController fmac = (FieldMapActorController)null;
        if ((int)this.gCur.cid == 4)
        {
            actor1 = (Actor)this.gCur;
            gameObject = actor1.go;
            po = (PosObj)this.gCur;
        }
        this._lastIP = this.gExec.ip;
        int code = this.geti();
        this.gArgFlag = this.geti();
        this.gArgUsed = 0;
        EBin.event_code_binary eventCodeBinary = (EBin.event_code_binary)code;
        Vector3 eulerAngles1;
        int num1;
        int num2;
        int num3;

        //string stttt = eventCodeBinary.ToString();
        //if (!stttt.StartsWith("BG"))
        //    Memoria.Log.Message(stttt);

        switch (eventCodeBinary)
        {
            case EBin.event_code_binary.NOP:
                label_832:
                return 1;
            case EBin.event_code_binary.NEW:
                NewThread(this.gArgFlag, this.geti());
                this.gArgUsed = 1;
                return 0;
            case EBin.event_code_binary.NEW2:
                Quad quad1 = new Quad(this.gArgFlag, this.geti());
                this.gArgUsed = 1;
                return 0;
            case EBin.event_code_binary.NEW3:
                int sid1 = this.gArgFlag;
                int uid1 = this.geti();
                if (sid1 >= 251 && sid1 < (int)byte.MaxValue)
                    sid1 = (int)this._context.partyUID[sid1 - 251];
                Actor actor2 = new Actor(sid1, uid1, EventEngine.sizeOfActor);
                if (this.gMode == 3)
                    Singleton<WMWorld>.Instance.addWMActorOnly(actor2);
                if (this.gMode == 1)
                    this.turnOffTriManually(sid1);
                this.gArgUsed = 1;
                return 0;
            case EBin.event_code_binary.REQ:
                int level = this.getv1();
                Obj p1 = this.GetObj1();
                int tag1 = this.geti();
                this.Request(p1, level, tag1, false);
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 900 && p1 != null && (level == 2 && tag1 == 11) && (int)p1.uid == 14)
                    this.fieldmap.walkMesh.BGI_triSetActive(62U, 1U);
                return 0;
            case EBin.event_code_binary.REQSW:
                int num4 = this.getv1();
                Obj p2 = this.GetObj1();
                int tag2 = this.geti();
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2803 && p2 != null && (tag2 == 18 && (int)p2.uid == 20))
                {
                    this.fieldmap.walkMesh.BGI_triSetActive(105U, 1U);
                    this.fieldmap.walkMesh.BGI_triSetActive(106U, 1U);
                }
                if (this.requestAcceptable(p2, num4))
                {
                    this.Request(p2, num4, tag2, false);
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 262)
                    {
                        this._geoTexAnim = this.GetObjUID(12).go.GetComponent<GeoTexAnim>();
                        if ((int)p2.sid == 10 && tag2 == 12)
                        {
                            this._geoTexAnim.geoTexAnimStop(2);
                            this._geoTexAnim.geoTexAnimPlay(0);
                        }
                        else if ((int)p2.sid == 12 && tag2 == 20)
                            this._geoTexAnim.geoTexAnimPlay(2);
                    }
                    return 0;
                }
                this.stay();
                return 1;
            case EBin.event_code_binary.REQEW:
                int num5 = this.getv1();
                Obj p3 = this.GetObj1();
                int tag3 = this.geti();
                if (this.gMode == 3)
                {
                    if (tag3 == 19)
                    {
                        Singleton<WMTweaker>.Instance.FixTypeCamEyeYTarget = 1419;
                        Singleton<WMTweaker>.Instance.FixTypeCamAimYTarget = 996;
                    }
                    else if (tag3 == 21)
                    {
                        Singleton<WMTweaker>.Instance.FixTypeCamEyeYTarget = 558;
                        Singleton<WMTweaker>.Instance.FixTypeCamAimYTarget = 312;
                    }
                }
                if (this.requestAcceptable(p3, num5))
                    this.Request(p3, num5, tag3, true);
                else
                    this.stay();
                return 1;
            case EBin.event_code_binary.REPLY:
                this.Request(this.getSender(this.gExec), this.getv1(), this.geti(), false);
                return 0;
            case EBin.event_code_binary.REPLYSW:
                int num6 = this.getv1();
                int tag4 = this.geti();
                if (this.requestAcceptable(this.getSender(this.gExec), num6))
                {
                    this.Request(this.getSender(this.gExec), num6, tag4, false);
                    return 0;
                }
                this.stay();
                return 1;
            case EBin.event_code_binary.REPLYEW:
                int num7 = this.getv1();
                int tag5 = this.geti();
                this.getSender(this.gExec);
                if (this.requestAcceptable(this.getSender(this.gExec), num7))
                    this.Request(this.getSender(this.gExec), num7, tag5, true);
                else
                    this.stay();
                return 1;
            case EBin.event_code_binary.SONGFLAG:
                if (this.getv1() != 0)
                    FF9StateSystem.Common.FF9.btl_flag |= (byte)8;
                else
                    FF9StateSystem.Common.FF9.btl_flag &= (byte)247;
                return 0;
            case EBin.event_code_binary.POS:
            case EBin.event_code_binary.DPOS:
                if (eventCodeBinary == EBin.event_code_binary.DPOS)
                    po = (PosObj)this.GetObj1();
                int num8 = this.getv2();
                int num9 = this.getv2();
                int num10 = this.gMode != 1 || (int)po.model == (int)ushort.MaxValue ? 0 : 1;
                if (num10 == 1 && po != null)
                {
                    FieldMapActorController component = po.go.GetComponent<FieldMapActorController>();
                    if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.walkMesh != null)
                    {
                        component.walkMesh.BGI_charSetActive(component, 1U);
                        if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2050 && (int)po.sid == 5)
                            component.walkMesh.BGI_charSetActive(component, 0U);
                        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2917 && (int)po.sid == 4)
                        {
                            if (num8 == 0 && num9 == -1787)
                                num8 = -15;
                        }
                        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 450 && (int)po.sid == 3 && (num8 == 363 && num9 == 88))
                            component.walkMesh.BGI_triSetActive(24U, 0U);
                    }
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1421 && (int)po.sid == 5)
                    {
                        if (num8 == 1510 && (num9 == -2331 || num9 == -2231))
                        {
                            if (num9 == -2331)
                                num9 = -2231;
                            this.fieldmap.walkMesh.BGI_triSetActive(109U, 0U);
                            this.fieldmap.walkMesh.BGI_triSetActive(110U, 0U);
                        }
                        else if (num8 == 34 && num9 == -598)
                        {
                            this.fieldmap.walkMesh.BGI_triSetActive(109U, 1U);
                            this.fieldmap.walkMesh.BGI_triSetActive(110U, 1U);
                        }
                    }
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 563 && (int)po.sid == 16)
                    {
                        if (num8 == -1614)
                            num8 = -1635;
                    }
                    else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1310 && (int)po.sid == 12)
                    {
                        if (num8 == -1614)
                            num8 = -1635;
                    }
                    else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2110 && (int)po.sid == 9)
                    {
                        if (num8 == -1614)
                            num8 = -1635;
                    }
                    else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 572 && (int)po.sid == 16 && num8 == -1750)
                        num8 = -1765;
                }
                this.SetActorPosition(po, (float)num8, this.POS_COMMAND_DEFAULTY, (float)num9);
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2050 && (int)po.sid == 5 && (num10 == 1 && po != null))
                {
                    FieldMapActorController component = po.go.GetComponent<FieldMapActorController>();
                    if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.walkMesh != null)
                        component.walkMesh.BGI_charSetActive(component, 1U);
                }
                if ((int)po.cid == 4)
                    this.clrdist((Actor)po);
                this._posUsed = true;
                return 0;
            case EBin.event_code_binary.BGVPORT:
                this.fieldmap.EBG_cameraSetViewport((uint)this.getv1(), (short)this.getv2(), (short)this.getv2(), (short)this.getv2(), (short)this.getv2());
                return 0;
            case EBin.event_code_binary.MES:
                this.gCur.winnum = (byte)this.getv1();
                int flags1 = this.getv1();
                this.SetFollow(this.gCur, (int)this.gCur.winnum, flags1);
                int index1 = this.getv2();
                //Memoria.Log.Message($"MES: Map: ${(int)FF9StateSystem.Common.FF9.fldMapNo}, v1: {this.gCur.winnum}, flag: {flags1}, index: {index1}");
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1757 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6740 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 30)
                {
                    //Memoria.Log.Message($"MES: stay");
                    this.stay();
                    return 1;
                }
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1060)
                {
                    //Memoria.Log.Message($"MES: 1060");
                    string symbol = Localization.GetSymbol();
                    Dictionary<int, int> dictionary = (Dictionary<int, int>)null;
                    if (symbol == "JP")
                    {
                        if (index1 == 271)
                        {
                            HonoBehaviorSystem.FrameSkipEnabled = true;
                            HonoBehaviorSystem.TargetFrameTime = 0.03333334f;
                        }
                        else if (index1 == 272)
                            HonoBehaviorSystem.FrameSkipEnabled = false;
                    }
                    else if (index1 == 262)
                    {
                        HonoBehaviorSystem.TargetFrameTime = 0.03333334f;
                        HonoBehaviorSystem.FrameSkipEnabled = true;
                    }
                    else if (index1 == 264)
                        HonoBehaviorSystem.FrameSkipEnabled = false;
                    if (symbol == "ES" || symbol == "FR")
                        dictionary = this._mesIdES_FR;
                    else if (symbol == "GR")
                        dictionary = this._mesIdGR;
                    else if (symbol == "IT")
                        dictionary = this._mesIdIT;
                    if (dictionary != null && dictionary.ContainsKey(index1))
                        index1 = dictionary[index1];
                }
                this.eTb.NewMesWin(index1, (int)this.gCur.winnum, flags1, !this.isPosObj(this.gCur) ? (PosObj)null : (PosObj)this.gCur);
                this.gCur.wait = (byte)254;
                return 1;
            case EBin.event_code_binary.MESN:
                this.gCur.winnum = (byte)this.getv1();
                int flags2 = this.getv1();
                this.SetFollow(this.gCur, (int)this.gCur.winnum, flags2);
                int index2 = this.getv2();
                //Memoria.Log.Message($"MESN: Map: {(int)FF9StateSystem.Common.FF9.fldMapNo}, v1: {this.gCur.winnum}, flag: {flags2}, index: {index2}");
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1757 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6740 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 30)
                {
                    //Memoria.Log.Message($"MESN: stay");
                    this.stay();
                    return 1;
                }
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1060)
                {
                    //Memoria.Log.Message($"MESN: 1060");
                    string symbol = Localization.GetSymbol();
                    Dictionary<int, int> dictionary = (Dictionary<int, int>)null;
                    if (symbol == "JP")
                    {
                        if (index2 == 271)
                        {
                            HonoBehaviorSystem.TargetFrameTime = 0.03333334f;
                            HonoBehaviorSystem.FrameSkipEnabled = true;
                        }
                        else if (index2 == 272)
                            HonoBehaviorSystem.FrameSkipEnabled = false;
                    }
                    else if (index2 == 262)
                    {
                        HonoBehaviorSystem.TargetFrameTime = 0.03333334f;
                        HonoBehaviorSystem.FrameSkipEnabled = true;
                    }
                    else if (index2 == 264)
                        HonoBehaviorSystem.FrameSkipEnabled = false;
                    if (symbol == "ES" || symbol == "FR")
                        dictionary = this._mesIdES_FR;
                    else if (symbol == "GR")
                        dictionary = this._mesIdGR;
                    else if (symbol == "IT")
                        dictionary = this._mesIdIT;
                    if (dictionary != null && dictionary.ContainsKey(index2))
                        index2 = dictionary[index2];
                }
                PersistenSingleton<CheatingManager>.Instance.CheatJumpingRobe();
                this.eTb.NewMesWin(index2, (int)this.gCur.winnum, flags2, !this.isPosObj(this.gCur) ? (PosObj)null : (PosObj)this.gCur);
                return 0;
            case EBin.event_code_binary.CLOSE:
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 62 || (int)FF9StateSystem.Common.FF9.fldMapNo == 63)
                {
                    goto case EBin.event_code_binary.WAITMES;
                }
                else
                {
                    var v1 = this.getv1();
                    //Memoria.Log.Message($"CLOSE: " + v1);
                    this.eTb.DisposWindowByID(v1);
                    return 0;
                }
            case EBin.event_code_binary.MOVE:
                int num11 = this.getv2();
                int num12 = this.getv2();
                bool flag1 = false;
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2954 && (int)po.sid == 4)
                {
                    int num13 = 0;
                    SettingUtils.FieldMapSettings fieldMapSettings = SettingUtils.fieldMapSettings;
                    if (num11 == -2181 && num12 == 14842)
                        num13 = 0;
                    else if (num11 == -2407 && num12 == 14508)
                        num13 = 1;
                    else if (num11 == -1146 && num12 == 13438)
                        num13 = 2;
                    else if (num11 == -1159 && num12 == 13130)
                    {
                        num13 = 3;
                        num11 = -1275;
                        num12 = 13130;
                    }
                    else if (num11 == -3644 && num12 == 11849)
                    {
                        num13 = 4;
                        num11 = -3750;
                        num12 = 11849;
                    }
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 108 && (int)po.sid == 2)
                {
                    if (num11 == -111 && num12 == -210)
                    {
                        num11 = -150;
                        num12 = -270;
                    }
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1857 && (int)po.sid == 3)
                {
                    if (num11 == -111 && num12 == -210)
                    {
                        num11 = -150;
                        num12 = -270;
                    }
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2101 && (int)po.sid == 2)
                {
                    if (num11 == 781 && num12 == 1587)
                    {
                        num11 = 805;
                        num12 = 1564;
                    }
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 401 && (int)po.sid == 2)
                {
                    if (num11 == -1869 && num12 == -783)
                        num11 = -1782;
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 64)
                {
                    if (PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 1600 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 327 && (num11 == 1111 && num12 == -14400))
                    {
                        num11 = 1261;
                        num12 = -14550;
                    }
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 307 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 2520 && ((int)po.sid == 12 || (int)po.sid == 14))
                {
                    if (num11 == 636 && num12 == -1359)
                        num11 = 781;
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 563 && (int)po.sid == 16)
                {
                    if (num11 == -1614)
                        num11 = -1635;
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1310 && (int)po.sid == 12)
                {
                    if (num11 == -1614)
                        num11 = -1635;
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2110 && (int)po.sid == 9)
                {
                    if (num11 == -1614)
                        num11 = -1635;
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 572 && (int)po.sid == 16)
                {
                    if (num11 == -1750)
                        num11 = -1765;
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1601 && (int)po.uid == 130)
                {
                    if (num11 == -1705 && num12 == -1233)
                    {
                        num11 = -1680;
                        num12 = -1140;
                    }
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 915)
                {
                    if ((int)po.sid == 3)
                    {
                        if (num11 == -10336 && num12 == -7750)
                        {
                            if ((double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                                flag1 = true;
                        }
                        else if (num11 == -8746 && num12 == -6516 && (double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                            flag1 = true;
                    }
                    else if ((int)po.sid == 4)
                    {
                        if (num11 == -8746 && num12 == -6516)
                        {
                            if ((double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                                flag1 = true;
                        }
                        else if (num11 == -8967 && num12 == -6173)
                        {
                            if ((double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                                flag1 = true;
                        }
                        else if (num11 == -10498 && num12 == 2678 && (double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                            flag1 = true;
                    }
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1915)
                {
                    if ((int)po.sid == 3)
                    {
                        if (num11 == -10336 && num12 == -7750)
                        {
                            if ((double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                                flag1 = true;
                        }
                        else if (num11 == -8746 && num12 == -6516 && (double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                            flag1 = true;
                    }
                    else if ((int)po.sid == 4)
                    {
                        if (num11 == -8967 && num12 == -6173)
                        {
                            if ((double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                                flag1 = true;
                        }
                        else if (num11 == -10498 && num12 == 2678 && (double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                            flag1 = true;
                    }
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 550)
                {
                    if ((int)po.sid == 14 && num11 == 348 && num12 == -2500 && (double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                        flag1 = true;
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 101)
                {
                    if ((int)po.sid == 7 && num11 == -4000 && num12 == -400 && (double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                        flag1 = true;
                    if ((int)po.sid == 8 && num11 == -4000 && num12 == -200 && (double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((float)num11, (float)num12)).sqrMagnitude > 100.0)
                        flag1 = true;
                }
                if ((int)po.sid != 15)
                ;
                bool flag2 = this.MoveToward_mixed((float)num11, 0.0f, (float)num12, 0, (PosObj)null);
                eulerAngles1 = po.go.transform.localRotation.eulerAngles;
                if (flag2)
                    this.stay();
                else if (flag1)
                    this.stay();
                return 1;
            case EBin.event_code_binary.MOVA:
                PosObj flagsPosObj = (PosObj)this.GetObj1();
                if (this.MoveToward_mixed(flagsPosObj.pos[0], flagsPosObj.pos[1], flagsPosObj.pos[2], 0, flagsPosObj))
                    this.stay();
                this.gArgUsed = 1;
                return 1;
            case EBin.event_code_binary.CLRDIST:
                actor1.loopCount = byte.MaxValue;
                this.clrdist(actor1);
                return 0;
            case EBin.event_code_binary.MSPEED:
                byte num14 = (byte)this.getv1();
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 3010)
                {
                    string symbol = Localization.GetSymbol();
                    if (!(symbol != "US") || !(symbol != "JP"))
                    {
                        if (symbol == "US" && (int)actor1.sid == 17)
                        {
                            if ((int)num14 == 15)
                                num14 = (byte)20;
                            else if ((int)num14 == 23)
                                num14 = (byte)25;
                        }
                        if (symbol == "JP" && (int)actor1.sid == 16)
                        {
                            if ((int)num14 == 15)
                                num14 = (byte)20;
                            else if ((int)num14 == 23)
                                num14 = (byte)25;
                        }
                    }
                }
                actor1.speed = num14;
                return 0;
            case EBin.event_code_binary.BGIMASK:
                this.BGI_systemSetAttributeMask((byte)this.getv1());
                return 0;
            case EBin.event_code_binary.FMV:
                Singleton<fldfmv>.Instance.FF9FieldFMVDispatch(this.getv1(), this.getv2(), this.getv1());
                return 0;
            case EBin.event_code_binary.QUAD:
                int index3 = 0;
                ((Quad)this.gCur).n = this.getv1();
                int num15 = ((Quad)this.gCur).n;
                while (num15 != 0)
                {
                    QuadPos quadPos = ((Quad)this.gCur).q[index3];
                    quadPos.X = (short)this.getv2();
                    quadPos.Z = (short)this.getv2();
                    if (((int)FF9StateSystem.Common.FF9.fldMapNo == 1608 || (int)FF9StateSystem.Common.FF9.fldMapNo == 1707) && (int)quadPos.Z == -257)
                        quadPos.Z = (short)-157;
                    --num15;
                    ++index3;
                }
                if ((int)this.gCur.sid != 4)
                ;
                return 0;
            case EBin.event_code_binary.ENCOUNT:
                this.getv1(); // rush_type
                this._ff9.btlSubMapNo = (sbyte)-1;
                int num16 = this.getv2();
                this._ff9.steiner_state = (byte)(num16 >> 15 & 1);
                this.SetBattleScene(num16 & (int)short.MaxValue);
                FF9StateSystem.Battle.isRandomEncounter = false;
                return 3;
            case EBin.event_code_binary.MAPJUMP:
                this.SetNextMap(this.getv2());
                return 4;
            case EBin.event_code_binary.CC:
                Actor activeActorByUid = this.getActiveActorByUID((int)this._context.controlUID);
                if (activeActorByUid != null && this.gMode == 1)
                {
                    activeActorByUid.fieldMapActorController.isPlayer = false;
                    activeActorByUid.fieldMapActorController.gameObject.name = "obj" + (object)activeActorByUid.uid;
                }
                this._context.controlUID = this.gExec.uid;
                return 0;
            case EBin.event_code_binary.UCOFF:
                this._context.usercontrol = (byte)0;
                EIcon.SetHereIcon(0);
                this.eTb.gMesCount = this.gAnimCount = 0;
                PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
                if (this.gMode == 3)
                    UIManager.World.SetMinimapPressable(false);
                else if (this.gMode == 1)
                {
                    Obj objUid = this.GetObjUID(250);
                    if (objUid != null && (int)objUid.cid == 4)
                        ((Actor)objUid).fieldMapActorController.ClearMoveTargetAndPath();
                }
                if (!EMinigame.CheckChocoboVirtual())
                    PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (System.Action)null);
                return 0;
            case EBin.event_code_binary.UCON:
                this._context.usercontrol = (byte)1;
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
            case EBin.event_code_binary.MODEL:
                po.model = (ushort)this.getv2();
                this.gExec.flags |= (byte)1;
                po.eye = (short)(-4 * this.getv1());
                if (this.gMode == 1)
                {
                    string str = FF9BattleDB.GEO[(int)po.model];
                    po.go = ModelFactory.CreateModel(str, false);
                    GeoTexAnim.addTexAnim(po.go, str);
                    if (ModelFactory.garnetShortHairTable.Contains(str))
                    {
                        po.garnet = true;
                        ushort uint16 = BitConverter.ToUInt16(FF9StateSystem.EventState.gEventGlobal, 0);
                        po.shortHair = (int)uint16 >= 10300;
                    }
                    if ((UnityEngine.Object)po.go != (UnityEngine.Object)null)
                    {
                        int length = 0;
                        IEnumerator enumerator = po.go.transform.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                if (((UnityEngine.Object)enumerator.Current).name.Contains("mesh"))
                                    ++length;
                            }
                        }
                        finally
                        {
                            IDisposable disposable = enumerator as IDisposable;
                            if (disposable != null)
                                disposable.Dispose();
                        }
                        if (po.garnet)
                            ++length;
                        po.meshIsRendering = new bool[length];
                        for (int index4 = 0; index4 < length; ++index4)
                            po.meshIsRendering[index4] = true;
                        FF9Char ff9Char = new FF9Char();
                        ff9Char.geo = po.go;
                        ff9Char.evt = po;
                        if (FF9StateSystem.Common.FF9.charArray.ContainsKey((int)po.uid))
                            return 0;
                        FF9StateSystem.Common.FF9.charArray.Add((int)po.uid, ff9Char);
                        FF9FieldCharState ff9FieldCharState = new FF9FieldCharState();
                        FF9StateSystem.Field.FF9Field.loc.map.charStateArray.Add((int)po.uid, ff9FieldCharState);
                        FF9StateSystem.Field.FF9Field.loc.map.shadowArray.Add((int)po.uid, new FF9Shadow());
                        Obj obj = (Obj)po;
                        obj.go.name = "obj" + (object)po.uid;
                        this.fieldmap.AddFieldChar(obj.go, po.posField, po.rotField, false, (Actor)obj, false);
                    }
                }
                else if (this.gMode == 3)
                {
                    po.go = ModelFactory.CreateModel(FF9BattleDB.GEO[(int)po.model], false);
                    Singleton<WMWorld>.Instance.addGameObjectToWMActor(po.go, ((Actor)po).wmActor);
                }
                return 0;
            case EBin.event_code_binary.AIDLE:
                actor1.idle = (ushort)this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB[(int)actor1.idle]);
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2365 && (int)actor1.uid == 14 && (int)actor1.idle == 11611)
                {
                    this._geoTexAnim = actor1.go.GetComponent<GeoTexAnim>();
                    this._geoTexAnim.geoTexAnimStop(2);
                    this._geoTexAnim.geoTexAnimPlay(0);
                }
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2657 && (int)actor1.uid == 7)
                {
                    this._geoTexAnim = actor1.go.GetComponent<GeoTexAnim>();
                    if ((int)actor1.idle == 1044)
                    {
                        this._geoTexAnim.geoTexAnimStop(2);
                        this._geoTexAnim.geoTexAnimPlay(0);
                    }
                    else if ((int)actor1.idle == 816)
                        this._geoTexAnim.geoTexAnimPlay(2);
                }
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1605 && (int)actor1.uid == 18)
                {
                    this._geoTexAnim = actor1.go.GetComponent<GeoTexAnim>();
                    if ((int)actor1.idle == 7503)
                        this._geoTexAnim.geoTexAnimPlay(2);
                }
                return 0;
            case EBin.event_code_binary.AWALK:
                actor1.walk = (ushort)this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB[(int)actor1.walk]);
                return 0;
            case EBin.event_code_binary.ARUN:
                actor1.run = (ushort)this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB[(int)actor1.run]);
                return 0;
            case EBin.event_code_binary.DIRE:
            case EBin.event_code_binary.DDIR:
                if (eventCodeBinary == EBin.event_code_binary.DDIR)
                    po = (PosObj)this.GetObj1();
                int num17 = this.getv1();
                if (po == null || (UnityEngine.Object)po.go == (UnityEngine.Object)null)
                    return 0;
                if (this.gMode == 1)
                {
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 504 && (int)po.sid == 15 && (this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 4 && num17 == 240))
                        num17 = 128;
                    Vector3 eulerAngles2 = po.go.transform.localRotation.eulerAngles;
                    eulerAngles2.y = EventEngineUtils.ConvertFixedPointAngleToDegree((short)(num17 << 4));
                    po.rotAngle[1] = eulerAngles2.y;
                }
                else if (this.gMode == 3)
                {
                    Vector3 rot = ((Actor)po).wmActor.rot;
                    rot.y = EventEngineUtils.ConvertFixedPointAngleToDegree((short)(num17 << 4));
                    ((Actor)po).wmActor.rot = rot;
                }
                return 0;
            case EBin.event_code_binary.ROTXZ:
                int num18 = (int)(short)(this.getv1() << 4);
                int num19 = (int)(short)(this.getv1() << 4);
                float num20 = EventEngineUtils.ClampAngle(EventEngineUtils.ConvertFixedPointAngleToDegree((short)num18));
                float num21 = EventEngineUtils.ClampAngle(EventEngineUtils.ConvertFixedPointAngleToDegree((short)num19));
                po.rotAngle[0] = num20;
                po.rotAngle[2] = num21;
                return 0;
            case EBin.event_code_binary.BTLCMD:
                switch (this.gExec.level)
                {
                    case 0:
                        btl_cmd.SetEnemyCommand((ushort)this.GetSysList(1), (ushort)this.GetSysList(0), 54U, (uint)this.getv1());
                        break;
                    case 1:
                        btl_cmd.SetEnemyCommand((ushort)this.GetSysList(1), (ushort)this.GetSysList(0), 53U, (uint)this.getv1());
                        break;
                    case 3:
                        this.gExec.btlchk = (byte)0;
                        btl_cmd.SetEnemyCommand((ushort)this.GetSysList(1), (ushort)this.GetSysList(0), 47U, (uint)this.getv1());
                        break;
                }
                return 0;
            case EBin.event_code_binary.MESHSHOW:
                PosObj posObj1 = (PosObj)this.GetObj1();
                int mesh1 = this.getv1();
                if (posObj1 != null)
                    posObj1.geoMeshShow(mesh1);
                return 0;
            case EBin.event_code_binary.MESHHIDE:
                PosObj posObj2 = (PosObj)this.GetObj1();
                int mesh2 = this.getv1();
                if (posObj2 != null)
                    posObj2.geoMeshHide(mesh2);
                return 0;
            case EBin.event_code_binary.OBJINDEX:
                this.gExec.index = (byte)this.getv1();
                return 0;
            case EBin.event_code_binary.ENCSCENE:
                this._enCountData.pattern = (byte)this.getv1();
                this._enCountData.scene[0] = (ushort)this.getv2();
                this._enCountData.scene[1] = (ushort)this.getv2();
                this._enCountData.scene[2] = (ushort)this.getv2();
                this._enCountData.scene[3] = (ushort)this.getv2();
                return 0;
            case EBin.event_code_binary.AFRAME:
                actor1.inFrame = (byte)this.getv1();
                actor1.outFrame = (byte)this.getv1();
                return 0;
            case EBin.event_code_binary.ASPEED:
                if ((int)FF9StateSystem.Common.FF9.fldMapNo >= 3009 && (int)FF9StateSystem.Common.FF9.fldMapNo <= 3011)
                {
                    this.getv1();
                    return 0;
                }
                actor1.aspeed0 = (byte)this.getv1();
                return 0;
            case EBin.event_code_binary.AMODE:
                int num22 = this.getv1() << 3 & (EventEngine.afHold | EventEngine.afLoop | EventEngine.afPalindrome);
                actor1.animFlag = (byte)num22;
                actor1.loopCount = (byte)this.getv1();
                return 0;
            case EBin.event_code_binary.ANIM:
                int anim = this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB[anim]);
                if (this.gMode == 1)
                {
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 800 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 3740 && anim == 13158)
                        actor1.inFrame = (byte)19;
                    this.ExecAnim(actor1, anim);
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2934 && (int)actor1.uid == 2 && anim == 12429)
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
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1605 && (int)actor1.uid == 18)
                    {
                        this._geoTexAnim = actor1.go.GetComponent<GeoTexAnim>();
                        if (anim == 11958)
                        {
                            this._geoTexAnim.geoTexAnimStop(2);
                            this._geoTexAnim.geoTexAnimPlay(0);
                        }
                    }
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 307 && (int)actor1.uid == 13)
                    {
                        this._geoTexAnim = actor1.go.GetComponent<GeoTexAnim>();
                        if (anim == 10328)
                        {
                            this._geoTexAnim.geoTexAnimStop(2);
                            this._geoTexAnim.geoTexAnimPlay(0);
                        }
                    }
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 561 && (int)actor1.uid == 6)
                    {
                        this._geoTexAnim = actor1.go.GetComponent<GeoTexAnim>();
                        if (anim == 351)
                        {
                            this._geoTexAnim.geoTexAnimStop(2);
                            this._geoTexAnim.geoTexAnimPlay(1);
                        }
                    }
                }
                else if (this.gMode == 3)
                    this.ExecAnim(actor1, anim);
                return 0;
            case EBin.event_code_binary.WAITANIM:
                if (((int)actor1.animFlag & EventEngine.afExec) == 0)
                    return 0;
                this.stay();
                return 1;
            case EBin.event_code_binary.ENDANIM:
                this.AnimStop(actor1);
                return 0;
            case EBin.event_code_binary.STARTSEQ:
                int uid2 = (int)this.gExec.uid + EventEngine.cSeqOfs;
                Obj objByUid1 = this.FindObjByUID(uid2);
                if (objByUid1 != null)
                    this.DisposeObj(objByUid1);
                int sid2 = this.getv1();
                Seq seq = new Seq(sid2, uid2);
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1610)
                {
                    PosObj posObj3 = (PosObj)this.GetObjUID(21);
                    if (posObj3 != null)
                    {
                        int varManually = this.eBin.getVarManually(EBin.MAP_INDEX_SVR);
                        Debug.Log((object)("map_id = " + (object)varManually));
                        if (varManually == 26)
                        {
                            this._geoTexAnim = posObj3.go.GetComponent<GeoTexAnim>();
                            if (sid2 == 13)
                            {
                                this._geoTexAnim.geoTexAnimStop(2);
                                this._geoTexAnim.geoTexAnimPlay(0);
                            }
                        }
                    }
                }
                return 0;
            case EBin.event_code_binary.WAITSEQ:
                if (this.FindObjByUID((int)this.gExec.uid + EventEngine.cSeqOfs) == null)
                    return 0;
                this.stay();
                return 1;
            case EBin.event_code_binary.ENDSEQ:
                Obj objByUid2 = this.FindObjByUID((int)this.gExec.uid + EventEngine.cSeqOfs);
                if (objByUid2 != null)
                    this.DisposeObj(objByUid2);
                return 0;
            case EBin.event_code_binary.DEBUGCC:
                return 0;
            case EBin.event_code_binary.NECKFLAG:
                actor1.actf &= (ushort)~(EventEngine.actNeckT | EventEngine.actNeckM | EventEngine.actNeckTalk);
                actor1.actf |= (ushort)(this.getv1() & (EventEngine.actNeckT | EventEngine.actNeckM | EventEngine.actNeckTalk));
                return 0;
            case EBin.event_code_binary.ITEMADD:
                int id1 = this.getv2();
                int count1 = this.getv1();
                if (id1 < EventEngine.kSItemOfs)
                    ff9item.FF9Item_Add(id1, count1);
                else if (id1 < EventEngine.kCItemOfs)
                {
                    ff9item.FF9Item_AddImportant(id1 - EventEngine.kSItemOfs);
                }
                else
                {
                    while (count1-- > 0)
                        QuadMistDatabase.MiniGame_SetCard(id1 - EventEngine.kCItemOfs);
                }
                EMinigame.Auction10TimesAchievement(this.gCur);
                if (id1 == 288)
                    EMinigame.ViviWinHuntAchievement();
                else if (id1 == 203)
                    EMinigame.DigUpMadainRingAchievement();
                else if (id1 == 324)
                    EMinigame.SuperSlickOilAchievement();
                else if (id1 == 283)
                    EMinigame.AtleteQueenAchievement();
                return 0;
            case EBin.event_code_binary.ITEMDELETE:
                int id2 = this.getv2();
                int count2 = this.getv1();
                if (id2 < EventEngine.kSItemOfs)
                    ff9item.FF9Item_Remove(id2, count2);
                else if (id2 < EventEngine.kCItemOfs)
                    ff9item.FF9Item_RemoveImportant(id2 - EventEngine.kSItemOfs);
                if (id2 == 324)
                    EMinigame.MognetCentralAchievement();
                return 0;
            case EBin.event_code_binary.BTLSET:
                int num23 = this.getv1();
                int val = this.getv2();
                if (num23 == 33)
                    Debug.Log((object)"BTLSET 33");
                btl_scrp.SetBattleData((uint)num23, val);
                return 0;
            case EBin.event_code_binary.RADIUS:
                int num24 = this.getv1();
                int num25 = (int)(byte)this.getv1();
                int num26 = (int)(byte)this.getv1();
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1823 && (int)this.gCur.sid == 13 && (num24 == 8 && num25 == 8) && num26 == 8)
                    num24 = 30;
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 657 && (int)this.gCur.sid == 22 && num24 == 40)
                    num24 = 35;
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 164 && (int)this.gCur.sid == 7 && num24 == 30)
                    num24 = 20;
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2903 && (int)this.gCur.sid == 4 && num24 == 20)
                    num24 = 16;
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 3100 && (int)this.gCur.sid == 20 && num24 == 20)
                    num24 = 25;
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1104 && ((int)this.gCur.sid == 13 || (int)this.gCur.sid == 5))
                {
                    int num13;
                    num26 = num13 = 30;
                    num25 = num13;
                    num24 = num13;
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1108 && (int)this.gCur.sid == 40)
                {
                    int num13;
                    num26 = num13 = 30;
                    num25 = num13;
                    num24 = num13;
                }
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1606 && (int)this.gCur.sid == 34)
                    num24 = 15;
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2714 && (int)this.gCur.sid == 26)
                    num24 = 22;
                po.collRad = (byte)num25;
                po.talkRad = (byte)num26;
                if (this.gMode == 1)
                {
                    if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
                        return 0;
                    FieldMapActorController component1 = gameObject.GetComponent<FieldMapActorController>();
                    if ((UnityEngine.Object)component1 != (UnityEngine.Object)null)
                        component1.radius = (float)(num24 * 4);
                    FieldMapActor component2 = gameObject.GetComponent<FieldMapActor>();
                    if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
                        component2.CharRadius = (float)(num24 * 4);
                }
                else if (this.gMode != 3)
                ;
                return 0;
            case EBin.event_code_binary.ATTACH:
                Obj sourceObj = this.GetObj1();
                Obj targetObj = this.GetObj1();
                GameObject sourceObject1 = sourceObj.go;
                GameObject targetObject = targetObj.go;
                int bone_index = this.getv1();
                if ((UnityEngine.Object)sourceObject1 != (UnityEngine.Object)null && (UnityEngine.Object)targetObject != (UnityEngine.Object)null)
                {
                    if (this.gMode == 1 || this.gMode == 2)
                    {
                        if ((int)this.gCur.sid == 8 && (int)FF9StateSystem.Common.FF9.fldMapNo == 62 || (int)this.gCur.sid == 2 && (int)FF9StateSystem.Common.FF9.fldMapNo == 3010)
                            bone_index = 19;
                        geo.geoAttach(sourceObject1, targetObject, bone_index);
                    }
                    else if (this.gMode == 3)
                        geo.geoAttachInWorld(sourceObj, targetObj, bone_index);
                }
                return 0;
            case EBin.event_code_binary.DETACH:
                Obj obj1 = this.GetObj1();
                if (obj1 != null)
                {
                    bool restorePosAndScaling = false;
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 656 || (int)FF9StateSystem.Common.FF9.fldMapNo == 657 || ((int)FF9StateSystem.Common.FF9.fldMapNo == 658 || (int)FF9StateSystem.Common.FF9.fldMapNo == 659))
                        restorePosAndScaling = true;
                    if (this.gMode == 1)
                        geo.geoDetach(obj1.go, restorePosAndScaling);
                    else if (this.gMode != 2 && this.gMode == 3)
                        geo.geoDetachInWorld(obj1);
                }
                return 0;
            case EBin.event_code_binary.WATCH:
                int num27 = this.gExec.ip - 1;
                this.gExec.ip = num27;
                int num28 = (int)this.gExec.getByteIP();
                while (num28 != 0)
                {
                    ++this.gExec.ip;
                    num28 = (int)this.gExec.getByteIP();
                    ++num27;
                }
                this.eBin.SetVariableSpec(ref num27);
                this.gExec.ip = num27 + 1;
                this.gArgUsed = 1;
                return 0;
            case EBin.event_code_binary.STOP:
                num1 = (int)this.gExec.getByteIP(-1) | (int)this.gExec.getByteIP() << 8;
                ++this.gExec.ip;
                this.gArgUsed = 1;
                return 6;
            case EBin.event_code_binary.WAITTURN:
            case EBin.event_code_binary.DWAITTURN:
                if (eventCodeBinary == EBin.event_code_binary.DWAITTURN)
                    actor1 = (Actor)this.GetObj1();
                if (((int)actor1.flags & 128) != 0)
                    this.stay();
                return 1;
            case EBin.event_code_binary.TURNA:
                Obj obj2 = this.GetObj1();
                int tspeed1 = this.getv1();
                if (obj2 != null)
                {
                    PosObj posObj3 = (PosObj)obj2;
                    Vector3 vector3_1 = new Vector3(posObj3.pos[0], posObj3.pos[1], posObj3.pos[2]);
                    Vector3 vector3_2 = new Vector3(actor1.pos[0], actor1.pos[1], actor1.pos[2]);
                    float a = this.eBin.angleAsm(vector3_1.x - vector3_2.x, vector3_1.z - vector3_2.z);
                    this.StartTurn(actor1, a, true, tspeed1);
                }
                return 0;
            case EBin.event_code_binary.ASLEEP:
                actor1.sleep = (ushort)this.getv2();
                return 0;
            case EBin.event_code_binary.NOINITMES:
                this.eTb.InhInitMes();
                return 0;
            case EBin.event_code_binary.WAITMES:
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1650 && FF9StateSystem.Settings.CurrentLanguage == "Japanese" && ((int)this.gCur.sid == 19 && this.gCur.ip == 1849))
                {
                    this.getv1();
                    return 0;
                }
                this.gCur.winnum = (byte)this.getv1();
                this.gCur.wait = (byte)254;
                return 1;
            case EBin.event_code_binary.MROT:
                int num29 = this.getv1();
                if (num29 == 0)
                    num29 = (int)byte.MaxValue;
                actor1.omega = (byte)num29;
                return 0;
            case EBin.event_code_binary.TURN:
            case EBin.event_code_binary.DTURN:
                if (eventCodeBinary == EBin.event_code_binary.DTURN)
                    actor1 = (Actor)this.GetObj1();
                int num30 = this.getv1() << 4;
                int tspeed2 = this.getv1();
                this.StartTurn(actor1, EventEngineUtils.ConvertFixedPointAngleToDegree((short)num30), true, tspeed2);
                return 0;
            case EBin.event_code_binary.ENCRATE:
                this._context.encratio = (byte)this.getv1();
                if ((int)this._context.encratio == 0)
                    this._encountBase = 0;
                return 0;
            case EBin.event_code_binary.BGSMOVE:
                Debug.Log((object)("DoEventCode BGSMOVE : a = " + (object)this.getv2() + ", b = " + (object)this.getv2() + ", temp = " + (object)this.getv2()));
                return 0;
            case EBin.event_code_binary.BGLCOLOR:
                this.fieldmap.EBG_overlaySetShadeColor((uint)this.getv1(), (byte)this.getv1(), (byte)this.getv1(), (byte)this.getv1());
                return 0;
            case EBin.event_code_binary.BGLMOVE:
                this.fieldmap.EBG_overlayMove(this.getv1(), (short)this.getv2(), (short)this.getv2(), (short)this.getv2());
                return 0;
            case EBin.event_code_binary.BGLACTIVE:
                this.fieldmap.EBG_overlaySetActive(this.getv1(), this.getv1());
                return 0;
            case EBin.event_code_binary.BGLLOOP:
                this.fieldmap.EBG_overlaySetLoop((uint)this.getv1(), (uint)this.getv1(), this.getv2(), this.getv2());
                return 0;
            case EBin.event_code_binary.BGLPARALLAX:
                this.fieldmap.EBG_overlaySetParallax((uint)this.getv1(), (uint)this.getv1(), this.getv2(), this.getv2());
                return 0;
            case EBin.event_code_binary.BGLORIGIN:
                this.fieldmap.EBG_overlaySetOrigin(this.getv1(), this.getv2(), this.getv2());
                return 0;
            case EBin.event_code_binary.BGAANIME:
                this.fieldmap.EBG_animAnimate(this.getv1(), this.getv1());
                return 0;
            case EBin.event_code_binary.BGAACTIVE:
                this.fieldmap.EBG_animSetActive(this.getv1(), this.getv1());
                return 0;
            case EBin.event_code_binary.BGARATE:
                int animNdx = this.getv1();
                int frameRate = this.getv2();
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 66 && FF9StateSystem.Settings.IsFastForward && (animNdx == 3 || animNdx == 2) && frameRate == 128)
                    frameRate = 320;
                this.fieldmap.EBG_animSetFrameRate(animNdx, frameRate);
                return 0;
            case EBin.event_code_binary.SETROW:
                num1 = this.chr2slot(this.getv1());
                num2 = this.getv1();
                return 0;
            case EBin.event_code_binary.BGAWAIT:
                this.fieldmap.EBG_animSetFrameWait(this.getv1(), this.getv1(), this.getv1());
                return 0;
            case EBin.event_code_binary.BGAFLAG:
                this.fieldmap.EBG_animSetFlags(this.getv1(), this.getv1());
                return 0;
            case EBin.event_code_binary.BGARANGE:
                this.fieldmap.EBG_animSetPlayRange(this.getv1(), this.getv1(), this.getv1());
                return 0;
            case EBin.event_code_binary.MESVALUE:
                this.eTb.SetMesValue(this.getv1(), this.getv2());
                return 0;
            case EBin.event_code_binary.TWIST:
                this._context.twist_a = (short)this.getv1();
                this._context.twist_d = (short)this.getv1();
                FF9StateSystem.Field.SetTwistAD((int)this._context.twist_a, (int)this._context.twist_d);
                return 0;
            case EBin.event_code_binary.FICON:
                int type = this.getv1();
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2955)
                {
                    if ((int)this.gCur.uid == 24)
                        EIcon.PollFIcon(2);
                    else
                        EIcon.PollFIcon(type);
                }
                else
                    EIcon.PollFIcon(type);
                return 0;
            case EBin.event_code_binary.TIMERSET:
                TimerUI.SetTime(this.getv2());
                return 0;
            case EBin.event_code_binary.DASHOFF:
                this._context.dashinh = (byte)1;
                return 0;
            case EBin.event_code_binary.CLEARCOLOR:
                this.fieldmap.GetMainCamera().backgroundColor = new Color((float)this.getv1() / (float)byte.MaxValue, (float)this.getv1() / (float)byte.MaxValue, (float)this.getv1() / (float)byte.MaxValue);
                return 0;
            case EBin.event_code_binary.BGSSCROLL:
                this.fieldmap.EBG_scene2DScroll((short)this.getv2(), (short)this.getv2(), (ushort)this.getv1(), (uint)this.getv1());
                return 0;
            case EBin.event_code_binary.BGSRELEASE:
                this.fieldmap.EBG_scene2DScrollRelease(this.getv1(), (uint)this.getv1());
                return 0;
            case EBin.event_code_binary.BGCACTIVE:
                this.fieldmap.EBG_char3DScrollSetActive((uint)this.getv1(), this.getv1(), (uint)this.getv1());
                return 0;
            case EBin.event_code_binary.BGCHEIGHT:
                this.fieldmap.charAimHeight = (short)this.getv2();
                return 0;
            case EBin.event_code_binary.BGCLOCK:
                this.fieldmap.EBG_charLookAtLock();
                return 0;
            case EBin.event_code_binary.BGCUNLOCK:
                this.fieldmap.EBG_charLookAtUnlock();
                return 0;
            case EBin.event_code_binary.MENU:
                EventService.StartMenu(Convert.ToUInt32(this.getv1()), Convert.ToUInt32(this.getv1()));
                PersistenSingleton<UIManager>.Instance.MenuOpenEvent();
                return 1;
            case EBin.event_code_binary.TRACKSTART:
                Quad quad2 = (Quad)this.gCur;
                quad2.n = 2;
                QuadPos quadPos1 = quad2.q[0];
                short num31 = (short)this.getv2();
                quad2.q[1].X = num31;
                int num32 = (int)num31;
                quadPos1.X = (short)num32;
                QuadPos quadPos2 = quad2.q[0];
                short num33 = (short)this.getv2();
                quad2.q[1].Z = num33;
                int num34 = (int)num33;
                quadPos2.Z = (short)num34;
                return 0;
            case EBin.event_code_binary.TRACK:
                Quad quad3 = (Quad)this.gCur;
                int val1 = quad3.n;
                QuadPos quadPos3 = quad3.q[Math.Max(val1, 1) - 1];
                quadPos3.X = (short)this.getv2();
                quadPos3.Z = (short)this.getv2();
                return 0;
            case EBin.event_code_binary.TRACKADD:
                Quad quad4 = (Quad)this.gCur;
                if (quad4.n > 0 && quad4.n < 8)
                {
                    quad4.q[quad4.n] = quad4.q[quad4.n - 1];
                    ++quad4.n;
                }
                return 0;
            case EBin.event_code_binary.PRINTQUAD:
                return 0;
            case EBin.event_code_binary.ATURNL:
                actor1.turnl = (ushort)this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB[(int)actor1.turnl]);
                return 0;
            case EBin.event_code_binary.ATURNR:
                actor1.turnr = (ushort)this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB[(int)actor1.turnr]);
                return 0;
            case EBin.event_code_binary.CHOOSEPARAM:
                this.eTb.SetChooseParam(this.getv2(), this.getv1());
                return 0;
            case EBin.event_code_binary.TIMERCONTROL:
                this._ff9.timerControl = this.getv1() != 0;
                TimerUI.SetPlay(this._ff9.timerControl);
                return 0;
            case EBin.event_code_binary.SETCAM:
                int newCamIdx = this.getv1();
                this.fieldmap.GetCurrentBgCamera().projectedWalkMesh.SetActive(false);
                this.fieldmap.SetCurrentCameraIndex(newCamIdx);
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1205 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 4800 && this.eBin.getVarManually(6357) == 3)
                    this.SetActorPosition(this._fixThornPosObj, (float)this._fixThornPosA, (float)this._fixThornPosB, (float)this._fixThornPosC);
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 3009 && (int)this.gCur.uid == 17 && newCamIdx == 0)
                {
                    EventEngine.resyncBGMSignal = 1;
                    Debug.Log((object)("SET resyncBGMSignal = " + (object)EventEngine.resyncBGMSignal));
                }
                return 0;
            case EBin.event_code_binary.IDLESPEED:
                actor1.idleSpeed[0] = (byte)this.getv1();
                actor1.idleSpeed[1] = (byte)this.getv1();
                actor1.idleSpeed[2] = (byte)this.getv1();
                actor1.idleSpeed[3] = (byte)this.getv1();
                return 0;
            case EBin.event_code_binary.CHRFX:
                int Parm = this.getv1();
                int num35 = this.getv2();
                int num36 = this.getv2();
                int num37 = this.getv2();
                fldchar.FF9FieldCharDispatch((int)po.uid, Parm, num35, num36, num37);
                return 0;
            case EBin.event_code_binary.SEPV:
                int PosPtr1;
                int VolPtr1;
                FF9Snd.FF9FieldSoundGetPositionVolume(this.getv2(), this.getv2(), this.getv2(), out PosPtr1, out VolPtr1);
                this.sSEPos = PosPtr1;
                int num38 = this.getv1();
                this.sSEVol = VolPtr1 * num38 >> 7;
                if (this.sSEVol > (int)sbyte.MaxValue)
                    this.sSEVol = (int)sbyte.MaxValue;
                return 0;
            case EBin.event_code_binary.SEPVA:
                PosObj posObj4 = (PosObj)this.GetObj1();
                int PosPtr2;
                int VolPtr2;
                FF9Snd.FF9FieldSoundGetPositionVolume((int)posObj4.pos[0], (int)posObj4.pos[1], (int)posObj4.pos[2], out PosPtr2, out VolPtr2);
                this.sSEPos = PosPtr2;
                int num39 = this.getv1();
                this.sSEVol = VolPtr2 * num39 >> 7;
                if (this.sSEVol > (int)sbyte.MaxValue)
                    this.sSEVol = (int)sbyte.MaxValue;
                return 0;
            case EBin.event_code_binary.NECKID:
                actor1.neckMyID = (byte)this.getv1();
                actor1.neckTargetID = (byte)this.getv1();
                actor1.actf |= (ushort)(EventEngine.actNeckT | EventEngine.actNeckM);
                return 0;
            case EBin.event_code_binary.ENCOUNT2:
                this.getv1(); // rush_type
                this._ff9.btlSubMapNo = (sbyte)this.getv1();
                int num40 = this.getv2();
                this._ff9.steiner_state = (byte)(num40 >> 15 & 1);
                this.SetBattleScene(num40 & (int)short.MaxValue);
                FF9StateSystem.Battle.isRandomEncounter = false;
                return 3;
            case EBin.event_code_binary.TIMERDISPLAY:
                this._ff9.timerDisplay = this.getv1() != 0;
                TimerUI.SetEnable(this._ff9.timerDisplay);
                TimerUI.SetDisplay(this._ff9.timerDisplay);
                return 0;
            case EBin.event_code_binary.RAISE:
                this.eTb.RaiseAllWindow();
                return 0;
            case EBin.event_code_binary.CHRCOLOR:
                if (fldmcf.FF9FieldMCFSetCharColor((int)this.GetObj1().uid, this.getv1(), this.getv1(), this.getv1()) == 0)
                    return 0;
                this.stay();
                return 1;
            case EBin.event_code_binary.SLEEPINH:
                this._context.idletimer = (short)-1;
                return 0;
            case EBin.event_code_binary.AUTOTURN:
                int num41 = this.getv1();
                actor1.turninst0 = num41 == 0 ? (short)4 : (short)167;
                return 0;
            case EBin.event_code_binary.BGLATTACH:
                this.fieldmap.EBG_charAttachOverlay((uint)this.getv1(), (short)this.getv2(), (short)this.getv1(), (sbyte)this.getv1(), (byte)this.getv1(), (byte)this.getv1(), (byte)this.getv1());
                return 0;
            case EBin.event_code_binary.CFLAG:
                int cflag = (int)(byte)this.getv1();
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2934 && MBG.Instance.GetFrame < 10)
                {
                    this.StartCoroutine(this.DelayedCFLAG(this.gCur, cflag));
                }
                else
                {
                    this.gCur.flags = (byte)((this.gCur.flags & -64) | (cflag & 63));
                }
                return 0;
            case EBin.event_code_binary.AJUMP:
                actor1.jump = (ushort)this.getv2();
                actor1.jump0 = (byte)this.getv1();
                actor1.jump1 = (byte)this.getv1();
                return 0;
            case EBin.event_code_binary.MESA:
                PosObj targetPo1 = (PosObj)this.GetObj1();
                this.gCur.winnum = (byte)this.getv1();
                int flags3 = this.getv1();
                this.SetFollow((Obj)targetPo1, (int)this.gCur.winnum, flags3);
                this.eTb.NewMesWin(this.getv2(), (int)this.gCur.winnum, flags3, targetPo1);
                this.gCur.wait = (byte)254;
                return 1;
            case EBin.event_code_binary.MESAN:
                PosObj targetPo2 = (PosObj)this.GetObj1();
                this.gCur.winnum = (byte)this.getv1();
                int flags4 = this.getv1();
                this.SetFollow((Obj)targetPo2, (int)this.gCur.winnum, flags4);
                this.eTb.NewMesWin(this.getv2(), (int)this.gCur.winnum, flags4, targetPo2);
                return 0;
            case EBin.event_code_binary.DRET:
                Obj obj3 = this.GetObj1();
                if (obj3 != null)
                {
                    obj3.sx = (byte)0;
                    obj3.state = EventEngine.stateInit;
                    this.Return(obj3);
                }
                return obj3 == this.gExec ? 1 : 0;
            case EBin.event_code_binary.MOVT:
                actor1.loopCount = (byte)this.getv1();
                return 0;
            case EBin.event_code_binary.TSPEED:
                actor1.tspeed = (byte)this.getv1();
                if ((int)actor1.tspeed == 0)
                    actor1.tspeed = (byte)16;
                return 0;
            case EBin.event_code_binary.BGIACTIVET:
                int num42 = this.getv2();
                int num43 = this.getv1();
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1753 && num42 == 207)
                    this.fieldmap.walkMesh.BGI_triSetActive(208U, (uint)num43);
                else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1606 && num42 == 107)
                    num43 = 1;
                this.fieldmap.walkMesh.BGI_triSetActive((uint)num42, (uint)num43);
                return 0;
            case EBin.event_code_binary.TURNTO:
                int num44 = this.getv2();
                int num45 = this.getv2();
                if (!EventEngineUtils.nearlyEqual((float)num44, gameObject.transform.localPosition.x) || !EventEngineUtils.nearlyEqual((float)num45, gameObject.transform.localPosition.z))
                {
                    FieldMapActorController component = gameObject.GetComponent<FieldMapActorController>();
                    float a = this.eBin.angleAsm((float)num44 - component.curPos.x, (float)num45 - component.curPos.z);
                    this.StartTurn(actor1, a, true, (int)actor1.tspeed);
                }
                return 0;
            case EBin.event_code_binary.PREJUMP:
                actor1.animFlag = (byte)EventEngine.afHold;
                actor1.inFrame = (byte)0;
                actor1.outFrame = (byte)((uint)actor1.jump0 - 1U);
                this.ExecAnim(actor1, (int)actor1.jump);
                return 0;
            case EBin.event_code_binary.POSTJUMP:
                ff9shadow.FF9ShadowOnField((int)actor1.uid);
                actor1.animFlag = (byte)0;
                actor1.inFrame = (byte)((uint)actor1.jump1 + 1U);
                actor1.outFrame = byte.MaxValue;
                this.ExecAnim(actor1, (int)actor1.jump);
                return 0;
            case EBin.event_code_binary.MOVQ:
                PosObj posObj5 = (PosObj)this.FindObjByUID((int)this._context.controlUID);
                if (posObj5 != null)
                {
                    this.Call((Obj)posObj5, 0, 0, false, Obj.movQData);
                    this._context.usercontrol = (byte)0;
                    for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
                        objList.obj.flags |= (byte)6;
                }
                return 0;
            case EBin.event_code_binary.CHRSCALE:
                PosObj posObj6 = (PosObj)this.GetObj1();
                int num46 = this.getv1();
                int num47 = this.getv1();
                int num48 = this.getv1();
                int num49 = 18;
                if (posObj6 == null)
                    return 0;
                if ((UnityEngine.Object)posObj6.go != (UnityEngine.Object)null)
                    geo.geoScaleSetXYZ(posObj6.go, num46 << 24 >> num49, num47 << 24 >> num49, num48 << 24 >> num49);
                posObj6.scaley = (byte)num47;
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 576 && ((int)posObj6.uid == 4 || (int)posObj6.uid == 8 || ((int)posObj6.uid == 9 || (int)posObj6.uid == 10) || (int)posObj6.uid == 11))
                {
                    this._geoTexAnim = posObj6.go.GetComponent<GeoTexAnim>();
                    this._geoTexAnim.geoTexAnimStop(2);
                    this._geoTexAnim.geoTexAnimPlay(1);
                }
                return 0;
            case EBin.event_code_binary.MOVJ:
                if (this.MoveToward_mixed((float)this.sMapJumpX, 0.0f, (float)this.sMapJumpZ, 0, (PosObj)null))
                    this.stay();
                return 1;
            case EBin.event_code_binary.POS3:
            case EBin.event_code_binary.DPOS3:
                if (eventCodeBinary == EBin.event_code_binary.DPOS3)
                    po = (PosObj)this.GetObj1();
                int num50 = this.getv2();
                int num51 = -this.getv2();
                int num52 = this.getv2();
                if ((this.gMode != 1 || (int)po.model == (int)ushort.MaxValue ? 0 : 1) == 1)
                {
                    if (po != null)
                    {
                        FieldMapActorController component = po.go.GetComponent<FieldMapActorController>();
                        if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.walkMesh != null)
                            component.walkMesh.BGI_charSetActive(component, 0U);
                        if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1205 && (int)po.sid == 6 && (this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 4800 && num50 == 418) && num52 == 9733)
                        {
                            this._fixThornPosA = num50;
                            this._fixThornPosB = num51;
                            this._fixThornPosC = num52;
                            this._fixThornPosObj = po;
                            num50 = 600;
                            num52 = 9999;
                        }
                        if ((int)FF9StateSystem.Common.FF9.fldMapNo == 563 && (int)po.sid == 16)
                        {
                            if (num50 == -1614)
                                num50 = -1635;
                        }
                        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1310 && (int)po.sid == 12)
                        {
                            if (num50 == -1614)
                                num50 = -1635;
                        }
                        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2110 && (int)po.sid == 9)
                        {
                            if (num50 == -1614)
                                num50 = -1635;
                        }
                        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 572 && (int)po.sid == 16)
                        {
                            if (num50 == -1750)
                                num50 = -1765;
                        }
                        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1607)
                        {
                            int varManually1 = this.eBin.getVarManually(EBin.SC_COUNTER_SVR);
                            int varManually2 = this.eBin.getVarManually(9169);
                            if (((int)po.model == 236 || (int)po.model == 237) && (varManually1 >= 6640 && varManually1 < 6690) && varManually2 == 0)
                                num51 += 50000;
                        }
                        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1606 && ((int)po.uid == 9 && num50 == -171 && (num51 == 641 && num52 == 1042) || (int)po.uid == 128 && num50 == -574 && (num51 == 624 && num52 == 903) || ((int)po.uid == 129 && num50 == -226 && (num51 == 639 && num52 == 1009) || (int)po.uid == 130 && num50 == -93 && (num51 == 647 && num52 == 1017)) || ((int)po.uid == 131 && num50 == 123 && (num51 == 667 && num52 == 1006) || (int)po.uid == 132 && num50 == -689 && (num51 == 621 && num52 == 859))))
                            gameObject.GetComponent<FieldMapActor>().SetRenderQueue(2000);
                    }
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1756 && (int)po.sid == 6 && (num50 == -3019 && num52 == 1226))
                    {
                        num50 = -3145;
                        num51 = -2035;
                        num52 = 1274;
                    }
                }
                this.SetActorPosition(po, (float)num50, (float)num51, (float)num52);
                if ((int)po.cid == 4)
                    this.clrdist((Actor)po);
                return 0;
            case EBin.event_code_binary.MOVE3:
                if (this.MoveToward_mixed((float)this.getv2(), (float)-this.getv2(), (float)this.getv2(), 2, (PosObj)null))
                    this.stay();
                return 1;
            case EBin.event_code_binary.DRADIUS:
                this.getv1();
                this.getv1();
                this.getv1();
                this.getv1();
                return 0;
            case EBin.event_code_binary.MJPOS:
                PosObj posObj7 = (PosObj)this.FindObjByUID((int)this._context.controlUID);
                if (posObj7 != null)
                {
                    QuadPos quadPos4 = ((Quad)this.gCur).q[0];
                    QuadPos quadPos5 = ((Quad)this.gCur).q[1];
                    int num13 = (int)quadPos5.X - (int)quadPos4.X;
                    int num53 = (int)quadPos5.Z - (int)quadPos4.Z;
                    int num54 = num13 * num13 + num53 * num53 >> 8;
                    if (num54 != 0)
                    {
                        num54 = ((int)((double)num13 * ((double)posObj7.pos[0] - (double)quadPos4.X)) + (int)((double)num53 * ((double)posObj7.pos[2] - (double)quadPos4.Z))) / num54;
                        if (num54 < 0)
                            num54 = 0;
                        else if (num54 > 256)
                            num54 = 256;
                    }
                    this.sMapJumpX = (num54 * num13 >> 8) + (int)quadPos4.X;
                    this.sMapJumpZ = (num54 * num53 >> 8) + (int)quadPos4.Z;
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 552 && (int)quadPos4.X == 1231 && ((int)quadPos4.Z == 1556 && (int)quadPos5.X == 1291) && (int)quadPos5.Z == 1376)
                    {
                        this.sMapJumpX = 1226;
                        this.sMapJumpZ = 1430;
                    }
                }
                else
                    this.sMapJumpX = this.sMapJumpZ = 0;
                return 0;
            case EBin.event_code_binary.MOVH:
                int num55 = this.getv2();
                int num56 = this.getv2();
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 66 && (int)po.sid == 14 && (num55 == -145 && num56 == -9135))
                {
                    num55 = -160;
                    num56 = -9080;
                }
                bool flag3 = this.MoveToward_mixed((float)num55, 0.0f, (float)num56, 1, (PosObj)null);
                eulerAngles1 = po.go.transform.localRotation.eulerAngles;
                if (flag3)
                    this.stay();
                return 1;
            case EBin.event_code_binary.SPEEDTH:
                actor1.speedth = (byte)this.getv1();
                return 0;
            case EBin.event_code_binary.TURNDS:
                int num57 = this.getv1() << 4;
                this.StartTurn(actor1, EventEngineUtils.ConvertFixedPointAngleToDegree((short)num57), true, (int)actor1.tspeed);
                return 0;
            case EBin.event_code_binary.BGI:
                int num58 = this.getv1();
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2508 && (int)po.sid == 2 && num58 == 1)
                    num58 = 0;
                if ((int)po.model != (int)ushort.MaxValue)
                {
                    FieldMapActorController component = gameObject.GetComponent<FieldMapActorController>();
                    component.walkMesh.BGI_charSetActive(component, (uint)num58);
                }
                return 0;
            case EBin.event_code_binary.GETSCREEN:
                Obj obj4 = this.GetObj1();
                if (obj4 != null && (UnityEngine.Object)obj4.go != (UnityEngine.Object)null)
                {
                    float x;
                    float y;
                    ETb.World2Screen(obj4.go.GetComponent<FieldMapActorController>().curPos, out x, out y);
                    this.sSysX = (int)x;
                    this.sSysY = (int)y;
                }
                return 0;
            case EBin.event_code_binary.MENUON:
                EventInput.PSXCntlClearPadMask(0, 262144U);
                PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(true);
                return 0;
            case EBin.event_code_binary.MENUOFF:
                EventInput.PSXCntlSetPadMask(0, 262144U);
                PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
                return 1;
            case EBin.event_code_binary.DISCCHANGE:
                int num59 = this.getv2();
                this.FF9FieldDiscRequest((byte)(num59 >> 14 & 3), (ushort)(num59 & 16383));
                return 1;
            case EBin.event_code_binary.MINIGAME:
                int minigameFlag = this.getv2();
                EventService.SetMiniGame((ushort)minigameFlag);
                EMinigame.SetQuadmistStadiumOpponentId(this.gCur, minigameFlag);
                EMinigame.SetThiefId(this.gCur);
                EMinigame.SetFatChocoboId(this.gCur);
                return 7;
            case EBin.event_code_binary.DELETEALLCARD:
                QuadMistDatabase.MiniGame_AwayAllCard();
                return 0;
            case EBin.event_code_binary.SETMAPNAME:
                FF9StateSystem.Common.FF9.mapNameStr = FF9TextTool.FieldText(this.getv2());
                return 0;
            case EBin.event_code_binary.RESETMAPNAME:
                FF9StateSystem.Common.FF9.mapNameStr = this._defaultMapName;
                return 0;
            case EBin.event_code_binary.PARTYMENU:
                FF9PARTY_INFO sPartyInfo = new FF9PARTY_INFO();
                sPartyInfo.party_ct = this.getv1();
                int num60 = 0;
                for (int index4 = 8; index4 >= 0; --index4)
                    num60 = num60 << 1 | ((int)FF9StateSystem.Common.FF9.player[index4].info.party == 0 ? 0 : 1);
                int num61 = this.getv2();
                for (int index4 = 0; index4 < 9; ++index4)
                {
                    sPartyInfo.fix[index4] = (num61 & 1) > 0;
                    num61 >>= 1;
                }
                for (int index4 = 0; index4 < 4; ++index4)
                {
                    if (FF9StateSystem.Common.FF9.party.member[index4] != null)
                    {
                        sPartyInfo.menu[index4] = FF9StateSystem.Common.FF9.party.member[index4].info.slot_no;
                        int int32 = Convert.ToInt32(sPartyInfo.menu[index4]);
                        num60 &= ~(1 << int32);
                    }
                    else
                        sPartyInfo.menu[index4] = Convert.ToByte((int)byte.MaxValue);
                }
                int num62;
                int num63 = num62 = 0;
                while (num63 < 9 && num62 < PartySettingUI.FF9PARTY_PLAYER_MAX && num60 > 0)
                {
                    if ((num60 & 1) > 0)
                        sPartyInfo.select[num62++] = Convert.ToByte(num63);
                    ++num63;
                    num60 >>= 1;
                }
                while (num62 < PartySettingUI.FF9PARTY_PLAYER_MAX)
                    sPartyInfo.select[num62++] = PartySettingUI.FF9PARTY_NONE;
                EventService.OpenPartyMenu(sPartyInfo);
                return 1;
            case EBin.event_code_binary.SPS:
                this.fieldSps.FF9FieldSPSSetObjParm(this.getv1(), this.getv1(), this.getv2(), this.getv2(), this.getv2());
                return 0;
            case EBin.event_code_binary.FULLMEMBER:
                int num64 = this.getv2();
                int num65 = num64 | num64 >> 4 & 224;
                for (int slot_id = 0; slot_id < 9; ++slot_id)
                    ff9play.FF9Play_Delete(slot_id);
                for (int slot_id = 0; slot_id < 9; ++slot_id)
                {
                    if ((num65 & 1) > 0)
                        ff9play.FF9Play_Add(slot_id);
                    num65 >>= 1;
                }
                return 0;
            case EBin.event_code_binary.PRETEND:
                Obj obj5 = this.GetObj1();
                if (obj5 != null)
                {
                    if (obj5 != po)
                    {
                        actor1.parent = (Actor)obj5;
                        actor1.animFlag = (byte)EventEngine.afExec;
                    }
                    else
                    {
                        actor1.parent = (Actor)null;
                        actor1.animFlag = (byte)0;
                    }
                }
                return 0;
            case EBin.event_code_binary.WMAPJUMP:
                this.SetNextMap(this.getv2());
                return 5;
            case EBin.event_code_binary.EYE:
                if (this.gMode == 3)
                {
                    Vector3 eyePtr = ff9.w_cameraGetEyePtr();
                    this.SetActorPosition(po, eyePtr.x, eyePtr.y, eyePtr.z);
                    actor1.actf = (ushort)((uint)actor1.actf | (uint)EventEngine.actEye);
                }
                return 0;
            case EBin.event_code_binary.AIM:
                if (this.gMode == 3)
                {
                    Vector3 aimPtr = ff9.w_cameraGetAimPtr();
                    this.SetActorPosition(po, aimPtr.x, aimPtr.y, aimPtr.z);
                    actor1.actf = (ushort)((uint)actor1.actf | (uint)EventEngine.actAim);
                }
                return 0;
            case EBin.event_code_binary.SETKEYMASK:
                EventInput.PSXCntlSetPadMask(this.getv1(), Convert.ToUInt32(this.getv2()));
                return 0;
            case EBin.event_code_binary.CLEARKEYMASK:
                EventInput.PSXCntlClearPadMask(this.getv1(), Convert.ToUInt32(this.getv2()));
                return 0;
            case EBin.event_code_binary.DANIM:
                this.ExecAnim((Actor)this.GetObj1(), this.getv2());
                return 0;
            case EBin.event_code_binary.DWAITANIM:
                if (((int)((Actor)this.GetObj1()).animFlag & EventEngine.afExec) == 0)
                    return 0;
                this.stay();
                return 1;
            case EBin.event_code_binary.TEXPLAY:
                PosObj posObj8 = (PosObj)this.GetObj1();
                this._geoTexAnim = posObj8.go.GetComponent<GeoTexAnim>();
                int anum1 = this.getv1();
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 114 && (int)posObj8.uid == 2 && anum1 == 0 || (int)FF9StateSystem.Common.FF9.fldMapNo == 450 && (int)posObj8.uid == 3 && anum1 == 0 || ((int)FF9StateSystem.Common.FF9.fldMapNo == 551 && (int)posObj8.uid == 10 && anum1 == 0 || (int)FF9StateSystem.Common.FF9.fldMapNo == 555 && (int)posObj8.uid == 12 && anum1 == 0) || ((int)FF9StateSystem.Common.FF9.fldMapNo == 559 && (int)posObj8.uid == 17 && anum1 == 0 || (int)FF9StateSystem.Common.FF9.fldMapNo == 2105 && (int)posObj8.uid == 2 && anum1 == 0 || (int)FF9StateSystem.Common.FF9.fldMapNo == 2450 && (int)posObj8.uid == 8 && anum1 == 0))
                    anum1 = 2;
                if ((UnityEngine.Object)posObj8.go != (UnityEngine.Object)null && (UnityEngine.Object)this._geoTexAnim != (UnityEngine.Object)null)
                    this._geoTexAnim.geoTexAnimPlay(anum1);
                return 0;
            case EBin.event_code_binary.TEXPLAY1:
                PosObj posObj9 = (PosObj)this.GetObj1();
                this._geoTexAnim = posObj9.go.GetComponent<GeoTexAnim>();
                int anum2 = this.getv1();
                if ((UnityEngine.Object)posObj9.go != (UnityEngine.Object)null && (UnityEngine.Object)this._geoTexAnim != (UnityEngine.Object)null)
                    this._geoTexAnim.geoTexAnimPlayOnce(anum2);
                return 0;
            case EBin.event_code_binary.TEXSTOP:
                PosObj posObj10 = (PosObj)this.GetObj1();
                this._geoTexAnim = posObj10.go.GetComponent<GeoTexAnim>();
                int anum3 = this.getv1();
                if ((UnityEngine.Object)posObj10.go != (UnityEngine.Object)null && (UnityEngine.Object)this._geoTexAnim != (UnityEngine.Object)null)
                    this._geoTexAnim.geoTexAnimStop(anum3);
                return 0;
            case EBin.event_code_binary.BGVSET:
                this.fieldmap.EBG_overlaySetViewport((uint)this.getv1(), (uint)this.getv1());
                return 0;
            case EBin.event_code_binary.WPRM:
                ff9.w_frameSetParameter(this.getv1(), this.getv2());
                return 0;
            case EBin.event_code_binary.FLDSND0:
                FF9Snd.FF9SoundArg0(this.getv2(), this.getv2());
                return 0;
            case EBin.event_code_binary.FLDSND1:
                int _parmtype1 = this.getv2();
                int _objno1 = this.getv2();
                int num66 = this.getv3();
                FF9Snd.FF9SoundArg1(_parmtype1, _objno1, num66);
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2928 && _objno1 == 2787)
                {
                    FF9Snd.FF9SoundArg1(_parmtype1, 2982, num66);
                    FF9Snd.FF9SoundArg1(_parmtype1, 2983, num66);
                }
                return 0;
            case EBin.event_code_binary.FLDSND2:
                FF9Snd.FF9SoundArg2(this.getv2(), this.getv2(), this.getv3(), this.getv1());
                return 0;
            case EBin.event_code_binary.FLDSND3:
                int _parmtype2 = this.getv2();
                int _objno2 = this.getv2();
                int num67 = this.getv3();
                int num68 = this.getv1();
                int num69 = this.getv1();
                FF9Snd.FF9SoundArg3(_parmtype2, _objno2, num67, num68, num69);
                if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2928)
                {
                    if (_objno2 == 2786)
                    {
                        FF9Snd.FF9SoundArg3(_parmtype2, 2980, num67, num68, num69);
                        FF9Snd.FF9SoundArg3(_parmtype2, 2981, num67, num68, num69);
                    }
                    else if (_parmtype2 == -12288 && _objno2 == 2787 && (num67 == 0 && num68 == 128) && num69 == 125)
                    {
                        FF9Snd.FF9SoundArg1(20736, 2980, 0);
                        FF9Snd.FF9SoundArg1(20736, 2981, 0);
                        FF9Snd.FF9SoundArg3(_parmtype2, 2982, num67, num68, num69);
                        FF9Snd.FF9SoundArg3(_parmtype2, 2983, num67, num68, num69);
                    }
                    else if (_objno2 == 2787)
                    {
                        FF9Snd.FF9SoundArg3(_parmtype2, 2982, num67, num68, num69);
                        FF9Snd.FF9SoundArg3(_parmtype2, 2983, num67, num68, num69);
                    }
                }
                return 0;
            case EBin.event_code_binary.BGVDEFINE:
                this.fieldmap.EBG_overlayDefineViewport((uint)this.getv1(), (short)this.getv2(), (short)this.getv2(), (short)this.getv2(), (short)this.getv2());
                return 0;
            case EBin.event_code_binary.BGAVISIBLE:
                this.fieldmap.EBG_animSetVisible(this.getv1(), this.getv1());
                return 0;
            case EBin.event_code_binary.BGIACTIVEF:
                this.fieldmap.walkMesh.BGI_floorSetActive((uint)this.getv1(), (uint)this.getv1());
                return 0;
            case EBin.event_code_binary.CHRSET:
                int attr1 = this.getv2();
                FF9Char.ff9char_attr_set((int)po.uid, attr1);
                return 0;
            case EBin.event_code_binary.CHRCLEAR:
                int attr2 = this.getv2();
                FF9Char.ff9char_attr_clear((int)po.uid, attr2);
                return 0;
            case EBin.event_code_binary.GILADD:
                if ((this._ff9.party.gil += (uint)this.getv3()) > 9999999U)
                    this._ff9.party.gil = 9999999U;
                return 0;
            case EBin.event_code_binary.GILDELETE:
                int gilDecrease = this.getv3();
                if ((this._ff9.party.gil -= (uint)gilDecrease) > 9999999U)
                    this._ff9.party.gil = 0U;
                if (this.isPosObj(this.gCur))
                    EMinigame.StiltzkinAchievement((PosObj)this.gCur, gilDecrease);
                return 0;
            case EBin.event_code_binary.MESB:
                UIManager.Battle.SetBattleMessage(FF9TextTool.BattleText(this.getv2()), (byte)3);
                return 0;
            case EBin.event_code_binary.GLOBALCLEAR:
                return 0;
            case EBin.event_code_binary.DEBUGSAVE:
                return 0;
            case EBin.event_code_binary.DEBUGLOAD:
                return 0;
            case EBin.event_code_binary.ATTACHOFFSET:
                GameObject sourceObject2 = this.gCur.go;
                int x1 = this.getv2();
                int y1 = this.getv2();
                int z = this.getv2();
                if ((UnityEngine.Object)sourceObject2 != (UnityEngine.Object)null)
                    geo.geoAttachOffset(sourceObject2, x1, y1, z);
                return 0;
            case EBin.event_code_binary.PUSHHIDE:
                for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
                {
                    Obj obj6 = objList.obj;
                    if (this.isPosObj(obj6))
                    {
                        ((PosObj)obj6).pflags = obj6.flags;
                        if (((int)obj6.flags & 32) == 0)
                            obj6.flags = (byte)((uint)obj6.flags & 4294967294U);
                    }
                }
                return 0;
            case EBin.event_code_binary.POPSHOW:
                for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
                {
                    Obj obj6 = objList.obj;
                    if (this.isPosObj(obj6))
                    {
                        obj6.flags = (byte)((uint)obj6.flags & 4294967294U);
                        obj6.flags = (byte)((uint)obj6.flags | (uint)((PosObj)obj6).pflags & 1U);
                    }
                }
                return 0;
            case EBin.event_code_binary.AICON:
                EIcon.SetAIcon(this.getv1());
                return 0;
            case EBin.event_code_binary.CLEARSTATUS:
                int num70 = (int)this.fieldCalc.FieldRemoveStatus(FF9StateSystem.Common.FF9.player[this.chr2slot(this.getv1())], (byte)this.getv1());
                return 0;
            case EBin.event_code_binary.SPS2:
                this.fieldSps.FF9FieldSPSSetObjParm(this.getv1(), this.getv1(), this.getv1(), this.getv2(), this.getv2());
                return 0;
            case EBin.event_code_binary.WINPOSE:
                int index5 = this.chr2slot(this.getv1());
                int num71 = this.getv1();
                if (index5 >= 0 && index5 < 9)
                    this._ff9.player[index5].info.win_pose = (byte)num71;
                return 0;
            case EBin.event_code_binary.JUMP3:
                int num72 = (int)actor1.jframe;
                ++actor1.jframe;
                int num73 = (int)actor1.jframeN;
                actor1.pos[0] = (float)(((int)actor1.x0 * (num73 - num72) + (int)actor1.jumpx * num72) / num73);
                actor1.pos[1] = (float)((int)actor1.y0 - num72 * (num72 << 3) + num72 * ((int)actor1.jumpy - (int)actor1.y0) / num73 + num72 * (num73 << 3));
                actor1.pos[2] = (float)(((int)actor1.z0 * (num73 - num72) + (int)actor1.jumpz * num72) / num73);
                this.SetActorPosition(po, actor1.pos[0], actor1.pos[1], actor1.pos[2]);
                if (num72 >= num73)
                    return 0;
                this.stay();
                return 1;
            case EBin.event_code_binary.PARTYDELETE:
                int num74 = this.chr2slot(this.getv1());
                int party_id = 0;
                while (party_id < 4 && (this._ff9.party.member[party_id] == null || (int)this._ff9.party.member[party_id].info.slot_no != num74))
                    ++party_id;
                if (party_id < 4)
                {
                    ff9play.FF9Play_SetParty(party_id, -1);
                    this.SetupPartyUID();
                }
                return 0;
            case EBin.event_code_binary.PLAYERNAME:
                int index6 = this.chr2slot(this.getv1());
                int textId = this.getv2();
                if (index6 >= 0 && index6 < 9)
                    this._ff9.player[index6].name = FF9TextTool.RemoveOpCode(FF9TextTool.FieldText(textId));
                return 0;
            case EBin.event_code_binary.OVAL:
                po.ovalRatio = (byte)this.getv1();
                return 0;
            case EBin.event_code_binary.INCFROG:
                if ((int)this._ff9.frog_no < (int)short.MaxValue)
                    ++this._ff9.frog_no;
                EMinigame.CatchingGoldenFrogAchievement(this.gCur);
                EMinigame.Catching99FrogAchievement((int)this._ff9.frog_no);
                return 0;
            case EBin.event_code_binary.BEND:
                this._noEvents = true;
                PersistenSingleton<UIManager>.Instance.BattleResultScene.ShutdownBattleResultUI();
                return 0;
            case EBin.event_code_binary.SETVY3:
                actor1.jumpx = (short)this.getv2();
                actor1.jumpy = (short)-this.getv2();
                actor1.jumpz = (short)this.getv2();
                int num75 = (int)(byte)this.getv1();
                if (num75 == 0)
                    num75 = 8;
                actor1.actf |= (ushort)EventEngine.actJump;
                ff9shadow.FF9ShadowOffField((int)actor1.uid);
                actor1.inFrame = actor1.jump0;
                actor1.outFrame = actor1.jump1;
                this.ExecAnim(actor1, (int)actor1.jump);
                int num76 = ((int)actor1.outFrame - (int)actor1.inFrame << 4) / num75;
                actor1.aspeed = (byte)num76;
                if (this.gMode == 1 && (int)po.model != (int)ushort.MaxValue)
                {
                    fmac = gameObject.GetComponent<FieldMapActorController>();
                    fmac.walkMesh.BGI_charSetActive(fmac, 0U);
                }
                actor1.x0 = (short)fmac.curPos.x;
                actor1.y0 = (short)fmac.curPos.y;
                actor1.z0 = (short)fmac.curPos.z;
                actor1.jframe = (byte)0;
                actor1.jframeN = (byte)num75;
                return 0;
            case EBin.event_code_binary.SETSIGNAL:
                ETb.gMesSignal = this.getv1();
                return 0;
            case EBin.event_code_binary.BGLSCROLLOFFSET:
                this.fieldmap.EBG_overlaySetScrollWithOffset((uint)this.getv1(), (uint)this.getv1(), this.getv2(), this.getv2(), (uint)this.getv1());
                return 0;
            case EBin.event_code_binary.BTLSEQ:
                btlseq.StartBtlSeq(this.GetSysList(1), this.GetSysList(0), this.getv1());
                return 0;
            case EBin.event_code_binary.BGLLOOPTYPE:
                this.fieldmap.EBG_overlaySetLoopType((uint)this.getv1(), (uint)this.getv1());
                return 0;
            case EBin.event_code_binary.BGAFRAME:
                this.fieldmap.EBG_animShowFrame(this.getv1(), this.getv1());
                return 0;
            case EBin.event_code_binary.MOVE3H:
                if (this.MoveToward_mixed((float)this.getv2(), (float)-this.getv2(), (float)this.getv2(), 3, (PosObj)null))
                    this.stay();
                return 1;
            case EBin.event_code_binary.SYNCPARTY:
                this.SetupPartyUID();
                return 0;
            case EBin.event_code_binary.VRP:
                short x2 = 0;
                short y2 = 0;
                this.fieldmap.EBG_sceneGetVRP(ref x2, ref y2);
                this.sSysX = (int)x2;
                this.sSysY = (int)y2;
                return 0;
            case EBin.event_code_binary.CLOSEALL:
                this.eTb.YWindow_CloseAll();
                return 0;
            case EBin.event_code_binary.WIPERGB:
                int num77 = this.getv1();
                int frame = this.getv1();
                this.getv1();
                int num78 = this.getv1();
                int num79 = this.getv1();
                int num80 = this.getv1();
                SceneDirector.InitFade((num77 >> 1 & 1) != 0 ? FadeMode.Sub : FadeMode.Add, frame, (Color32)new Color((float)num78 / (float)byte.MaxValue, (float)num79 / (float)byte.MaxValue, (float)num80 / (float)byte.MaxValue));
                return 0;
            case EBin.event_code_binary.BGVALPHA:
                this.fieldmap.EBG_overlayDefineViewportAlpha((uint)this.getv1(), this.getv2(), this.getv2());
                return 0;
            case EBin.event_code_binary.SLEEPON:
                this._context.idletimer = (short)0;
                return 0;
            case EBin.event_code_binary.HEREON:
                EIcon.SetHereIcon(this.getv1());
                return 0;
            case EBin.event_code_binary.DASHON:
                this._context.dashinh = (byte)0;
                return 0;
            case EBin.event_code_binary.SETHP:
                int num81 = this.chr2slot(this.getv1());
                if (num81 >= 0 && num81 < 9)
                {
                    int index4 = num81;
                    PLAYER player = FF9StateSystem.Common.FF9.player[index4];
                    int num13 = this.getv2();
                    if (num13 > (int)player.max.hp)
                        num13 = (int)player.max.hp;
                    player.cur.hp = (ushort)num13;
                    FF9StateSystem.Common.FF9.player[index4] = player;
                }
                return 0;
            case EBin.event_code_binary.SETMP:
                int num82 = this.chr2slot(this.getv1());
                if (num82 >= 0 && num82 < 9)
                {
                    int index4 = num82;
                    PLAYER player = FF9StateSystem.Common.FF9.player[index4];
                    int num13 = this.getv2();
                    if (num13 > (int)player.max.mp)
                        num13 = (int)player.max.mp;
                    player.cur.mp = (short)num13;
                    FF9StateSystem.Common.FF9.player[index4] = player;
                }
                return 0;
            case EBin.event_code_binary.CLEARAP:
                ff9abil.FF9Abil_ClearAp(this.chr2slot(this.getv1()), this.getv1());
                return 0;
            case EBin.event_code_binary.MAXAP:
                ff9abil.FF9Abil_SetMaster(this.chr2slot(this.getv1()), this.getv1());
                return 0;
            case EBin.event_code_binary.GAMEOVER:
                return 8;
            case EBin.event_code_binary.VIBSTART:
                vib.VIB_vibrate((short)this.getv1());
                return 0;
            case EBin.event_code_binary.VIBACTIVE:
                vib.VIB_setActive(this.getv1() != 0);
                return 0;
            case EBin.event_code_binary.VIBTRACK1:
                vib.VIB_setTrackActive(this.getv1(), this.getv1(), this.getv1() != 0);
                return 0;
            case EBin.event_code_binary.VIBTRACK:
                vib.VIB_setTrackToModulate((uint)this.getv1(), (uint)this.getv1(), this.getv1() != 0);
                return 0;
            case EBin.event_code_binary.VIBRATE:
                vib.VIB_setFrameRate((short)this.getv2());
                return 0;
            case EBin.event_code_binary.VIBFLAG:
                vib.VIB_setFlags((short)this.getv1());
                return 0;
            case EBin.event_code_binary.VIBRANGE:
                vib.VIB_setPlayRange((short)this.getv1(), (short)this.getv1());
                return 0;
            case EBin.event_code_binary.HINT:
                num1 = this.getv1();
                num3 = this.getv2();
                return 0;
            case EBin.event_code_binary.JOIN:
                int slot_no = this.chr2slot(this.getv1());
                int num83 = this.getv1();
                int eqp_id = this.getv1();
                if (slot_no >= 0 && slot_no < 9)
                {
                    PLAYER player = FF9StateSystem.Common.FF9.player[slot_no];
                    int num13 = this.getv1();
                    if (num13 != (int)byte.MaxValue)
                        player.category = (byte)num13;
                    int num53 = this.getv1();
                    if (num53 != (int)byte.MaxValue)
                        player.info.menu_type = (byte)num53;
                    ff9play.FF9Play_Change(slot_no, num83 != 0, eqp_id);
                }
                else
                {
                    this.getv1();
                    this.getv1();
                }
                return 0;
            case EBin.event_code_binary.EXT:
                int num84 = this.gArgFlag | 65280;
                this.gArgFlag = this.geti();
                switch (num84)
                {
                    case 65280:
                        num1 = this.getv1();
                        num3 = this.getv1();
                        return 0;
                    case 65281:
                        num1 = this.getv1();
                        num3 = this.getv1();
                        return 0;
                    case 65282:
                        this.fieldmap.walkMesh.BGI_simSetActive((uint)this.getv1(), (uint)this.getv1());
                        return 0;
                    case 65283:
                        this.fieldmap.walkMesh.BGI_simSetFlags((uint)this.getv1(), (uint)this.getv1());
                        return 0;
                    case 65284:
                        this.fieldmap.walkMesh.BGI_simSetFloor((uint)this.getv1(), (uint)this.getv1());
                        return 0;
                    case 65285:
                        this.fieldmap.walkMesh.BGI_simSetFrameRate((uint)this.getv1(), (short)this.getv2());
                        return 0;
                    case 65286:
                        this.fieldmap.walkMesh.BGI_simSetAlgorithm((uint)this.getv1(), (uint)this.getv1());
                        return 0;
                    case 65287:
                        this.fieldmap.walkMesh.BGI_simSetDelta((uint)this.getv1(), (short)this.getv2(), (short)this.getv2());
                        return 0;
                    case 65288:
                        this.fieldmap.walkMesh.BGI_simSetAxis((uint)this.getv1(), (uint)this.getv1());
                        return 0;
                    case 65289:
                        num1 = this.getv1();
                        num3 = this.getv1();
                        return 0;
                    case 65290:
                        this.fieldmap.walkMesh.BGI_animShowFrame((uint)this.getv1(), (uint)this.getv1());
                        return 0;
                    case 65291:
                        num1 = this.getv1();
                        num3 = this.getv1();
                        return 0;
                    case 65292:
                        num1 = this.getv1();
                        num3 = this.getv1();
                        return 0;
                    case 65293:
                        num1 = this.getv1();
                        num3 = this.getv2();
                        return 0;
                    case 65294:
                        num1 = this.getv1();
                        num3 = this.getv1();
                        return 0;
                    case 65295:
                        num1 = this.getv1();
                        num2 = this.getv1();
                        num3 = this.getv1();
                        return 0;
                    case 65296:
                        num1 = this.getv1();
                        num2 = this.getv1();
                        num3 = this.getv1();
                        return 0;
                    case 65297:
                        num1 = this.getv1();
                        num3 = this.getv1();
                        return 0;
                    default:
                        return 0;
                }
            default:
                switch (this.gMode)
                {
                    case 1:
                        return this.DoEventCodeField(po, code);
                    case 2:
                        return this.DoEventCodeBattle(po, code);
                    case 3:
                        return this.DoEventCodeWorld(po, code);
                    default:
                        goto label_832;
                }
        }
    }

    [DebuggerHidden]
    private IEnumerator DelayedCFLAG(Obj obj, int cflag)
    {
        yield return new WaitForEndOfFrame();
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            yield return new WaitForEndOfFrame();
        obj.flags = (byte)((obj.flags & -64) | (cflag & 63));
    }

    private void StartTurn(Actor actor, float a, bool opt, int tspeed)
    {
        Vector3 vector3_1 = actor.rotAngle;
        float num1 = EventEngineUtils.ClampAngle(vector3_1.y);
        actor.turnRot = EventEngineUtils.ClampAngle(a);
        a -= num1;
        if (opt)
        {
            if ((double)a > 180.0 && !EventEngineUtils.nearlyEqual(a, 180f))
                a -= 360f;
            else if ((double)a < -180.0 && !EventEngineUtils.nearlyEqual(a, -180f))
                a += 360f;
        }
        if (EventEngineUtils.nearlyEqual(a, 0.0f))
            return;
        actor.lastAnim = actor.anim;
        actor.flags |= (byte)128;
        actor.inFrame = (byte)0;
        actor.outFrame = byte.MaxValue;
        actor.trot = num1;
        if (tspeed == 0)
            tspeed = 16;
        int num2;
        if ((double)a < 0.0)
        {
            int charAnimFrame = EventEngineUtils.GetCharAnimFrame(actor.go, (int)actor.turnl);
            num2 = (charAnimFrame << 4) / tspeed;
            if (-(double)a < 90.0)
            {
                float num3 = (float)num2 * (float)(-(double)a / 90.0);
                num2 = (int)((double)num2 * (-(double)a / 90.0));
            }
            if (num2 > 5)
            {
                if (IsCaseShiftWhenTurn(actor))
                {
                    byte num3 = (byte)EventEngineUtils.GetCharAnimFrame(actor.go, (int)actor.turnl);
                    Quaternion localRotation = actor.go.transform.localRotation;
                    Vector3 vector3_2 = new Vector3(actor.go.transform.eulerAngles.x, 90f, actor.go.transform.eulerAngles.z);
                    actor.go.transform.eulerAngles = vector3_2;
                    Vector3 rootBonePos1 = this.GetRootBonePos(actor, (int)actor.turnl, charAnimFrame);
                    vector3_2.y = 0.0f;
                    actor.go.transform.eulerAngles = vector3_2;
                    Vector3 rootBonePos2 = this.GetRootBonePos(actor, (int)actor.idle, 0);
                    actor.offsetTurn = rootBonePos2 - rootBonePos1;
                    actor.offsetTurn = Quaternion.Euler(0.0f, actor.turnRot, 0.0f) * actor.offsetTurn;
                    actor.go.transform.localRotation = localRotation;
                }
                actor.turnAdd = (a + 90f) / (float)num2;
                actor.animFlag = (byte)0;
                this.ExecAnim(actor, (int)actor.turnl);
                actor.aspeed = (byte)((charAnimFrame << 4) / num2);
            }
            else
            {
                if ((double)actor.turnRot > 180.0)
                    actor.turnRot -= 360f;
                else if ((double)actor.turnRot < -180.0)
                    actor.turnRot += 360f;
                actor.turnAdd = 32766f;
                vector3_1.y = num1 + ((double)actor.turnRot < (double)num1 ? 0.0f : 360f);
                actor.rotAngle[1] = vector3_1.y;
            }
        }
        else
        {
            int charAnimFrame = EventEngineUtils.GetCharAnimFrame(actor.go, (int)actor.turnr);
            num2 = (charAnimFrame << 4) / tspeed;
            if ((double)a < 90.0)
            {
                float num3 = (float)num2 * (a / 90f);
                num2 = (int)((double)num2 * ((double)a / 90.0));
            }
            if (num2 > 5)
            {
                if (IsCaseShiftWhenTurn(actor))
                {
                    Quaternion localRotation = actor.go.transform.localRotation;
                    Vector3 vector3_2 = new Vector3(actor.go.transform.eulerAngles.x, -90f, actor.go.transform.eulerAngles.z);
                    actor.go.transform.eulerAngles = vector3_2;
                    Vector3 rootBonePos1 = this.GetRootBonePos(actor, (int)actor.turnr, charAnimFrame);
                    vector3_2.y = 0.0f;
                    actor.go.transform.eulerAngles = vector3_2;
                    Vector3 rootBonePos2 = this.GetRootBonePos(actor, (int)actor.idle, 0);
                    actor.offsetTurn = rootBonePos2 - rootBonePos1;
                    actor.offsetTurn = Quaternion.Euler(0.0f, actor.turnRot, 0.0f) * actor.offsetTurn;
                    actor.go.transform.localRotation = localRotation;
                }
                actor.turnAdd = (a - 90f) / (float)num2;
                actor.animFlag = (byte)0;
                this.ExecAnim(actor, (int)actor.turnr);
                actor.aspeed = (byte)((charAnimFrame << 4) / num2);
            }
            else
            {
                if ((double)actor.turnRot > 180.0)
                    actor.turnRot -= 360f;
                else if ((double)actor.turnRot < -180.0)
                    actor.turnRot += 360f;
                actor.turnAdd = (float)short.MaxValue;
                vector3_1.y = num1 - ((double)actor.turnRot > (double)num1 ? 0.0f : 360f);
                actor.rotAngle[1] = vector3_1.y;
            }
        }
        if (num2 < 1)
            num2 = 1;
        actor.trotAdd = a / (float)num2;
    }

    private Vector3 GetRootBonePos(Actor actor, int animID, int frameIndex)
    {
        string animation = FF9DBAll.AnimationDB[animID];
        Animation component = actor.fieldMapActorController.GetComponent<Animation>();
        component.Play(animation);
        int num = (int)(byte)EventEngineUtils.GetCharAnimFrame(actor.go, animID);
        component[animation].time = (float)frameIndex;
        component.Sample();
        return actor.fieldMapActorController.transform.FindChild("bone000").position;
    }

    private static bool IsCaseShiftWhenTurn(Actor currentActor)
    {
        int num = (int)FF9StateSystem.Common.FF9.fldMapNo;
        PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
        int varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
        return (num == 50 || num == 52 || num == 60) && (int)currentActor.model == 5464 || (num == 66 && (int)currentActor.model == 5501 || num == 562 && varManually == 99 && (int)currentActor.model == 5476) || (num >= 1051 && num <= 1059 || num >= 1103 && num <= 1109 && (int)currentActor.model == 269);
    }

    private bool requestAcceptable(Obj p, int lv)
    {
        if (p != null)
            return lv < (int)p.level;
        return false;
    }

    private Obj GetObj1()
    {
        return this.GetObjUID(this.getv1());
    }

    private void FF9FieldDiscRequest(byte disc_id, ushort map_id)
    {
        //this._ff9fieldDisc.disc_id = disc_id;
        //this._ff9fieldDisc.cdType = (byte)(1U << (int)disc_id);
        this._ff9fieldDisc.FieldMapNo = (short)map_id;
        //this._ff9fieldDisc.FieldLocNo = (short)-1;
        FF9StateFieldSystem stateFieldSystem = FF9StateSystem.Field.FF9Field;
        FF9StateSystem instance = PersistenSingleton<FF9StateSystem>.Instance;
        stateFieldSystem.attr |= 1048576U;
        instance.attr |= 8U;
    }

    private void SetActorPosition(PosObj po, float x, float y, float z)
    {
        po.pos[0] = po.lastx = x;
        po.pos[1] = po.lasty = y;
        po.pos[2] = po.lastz = z;
        if (this.gMode == 1)
        {
            FieldMapActorController mapActorController = ((Actor)po).fieldMapActorController;
            if (!((UnityEngine.Object)mapActorController != (UnityEngine.Object)null))
                return;
            mapActorController.SetPosition(new Vector3(po.pos[0], po.pos[1], po.pos[2]), true, true);
        }
        else
        {
            if (this.gMode != 3)
                return;
            if ((int)po.uid != (int)this._context.controlUID)
                ;
            ((Actor)po).wmActor.SetPosition(po.pos[0], po.pos[1], po.pos[2]);
            if ((int)po.index < 3 || (int)po.index > 7)
                return;
            int posX = (int)po.pos[0];
            int posY = (int)po.pos[1];
            int posZ = (int)po.pos[2];
            ff9.w_movementChrVerifyValidCastPosition(ref posX, ref posY, ref posZ);
            po.pos[0] = (float)posX;
            po.pos[1] = (float)posY;
            po.pos[2] = (float)posZ;
            ((Actor)po).wmActor.SetPosition(po.pos[0], po.pos[1], po.pos[2]);
        }
    }
}