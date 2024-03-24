using System;
using System.Collections.Generic;
using System.IO;
using Memoria.Scripts;
using UnityEngine;

public class BattleSPS : MonoBehaviour
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
		this.posOffset = Vector3.zero;
		this.depthOffset = 0;
		this.spsIndex = -1;
		this.spsTransform = (Transform)null;
		this.meshRenderer = (MeshRenderer)null;
		this.meshFilter = (MeshFilter)null;
		this.spsBin = null;
		this.works = new BattleSPS.FieldSPSWork();
		this.spsPrims = new List<BattleSPS.FieldSPSPrim>();
		this.spsActor = (FieldSPSActor)null;
		this._vertices = new List<Vector3>();
		this._colors = new List<Color>();
		this._uv = new List<Vector2>();
		this._indices = new List<Int32>();
		this.materials = new Material[5];
		this.materials[0] = ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_0");
		this.materials[1] = ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_1");
		this.materials[2] = ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_2");
		this.materials[3] = ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_3");
		this.materials[4] = ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_None");
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
		BattleSPS.FieldSPSPrim item = default(BattleSPS.FieldSPSPrim);
		reader.BaseStream.Seek((Int64)spsOffset, SeekOrigin.Begin);
		Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
		Vector3 a = camera.worldToCameraMatrix.inverse.GetColumn(3);
		Single num = Vector3.Distance(a, base.transform.localPosition);
		Single num2 = 1f / (num * this.spsDistance * BattleSPS.spsK);
		if (num2 < 0.33f)
		{
			num2 = 0.33f;
		}
		for (Int32 i = 0; i < this.works.primCount; i++)
		{
			item.code = this.works.code;
			item.tpage = this.works.tpage;
			item.clut = this.works.clut;
			Int32 num3 = (Int32)reader.ReadSByte();
			Int32 num4 = (Int32)reader.ReadSByte();
			num3 <<= 2;
			num4 = (Int32)((Single)num4 * num2);
			num3 = (Int32)((Single)num3 * num2);
			Int32 num5 = num3 - this.works.h;
			Int32 num6 = num3 + this.works.h;
			num4 <<= 2;
			Int32 num7 = num4 - this.works.w;
			num4 += this.works.w;
			num5 &= 65535;
			num6 &= 65535;
			num7 <<= 16;
			num4 <<= 16;
			Int32toSByteConverter int32toSByteConverter = num5 | num7;
			item.v0 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			int32toSByteConverter = (num6 | num7);
			item.v1 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			int32toSByteConverter = (num5 | num4);
			item.v2 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			int32toSByteConverter = (num6 | num4);
			item.v3 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			num5 = (Int32)reader.ReadByte();
			Int64 position = reader.BaseStream.Position;
			num7 = (num5 & 15);
			num7 <<= 1;
			num7 += this.works.pt;
			reader.BaseStream.Seek((Int64)num7, SeekOrigin.Begin);
			num6 = (Int32)reader.ReadUInt16();
			Int32 num8 = (Int32)((UInt32)this.works.h >> 1);
			num7 = (num6 & 255);
			num6 = (Int32)((UInt32)num6 >> 8);
			num4 = num7 + num8;
			num8 = (Int32)((UInt32)this.works.w >> 1);
			num3 = num6 + num8;
			num6 <<= 8;
			num3 <<= 8;
			num8 = (num7 | num6);
			item.uv0 = new Vector2((Single)(num8 & 255), (Single)(num8 >> 8 & 255));
			num8 = (num4 | num6);
			item.uv1 = new Vector2((Single)(num8 & 255), (Single)(num8 >> 8 & 255));
			num8 = (num7 | num3);
			item.uv2 = new Vector2((Single)(num8 & 255), (Single)(num8 >> 8 & 255));
			num8 = (num4 | num3);
			item.uv3 = new Vector2((Single)(num8 & 255), (Single)(num8 >> 8 & 255));
			num3 = (Int32)((UInt32)num5 >> 4);
			num3 <<= 2;
			num3 += this.works.rgb;
			reader.BaseStream.Seek((Int64)num3, SeekOrigin.Begin);
			num5 = (Int32)reader.ReadUInt16();
			num7 = (Int32)reader.ReadSByte();
			num7 <<= 16;
			num5 |= num7;
			if (this.works.fade >= 0)
			{
				UInt32 num9 = (UInt32)this.works.fade;
				num7 = (num5 & 255);
				UInt32 num10 = (UInt32)num7;
				num6 = (Int32)((UInt32)num5 >> 8);
				num6 &= 255;
				UInt32 num11 = (UInt32)num6;
				num7 = (Int32)((UInt32)num5 >> 16);
				num7 &= 255;
				UInt32 num12 = (UInt32)num7;
				num10 = num10 * num9 >> 10;
				num11 = num11 * num9 >> 10;
				num12 = num12 * num9 >> 10;
				num10 = (UInt32)Mathf.Clamp(num10, -32768f, 32767f);
				num11 = (UInt32)Mathf.Clamp(num11, -32768f, 32767f);
				num12 = (UInt32)Mathf.Clamp(num12, -32768f, 32767f);
				item.color = new Color(num10 / 255f, num11 / 255f, num12 / 255f, 1f);
			}
			else
			{
				num7 = (num5 & 255);
				UInt32 num10 = (UInt32)num7;
				num6 = (Int32)((UInt32)num5 >> 8);
				num6 &= 255;
				UInt32 num11 = (UInt32)num6;
				num7 = (Int32)((UInt32)num5 >> 16);
				num7 &= 255;
				UInt32 num12 = (UInt32)num7;
				item.color = new Color(num10 / 255f, num11 / 255f, num12 / 255f, 1f);
			}
			item.otz = 0;
			this.spsPrims.Add(item);
			reader.BaseStream.Seek(position, SeekOrigin.Begin);
		}
	}

	private void _GenerateSPSMesh()
	{
		if (this.spsPrims.Count == 0)
		{
			return;
		}
		Single num = (Single)this.scale / 4096f;
		base.transform.localScale = new Vector3(-num, -num, num);
		base.transform.localRotation = Quaternion.Euler(this.rot.x, this.rot.y, this.rot.z);
		base.transform.localPosition = this.pos;
		Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
		Vector3 a = camera.worldToCameraMatrix.inverse.GetColumn(3);
		Single num2 = Vector3.Distance(a, this.pos);
		Single num3 = num2 * this.spsScale * BattleSPS.spsK;
		if (num3 > 3f)
		{
			num3 = 3f;
		}
		base.transform.localScale *= num3;
		Matrix4x4 inverse = camera.worldToCameraMatrix.inverse;
		Vector3 vector = inverse.MultiplyVector(Vector3.forward);
		Vector3 rhs = inverse.MultiplyVector(Vector3.right);
		Vector3 a2 = Vector3.Cross(vector, rhs);
		base.transform.LookAt(base.transform.position + vector, -a2);
		Single d = 0.00390625f;
		this._vertices.Clear();
		this._colors.Clear();
		this._uv.Clear();
		this._indices.Clear();
		for (Int32 i = 0; i < this.spsPrims.Count; i++)
		{
			BattleSPS.FieldSPSPrim fieldSPSPrim = this.spsPrims[i];
			Vector3 b = new Vector3(0f, 0f, (Single)(this.spsPrims.Count - i) / 100f);
			Int32 count = this._vertices.Count;
			Vector3 v = fieldSPSPrim.v0;
			Vector3 v2 = fieldSPSPrim.v1;
			Vector3 v3 = fieldSPSPrim.v2;
			Vector3 v4 = fieldSPSPrim.v3;
			this._vertices.Add(v + b);
			this._vertices.Add(v2 + b);
			this._vertices.Add(v3 + b);
			this._vertices.Add(v4 + b);
			this._colors.Add(fieldSPSPrim.color);
			this._colors.Add(fieldSPSPrim.color);
			this._colors.Add(fieldSPSPrim.color);
			this._colors.Add(fieldSPSPrim.color);
			this._uv.Add(fieldSPSPrim.uv0 * d);
			this._uv.Add(fieldSPSPrim.uv1 * d);
			this._uv.Add(fieldSPSPrim.uv2 * d);
			this._uv.Add(fieldSPSPrim.uv3 * d);
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
		BattleSPS.FieldSPSPrim fieldSPSPrim2 = this.spsPrims[0];
		PSXTexture texture = PSXTextureMgr.GetTexture(fieldSPSPrim2.FlagTP, fieldSPSPrim2.FlagTY, fieldSPSPrim2.FlagTX, fieldSPSPrim2.FlagClutY, fieldSPSPrim2.FlagClutX);
		texture.SetFilter(FilterMode.Bilinear);
		Int32 num4 = (Int32)((this.arate >= 4) ? 4 : this.arate);
		this.materials[num4].mainTexture = BattleSPSSystem.statusTextures[this.refNo].textures[0];
		this.meshRenderer.material = this.materials[num4];
		if (this.spsActor != (UnityEngine.Object)null)
		{
			this.spsActor.spsPos = this.pos;
		}
	}

	public void GenerateSHP()
	{
		BattleSPSSystem.SPSTexture spstexture = BattleSPSSystem.statusTextures[this.refNo];
		this.shpGo = new GameObject[(Int32)spstexture.textures.Length];
		this.spsScale = spstexture.spsScale;
		this.spsDistance = spstexture.spsDistance;
		for (Int32 i = 0; i < (Int32)spstexture.textures.Length; i++)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = spstexture.extraPos;
			gameObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			component.material = new Material(this.materials[0])
			{
				mainTexture = spstexture.textures[i]
			};
			this.shpGo[i] = gameObject;
			this.shpGo[i].SetActive(false);
		}
	}

	public void AnimateSHP()
	{
		if (this.isUpdate)
		{
			Single num = (Single)this.scale / 4096f;
			base.transform.localScale = new Vector3(num, num, num);
			base.transform.localRotation = Quaternion.Euler(this.rot.x, this.rot.y, this.rot.z);
			base.transform.localPosition = this.pos;
			Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
			Vector3 a = camera.worldToCameraMatrix.inverse.GetColumn(3);
			Single num2 = Vector3.Distance(a, base.transform.localPosition);
			Single num3 = num2 * BattleSPS.spsK;
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			else if (num3 < 0.3f)
			{
				num3 = 0.3f;
			}
			base.transform.localScale *= num3;
			Matrix4x4 inverse = camera.worldToCameraMatrix.inverse;
			Vector3 vector = inverse.MultiplyVector(Vector3.forward);
			Vector3 rhs = inverse.MultiplyVector(Vector3.right);
			Vector3 a2 = Vector3.Cross(vector, rhs);
			base.transform.LookAt(base.transform.position + vector, -a2);
			for (Int32 i = 0; i < (Int32)this.shpGo.Length; i++)
			{
				this.shpGo[i].SetActive(false);
			}
			Int32 num4 = this.curFrame >> 4;
			num4 = num4 * (Int32)this.shpGo.Length / 9;
			if (num4 >= (Int32)this.shpGo.Length)
			{
				num4 %= (Int32)this.shpGo.Length;
			}
			BattleSPSSystem.SPSTexture spstexture = BattleSPSSystem.statusTextures[this.refNo];
			this.shpGo[num4].SetActive(true);
			this.isUpdate = false;
		}
		else
		{
			for (Int32 j = 0; j < (Int32)this.shpGo.Length; j++)
			{
				this.shpGo[j].SetActive(false);
			}
		}
	}

	public static Single spsK = 0.000259551482f;

	public FieldMap fieldMap;

	public Int32 spsIndex;

	public Transform spsTransform;

	public MeshRenderer meshRenderer;

	public MeshFilter meshFilter;

	public Byte[] spsBin;

	public BattleSPS.FieldSPSWork works;

	public List<BattleSPS.FieldSPSPrim> spsPrims;

	public FieldSPSActor spsActor;

	public Material[] materials;

	public Transform charTran;

	public Transform boneTran;

	public Int32 type;

	public GameObject[] shpGo;

	public Boolean isUpdate;

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

    public Boolean rotate;

    public BTL_DATA btl;

    public Int32 bone;

    public Vector3 rotArg;

	public Vector3 posOffset;

	public Int32 depthOffset;

	public Single spsScale;

	public Single spsDistance;

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
