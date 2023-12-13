using System;
using System.Collections.Generic;
using Memoria.Data;
using UnityEngine;

public class BattleSPSSystem : MonoBehaviour
{
	public List<BattleSPS> GetSPSList()
	{
		return this._spsList;
	}

	public void Init()
	{
		this.rot = new Vector3(0f, 0f, 0f);
		this._isReady = false;
		this._spsList = new List<BattleSPS>();
		this._specialSpsList = new List<BattleSPS>();
		this._specialSpsFadingList = new List<float>();
		this._specialSpsRemovingList = new List<bool>();
		this._spsBinDict = new Dictionary<Int32, KeyValuePair<Int32, Byte[]>>();
		for (Int32 i = 0; i < 96; i++)
		{
			GameObject gameObject = new GameObject("SPS_" + i.ToString("D4"));
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			BattleSPS battleSPS = gameObject.AddComponent<BattleSPS>();
			battleSPS.Init();
			battleSPS.spsIndex = i;
			battleSPS.spsTransform = gameObject.transform;
			battleSPS.meshRenderer = meshRenderer;
			battleSPS.meshFilter = meshFilter;
			this._spsList.Add(battleSPS);
		}
		this.MapName = FF9StateSystem.Field.SceneName;
		this._isReady = this._loadSPSTexture();
	}

	public void Service()
	{
		if (!this._isReady)
		{
			return;
		}
		for (Int32 i = 0; i < this._spsList.Count; i++)
		{
			BattleSPS battleSPS = this._spsList[i];
			if ((battleSPS.type != 0 || battleSPS.spsBin != null) && (battleSPS.attr & 1) != 0)
			{
				if (battleSPS.lastFrame != -1)
				{
					battleSPS.lastFrame = battleSPS.curFrame;
					battleSPS.curFrame += battleSPS.frameRate;
					if (battleSPS.curFrame >= battleSPS.frameCount)
					{
						battleSPS.curFrame = 0;
					}
					else if (battleSPS.curFrame < 0)
					{
						battleSPS.curFrame = (battleSPS.frameCount >> 4) - 1 << 4;
					}
				}
			}
		}
		for (Int32 i = 0; i < this._specialSpsList.Count; i++)
		{
			BattleSPS special_sps = this._specialSpsList[i];
			if ((special_sps.type != 0 || special_sps.spsBin != null) && (special_sps.attr & 1) != 0 && special_sps.lastFrame != -1 && special_sps.isUpdate)
			{
				special_sps.lastFrame = special_sps.curFrame;
				special_sps.curFrame += special_sps.frameRate;
				if (special_sps.curFrame >= special_sps.frameCount)
				{
					special_sps.curFrame = 0;
				}
				else if (special_sps.curFrame < 0)
				{
					special_sps.curFrame = (special_sps.frameCount >> 4) - 1 << 4;
				}
			}
		}
	}

