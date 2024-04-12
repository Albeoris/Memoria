using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FieldSPSSystem : HonoBehavior
{
	public override void HonoUpdate()
	{
		this.Service();
	}

	private void LateUpdate()
	{
		if (!PersistenSingleton<UIManager>.Instance.IsPause)
			this.GenerateSPS();
	}

	public void Init(FieldMap fieldMap)
	{
		this.rot = new Vector3(0f, 0f, 0f);
		this._isReady = false;
		this._spsList = new List<FieldSPS>();
		this._spsBinDict = new Dictionary<Int32, KeyValuePair<Int32, Byte[]>>();
		this._fieldMap = fieldMap;
		for (Int32 i = 0; i < FieldSPSConst.FF9FIELDSPS_MAX_OBJCOUNT; i++)
		{
			GameObject gameObject = new GameObject($"SPS_{i:D4}");
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

	public void ChangeFieldOrigin(Int32 fieldId)
	{
		foreach (FieldSPS fieldSPS in this._spsList)
		{
			fieldSPS.spsBin = null;
			fieldSPS.meshRenderer.enabled = false;
			fieldSPS.charTran = null;
			fieldSPS.boneTran = null;
		}
		this._spsBinDict.Clear();
		this.MapName = EventEngineUtils.eventIDToFBGID[fieldId];
		FieldMapInfo.fieldmapSPSExtraOffset.SetSPSOffset(this.MapName, this._spsList);
		this._isReady = this._loadSPSTexture();
	}

	public void Service()
	{
		if (!this._isReady)
			return;
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
						fieldSPS.curFrame = 0;
					else if (fieldSPS.curFrame < 0)
						fieldSPS.curFrame = (fieldSPS.frameCount >> 4) - 1 << 4;
				}
			}
		}
	}

	public void GenerateSPS()
	{
		if (!this._isReady)
			return;
		for (Int32 i = 0; i < this._spsList.Count; i++)
		{
			FieldSPS fieldSPS = this._spsList[i];
			if (fieldSPS.spsBin != null && (fieldSPS.attr & 1) != 0)
			{
				if (fieldSPS.charTran != null && fieldSPS.boneTran != null)
				{
					FieldMapActor component = fieldSPS.charTran.GetComponent<FieldMapActor>();
					if (component != null)
						component.UpdateGeoAttach();
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
		Byte[] binAsset = AssetManager.LoadBytes("FieldMaps/" + this.MapName + "/spt.tcb");
		if (binAsset != null)
		{
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(binAsset)))
			{
				UInt32 secondBatchOffset = binaryReader.ReadUInt32();
				UInt32 firstBatchOffset = binaryReader.ReadUInt32();
				Int32 firstBatchCount = binaryReader.ReadInt32();
				for (Int32 i = 0; i < firstBatchCount; i++)
				{
					binaryReader.BaseStream.Seek(firstBatchOffset, SeekOrigin.Begin);
					Int32 x = binaryReader.ReadInt16();
					Int32 y = binaryReader.ReadInt16();
					Int32 w = binaryReader.ReadInt16();
					Int32 h = binaryReader.ReadInt16();
					PSXTextureMgr.LoadImageBin(x, y, w, h, binaryReader);
					firstBatchOffset += (UInt32)(w * h * 2 + 8);
				}
				binaryReader.BaseStream.Seek(secondBatchOffset, SeekOrigin.Begin);
				UInt32 secondBatchImgOffset = binaryReader.ReadUInt32();
				Int32 secondBatchCount = binaryReader.ReadInt32();
				secondBatchOffset += 8u;
				for (Int32 i = 0; i < secondBatchCount; i++)
				{
					binaryReader.BaseStream.Seek(secondBatchOffset, SeekOrigin.Begin);
					Int32 x = binaryReader.ReadInt16();
					Int32 y = binaryReader.ReadInt16();
					Int32 w = binaryReader.ReadInt16();
					Int32 h = binaryReader.ReadInt16();
					binaryReader.BaseStream.Seek(secondBatchImgOffset, SeekOrigin.Begin);
					PSXTextureMgr.LoadImageBin(x, y, w, h, binaryReader);
					secondBatchImgOffset += (UInt32)(w * h * 2);
					secondBatchOffset += 8u;
				}
			}
			PSXTextureMgr.ClearObject();
			return true;
		}
		return false;
	}

	private Int32 _GetSpsFrameCount(Byte[] spsBin)
	{
		return (BitConverter.ToUInt16(spsBin, 0) & 0x7FFF) << 4;
	}

	private Boolean _loadSPSBin(Int32 spsNo)
	{
		if (this._spsBinDict.ContainsKey(spsNo))
			return true;
		Byte[] binAsset = AssetManager.LoadBytes($"FieldMaps/{this.MapName}/{spsNo}.sps");
		if (binAsset == null)
			return false;
		this._spsBinDict.Add(spsNo, new KeyValuePair<Int32, Byte[]>(this._GetSpsFrameCount(binAsset), binAsset));
		return true;
	}

	public void FF9FieldSPSSetObjParm(Int32 ObjNo, Int32 ParmType, Int32 Arg0, Int32 Arg1, Int32 Arg2)
	{
		if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_FIELD)
		{
			// TODO: Somehow find a way to have SPS from multiple fields simultaneously, which requires something better than PSXTextureMgr
			this.ChangeFieldOrigin(Arg0);
			return;
		}
		FieldSPS fieldSPS = this._spsList[ObjNo];
		if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_REF)
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
					// Wind Shrine/Interior
					fieldSPS.spsBin = null;
				}
			}
			else
			{
				if ((FF9StateSystem.Common.FF9.fldMapNo == 911 || FF9StateSystem.Common.FF9.fldMapNo == 1911) && (fieldSPS.refNo == 33 || fieldSPS.refNo == 34))
				{
					// Treno/Queen's House
					fieldSPS.pos = Vector3.zero;
					fieldSPS.scale = 4096;
					fieldSPS.rot = Vector3.zero;
					fieldSPS.rotArg = Vector3.zero;
				}
				fieldSPS.spsBin = null;
				fieldSPS.meshRenderer.enabled = false;
				fieldSPS.charTran = null;
				fieldSPS.boneTran = null;
			}
		}
		else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_ATTR)
		{
			if (Arg1 == 0)
				fieldSPS.attr &= (Byte)~Arg0;
			else
				fieldSPS.attr |= (Byte)Arg0;

			if ((fieldSPS.attr & 1) == 0)
			{
				fieldSPS.meshRenderer.enabled = false;
			}
			else if (FF9StateSystem.Common.FF9.fldMapNo == 2928 || FF9StateSystem.Common.FF9.fldMapNo == 1206 || FF9StateSystem.Common.FF9.fldMapNo == 1223)
			{
				// Hill of Despair
				// A. Castle/Queen's Chamber
				if (fieldSPS.spsBin != null)
					fieldSPS.meshRenderer.enabled = true;
			}
			else
			{
				fieldSPS.meshRenderer.enabled = true;
			}
		}
		else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_POS)
		{
			if (FF9StateSystem.Common.FF9.fldMapNo == 911 || FF9StateSystem.Common.FF9.fldMapNo == 1911)
			{
				// Treno/Queen's House
				if (fieldSPS.spsBin != null)
					fieldSPS.pos = new Vector3(Arg0, -Arg1, Arg2);
			}
			else
			{
				fieldSPS.pos = new Vector3(Arg0, -Arg1, Arg2);
			}
		}
		else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_ROT)
		{
			fieldSPS.rot = new Vector3(Arg0 / 4096f * 360f, Arg1 / 4096f * 360f, Arg2 / 4096f * 360f);
		}
		else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_SCALE)
		{
			fieldSPS.scale = Arg0;
		}
		else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_CHAR)
		{
			Obj objUID = PersistenSingleton<EventEngine>.Instance.GetObjUID(Arg0);
			fieldSPS.charNo = Arg0;
			fieldSPS.boneNo = Arg1;
			fieldSPS.charTran = objUID.go.transform;
			fieldSPS.boneTran = objUID.go.transform.GetChildByName("bone" + fieldSPS.boneNo.ToString("D3"));
		}
		else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_FADE)
		{
			fieldSPS.fade = (Byte)Arg0;
		}
		else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_ARATE)
		{
			fieldSPS.arate = (Byte)Arg0;
		}
		else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_FRAMERATE)
		{
			fieldSPS.frameRate = Arg0;
		}
		else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_FRAME)
		{
			fieldSPS.curFrame = Arg0 << 4;
		}
		else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_POSOFFSET)
		{
			fieldSPS.posOffset = new Vector3(Arg0, -Arg1, Arg2);
		}
		else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_DEPTHOFFSET)
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
