using System;
using System.Collections.Generic;
using Memoria;
using Memoria.Data;
using UnityEngine;

public class BattleSPSSystem : MonoBehaviour
{
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
                    sps.curFrame = 0;
                else if (sps.curFrame < 0)
                    sps.curFrame = (sps.frameCount >> 4) - 1 << 4;
            }
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
                if ((sps.attr & SPSConst.ATTR_UPDATE_PER_FRAME) != 0)
                {
                    sps.meshRenderer.enabled = true;
                    sps.GenerateSPS();
                    sps.attr &= unchecked((Byte)~SPSConst.ATTR_UPDATE_PER_FRAME);
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
            slot = this._FindFreeEffectSlot();
            this._eventNoToIndex[ObjNo] = slot;
        }
        Utility.SetObjParm(Utility.SpsList[slot], ParmType, Arg0, Arg1, Arg2);
	}

	public void UpdateBtlStatus(BTL_DATA btl, BattleStatus status, Vector3 pos, Vector3 rot, Int32 frame)
    {
        KeyValuePair<Int32, Int32> effectCode = new KeyValuePair<Int32, Int32>(btl.bi.line_no, (Int32)status);
        if (this._statusToSPSIndex.TryGetValue(effectCode, out Int32 spsIndex))
        {
            SPSEffect sps = Utility.SpsList[spsIndex];
            sps.pos = pos;
            sps.curFrame = frame << 4;
            sps.attr |= SPSConst.ATTR_VISIBLE | SPSConst.ATTR_UPDATE_PER_FRAME;
        }
        if (this._statusToSHPIndex.TryGetValue(effectCode, out Int32 shpIndex))
        {
            SHPEffect shp = this._shpEffects[shpIndex];
            shp.pos = pos;
            shp.frame = frame;
            shp.attr |= SPSConst.ATTR_VISIBLE | SPSConst.ATTR_UPDATE_PER_FRAME;
        }
	}

	public static btl2d.STAT_ICON_TBL GetStatusStatIcon(BattleStatus status)
	{
		for (Int32 i = 0; i < btl2d.wStatIconTbl.Length; i++)
			if (btl2d.wStatIconTbl[i].Mask == status)
				return btl2d.wStatIconTbl[i];
		return null;
	}

	public void AddBtlSPSObj(BattleUnit unit, BattleStatus status)
    {
        if (!BattleSPSSystem.StatusVisualEffects.TryGetValue(status, out BattleSPSSystem.StatusEffect effect))
            return;
        KeyValuePair<Int32, Int32> effectCode = new KeyValuePair<Int32, Int32>(unit.Position, (Int32)status);
        if (effect.spsIndex >= 0)
        {
            if (!this._statusToSPSIndex.ContainsKey(effectCode))
            {
                KeyValuePair<String, Int32> spsID = new KeyValuePair<String, Int32>($"PNG:EmbeddedAsset/BattleMap/Status/{status}", effect.spsIndex);
                Int32 slot = this._FindFreeEffectSlot();
                this._statusToSPSIndex[effectCode] = slot;
                SPSEffect sps = Utility.SpsList[slot];
                if (Utility.SetupSPSBinary(sps, spsID, true))
                {
                    btl2d.STAT_ICON_TBL table = BattleSPSSystem.GetStatusStatIcon(status);
                    sps.abr = table?.Abr ?? 0;
                    sps.attr = 0;
                    sps.meshRenderer.enabled = false;
                    sps.battleScaleFactor = effect.spsScale;
                    sps.battleDistanceFactor = effect.spsDistance;
                }
            }
        }
        if (effect.shpIndex >= 0)
        {
            if (this._statusToSHPIndex.TryGetValue(effectCode, out Int32 shpIndex))
            {
                this._shpEffects[shpIndex].attr |= SPSConst.ATTR_VISIBLE;
            }
            else
            {
                Int32 slot = this._shpEffects.Count;
                this._statusToSHPIndex[effectCode] = slot;
                GameObject shpGo = new GameObject($"SHP_{slot:D4}");
                shpGo.transform.parent = base.transform;
                shpGo.transform.localScale = Vector3.one;
                shpGo.transform.localPosition = Vector3.zero;
                shpGo.AddComponent<MeshRenderer>();
                shpGo.AddComponent<MeshFilter>();
                SHPEffect shp = shpGo.AddComponent<SHPEffect>();
                shp.Init(SHPEffect.Database[effect.shpIndex]);
                this._shpEffects.Add(shp);
            }
        }
	}

	public void RemoveBtlSPSObj(BTL_DATA btl, BattleStatus status)
    {
        KeyValuePair<Int32, Int32> effectCode = new KeyValuePair<Int32, Int32>(btl.bi.line_no, (Int32)status);
        if (this._statusToSPSIndex.TryGetValue(effectCode, out Int32 spsIndex))
        {
            Utility.SpsList[spsIndex].Unload();
            this._statusToSPSIndex.Remove(effectCode);
        }
        if (this._statusToSHPIndex.TryGetValue(effectCode, out Int32 shpIndex))
        {
            this._shpEffects[shpIndex].attr = 0;
            foreach (GameObject go in this._shpEffects[shpIndex].shpGo)
                go.SetActive(false);
        }
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

    private Int32 _FindFreeEffectSlot()
    {
        for (Int32 i = 0; i < Utility.SpsList.Count; i++)
            if (Utility.SpsList[i].spsId == -1)
                return i;
        Int32 slot = Utility.SpsList.Count;
        this.InitSPSInstance(slot);
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

    //public String MapName;
    //private Boolean _isReady;
    //private List<BattleSPS> _spsList;
    //
    //// Custom fields: special SPS effects
    //private List<BattleSPS> _specialSpsList;
    //private List<float> _specialSpsFadingList;
    //private List<bool> _specialSpsRemovingList;
    //
    //private Dictionary<Int32, KeyValuePair<Int32, Byte[]>> _spsBinDict;
    //public Vector3 rot;

	public class StatusEffect
	{
		public StatusEffect(Int32 shpIndex, Int32 spsIndex, Single scale = 4f, Single distance = 5f)
		{
			this.shpIndex = shpIndex;
			this.spsIndex = spsIndex;
			this.spsScale = scale;
			this.spsDistance = distance;
		}

        public Int32 shpIndex;
        public Int32 spsIndex;

		public Single spsScale;
		public Single spsDistance;
	}

    public static Dictionary<BattleStatus, BattleSPSSystem.StatusEffect> StatusVisualEffects = new Dictionary<BattleStatus, BattleSPSSystem.StatusEffect>()
    {
        { BattleStatus.Slow, new BattleSPSSystem.StatusEffect(0, -1) },
        { BattleStatus.Haste, new BattleSPSSystem.StatusEffect(1, -1) },
        { BattleStatus.Silence, new BattleSPSSystem.StatusEffect(2, -1) },
        { BattleStatus.Trouble, new BattleSPSSystem.StatusEffect(3, -1) },
        { BattleStatus.Poison, new BattleSPSSystem.StatusEffect(-1, 0, 6f, 4f) },
        { BattleStatus.Venom, new BattleSPSSystem.StatusEffect(-1, 1, 6f, 4f) },
        { BattleStatus.Sleep, new BattleSPSSystem.StatusEffect(-1, 2, 6f, 4f) },
        { BattleStatus.Heat, new BattleSPSSystem.StatusEffect(-1, 3, 6f, 4f) },
        { BattleStatus.Freeze, new BattleSPSSystem.StatusEffect(-1, 4, 6f, 4f) },
        { BattleStatus.Reflect, new BattleSPSSystem.StatusEffect(-1, 5, 6f, 4f) },
        { BattleStatus.Blind, new BattleSPSSystem.StatusEffect(-1, 6, 6f, 4f) },
        { BattleStatus.Berserk, new BattleSPSSystem.StatusEffect(-1, 7, 6f, 4f) }
    };
}
