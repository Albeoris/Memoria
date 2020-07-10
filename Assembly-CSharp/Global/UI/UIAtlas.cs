using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memoria;
using Memoria.Assets;
using Memoria.Assets.TexturePacker;
using Memoria.Prime;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Atlas")]
public class UIAtlas : MonoBehaviour
{
    public UIAtlas()
    {
		GameLoopManager.Start += OverrideAtlas;
	}

	private void OverrideAtlas()
	{
		GameLoopManager.Start -= OverrideAtlas;
		if (Configuration.Import.Enabled && Configuration.Import.Graphics)
		{
			ReadFromDisc(GraphicResources.Import.GetAtlasPath(name));
		}
		else if (GraphicResources.AtlasList.ContainsKey(name))
		{
			String[] modPath = Configuration.Mod.FolderNames;
			String atlasFilePath = GraphicResources.AtlasList[name];
			for (Int32 i = 0; i < modPath.Length; i++)
				if (File.Exists(modPath[i] + "/" + AssetManagerUtil.GetResourcesAssetsPath(true) + "/" + atlasFilePath) || File.Exists(modPath[i] + "/" + AssetManagerUtil.GetResourcesAssetsPath(true) + "/" + atlasFilePath + ".tpsheet"))
				{
					ReadFromDisc(modPath[i] + "/" + AssetManagerUtil.GetResourcesAssetsPath(true) + "/" + atlasFilePath);
					return;
				}
		}
	}

	public Boolean ReadFromDisc(String inputPath)
	{
		try
		{
			String tpsheetPath = inputPath + ".tpsheet";

			if (File.Exists(inputPath) && !File.Exists(tpsheetPath))
			{
				Byte[] raw = File.ReadAllBytes(inputPath);
				Texture2D newFullAtlas = new Texture2D(1, 1, AssetManager.DefaultTextureFormat, false);
				newFullAtlas.LoadImage(raw);
				SetTexture(newFullAtlas);
				return true;
			}

			if (!File.Exists(tpsheetPath))
				return false;

			TPSpriteSheetLoader loader = new TPSpriteSheetLoader(tpsheetPath);
			SpriteSheet spriteSheet = loader.Load();

			Dictionary<String, UISpriteData> original = mSprites.ToDictionary(s => s.name);
			Dictionary<String, UnityEngine.Sprite> external = spriteSheet.sheet.ToDictionary(s => s.name);
			if (original.Count != external.Count)
			{
				Log.Warning("[UIAtlas] Invalid sprite number: {0}, expected: {1}", external.Count, original.Count);
				return false;
			}

			Texture2D newTexture = spriteSheet.sheet[0].texture;
			foreach (KeyValuePair<String, UISpriteData> pair in original)
			{
				UISpriteData target = pair.Value;
				UnityEngine.Sprite source = external[pair.Key];

				target.x = (Int32)source.rect.x;
				target.y = (Int32)(newTexture.height - source.rect.height - source.rect.y);
				target.width = (Int32)source.rect.width;
				target.height = (Int32)source.rect.height;
			}

			mReplacement = null;
			SetTexture(newTexture);
			return true;
		}
		catch (Exception ex)
		{
			Log.Error(ex, "[UIAtlas] Failed to overide atlas.");
			return false;
		}
	}

    private void SetTexture(Texture newTexture)
    {
        if (this.mReplacement != null)
            this.mReplacement.SetTexture(newTexture);
        else if (this.material != null)
            this.material.mainTexture = newTexture;
    }

    public Material spriteMaterial
	{
		get
		{
			return (!(this.mReplacement != (UnityEngine.Object)null)) ? this.material : this.mReplacement.spriteMaterial;
		}
		set
		{
			if (this.mReplacement != (UnityEngine.Object)null)
			{
				this.mReplacement.spriteMaterial = value;
			}
			else if (this.material == (UnityEngine.Object)null)
			{
				this.mPMA = 0;
				this.material = value;
			}
			else
			{
				this.MarkAsChanged();
				this.mPMA = -1;
				this.material = value;
				this.MarkAsChanged();
			}
		}
	}

