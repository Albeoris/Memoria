using Memoria;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleSPSSystem : MonoBehaviour
{
    public List<SPSEffect> SpsList => this.Utility.SpsList;

    public void Init()
    {
        this.Utility = new CommonSPSSystem();
        this._shpEffects = new List<SHPEffect>();
        this._eventNoToIndex = new Dictionary<Int32, Int32>();
        this._statusToSPSIndex = new Dictionary<KeyValuePair<Int32, Int32>, Int32>();
        this._statusToSHPIndex = new Dictionary<KeyValuePair<Int32, Int32>, Int32>();
        for (Int32 i = 0; i < SPSConst.BATTLE_DEFAULT_OBJCOUNT; i++)
            this.InitSPSInstance(i);
    }

    private void InitSPSInstance(Int32 index)
    {
        if (index < 0 || index > Utility.SpsList.Count)
            return;
        if (index < Utility.SpsList.Count)
        {
            Utility.SpsList[index].Init(2);
            return;
        }
        GameObject spsGo = new GameObject($"SPS_{index:D4}");
        spsGo.transform.parent = base.transform;
        spsGo.transform.localScale = Vector3.one;
        spsGo.transform.localPosition = Vector3.zero;
        MeshRenderer meshRenderer = spsGo.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = spsGo.AddComponent<MeshFilter>();
        SPSEffect sps = spsGo.AddComponent<SPSEffect>();
        sps.Init(2);
        sps.spsIndex = index;
        sps.spsTransform = spsGo.transform;
        sps.meshRenderer = meshRenderer;
        sps.meshFilter = meshFilter;
        Utility.SpsList.Add(sps);
    }

    public void EffectUpdate()
    {
        foreach (SPSEffect sps in Utility.SpsList)
        {
            if (sps.spsBin != null && (sps.attr & SPSConst.ATTR_VISIBLE) != 0 && sps.lastFrame != -1)
            {
                sps.lastFrame = sps.curFrame;
                sps.curFrame += sps.frameRate;
                if (sps.curFrame >= sps.frameCount)
                {
                    sps.curFrame = 0;
                    if ((sps.attr & SPSConst.ATTR_UNLOAD_ON_FINISH) != 0)
                        sps.Unload();
                }
                else if (sps.curFrame < 0)
                {
                    sps.curFrame = (sps.frameCount >> 4) - 1 << 4;
                }
                if (sps.duration > 0)
                    sps.duration--;
                if (sps.duration == 0)
                    sps.Unload();
            }
        }
        foreach (SHPEffect shp in this._shpEffects)
        {
            shp.frame++;
            if (shp.cycleDuration > 0 && shp.frame >= shp.cycleDuration && (shp.attr & SPSConst.ATTR_UNLOAD_ON_FINISH) != 0)
                shp.Unload();
            if (shp.duration > 0)
                shp.duration--;
            if (shp.duration == 0)
                shp.Unload();
        }
        //for (Int32 i = 0; i < this._specialSpsList.Count; i++)
        //{
        //	BattleSPS special_sps = this._specialSpsList[i];
        //	if ((special_sps.type != 0 || special_sps.spsBin != null) && (special_sps.attr & 1) != 0 && special_sps.lastFrame != -1 && special_sps.isUpdate)
        //	{
        //		special_sps.lastFrame = special_sps.curFrame;
        //		special_sps.curFrame += special_sps.frameRate;
        //		if (special_sps.curFrame >= special_sps.frameCount)
        //		{
        //			special_sps.curFrame = 0;
        //		}
        //		else if (special_sps.curFrame < 0)
        //		{
        //			special_sps.curFrame = (special_sps.frameCount >> 4) - 1 << 4;
        //		}
        //	}
        //}
    }

    public void GenerateSPS()
    {
        foreach (SPSEffect sps in Utility.SpsList)
        {
            if (sps.spsBin != null && (sps.attr & SPSConst.ATTR_VISIBLE) != 0)
            {
                if (sps.charTran != null && sps.boneTran != null)
                    sps.pos = sps.boneTran.position + sps.posOffset;
                if ((sps.attr & (SPSConst.ATTR_UPDATE_THIS_FRAME | SPSConst.ATTR_UPDATE_ANY_FRAME)) != 0)
                {
                    sps.meshRenderer.enabled = true;
                    sps.GenerateSPS();
                    sps.attr &= unchecked((Byte)~SPSConst.ATTR_UPDATE_THIS_FRAME);
                }
                else
                {
                    sps.meshRenderer.enabled = false;
                }
            }
            sps.lastFrame = sps.curFrame;
        }
        foreach (SHPEffect shp in this._shpEffects)
            shp.AnimateSHP();
        //bool show_special = false;
        //for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
        //	if (next.bi.player == 0 && next.bi.disappear == 0)
        //		show_special = true;
        //for (Int32 i = 0; i < this._specialSpsList.Count; i++)
        //{
        //	BattleSPS special_sps = this._specialSpsList[i];
        //	if ((special_sps.type != 0 || special_sps.spsBin != null) && (special_sps.attr & 1) != 0 && special_sps.isUpdate)
        //	{
        //		double rotation_cos = Math.Cos(2 * Math.PI * special_sps.curFrame / 10000) * this._specialSpsFadingList[i]; // 1 turn every 10 seconds
        //		double rotation_sin = Math.Sin(2 * Math.PI * special_sps.curFrame / 10000) * this._specialSpsFadingList[i];
        //		Vector3 rotated_pos = BattleSPSSystem.statusTextures[special_sps.refNo].extraPos;
        //		float tmp = rotated_pos.x;
        //		rotated_pos.x = (float)(rotation_cos * tmp - rotation_sin * rotated_pos.z);
        //		rotated_pos.z = (float)(rotation_sin * tmp + rotation_cos * rotated_pos.z);
        //		for (int j = 0; j < special_sps.shpGo.Length; j++)
        //			special_sps.shpGo[j].transform.localPosition = rotated_pos;
        //		special_sps.isUpdate = show_special;
        //		special_sps.AnimateSHP();
        //		special_sps.lastFrame = special_sps.curFrame;
        //		if (this._specialSpsRemovingList[i])
        //			this._specialSpsFadingList[i] -= 0.05f;
        //		if (this._specialSpsFadingList[i] > 0.0f)
        //			special_sps.isUpdate = true;
        //		else
        //			for (int j = 0; j < special_sps.shpGo.Length; j++)
        //				special_sps.shpGo[j].SetActive(false);
        //	}
        //}
    }

    public void SetObjParm(Int32 ObjNo, Int32 ParmType, Int32 Arg0, Int32 Arg1, Int32 Arg2)
    {
        if (!this._eventNoToIndex.TryGetValue(ObjNo, out Int32 slot))
        {
            if (ParmType != SPSConst.OPERATION_LOAD && ParmType != SPSConst.OPERATION_CHANGE_FIELD)
                return;
            slot = this._FindFreeSPSSlot();
            this._eventNoToIndex[ObjNo] = slot;
        }
        Utility.SetObjParm(Utility.SpsList[slot], ParmType, Arg0, Arg1, Arg2);
        if (ParmType == SPSConst.OPERATION_LOAD && Arg0 == SPSConst.REF_DELETE)
            this._eventNoToIndex.Remove(ObjNo);
    }

    public void UpdateBtlStatus(BattleUnit unit, BattleStatusId statusId, Vector3? spsPos = null, Vector3? shpPos = null, Int32? frame = null)
    {
        KeyValuePair<Int32, Int32> effectCode = new KeyValuePair<Int32, Int32>(unit.Position, (Int32)statusId);
        if (this._statusToSPSIndex.TryGetValue(effectCode, out Int32 spsIndex))
        {
            SPSEffect sps = Utility.SpsList[spsIndex];
            sps.attr |= SPSConst.ATTR_VISIBLE | SPSConst.ATTR_UPDATE_THIS_FRAME;
            if (spsPos.HasValue)
                sps.pos = spsPos.Value;
            if (frame.HasValue)
                sps.curFrame = frame.Value << 4;
        }
        if (this._statusToSHPIndex.TryGetValue(effectCode, out Int32 shpIndex))
        {
            SHPEffect shp = this._shpEffects[shpIndex];
            shp.attr |= SPSConst.ATTR_VISIBLE | SPSConst.ATTR_UPDATE_THIS_FRAME;
            if (shpPos.HasValue)
                shp.pos = shpPos.Value;
            if (frame.HasValue)
                shp.frame = frame.Value;
        }
    }

    public void AddBtlSPSObj(BattleUnit unit, BattleStatusId statusId, Int32 spsId = -1, Int32 shpId = -1, Vector3 extraPos = default)
    {
        KeyValuePair<Int32, Int32> effectCode = new KeyValuePair<Int32, Int32>(unit.Position, (Int32)statusId);
        if (spsId >= 0)
        {
            if (this._statusToSPSIndex.TryGetValue(effectCode, out Int32 spsIndex))
            {
                Utility.SpsList[spsIndex].gameObject.transform.localPosition = extraPos;
                Utility.SpsList[spsIndex].attr |= SPSConst.ATTR_VISIBLE;
            }
            else
            {
                if (!CommonSPSSystem.SPSPrototypes.ContainsKey(spsId))
                {
                    Log.Error($"[{nameof(BattleSPSSystem)}] Status {statusId} tries to use the SPS {spsId} which does not exists");
                    return;
                }
                KeyValuePair<String, Int32> spsID = new KeyValuePair<String, Int32>($"FromPrototype", spsId);
                Int32 slot = this._FindFreeSPSSlot();
                this._statusToSPSIndex[effectCode] = slot;
                SPSEffect sps = Utility.SpsList[slot];
                if (Utility.SetupSPSBinary(sps, spsID, true))
                {
                    sps.meshRenderer.enabled = false;
                    sps.gameObject.transform.localPosition = extraPos;
                }
            }
        }
        if (shpId >= 0)
        {
            if (this._statusToSHPIndex.TryGetValue(effectCode, out Int32 shpIndex))
            {
                this._shpEffects[shpIndex].attr |= SPSConst.ATTR_VISIBLE;
                this._shpEffects[shpIndex].gameObject.transform.localPosition = extraPos;
            }
            else
            {
                if (!CommonSPSSystem.SPSPrototypes.ContainsKey(shpId))
                {
                    Log.Error($"[{nameof(BattleSPSSystem)}] Status {statusId} tries to use the SHP {shpId} which does not exists");
                    return;
                }
                Int32 slot = this._FindFreeSHPSlot();
                this._statusToSHPIndex[effectCode] = slot;
                this._shpEffects[slot].Init(CommonSPSSystem.SHPPrototypes[shpId]);
                this._shpEffects[slot].gameObject.transform.localPosition = extraPos;
            }
        }
    }

    public void RemoveBtlSPSObj(BattleUnit unit, BattleStatusId statusId)
    {
        // Don't unload the SPS / SHP; assume there's a relatively high probability it can be re-used
        KeyValuePair<Int32, Int32> effectCode = new KeyValuePair<Int32, Int32>(unit.Position, (Int32)statusId);
        if (this._statusToSPSIndex.TryGetValue(effectCode, out Int32 spsIndex))
        {
            Utility.SpsList[spsIndex].attr = 0;
            Utility.SpsList[spsIndex].meshRenderer.enabled = false;
            //Utility.SpsList[spsIndex].Unload();
            //this._statusToSPSIndex.Remove(effectCode);
        }
        if (this._statusToSHPIndex.TryGetValue(effectCode, out Int32 shpIndex))
        {
            this._shpEffects[shpIndex].attr = 0;
            foreach (GameObject go in this._shpEffects[shpIndex].shpGo)
                go.SetActive(false);
            //this._shpEffects[shpIndex].Unload();
            //this._statusToSHPIndex.Remove(effectCode);
        }
    }

    public SPSEffect AddSequenceSPS(Int32 spsId, Int32 duration, Single speed)
    {
        if (!CommonSPSSystem.SPSPrototypes.ContainsKey(spsId))
        {
            Log.Error($"[{nameof(BattleSPSSystem)}] A custom SFX sequence tries to use the SPS {spsId} which does not exists");
            return null;
        }
        KeyValuePair<String, Int32> spsID = new KeyValuePair<String, Int32>($"FromPrototype", spsId);
        Int32 slot = this._FindFreeSPSSlot();
        SPSEffect sps = Utility.SpsList[slot];
        if (!Utility.SetupSPSBinary(sps, spsID, true))
            return null;
        sps.meshRenderer.enabled = false;
        sps.frameRate = (Int32)(sps.frameRate * speed);
        sps.attr |= SPSConst.ATTR_UPDATE_ANY_FRAME;
        if (duration < 0 && sps.frameRate > 0)
            sps.attr |= SPSConst.ATTR_UNLOAD_ON_FINISH;
        else if (duration >= 0)
            sps.duration = duration;
        else if (sps.frameRate < 0)
            sps.duration = sps.frameCount / -sps.frameRate;
        else
            sps.duration = 0;
        return sps;
    }

    public SHPEffect AddSequenceSHP(Int32 shpId, Int32 duration, Single speed)
    {
        if (!CommonSPSSystem.SHPPrototypes.ContainsKey(shpId))
        {
            Log.Error($"[{nameof(BattleSPSSystem)}] A custom SFX sequence tries to use the SHP {shpId} which does not exists");
            return null;
        }
        Int32 slot = this._FindFreeSHPSlot();
        SHPEffect shp = this._shpEffects[slot];
        shp.Init(CommonSPSSystem.SHPPrototypes[shpId]);
        shp.cycleDuration = (speed >= 0 ? 1 : -1) * Math.Max(1, Math.Abs((Int32)(shp.cycleDuration / Math.Abs(speed))));
        shp.attr |= SPSConst.ATTR_UPDATE_ANY_FRAME;
        if (duration < 0 && shp.cycleDuration > 0)
            shp.attr |= SPSConst.ATTR_UNLOAD_ON_FINISH;
        else if (duration >= 0)
            shp.duration = duration;
        else
            shp.duration = -shp.cycleDuration;
        return shp;
    }

    public void AddSpecialSPSObj(int specialid, uint spstype, Vector3 pos, float scale)
    {
        //BattleSPS special_sps;
        //if (specialid < 0 || specialid > _specialSpsList.Count)
        //	specialid = _specialSpsList.Count;
        //if (specialid == _specialSpsList.Count)
        //{
        //	GameObject gameObject = new GameObject("SpecialSPS_" + specialid.ToString("D4"));
        //	gameObject.transform.parent = base.transform;
        //	gameObject.transform.localScale = Vector3.one;
        //	gameObject.transform.localPosition = Vector3.zero;
        //	MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //	MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        //	special_sps = gameObject.AddComponent<BattleSPS>();
        //	special_sps.Init();
        //	special_sps.spsIndex = specialid;
        //	special_sps.spsTransform = gameObject.transform;
        //	special_sps.meshRenderer = meshRenderer;
        //	special_sps.meshFilter = meshFilter;
        //	this._specialSpsList.Add(special_sps);
        //	this._specialSpsFadingList.Add(1.0f);
        //	this._specialSpsRemovingList.Add(false);
        //}
        //else
        //{
        //	special_sps = this._specialSpsList[specialid];
        //	this._specialSpsFadingList[specialid] = 1.0f;
        //	this._specialSpsRemovingList[specialid] = false;
        //}
        //special_sps.pos = pos;
        //special_sps.curFrame = 0;
        //special_sps.lastFrame = 0;
        //special_sps.frameCount = 10000;
        //special_sps.attr |= 1;
        //special_sps.isUpdate = true;
        //special_sps.refNo = (int)spstype;
        //special_sps.type = (BattleSPSSystem.statusTextures[(int)spstype].type.Equals("shp") ? 1 : 0);
        //special_sps.scale = (int)(scale * 4096);
        //if (special_sps.shpGo == null)
        //	special_sps.GenerateSHP();
    }

    public void RemoveSpecialSPSObj(int specialid)
    {
        //if (specialid < 0 || specialid >= _specialSpsList.Count)
        //	return;
        //this._specialSpsRemovingList[specialid] = true;
    }

    private Int32 _FindFreeSPSSlot()
    {
        for (Int32 i = 0; i < Utility.SpsList.Count; i++)
            if (Utility.SpsList[i].spsId == -1)
                return i;
        Int32 slot = Utility.SpsList.Count;
        this.InitSPSInstance(slot);
        return slot;
    }

    private Int32 _FindFreeSHPSlot()
    {
        for (Int32 i = 0; i < this._shpEffects.Count; i++)
            if (this._shpEffects[i].shpId == -1)
                return i;
        Int32 slot = this._shpEffects.Count;
        GameObject shpGo = new GameObject($"SHP_{slot:D4}");
        shpGo.transform.parent = base.transform;
        shpGo.transform.localScale = Vector3.one;
        shpGo.transform.localPosition = Vector3.zero;
        shpGo.AddComponent<MeshRenderer>();
        shpGo.AddComponent<MeshFilter>();
        SHPEffect shp = shpGo.AddComponent<SHPEffect>();
        this._shpEffects.Add(shp);
        return slot;
    }

    [NonSerialized]
    private CommonSPSSystem Utility;
    [NonSerialized]
    private List<SHPEffect> _shpEffects;
    [NonSerialized]
    private Dictionary<Int32, Int32> _eventNoToIndex;
    [NonSerialized]
    private Dictionary<KeyValuePair<Int32, Int32>, Int32> _statusToSPSIndex;
    [NonSerialized]
    private Dictionary<KeyValuePair<Int32, Int32>, Int32> _statusToSHPIndex;
}
