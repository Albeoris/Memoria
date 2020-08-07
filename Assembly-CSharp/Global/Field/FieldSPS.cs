using System;
using System.Collections.Generic;
using System.IO;
using Memoria.Scripts;
using UnityEngine;

public class FieldSPS : MonoBehaviour
{
	public void Init()
	{
		this.attr = 1;
		this.arate = 15;
		this.fade = 128;
		this.refNo = -1;
		this.charNo = -1;
		this.boneNo = 0;
		this.lastFrame = -1;
		this.curFrame = 0;
		this.frameCount = 0;
		this.frameRate = 16;
		this.pos = Vector3.zero;
		this.scale = 4096;
		this.rot = Vector3.zero;
		this.rotArg = Vector3.zero;
		this.zOffset = 0;
		this.posOffset = Vector3.zero;
		this.depthOffset = 0;
		this.spsIndex = -1;
		this.spsTransform = (Transform)null;
		this.meshRenderer = (MeshRenderer)null;
		this.meshFilter = (MeshFilter)null;
		this.spsBin = null;
		this.works = new FieldSPS.FieldSPSWork();
		this.spsPrims = new List<FieldSPS.FieldSPSPrim>();
		this.spsActor = (FieldSPSActor)null;
		this._vertices = new List<Vector3>();
		this._colors = new List<Color>();
		this._uv = new List<Vector2>();
		this._indices = new List<Int32>();
		this.materials = new Material[5];
		this.materials[0] = new Material(ShadersLoader.Find("PSX/FieldSPS_Abr_0"));
		this.materials[1] = new Material(ShadersLoader.Find("PSX/FieldSPS_Abr_1"));
		this.materials[2] = new Material(ShadersLoader.Find("PSX/FieldSPS_Abr_2"));
		this.materials[3] = new Material(ShadersLoader.Find("PSX/FieldSPS_Abr_3"));
		this.materials[4] = new Material(ShadersLoader.Find("PSX/FieldSPS_Abr_None"));
		this.charTran = (Transform)null;
		this.boneTran = (Transform)null;
	}

