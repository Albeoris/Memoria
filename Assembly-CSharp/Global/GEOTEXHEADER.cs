using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GEOTEXHEADER
{
	static GEOTEXHEADER()
	{
		// Note: this type is marked as 'beforefieldinit'.
		Dictionary<Int32, BGObjIndex> dictionary = new Dictionary<Int32, BGObjIndex>();
		dictionary.Add(4, new BGObjIndex(new Int32[1], new Int32[1]));
		Dictionary<Int32, BGObjIndex> dictionary2 = dictionary;
		Int32 key = 7;
		Int32[] array = new Int32[3];
		array[0] = 1;
		Int32[] array2 = new Int32[3];
		array2[0] = 1;
		dictionary2.Add(key, new BGObjIndex(array, array2));
		dictionary.Add(9, new BGObjIndex(new Int32[1], new Int32[]
		{
			10
		}));
		dictionary.Add(29, new BGObjIndex(new Int32[1], new Int32[1]));
		dictionary.Add(32, new BGObjIndex(new Int32[]
		{
			2
		}, new Int32[]
		{
			3
		}));
		dictionary.Add(42, new BGObjIndex(new Int32[]
		{
			1
		}, new Int32[]
		{
			3
		}));
		dictionary.Add(51, new BGObjIndex(new Int32[1], new Int32[]
		{
			10
		}));
		dictionary.Add(52, new BGObjIndex(new Int32[1], new Int32[]
		{
			6
		}));
		dictionary.Add(56, new BGObjIndex(new Int32[]
		{
			1
		}, new Int32[1]));
		dictionary.Add(57, new BGObjIndex(new Int32[1], new Int32[]
		{
			2
		}));
		dictionary.Add(66, new BGObjIndex(new Int32[1], new Int32[]
		{
			1
		}));
		dictionary.Add(67, new BGObjIndex(new Int32[1], new Int32[]
		{
			10
		}));
		Dictionary<Int32, BGObjIndex> dictionary3 = dictionary;
		Int32 key2 = 69;
		Int32[] array3 = new Int32[4];
		array3[0] = 2;
		array3[1] = 3;
		dictionary3.Add(key2, new BGObjIndex(array3, new Int32[]
		{
			5,
			0,
			5,
			4
		}));
		dictionary.Add(71, new BGObjIndex(new Int32[]
		{
			0,
			0,
			2
		}, new Int32[]
		{
			10,
			9,
			4
		}));
		dictionary.Add(82, new BGObjIndex(new Int32[]
		{
			2
		}, new Int32[1]));
		dictionary.Add(92, new BGObjIndex(new Int32[]
		{
			2
		}, new Int32[]
		{
			3
		}));
		dictionary.Add(108, new BGObjIndex(new Int32[1], new Int32[]
		{
			11
		}));
		dictionary.Add(118, new BGObjIndex(new Int32[1], new Int32[]
		{
			4
		}));
		dictionary.Add(128, new BGObjIndex(new Int32[2], new Int32[]
		{
			2,
			3
		}));
		dictionary.Add(137, new BGObjIndex(new Int32[2], new Int32[]
		{
			2,
			3
		}));
		dictionary.Add(143, new BGObjIndex(new Int32[1], new Int32[1]));
		dictionary.Add(147, new BGObjIndex(new Int32[1], new Int32[1]));
		dictionary.Add(155, new BGObjIndex(new Int32[1], new Int32[1]));
		dictionary.Add(164, new BGObjIndex(new Int32[1], new Int32[1]));
		dictionary.Add(172, new BGObjIndex(new Int32[1], new Int32[]
		{
			5
		}));
		GEOTEXHEADER.bgObjMappings = dictionary;
	}

	public void ReadTextureAnim(String path)
	{
		String[] tabInfo;
		Byte[] binAsset = AssetManager.LoadBytes(path, out tabInfo, false);
		if (binAsset == null)
		{
			global::Debug.LogWarning("Cannot find GeoTexAnim for : " + path);
			return;
		}
		using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(binAsset)))
		{
			this.count = binaryReader.ReadUInt16();
			this.pad = binaryReader.ReadUInt16();
			this.geotex = new GEOTEXANIMHEADER[(Int32)this.count];
			for (Int32 i = 0; i < (Int32)this.count; i++)
			{
				this.geotex[i] = new GEOTEXANIMHEADER();
				this.geotex[i].ReadData(binaryReader);
			}
		}
	}

	public void ReadPlayerTextureAnim(BTL_DATA btl, String path, Int32 scale = 1)
	{
		this.ReadTextureAnim(path);
		if (this.geotex == null)
		{
			return;
		}
		String geoName = FF9BattleDB.GEO.GetValue((Int32)btl.dms_geo_id);
		if (btl.originalGo != (UnityEngine.Object)null)
		{
			this._smrs = btl.originalGo.GetComponentsInChildren<SkinnedMeshRenderer>();
		}
		else
		{
			this._smrs = btl.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		}
		this.MultiTexAnim(geoName, scale);
	}

	public void ReadTrancePlayerTextureAnim(BTL_DATA btl, String geoName, Int32 scale = 1)
	{
		this.ReadTextureAnim("Models/GeoTexAnim/" + geoName + ".tab");
		if (this.geotex == null)
		{
			return;
		}
		btl.tranceGo.SetActive(true);
		this._smrs = btl.tranceGo.GetComponentsInChildren<SkinnedMeshRenderer>();
		btl.tranceGo.SetActive(false);
		this.MultiTexAnim(geoName, scale);
	}

	private void MultiTexAnim(String geoName, Int32 scale)
	{
		GeoTexAnim.SetTexAnimIndexs(geoName, out this._mainTextureIndexs, out this._subTextureIndexs);
		this.RenderTexWidth = (Single)this._smrs[this._mainTextureIndexs[0]].material.mainTexture.width;
		this.RenderTexHeight = (Single)this._smrs[this._mainTextureIndexs[0]].material.mainTexture.height;
		for (Int32 i = 0; i < (Int32)this.count; i++)
		{
			GeoTexAnim.MapTextureToTexAnimIndex(this._mainTextureIndexs[i], this._smrs[this._mainTextureIndexs[i]].material.mainTexture, this.TextureMapping);
			GeoTexAnim.MapTextureToTexAnimIndex(this._subTextureIndexs[i], this._smrs[this._subTextureIndexs[i]].material.mainTexture, this.TextureMapping);
			GeoTexAnim.MapRenderTexToTexAnimIndex(this._mainTextureIndexs[i], this.RenderTexWidth, this.RenderTexHeight, this._smrs[this._mainTextureIndexs[i]], this.TextureMapping[this._mainTextureIndexs[i]], this.RenderTexMapping);
		}
		Single renderTexWidth = this.RenderTexWidth;
		Single renderTexHeight = this.RenderTexHeight;
		for (Int32 j = 0; j < (Int32)this.count; j++)
		{
			Rect rect = this.geotex[j].targetuv;
			rect.x *= (Single)scale;
			rect.y *= (Single)scale;
			rect.width *= (Single)scale;
			rect.height *= (Single)scale;
			this.geotex[j].targetuv = rect;
			for (Int32 k = 0; k < (Int32)this.geotex[j].numframes; k++)
			{
				rect = this.geotex[j].rectuvs[k];
				rect.x *= (Single)scale;
				rect.y *= (Single)scale;
				rect.width *= (Single)scale;
				rect.height *= (Single)scale;
				rect.y = renderTexHeight - rect.y - rect.height;
				rect.x += 0.5f;
				rect.y += 0.5f;
				rect.width -= 1f;
				rect.height -= 1f;
				rect.x /= renderTexWidth;
				rect.y /= renderTexHeight;
				rect.width /= renderTexWidth;
				rect.height /= renderTexHeight;
				this.geotex[j].rectuvs[k] = rect;
			}
		}
	}

	public void ReadBGTextureAnim(String battleModelPath)
	{
		this.bbgnumber = Int32.Parse(battleModelPath.Replace("BBG_B", String.Empty));
		String path = "BattleMap/BattleTexAnim/" + battleModelPath.Replace("BBG", "TAM") + ".tab";
		this.ReadTextureAnim(path);
	}

	public void InitTextureAnim()
	{
		if (this.geotex == null)
		{
			return;
		}
		this._mrs = FF9StateSystem.Battle.FF9Battle.map.btlBGPtr.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[][] array = new MeshRenderer[2][];
		if (this.bbgnumber == 7)
		{
			array[0] = FF9StateSystem.Battle.FF9Battle.map.btlBGObjAnim[0].GetComponentsInChildren<MeshRenderer>();
			array[1] = FF9StateSystem.Battle.FF9Battle.map.btlBGObjAnim[1].GetComponentsInChildren<MeshRenderer>();
		}
		BGObjIndex bgobjIndex = GEOTEXHEADER.bgObjMappings[this.bbgnumber];
		this.texAnimMaterials = new Material[(Int32)this.count];
		this.TexAnimTextures = new Texture[(Int32)this.count];
		this.extraTexAimMaterials = new Material[(Int32)this.count];
		this.extraTexAnimTrTextures = new Texture[(Int32)this.count];
		for (Int32 i = 0; i < (Int32)this.count; i++)
		{
			if (this.bbgnumber == 7 && i != 0)
			{
				this.texAnimMaterials[i] = array[i - 1][bgobjIndex.groupIndex[i]].materials[bgobjIndex.materialIndex[i]];
				this.TexAnimTextures[i] = this.texAnimMaterials[i].mainTexture;
			}
			else
			{
				this.texAnimMaterials[i] = this._mrs[bgobjIndex.groupIndex[i]].materials[bgobjIndex.materialIndex[i]];
				this.TexAnimTextures[i] = this.texAnimMaterials[i].mainTexture;
				if (this.bbgnumber == 57)
				{
					this.extraTexAimMaterials[i] = this._mrs[2].materials[0];
					this.extraTexAnimTrTextures[i] = this.extraTexAimMaterials[i].mainTexture;
				}
				else if (this.bbgnumber == 71)
				{
					this.extraTexAimMaterials[i] = this._mrs[0].materials[11];
					this.extraTexAnimTrTextures[i] = this.extraTexAimMaterials[i].mainTexture;
				}
			}
		}
	}

	public UInt16 count;

	public UInt16 pad;

	public GEOTEXANIMHEADER[] geotex;

	public SkinnedMeshRenderer[] _smrs;

	private MeshRenderer[] _mrs;

	public Material[] texAnimMaterials;

	public Texture[] TexAnimTextures;

	public Material[] extraTexAimMaterials;

	public Texture[] extraTexAnimTrTextures;

	public Int32[] _mainTextureIndexs;

	public Int32[] _subTextureIndexs;

	public Dictionary<Int32, Texture> TextureMapping = new Dictionary<Int32, Texture>();

	public Dictionary<Int32, RenderTexture> RenderTexMapping = new Dictionary<Int32, RenderTexture>();

	public Single RenderTexWidth;

	public Single RenderTexHeight;

	public Int32 bbgnumber;

	public static Dictionary<Int32, BGObjIndex> bgObjMappings;
}
