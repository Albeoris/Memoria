extern alias Original;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memoria;
using Memoria.TexturePackerLoader;
using UnityEngine;

#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

// ReSharper disable SuspiciousTypeConversion.Global
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
[ExportedType("³îíĨ(!!!Û[ħNĲßķÓĹÏĵ}àÿĮĦ_I¨°W:¤*+!!!¿XğĸæĨĝêD*ĥėpĪėĆVáGēë«/~#bĮěĵÞgĬôĈ×¤8!!!ĿéòíøUd¨ġĕu·¾g>eē0İĎ²·áþ¶ĽĂod=tùăİ!ÙRÏíĒS0C¡¦&Õĳß(sÂÄÏį=Z/WĸlĬTĘRĿkô0öĹČĬīČ~¸ĤĲfČēÍĂĸ«tÇ$!!!įĊ8ć#!!!½çüę*!!!ĸªČ'ĉ`ļďļxįĨ_bÔµ'į«ĂģĘa¶'ÛÈùMØĆÖ$!!!Ó¼G%¯»(ĠńńńńmŁĿeńńńń%!!!õ¬ĢŁsĖ(S>:aóńńńńńńńń")]
public class UIAtlas : MonoBehaviour
{
    [HideInInspector][SerializeField] private Material material;
    [HideInInspector][SerializeField] private List<UISpriteData> mSprites;
    [HideInInspector][SerializeField] private Single mPixelSize;
    [HideInInspector][SerializeField] private UIAtlas mReplacement;
    [HideInInspector][SerializeField] private Coordinates mCoordinates;
    [HideInInspector][SerializeField] private List<Sprite> sprites;
    [HideInInspector][SerializeField] private Int32 mPMA; // A script behaviour (probably UIAtlas?) has a different serialization layout when loading. (Read 992 bytes but expected 996 bytes)
    private Dictionary<String, Int32> mSpriteIndices;

    public Material spriteMaterial
    {
        get
        {
            if (this.mReplacement != null)
                return this.mReplacement.spriteMaterial;
            return this.material;
        }
        set
        {
            if (this.mReplacement != null)
                this.mReplacement.spriteMaterial = value;
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
                return this.mReplacement.premultipliedAlpha;
            if (this.mPMA == -1)
            {
                Material sprMat = this.spriteMaterial;
                this.mPMA = sprMat?.shader == null || !sprMat.shader.name.Contains("Premultiplied") ? 0 : 1;
            }
            return this.mPMA == 1;
        }
    }

    public List<UISpriteData> spriteList
    {
        get
        {
            if (this.mReplacement != null)
                return this.mReplacement.spriteList;
            if (this.mSprites.Count == 0)
                this.Upgrade();
            return this.mSprites;
        }
        set
        {
            if (this.mReplacement != null)
                this.mReplacement.spriteList = value;
            else
                this.mSprites = value;
        }
    }

    public Texture texture
    {
        get
        {
            if (this.mReplacement != null)
                return this.mReplacement.texture;
            return this.material?.mainTexture;
        }
    }