	public Boolean premultipliedAlpha
	{
		get
		{
			if (this.mReplacement != (UnityEngine.Object)null)
			{
				return this.mReplacement.premultipliedAlpha;
			}
			if (this.mPMA == -1)
			{
				Material spriteMaterial = this.spriteMaterial;
				this.mPMA = (Int32)((!(spriteMaterial != (UnityEngine.Object)null) || !(spriteMaterial.shader != (UnityEngine.Object)null) || !spriteMaterial.shader.name.Contains("Premultiplied")) ? 0 : 1);
			}
			return this.mPMA == 1;
		}
	}

	public List<UISpriteData> spriteList
	{
		get
		{
			if (this.mReplacement != (UnityEngine.Object)null)
			{
				return this.mReplacement.spriteList;
			}
			if (this.mSprites.Count == 0)
			{
				this.Upgrade();
			}
			return this.mSprites;
		}
		set
		{
			if (this.mReplacement != (UnityEngine.Object)null)
			{
				this.mReplacement.spriteList = value;
			}
			else
			{
				this.mSprites = value;
			}
		}
	}

	public Texture texture
	{
		get
		{
			return (!(this.mReplacement != (UnityEngine.Object)null)) ? ((!(this.material != (UnityEngine.Object)null)) ? null : this.material.mainTexture) : this.mReplacement.texture;
		}
	}

	public Single pixelSize
	{
		get
		{
			return (!(this.mReplacement != (UnityEngine.Object)null)) ? this.mPixelSize : this.mReplacement.pixelSize;
		}
		set
		{
			if (this.mReplacement != (UnityEngine.Object)null)
			{
				this.mReplacement.pixelSize = value;
			}
			else
			{
				Single num = Mathf.Clamp(value, 0.25f, 4f);
				if (this.mPixelSize != num)
				{
					this.mPixelSize = num;
					this.MarkAsChanged();
				}
			}
		}
	}

	public UIAtlas replacement
	{
		get
		{
			return this.mReplacement;
		}
		set
		{
			UIAtlas uiatlas = value;
			if (uiatlas == this)
			{
				uiatlas = (UIAtlas)null;
			}
			if (this.mReplacement != uiatlas)
			{
				if (uiatlas != (UnityEngine.Object)null && uiatlas.replacement == this)
				{
					uiatlas.replacement = (UIAtlas)null;
				}
				if (this.mReplacement != (UnityEngine.Object)null)
				{
					this.MarkAsChanged();
				}
				this.mReplacement = uiatlas;
				if (uiatlas != (UnityEngine.Object)null)
				{
					this.material = (Material)null;
				}
				this.MarkAsChanged();
			}
		}
	}

	public UISpriteData GetSprite(String name)
	{
		if (this.mReplacement != (UnityEngine.Object)null)
		{
			return this.mReplacement.GetSprite(name);
		}
		if (!String.IsNullOrEmpty(name))
		{
			if (this.mSprites.Count == 0)
			{
				this.Upgrade();
			}
			if (this.mSprites.Count == 0)
			{
				return (UISpriteData)null;
			}
			if (this.mSpriteIndices.Count != this.mSprites.Count)
			{
				this.MarkSpriteListAsChanged();
			}
			Int32 num;
			if (this.mSpriteIndices.TryGetValue(name, out num))
			{
				if (num > -1 && num < this.mSprites.Count)
				{
					return this.mSprites[num];
				}
				this.MarkSpriteListAsChanged();
				return (!this.mSpriteIndices.TryGetValue(name, out num)) ? null : this.mSprites[num];
			}
			else
			{
				Int32 i = 0;
				Int32 count = this.mSprites.Count;
				while (i < count)
				{
					UISpriteData uispriteData = this.mSprites[i];
					if (!String.IsNullOrEmpty(uispriteData.name) && name == uispriteData.name)
					{
						this.MarkSpriteListAsChanged();
						return uispriteData;
					}
					i++;
				}
			}
		}
		return (UISpriteData)null;
	}

	public String GetRandomSprite(String startsWith)
	{
		if (this.GetSprite(startsWith) == null)
		{
			List<UISpriteData> spriteList = this.spriteList;
			List<String> list = new List<String>();
			foreach (UISpriteData uispriteData in spriteList)
			{
				if (uispriteData.name.StartsWith(startsWith))
				{
					list.Add(uispriteData.name);
				}
			}
			return (list.Count <= 0) ? null : list[UnityEngine.Random.Range(0, list.Count)];
		}
		return startsWith;
	}