	public void GenerateSPS()
	{
		Int32 num = this.curFrame >> 4;
		using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(this.spsBin)))
		{
			Int32 num2 = num;
			Int32 num3 = (Int32)(binaryReader.ReadUInt16() & 32767);
			if (num2 >= num3)
			{
				num2 %= num3;
			}
			Int32 num4 = num2 * 2 + 8;
			num3 = num3 * 2 + 8;
			binaryReader.BaseStream.Seek((Int64)num3, SeekOrigin.Begin);
			num2 = (Int32)binaryReader.ReadUInt16();
			this.works.pt = num3 + 2;
			this.works.rgb = this.works.pt + num2 * 2 + 2;
			binaryReader.BaseStream.Seek(2L, SeekOrigin.Begin);
			Int32 num5 = (Int32)((this.arate > 3) ? Byte.MaxValue : this.arate);
			this.works.tpage = (UInt16)((Int32)binaryReader.ReadUInt16() | (num5 & 3) << 5);
			this.works.clut = binaryReader.ReadUInt16();
			this.works.w = (Int32)((binaryReader.ReadByte() - 1) * 2);
			this.works.h = (Int32)((binaryReader.ReadByte() - 1) * 2);
			binaryReader.BaseStream.Seek((Int64)num4, SeekOrigin.Begin);
			num3 = (Int32)binaryReader.ReadUInt16();
			binaryReader.BaseStream.Seek((Int64)num3, SeekOrigin.Begin);
			this.works.primCount = (Int32)binaryReader.ReadByte();
			num3++;
			Int32 num6 = num3;
			binaryReader.BaseStream.Seek((Int64)num6, SeekOrigin.Begin);
			this.works.code = (Byte)(44 | (Int32)((num5 != 255) ? 2 : 0));
			this.works.fade = (Int32)this.fade << 4;
			this._GenerateSPSPrims(binaryReader, num6);
			this._GenerateSPSMesh();
		}
	}

	private void _GenerateSPSPrims(BinaryReader reader, Int32 spsOffset)
	{
		this.spsPrims.Clear();
		FieldSPS.FieldSPSPrim item = default(FieldSPS.FieldSPSPrim);
		reader.BaseStream.Seek((Int64)spsOffset, SeekOrigin.Begin);
		for (Int32 i = 0; i < this.works.primCount; i++)
		{
			item.code = this.works.code;
			item.tpage = this.works.tpage;
			item.clut = this.works.clut;
			Int32 num = (Int32)reader.ReadSByte();
			Int32 num2 = (Int32)reader.ReadSByte();
			num <<= 2;
			Int32 num3 = num - this.works.h;
			Int32 num4 = num + this.works.h;
			num2 <<= 2;
			Int32 num5 = num2 - this.works.w;
			num2 += this.works.w;
			num3 &= 65535;
			num4 &= 65535;
			num5 <<= 16;
			num2 <<= 16;
			Int32toSByteConverter int32toSByteConverter = num3 | num5;
			item.v0 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			int32toSByteConverter = (num4 | num5);
			item.v1 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			int32toSByteConverter = (num3 | num2);
			item.v2 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			int32toSByteConverter = (num4 | num2);
			item.v3 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			num3 = (Int32)reader.ReadByte();
			Int64 position = reader.BaseStream.Position;
			num5 = (num3 & 15);
			num5 <<= 1;
			num5 += this.works.pt;
			reader.BaseStream.Seek((Int64)num5, SeekOrigin.Begin);
			num4 = (Int32)reader.ReadUInt16();
			Int32 num6 = (Int32)((UInt32)this.works.h >> 1);
			num5 = (num4 & 255);
			num4 = (Int32)((UInt32)num4 >> 8);
			num2 = num5 + num6;
			num6 = (Int32)((UInt32)this.works.w >> 1);
			num = num4 + num6;
			num4 <<= 8;
			num <<= 8;
			num6 = (num5 | num4);
			item.uv0 = new Vector2((Single)(num6 & 255), (Single)(num6 >> 8 & 255));
			num6 = (num2 | num4);
			item.uv1 = new Vector2((Single)(num6 & 255), (Single)(num6 >> 8 & 255));
			num6 = (num5 | num);
			item.uv2 = new Vector2((Single)(num6 & 255), (Single)(num6 >> 8 & 255));
			num6 = (num2 | num);
			item.uv3 = new Vector2((Single)(num6 & 255), (Single)(num6 >> 8 & 255));
			num = (Int32)((UInt32)num3 >> 4);
			num <<= 2;
			num += this.works.rgb;
			reader.BaseStream.Seek((Int64)num, SeekOrigin.Begin);
			num3 = (Int32)reader.ReadUInt16();
			num5 = (Int32)reader.ReadSByte();
			num5 <<= 16;
			num3 |= num5;
			var ff9FldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
			if (this.works.fade >= 0)
			{
				UInt32 num7 = (UInt32)this.works.fade;
				num5 = (num3 & 255);
				UInt32 num8 = (UInt32)num5;
				num4 = (Int32)((UInt32)num3 >> 8);
				num4 &= 255;
				UInt32 num9 = (UInt32)num4;
				num5 = (Int32)((UInt32)num3 >> 16);
				num5 &= 255;
				UInt32 num10 = (UInt32)num5;
				num8 = num8 * num7 >> 12;
				num9 = num9 * num7 >> 12;
				num10 = num10 * num7 >> 12;
				num8 = (UInt32)Mathf.Clamp(num8, -32768f, 32767f);
				num9 = (UInt32)Mathf.Clamp(num9, -32768f, 32767f);
				num10 = (UInt32)Mathf.Clamp(num10, -32768f, 32767f);
				if ((ff9FldMapNo == 2901 && (this.refNo == 644 || this.refNo == 736)) || (ff9FldMapNo == 2913 && (this.refNo == 646 || this.refNo == 737)) || (ff9FldMapNo == 2925 && (this.refNo == 990 || this.refNo == 988)))
				{
					item.color = new Color(num8 / 255f, num9 / 255f, num10 / 255f, 1f);
				}
				else
				{
					item.color = new Color(num8 / 127f, num9 / 127f, num10 / 127f, 1f);
				}
			}
			else
			{
				num5 = (num3 & 255);
				UInt32 num8 = (UInt32)num5;
				num4 = (Int32)((UInt32)num3 >> 8);
				num4 &= 255;
				UInt32 num9 = (UInt32)num4;
				num5 = (Int32)((UInt32)num3 >> 16);
				num5 &= 255;
				UInt32 num10 = (UInt32)num5;
				item.color = new Color(num8 / 127f, num9 / 127f, num10 / 127f, 1f);
			}
			item.otz = 0;
			this.spsPrims.Add(item);
			reader.BaseStream.Seek(position, SeekOrigin.Begin);
			
			// TODO Check Native: #147
			if ((ff9FldMapNo == 155 || ff9FldMapNo == 1216 || ff9FldMapNo == 1808) && base.name.Equals("SPS_0008"))
				this.zOffset = 700;
		}
	}

	private void _GenerateSPSMesh()
	{
		if (this.spsPrims.Count == 0)
		{
			return;
		}
		Boolean flag = false;
		BGCAM_DEF currentBgCamera = this.fieldMap.GetCurrentBgCamera();
		if (currentBgCamera == null)
		{
			return;
		}
		Single num = (Single)this.scale / 4096f;
		Matrix4x4 localRTS = Matrix4x4.identity;
		Boolean flag2 = false;
		if (FF9StateSystem.Common.FF9.fldMapNo == 2929)
		{
			flag = true;
		}
		if (flag)
		{
			if (FF9StateSystem.Common.FF9.fldMapNo == 2929)
			{
				localRTS = Matrix4x4.TRS(this.pos * 0.9925f, Quaternion.Euler(-this.rot.x / 2f, -this.rot.y / 2f, this.rot.z / 2f), new Vector3(num, -num, 1f));
			}
			else
			{
				localRTS = Matrix4x4.TRS(this.pos, Quaternion.Euler(this.rot.x, this.rot.y, this.rot.z), new Vector3(num, -num, 1f));
			}
		}
		else
		{
			Vector3 vector = PSX.CalculateGTE_RTPT_POS(this.pos, Matrix4x4.identity, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset(), true);
			num *= currentBgCamera.GetViewDistance() / vector.z;
			if (vector.z < 0f)
			{
				flag2 = true;
			}
			vector.z /= 4f;
			vector.z += (Single)currentBgCamera.depthOffset;
			base.transform.localPosition = new Vector3(vector.x, vector.y, vector.z + (Single)this.zOffset);
			base.transform.localScale = new Vector3(num, -num, 1f);
			base.transform.localRotation = Quaternion.Euler(this.rot.x, this.rot.y, -this.rot.z);
		}
		this._vertices.Clear();
		this._colors.Clear();
		this._uv.Clear();
		this._indices.Clear();
		for (Int32 i = 0; i < this.spsPrims.Count; i++)
		{
			if (flag2)
			{
				break;
			}
			FieldSPS.FieldSPSPrim fieldSPSPrim = this.spsPrims[i];
			Int32 count = this._vertices.Count;
			if (flag)
			{
				Vector3 a = PSX.CalculateGTE_RTPT(fieldSPSPrim.v0, localRTS, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
				Vector3 a2 = PSX.CalculateGTE_RTPT(fieldSPSPrim.v1, localRTS, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
				Vector3 a3 = PSX.CalculateGTE_RTPT(fieldSPSPrim.v2, localRTS, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
				Vector3 a4 = PSX.CalculateGTE_RTPT(fieldSPSPrim.v3, localRTS, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
				Single num2 = PSX.CalculateGTE_RTPTZ(Vector3.zero, localRTS, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
				num2 /= 4f;
				num2 += (Single)currentBgCamera.depthOffset;
				Vector3 b = new Vector3(0f, 0f, num2 - (Single)i / 100f);
				this._vertices.Add(a + b);
				this._vertices.Add(a2 + b);
				this._vertices.Add(a3 + b);
				this._vertices.Add(a4 + b);
			}
			else
			{
				Vector3 b2 = new Vector3(0f, 0f, (Single)(this.spsPrims.Count - i) / 100f);
				this._vertices.Add(fieldSPSPrim.v0 + b2);
				this._vertices.Add(fieldSPSPrim.v1 + b2);
				this._vertices.Add(fieldSPSPrim.v2 + b2);
				this._vertices.Add(fieldSPSPrim.v3 + b2);
			}
			this._colors.Add(fieldSPSPrim.color);
			this._colors.Add(fieldSPSPrim.color);
			this._colors.Add(fieldSPSPrim.color);
			this._colors.Add(fieldSPSPrim.color);
			this._uv.Add((fieldSPSPrim.uv0 + new Vector2(0.5f, 0.5f)) * 0.00390625f);
			this._uv.Add((fieldSPSPrim.uv1 + new Vector2(-0.5f, 0.5f)) * 0.00390625f);
			this._uv.Add((fieldSPSPrim.uv2 + new Vector2(0.5f, -0.5f)) * 0.00390625f);
			this._uv.Add((fieldSPSPrim.uv3 + new Vector2(-0.5f, -0.5f)) * 0.00390625f);
			this._indices.Add(count);
			this._indices.Add(count + 1);
			this._indices.Add(count + 2);
			this._indices.Add(count + 1);
			this._indices.Add(count + 3);
			this._indices.Add(count + 2);
		}
		Mesh mesh = this.meshFilter.mesh;
		mesh.Clear();
		mesh.vertices = this._vertices.ToArray();
		mesh.colors = this._colors.ToArray();
		mesh.uv = this._uv.ToArray();
		mesh.triangles = this._indices.ToArray();
		this.meshFilter.mesh = mesh;
		FieldSPS.FieldSPSPrim fieldSPSPrim2 = this.spsPrims[0];
		PSXTexture texture = PSXTextureMgr.GetTexture(fieldSPSPrim2.FlagTP, fieldSPSPrim2.FlagTY, fieldSPSPrim2.FlagTX, fieldSPSPrim2.FlagClutY, fieldSPSPrim2.FlagClutX);
		texture.SetFilter(FilterMode.Bilinear);
		Int32 num3 = (Int32)((this.arate >= 4) ? 4 : this.arate);
		this.materials[num3].mainTexture = texture.texture;
		this.meshRenderer.material = this.materials[num3];
		if (this.spsActor != (UnityEngine.Object)null)
		{
			this.spsActor.spsPos = this.pos;
		}
	}

	public FieldMap fieldMap;

	public Int32 spsIndex;

	public Transform spsTransform;

	public MeshRenderer meshRenderer;

	public MeshFilter meshFilter;

	public Byte[] spsBin;

	public FieldSPS.FieldSPSWork works;

	public List<FieldSPS.FieldSPSPrim> spsPrims;

	public FieldSPSActor spsActor;

	public Material[] materials;

	public Transform charTran;

	public Transform boneTran;

	public Byte attr;

	public Byte arate;

	public Byte fade;

	public Int32 refNo;

	public Int32 charNo;

	public Int32 boneNo;

	public Int32 lastFrame;

	public Int32 curFrame;

	public Int32 frameCount;

	public Int32 frameRate;

	public Vector3 pos;

	public Int32 scale;

	public Vector3 rot;

	public Int32 zOffset;

	public Vector3 rotArg;

	public Vector3 posOffset;

	public Int32 depthOffset;

	private List<Vector3> _vertices;

	private List<Color> _colors;

	private List<Vector2> _uv;

	private List<Int32> _indices;

	public class FieldSPSWork
	{
		public Int32 pt;

		public Int32 rgb;

		public Int32 w;

		public Int32 h;

		public UInt16 tpage;

		public UInt16 clut;

		public Int32 fade;

		public Int32 primCount;

		public Byte code;
	}

	public struct FieldSPSPrim
	{
		public Int32 FlagTP
		{
			get
			{
				return this.tpage >> 7 & 3;
			}
		}

		public Int32 FlagABR
		{
			get
			{
				return this.tpage >> 5 & 3;
			}
		}

		public Int32 FlagTY
		{
			get
			{
				return this.tpage >> 4 & 1;
			}
		}

		public Int32 FlagTX
		{
			get
			{
				return (Int32)(this.tpage & 15);
			}
		}

		public Int32 FlagClutY
		{
			get
			{
				return this.clut >> 6 & 511;
			}
		}

		public Int32 FlagClutX
		{
			get
			{
				return (Int32)(this.clut & 63);
			}
		}

		public Boolean FlagSemitrans
		{
			get
			{
				return (this.code & 2) != 0;
			}
		}

		public Boolean FlagShadeTex
		{
			get
			{
				return (this.code & 1) != 0;
			}
		}

		public Byte code;

		public UInt16 tpage;

		public UInt16 clut;

		public Int32 otz;

		public Color color;

		public Vector3 v0;

		public Vector2 uv0;

		public Vector3 v1;

		public Vector2 uv1;

		public Vector3 v2;

		public Vector2 uv2;

		public Vector3 v3;

		public Vector2 uv3;
	}
}