	public void GenerateSPS()
	{
		if (!this._isReady)
		{
			return;
		}
		for (Int32 i = 0; i < this._spsList.Count; i++)
		{
			BattleSPS battleSPS = this._spsList[i];
			if ((battleSPS.type != 0 || battleSPS.spsBin != null) && (battleSPS.attr & 1) != 0)
			{
				if (battleSPS.type == 0)
				{
					if (battleSPS.charTran != (UnityEngine.Object)null && battleSPS.boneTran != (UnityEngine.Object)null)
					{
						battleSPS.pos = battleSPS.boneTran.position + battleSPS.posOffset;
					}
					if (battleSPS.isUpdate)
					{
						battleSPS.meshRenderer.enabled = true;
						battleSPS.GenerateSPS();
						battleSPS.isUpdate = false;
					}
					else
					{
						battleSPS.meshRenderer.enabled = false;
					}
				}
				else
				{
					battleSPS.AnimateSHP();
				}
				battleSPS.lastFrame = battleSPS.curFrame;
			}
		}
		bool show_special = false;
		for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
			if (next.bi.player == 0 && next.bi.disappear == 0)
				show_special = true;
		for (Int32 i = 0; i < this._specialSpsList.Count; i++)
		{
			BattleSPS special_sps = this._specialSpsList[i];
			if ((special_sps.type != 0 || special_sps.spsBin != null) && (special_sps.attr & 1) != 0 && special_sps.isUpdate)
			{
				if (special_sps.rotate)
				{
                    double rotation_cos = Math.Cos(2 * Math.PI * special_sps.curFrame / 10000) * this._specialSpsFadingList[i]; // 1 turn every 10 seconds
                    double rotation_sin = Math.Sin(2 * Math.PI * special_sps.curFrame / 10000) * this._specialSpsFadingList[i];
                    Vector3 rotated_pos = BattleSPSSystem.statusTextures[special_sps.refNo].extraPos;
                    float tmp = rotated_pos.x;
                    rotated_pos.x = (float)(rotation_cos * tmp - rotation_sin * rotated_pos.z);
                    rotated_pos.z = (float)(rotation_sin * tmp + rotation_cos * rotated_pos.z);
                    for (int j = 0; j < special_sps.shpGo.Length; j++)
                        special_sps.shpGo[j].transform.localPosition = rotated_pos;
                    special_sps.isUpdate = show_special;
                    special_sps.AnimateSHP();
                    special_sps.lastFrame = special_sps.curFrame;
                    if (this._specialSpsRemovingList[i])
                        this._specialSpsFadingList[i] -= 0.05f;
                    if (this._specialSpsFadingList[i] > 0.0f)
                        special_sps.isUpdate = true;
                    else
                        for (int j = 0; j < special_sps.shpGo.Length; j++)
                            special_sps.shpGo[j].SetActive(false);
                }
				else
				{
                    Vector3 sps_pos = statusTextures[special_sps.refNo].extraPos;
                    Vector3 bone_pos = special_sps.btl.gameObject.transform.GetChildByName("bone" + special_sps.bone.ToString("D3")).position;
                    btl2d.GetIconPosition(special_sps.btl, out Byte[] iconBones, out SByte[] iconOffsY, out SByte[] iconOffsZ);
                    Int16 dy = (Int16)(sps_pos.y * 8);
                    Int16 dz = (Int16)(sps_pos.z * 8);
                    Int32 angledx = ff9.rsin((Int32)(special_sps.btl.rot.eulerAngles.y / 360f * 4096f));
                    Int32 angledz = ff9.rcos((Int32)(special_sps.btl.rot.eulerAngles.y / 360f * 4096f));
                    bone_pos.x += dz * angledx >> 12;
                    bone_pos.y -= dy;
                    bone_pos.z += dz * angledz >> 12;
					special_sps.pos = bone_pos;
                    special_sps.isUpdate = show_special;
                    special_sps.AnimateSHP();
                    special_sps.lastFrame = special_sps.curFrame;
                    if (this._specialSpsRemovingList[i])
                        this._specialSpsFadingList[i] -= 1f;
                    if (this._specialSpsFadingList[i] > 0.0f)
                        special_sps.isUpdate = true;
                    else
                        for (int j = 0; j < special_sps.shpGo.Length; j++)
                            special_sps.shpGo[j].SetActive(false);                
				}
			}
		}
	}

	private Boolean _loadSPSTexture()
	{
		for (Int32 i = 0; i < BattleSPSSystem.statusTextures.Count; i++)
		{
			BattleSPSSystem.SPSTexture spstexture = BattleSPSSystem.statusTextures[i];
			for (Int32 j = 0; j < spstexture.textures.Length; j++)
			{
				String texturePath = spstexture.type == "shp" ? $"{spstexture.name}/{spstexture.name}_{j + 1}" : spstexture.name;
				spstexture.textures[j] = AssetManager.Load<Texture2D>("EmbeddedAsset/BattleMap/Status/" + texturePath, true);
				if (spstexture.textures[j] == null)
					spstexture.textures[j] = new Texture2D(0, 0);
			}
		}
		return true;
	}

