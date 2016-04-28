extern alias Original;

using System;
using System.Collections.Generic;
using System.IO;
using Memoria;
using UnityEngine;
using Object = UnityEngine.Object;

#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

// ReSharper disable ArrangeThisQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToConstant.Global
// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming

[AddComponentMenu("NGUI/UI/Atlas")]
public class UIAtlas : MonoBehaviour
{
    [Serializable]
    private class Sprite
    {
        public string name = "Unity Bug";

        public Rect outer = new Rect(0f, 0f, 1f, 1f);

        public Rect inner = new Rect(0f, 0f, 1f, 1f);

        public bool rotated;

        public float paddingLeft;

        public float paddingRight;

        public float paddingTop;

        public float paddingBottom;

        public bool hasPadding => this.paddingLeft != 0f || this.paddingRight != 0f || this.paddingTop != 0f || this.paddingBottom != 0f;
    }

    private enum Coordinates
    {
        Pixels,
        TexCoords
    }

    [HideInInspector, SerializeField]
    private Material material;

    [HideInInspector, SerializeField]
    private List<UISpriteData> mSprites = new List<UISpriteData>();

    [HideInInspector, SerializeField]
    private float mPixelSize = 1f;

    [HideInInspector, SerializeField]
    private UIAtlas mReplacement;

    [HideInInspector, SerializeField]
    private Coordinates mCoordinates;

    [HideInInspector, SerializeField]
    private List<Sprite> sprites = new List<Sprite>();

    private int mPMA = -1;

    private Dictionary<string, int> mSpriteIndices = new Dictionary<string, int>();

    public UIAtlas()
    {
        //Log.Message("UIAtlas constucted: {0}", name);
        //GameLoopManager.Start += OnStart;
    }

    private void OnStart()
    {
        //Log.Message("UIAtlas started: {0}", name);
        //if (material)
        //{
        //    Log.Message("UIAtlas material: {0}", material.mainTexture.name);
        //    TextureResources.WriteTextureToFile(material.mainTexture as Texture2D, Path.Combine(Configuration.Export.Path, material.mainTexture.name + ".png"));
        //}
    }