	public void MarkSpriteListAsChanged()
	{
		this.mSpriteIndices.Clear();
		Int32 i = 0;
		Int32 count = this.mSprites.Count;
		while (i < count)
		{
			this.mSpriteIndices[this.mSprites[i].name] = i;
			i++;
		}
	}

	public void SortAlphabetically()
	{
		this.mSprites.Sort((UISpriteData s1, UISpriteData s2) => s1.name.CompareTo(s2.name));
	}

	public BetterList<String> GetListOfSprites()
	{
		if (this.mReplacement != (UnityEngine.Object)null)
		{
			return this.mReplacement.GetListOfSprites();
		}
		if (this.mSprites.Count == 0)
		{
			this.Upgrade();
		}
		BetterList<String> betterList = new BetterList<String>();
		Int32 i = 0;
		Int32 count = this.mSprites.Count;
		while (i < count)
		{
			UISpriteData uispriteData = this.mSprites[i];
			if (uispriteData != null && !String.IsNullOrEmpty(uispriteData.name))
			{
				betterList.Add(uispriteData.name);
			}
			i++;
		}
		return betterList;
	}

	public BetterList<String> GetListOfSprites(String match)
	{
		if (this.mReplacement)
		{
			return this.mReplacement.GetListOfSprites(match);
		}
		if (String.IsNullOrEmpty(match))
		{
			return this.GetListOfSprites();
		}
		if (this.mSprites.Count == 0)
		{
			this.Upgrade();
		}
		BetterList<String> betterList = new BetterList<String>();
		Int32 i = 0;
		Int32 count = this.mSprites.Count;
		while (i < count)
		{
			UISpriteData uispriteData = this.mSprites[i];
			if (uispriteData != null && !String.IsNullOrEmpty(uispriteData.name) && String.Equals(match, uispriteData.name, StringComparison.OrdinalIgnoreCase))
			{
				betterList.Add(uispriteData.name);
				return betterList;
			}
			i++;
		}
		String[] array = match.Split(new Char[]
		{
			' '
		}, StringSplitOptions.RemoveEmptyEntries);
		for (Int32 j = 0; j < (Int32)array.Length; j++)
		{
			array[j] = array[j].ToLower();
		}
		Int32 k = 0;
		Int32 count2 = this.mSprites.Count;
		while (k < count2)
		{
			UISpriteData uispriteData2 = this.mSprites[k];
			if (uispriteData2 != null && !String.IsNullOrEmpty(uispriteData2.name))
			{
				String text = uispriteData2.name.ToLower();
				Int32 num = 0;
				for (Int32 l = 0; l < (Int32)array.Length; l++)
				{
					if (text.Contains(array[l]))
					{
						num++;
					}
				}
				if (num == (Int32)array.Length)
				{
					betterList.Add(uispriteData2.name);
				}
			}
			k++;
		}
		return betterList;
	}

	private Boolean References(UIAtlas atlas)
	{
		return !(atlas == (UnityEngine.Object)null) && (atlas == this || (this.mReplacement != (UnityEngine.Object)null && this.mReplacement.References(atlas)));
	}

	public static Boolean CheckIfRelated(UIAtlas a, UIAtlas b)
	{
		return !(a == (UnityEngine.Object)null) && !(b == (UnityEngine.Object)null) && (a == b || a.References(b) || b.References(a));
	}