	private Int32 _GetSpsFrameCount(Byte[] spsBin)
	{
		return (Int32)(BitConverter.ToUInt16(spsBin, 0) & 32767) << 4;
	}

	private Boolean _loadSPSBin(Int32 spsNo)
	{
		if (this._spsBinDict.ContainsKey(spsNo))
			return true;
		String[] spsNames = new String[]
		{
			"st_doku",
			"st_mdoku",
			"st_slow",
			"st_heis",
			"st_nemu",
			"st_heat",
			"st_friz",
			"st_rif",
			"st_moku",
			"st_moum",
			"st_meiwa",
			"st_basak"
		};
		Byte[] bytes = AssetManager.LoadBytes("BattleMap/BattleSPS/" + spsNames[spsNo] + ".sps", true);
		if (bytes == null)
			return false;
		Int32 key = this._GetSpsFrameCount(bytes);
		this._spsBinDict.Add(spsNo, new KeyValuePair<Int32, Byte[]>(key, bytes));
		return true;
	}

	public void FF9FieldSPSSetObjParm(Int32 ObjNo, Int32 ParmType, Int32 Arg0, Int32 Arg1, Int32 Arg2)
	{
		BattleSPS battleSPS = this._spsList[ObjNo];
		if (ParmType == 130)
		{
			if (Arg0 != -1)
			{
				if (this._loadSPSBin(Arg0))
				{
					battleSPS.spsBin = this._spsBinDict[Arg0].Value;
					battleSPS.curFrame = 0;
					battleSPS.frameCount = this._spsBinDict[Arg0].Key;
				}
				battleSPS.refNo = Arg0;
			}
			else
			{
				battleSPS.spsBin = null;
				battleSPS.meshRenderer.enabled = false;
			}
		}
		else if (ParmType == 131)
		{
			if (Arg1 != 0)
			{
				BattleSPS battleSPS2 = battleSPS;
				battleSPS2.attr = (Byte)(battleSPS2.attr | (Byte)Arg0);
			}
			else
			{
				BattleSPS battleSPS3 = battleSPS;
				battleSPS3.attr = (Byte)(battleSPS3.attr & (Byte)(~(Byte)Arg0));
			}
			if ((battleSPS.attr & 1) == 0)
			{
				battleSPS.meshRenderer.enabled = false;
			}
			else
			{
				battleSPS.meshRenderer.enabled = true;
			}
		}
		else if (ParmType == 135)
		{
			battleSPS.pos = new Vector3((Single)Arg0, (Single)(Arg1 * -1), (Single)Arg2);
		}
		else if (ParmType == 140)
		{
			battleSPS.rot = new Vector3((Single)Arg0 / 4096f * 360f, (Single)Arg1 / 4096f * 360f, (Single)Arg2 / 4096f * 360f);
		}
		else if (ParmType == 145)
		{
			battleSPS.scale = Arg0;
		}
		else if (ParmType == 150)
		{
			Obj objUID = PersistenSingleton<EventEngine>.Instance.GetObjUID(Arg0);
			battleSPS.charNo = Arg0;
			battleSPS.boneNo = Arg1;
			battleSPS.charTran = objUID.go.transform;
			battleSPS.boneTran = objUID.go.transform.GetChildByName("bone" + battleSPS.boneNo.ToString("D3"));
		}
		else if (ParmType == 155)
		{
			battleSPS.fade = (Byte)Arg0;
		}
		else if (ParmType == 156)
		{
			battleSPS.arate = (Byte)Arg0;
		}
		else if (ParmType == 160)
		{
			battleSPS.frameRate = Arg0;
		}
		else if (ParmType == 161)
		{
			battleSPS.curFrame = Arg0 << 4;
		}
		else if (ParmType == 165)
		{
			battleSPS.posOffset = new Vector3((Single)Arg0, (Single)(-(Single)Arg1), (Single)Arg2);
		}
		else if (ParmType == 170)
		{
			battleSPS.depthOffset = Arg0;
		}
	}

