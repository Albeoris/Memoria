using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GeoTexAnim : HonoBehavior
{
	public override void HonoAwake()
	{
		this.Count = 0;
		this._isLoaded = false;
		this._cTime = 0f;
	}

	public override void HonoOnDestroy()
	{
		base.HonoOnDestroy();
		if (this.RenderTex != (UnityEngine.Object)null)
		{
			this.RenderTex.Release();
			this.RenderTex = (RenderTexture)null;
		}
	}

	public void Load(String modelName, Int32 mainTextureIndex, Int32 subTextureIndex, Int32 scale = 1)
	{
		this._mainTextureIndex = mainTextureIndex;
		this._subTextureIndex = subTextureIndex;
		TextAsset textAsset = AssetManager.Load<TextAsset>(modelName + ".tab", false);
		if (textAsset == (UnityEngine.Object)null)
		{
			return;
		}
		Byte[] bytes = textAsset.bytes;
		using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes)))
		{
			this.Count = (Int32)binaryReader.ReadUInt16();
			Int32 num = (Int32)binaryReader.ReadUInt16();
			this.Anims = new GEOTEXANIMHEADER[this.Count];
			for (Int32 i = 0; i < this.Count; i++)
			{
				this.Anims[i] = new GEOTEXANIMHEADER();
				this.Anims[i].ReadData(binaryReader);
			}
		}
		this._isLoaded = true;
		this._smrs = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		if (modelName.Contains("GEO_SUB_F0_KUW"))
		{
			this._mainTexture = this._smrs[0].materials[this._mainTextureIndex].mainTexture;
			this._subTexture = this._smrs[0].materials[this._subTextureIndex].mainTexture;
		}
		else
		{
			this._mainTexture = this._smrs[this._mainTextureIndex].material.mainTexture;
			this._subTexture = this._smrs[this._subTextureIndex].material.mainTexture;
		}
		this.RenderTex = new RenderTexture(this._mainTexture.width, this._mainTexture.height, 24);
		this.RenderTex.filterMode = FilterMode.Bilinear;
		this.RenderTex.wrapMode = TextureWrapMode.Repeat;
		this.RenderTex.name = modelName + "_RT";
		this.RenderTexWidth = (Single)this._mainTexture.width;
		this.RenderTexHeight = (Single)this._mainTexture.height;
		this._smrs[this._mainTextureIndex].material.mainTexture = this.RenderTex;
		Single num2 = (Single)this._subTexture.width;
		Single num3 = (Single)this._subTexture.height;
		for (Int32 j = 0; j < this.Count; j++)
		{
			Rect rect = this.Anims[j].targetuv;
			rect.x *= (Single)scale;
			rect.y *= (Single)scale;
			rect.width *= (Single)scale;
			rect.height *= (Single)scale;
			this.Anims[j].targetuv = rect;
			for (Int32 k = 0; k < (Int32)this.Anims[j].numframes; k++)
			{
				rect = this.Anims[j].rectuvs[k];
				rect.x *= (Single)scale;
				rect.y *= (Single)scale;
				rect.width *= (Single)scale;
				rect.height *= (Single)scale;
				rect.y = num3 - rect.y - rect.height;
				rect.x += 0.5f;
				rect.y += 0.5f;
				rect.width -= 1f;
				rect.height -= 1f;
				rect.x /= num2;
				rect.y /= num3;
				rect.width /= num2;
				rect.height /= num3;
				this.Anims[j].rectuvs[k] = rect;
			}
		}
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = this.RenderTex;
		GL.PushMatrix();
		GL.Clear(true, true, Color.clear);
		GL.LoadPixelMatrix(0f, this.RenderTexWidth, this.RenderTexHeight, 0f);
		Graphics.DrawTexture(new Rect(0f, 0f, this.RenderTexWidth, this.RenderTexHeight), this._mainTexture);
		GL.PopMatrix();
		RenderTexture.active = active;
	}

	private IEnumerator WaitToRecreateTexAnim(RenderTexture reRenderTex, Texture reMainTex)
	{
		this._smrs[this._mainTextureIndex].material.mainTexture = this._mainTexture;
		this._smrs[this._subTextureIndex].material.mainTexture = this._subTexture;
		yield return new WaitForEndOfFrame();
		this._smrs[this._mainTextureIndex].material.mainTexture = this.RenderTex;
		this.RecreateTexAnim(reRenderTex, reMainTex);
		yield break;
	}

	private void RecreateTexAnim(RenderTexture reRenderTex, Texture reMainTex)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = reRenderTex;
		GL.PushMatrix();
		GL.Clear(true, true, Color.clear);
		GL.LoadPixelMatrix(0f, this.RenderTexWidth, this.RenderTexHeight, 0f);
		Graphics.DrawTexture(new Rect(0f, 0f, this.RenderTexWidth, this.RenderTexHeight), reMainTex);
		GL.PopMatrix();
		RenderTexture.active = active;
	}

	public Int32 geoTexAnimGetCount()
	{
		return this.Count;
	}

	public void geoTexAnimPlay(Int32 anum)
	{
		if (!this._isLoaded)
		{
			return;
		}
		GEOTEXANIMHEADER geotexanimheader = this.Anims[anum];
		geotexanimheader.flags = (Byte)(geotexanimheader.flags | 1);
		this.Anims[anum].frame = 0;
		this.Anims[anum].lastframe = 4096;
		if (anum == 0)
		{
			Byte b = 3;
			GEOTEXANIMHEADER geotexanimheader2 = this.Anims[2];
			geotexanimheader2.flags = (Byte)(geotexanimheader2.flags & (Byte)(~b));
		}
	}

	public static void geoTexAnimPlay(GEOTEXHEADER tab, Int32 anum)
	{
		if (tab == null || tab.geotex == null)
		{
			return;
		}
		GEOTEXANIMHEADER geotexanimheader = tab.geotex[anum];
		geotexanimheader.flags = (Byte)(geotexanimheader.flags | 1);
		tab.geotex[anum].frame = 0;
		tab.geotex[anum].lastframe = 4096;
	}

	public void geoTexAnimPlayOnce(Int32 anum)
	{
		if (!this._isLoaded)
		{
			return;
		}
		GEOTEXANIMHEADER geotexanimheader = this.Anims[anum];
		geotexanimheader.flags = (Byte)(geotexanimheader.flags | 3);
		this.Anims[anum].frame = 0;
		this.Anims[anum].lastframe = 4096;
	}

	public static void geoTexAnimPlayOnce(GEOTEXHEADER tab, Int32 anum)
	{
		if (tab == null || tab.geotex == null)
		{
			return;
		}
		GEOTEXANIMHEADER geotexanimheader = tab.geotex[anum];
		geotexanimheader.flags = (Byte)(geotexanimheader.flags | 3);
		tab.geotex[anum].frame = 0;
		tab.geotex[anum].lastframe = 4096;
	}

	public void geoTexAnimStop(Int32 anum)
	{
		if (!this._isLoaded)
		{
			return;
		}
		Byte b = 3;
		GEOTEXANIMHEADER geotexanimheader = this.Anims[anum];
		geotexanimheader.flags = (Byte)(geotexanimheader.flags & (Byte)(~b));
	}

	public static void geoTexAnimStop(GEOTEXHEADER tab, Int32 anum)
	{
		if (tab == null || tab.geotex == null)
		{
			return;
		}
		Byte b = 3;
		GEOTEXANIMHEADER geotexanimheader = tab.geotex[anum];
		geotexanimheader.flags = (Byte)(geotexanimheader.flags & (Byte)(~b));
	}

	public static void geoTexAnimFreezeState(BTL_DATA btl)
	{
		if (btl.backupGeotex != null || btl.texanimptr == null || btl.texanimptr.geotex == null)
		{
			return;
		}
		Int32 num = (Int32)btl.texanimptr.geotex.Length;
		btl.backupGeotex = new Byte[num];
		for (Int32 i = 0; i < num; i++)
		{
			btl.backupGeotex[i] = btl.texanimptr.geotex[i].flags;
			GeoTexAnim.geoTexAnimStop(btl.texanimptr, i);
		}
		if (btl.bi.player != 0 && btl.tranceTexanimptr != null && btl.tranceTexanimptr.geotex != null)
		{
			num = (Int32)btl.tranceTexanimptr.geotex.Length;
			btl.backupGeoTrancetex = new Byte[num];
			for (Int32 j = 0; j < num; j++)
			{
				btl.backupGeoTrancetex[j] = btl.tranceTexanimptr.geotex[j].flags;
				GeoTexAnim.geoTexAnimStop(btl.tranceTexanimptr, j);
			}
		}
	}

	public static void geoTexAnimReturnState(BTL_DATA btl)
	{
		if (btl.texanimptr == null || btl.texanimptr.geotex == null)
		{
			return;
		}
		Int32 num = (Int32)btl.texanimptr.geotex.Length;
		Int32 num2 = 0;
		while (num2 < num && btl.backupGeotex != null)
		{
			btl.texanimptr.geotex[num2].flags = btl.backupGeotex[num2];
			num2++;
		}
		if (btl.bi.player != 0 && btl.tranceTexanimptr != null && btl.tranceTexanimptr.geotex != null)
		{
			num = (Int32)btl.tranceTexanimptr.geotex.Length;
			Int32 num3 = 0;
			while (num3 < num && btl.backupGeoTrancetex != null)
			{
				btl.tranceTexanimptr.geotex[num3].flags = btl.backupGeoTrancetex[num3];
				num3++;
			}
		}
		btl.backupGeotex = null;
		btl.backupGeoTrancetex = null;
	}

	public Boolean ff9fieldCharIsTexAnimActive()
	{
		if (!this._isLoaded)
		{
			return false;
		}
		Int32 num = this.geoTexAnimGetCount();
		for (Int32 i = num - 1; i >= 0; i--)
		{
			if ((this.Anims[i].flags & 1) != 0)
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		if (!this._isLoaded)
		{
			return;
		}
		if (!this.RenderTex.IsCreated())
		{
			base.StartCoroutine(this.WaitToRecreateTexAnim(this.RenderTex, this._mainTexture));
		}
	}

	public override void HonoUpdate()
	{
		if (!this._isLoaded)
		{
			return;
		}
		this._cTime += Time.deltaTime;
		if (this._cTime >= 0.06666667f)
		{
			this._cTime = 0f;
			for (Int32 i = 0; i < this.Count; i++)
			{
				GEOTEXANIMHEADER geotexanimheader = this.Anims[i];
				if ((geotexanimheader.flags & 1) != 0)
				{
					Int32 num = geotexanimheader.frame;
					Int32 lastframe = (Int32)geotexanimheader.lastframe;
					Int16 num2 = (Int16)(num >> 12);
					if (num2 >= 0)
					{
						if ((Int32)num2 != lastframe)
						{
							for (Int32 j = 0; j < (Int32)geotexanimheader.count; j++)
							{
								RenderTexture active = RenderTexture.active;
								RenderTexture.active = this.RenderTex;
								GL.PushMatrix();
								GL.LoadPixelMatrix(0f, this.RenderTexWidth, this.RenderTexHeight, 0f);
								Graphics.DrawTexture(new Rect(0f, 0f, this.RenderTexWidth, this.RenderTexHeight), this._mainTexture);
								Graphics.DrawTexture(geotexanimheader.targetuv, this._subTexture, geotexanimheader.rectuvs[(Int32)num2], 0, 0, 0, 0);
								GL.PopMatrix();
								RenderTexture.active = active;
							}
							geotexanimheader.lastframe = num2;
						}
						Int16 rate = geotexanimheader.rate;
						num += (Int32)rate;
					}
					else
					{
						num += 4096;
					}
					if (num >> 12 < (Int32)geotexanimheader.numframes)
					{
						geotexanimheader.frame = num;
					}
					else if (geotexanimheader.randrange > 0)
					{
						UInt32 num3 = this.geoTexAnimRandom((UInt32)geotexanimheader.randmin, (UInt32)geotexanimheader.randrange);
						geotexanimheader.frame = (Int32)(-(Int32)((UInt64)((UInt64)num3 << 12)));
					}
					else if ((geotexanimheader.flags & 2) != 0)
					{
						Byte b = 3;
						GEOTEXANIMHEADER geotexanimheader2 = geotexanimheader;
						geotexanimheader2.flags = (Byte)(geotexanimheader2.flags & (Byte)(~b));
					}
					else
					{
						geotexanimheader.frame = 0;
					}
				}
			}
			return;
		}
	}

	private UInt32 geoTexAnimRandom(UInt32 randmin, UInt32 randrange)
	{
		return (UInt32)UnityEngine.Random.Range((Int32)randmin, (Int32)(randrange + 1u));
	}

	public static void geoTexAnimService(GEOTEXHEADER texHeader)
	{
		if (texHeader == null)
		{
			return;
		}
		for (Int32 i = 0; i < (Int32)texHeader.count; i++)
		{
			GEOTEXANIMHEADER geotexanimheader = texHeader.geotex[i];
			if ((geotexanimheader.flags & 1) != 0)
			{
				if (geotexanimheader.numframes != 0)
				{
					Int32 num = geotexanimheader.frame;
					Int16 lastframe = geotexanimheader.lastframe;
					Int16 num2 = (Int16)(num >> 12);
					if (num2 >= 0)
					{
						if (num2 != lastframe)
						{
							for (Int32 j = 0; j < (Int32)geotexanimheader.count; j++)
							{
								if (texHeader.geotex != null)
								{
									GeoTexAnim.MultiTexAnimService(geotexanimheader, texHeader, i, num2);
								}
							}
							geotexanimheader.lastframe = num2;
						}
						Int16 rate = geotexanimheader.rate;
						num += (Int32)rate;
					}
					else
					{
						num += 4096;
					}
					if (num >> 12 < (Int32)geotexanimheader.numframes)
					{
						geotexanimheader.frame = num;
					}
					else if (geotexanimheader.randrange > 0)
					{
						UInt32 num3 = (UInt32)UnityEngine.Random.Range((Int32)geotexanimheader.randmin, (Int32)(geotexanimheader.randrange + 1));
						geotexanimheader.frame = (Int32)(-(Int32)((UInt64)((UInt64)num3 << 12)));
					}
					else if ((geotexanimheader.flags & 2) != 0)
					{
						Byte b = 3;
						GEOTEXANIMHEADER geotexanimheader2 = geotexanimheader;
						geotexanimheader2.flags = (Byte)(geotexanimheader2.flags & (Byte)(~b));
					}
					else
					{
						geotexanimheader.frame = 0;
					}
				}
				else if ((geotexanimheader.flags & 4) != 0)
				{
					global::Debug.Log("SCROLL!!!");
				}
			}
		}
	}

	private static void MultiTexAnimService(GEOTEXANIMHEADER texAnimHeader, GEOTEXHEADER texHeader, Int32 i, Int16 framenum)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = texHeader.RenderTexMapping[texHeader._mainTextureIndexs[i]];
		GL.PushMatrix();
		GL.LoadPixelMatrix(0f, texHeader.RenderTexWidth, texHeader.RenderTexHeight, 0f);
		Graphics.DrawTexture(new Rect(0f, 0f, texHeader.RenderTexWidth, texHeader.RenderTexHeight), texHeader.TextureMapping[texHeader._mainTextureIndexs[i]], GEOTEXANIMHEADER.texAnimMat);
		Graphics.DrawTexture(texAnimHeader.targetuv, texHeader.TextureMapping[texHeader._subTextureIndexs[i]], texAnimHeader.rectuvs[(Int32)framenum], 0, 0, 0, 0);
		GL.PopMatrix();
		RenderTexture.active = active;
	}

	public static void RecreateMultiTexAnim(GEOTEXANIMHEADER texAnimHeader, GEOTEXHEADER texHeader, Int32 i)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = texHeader.RenderTexMapping[texHeader._mainTextureIndexs[i]];
		GL.PushMatrix();
		GL.Clear(true, true, Color.clear);
		GL.LoadPixelMatrix(0f, texHeader.RenderTexWidth, texHeader.RenderTexHeight, 0f);
		Graphics.DrawTexture(new Rect(0f, 0f, texHeader.RenderTexWidth, texHeader.RenderTexHeight), texHeader.TextureMapping[texHeader._mainTextureIndexs[i]]);
		GL.PopMatrix();
		RenderTexture.active = active;
	}

	public static void addTexAnim(GameObject go, String geoName)
	{
		GeoTexAnim geoTexAnim = go.AddComponent<GeoTexAnim>();
		Int32 mainTextureIndex = 1;
		Int32 subTextureIndex = 1;
		Int32 scale = 1;
		if (ModelFactory.HaveUpScaleModel(geoName))
		{
			scale = 4;
		}
		UInt16 num = BitConverter.ToUInt16(FF9StateSystem.EventState.gEventGlobal, 0);
		Boolean flag = num >= 10300;
		if (geoName.Equals("GEO_MAIN_F0_GRN") && flag)
		{
			geoName = "GEO_MAIN_F1_GRN";
		}
		GeoTexAnim.SetTexAnimIndex(geoName, out mainTextureIndex, out subTextureIndex);
		geoTexAnim.Load("Models/GeoTexAnim/" + geoName, mainTextureIndex, subTextureIndex, scale);
	}

	public static void SetTexAnimIndex(String geoName, out Int32 mainTextureIndex, out Int32 subTextureIndex)
	{
		mainTextureIndex = 0;
		subTextureIndex = 0;
		TextAsset textAsset = AssetManager.Load<TextAsset>("EmbeddedAsset/GeoTexAnimIndex/" + geoName + ".csv", false);
		if (textAsset == (UnityEngine.Object)null)
		{
			return;
		}
		String[] array = textAsset.text.Split(new Char[]
		{
			'\n'
		});
		Int32 num = (Int32)array.Length;
		Int32[,] array2 = new Int32[num - 1, num - 1];
		for (Int32 i = 1; i < num; i++)
		{
			String[] array3 = array[i].Split(new Char[]
			{
				','
			});
			String s = array3[0];
			String s2 = array3[1];
			Int32 num2 = Int32.Parse(s);
			Int32 num3 = Int32.Parse(s2);
			array2[i - 1, 0] = num2;
			array2[i - 1, 1] = num3;
		}
		mainTextureIndex = array2[0, 0];
		subTextureIndex = array2[0, 1];
	}

	public static void SetTexAnimIndexs(String geoName, out Int32[] mainTextureIndexs, out Int32[] subTextureIndexs)
	{
		TextAsset textAsset = AssetManager.Load<TextAsset>("EmbeddedAsset/GeoTexAnimIndex/" + geoName + ".csv", false);
		if (textAsset == (UnityEngine.Object)null)
		{
			mainTextureIndexs = null;
			subTextureIndexs = null;
			return;
		}
		String[] array = textAsset.text.Split(new Char[]
		{
			'\n'
		});
		Int32 num = (Int32)array.Length;
		mainTextureIndexs = new Int32[num - 1];
		subTextureIndexs = new Int32[num - 1];
		for (Int32 i = 1; i < num; i++)
		{
			String[] array2 = array[i].Split(new Char[]
			{
				','
			});
			String s = array2[0];
			String s2 = array2[1];
			Int32 num2 = Int32.Parse(s);
			Int32 num3 = Int32.Parse(s2);
			mainTextureIndexs[i - 1] = num2;
			subTextureIndexs[i - 1] = num3;
		}
	}

	public static void MapTextureToTexAnimIndex(Int32 texAnimIndex, Texture texture, Dictionary<Int32, Texture> textureMapping)
	{
		if (!textureMapping.ContainsKey(texAnimIndex))
		{
			textureMapping.Add(texAnimIndex, texture);
		}
	}

	public static void MapRenderTexToTexAnimIndex(Int32 texAnimIndex, Single textureWidth, Single textureHeight, SkinnedMeshRenderer skinnedMeshRenderer, Texture mainTexture, Dictionary<Int32, RenderTexture> renderTexMapping)
	{
		if (!renderTexMapping.ContainsKey(texAnimIndex))
		{
			RenderTexture renderTexture = new RenderTexture((Int32)textureWidth, (Int32)textureHeight, 24);
			renderTexture.filterMode = FilterMode.Bilinear;
			renderTexture.wrapMode = TextureWrapMode.Repeat;
			renderTexture.name = skinnedMeshRenderer.transform.parent.name + "_" + skinnedMeshRenderer.name + "_RT";
			skinnedMeshRenderer.material.mainTexture = renderTexture;
			renderTexMapping.Add(texAnimIndex, renderTexture);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;
			GL.PushMatrix();
			GL.Clear(true, true, Color.clear);
			GL.LoadPixelMatrix(0f, textureWidth, textureHeight, 0f);
			Graphics.DrawTexture(new Rect(0f, 0f, textureWidth, textureHeight), mainTexture);
			GL.PopMatrix();
			RenderTexture.active = active;
		}
	}

	public Int32 Count;

	public GEOTEXANIMHEADER[] Anims;

	public RenderTexture RenderTex;

	public Single RenderTexWidth;

	public Single RenderTexHeight;

	private Boolean _isLoaded;

	private Single _cTime;

	private SkinnedMeshRenderer[] _smrs;

	private Int32 _mainTextureIndex;

	private Int32 _subTextureIndex;

	private Texture _mainTexture;

	private Texture _subTexture;
}
