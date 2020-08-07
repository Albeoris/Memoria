using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria.Field;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Memoria;
using Memoria.Prime;
using Memoria.Assets;
using Memoria.Data;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = System.Object;

public partial class EventEngine
{
    public Int32 DoEventCode()
    {
        Actor actor1 = (Actor)null;
        GameObject gameObject = (GameObject)null;
        PosObj po = (PosObj)null;
        FieldMapActorController fmac = (FieldMapActorController)null;
        if ((Int32)this.gCur.cid == 4)
        {
            actor1 = (Actor)this.gCur;
            gameObject = actor1.go;
            po = (PosObj)this.gCur;
        }
        this._lastIP = this.gExec.ip;
        Int32 code = this.geti();
        this.gArgFlag = this.geti();
        this.gArgUsed = 0;
        EBin.event_code_binary eventCodeBinary = (EBin.event_code_binary)code;
        Vector3 eulerAngles1;
        Int32 num1;
        Int32 num2;
        Int32 num3;

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
                Int32 sid1 = this.gArgFlag;
                Int32 uid1 = this.geti();
                if (sid1 >= 251 && sid1 < (Int32)Byte.MaxValue)
                {
                    sid1 = (Int32)this._context.partyUID[sid1 - 251];

                    if (sid1 == Byte.MaxValue)
                    {
                        Log.Warning($"[EventEnginge] Failed to perform an event code [NEW3] because there is no party member with index {this.gArgFlag}");
                        return 0;
                    }
                }

                Actor actor2 = new Actor(sid1, uid1, EventEngine.sizeOfActor);
                if (this.gMode == 3)
                    Singleton<WMWorld>.Instance.addWMActorOnly((Actor)actor2);
                if (this.gMode == 1)
                    this.turnOffTriManually(sid1);
                this.gArgUsed = 1;
                return 0;
            case EBin.event_code_binary.REQ:
                Int32 level = this.getv1();
                Obj p1 = this.GetObj1();
                Int32 tag1 = this.geti();
                this.Request(p1, level, tag1, false);
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 900 && p1 != null && (level == 2 && tag1 == 11) && (Int32)p1.uid == 14)
                    this.fieldmap.walkMesh.BGI_triSetActive(62U, 1U);
                return 0;
            case EBin.event_code_binary.REQSW:
                Int32 num4 = this.getv1();
                Obj p2 = this.GetObj1();
                Int32 tag2 = this.geti();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2803 && p2 != null && (tag2 == 18 && (Int32)p2.uid == 20))
                {
                    this.fieldmap.walkMesh.BGI_triSetActive(105U, 1U);
                    this.fieldmap.walkMesh.BGI_triSetActive(106U, 1U);
                }
                if (this.requestAcceptable(p2, num4))
                {
                    this.Request(p2, num4, tag2, false);
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 262)
                    {
                        this._geoTexAnim = this.GetObjUID(12).go.GetComponent<GeoTexAnim>();
                        if ((Int32)p2.sid == 10 && tag2 == 12)
                        {
                            this._geoTexAnim.geoTexAnimStop(2);
                            this._geoTexAnim.geoTexAnimPlay(0);
                        }
                        else if ((Int32)p2.sid == 12 && tag2 == 20)
                            this._geoTexAnim.geoTexAnimPlay(2);
                    }
                    return 0;
                }
                this.stay();
                return 1;
            case EBin.event_code_binary.REQEW:
                Int32 num5 = this.getv1();
                Obj p3 = this.GetObj1();
                Int32 tag3 = this.geti();
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
                Int32 num6 = this.getv1();
                Int32 tag4 = this.geti();
                if (this.requestAcceptable(this.getSender(this.gExec), num6))
                {
                    this.Request(this.getSender(this.gExec), num6, tag4, false);
                    return 0;
                }
                this.stay();
                return 1;
            case EBin.event_code_binary.REPLYEW:
                Int32 num7 = this.getv1();
                Int32 tag5 = this.geti();
                this.getSender(this.gExec);
                if (this.requestAcceptable(this.getSender(this.gExec), num7))
                    this.Request(this.getSender(this.gExec), num7, tag5, true);
                else
                    this.stay();
                return 1;
            case EBin.event_code_binary.SONGFLAG:
                if (this.getv1() != 0)
                    FF9StateSystem.Common.FF9.btl_flag |= (Byte)8;
                else
                    FF9StateSystem.Common.FF9.btl_flag &= (Byte)247;
                return 0;
            case EBin.event_code_binary.POS:
            case EBin.event_code_binary.DPOS:
                if (eventCodeBinary == EBin.event_code_binary.DPOS)
                    po = (PosObj)this.GetObj1();
                Int32 num8 = this.getv2();
                Int32 num9 = this.getv2();
                Int32 num10 = this.gMode != 1 || (Int32)po.model == (Int32)UInt16.MaxValue ? 0 : 1;
                if (num10 == 1 && po != null)
                {
                    FieldMapActorController component = po.go.GetComponent<FieldMapActorController>();
                    if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.walkMesh != null)
                    {
                        component.walkMesh.BGI_charSetActive(component, 1U);
                        if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2050 && (Int32)po.sid == 5)
                            component.walkMesh.BGI_charSetActive(component, 0U);
                        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2917 && (Int32)po.sid == 4)
                        {
                            if (num8 == 0 && num9 == -1787)
                                num8 = -15;
                        }
                        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 450 && (Int32)po.sid == 3 && (num8 == 363 && num9 == 88))
                            component.walkMesh.BGI_triSetActive(24U, 0U);
                    }
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1421 && (Int32)po.sid == 5)
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
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 563 && (Int32)po.sid == 16)
                    {
                        if (num8 == -1614)
                            num8 = -1635;
                    }
                    else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1310 && (Int32)po.sid == 12)
                    {
                        if (num8 == -1614)
                            num8 = -1635;
                    }
                    else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2110 && (Int32)po.sid == 9)
                    {
                        if (num8 == -1614)
                            num8 = -1635;
                    }
                    else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 572 && (Int32)po.sid == 16 && num8 == -1750)
                        num8 = -1765;
                }
                this.SetActorPosition(po, (Single)num8, this.POS_COMMAND_DEFAULTY, (Single)num9);
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2050 && (Int32)po.sid == 5 && (num10 == 1 && po != null))
                {
                    FieldMapActorController component = po.go.GetComponent<FieldMapActorController>();
                    if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.walkMesh != null)
                        component.walkMesh.BGI_charSetActive(component, 1U);
                }
                if ((Int32)po.cid == 4)
                    this.clrdist((Actor)po);
                this._posUsed = true;
                return 0;
            case EBin.event_code_binary.BGVPORT:
                this.fieldmap.EBG_cameraSetViewport((UInt32)this.getv1(), (Int16)this.getv2(), (Int16)this.getv2(), (Int16)this.getv2(), (Int16)this.getv2());
                return 0;
            case EBin.event_code_binary.MES:
                this.gCur.winnum = (Byte)this.getv1();
                Int32 flags1 = this.getv1();
                this.SetFollow(this.gCur, (Int32)this.gCur.winnum, flags1);
                Int32 index1 = this.getv2();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1757 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6740 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 30)
                {
                    this.stay();
                    return 1;
                }
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1060)
                {
                    String symbol = Localization.GetSymbol();
                    Dictionary<Int32, Int32> dictionary = (Dictionary<Int32, Int32>)null;
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
                if (FF9StateSystem.Common.FF9.fldMapNo == 2172 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) < 9100 && Localization.GetSymbol() == "JP" && index1 == 91 && this.gCur.sid == 1 && this.gCur.ip == 145 && EIcon.AIconMode == 0)
                {
                    DialogManager.SelectChoice = 15;
                    return 0;
                }
                this.eTb.NewMesWin(index1, (Int32)this.gCur.winnum, flags1, !this.isPosObj(this.gCur) ? (PosObj)null : (PosObj)this.gCur);
                this.gCur.wait = (Byte)254;
                return 1;
            case EBin.event_code_binary.MESN:
                this.gCur.winnum = (Byte)this.getv1();
                Int32 flags2 = this.getv1();
                this.SetFollow(this.gCur, (Int32)this.gCur.winnum, flags2);
                Int32 index2 = this.getv2();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1757 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6740 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 30)
                {
                    this.stay();
                    return 1;
                }
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1060)
                {
                    String symbol = Localization.GetSymbol();
                    Dictionary<Int32, Int32> dictionary = (Dictionary<Int32, Int32>)null;
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
                this.eTb.NewMesWin(index2, (Int32)this.gCur.winnum, flags2, !this.isPosObj(this.gCur) ? (PosObj)null : (PosObj)this.gCur);
                return 0;
            case EBin.event_code_binary.CLOSE:
                // For the stage
                if (IsAlexandriaStageScene())
                {
                    DialogManager.Instance.ForceControlByEvent(false);
                    goto case EBin.event_code_binary.WAITMES;
                }

                var v1 = this.getv1();
                this.eTb.DisposWindowByID(v1);
                return 0;
            case EBin.event_code_binary.MOVE:
                Int32 num11 = this.getv2();
                Int32 num12 = this.getv2();
                Boolean flag1 = false;
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2954 && (Int32)po.sid == 4)
                {
                    //Int32 num13 = 0;
                    SettingUtils.FieldMapSettings fieldMapSettings = SettingUtils.fieldMapSettings;
                    if (num11 == -2181 && num12 == 14842)
                    {
                        //num13 = 0;
                    }
                    else if (num11 == -2407 && num12 == 14508)
                    {
                        //num13 = 1;
                    }
                    else if (num11 == -1146 && num12 == 13438)
                    {
                        //num13 = 2;
                    }
                    else if (num11 == -1159 && num12 == 13130)
                    {
                        //num13 = 3;
                        num11 = -1275;
                        num12 = 13130;
                    }
                    else if (num11 == -3644 && num12 == 11849)
                    {
                        //num13 = 4;
                        num11 = -3750;
                        num12 = 11849;
                    }
                }
                // TODO Check Native: #147
                else if (FF9StateSystem.Common.FF9.fldMapNo == 2800 && po.sid == 17)
                {
                    num11 = -4702;
                    num12 = 2702;
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 108 && (Int32)po.sid == 2)
                {
                    if (num11 == -111 && num12 == -210)
                    {
                        num11 = -150;
                        num12 = -270;
                    }
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1857 && (Int32)po.sid == 3)
                {
                    if (num11 == -111 && num12 == -210)
                    {
                        num11 = -150;
                        num12 = -270;
                    }
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2101 && (Int32)po.sid == 2)
                {
                    if (num11 == 781 && num12 == 1587)
                    {
                        num11 = 805;
                        num12 = 1564;
                    }
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 401 && (Int32)po.sid == 2)
                {
                    if (num11 == -1869 && num12 == -783)
                        num11 = -1782;
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 64)
                {
                    if (PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 1600 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 327 && (num11 == 1111 && num12 == -14400))
                    {
                        num11 = 1261;
                        num12 = -14550;
                    }
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 307 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 2520 && ((Int32)po.sid == 12 || (Int32)po.sid == 14))
                {
                    if (num11 == 636 && num12 == -1359)
                        num11 = 781;
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 563 && (Int32)po.sid == 16)
                {
                    if (num11 == -1614)
                        num11 = -1635;
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1310 && (Int32)po.sid == 12)
                {
                    if (num11 == -1614)
                        num11 = -1635;
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2110 && (Int32)po.sid == 9)
                {
                    if (num11 == -1614)
                        num11 = -1635;
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 572 && (Int32)po.sid == 16)
                {
                    if (num11 == -1750)
                        num11 = -1765;
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1601 && (Int32)po.uid == 130)
                {
                    if (num11 == -1705 && num12 == -1233)
                    {
                        num11 = -1680;
                        num12 = -1140;
                    }
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 915)
                {
                    if ((Int32)po.sid == 3)
                    {
                        if (num11 == -10336 && num12 == -7750)
                        {
                            if ((Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                                flag1 = true;
                        }
                        else if (num11 == -8746 && num12 == -6516 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                            flag1 = true;
                    }
                    else if ((Int32)po.sid == 4)
                    {
                        if (num11 == -8746 && num12 == -6516)
                        {
                            if ((Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                                flag1 = true;
                        }
                        else if (num11 == -8967 && num12 == -6173)
                        {
                            if ((Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                                flag1 = true;
                        }
                        else if (num11 == -10498 && num12 == 2678 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                            flag1 = true;
                    }
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1915)
                {
                    if ((Int32)po.sid == 3)
                    {
                        if (num11 == -10336 && num12 == -7750)
                        {
                            if ((Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                                flag1 = true;
                        }
                        else if (num11 == -8746 && num12 == -6516 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                            flag1 = true;
                    }
                    else if ((Int32)po.sid == 4)
                    {
                        if (num11 == -8967 && num12 == -6173)
                        {
                            if ((Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                                flag1 = true;
                        }
                        else if (num11 == -10498 && num12 == 2678 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                            flag1 = true;
                    }
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 550)
                {
                    if ((Int32)po.sid == 14 && num11 == 348 && num12 == -2500 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                        flag1 = true;
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 101)
                {
                    if ((Int32)po.sid == 7 && num11 == -4000 && num12 == -400 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                        flag1 = true;
                    if ((Int32)po.sid == 8 && num11 == -4000 && num12 == -200 && (Double)(new Vector2(po.pos[0], po.pos[2]) - new Vector2((Single)num11, (Single)num12)).sqrMagnitude > 100.0)
                        flag1 = true;
                }
                else if (FF9StateSystem.Common.FF9.fldMapNo == 1800 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 10100 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 1 && actor1.sid == 11 && num11 == 510 && num12 == 3054)
                {
                    num12 = 2970;
                }
                Boolean flag2 = this.MoveToward_mixed((Single)num11, 0.0f, (Single)num12, 0, (PosObj)null);
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
                actor1.loopCount = Byte.MaxValue;
                this.clrdist(actor1);
                return 0;
            case EBin.event_code_binary.MSPEED:
                Byte num14 = (Byte)this.getv1();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 3010)
                {
                    String symbol = Localization.GetSymbol();
                    if (!(symbol != "US") || !(symbol != "JP"))
                    {
                        if (symbol == "US" && (Int32)actor1.sid == 17)
                        {
                            if ((Int32)num14 == 15)
                                num14 = (Byte)20;
                            else if ((Int32)num14 == 23)
                                num14 = (Byte)25;
                        }
                        if (symbol == "JP" && (Int32)actor1.sid == 16)
                        {
                            if ((Int32)num14 == 15)
                                num14 = (Byte)20;
                            else if ((Int32)num14 == 23)
                                num14 = (Byte)25;
                        }
                    }
                }
                if (FF9StateSystem.Common.FF9.fldMapNo == 658 && actor1.sid == 18 && num14 == 30 && actor1.ip == 323)
                {
                    num14 = 25;
                }
                actor1.speed = num14;
                return 0;
            case EBin.event_code_binary.BGIMASK:
                this.BGI_systemSetAttributeMask((Byte)this.getv1());
                return 0;
            case EBin.event_code_binary.FMV:
                Singleton<fldfmv>.Instance.FF9FieldFMVDispatch(this.getv1(), this.getv2(), this.getv1());
                return 0;
            case EBin.event_code_binary.QUAD:
                Int32 index3 = 0;
                ((Quad)this.gCur).n = this.getv1();
                Int32 num15 = ((Quad)this.gCur).n;
                while (num15 != 0)
                {
                    QuadPos quadPos = ((Quad)this.gCur).q[index3];
                    quadPos.X = (Int16)this.getv2();
                    quadPos.Z = (Int16)this.getv2();
                    if (((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1608 || (Int32)FF9StateSystem.Common.FF9.fldMapNo == 1707) && (Int32)quadPos.Z == -257)
                        quadPos.Z = (short)(-157);
                    --num15;
                    ++index3;
                }
                //if ((Int32)this.gCur.sid != 4)
                //    ;
                return 0;
            case EBin.event_code_binary.ENCOUNT:
                this.getv1(); // rush_type
                this._ff9.btlSubMapNo = -1;
                Int32 num16 = this.getv2();
                this._ff9.steiner_state = (Byte)(num16 >> 15 & 1);
                this.SetBattleScene(num16 & (Int32)Int16.MaxValue);
                FF9StateSystem.Battle.isRandomEncounter = false;
                return 3;
            case EBin.event_code_binary.MAPJUMP:
                this.SetNextMap(this.getv2());
                return 4;
            case EBin.event_code_binary.CC:
                Actor activeActorByUid = this.getActiveActorByUID((Int32)this._context.controlUID);
                if (activeActorByUid != null && this.gMode == 1)
                {
                    if (FF9StateSystem.Common.FF9.fldMapNo == 1607)
                    {
                        if (this.gExec.uid != activeActorByUid.uid)
                        {
                            activeActorByUid.fieldMapActorController.isPlayer = false;
                            activeActorByUid.fieldMapActorController.gameObject.name = "obj" + activeActorByUid.uid;
                        }
                    }
                    else
                    {
                        activeActorByUid.fieldMapActorController.isPlayer = false;
                        activeActorByUid.fieldMapActorController.gameObject.name = "obj" + activeActorByUid.uid;
                    }
                }
                this._context.controlUID = this.gExec.uid;
                return 0;
            case EBin.event_code_binary.UCOFF:
                this._context.usercontrol = (Byte)0;
                EIcon.SetHereIcon(0);
                this.eTb.gMesCount = this.gAnimCount = 0;
                PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
                if (this.gMode == 3)
                    UIManager.World.SetMinimapPressable(false);
                else if (this.gMode == 1)
                {
                    Obj objUid = this.GetObjUID(250);
                    if (objUid != null && (Int32)objUid.cid == 4)
                        ((Actor)objUid).fieldMapActorController.ClearMoveTargetAndPath();
                }
                if (!EMinigame.CheckChocoboVirtual())
                    PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (System.Action)null);
                return 0;
            case EBin.event_code_binary.UCON:
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
            case EBin.event_code_binary.MODEL:
                po.model = (UInt16)this.getv2();
                this.gExec.flags |= (Byte)1;
                po.eye = (Int16)(-4 * this.getv1());
                if (this.gMode == 1)
                {
                    String str = FF9BattleDB.GEO.GetValue((Int32)po.model);

                    po.go = ModelFactory.CreateModel(str, false);
                    GeoTexAnim.addTexAnim(po.go, str);
                    if (ModelFactory.garnetShortHairTable.Contains(str))
                    {
                        po.garnet = true;
                        UInt16 uint16 = BitConverter.ToUInt16(FF9StateSystem.EventState.gEventGlobal, 0);
                        po.shortHair = (Int32)uint16 >= 10300;
                    }
                    if (po.go != (UnityEngine.Object)null)
                    {
                        Int32 length = 0;
                        foreach (UnityEngine.Object child in po.go.transform)
                        {
                            if (child.name.Contains("mesh"))
                                ++length;
                        }

                        if (po.garnet)
                            ++length;

                        po.meshIsRendering = new Boolean[length];
                        for (Int32 index4 = 0; index4 < length; ++index4)
                            po.meshIsRendering[index4] = true;

                        FF9Char ff9Char = new FF9Char();
                        ff9Char.geo = po.go;
                        ff9Char.evt = po;
                        if (FF9StateSystem.Common.FF9.charArray.ContainsKey((Int32)po.uid))
                            return 0;
                        FF9StateSystem.Common.FF9.charArray.Add((Int32)po.uid, ff9Char);
                        FF9FieldCharState ff9FieldCharState = new FF9FieldCharState();
                        FF9StateSystem.Field.FF9Field.loc.map.charStateArray.Add((Int32)po.uid, ff9FieldCharState);
                        FF9StateSystem.Field.FF9Field.loc.map.shadowArray.Add((Int32)po.uid, new FF9Shadow());
                        Obj obj = (Obj)po;
                        obj.go.name = "obj" + (Object)po.uid;
                        this.fieldmap.AddFieldChar(obj.go, po.posField, po.rotField, false, (Actor)obj, false);
                    }
                }
                else if (this.gMode == 3)
                {
                    po.go = ModelFactory.CreateModel(FF9BattleDB.GEO.GetValue((Int32)po.model), false);
                    Singleton<WMWorld>.Instance.addGameObjectToWMActor(po.go, ((Actor)po).wmActor);
                }
                return 0;
            case EBin.event_code_binary.AIDLE:
                actor1.idle = (UInt16)this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB.GetValue((Int32)actor1.idle));
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2365 && (Int32)actor1.uid == 14 && (Int32)actor1.idle == 11611)
                {
                    this._geoTexAnim = actor1.go.GetComponent<GeoTexAnim>();
                    this._geoTexAnim.geoTexAnimStop(2);
                    this._geoTexAnim.geoTexAnimPlay(0);
                }
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2657 && (Int32)actor1.uid == 7)
                {
                    this._geoTexAnim = actor1.go.GetComponent<GeoTexAnim>();
                    if ((Int32)actor1.idle == 1044)
                    {
                        this._geoTexAnim.geoTexAnimStop(2);
                        this._geoTexAnim.geoTexAnimPlay(0);
                    }
                    else if ((Int32)actor1.idle == 816)
                        this._geoTexAnim.geoTexAnimPlay(2);
                }
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1605 && (Int32)actor1.uid == 18)
                {
                    this._geoTexAnim = actor1.go.GetComponent<GeoTexAnim>();
                    if ((Int32)actor1.idle == 7503)
                        this._geoTexAnim.geoTexAnimPlay(2);
                }
                return 0;
            case EBin.event_code_binary.AWALK:
                actor1.walk = (UInt16)this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB.GetValue((Int32)actor1.walk));
                return 0;
            case EBin.event_code_binary.ARUN:
                actor1.run = (UInt16)this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB.GetValue((Int32)actor1.run));
                return 0;
            case EBin.event_code_binary.DIRE:
            case EBin.event_code_binary.DDIR:
                if (eventCodeBinary == EBin.event_code_binary.DDIR)
                    po = (PosObj)this.GetObj1();
                Int32 num17 = this.getv1();
                if (po == null || (UnityEngine.Object)po.go == (UnityEngine.Object)null)
                    return 0;
                if (this.gMode == 1)
                {
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 504 && (Int32)po.sid == 15 && (this.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 4 && num17 == 240))
                        num17 = 128;
                    Vector3 eulerAngles2 = po.go.transform.localRotation.eulerAngles;
                    eulerAngles2.y = EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)(num17 << 4));
                    po.rotAngle[1] = eulerAngles2.y;
                }
                else if (this.gMode == 3)
                {
                    Vector3 rot = ((Actor)po).wmActor.rot;
                    rot.y = EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)(num17 << 4));
                    ((Actor)po).wmActor.rot = rot;
                }
                return 0;
            case EBin.event_code_binary.ROTXZ:
                Int32 num18 = (Int32)(Int16)(this.getv1() << 4);
                Int32 num19 = (Int32)(Int16)(this.getv1() << 4);
                Single num20 = EventEngineUtils.ClampAngle(EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)num18));
                Single num21 = EventEngineUtils.ClampAngle(EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)num19));
                po.rotAngle[0] = num20;
                po.rotAngle[2] = num21;
                return 0;
            case EBin.event_code_binary.BTLCMD:
                switch (this.gExec.level)
                {
                    case 0:
                        btl_cmd.SetEnemyCommand((UInt16)this.GetSysList(1), (UInt16)this.GetSysList(0), BattleCommandId.EnemyDying, (UInt32)this.getv1());
                        break;
                    case 1:
                        btl_cmd.SetEnemyCommand((UInt16)this.GetSysList(1), (UInt16)this.GetSysList(0), BattleCommandId.EnemyCounter, (UInt32)this.getv1());
                        break;
                    case 3:
                        this.gExec.btlchk = (Byte)0;
                        btl_cmd.SetEnemyCommand((UInt16)this.GetSysList(1), (UInt16)this.GetSysList(0), BattleCommandId.EnemyAtk, (UInt32)this.getv1());
                        break;
                }
                return 0;
            case EBin.event_code_binary.MESHSHOW:
                PosObj posObj1 = (PosObj)this.GetObj1();
                Int32 mesh1 = this.getv1();
                if (posObj1 != null)
                    posObj1.geoMeshShow(mesh1);
                return 0;
            case EBin.event_code_binary.MESHHIDE:
                PosObj posObj2 = (PosObj)this.GetObj1();
                Int32 mesh2 = this.getv1();
                if (posObj2 != null)
                    posObj2.geoMeshHide(mesh2);
                return 0;
            case EBin.event_code_binary.OBJINDEX:
                this.gExec.index = (Byte)this.getv1();
                return 0;
            case EBin.event_code_binary.ENCSCENE:
                this._enCountData.pattern = (Byte)this.getv1();
                this._enCountData.scene[0] = (UInt16)this.getv2();
                this._enCountData.scene[1] = (UInt16)this.getv2();
                this._enCountData.scene[2] = (UInt16)this.getv2();
                this._enCountData.scene[3] = (UInt16)this.getv2();
                return 0;
            case EBin.event_code_binary.AFRAME:
                actor1.inFrame = (Byte)this.getv1();
                actor1.outFrame = (Byte)this.getv1();
                return 0;
            case EBin.event_code_binary.ASPEED:
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo >= 3009 && (Int32)FF9StateSystem.Common.FF9.fldMapNo <= 3011)
                {
                    this.getv1();
                    return 0;
                }
                actor1.aspeed0 = (Byte)this.getv1();
                return 0;
            case EBin.event_code_binary.AMODE:
                Int32 num22 = this.getv1() << 3 & (EventEngine.afHold | EventEngine.afLoop | EventEngine.afPalindrome);
                actor1.animFlag = (Byte)num22;
                actor1.loopCount = (Byte)this.getv1();
                return 0;
            case EBin.event_code_binary.ANIM:
                Int32 anim = this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB.GetValue(anim));
                if (this.gMode == 1)
                {
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 800 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 3740 && anim == 13158)
                        actor1.inFrame = (Byte)19;
                    this.ExecAnim(actor1, anim);
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2934 && (Int32)actor1.uid == 2 && anim == 12429)
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
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1605 && (Int32)actor1.uid == 18)
                    {
                        this._geoTexAnim = actor1.go.GetComponent<GeoTexAnim>();
                        if (anim == 11958)
                        {
                            this._geoTexAnim.geoTexAnimStop(2);
                            this._geoTexAnim.geoTexAnimPlay(0);
                        }
                    }
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 307 && (Int32)actor1.uid == 13)
                    {
                        this._geoTexAnim = actor1.go.GetComponent<GeoTexAnim>();
                        if (anim == 10328)
                        {
                            this._geoTexAnim.geoTexAnimStop(2);
                            this._geoTexAnim.geoTexAnimPlay(0);
                        }
                    }
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 561 && (Int32)actor1.uid == 6)
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
                if (((Int32)actor1.animFlag & EventEngine.afExec) == 0)
                    return 0;
                this.stay();
                return 1;
            case EBin.event_code_binary.ENDANIM:
                this.AnimStop(actor1);
                return 0;
            case EBin.event_code_binary.STARTSEQ:
                Int32 uid2 = (Int32)this.gExec.uid + EventEngine.cSeqOfs;
                Obj objByUid1 = this.FindObjByUID(uid2);
                if (objByUid1 != null)
                    this.DisposeObj(objByUid1);
                Int32 sid2 = this.getv1();
                Seq seq = new Seq(sid2, uid2);
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1610)
                {
                    PosObj posObj3 = (PosObj)this.GetObjUID(21);
                    if (posObj3 != null)
                    {
                        Int32 varManually = this.eBin.getVarManually(EBin.MAP_INDEX_SVR);
                        //Debug.Log((object)("map_id = " + (object)varManually));
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
                if (this.FindObjByUID((Int32)this.gExec.uid + EventEngine.cSeqOfs) == null)
                    return 0;
                this.stay();
                return 1;
            case EBin.event_code_binary.ENDSEQ:
                Obj objByUid2 = this.FindObjByUID((Int32)this.gExec.uid + EventEngine.cSeqOfs);
                if (objByUid2 != null)
                    this.DisposeObj(objByUid2);
                return 0;
            case EBin.event_code_binary.DEBUGCC:
                return 0;
            case EBin.event_code_binary.NECKFLAG:
                actor1.actf &= (UInt16)~(EventEngine.actNeckT | EventEngine.actNeckM | EventEngine.actNeckTalk);
                actor1.actf |= (UInt16)(this.getv1() & (EventEngine.actNeckT | EventEngine.actNeckM | EventEngine.actNeckTalk));
                return 0;
            case EBin.event_code_binary.ITEMADD:
                Int32 id1 = this.getv2();
                Int32 count1 = this.getv1();
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
                Int32 id2 = this.getv2();
                Int32 count2 = this.getv1();
                if (id2 < EventEngine.kSItemOfs)
                    ff9item.FF9Item_Remove(id2, count2);
                else if (id2 < EventEngine.kCItemOfs)
                    ff9item.FF9Item_RemoveImportant(id2 - EventEngine.kSItemOfs);
                if (id2 == 324)
                    EMinigame.MognetCentralAchievement();
                return 0;
            case EBin.event_code_binary.BTLSET:
                Int32 num23 = this.getv1();
                Int32 val = this.getv2();
                //if (num23 == 33)
                //    Debug.Log((object)"BTLSET 33");
                btl_scrp.SetBattleData((UInt32)num23, val);
                return 0;
            case EBin.event_code_binary.RADIUS:
                Int32 num24 = this.getv1();
                Int32 num25 = (Int32)(Byte)this.getv1();
                Int32 num26 = (Int32)(Byte)this.getv1();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1823 && (Int32)this.gCur.sid == 13 && (num24 == 8 && num25 == 8) && num26 == 8)
                    num24 = 30;
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 657 && (Int32)this.gCur.sid == 22 && num24 == 40)
                    num24 = 35;
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 164 && (Int32)this.gCur.sid == 7 && num24 == 30)
                    num24 = 20;
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2903 && (Int32)this.gCur.sid == 4 && num24 == 20)
                    num24 = 16;
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 3100 && (Int32)this.gCur.sid == 20 && num24 == 20)
                    num24 = 25;
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1104 && ((Int32)this.gCur.sid == 13 || (Int32)this.gCur.sid == 5))
                {
                    Int32 num13;
                    num26 = num13 = 30;
                    num25 = num13;
                    num24 = num13;
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1108 && (Int32)this.gCur.sid == 40)
                {
                    Int32 num13;
                    num26 = num13 = 30;
                    num25 = num13;
                    num24 = num13;
                }
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1606 && (Int32)this.gCur.sid == 34)
                    num24 = 15;
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2714 && (Int32)this.gCur.sid == 26)
                    num24 = 22;
                po.collRad = (Byte)num25;
                po.talkRad = (Byte)num26;
                if (this.gMode == 1)
                {
                    if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
                        return 0;
                    FieldMapActorController component1 = gameObject.GetComponent<FieldMapActorController>();
                    if ((UnityEngine.Object)component1 != (UnityEngine.Object)null)
                        component1.radius = (Single)(num24 * 4);
                    FieldMapActor component2 = gameObject.GetComponent<FieldMapActor>();
                    if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
                        component2.CharRadius = (Single)(num24 * 4);
                }
                //else if (this.gMode != 3)
                //    ;
                return 0;
            case EBin.event_code_binary.ATTACH:
                Obj sourceObj = this.GetObj1();
                Obj targetObj = this.GetObj1();
                GameObject sourceObject1 = sourceObj.go;
                GameObject targetObject = targetObj.go;
                Int32 bone_index = this.getv1();
                if ((UnityEngine.Object)sourceObject1 != (UnityEngine.Object)null && (UnityEngine.Object)targetObject != (UnityEngine.Object)null)
                {
                    if (this.gMode == 1 || this.gMode == 2)
                    {
                        if ((Int32)this.gCur.sid == 8 && (Int32)FF9StateSystem.Common.FF9.fldMapNo == 62 || (Int32)this.gCur.sid == 2 && (Int32)FF9StateSystem.Common.FF9.fldMapNo == 3010)
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
                    Boolean restorePosAndScaling = false;
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 656 || (Int32)FF9StateSystem.Common.FF9.fldMapNo == 657 || ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 658 || (Int32)FF9StateSystem.Common.FF9.fldMapNo == 659))
                        restorePosAndScaling = true;
                    if (this.gMode == 1)
                        geo.geoDetach(obj1.go, restorePosAndScaling);
                    else if (this.gMode != 2 && this.gMode == 3)
                        geo.geoDetachInWorld(obj1);
                }
                return 0;
            case EBin.event_code_binary.WATCH:
                Int32 num27 = this.gExec.ip - 1;
                this.gExec.ip = num27;
                Int32 num28 = (Int32)this.gExec.getByteIP();
                while (num28 != 0)
                {
                    ++this.gExec.ip;
                    num28 = (Int32)this.gExec.getByteIP();
                    ++num27;
                }
                this.eBin.SetVariableSpec(ref num27);
                this.gExec.ip = num27 + 1;
                this.gArgUsed = 1;
                return 0;
            case EBin.event_code_binary.STOP:
                num1 = (Int32)this.gExec.getByteIP(-1) | (Int32)this.gExec.getByteIP() << 8;
                ++this.gExec.ip;
                this.gArgUsed = 1;
                return 6;
            case EBin.event_code_binary.WAITTURN:
            case EBin.event_code_binary.DWAITTURN:
                if (eventCodeBinary == EBin.event_code_binary.DWAITTURN)
                    actor1 = (Actor)this.GetObj1();
                if (((Int32)actor1.flags & 128) != 0)
                    this.stay();
                return 1;
            case EBin.event_code_binary.TURNA:
                Obj obj2 = this.GetObj1();
                Int32 tspeed1 = this.getv1();
                if (obj2 != null)
                {
                    PosObj posObj3 = (PosObj)obj2;
                    Vector3 vector3_1 = new Vector3(posObj3.pos[0], posObj3.pos[1], posObj3.pos[2]);
                    Vector3 vector3_2 = new Vector3(actor1.pos[0], actor1.pos[1], actor1.pos[2]);
                    Single a = this.eBin.angleAsm(vector3_1.x - vector3_2.x, vector3_1.z - vector3_2.z);
                    this.StartTurn(actor1, a, true, tspeed1);
                }
                return 0;
            case EBin.event_code_binary.ASLEEP:
                actor1.sleep = (UInt16)this.getv2();
                return 0;
            case EBin.event_code_binary.NOINITMES:
                this.eTb.InhInitMes();
                return 0;
            case EBin.event_code_binary.WAITMES:
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1650 && FF9StateSystem.Settings.CurrentLanguage == "Japanese" && ((Int32)this.gCur.sid == 19 && this.gCur.ip == 1849))
                {
                    this.getv1();
                    return 0;
                }
                this.gCur.winnum = (Byte)this.getv1();
                this.gCur.wait = (Byte)254;
                return 1;
            case EBin.event_code_binary.MROT:
                Int32 num29 = this.getv1();
                if (num29 == 0)
                    num29 = (Int32)Byte.MaxValue;
                actor1.omega = (Byte)num29;
                return 0;
            case EBin.event_code_binary.TURN:
            case EBin.event_code_binary.DTURN:
                if (eventCodeBinary == EBin.event_code_binary.DTURN)
                    actor1 = (Actor)this.GetObj1();
                Int32 num30 = this.getv1() << 4;
                Int32 tspeed2 = this.getv1();
                if (FF9StateSystem.Common.FF9.fldMapNo == 1209 && actor1.sid == 9 && num30 == 2048)
                {
                    num3 = 0;
                }
                this.StartTurn(actor1, EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)num30), true, tspeed2);
                return 0;
            case EBin.event_code_binary.ENCRATE:
                this._context.encratio = (Byte)this.getv1();
                if ((Int32)this._context.encratio == 0)
                    this._encountBase = 0;
                return 0;
            case EBin.event_code_binary.BGSMOVE:
                this.getv2();
                this.getv2();
                this.getv2();
                //Debug.Log((object)("DoEventCode BGSMOVE : a = " + (object)this.getv2() + ", b = " + (object)this.getv2() + ", temp = " + (object)this.getv2()));
                return 0;
            case EBin.event_code_binary.BGLCOLOR:
                this.fieldmap.EBG_overlaySetShadeColor((UInt32)this.getv1(), (Byte)this.getv1(), (Byte)this.getv1(), (Byte)this.getv1());
                return 0;
            case EBin.event_code_binary.BGLMOVE:
                this.fieldmap.EBG_overlayMove(this.getv1(), (Int16)this.getv2(), (Int16)this.getv2(), (Int16)this.getv2());
                return 0;
            case EBin.event_code_binary.BGLACTIVE:
                this.fieldmap.EBG_overlaySetActive(this.getv1(), this.getv1());
                return 0;
            case EBin.event_code_binary.BGLLOOP:
                this.fieldmap.EBG_overlaySetLoop((UInt32)this.getv1(), (UInt32)this.getv1(), this.getv2(), this.getv2());
                return 0;
            case EBin.event_code_binary.BGLPARALLAX:
                this.fieldmap.EBG_overlaySetParallax((UInt32)this.getv1(), (UInt32)this.getv1(), this.getv2(), this.getv2());
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
                Int32 animNdx = this.getv1();
                Int32 frameRate = this.getv2();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 66 && FF9StateSystem.Settings.IsFastForward && (animNdx == 3 || animNdx == 2) && frameRate == 128)
                    frameRate = 320;
                this.fieldmap.EBG_animSetFrameRate(animNdx, frameRate);
                return 0;
            case EBin.event_code_binary.SETROW:
                num1 = this.chr2slot(this.getv1());
                num2 = this.getv1();
                if (num1 >= 0 && num1 < 9)
                {
                    FF9StateSystem.Common.FF9.player[num1].info.row = (byte)num2;
                }
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
                this._context.twist_a = (Int16)this.getv1();
                this._context.twist_d = (Int16)this.getv1();
                FF9StateSystem.Field.SetTwistAD((Int32)this._context.twist_a, (Int32)this._context.twist_d);
                return 0;
            case EBin.event_code_binary.FICON:
                Int32 type = this.getv1();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2955)
                {
                    if ((Int32)this.gCur.uid == 24)
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
                this._context.dashinh = (Byte)1;
                return 0;
            case EBin.event_code_binary.CLEARCOLOR:
                this.fieldmap.GetMainCamera().backgroundColor = new Color((Single)this.getv1() / (Single)Byte.MaxValue, (Single)this.getv1() / (Single)Byte.MaxValue, (Single)this.getv1() / (Single)Byte.MaxValue);
                return 0;
            case EBin.event_code_binary.BGSSCROLL:
                this.fieldmap.EBG_scene2DScroll((Int16)this.getv2(), (Int16)this.getv2(), (UInt16)this.getv1(), (UInt32)this.getv1());
                return 0;
            case EBin.event_code_binary.BGSRELEASE:
                this.fieldmap.EBG_scene2DScrollRelease(this.getv1(), (UInt32)this.getv1());
                return 0;
            case EBin.event_code_binary.BGCACTIVE:
                this.fieldmap.EBG_char3DScrollSetActive((UInt32)this.getv1(), this.getv1(), (UInt32)this.getv1());
                return 0;
            case EBin.event_code_binary.BGCHEIGHT:
                this.fieldmap.charAimHeight = (Int16)this.getv2();
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
                Int16 num31 = (Int16)this.getv2();
                quad2.q[1].X = num31;
                Int32 num32 = (Int32)num31;
                quadPos1.X = (Int16)num32;
                QuadPos quadPos2 = quad2.q[0];
                Int16 num33 = (Int16)this.getv2();
                quad2.q[1].Z = num33;
                Int32 num34 = (Int32)num33;
                quadPos2.Z = (Int16)num34;
                return 0;
            case EBin.event_code_binary.TRACK:
                Quad quad3 = (Quad)this.gCur;
                Int32 val1 = quad3.n;
                QuadPos quadPos3 = quad3.q[Math.Max(val1, 1) - 1];
                quadPos3.X = (Int16)this.getv2();
                quadPos3.Z = (Int16)this.getv2();
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
                actor1.turnl = (UInt16)this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB.GetValue((Int32)actor1.turnl));
                return 0;
            case EBin.event_code_binary.ATURNR:
                actor1.turnr = (UInt16)this.getv2();
                AnimationFactory.AddAnimWithAnimatioName(actor1.go, FF9DBAll.AnimationDB.GetValue((Int32)actor1.turnr));
                return 0;
            case EBin.event_code_binary.CHOOSEPARAM:
                this.eTb.SetChooseParam(this.getv2(), this.getv1());
                return 0;
            case EBin.event_code_binary.TIMERCONTROL:
                this._ff9.timerControl = this.getv1() != 0;
                TimerUI.SetPlay(this._ff9.timerControl);
                return 0;
            case EBin.event_code_binary.SETCAM:
                Int32 newCamIdx = this.getv1();
                this.fieldmap.SetCurrentCameraIndex(newCamIdx);
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1205 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 4800 && this.eBin.getVarManually(6357) == 3)
                    this.SetActorPosition(this._fixThornPosObj, (Single)this._fixThornPosA, (Single)this._fixThornPosB, (Single)this._fixThornPosC);
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 3009 && (Int32)this.gCur.uid == 17 && newCamIdx == 0)
                {
                    EventEngine.resyncBGMSignal = 1;
                    //Debug.Log((object)("SET resyncBGMSignal = " + (object)EventEngine.resyncBGMSignal));
                }
                return 0;
            case EBin.event_code_binary.IDLESPEED:
                actor1.idleSpeed[0] = (Byte)this.getv1();
                actor1.idleSpeed[1] = (Byte)this.getv1();
                actor1.idleSpeed[2] = (Byte)this.getv1();
                actor1.idleSpeed[3] = (Byte)this.getv1();
                return 0;
            case EBin.event_code_binary.CHRFX:
                Int32 Parm = this.getv1();
                Int32 num35 = this.getv2();
                Int32 num36 = this.getv2();
                Int32 num37 = this.getv2();
                fldchar.FF9FieldCharDispatch((Int32)po.uid, Parm, num35, num36, num37);
                return 0;
            case EBin.event_code_binary.SEPV:
                Int32 PosPtr1;
                Int32 VolPtr1;
                FF9Snd.FF9FieldSoundGetPositionVolume(this.getv2(), this.getv2(), this.getv2(), out PosPtr1, out VolPtr1);
                this.sSEPos = PosPtr1;
                Int32 num38 = this.getv1();
                this.sSEVol = VolPtr1 * num38 >> 7;
                if (this.sSEVol > (Int32)SByte.MaxValue)
                    this.sSEVol = (Int32)SByte.MaxValue;
                return 0;
            case EBin.event_code_binary.SEPVA:
                PosObj posObj4 = (PosObj)this.GetObj1();
                Int32 PosPtr2;
                Int32 VolPtr2;
                FF9Snd.FF9FieldSoundGetPositionVolume((Int32)posObj4.pos[0], (Int32)posObj4.pos[1], (Int32)posObj4.pos[2], out PosPtr2, out VolPtr2);
                this.sSEPos = PosPtr2;
                Int32 num39 = this.getv1();
                this.sSEVol = VolPtr2 * num39 >> 7;
                if (this.sSEVol > (Int32)SByte.MaxValue)
                    this.sSEVol = (Int32)SByte.MaxValue;
                return 0;
            case EBin.event_code_binary.NECKID:
                actor1.neckMyID = (Byte)this.getv1();
                actor1.neckTargetID = (Byte)this.getv1();
                actor1.actf |= (UInt16)(EventEngine.actNeckT | EventEngine.actNeckM);
                return 0;
            case EBin.event_code_binary.ENCOUNT2:
                this.getv1(); // rush_type
                this._ff9.btlSubMapNo = (SByte)this.getv1();
                Int32 num40 = this.getv2();
                this._ff9.steiner_state = (Byte)(num40 >> 15 & 1);
                this.SetBattleScene(num40 & (Int32)Int16.MaxValue);
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
                if (fldmcf.FF9FieldMCFSetCharColor((Int32)this.GetObj1().uid, this.getv1(), this.getv1(), this.getv1()) == 0)
                    return 0;
                this.stay();
                return 1;
            case EBin.event_code_binary.SLEEPINH:
                this._context.idletimer = -1;
                return 0;
            case EBin.event_code_binary.AUTOTURN:
                Int32 num41 = this.getv1();
                actor1.turninst0 = num41 == 0 ? (Int16)4 : (Int16)167;
                return 0;
            case EBin.event_code_binary.BGLATTACH:
                this.fieldmap.EBG_charAttachOverlay((UInt32)this.getv1(), (Int16)this.getv2(), (Int16)this.getv1(), (SByte)this.getv1(), (Byte)this.getv1(), (Byte)this.getv1(), (Byte)this.getv1());
                return 0;
            case EBin.event_code_binary.CFLAG:
                Int32 cflag = (Int32)(Byte)this.getv1();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2934 && MBG.Instance.GetFrame < 10)
                {
                    this.StartCoroutine(this.DelayedCFLAG(this.gCur, cflag));
                }
                else
                {
                    this.gCur.flags = (Byte)((this.gCur.flags & -64) | (cflag & 63));
                }
                return 0;
            case EBin.event_code_binary.AJUMP:
                actor1.jump = (UInt16)this.getv2();
                actor1.jump0 = (Byte)this.getv1();
                actor1.jump1 = (Byte)this.getv1();
                return 0;
            case EBin.event_code_binary.MESA:
                PosObj targetPo1 = (PosObj)this.GetObj1();
                this.gCur.winnum = (Byte)this.getv1();
                Int32 flags3 = this.getv1();
                this.SetFollow((Obj)targetPo1, (Int32)this.gCur.winnum, flags3);
                this.eTb.NewMesWin(this.getv2(), (Int32)this.gCur.winnum, flags3, targetPo1);
                this.gCur.wait = (Byte)254;
                return 1;
            case EBin.event_code_binary.MESAN:
                PosObj targetPo2 = (PosObj)this.GetObj1();
                this.gCur.winnum = (Byte)this.getv1();
                Int32 flags4 = this.getv1();
                this.SetFollow((Obj)targetPo2, (Int32)this.gCur.winnum, flags4);
                this.eTb.NewMesWin(this.getv2(), (Int32)this.gCur.winnum, flags4, targetPo2);
                return 0;
            case EBin.event_code_binary.DRET:
                Obj obj3 = this.GetObj1();
                if (obj3 != null)
                {
                    obj3.sx = (Byte)0;
                    obj3.state = EventEngine.stateInit;
                    this.Return(obj3);
                }
                return obj3 == this.gExec ? 1 : 0;
            case EBin.event_code_binary.MOVT:
                actor1.loopCount = (Byte)this.getv1();
                return 0;
            case EBin.event_code_binary.TSPEED:
                actor1.tspeed = (Byte)this.getv1();
                if ((Int32)actor1.tspeed == 0)
                    actor1.tspeed = (Byte)16;
                return 0;
            case EBin.event_code_binary.BGIACTIVET:
                Int32 num42 = this.getv2();
                Int32 num43 = this.getv1();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1753 && num42 == 207)
                    this.fieldmap.walkMesh.BGI_triSetActive(208U, (UInt32)num43);
                else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1606 && num42 == 107)
                    num43 = 1;
                this.fieldmap.walkMesh.BGI_triSetActive((UInt32)num42, (UInt32)num43);
                return 0;
            case EBin.event_code_binary.TURNTO:
                Int32 num44 = this.getv2();
                Int32 num45 = this.getv2();
                if (!EventEngineUtils.nearlyEqual((Single)num44, gameObject.transform.localPosition.x) || !EventEngineUtils.nearlyEqual((Single)num45, gameObject.transform.localPosition.z))
                {
                    FieldMapActorController component = gameObject.GetComponent<FieldMapActorController>();
                    Single a = this.eBin.angleAsm((Single)num44 - component.curPos.x, (Single)num45 - component.curPos.z);
                    this.StartTurn(actor1, a, true, (Int32)actor1.tspeed);
                }
                return 0;
            case EBin.event_code_binary.PREJUMP:
                actor1.animFlag = (Byte)EventEngine.afHold;
                actor1.inFrame = (Byte)0;
                actor1.outFrame = (Byte)((UInt32)actor1.jump0 - 1U);
                this.ExecAnim(actor1, (Int32)actor1.jump);
                return 0;
            case EBin.event_code_binary.POSTJUMP:
                ff9shadow.FF9ShadowOnField((Int32)actor1.uid);
                actor1.animFlag = (Byte)0;
                actor1.inFrame = (Byte)((UInt32)actor1.jump1 + 1U);
                actor1.outFrame = Byte.MaxValue;
                this.ExecAnim(actor1, (Int32)actor1.jump);
                return 0;
            case EBin.event_code_binary.MOVQ:
                PosObj posObj5 = (PosObj)this.FindObjByUID((Int32)this._context.controlUID);
                if (posObj5 != null)
                {
                    this.Call((Obj)posObj5, 0, 0, false, Obj.movQData);
                    this._context.usercontrol = (Byte)0;
                    for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
                        objList.obj.flags |= (Byte)6;
                }
                return 0;
            case EBin.event_code_binary.CHRSCALE:
                PosObj posObj6 = (PosObj)this.GetObj1();
                Int32 num46 = this.getv1();
                Int32 num47 = this.getv1();
                Int32 num48 = this.getv1();
                Int32 num49 = 18;
                if (posObj6 == null)
                    return 0;
                if ((UnityEngine.Object)posObj6.go != (UnityEngine.Object)null)
                    geo.geoScaleSetXYZ(posObj6.go, num46 << 24 >> num49, num47 << 24 >> num49, num48 << 24 >> num49);
                posObj6.scaley = (Byte)num47;
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 576 && ((Int32)posObj6.uid == 4 || (Int32)posObj6.uid == 8 || ((Int32)posObj6.uid == 9 || (Int32)posObj6.uid == 10) || (Int32)posObj6.uid == 11))
                {
                    this._geoTexAnim = posObj6.go.GetComponent<GeoTexAnim>();
                    this._geoTexAnim.geoTexAnimStop(2);
                    this._geoTexAnim.geoTexAnimPlay(1);
                }
                return 0;
            case EBin.event_code_binary.MOVJ:
                if (this.MoveToward_mixed((Single)this.sMapJumpX, 0.0f, (Single)this.sMapJumpZ, 0, (PosObj)null))
                    this.stay();
                return 1;
            case EBin.event_code_binary.POS3:
            case EBin.event_code_binary.DPOS3:
                if (eventCodeBinary == EBin.event_code_binary.DPOS3)
                    po = (PosObj)this.GetObj1();
                Int32 num50 = this.getv2();
                Int32 num51 = -this.getv2();
                Int32 num52 = this.getv2();
                if ((this.gMode != 1 || (Int32)po.model == (Int32)UInt16.MaxValue ? 0 : 1) == 1)
                {
                    if (po != null)
                    {
                        FieldMapActorController component = po.go.GetComponent<FieldMapActorController>();
                        if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.walkMesh != null)
                            component.walkMesh.BGI_charSetActive(component, 0U);
                        if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1205 && (Int32)po.sid == 6 && (this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 4800 && num50 == 418) && num52 == 9733)
                        {
                            this._fixThornPosA = num50;
                            this._fixThornPosB = num51;
                            this._fixThornPosC = num52;
                            this._fixThornPosObj = po;
                            num50 = 600;
                            num52 = 9999;
                        }
                        if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 563 && (Int32)po.sid == 16)
                        {
                            if (num50 == -1614)
                                num50 = -1635;
                        }
                        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1310 && (Int32)po.sid == 12)
                        {
                            if (num50 == -1614)
                                num50 = -1635;
                        }
                        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2110 && (Int32)po.sid == 9)
                        {
                            if (num50 == -1614)
                                num50 = -1635;
                        }
                        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 572 && (Int32)po.sid == 16)
                        {
                            if (num50 == -1750)
                                num50 = -1765;
                        }
                        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1607)
                        {
                            Int32 varManually1 = this.eBin.getVarManually(EBin.SC_COUNTER_SVR);
                            Int32 varManually2 = this.eBin.getVarManually(9169);
                            if (((Int32)po.model == 236 || (Int32)po.model == 237) && (varManually1 >= 6640 && varManually1 < 6690) && varManually2 == 0)
                                num51 += 50000;
                        }
                        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1606 && ((Int32)po.uid == 9 && num50 == -171 && (num51 == 641 && num52 == 1042) || (Int32)po.uid == 128 && num50 == -574 && (num51 == 624 && num52 == 903) || ((Int32)po.uid == 129 && num50 == -226 && (num51 == 639 && num52 == 1009) || (Int32)po.uid == 130 && num50 == -93 && (num51 == 647 && num52 == 1017)) || ((Int32)po.uid == 131 && num50 == 123 && (num51 == 667 && num52 == 1006) || (Int32)po.uid == 132 && num50 == -689 && (num51 == 621 && num52 == 859))))
                            gameObject.GetComponent<FieldMapActor>().SetRenderQueue(2000);
                    }
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1756 && (Int32)po.sid == 6 && (num50 == -3019 && num52 == 1226))
                    {
                        num50 = -3145;
                        num51 = -2035;
                        num52 = 1274;
                    }
                }
                this.SetActorPosition(po, (Single)num50, (Single)num51, (Single)num52);
                if ((Int32)po.cid == 4)
                    this.clrdist((Actor)po);
                return 0;
            case EBin.event_code_binary.MOVE3:
                if (this.MoveToward_mixed((Single)this.getv2(), -this.getv2(), (Single)this.getv2(), 2, (PosObj)null))
                    this.stay();
                return 1;
            case EBin.event_code_binary.DRADIUS:
                this.getv1();
                this.getv1();
                this.getv1();
                this.getv1();
                return 0;
            case EBin.event_code_binary.MJPOS:
                PosObj posObj7 = (PosObj)this.FindObjByUID((Int32)this._context.controlUID);
                if (posObj7 != null)
                {
                    QuadPos quadPos4 = ((Quad)this.gCur).q[0];
                    QuadPos quadPos5 = ((Quad)this.gCur).q[1];
                    Int32 num13 = (Int32)quadPos5.X - (Int32)quadPos4.X;
                    Int32 num53 = (Int32)quadPos5.Z - (Int32)quadPos4.Z;
                    Int32 num54 = num13 * num13 + num53 * num53 >> 8;
                    if (num54 != 0)
                    {
                        num54 = ((Int32)((Double)num13 * ((Double)posObj7.pos[0] - (Double)quadPos4.X)) + (Int32)((Double)num53 * ((Double)posObj7.pos[2] - (Double)quadPos4.Z))) / num54;
                        if (num54 < 0)
                            num54 = 0;
                        else if (num54 > 256)
                            num54 = 256;
                    }
                    this.sMapJumpX = (num54 * num13 >> 8) + (Int32)quadPos4.X;
                    this.sMapJumpZ = (num54 * num53 >> 8) + (Int32)quadPos4.Z;
                    if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 552 && (Int32)quadPos4.X == 1231 && ((Int32)quadPos4.Z == 1556 && (Int32)quadPos5.X == 1291) && (Int32)quadPos5.Z == 1376)
                    {
                        this.sMapJumpX = 1226;
                        this.sMapJumpZ = 1430;
                    }
                }
                else
                    this.sMapJumpX = this.sMapJumpZ = 0;
                return 0;
            case EBin.event_code_binary.MOVH:
                Int32 num55 = this.getv2();
                Int32 num56 = this.getv2();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 66 && (Int32)po.sid == 14 && (num55 == -145 && num56 == -9135))
                {
                    num55 = -160;
                    num56 = -9080;
                }
                Boolean flag3 = this.MoveToward_mixed((Single)num55, 0.0f, (Single)num56, 1, (PosObj)null);
                eulerAngles1 = po.go.transform.localRotation.eulerAngles;
                if (flag3)
                    this.stay();
                return 1;
            case EBin.event_code_binary.SPEEDTH:
                actor1.speedth = (Byte)this.getv1();
                return 0;
            case EBin.event_code_binary.TURNDS:
                Int32 num57 = this.getv1() << 4;
                this.StartTurn(actor1, EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)num57), true, (Int32)actor1.tspeed);
                return 0;
            case EBin.event_code_binary.BGI:
                Int32 num58 = this.getv1();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2508 && (Int32)po.sid == 2 && num58 == 1)
                    num58 = 0;
                if ((Int32)po.model != (Int32)UInt16.MaxValue)
                {
                    FieldMapActorController component = gameObject.GetComponent<FieldMapActorController>();
                    component.walkMesh.BGI_charSetActive(component, (UInt32)num58);
                }
                return 0;
            case EBin.event_code_binary.GETSCREEN:
                Obj obj4 = this.GetObj1();
                if (obj4 != null && (UnityEngine.Object)obj4.go != (UnityEngine.Object)null)
                {
                    Single x;
                    Single y;
                    ETb.World2Screen(obj4.go.GetComponent<FieldMapActorController>().curPos, out x, out y);
                    this.sSysX = (Int32)x;
                    this.sSysY = (Int32)y;
                }
                return 0;
            case EBin.event_code_binary.MENUON:
                if (FF9StateSystem.Common.FF9.fldMapNo == 2172 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) < 9100 && Localization.GetSymbol() == "JP" && this.gCur.sid == 1 && this.gCur.ip == 2964 && EIcon.AIconMode == 0)
                {
                    return 0;
                }
                EventInput.PSXCntlClearPadMask(0, 262144U);
                PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(true);
                return 0;
            case EBin.event_code_binary.MENUOFF:
                if (FF9StateSystem.Common.FF9.fldMapNo == 2172 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) < 9100 && Localization.GetSymbol() == "JP" && this.gCur.sid == 1 && this.gCur.ip == 119 && EIcon.AIconMode == 0)
                {
                    return 0;
                }
                EventInput.PSXCntlSetPadMask(0, 262144U);
                PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
                return 1;
            case EBin.event_code_binary.DISCCHANGE:
                Int32 num59 = this.getv2();
                this.FF9FieldDiscRequest((Byte)(num59 >> 14 & 3), (UInt16)(num59 & 16383));
                return 1;
            case EBin.event_code_binary.MINIGAME:
                Int32 minigameFlag = this.getv2();
                EventService.SetMiniGame((UInt16)minigameFlag);
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
                Int32 num60 = 0;
                for (Int32 index4 = 8; index4 >= 0; --index4)
                    num60 = num60 << 1 | ((Int32)FF9StateSystem.Common.FF9.player[index4].info.party == 0 ? 0 : 1);
                Int32 num61 = this.getv2();
                for (Int32 index4 = 0; index4 < 9; ++index4)
                {
                    sPartyInfo.fix[index4] = (num61 & 1) > 0;
                    num61 >>= 1;
                }
                for (Int32 index4 = 0; index4 < 4; ++index4)
                {
                    if (FF9StateSystem.Common.FF9.party.member[index4] != null)
                    {
                        sPartyInfo.menu[index4] = FF9StateSystem.Common.FF9.party.member[index4].info.slot_no;
                        Int32 int32 = Convert.ToInt32(sPartyInfo.menu[index4]);
                        num60 &= ~(1 << int32);
                    }
                    else
                        sPartyInfo.menu[index4] = Convert.ToByte((Int32)Byte.MaxValue);
                }
                Int32 num62;
                Int32 num63 = num62 = 0;
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
                Int32 num64 = this.getv2();
                Int32 num65 = num64 | num64 >> 4 & 224;
                for (Int32 slot_id = 0; slot_id < 9; ++slot_id)
                    ff9play.FF9Play_Delete(slot_id);
                for (Int32 slot_id = 0; slot_id < 9; ++slot_id)
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
                        actor1.animFlag = (Byte)EventEngine.afExec;
                    }
                    else
                    {
                        actor1.parent = (Actor)null;
                        actor1.animFlag = (Byte)0;
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
                    actor1.actf = (UInt16)((UInt32)actor1.actf | (UInt32)EventEngine.actEye);
                }
                return 0;
            case EBin.event_code_binary.AIM:
                if (this.gMode == 3)
                {
                    Vector3 aimPtr = ff9.w_cameraGetAimPtr();
                    this.SetActorPosition(po, aimPtr.x, aimPtr.y, aimPtr.z);
                    actor1.actf = (UInt16)((UInt32)actor1.actf | (UInt32)EventEngine.actAim);
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
                if (((Int32)((Actor)this.GetObj1()).animFlag & EventEngine.afExec) == 0)
                    return 0;
                this.stay();
                return 1;
            case EBin.event_code_binary.TEXPLAY:
                PosObj posObj8 = (PosObj)this.GetObj1();
                this._geoTexAnim = posObj8.go.GetComponent<GeoTexAnim>();
                Int32 anum1 = this.getv1();
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 114 && (Int32)posObj8.uid == 2 && anum1 == 0 || (Int32)FF9StateSystem.Common.FF9.fldMapNo == 450 && (Int32)posObj8.uid == 3 && anum1 == 0 || ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 551 && (Int32)posObj8.uid == 10 && anum1 == 0 || (Int32)FF9StateSystem.Common.FF9.fldMapNo == 555 && (Int32)posObj8.uid == 12 && anum1 == 0) || ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 559 && (Int32)posObj8.uid == 17 && anum1 == 0 || (Int32)FF9StateSystem.Common.FF9.fldMapNo == 2105 && (Int32)posObj8.uid == 2 && anum1 == 0 || (Int32)FF9StateSystem.Common.FF9.fldMapNo == 2450 && (Int32)posObj8.uid == 8 && anum1 == 0))
                    anum1 = 2;
                if ((UnityEngine.Object)posObj8.go != (UnityEngine.Object)null && (UnityEngine.Object)this._geoTexAnim != (UnityEngine.Object)null)
                    this._geoTexAnim.geoTexAnimPlay(anum1);
                return 0;
            case EBin.event_code_binary.TEXPLAY1:
                PosObj posObj9 = (PosObj)this.GetObj1();
                this._geoTexAnim = posObj9.go.GetComponent<GeoTexAnim>();
                Int32 anum2 = this.getv1();
                if ((UnityEngine.Object)posObj9.go != (UnityEngine.Object)null && (UnityEngine.Object)this._geoTexAnim != (UnityEngine.Object)null)
                    this._geoTexAnim.geoTexAnimPlayOnce(anum2);
                return 0;
            case EBin.event_code_binary.TEXSTOP:
                PosObj posObj10 = (PosObj)this.GetObj1();
                this._geoTexAnim = posObj10.go.GetComponent<GeoTexAnim>();
                Int32 anum3 = this.getv1();
                if ((UnityEngine.Object)posObj10.go != (UnityEngine.Object)null && (UnityEngine.Object)this._geoTexAnim != (UnityEngine.Object)null)
                    this._geoTexAnim.geoTexAnimStop(anum3);
                return 0;
            case EBin.event_code_binary.BGVSET:
                this.fieldmap.EBG_overlaySetViewport((UInt32)this.getv1(), (UInt32)this.getv1());
                return 0;
            case EBin.event_code_binary.WPRM:
                ff9.w_frameSetParameter(this.getv1(), this.getv2());
                return 0;
            case EBin.event_code_binary.FLDSND0:
                FF9Snd.FF9SoundArg0(this.getv2(), this.getv2());
                return 0;
            case EBin.event_code_binary.FLDSND1:
                Int32 _parmtype1 = this.getv2();
                Int32 _objno1 = this.getv2();
                Int32 num66 = this.getv3();
                FF9Snd.FF9SoundArg1(_parmtype1, _objno1, num66);
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2928 && _objno1 == 2787)
                {
                    FF9Snd.FF9SoundArg1(_parmtype1, 2982, num66);
                    FF9Snd.FF9SoundArg1(_parmtype1, 2983, num66);
                }
                return 0;
            case EBin.event_code_binary.FLDSND2:
                FF9Snd.FF9SoundArg2(this.getv2(), this.getv2(), this.getv3(), this.getv1());
                return 0;
            case EBin.event_code_binary.FLDSND3:
                Int32 _parmtype2 = this.getv2();
                Int32 _objno2 = this.getv2();
                Int32 num67 = this.getv3();
                Int32 num68 = this.getv1();
                Int32 num69 = this.getv1();
                FF9Snd.FF9SoundArg3(_parmtype2, _objno2, num67, num68, num69);
                if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2928)
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
                this.fieldmap.EBG_overlayDefineViewport((UInt32)this.getv1(), (Int16)this.getv2(), (Int16)this.getv2(), (Int16)this.getv2(), (Int16)this.getv2());
                return 0;
            case EBin.event_code_binary.BGAVISIBLE:
                this.fieldmap.EBG_animSetVisible(this.getv1(), this.getv1());
                return 0;
            case EBin.event_code_binary.BGIACTIVEF:
                this.fieldmap.walkMesh.BGI_floorSetActive((UInt32)this.getv1(), (UInt32)this.getv1());
                return 0;
            case EBin.event_code_binary.CHRSET:
                Int32 attr1 = this.getv2();
                FF9Char.ff9char_attr_set((Int32)po.uid, attr1);
                return 0;
            case EBin.event_code_binary.CHRCLEAR:
                Int32 attr2 = this.getv2();
                FF9Char.ff9char_attr_clear((Int32)po.uid, attr2);
                return 0;
            case EBin.event_code_binary.GILADD:
                if ((this._ff9.party.gil += (UInt32)this.getv3()) > 9999999U)
                    this._ff9.party.gil = 9999999U;
                return 0;
            case EBin.event_code_binary.GILDELETE:
                Int32 gilDecrease = this.getv3();
                if ((this._ff9.party.gil -= (UInt32)gilDecrease) > 9999999U)
                    this._ff9.party.gil = 0U;
                if (this.isPosObj(this.gCur))
                    EMinigame.StiltzkinAchievement((PosObj)this.gCur, gilDecrease);
                return 0;
            case EBin.event_code_binary.MESB:
                UIManager.Battle.SetBattleMessage(FF9TextTool.BattleText(this.getv2()), (Byte)3);
                return 0;
            case EBin.event_code_binary.GLOBALCLEAR:
                return 0;
            case EBin.event_code_binary.DEBUGSAVE:
                return 0;
            case EBin.event_code_binary.DEBUGLOAD:
                return 0;
            case EBin.event_code_binary.ATTACHOFFSET:
                GameObject sourceObject2 = this.gCur.go;
                Int32 x1 = this.getv2();
                Int32 y1 = this.getv2();
                Int32 z = this.getv2();
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
                        if (((Int32)obj6.flags & 32) == 0)
                            obj6.flags = (Byte)((UInt32)obj6.flags & 4294967294U);
                    }
                }
                return 0;
            case EBin.event_code_binary.POPSHOW:
                for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
                {
                    Obj obj6 = objList.obj;
                    if (this.isPosObj(obj6))
                    {
                        obj6.flags = (Byte)((UInt32)obj6.flags & 4294967294U);
                        obj6.flags = (Byte)((UInt32)obj6.flags | (UInt32)((PosObj)obj6).pflags & 1U);
                    }
                }
                return 0;
            case EBin.event_code_binary.AICON:
                EIcon.SetAIcon(this.getv1());
                return 0;
            case EBin.event_code_binary.CLEARSTATUS:
                Int32 num70 = (Int32)SFieldCalculator.FieldRemoveStatus(FF9StateSystem.Common.FF9.player[this.chr2slot(this.getv1())], (Byte)this.getv1());
                return 0;
            case EBin.event_code_binary.SPS2:
                this.fieldSps.FF9FieldSPSSetObjParm(this.getv1(), this.getv1(), this.getv1(), this.getv2(), this.getv2());
                return 0;
            case EBin.event_code_binary.WINPOSE:
                Int32 index5 = this.chr2slot(this.getv1());
                Int32 num71 = this.getv1();
                if (index5 >= 0 && index5 < 9)
                    this._ff9.player[index5].info.win_pose = (Byte)num71;
                return 0;
            case EBin.event_code_binary.JUMP3:
                Int32 num72 = (Int32)actor1.jframe;
                ++actor1.jframe;
                Int32 num73 = (Int32)actor1.jframeN;
                actor1.pos[0] = (Single)(((Int32)actor1.x0 * (num73 - num72) + (Int32)actor1.jumpx * num72) / num73);
                actor1.pos[1] = (Single)((Int32)actor1.y0 - num72 * (num72 << 3) + num72 * ((Int32)actor1.jumpy - (Int32)actor1.y0) / num73 + num72 * (num73 << 3));
                actor1.pos[2] = (Single)(((Int32)actor1.z0 * (num73 - num72) + (Int32)actor1.jumpz * num72) / num73);
                this.SetActorPosition(po, actor1.pos[0], actor1.pos[1], actor1.pos[2]);
                if (num72 >= num73)
                    return 0;
                this.stay();
                return 1;
            case EBin.event_code_binary.PARTYDELETE:
                Int32 num74 = this.chr2slot(this.getv1());
                Int32 party_id = 0;
                while (party_id < 4 && (this._ff9.party.member[party_id] == null || (Int32)this._ff9.party.member[party_id].info.slot_no != num74))
                    ++party_id;
                if (party_id < 4)
                {
                    ff9play.FF9Play_SetParty(party_id, -1);
                    this.SetupPartyUID();
                }
                return 0;
            case EBin.event_code_binary.PLAYERNAME:
                Int32 index6 = this.chr2slot(this.getv1());
                Int32 textId = this.getv2();
                if (index6 >= 0 && index6 < 9)
                    this._ff9.player[index6].name = FF9TextTool.RemoveOpCode(FF9TextTool.FieldText(textId));
                return 0;
            case EBin.event_code_binary.OVAL:
                po.ovalRatio = (Byte)this.getv1();
                return 0;
            case EBin.event_code_binary.INCFROG:
                _ff9.Frogs.Increment();
                EMinigame.CatchingGoldenFrogAchievement(this.gCur);
                return 0;
            case EBin.event_code_binary.BEND:
                this._noEvents = true;
                PersistenSingleton<UIManager>.Instance.BattleResultScene.ShutdownBattleResultUI();
                return 0;
            case EBin.event_code_binary.SETVY3:
                actor1.jumpx = (Int16)this.getv2();
                actor1.jumpy = (short)-this.getv2();
                actor1.jumpz = (Int16)this.getv2();
                Int32 num75 = (Int32)(Byte)this.getv1();
                if (num75 == 0)
                    num75 = 8;
                actor1.actf |= (UInt16)EventEngine.actJump;
                ff9shadow.FF9ShadowOffField((Int32)actor1.uid);
                actor1.inFrame = actor1.jump0;
                actor1.outFrame = actor1.jump1;
                this.ExecAnim(actor1, (Int32)actor1.jump);
                Int32 num76 = ((Int32)actor1.outFrame - (Int32)actor1.inFrame << 4) / num75;
                actor1.aspeed = (Byte)num76;
                if (this.gMode == 1 && (Int32)po.model != (Int32)UInt16.MaxValue)
                {
                    fmac = gameObject.GetComponent<FieldMapActorController>();
                    fmac.walkMesh.BGI_charSetActive(fmac, 0U);
                }
                actor1.x0 = (Int16)fmac.curPos.x;
                actor1.y0 = (Int16)fmac.curPos.y;
                actor1.z0 = (Int16)fmac.curPos.z;
                actor1.jframe = (Byte)0;
                actor1.jframeN = (Byte)num75;
                return 0;
            case EBin.event_code_binary.SETSIGNAL:
                ETb.gMesSignal = this.getv1();
                return 0;
            case EBin.event_code_binary.BGLSCROLLOFFSET:
                this.fieldmap.EBG_overlaySetScrollWithOffset((UInt32)this.getv1(), (UInt32)this.getv1(), this.getv2(), this.getv2(), (UInt32)this.getv1());
                return 0;
            case EBin.event_code_binary.BTLSEQ:
                btlseq.StartBtlSeq(this.GetSysList(1), this.GetSysList(0), this.getv1());
                return 0;
            case EBin.event_code_binary.BGLLOOPTYPE:
                this.fieldmap.EBG_overlaySetLoopType((UInt32)this.getv1(), (UInt32)this.getv1());
                return 0;
            case EBin.event_code_binary.BGAFRAME:
                this.fieldmap.EBG_animShowFrame(this.getv1(), this.getv1());
                return 0;
            case EBin.event_code_binary.MOVE3H:
                if (this.MoveToward_mixed((Single)this.getv2(), -this.getv2(), (Single)this.getv2(), 3, (PosObj)null))
                    this.stay();
                return 1;
            case EBin.event_code_binary.SYNCPARTY:
                this.SetupPartyUID();
                return 0;
            case EBin.event_code_binary.VRP:
                Int16 x2 = 0;
                Int16 y2 = 0;
                this.fieldmap.EBG_sceneGetVRP(ref x2, ref y2);
                this.sSysX = (Int32)x2;
                this.sSysY = (Int32)y2;
                return 0;
            case EBin.event_code_binary.CLOSEALL:
                this.eTb.YWindow_CloseAll();
                return 0;
            case EBin.event_code_binary.WIPERGB:
                Int32 num77 = this.getv1();
                Int32 frame = this.getv1();
                this.getv1();
                Int32 num78 = this.getv1();
                Int32 num79 = this.getv1();
                Int32 num80 = this.getv1();
                SceneDirector.InitFade((num77 >> 1 & 1) != 0 ? FadeMode.Sub : FadeMode.Add, frame, (Color32)new Color((Single)num78 / (Single)Byte.MaxValue, (Single)num79 / (Single)Byte.MaxValue, (Single)num80 / (Single)Byte.MaxValue));
                return 0;
            case EBin.event_code_binary.BGVALPHA:
                this.fieldmap.EBG_overlayDefineViewportAlpha((UInt32)this.getv1(), this.getv2(), this.getv2());
                return 0;
            case EBin.event_code_binary.SLEEPON:
                this._context.idletimer = (Int16)0;
                return 0;
            case EBin.event_code_binary.HEREON:
                EIcon.SetHereIcon(this.getv1());
                return 0;
            case EBin.event_code_binary.DASHON:
                this._context.dashinh = (Byte)0;
                return 0;
            case EBin.event_code_binary.SETHP:
            {
                Int32 characterIndex = this.chr2slot(this.getv1());
                if (characterIndex >= 0 && characterIndex < 9)
                {
                    PLAYER player = FF9StateSystem.Common.FF9.player[characterIndex];
                    Int32 newHp = this.getv2();
                    if (newHp > player.max.hp)
                        newHp = (Int32)player.max.hp;

                    player.cur.hp = (UInt32)newHp;
                    FF9StateSystem.Common.FF9.player[characterIndex] = player;

                    // https://github.com/Albeoris/Memoria/issues/22
                    if (characterIndex == 6 && newHp == player.max.hp)
                    {
                        player = FF9StateSystem.Common.FF9.player[8];
                        player.cur.hp = player.max.hp;
                    }
                }
                return 0;
            }
            case EBin.event_code_binary.SETMP:
            {
                Int32 characterIndex = this.chr2slot(this.getv1());
                if (characterIndex >= 0 && characterIndex < 9)
                {
                    PLAYER player = FF9StateSystem.Common.FF9.player[characterIndex];
                    Int32 newMp = this.getv2();
                    if (newMp > player.max.mp)
                        newMp = (Int32)player.max.mp;

                    player.cur.mp = (UInt32)newMp;
                    FF9StateSystem.Common.FF9.player[characterIndex] = player;

                    // https://github.com/Albeoris/Memoria/issues/22
                    if (characterIndex == 6 && newMp == player.max.mp)
                    {
                        player = FF9StateSystem.Common.FF9.player[8];
                        player.cur.mp = player.max.mp;
                    }
                }
                return 0;
            }
            case EBin.event_code_binary.CLEARAP:
                ff9abil.FF9Abil_ClearAp(this.chr2slot(this.getv1()), this.getv1());
                return 0;
            case EBin.event_code_binary.MAXAP:
                ff9abil.FF9Abil_SetMaster(this.chr2slot(this.getv1()), this.getv1());
                return 0;
            case EBin.event_code_binary.GAMEOVER:
                return 8;
            case EBin.event_code_binary.VIBSTART:
                vib.VIB_vibrate((Int16)this.getv1());
                return 0;
            case EBin.event_code_binary.VIBACTIVE:
                vib.VIB_setActive(this.getv1() != 0);
                return 0;
            case EBin.event_code_binary.VIBTRACK1:
                vib.VIB_setTrackActive(this.getv1(), this.getv1(), this.getv1() != 0);
                return 0;
            case EBin.event_code_binary.VIBTRACK:
                vib.VIB_setTrackToModulate((UInt32)this.getv1(), (UInt32)this.getv1(), this.getv1() != 0);
                return 0;
            case EBin.event_code_binary.VIBRATE:
                vib.VIB_setFrameRate((Int16)this.getv2());
                return 0;
            case EBin.event_code_binary.VIBFLAG:
                vib.VIB_setFlags((Int16)this.getv1());
                return 0;
            case EBin.event_code_binary.VIBRANGE:
                vib.VIB_setPlayRange((Int16)this.getv1(), (Int16)this.getv1());
                return 0;
            case EBin.event_code_binary.HINT:
                num1 = this.getv1(); // The only values of num1 are 0x5, 0x11 and 0x91 in non-modded scripts
                num2 = this.getv2();
                if (num1 == 0x30)
                {
                    // PreloadField( 48, ... )
                    // Add a field to the list of background free-view mode
                    this.sExternalFieldList.Add((short)num2);
                }
                else if (num1 == 0x31)
                {
                    // PreloadField( 49, ... )
                    // Start the background free-view mode, starting by the selected field
                    this.sOriginalFieldName = this.fieldmap.mapName;
                    this.sOriginalFieldNo = FF9StateSystem.Common.FF9.fldMapNo;
                    this.sExternalFieldChangeField = num2;
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
            case EBin.event_code_binary.JOIN:
                Int32 slot_no = this.chr2slot(this.getv1());
                Int32 num83 = this.getv1();
                EquipmentSetId eqp_id = new EquipmentSetId(this.getv1());
                if (slot_no >= 0 && slot_no < 9)
                {
                    PLAYER player = FF9StateSystem.Common.FF9.player[slot_no];
                    List<Int32> defaultEquipment = ff9play.GetDefaultEquipment(player.PresetId);
                    Int32 num13 = this.getv1();
                    if (num13 != (Int32)Byte.MaxValue)
                        player.category = (Byte)num13;
                    Int32 num53 = this.getv1();
                    if (num53 != (Int32)Byte.MaxValue)
                        player.info.menu_type = (Byte)num53;
                    ff9play.FF9Play_Change(slot_no, num83 != 0, defaultEquipment, eqp_id);
                }
                else
                {
                    this.getv1();
                    this.getv1();
                }
                return 0;
            case EBin.event_code_binary.EXT:
                Int32 num84 = this.gArgFlag | 65280;
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
                        this.fieldmap.walkMesh.BGI_simSetActive((UInt32)this.getv1(), (UInt32)this.getv1());
                        return 0;
                    case 65283:
                        this.fieldmap.walkMesh.BGI_simSetFlags((UInt32)this.getv1(), (UInt32)this.getv1());
                        return 0;
                    case 65284:
                        this.fieldmap.walkMesh.BGI_simSetFloor((UInt32)this.getv1(), (UInt32)this.getv1());
                        return 0;
                    case 65285:
                        this.fieldmap.walkMesh.BGI_simSetFrameRate((UInt32)this.getv1(), (Int16)this.getv2());
                        return 0;
                    case 65286:
                        this.fieldmap.walkMesh.BGI_simSetAlgorithm((UInt32)this.getv1(), (UInt32)this.getv1());
                        return 0;
                    case 65287:
                        this.fieldmap.walkMesh.BGI_simSetDelta((UInt32)this.getv1(), (Int16)this.getv2(), (Int16)this.getv2());
                        return 0;
                    case 65288:
                        this.fieldmap.walkMesh.BGI_simSetAxis((UInt32)this.getv1(), (UInt32)this.getv1());
                        return 0;
                    case 65289:
                        num1 = this.getv1();
                        num3 = this.getv1();
                        return 0;
                    case 65290:
                        this.fieldmap.walkMesh.BGI_animShowFrame((UInt32)this.getv1(), (UInt32)this.getv1());
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

    private static Boolean IsAlexandriaStageScene()
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
    }

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
        Vector3 vector3_1 = actor.rotAngle;
        Single num1 = EventEngineUtils.ClampAngle(vector3_1.y);
        actor.turnRot = EventEngineUtils.ClampAngle(a);
        a -= num1;
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
        actor.trot = num1;
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
                vector3_1.y = num1 + ((Double)actor.turnRot < (Double)num1 ? 0.0f : 360f);
                actor.rotAngle[1] = vector3_1.y;
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
                vector3_1.y = num1 - ((Double)actor.turnRot > (Double)num1 ? 0.0f : 360f);
                actor.rotAngle[1] = vector3_1.y;
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
            FieldMapActorController mapActorController = ((Actor)po).fieldMapActorController;
            if (!((UnityEngine.Object)mapActorController != (UnityEngine.Object)null))
                return;
            mapActorController.SetPosition(new Vector3(po.pos[0], po.pos[1], po.pos[2]), true, true);
        }
        else
        {
            if (this.gMode != 3)
                return;
            //if ((Int32)po.uid != (Int32)this._context.controlUID)
            //    ;
            ((Actor)po).wmActor.SetPosition(po.pos[0], po.pos[1], po.pos[2]);
            if ((Int32)po.index < 3 || (Int32)po.index > 7)
                return;
            Int32 posX = (Int32)po.pos[0];
            Int32 posY = (Int32)po.pos[1];
            Int32 posZ = (Int32)po.pos[2];
            ff9.w_movementChrVerifyValidCastPosition(ref posX, ref posY, ref posZ);
            po.pos[0] = (Single)posX;
            po.pos[1] = (Single)posY;
            po.pos[2] = (Single)posZ;
            ((Actor)po).wmActor.SetPosition(po.pos[0], po.pos[1], po.pos[2]);
        }
    }
}