	public void SetBtlStatus(Int32 ObjNo, Int32 StatusNo, Byte abr = 0, Int32 type = 0)
	{
		BattleSPS battleSPS = this._spsList[ObjNo];
        if (StatusNo != -1)
		{
			battleSPS.type = type;
			if (type == 0)
			{
				if (this._loadSPSBin(StatusNo))
				{
					battleSPS.spsBin = this._spsBinDict[StatusNo].Value;
					battleSPS.curFrame = 0;
					battleSPS.frameCount = this._spsBinDict[StatusNo].Key;
					battleSPS.arate = abr;
					BattleSPS battleSPS2 = battleSPS;
					battleSPS2.attr = (Byte)(battleSPS2.attr & 254);
					if ((battleSPS.attr & 1) == 0)
					{
						battleSPS.meshRenderer.enabled = false;
					}
					battleSPS.refNo = StatusNo;
					battleSPS.spsScale = BattleSPSSystem.statusTextures[battleSPS.refNo].spsScale;
					battleSPS.spsDistance = BattleSPSSystem.statusTextures[battleSPS.refNo].spsDistance;
				}
			}
			else
			{
				battleSPS.refNo = StatusNo;
				if (battleSPS.shpGo == null)
				{
					battleSPS.GenerateSHP();
				}
			}
		}
		else
		{
			battleSPS.spsBin = null;
			battleSPS.meshRenderer.enabled = false;
			if (battleSPS.type == 1)
			{
				battleSPS.type = 0;
				for (Int32 i = 0; i < (Int32)battleSPS.shpGo.Length; i++)
				{
					battleSPS.shpGo[i].SetActive(false);
				}
			}
		}
	}

	public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{
		Quaternion result = default(Quaternion);
		result.w = Mathf.Sqrt(Mathf.Max(0f, 1f + m[0, 0] + m[1, 1] + m[2, 2])) / 2f;
		result.x = Mathf.Sqrt(Mathf.Max(0f, 1f + m[0, 0] - m[1, 1] - m[2, 2])) / 2f;
		result.y = Mathf.Sqrt(Mathf.Max(0f, 1f - m[0, 0] + m[1, 1] - m[2, 2])) / 2f;
		result.z = Mathf.Sqrt(Mathf.Max(0f, 1f - m[0, 0] - m[1, 1] + m[2, 2])) / 2f;
		result.x *= Mathf.Sign(result.x * (m[2, 1] - m[1, 2]));
		result.y *= Mathf.Sign(result.y * (m[0, 2] - m[2, 0]));
		result.z *= Mathf.Sign(result.z * (m[1, 0] - m[0, 1]));
		return result;
	}

	public void UpdateBtlStatus(BTL_DATA btl, BattleStatus status, Vector3 pos, Vector3 rot, Int32 frame)
	{
		Int32 objSpsIndex = this.GetObjSpsIndex(btl, status);
		BattleSPS battleSPS = this._spsList[objSpsIndex];
		battleSPS.pos = new Vector3(pos.x, pos.y, pos.z);
		battleSPS.curFrame = frame << 4;
		if ((battleSPS.attr & 1) == 0)
		{
			BattleSPS battleSPS2 = battleSPS;
			battleSPS2.attr = (Byte)(battleSPS2.attr | 1);
		}
		battleSPS.isUpdate = true;
	}

