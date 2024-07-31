using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldSPSSystem : MonoBehaviour
{
    public List<SPSEffect> SpsList => Utility.SpsList;

    public void Init()
    {
        this.Utility = new CommonSPSSystem();
        this._eventNoToIndex = new Dictionary<Int32, Int32>();
        for (Int32 i = 0; i < SPSConst.WORLD_DEFAULT_OBJCOUNT; i++)
            this.InitSPSInstance(i);
        for (Int32 i = 0; i < SPSConst.WORLD_DEFAULT_OBJLOAD; i++)
            if (!Utility.LoadSPSBin(new KeyValuePair<String, Int32>("WorldMap", i), out _))
                global::Debug.Log("Can't load sps id : " + i);
        Utility.LoadMapTextureInVram("WorldMap");
    }

    private void InitSPSInstance(Int32 index)
    {
        if (index < 0 || index > Utility.SpsList.Count)
            return;
        if (index < Utility.SpsList.Count)
        {
            Utility.SpsList[index].Unload();
            return;
        }
        GameObject spsGo = new GameObject($"SPS_{index:D4}");
        spsGo.transform.parent = base.transform;
        spsGo.transform.localScale = Vector3.one;
        spsGo.transform.localPosition = Vector3.zero;
        MeshRenderer meshRenderer = spsGo.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = spsGo.AddComponent<MeshFilter>();
        SPSEffect sps = spsGo.AddComponent<SPSEffect>();
        sps.Init(3);
        sps.spsIndex = index;
        sps.spsTransform = spsGo.transform;
        sps.meshRenderer = meshRenderer;
        sps.meshFilter = meshFilter;
        Utility.SpsList.Add(sps);
    }

    public void EffectUpdate()
    {
        if (!Utility.IsTCBReady)
            return;
        Int32 speed_move = ff9.w_moveCHRControlPtr.speed_move;
        ff9.w_effectMoveStock = speed_move != 0 ? (Int32)ff9.abs(ff9.w_moveCHRControl_XZSpeed * 256f * 4096f) / speed_move : 0;
        ff9.w_effectMoveStockTrue += (ff9.w_effectMoveStock - ff9.w_effectMoveStockTrue) / 32;
        foreach (SPSEffect sps in Utility.SpsList)
        {
            if (sps.spsBin != null && sps.spsId != -1)
            {
                SPSConst.WorldSPSEffect no = sps.worldSpsId;
                Int32 curFrame = sps.curFrame >> 4;
                sps.lastFrame = sps.curFrame;
                if (no == SPSConst.WorldSPSEffect.SMOKE_SOUTH_GATE)
                {
                    sps.pos[1] += ff9.S(ff9.random8() % 24 + 60);
                    sps.pos[0] += ff9.S(ff9.random8() % 60 - 30);
                    sps.pos[2] += ff9.S(ff9.random8() % 60 - 30);
                }
                else if (no == SPSConst.WorldSPSEffect.SMOKE_FIRE_SHRINE)
                {
                    sps.pos[1] += ff9.S(ff9.random8() % 24 + 60);
                    sps.pos[0] += ff9.S(ff9.random8() % 60 - 60);
                    sps.pos[2] += ff9.S(ff9.random8() % 60 - 30);
                }
                else if (no == SPSConst.WorldSPSEffect.SANDSTORM)
                {
                    sps.pos[0] = ff9.rsin(curFrame * 64 + sps.prm0) / 2 + ff9.w_effectTwisPos.x;
                    sps.pos[1] -= 80f;
                    sps.pos[2] = ff9.rcos(curFrame * 64 + sps.prm0) / 2 + ff9.w_effectTwisPos.z;
                }
                else if (no == SPSConst.WorldSPSEffect.WIND_SHRINE)
                {
                    sps.pos[0] += ff9.S(ff9.random8() % 130 + 40);
                    sps.pos[1] += ff9.S(ff9.random8() % 80 - 40);
                    sps.pos[2] += ff9.S(ff9.random8() % 300 + 300);
                }
                sps.curFrame += sps.frameRate;
                curFrame = sps.curFrame >> 4;
                if (no == SPSConst.WorldSPSEffect.SMOKE_SOUTH_GATE)
                {
                    if (curFrame > 76)
                        this.EffectRelease(sps);
                }
                else if (no == SPSConst.WorldSPSEffect.SMOKE_FIRE_SHRINE)
                {
                    if (curFrame > 45)
                        this.EffectRelease(sps);
                }
                else if (no == SPSConst.WorldSPSEffect.MOVE_FOREST || no == SPSConst.WorldSPSEffect.UNKNOWN17)
                {
                    if (curFrame > 8)
                        this.EffectRelease(sps);
                }
                else if (no == SPSConst.WorldSPSEffect.UNKNOWN32)
                {
                    if (ff9.abs(sps.pos[0] - ff9.w_moveActorPtr.pos[0]) > ff9.S(32000) || ff9.abs(sps.pos[2] - ff9.w_moveActorPtr.pos[2]) > ff9.S(32000))
                        this.EffectRelease(sps);
                }
                else if ((sps.attr & SPSConst.ATTR_UNLOAD_ON_FINISH) != 0)
                {
                    if (sps.curFrame >= sps.frameCount)
                        this.EffectRelease(sps);
                }
                if (sps.duration > 0)
                    sps.duration--;
                if (sps.duration == 0)
                    sps.Unload();
            }
        }
        if (ff9.w_evaCoreSPS != null)
        {
            ff9.w_evaCoreSPSCurrentSize += ff9.w_evaCoreSPSSpeedtSize;
            if (ff9.w_evaCoreSPSCurrentSize >= 60000)
                ff9.w_evaCoreSPSSpeedtSize = -ff9.w_evaCoreSPSSpeedtSize;
            else if (ff9.w_evaCoreSPSCurrentSize <= 46000)
                ff9.w_evaCoreSPSSpeedtSize = -ff9.w_evaCoreSPSSpeedtSize;
            ff9.w_evaCoreSPS.scale = ff9.w_evaCoreSPSCurrentSize;
        }
    }

    public void GenerateSPS()
    {
        if (!Utility.IsTCBReady)
            return;
        foreach (SPSEffect sps in Utility.SpsList)
        {
            if (sps.spsBin != null && sps.spsId != -1)
            {
                if ((sps.attr & (SPSConst.ATTR_UPDATE_THIS_FRAME | SPSConst.ATTR_UPDATE_ANY_FRAME)) == 0)
                {
                    sps.meshRenderer.enabled = false;
                    continue;
                }
                sps.meshRenderer.enabled = true;
                sps.attr &= unchecked((Byte)~SPSConst.ATTR_UPDATE_THIS_FRAME);
                if (sps.charTran != null && sps.boneTran != null)
                    sps.pos = sps.boneTran.position;
                SPSConst.WorldSPSEffect no = sps.worldSpsId;
                Vector3 goPos = sps.pos;
                Int32 curFrame = sps.curFrame >> 4;
                switch (no)
                {
                    case SPSConst.WorldSPSEffect.SMOKE_LARGE:
                    case SPSConst.WorldSPSEffect.SMOKE_NORMAL:
                        goPos.y += ff9.S(150);
                        ff9.w_frameShadowOTOffset = 40;
                        sps.fade = -1;
                        sps.GenerateSPS();
                        break;
                    case SPSConst.WorldSPSEffect.SMOKE_SOUTH_GATE:
                        ff9.w_frameShadowOTOffset = 0;
                        sps.fade = -1;
                        sps.scale = curFrame * 400 + 6000;
                        sps.works.wFactor = 2f;
                        sps.works.hFactor = 2f;
                        sps.GenerateSPS();
                        break;
                    case SPSConst.WorldSPSEffect.SMOKE_FIRE_SHRINE:
                        ff9.w_frameShadowOTOffset = 0;
                        sps.fade = -1;
                        sps.scale = curFrame * 400 + 6000;
                        sps.works.wFactor = 2f;
                        sps.works.hFactor = 2f;
                        sps.GenerateSPS();
                        break;
                    case SPSConst.WorldSPSEffect.CHEST_LAND:
                        ff9.w_frameShadowOTOffset = 20;
                        sps.rot = new Vector3(ff9.PsxRot(1024), 0f, 0f);
                        sps.fade = -1;
                        sps.GenerateSPS();
                        break;
                    case SPSConst.WorldSPSEffect.MOVE_CHOCOBO_ABOVE_DESERT:
                    case SPSConst.WorldSPSEffect.MOVE_AIRSHIP_ABOVE_WATER:
                    case SPSConst.WorldSPSEffect.MOVE_AIRSHIP_ABOVE_DESERT:
                    {
                        Int32 fade = ff9.w_effectMoveStockTrue * ff9.w_effectMoveStockHeightTrue >> 12;
                        ff9.w_effectMoveStockHeight = Mathf.Clamp(ff9.w_effectMoveStockHeight, 0, 4095);
                        ff9.w_effectMoveStockHeightTrue += (ff9.w_effectMoveStockHeight - ff9.w_effectMoveStockHeightTrue) / 32;
                        ff9.w_frameShadowOTOffset = 50;
                        sps.rot = new Vector3(ff9.PsxRot(1024), 0f, 0f);
                        if (fade > 24)
                        {
                            sps.fade = (Int32)(fade * 0.000244200259f * 255f);
                            sps.GenerateSPS();
                        }
                        break;
                    }
                    case SPSConst.WorldSPSEffect.MOVE_SHIP:
                        ff9.w_frameShadowOTOffset = 20;
                        sps.rot = new Vector3(ff9.PsxRot(1024), 0f, 0f);
                        sps.fade = 12;
                        sps.GenerateSPS();
                        break;
                    case SPSConst.WorldSPSEffect.MOVE_FOREST:
                        goPos.y += ff9.S(400);
                        ff9.w_frameShadowOTOffset = 20;
                        sps.scale = (8 - curFrame) * 4000;
                        sps.fade = -1;
                        if (ff9.effect16FrameCounter % 2 == 0)
                        {
                            sps.rot = new Vector3(ff9.PsxRot(1024), curFrame * 128, 0f);
                            sps.abr = SPSConst.ABR_ADD;
                        }
                        else
                        {
                            sps.rot = new Vector3(ff9.PsxRot(1024), curFrame * 200, 0f);
                            sps.abr = SPSConst.ABR_SUB;
                        }
                        sps.GenerateSPS();
                        ff9.effect16FrameCounter++;
                        break;
                    case SPSConst.WorldSPSEffect.WATERFALL:
                        goPos.z += ff9.S(-250);
                        ff9.w_frameShadowOTOffset = 40;
                        sps.fade = -1;
                        sps.GenerateSPS();
                        break;
                    case SPSConst.WorldSPSEffect.MOVE_WATER:
                        ff9.w_frameShadowOTOffset = 20;
                        sps.rot = new Vector3(ff9.PsxRot(1024), 0f, 0f);
                        sps.fade = 236;
                        sps.GenerateSPS();
                        break;
                    case SPSConst.WorldSPSEffect.MEMORIA:
                    case SPSConst.WorldSPSEffect.UNKNOWN26:
                    {
                        goPos.z -= 0.2f;
                        ff9.w_frameShadowOTOffset = 0;
                        sps.abr = SPSConst.ABR_ADD;
                        sps.fade = -1;
                        sps.GenerateSPS();
                        break;
                    }
                    case SPSConst.WorldSPSEffect.WATER_SHRINE:
                        goPos.y += ff9.S(300);
                        ff9.w_frameShadowOTOffset = 40;
                        sps.fade = -1;
                        sps.GenerateSPS();
                        break;
                    default:
                        ff9.w_frameShadowOTOffset = 40;
                        sps.fade = -1;
                        sps.GenerateSPS();
                        break;
                }
                sps.transform.position = goPos;
                if (sps.rot == Vector3.zero || no == SPSConst.WorldSPSEffect.NOT_WORLD_SPS)
                {
                    Quaternion rotation = ff9.world.MainCamera.transform.rotation;
                    if (no == SPSConst.WorldSPSEffect.MEMORIA)
                    {
                        Transform effectBlackTransform = ff9.world.kWorldPackEffectBlack;
                        sps.transform.position = effectBlackTransform.position + effectBlackTransform.TransformDirection(new Vector3(0f, 0f, 0.5f));
                    }
                    sps.transform.LookAt(sps.transform.position + rotation * Vector3.back, rotation * Vector3.up);
                }
                else
                {
                    sps.transform.rotation = Quaternion.Euler(sps.rot);
                }
            }
        }
    }

    public Int32 EffectRegist(Single x, Single y, Single z, SPSConst.WorldSPSEffect no, Int32 size)
    {
        Int32 slot = this._FindFreeEffectSlot();
        SPSEffect sps = Utility.SpsList[slot];
        if (!Utility.SetupSPSBinary(sps, new KeyValuePair<String, Int32>("WorldMap", (Int32)no), false))
            return -1;
        switch (no)
        {
            case SPSConst.WorldSPSEffect.SMOKE_SOUTH_GATE:
            case SPSConst.WorldSPSEffect.SMOKE_FIRE_SHRINE:
                sps.abr = SPSConst.ABR_SUB;
                break;
            case SPSConst.WorldSPSEffect.MOVE_FOREST:
                sps.abr = Byte.MaxValue;
                break;
            default:
                sps.abr = SPSConst.ABR_ADD;
                break;
        }
        sps.attr |= SPSConst.ATTR_UPDATE_ANY_FRAME;
        if (no != SPSConst.WorldSPSEffect.MEMORIA)
            sps.attr |= SPSConst.ATTR_UNLOAD_ON_FINISH;
        sps.pos.x = x;
        sps.pos.y = y;
        sps.pos.z = z;
        sps.rot = Vector3.zero;
        sps.scale = size;
        sps.prm0 = 0;
        sps.meshRenderer.enabled = true;
        if (no == SPSConst.WorldSPSEffect.SMOKE_SOUTH_GATE)
            sps.frameRate = 4;
        else if (no == SPSConst.WorldSPSEffect.SMOKE_FIRE_SHRINE)
            sps.frameRate = 8;
        else if (no == SPSConst.WorldSPSEffect.MEMORIA)
            sps.frameRate = 0;
        return slot;
    }

    public void ShiftRight()
    {
        foreach (SPSEffect sps in Utility.SpsList)
        {
            if (sps.spsId != -1)
            {
                Vector3 position = sps.transform.position;
                position.x += 64f;
                if (position.x >= 1536f)
                    position.x -= 1536f;
                sps.pos = position;
                sps.transform.position = position;
            }
        }
    }

    public void ShiftLeft()
    {
        foreach (SPSEffect sps in Utility.SpsList)
        {
            if (sps.spsId != -1)
            {
                Vector3 position = sps.transform.position;
                position.x -= 64f;
                if (position.x < 0f)
                    position.x += 1536f;
                sps.pos = position;
                sps.transform.position = position;
            }
        }
    }

    public void ShiftDown()
    {
        foreach (SPSEffect sps in Utility.SpsList)
        {
            if (sps.spsId != -1)
            {
                Vector3 position = sps.transform.position;
                position.z -= 64f;
                if (position.z <= -1280f)
                    position.z += 1280f;
                sps.pos = position;
                sps.transform.position = position;
            }
        }
    }

    public void ShiftUp()
    {
        foreach (SPSEffect sps in Utility.SpsList)
        {
            if (sps.spsId != -1)
            {
                Vector3 position = sps.transform.position;
                position.z += 64f;
                if (position.z > 0f)
                    position.z -= 1280f;
                sps.pos = position;
                sps.transform.position = position;
            }
        }
    }

    public void EffectRelease(SPSEffect sps)
    {
        sps.Unload();
        sps.spsId = -1;
    }

    public void SetObjParm(Int32 ObjNo, Int32 ParmType, Int32 Arg0, Int32 Arg1, Int32 Arg2)
    {
        if (!this._eventNoToIndex.TryGetValue(ObjNo, out Int32 slot))
        {
            if (ParmType != SPSConst.OPERATION_LOAD && ParmType != SPSConst.OPERATION_CHANGE_FIELD)
                return;
            slot = this._FindFreeEffectSlot();
            this._eventNoToIndex[ObjNo] = slot;
        }
        if (ParmType == SPSConst.OPERATION_POS)
        {
            Single x = ff9.S(Arg0);
            Single y = ff9.S(-Arg1);
            Single z = ff9.S(Arg2);
            ff9.world.SetAbsolutePositionOf(out Vector3 absPos, new Vector3(x, y, z));
            Arg0 = (Int32)absPos.x;
            Arg1 = -(Int32)absPos.y;
            Arg2 = (Int32)absPos.z;
        }
        Utility.SetObjParm(Utility.SpsList[slot], ParmType, Arg0, Arg1, Arg2);
        if (ParmType == SPSConst.OPERATION_LOAD && Arg0 == SPSConst.REF_DELETE)
            this._eventNoToIndex.Remove(ObjNo);
    }

    private Boolean _IsSlotAvailable(Int32 index)
    {
        return Utility.SpsList[index].spsId == -1;
    }

    private Int32 _FindFreeEffectSlot()
    {
        for (Int32 i = 0; i < Utility.SpsList.Count; i++)
            if (this._IsSlotAvailable(i))
                return i;
        Int32 slot = Utility.SpsList.Count;
        this.InitSPSInstance(slot);
        return slot;
    }

    [NonSerialized]
    private CommonSPSSystem Utility;
    [NonSerialized]
    private Dictionary<Int32, Int32> _eventNoToIndex;
}