    public Material spriteMaterial
    {
        get
        {
            return !(this.mReplacement != null) ? this.material : this.mReplacement.spriteMaterial;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.spriteMaterial = value;
            }
            else if (this.material == null)
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

    public bool premultipliedAlpha
    {
        get
        {
            if (this.mReplacement != null)
            {
                return this.mReplacement.premultipliedAlpha;
            }
            if (this.mPMA == -1)
            {
                Material sprMaterial = this.spriteMaterial;
                this.mPMA = ((sprMaterial?.shader == null || !sprMaterial.shader.name.Contains("Premultiplied")) ? 0 : 1);
            }
            return this.mPMA == 1;
        }
    }

    public List<UISpriteData> spriteList
    {
        get
        {
            if (this.mReplacement != null)
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
            if (this.mReplacement != null)
            {
                this.mReplacement.spriteList = value;
            }
            else
            {
                this.mSprites = value;
            }
        }
    }

    public Texture texture => (!(this.mReplacement != null)) ? this.material?.mainTexture : this.mReplacement.texture;

    public float pixelSize
    {
        get
        {
            return this.mReplacement?.pixelSize ?? this.mPixelSize;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.pixelSize = value;
            }
            else
            {
                float num = Mathf.Clamp(value, 0.25f, 4f);
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
            UIAtlas uIAtlas = value;
            if (uIAtlas == this)
            {
                uIAtlas = null;
            }
            if (this.mReplacement != uIAtlas)
            {
                if (uIAtlas != null && uIAtlas.replacement == this)
                {
                    uIAtlas.replacement = null;
                }
                if (this.mReplacement != null)
                {
                    this.MarkAsChanged();
                }
                this.mReplacement = uIAtlas;
                if (uIAtlas != null)
                {
                    this.material = null;
                }
                this.MarkAsChanged();
            }
        }
    }

    public UISpriteData GetSprite(string spriteName)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.GetSprite(spriteName);
        }
        if (!string.IsNullOrEmpty(spriteName))
        {
            if (this.mSprites.Count == 0)
            {
                this.Upgrade();
            }
            if (this.mSprites.Count == 0)
            {
                return null;
            }
            if (this.mSpriteIndices.Count != this.mSprites.Count)
            {
                this.MarkSpriteListAsChanged();
            }
            int num;
            if (this.mSpriteIndices.TryGetValue(spriteName, out num))
            {
                if (num > -1 && num < this.mSprites.Count)
                {
                    return this.mSprites[num];
                }
                this.MarkSpriteListAsChanged();
                return (!this.mSpriteIndices.TryGetValue(spriteName, out num)) ? null : this.mSprites[num];
            }
            else
            {
                int i = 0;
                int count = this.mSprites.Count;
                while (i < count)
                {
                    UISpriteData uISpriteData = this.mSprites[i];
                    if (!string.IsNullOrEmpty(uISpriteData.name) && spriteName == uISpriteData.name)
                    {
                        this.MarkSpriteListAsChanged();
                        return uISpriteData;
                    }
                    i++;
                }
            }
        }
        return null;
    }

    public string GetRandomSprite(string startsWith)
    {
        if (this.GetSprite(startsWith) != null)
            return startsWith;

        List<string> list = new List<string>();
        foreach (UISpriteData current in this.spriteList)
        {
            if (current.name.StartsWith(startsWith))
            {
                list.Add(current.name);
            }
        }
        return (list.Count <= 0) ? null : list[UnityEngine.Random.Range(0, list.Count)];
    }

    public void MarkSpriteListAsChanged()
    {
        this.mSpriteIndices.Clear();
        int i = 0;
        int count = this.mSprites.Count;
        while (i < count)
        {
            this.mSpriteIndices[this.mSprites[i].name] = i;
            i++;
        }
    }

    public void SortAlphabetically()
    {
        this.mSprites.Sort((s1, s2) => s1.name.CompareTo(s2.name));
    }

    public BetterList<string> GetListOfSprites()
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.GetListOfSprites();
        }
        if (this.mSprites.Count == 0)
        {
            this.Upgrade();
        }
        BetterList<string> betterList = new BetterList<string>();
        int i = 0;
        int count = this.mSprites.Count;
        while (i < count)
        {
            UISpriteData uISpriteData = this.mSprites[i];
            if (!string.IsNullOrEmpty(uISpriteData?.name))
            {
                betterList.Add(uISpriteData.name);
            }
            i++;
        }
        return betterList;
    }

    public BetterList<string> GetListOfSprites(string match)
    {
        if (this.mReplacement)
        {
            return this.mReplacement.GetListOfSprites(match);
        }
        if (string.IsNullOrEmpty(match))
        {
            return this.GetListOfSprites();
        }
        if (this.mSprites.Count == 0)
        {
            this.Upgrade();
        }
        BetterList<string> betterList = new BetterList<string>();
        int i = 0;
        int count = this.mSprites.Count;
        while (i < count)
        {
            UISpriteData uISpriteData = this.mSprites[i];
            if (!string.IsNullOrEmpty(uISpriteData?.name) && string.Equals(match, uISpriteData.name, StringComparison.OrdinalIgnoreCase))
            {
                betterList.Add(uISpriteData.name);
                return betterList;
            }
            i++;
        }
        string[] array = match.Split(new[]
        {
            ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        for (int j = 0; j < array.Length; j++)
        {
            array[j] = array[j].ToLower();
        }
        int k = 0;
        int count2 = this.mSprites.Count;
        while (k < count2)
        {
            UISpriteData uISpriteData2 = this.mSprites[k];
            if (!string.IsNullOrEmpty(uISpriteData2?.name))
            {
                string text = uISpriteData2.name.ToLower();
                int num = 0;
                for (int l = 0; l < array.Length; l++)
                {
                    if (text.Contains(array[l]))
                    {
                        num++;
                    }
                }
                if (num == array.Length)
                {
                    betterList.Add(uISpriteData2.name);
                }
            }
            k++;
        }
        return betterList;
    }

    private bool References(UIAtlas atlas)
    {
        return !(atlas == null) && (atlas == this || (this.mReplacement != null && this.mReplacement.References(atlas)));
    }

    public static bool CheckIfRelated(UIAtlas a, UIAtlas b)
    {
        return !(a == null) && !(b == null) && (a == b || a.References(b) || b.References(a));
    }

    public void MarkAsChanged()
    {
        this.mReplacement?.MarkAsChanged();
        UISprite[] array = NGUITools.FindActive<UISprite>();
        int i = 0;
        int num = array.Length;
        while (i < num)
        {
            UISprite uISprite = array[i];
            if (CheckIfRelated(this, uISprite.atlas))
            {
                UIAtlas atlas = uISprite.atlas;
                uISprite.atlas = null;
                uISprite.atlas = atlas;
            }
            i++;
        }
        UIFont[] array2 = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];
        int j = 0;
        if (array2 != null)
        {
            int num2 = array2.Length;
            while (j < num2)
            {
                UIFont uIFont = array2[j];
                if (CheckIfRelated(this, (UIAtlas)(Object)uIFont.atlas))
                {
                    Original::UIAtlas atlas2 = uIFont.atlas;
                    uIFont.atlas = null;
                    uIFont.atlas = atlas2;
                }
                j++;
            }
        }
        UILabel[] array3 = NGUITools.FindActive<UILabel>();
        int k = 0;
        int num3 = array3.Length;
        while (k < num3)
        {
            UILabel uILabel = array3[k];
            if (uILabel.bitmapFont != null && CheckIfRelated(this, (UIAtlas)(Object)uILabel.bitmapFont.atlas))
            {
                UIFont bitmapFont = uILabel.bitmapFont;
                uILabel.bitmapFont = null;
                uILabel.bitmapFont = bitmapFont;
            }
            k++;
        }
    }

    private bool Upgrade()
    {
        if (this.mReplacement)
        {
            return this.mReplacement.Upgrade();
        }
        if (this.mSprites.Count == 0 && this.sprites.Count > 0 && this.material)
        {
            Texture mainTexture = this.material.mainTexture;
            int width = mainTexture?.width ?? 512;
            int height = mainTexture?.height ?? 512;
            for (int i = 0; i < this.sprites.Count; i++)
            {
                Sprite sprite = this.sprites[i];
                Rect outer = sprite.outer;
                Rect inner = sprite.inner;
                if (this.mCoordinates == Coordinates.TexCoords)
                {
                    NGUIMath.ConvertToPixels(outer, width, height, true);
                    NGUIMath.ConvertToPixels(inner, width, height, true);
                }
                UISpriteData uISpriteData = new UISpriteData
                {
                    name = sprite.name,
                    x = Mathf.RoundToInt(outer.xMin),
                    y = Mathf.RoundToInt(outer.yMin),
                    width = Mathf.RoundToInt(outer.width),
                    height = Mathf.RoundToInt(outer.height),
                    paddingLeft = Mathf.RoundToInt(sprite.paddingLeft * outer.width),
                    paddingRight = Mathf.RoundToInt(sprite.paddingRight * outer.width),
                    paddingBottom = Mathf.RoundToInt(sprite.paddingBottom * outer.height),
                    paddingTop = Mathf.RoundToInt(sprite.paddingTop * outer.height),
                    borderLeft = Mathf.RoundToInt(inner.xMin - outer.xMin),
                    borderRight = Mathf.RoundToInt(outer.xMax - inner.xMax),
                    borderBottom = Mathf.RoundToInt(outer.yMax - inner.yMax),
                    borderTop = Mathf.RoundToInt(inner.yMin - outer.yMin)
                };
                this.mSprites.Add(uISpriteData);
            }
            this.sprites.Clear();
            return true;
        }
        return false;
    }
}