    public float pixelSize
    {
        get
        {
            if (this.mReplacement != null)
                return this.mReplacement.pixelSize;
            return this.mPixelSize;
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
                if (this.mPixelSize == (double)num)
                    return;
                this.mPixelSize = num;
                this.MarkAsChanged();
            }
        }
    }

    public UIAtlas replacement
    {
        get { return this.mReplacement; }
        set
        {
            UIAtlas uiAtlas = value;
            if (uiAtlas == (this))
                uiAtlas = null;
            if (!(this.mReplacement != uiAtlas))
                return;
            if (uiAtlas != null && uiAtlas.replacement == (this))
                uiAtlas.replacement = null;
            if (this.mReplacement != null)
                this.MarkAsChanged();
            this.mReplacement = uiAtlas;
            if (uiAtlas != null)
                this.material = null;
            this.MarkAsChanged();
        }
    }

    public UIAtlas()
    {
        this.mSprites = new List<UISpriteData>();
        this.mPixelSize = 1f;
        this.sprites = new List<Sprite>();
        this.mPMA = -1;
        this.mSpriteIndices = new Dictionary<string, int>();
        if (Configuration.Import.Enabled && Configuration.Import.Graphics)
            GameLoopManager.Start += OverrideAtlas;
    }

    private void OverrideAtlas()
    {
        GameLoopManager.Start -= OverrideAtlas;

        try
        {
            String inputPath = GraphicResources.Import.GetAtlasPath(name);
            String tpsheetPath = inputPath + ".tpsheet";

            if (!File.Exists(tpsheetPath))
                return;

            TPSpriteSheetLoader loader = new TPSpriteSheetLoader(tpsheetPath);
            SpriteSheet spriteSheet = loader.Load();

            Dictionary<String, UISpriteData> original = mSprites.ToDictionary(s => s.name);
            Dictionary<String, UnityEngine.Sprite> external = spriteSheet.sheet.ToDictionary(s => s.name);
            if (original.Count != external.Count)
            {
                Log.Warning("[UIAtlas] Invalid sprite number: {0}, expected: {1}", external.Count, original.Count);
                return;
            }

            Texture2D newTexture = spriteSheet.sheet[0].texture;
            foreach (KeyValuePair<String, UISpriteData> pair in original)
            {
                UISpriteData target = pair.Value;
                UnityEngine.Sprite source = external[pair.Key];

                target.x = (int)source.rect.x;
                target.y = (int)(newTexture.height - source.rect.height - source.rect.y);
                target.width = (int)source.rect.width;
                target.height = (int)source.rect.height;
            }

            mReplacement = null;
            SetTexture(newTexture);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[UIAtlas] Faield to oveeride atlas.");
        }
    }

    private void SetTexture(Texture newTexture)
    {
        if (this.mReplacement != null)
            this.mReplacement.SetTexture(newTexture);
        else if (this.material != null)
            this.material.mainTexture = newTexture;
    }

    public UISpriteData GetSprite(string spriteName)
    {
        if (this.mReplacement != null)
            return this.mReplacement.GetSprite(spriteName);
        if (!string.IsNullOrEmpty(spriteName))
        {
            if (this.mSprites.Count == 0)
                this.Upgrade();
            if (this.mSprites.Count == 0)
                return null;
            if (this.mSpriteIndices.Count != this.mSprites.Count)
                this.MarkSpriteListAsChanged();
            int index1;
            if (this.mSpriteIndices.TryGetValue(spriteName, out index1))
            {
                if (index1 > -1 && index1 < this.mSprites.Count)
                    return this.mSprites[index1];
                this.MarkSpriteListAsChanged();
                if (this.mSpriteIndices.TryGetValue(spriteName, out index1))
                    return this.mSprites[index1];
                return null;
            }
            int index2 = 0;
            for (int count = this.mSprites.Count; index2 < count; ++index2)
            {
                UISpriteData uiSpriteData = this.mSprites[index2];
                if (!string.IsNullOrEmpty(uiSpriteData.name) && spriteName == uiSpriteData.name)
                {
                    this.MarkSpriteListAsChanged();
                    return uiSpriteData;
                }
            }
        }
        return null;
    }

    public string GetRandomSprite(string startsWith)
    {
        if (this.GetSprite(startsWith) != null)
            return startsWith;
        List<UISpriteData> spritesData = this.spriteList;
        List<string> list = new List<string>();
        using (List<UISpriteData>.Enumerator enumerator = spritesData.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                UISpriteData current = enumerator.Current;
                if (current.name.StartsWith(startsWith))
                    list.Add(current.name);
            }
        }
        if (list.Count > 0)
            return list[UnityEngine.Random.Range(0, list.Count)];
        return null;
    }

    public void MarkSpriteListAsChanged()
    {
        this.mSpriteIndices.Clear();
        int index = 0;
        for (int count = this.mSprites.Count; index < count; ++index)
            this.mSpriteIndices[this.mSprites[index].name] = index;
    }

    public void SortAlphabetically()
    {
        List<UISpriteData> list = this.mSprites;
        list.Sort((s1, s2) => s1.name.CompareTo(s2.name));
    }

    public BetterList<string> GetListOfSprites()
    {
        if (this.mReplacement != null)
            return this.mReplacement.GetListOfSprites();
        if (this.mSprites.Count == 0)
            this.Upgrade();
        BetterList<string> betterList = new BetterList<string>();
        int index = 0;
        for (int count = this.mSprites.Count; index < count; ++index)
        {
            UISpriteData uiSpriteData = this.mSprites[index];
            if (!String.IsNullOrEmpty(uiSpriteData?.name))
                betterList.Add(uiSpriteData.name);
        }
        return betterList;
    }

    public BetterList<string> GetListOfSprites(string match)
    {
        if (this.mReplacement)
            return this.mReplacement.GetListOfSprites(match);
        if (string.IsNullOrEmpty(match))
            return this.GetListOfSprites();
        if (this.mSprites.Count == 0)
            this.Upgrade();
        BetterList<string> betterList = new BetterList<string>();
        int index1 = 0;
        for (int count = this.mSprites.Count; index1 < count; ++index1)
        {
            UISpriteData uiSpriteData = this.mSprites[index1];
            if (!string.IsNullOrEmpty(uiSpriteData?.name) && string.Equals(match, uiSpriteData.name, StringComparison.OrdinalIgnoreCase))
            {
                betterList.Add(uiSpriteData.name);
                return betterList;
            }
        }
        string[] strArray = match.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        for (int index2 = 0; index2 < strArray.Length; ++index2)
            strArray[index2] = strArray[index2].ToLower();
        int index3 = 0;
        for (int count = this.mSprites.Count; index3 < count; ++index3)
        {
            UISpriteData uiSpriteData = this.mSprites[index3];
            if (!string.IsNullOrEmpty(uiSpriteData?.name))
            {
                string str = uiSpriteData.name.ToLower();
                int num = 0;
                for (int index2 = 0; index2 < strArray.Length; ++index2)
                {
                    if (str.Contains(strArray[index2]))
                        ++num;
                }
                if (num == strArray.Length)
                    betterList.Add(uiSpriteData.name);
            }
        }
        return betterList;
    }

    private bool References(UIAtlas atlas)
    {
        if (atlas == null)
            return false;
        if (atlas == (this))
            return true;
        if (this.mReplacement != null)
            return this.mReplacement.References(atlas);
        return false;
    }

    public static bool CheckIfRelated(UIAtlas a, UIAtlas b)
    {
        if (a == null || b == null)
            return false;
        if (!(a == b) && !a.References(b))
            return b.References(a);
        return true;
    }

    public void MarkAsChanged()
    {
        this.mReplacement?.MarkAsChanged();
        UISprite[] active1 = NGUITools.FindActive<UISprite>();
        int index1 = 0;
        for (int length = active1.Length; index1 < length; ++index1)
        {
            UISprite uiSprite = active1[index1];
            if (CheckIfRelated(this, uiSprite.atlas))
            {
                UIAtlas atlas = uiSprite.atlas;
                uiSprite.atlas = null;
                uiSprite.atlas = atlas;
            }
        }
        UIFont[] uiFontArray = (UIFont[])Resources.FindObjectsOfTypeAll(typeof(UIFont));
        int index2 = 0;
        for (int length = uiFontArray.Length; index2 < length; ++index2)
        {
            UIFont uiFont = uiFontArray[index2];
            if (CheckIfRelated(this, (UIAtlas)(object)uiFont.atlas))
            {
                Original::UIAtlas atlas = uiFont.atlas;
                uiFont.atlas = null;
                uiFont.atlas = atlas;
            }
        }
        UILabel[] active2 = NGUITools.FindActive<UILabel>();
        int index3 = 0;
        for (int length = active2.Length; index3 < length; ++index3)
        {
            UILabel uiLabel = active2[index3];
            if (uiLabel.bitmapFont != null && CheckIfRelated(this, (UIAtlas)(object)uiLabel.bitmapFont.atlas))
            {
                UIFont bitmapFont = uiLabel.bitmapFont;
                uiLabel.bitmapFont = null;
                uiLabel.bitmapFont = bitmapFont;
            }
        }
    }

    private bool Upgrade()
    {
        if (this.mReplacement)
            return this.mReplacement.Upgrade();
        if (this.mSprites.Count != 0 || this.sprites.Count <= 0 || !this.material)
            return false;
        Texture mainTexture = this.material.mainTexture;
        int width = mainTexture?.width ?? 512;
        int height = mainTexture?.height ?? 512;
        for (int index = 0; index < this.sprites.Count; ++index)
        {
            Sprite sprite = this.sprites[index];
            Rect rect1 = sprite.outer;
            Rect rect2 = sprite.inner;
            if (this.mCoordinates == Coordinates.TexCoords)
            {
                NGUIMath.ConvertToPixels(rect1, width, height, true);
                NGUIMath.ConvertToPixels(rect2, width, height, true);
            }
            UISpriteData uiSpriteData = new UISpriteData
            {
                name = sprite.name,
                x = Mathf.RoundToInt(rect1.xMin),
                y = Mathf.RoundToInt(rect1.yMin),
                width = Mathf.RoundToInt(rect1.width),
                height = Mathf.RoundToInt(rect1.height),
                paddingLeft = Mathf.RoundToInt(sprite.paddingLeft * rect1.width),
                paddingRight = Mathf.RoundToInt(sprite.paddingRight * rect1.width),
                paddingBottom = Mathf.RoundToInt(sprite.paddingBottom * rect1.height),
                paddingTop = Mathf.RoundToInt(sprite.paddingTop * rect1.height),
                borderLeft = Mathf.RoundToInt(rect2.xMin - rect1.xMin),
                borderRight = Mathf.RoundToInt(rect1.xMax - rect2.xMax),
                borderBottom = Mathf.RoundToInt(rect1.yMax - rect2.yMax),
                borderTop = Mathf.RoundToInt(rect2.yMin - rect1.yMin)
            };
            this.mSprites.Add(uiSpriteData);
        }
        this.sprites.Clear();
        return true;
    }

    [Serializable]
    private class Sprite
    {
        public string name;
        public Rect outer;
        public Rect inner;
        public bool rotated;
        public float paddingLeft;
        public float paddingRight;
        public float paddingTop;
        public float paddingBottom;

        public bool hasPadding
        {
            get
            {
                if (this.paddingLeft == 0.0 && this.paddingRight == 0.0 && this.paddingTop == 0.0)
                    return this.paddingBottom != 0.0;
                return true;
            }
        }

        public Sprite()
        {
            this.name = "Unity Bug";
            this.outer = new Rect(0.0f, 0.0f, 1f, 1f);
            this.inner = new Rect(0.0f, 0.0f, 1f, 1f);
        }
    }

    private enum Coordinates
    {
        Pixels,
        TexCoords,
    }
}