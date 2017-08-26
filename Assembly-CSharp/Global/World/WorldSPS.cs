using System;
using System.Collections.Generic;
using System.IO;
using Memoria.Scripts;
using UnityEngine;

public class WorldSPS : MonoBehaviour
{
	public void Init()
	{
		this.spsBin = null;
		this.prev = (WorldSPS)null;
		this.next = (WorldSPS)null;
		this.type = 0;
		this.no = -1;
		this.size = 0;
		this.frame = 0;
		this.prm0 = 0;
		this.prm1 = 0;
		this.arate = 0;
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
		this.works = new WorldSPS.WorldSPSWork();
		this.spsPrims = new List<WorldSPS.WorldSPSPrim>();
		this._vertices = new List<Vector3>();
		this._colors = new List<Color>();
		this._uv = new List<Vector2>();
		this._indices = new List<Int32>();
		this.materials = new Material[5];
		this.materials[0] = new Material(ShadersLoader.Find("WorldMap/SPS_Abr_0"));
		this.materials[1] = new Material(ShadersLoader.Find("WorldMap/SPS_Abr_1"));
		this.materials[2] = new Material(ShadersLoader.Find("WorldMap/SPS_Abr_2"));
		this.materials[3] = new Material(ShadersLoader.Find("WorldMap/SPS_Abr_3"));
		this.materials[4] = new Material(ShadersLoader.Find("WorldMap/SPS_Abr_None"));
	}