    public Int32 GetStatusSPSIndex(BattleStatus status)
	{
		Int32 result = 0;
		for (Int32 i = 0; i < (Int32)btl2d.wStatIconTbl.Length; i++)
		{
			if (btl2d.wStatIconTbl[i].Mask == status)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public Int32 GetObjSpsIndex(BTL_DATA btl, BattleStatus status)
	{
		Int32 statusSPSIndex = this.GetStatusSPSIndex(status);
		return (Int32)(btl.bi.line_no * 12) + statusSPSIndex;
	}

	public void AddBtlSPSObj(BTL_DATA btl, BattleStatus status)
	{
		Int32 statusSPSIndex = this.GetStatusSPSIndex(status);
		Int32 objSpsIndex = this.GetObjSpsIndex(btl, status);
		btl2d.STAT_ICON_TBL stat_ICON_TBL = btl2d.wStatIconTbl[statusSPSIndex];
		this.SetBtlStatus(objSpsIndex, statusSPSIndex, stat_ICON_TBL.Abr, (Int32)stat_ICON_TBL.Type);
	}

	public void RemoveBtlSPSObj(BTL_DATA btl, BattleStatus status)
	{
		Int32 statusSPSIndex = this.GetStatusSPSIndex(status);
		Int32 objSpsIndex = this.GetObjSpsIndex(btl, status);
		btl2d.STAT_ICON_TBL stat_ICON_TBL = btl2d.wStatIconTbl[statusSPSIndex];
		this.SetBtlStatus(objSpsIndex, -1, stat_ICON_TBL.Abr, (Int32)stat_ICON_TBL.Type);
	}

    public void ResetBtlSPSObj(BTL_DATA btl, BattleStatus status)
    {
        btl2d.STAT_ICON_TBL stat_ICON_TBL = btl2d.wStatIconTbl[GetStatusSPSIndex(status)];
        SetBtlStatus(GetObjSpsIndex(btl, status), -1, stat_ICON_TBL.Abr, (Int32)stat_ICON_TBL.Type);
        SetBtlStatus(GetObjSpsIndex(btl, status), GetStatusSPSIndex(status), stat_ICON_TBL.Abr, (Int32)stat_ICON_TBL.Type);
    }

    public void SetActiveSHP(Boolean active)
	{
		for (Int32 i = 0; i < this._spsList.Count; i++)
		{
			BattleSPS battleSPS = this._spsList[i];
			if (battleSPS.shpGo == null)
			{
				return;
			}
			for (Int32 j = 0; j < (Int32)battleSPS.shpGo.Length; j++)
			{
				battleSPS.shpGo[j].SetActive(active);
			}
		}
	}

	public void AddSpecialSPSObj(int specialid, uint spstype, BTL_DATA btl, int bone, float scale, out int SPSid, Boolean rotate = false)
	{
		BattleSPS special_sps;
		if (specialid < 0 || specialid > _specialSpsList.Count)
			specialid = _specialSpsList.Count;
		if (specialid == _specialSpsList.Count)
		{
			GameObject gameObject = new GameObject("SpecialSPS_" + specialid.ToString("D4"));
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			special_sps = gameObject.AddComponent<BattleSPS>();
			special_sps.Init();
			special_sps.spsIndex = specialid;
			special_sps.spsTransform = gameObject.transform;
			special_sps.meshRenderer = meshRenderer;
			special_sps.meshFilter = meshFilter;
            this._specialSpsList.Add(special_sps);
			this._specialSpsFadingList.Add(1.0f);
			this._specialSpsRemovingList.Add(false);
		}
		else
		{
			special_sps = this._specialSpsList[specialid];
			this._specialSpsFadingList[specialid] = 1.0f;
			this._specialSpsRemovingList[specialid] = false;
		}
        if (bone < 0 || bone > FF9StateSystem.Battle.FF9Battle.enemy[btl.bi.slot_no].et.icon_bone[5])
		{
            btl2d.GetIconPosition(btl, out Byte[] iconBones, out _, out _);
            bone = iconBones[3];
        }
		SPSid = specialid;
        special_sps.pos = btl.gameObject.transform.GetChildByName("bone" + bone.ToString("D3")).position;
        special_sps.curFrame = 0;
		special_sps.lastFrame = 0;
		special_sps.frameCount = 10000;
        special_sps.rotate = rotate;
        special_sps.btl = btl;
        special_sps.bone = bone;
        special_sps.attr |= 1;
		special_sps.isUpdate = true;
		special_sps.refNo = (int)spstype;
		special_sps.type = (BattleSPSSystem.statusTextures[(int)spstype].type.Equals("shp") ? 1 : 0);
		special_sps.scale = (int)(scale * 4096);
		if (special_sps.shpGo == null)
			special_sps.GenerateSHP();
	}

	public void RemoveSpecialSPSObj(int specialid)
	{
		if (specialid < 0 || specialid >= _specialSpsList.Count)
			return;
		this._specialSpsRemovingList[specialid] = true;
	}

	public String MapName;

	private Boolean _isReady;

	private List<BattleSPS> _spsList;

	// Custom fields: special SPS effects
	private List<BattleSPS> _specialSpsList;
	private List<float> _specialSpsFadingList;
	private List<bool> _specialSpsRemovingList;

	private Dictionary<Int32, KeyValuePair<Int32, Byte[]>> _spsBinDict;

	public Vector3 rot; 

    public static List<BattleSPSSystem.SPSTexture> statusTextures = new List<BattleSPSSystem.SPSTexture>()
    {
		new BattleSPSSystem.SPSTexture("poison", "sps", 1, Vector3.zero, 6f, 4f),
		new BattleSPSSystem.SPSTexture("venom", "sps", 1, Vector3.zero, 2f, 1.5f),
		new BattleSPSSystem.SPSTexture("slow", "shp", 6, new Vector3(212f, 0f, 0f), 4f, 5f),
		new BattleSPSSystem.SPSTexture("haste", "shp", 6, new Vector3(-148f, 0f, 0f), 4f, 5f),
		new BattleSPSSystem.SPSTexture("sleep", "sps", 1, Vector3.zero, 2.5f, 4.5f),
		new BattleSPSSystem.SPSTexture("heat", "sps", 1, Vector3.zero, 4f, 5f),
		new BattleSPSSystem.SPSTexture("freeze", "sps", 1, Vector3.zero, 4f, 5f),
		new BattleSPSSystem.SPSTexture("reflect", "sps", 1, Vector3.zero, 3f, 3f),
		new BattleSPSSystem.SPSTexture("silence", "shp", 3, new Vector3(-92f, 0f, 0f), 4f, 5f),
		new BattleSPSSystem.SPSTexture("blind", "sps", 1, Vector3.zero, 5f, 5.5f),
		new BattleSPSSystem.SPSTexture("trouble", "shp", 4, new Vector3(92f, 0f, 0f), 4f, 5f),
		new BattleSPSSystem.SPSTexture("berserk", "sps", 1, Vector3.zero, 3f, 2f),
		new BattleSPSSystem.SPSTexture("customfireorb", "shp", 3, new Vector3(400f, 0f, 0f), 5f, 5f),
		new BattleSPSSystem.SPSTexture("customthunderorb", "shp", 4, new Vector3(-200f, 0f, -346.41f), 5f, 5f),
		new BattleSPSSystem.SPSTexture("customiceorb", "shp", 4, new Vector3(-200f, 0f, 346.41f), 5f, 5f)
    };

	public class SPSTexture
	{
		public SPSTexture(String name, String type, Int32 textureNum, Vector3 extraPos, Single scale = 4f, Single distance = 5f)
		{
			this.name = name;
			this.type = type;
			this.textures = new Texture2D[textureNum];
			this.extraPos = extraPos;
			this.spsScale = scale;
			this.spsDistance = distance;
		}

		public String name;

		public String type;

		public Texture2D[] textures;

		public Vector3 extraPos;

		public Single spsScale;

		public Single spsDistance;
	}
}
