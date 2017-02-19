using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = System.Object;

public class FieldSPSSystem : HonoBehavior
{
	public override void HonoUpdate()
	{
		this.Service();
	}

	private void LateUpdate()
	{
		if (!PersistenSingleton<UIManager>.Instance.IsPause)
		{
			this.GenerateSPS();
		}
	}

	public void Init(FieldMap fieldMap)
	{
		this.rot = new Vector3(0f, 0f, 0f);
		this._isReady = false;
		this._spsList = new List<FieldSPS>();
		this._spsBinDict = new Dictionary<Int32, KeyValuePair<Int32, Byte[]>>();
		this._fieldMap = fieldMap;
		for (Int32 i = 0; i < 16; i++)
		{
			GameObject gameObject = new GameObject("SPS_" + i.ToString("D4"));
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			FieldSPS fieldSPS = gameObject.AddComponent<FieldSPS>();
			fieldSPS.Init();
			fieldSPS.fieldMap = fieldMap;
			fieldSPS.spsIndex = i;
			fieldSPS.spsTransform = gameObject.transform;
			fieldSPS.meshRenderer = meshRenderer;
			fieldSPS.meshFilter = meshFilter;
			this._spsList.Add(fieldSPS);
			FieldSPSActor fieldSPSActor = gameObject.AddComponent<FieldSPSActor>();
			fieldSPSActor.sps = fieldSPS;
			fieldSPS.spsActor = fieldSPSActor;
		}
		this.MapName = FF9StateSystem.Field.SceneName;
		FieldMapInfo.fieldmapSPSExtraOffset.SetSPSOffset(this.MapName, this._spsList);
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
			FieldSPS fieldSPS = this._spsList[i];
			if (fieldSPS.spsBin != null && (fieldSPS.attr & 1) != 0)
			{
				if (fieldSPS.lastFrame != -1)
				{
					fieldSPS.lastFrame = fieldSPS.curFrame;
					fieldSPS.curFrame += fieldSPS.frameRate;
					if (fieldSPS.curFrame >= fieldSPS.frameCount)
					{
						fieldSPS.curFrame = 0;
					}
					else if (fieldSPS.curFrame < 0)
					{
						fieldSPS.curFrame = (fieldSPS.frameCount >> 4) - 1 << 4;
					}
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
			FieldSPS fieldSPS = this._spsList[i];
			if (fieldSPS.spsBin != null && (fieldSPS.attr & 1) != 0)
			{
				if (fieldSPS.charTran != (UnityEngine.Object)null && fieldSPS.boneTran != (UnityEngine.Object)null)
				{
					FieldMapActor component = fieldSPS.charTran.GetComponent<FieldMapActor>();
					if (component != (UnityEngine.Object)null)
					{
						component.UpdateGeoAttach();
					}
					fieldSPS.pos = fieldSPS.boneTran.position + fieldSPS.posOffset;
				}
				fieldSPS.GenerateSPS();
				fieldSPS.lastFrame = fieldSPS.curFrame;
				fieldSPS.meshRenderer.enabled = true;
			}
		}
	}

	private Boolean _loadSPSTexture()
	{
		TextAsset textAsset = AssetManager.Load<TextAsset>("FieldMaps/" + this.MapName + "/spt.tcb", false);
		if (textAsset != (UnityEngine.Object)null)
		{
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(textAsset.bytes)))
			{
				UInt32 num = binaryReader.ReadUInt32();
				UInt32 num2 = binaryReader.ReadUInt32();
				Int32 num3 = binaryReader.ReadInt32();
				binaryReader.BaseStream.Seek((Int64)((UInt64)num), SeekOrigin.Begin);
				UInt32 num4 = binaryReader.ReadUInt32();
				Int32 num5 = binaryReader.ReadInt32();
				for (Int32 i = 0; i < num3; i++)
				{
					binaryReader.BaseStream.Seek((Int64)((UInt64)num2), SeekOrigin.Begin);
					Int32 x = (Int32)binaryReader.ReadInt16();
					Int32 y = (Int32)binaryReader.ReadInt16();
					Int32 num6 = (Int32)binaryReader.ReadInt16();
					Int32 num7 = (Int32)binaryReader.ReadInt16();
					PSXTextureMgr.LoadImageBin(x, y, num6, num7, binaryReader);
					UInt32 num8 = (UInt32)(num6 * num7 * 2);
					num2 += num8 + 8u;
				}
				num += 8u;
				for (Int32 j = 0; j < num5; j++)
				{
					binaryReader.BaseStream.Seek((Int64)((UInt64)num), SeekOrigin.Begin);
					Int32 x2 = (Int32)binaryReader.ReadInt16();
					Int32 y2 = (Int32)binaryReader.ReadInt16();
					Int32 num9 = (Int32)binaryReader.ReadInt16();
					Int32 num10 = (Int32)binaryReader.ReadInt16();
					binaryReader.BaseStream.Seek((Int64)((UInt64)num4), SeekOrigin.Begin);
					PSXTextureMgr.LoadImageBin(x2, y2, num9, num10, binaryReader);
					UInt32 num8 = (UInt32)(num9 * num10 * 2);
					num4 += num8;
					num += 8u;
				}
			}
			PSXTextureMgr.ClearObject();
			return true;
		}
		return false;
	}

	private Int32 _GetSpsFrameCount(Byte[] spsBin)
	{
		return (Int32)(BitConverter.ToUInt16(spsBin, 0) & 32767) << 4;
	}

	private Boolean _loadSPSBin(Int32 spsNo)
	{
		if (this._spsBinDict.ContainsKey(spsNo))
		{
			return true;
		}
		TextAsset textAsset = AssetManager.Load<TextAsset>(String.Concat(new Object[]
		{
			"FieldMaps/",
			this.MapName,
			"/",
			spsNo,
			".sps"
		}), false);
		if (textAsset == (UnityEngine.Object)null)
		{
			return false;
		}
		Byte[] bytes = textAsset.bytes;
		Int32 key = this._GetSpsFrameCount(bytes);
		this._spsBinDict.Add(spsNo, new KeyValuePair<Int32, Byte[]>(key, bytes));
		return true;
	}

	public void FF9FieldSPSSetObjParm(Int32 ObjNo, Int32 ParmType, Int32 Arg0, Int32 Arg1, Int32 Arg2)
	{
		FieldSPS fieldSPS = this._spsList[ObjNo];
		if (ParmType == 130)
		{
			if (Arg0 != -1)
			{
				if (this._loadSPSBin(Arg0))
				{
					fieldSPS.spsBin = this._spsBinDict[Arg0].Value;
					fieldSPS.curFrame = 0;
					fieldSPS.lastFrame = -1;
					fieldSPS.frameCount = this._spsBinDict[Arg0].Key;
				}
				fieldSPS.refNo = Arg0;
				if (FF9StateSystem.Common.FF9.fldMapNo == 2553 && (fieldSPS.refNo == 464 || fieldSPS.refNo == 467 || fieldSPS.refNo == 506 || fieldSPS.refNo == 510))
				{
					fieldSPS.spsBin = null;
				}
			}
			else
			{
				if ((FF9StateSystem.Common.FF9.fldMapNo == 911 || FF9StateSystem.Common.FF9.fldMapNo == 1911) && (fieldSPS.refNo == 33 || fieldSPS.refNo == 34))
				{
					fieldSPS.pos = Vector3.zero;
					fieldSPS.scale = 4096;
					fieldSPS.rot = Vector3.zero;
					fieldSPS.rotArg = Vector3.zero;
				}
				fieldSPS.spsBin = null;
				fieldSPS.meshRenderer.enabled = false;
				fieldSPS.charTran = (Transform)null;
				fieldSPS.boneTran = (Transform)null;
			}
		}
		else if (ParmType == 131)
		{
			if (Arg1 != 0)
			{
				FieldSPS fieldSPS2 = fieldSPS;
				fieldSPS2.attr = (Byte)(fieldSPS2.attr | (Byte)Arg0);
			}
			else
			{
				FieldSPS fieldSPS3 = fieldSPS;
				fieldSPS3.attr = (Byte)(fieldSPS3.attr & (Byte)(~(Byte)Arg0));
			}
			if ((fieldSPS.attr & 1) == 0)
			{
				fieldSPS.meshRenderer.enabled = false;
			}
			else if (FF9StateSystem.Common.FF9.fldMapNo == 2928 || FF9StateSystem.Common.FF9.fldMapNo == 1206 || FF9StateSystem.Common.FF9.fldMapNo == 1223)
			{
				if (fieldSPS.spsBin != null)
				{
					fieldSPS.meshRenderer.enabled = true;
				}
			}
			else
			{
				fieldSPS.meshRenderer.enabled = true;
			}
		}
		else if (ParmType == 135)
		{
			if (FF9StateSystem.Common.FF9.fldMapNo == 911 || FF9StateSystem.Common.FF9.fldMapNo == 1911)
			{
				if (fieldSPS.spsBin != null)
				{
					fieldSPS.pos = new Vector3((Single)Arg0, (Single)(Arg1 * -1), (Single)Arg2);
				}
			}
			else
			{
				fieldSPS.pos = new Vector3((Single)Arg0, (Single)(Arg1 * -1), (Single)Arg2);
			}
		}
		else if (ParmType == 140)
		{
			fieldSPS.rot = new Vector3((Single)Arg0 / 4096f * 360f, (Single)Arg1 / 4096f * 360f, (Single)Arg2 / 4096f * 360f);
		}
		else if (ParmType == 145)
		{
			fieldSPS.scale = Arg0;
		}
		else if (ParmType == 150)
		{
			Obj objUID = PersistenSingleton<EventEngine>.Instance.GetObjUID(Arg0);
			fieldSPS.charNo = Arg0;
			fieldSPS.boneNo = Arg1;
			fieldSPS.charTran = objUID.go.transform;
			fieldSPS.boneTran = objUID.go.transform.GetChildByName("bone" + fieldSPS.boneNo.ToString("D3"));
		}
		else if (ParmType == 155)
		{
			fieldSPS.fade = (Byte)Arg0;
		}
		else if (ParmType == 156)
		{
			fieldSPS.arate = (Byte)Arg0;
		}
		else if (ParmType == 160)
		{
			fieldSPS.frameRate = Arg0;
		}
		else if (ParmType == 161)
		{
			fieldSPS.curFrame = Arg0 << 4;
		}
		else if (ParmType == 165)
		{
			fieldSPS.posOffset = new Vector3((Single)Arg0, (Single)(-(Single)Arg1), (Single)Arg2);
		}
		else if (ParmType == 170)
		{
			fieldSPS.depthOffset = Arg0;
		}
	}

	public String MapName;

	private Boolean _isReady;

	private FieldMap _fieldMap;

	private List<FieldSPS> _spsList;

	private Dictionary<Int32, KeyValuePair<Int32, Byte[]>> _spsBinDict;

	public Vector3 rot;
}