	public void S_WoSpsPut(Byte[] spsBin, Vector3 pos, Vector3 rot, Int32 size, Int32 frame, Int32 abr, Int32 fade, Int32 clutoff, Int32 shtsc)
	{
		this.spsBin = spsBin;
		this.pos = pos;
		this.rot = rot;
		this.size = size;
		this.arate = abr;
		using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(spsBin)))
		{
			if (clutoff != 0)
			{
				binaryReader.BaseStream.Seek(4L, SeekOrigin.Begin);
				UInt16 num = binaryReader.ReadUInt16();
				Int32 num2 = (Int32)num;
				num2 += clutoff;
				num = (UInt16)num2;
				using (BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream(spsBin)))
				{
					binaryWriter.BaseStream.Seek(4L, SeekOrigin.Begin);
					binaryWriter.Write(num);
				}
				binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
			}
			Int32 num3 = frame;
			Int32 num4 = (Int32)(binaryReader.ReadUInt16() & 32767);
			if (num3 >= num4)
			{
				num3 %= num4;
			}
			Int32 num5 = num3 * 2 + 8;
			num4 = num4 * 2 + 8;
			binaryReader.BaseStream.Seek((Int64)num4, SeekOrigin.Begin);
			num3 = (Int32)binaryReader.ReadUInt16();
			this.works.pt = num4 + 2;
			this.works.rgb = this.works.pt + num3 * 2 + 2;
			binaryReader.BaseStream.Seek(2L, SeekOrigin.Begin);
			this.works.tpage = (UInt16)((Int32)binaryReader.ReadUInt16() | (abr & 3) << 5);
			this.works.clut = binaryReader.ReadUInt16();
			this.works.w = (Int32)((binaryReader.ReadByte() - 1) * 2);
			this.works.h = (Int32)((binaryReader.ReadByte() - 1) * 2);
			binaryReader.BaseStream.Seek((Int64)num5, SeekOrigin.Begin);
			num4 = (Int32)binaryReader.ReadUInt16();
			binaryReader.BaseStream.Seek((Int64)num4, SeekOrigin.Begin);
			this.works.primCount = (Int32)binaryReader.ReadByte();
			num4++;
			Int32 num6 = num4;
			binaryReader.BaseStream.Seek((Int64)num6, SeekOrigin.Begin);
			this.works.code = (Byte)(44 | (Int32)((abr != 255) ? 2 : 0));
			this.works.fade = fade << 4;
			this.works.shtsc = shtsc;
			this.works.otshift = 0;
			this._GenerateSPSPrims(binaryReader, num6);
			this._GenerateSPSMesh();
		}
	}

	private void _GenerateSPSPrims(BinaryReader reader, Int32 spsOffset)
	{
		this.spsPrims.Clear();
		WorldSPS.WorldSPSPrim item = default(WorldSPS.WorldSPSPrim);
		Int32 num = this.works.shtsc;
		Int32 num2 = this.works.w << num;
		Int32 num3 = this.works.h << num;
		num++;
		reader.BaseStream.Seek((Int64)spsOffset, SeekOrigin.Begin);
		for (Int32 i = 0; i < this.works.primCount; i++)
		{
			item.code = this.works.code;
			item.tpage = this.works.tpage;
			item.clut = this.works.clut;
			Int32 num4 = (Int32)reader.ReadSByte();
			Int32 num5 = (Int32)reader.ReadSByte();
			num4 <<= 2;
			Int32 num6 = num4 - num3;
			Int32 num7 = num4 + num3;
			num5 <<= 2;
			Int32 num8 = num5 - num2;
			num5 += num2;
			num6 &= 65535;
			num7 &= 65535;
			num8 <<= 16;
			num5 <<= 16;
			Int32toSByteConverter int32toSByteConverter = num6 | num8;
			item.v0 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			int32toSByteConverter = (num7 | num8);
			item.v1 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			int32toSByteConverter = (num6 | num5);
			item.v2 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			int32toSByteConverter = (num7 | num5);
			item.v3 = new Vector3((Single)((Int32)int32toSByteConverter.SByte2 << 8 | (Int32)((Byte)int32toSByteConverter.SByte1)), (Single)((Int32)int32toSByteConverter.SByte4 << 8 | (Int32)((Byte)int32toSByteConverter.SByte3)), 0f);
			num6 = (Int32)reader.ReadByte();
			Int64 position = reader.BaseStream.Position;
			num8 = (num6 & 15);
			num8 <<= 1;
			num8 += this.works.pt;
			reader.BaseStream.Seek((Int64)num8, SeekOrigin.Begin);
			num7 = (Int32)reader.ReadUInt16();
			Int32 num9 = (Int32)((UInt32)num3 >> num);
			num8 = (num7 & 255);
			num7 = (Int32)((UInt32)num7 >> 8);
			num5 = num8 + num9;
			num9 = (Int32)((UInt32)num2 >> num);
			num4 = num7 + num9;
			num7 <<= 8;
			num4 <<= 8;
			num9 = (num8 | num7);
			item.uv0 = new Vector2((Single)(num9 & 255), (Single)(num9 >> 8 & 255));
			num9 = (num5 | num7);
			item.uv1 = new Vector2((Single)(num9 & 255), (Single)(num9 >> 8 & 255));
			num9 = (num8 | num4);
			item.uv2 = new Vector2((Single)(num9 & 255), (Single)(num9 >> 8 & 255));
			num9 = (num5 | num4);
			item.uv3 = new Vector2((Single)(num9 & 255), (Single)(num9 >> 8 & 255));
			num4 = (Int32)((UInt32)num6 >> 4);
			num4 <<= 2;
			num4 += this.works.rgb;
			reader.BaseStream.Seek((Int64)num4, SeekOrigin.Begin);
			num6 = (Int32)reader.ReadUInt16();
			num8 = (Int32)reader.ReadSByte();
			num8 <<= 16;
			num6 |= num8;
			if (this.works.fade >= 0)
			{
				UInt32 fade = (UInt32)this.works.fade;
				num8 = (num6 & 255);
				UInt32 num10 = (UInt32)num8;
				num7 = (Int32)((UInt32)num6 >> 8);
				num7 &= 255;
				UInt32 num11 = (UInt32)num7;
				num8 = (Int32)((UInt32)num6 >> 16);
				num8 &= 255;
				UInt32 num12 = (UInt32)num8;
				num10 = num10 * fade >> 12;
				num11 = num11 * fade >> 12;
				num12 = num12 * fade >> 12;
				num10 = (UInt32)Mathf.Clamp(num10, -32768f, 32767f);
				num11 = (UInt32)Mathf.Clamp(num11, -32768f, 32767f);
				num12 = (UInt32)Mathf.Clamp(num12, -32768f, 32767f);
				item.color = new Color(num10 / 127f, num11 / 127f, num12 / 127f, 1f);
			}
			else
			{
				num8 = (num6 & 255);
				UInt32 num10 = (UInt32)num8;
				num7 = (Int32)((UInt32)num6 >> 8);
				num7 &= 255;
				UInt32 num11 = (UInt32)num7;
				num8 = (Int32)((UInt32)num6 >> 16);
				num8 &= 255;
				UInt32 num12 = (UInt32)num8;
				item.color = new Color(num10 / 127f, num11 / 127f, num12 / 127f, 1f);
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
		Matrix4x4 matrix4x = Matrix4x4.TRS(this.pos, Quaternion.Euler(this.rot.x, this.rot.y, this.rot.z), new Vector3(-num, -num, num));
		base.transform.position = this.pos;
		Single d = 0.00390625f;
		this._vertices.Clear();
		this._colors.Clear();
		this._uv.Clear();
		this._indices.Clear();
		for (Int32 i = 0; i < this.spsPrims.Count; i++)
		{
			WorldSPS.WorldSPSPrim worldSPSPrim = this.spsPrims[i];
			Vector3 b = new Vector3(0f, 0f, (Single)(this.spsPrims.Count - i) / 100f);
			Int32 count = this._vertices.Count;
			this._vertices.Add(worldSPSPrim.v0 + b);
			this._vertices.Add(worldSPSPrim.v1 + b);
			this._vertices.Add(worldSPSPrim.v2 + b);
			this._vertices.Add(worldSPSPrim.v3 + b);
			this._colors.Add(worldSPSPrim.color);
			this._colors.Add(worldSPSPrim.color);
			this._colors.Add(worldSPSPrim.color);
			this._colors.Add(worldSPSPrim.color);
			this._uv.Add(worldSPSPrim.uv0 * d);
			this._uv.Add(worldSPSPrim.uv1 * d);
			this._uv.Add(worldSPSPrim.uv2 * d);
			this._uv.Add(worldSPSPrim.uv3 * d);
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
		WorldSPS.WorldSPSPrim worldSPSPrim2 = this.spsPrims[0];
		PSXTexture texture = PSXTextureMgr.GetTexture(worldSPSPrim2.FlagTP, worldSPSPrim2.FlagTY, worldSPSPrim2.FlagTX, worldSPSPrim2.FlagClutY, worldSPSPrim2.FlagClutX);
		if (texture.texture.filterMode != FilterMode.Bilinear)
		{
			texture.SetFilter(FilterMode.Bilinear);
		}
		Int32 num2 = (Int32)((this.arate >= 4) ? 4 : this.arate);
		this.materials[num2].mainTexture = texture.texture;
		this.meshRenderer.material = this.materials[num2];
	}

	public Int32 spsIndex;

	public Transform spsTransform;

	public MeshRenderer meshRenderer;

	public MeshFilter meshFilter;

	public WorldSPS.WorldSPSWork works;

	public List<WorldSPS.WorldSPSPrim> spsPrims;

	public Material[] materials;

	public Byte[] spsBin;

	public WorldSPS prev;

	public WorldSPS next;

	public Int32 type;

	public Int32 no;

	public Int32 size;

	public Int32 frame;

	public Int32 prm0;

	public Int32 prm1;

	public Int32 arate;

	public Vector3 pos;

	public Int32 scale;

	public Vector3 rot;

	public Vector3 rotArg;

	public Vector3 posOffset;

	public Int32 depthOffset;

	private List<Vector3> _vertices;

	private List<Color> _colors;

	private List<Vector2> _uv;

	private List<Int32> _indices;

	public class WorldSPSWork
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

		public Int32 shtsc;

		public Int32 otshift;
	}

	public struct WorldSPSPrim
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