	public void MarkAsChanged()
	{
		if (this.mReplacement != (UnityEngine.Object)null)
		{
			this.mReplacement.MarkAsChanged();
		}
		UISprite[] array = NGUITools.FindActive<UISprite>();
		Int32 i = 0;
		Int32 num = (Int32)array.Length;
		while (i < num)
		{
			UISprite uisprite = array[i];
			if (UIAtlas.CheckIfRelated(this, uisprite.atlas))
			{
				UIAtlas atlas = uisprite.atlas;
				uisprite.atlas = (UIAtlas)null;
				uisprite.atlas = atlas;
			}
			i++;
		}
		UIFont[] array2 = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];
		Int32 j = 0;
		Int32 num2 = (Int32)array2.Length;
		while (j < num2)
		{
			UIFont uifont = array2[j];
			if (UIAtlas.CheckIfRelated(this, uifont.atlas))
			{
				UIAtlas atlas2 = uifont.atlas;
				uifont.atlas = (UIAtlas)null;
				uifont.atlas = atlas2;
			}
			j++;
		}
		UILabel[] array3 = NGUITools.FindActive<UILabel>();
		Int32 k = 0;
		Int32 num3 = (Int32)array3.Length;
		while (k < num3)
		{
			UILabel uilabel = array3[k];
			if (uilabel.bitmapFont != (UnityEngine.Object)null && UIAtlas.CheckIfRelated(this, uilabel.bitmapFont.atlas))
			{
				UIFont bitmapFont = uilabel.bitmapFont;
				uilabel.bitmapFont = (UIFont)null;
				uilabel.bitmapFont = bitmapFont;
			}
			k++;
		}
	}

	private Boolean Upgrade()
	{
		if (this.mReplacement)
		{
			return this.mReplacement.Upgrade();
		}
		if (this.mSprites.Count == 0 && this.sprites.Count > 0 && this.material)
		{
			Texture mainTexture = this.material.mainTexture;
			Int32 width = (Int32)((!(mainTexture != (UnityEngine.Object)null)) ? 512 : mainTexture.width);
			Int32 height = (Int32)((!(mainTexture != (UnityEngine.Object)null)) ? 512 : mainTexture.height);
			for (Int32 i = 0; i < this.sprites.Count; i++)
			{
				UIAtlas.Sprite sprite = this.sprites[i];
				Rect outer = sprite.outer;
				Rect inner = sprite.inner;
				if (this.mCoordinates == UIAtlas.Coordinates.TexCoords)
				{
					NGUIMath.ConvertToPixels(outer, width, height, true);
					NGUIMath.ConvertToPixels(inner, width, height, true);
				}
				UISpriteData uispriteData = new UISpriteData();
				uispriteData.name = sprite.name;
				uispriteData.x = Mathf.RoundToInt(outer.xMin);
				uispriteData.y = Mathf.RoundToInt(outer.yMin);
				uispriteData.width = Mathf.RoundToInt(outer.width);
				uispriteData.height = Mathf.RoundToInt(outer.height);
				uispriteData.paddingLeft = Mathf.RoundToInt(sprite.paddingLeft * outer.width);
				uispriteData.paddingRight = Mathf.RoundToInt(sprite.paddingRight * outer.width);
				uispriteData.paddingBottom = Mathf.RoundToInt(sprite.paddingBottom * outer.height);
				uispriteData.paddingTop = Mathf.RoundToInt(sprite.paddingTop * outer.height);
				uispriteData.borderLeft = Mathf.RoundToInt(inner.xMin - outer.xMin);
				uispriteData.borderRight = Mathf.RoundToInt(outer.xMax - inner.xMax);
				uispriteData.borderBottom = Mathf.RoundToInt(outer.yMax - inner.yMax);
				uispriteData.borderTop = Mathf.RoundToInt(inner.yMin - outer.yMin);
				this.mSprites.Add(uispriteData);
			}
			this.sprites.Clear();
			return true;
		}
		return false;
	}

	[HideInInspector]
	[SerializeField]
	private Material material;

	[HideInInspector]
	[SerializeField]
	private List<UISpriteData> mSprites = new List<UISpriteData>();

	[SerializeField]
	[HideInInspector]
	private Single mPixelSize = 1f;

	[SerializeField]
	[HideInInspector]
	private UIAtlas mReplacement;

	[HideInInspector]
	[SerializeField]
	private UIAtlas.Coordinates mCoordinates;

	[HideInInspector]
	[SerializeField]
	private List<UIAtlas.Sprite> sprites = new List<UIAtlas.Sprite>();

	private Int32 mPMA = -1;

	private Dictionary<String, Int32> mSpriteIndices = new Dictionary<String, Int32>();

	[Serializable]
	private class Sprite
	{
		public Boolean hasPadding
		{
			get
			{
				return this.paddingLeft != 0f || this.paddingRight != 0f || this.paddingTop != 0f || this.paddingBottom != 0f;
			}
		}

		public String name = "Unity Bug";

		public Rect outer = new Rect(0f, 0f, 1f, 1f);

		public Rect inner = new Rect(0f, 0f, 1f, 1f);

		public Boolean rotated;

		public Single paddingLeft;

		public Single paddingRight;

		public Single paddingTop;

		public Single paddingBottom;
	}

	private enum Coordinates
	{
		Pixels,
		TexCoords
	}
}
