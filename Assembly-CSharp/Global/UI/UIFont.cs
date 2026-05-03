using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Font")]
[ExecuteInEditMode]
public class UIFont : MonoBehaviour
{
    public BMFont bmFont
    {
        get
        {
            return (!(this.mReplacement != (UnityEngine.Object)null)) ? this.mFont : this.mReplacement.bmFont;
        }
        set
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                this.mReplacement.bmFont = value;
            }
            else
            {
                this.mFont = value;
            }
        }
    }

    public Int32 texWidth
    {
        get
        {
            return (Int32)((!(this.mReplacement != (UnityEngine.Object)null)) ? ((Int32)((this.mFont == null) ? 1 : this.mFont.texWidth)) : this.mReplacement.texWidth);
        }
        set
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                this.mReplacement.texWidth = value;
            }
            else if (this.mFont != null)
            {
                this.mFont.texWidth = value;
            }
        }
    }

    public Int32 texHeight
    {
        get
        {
            return (Int32)((!(this.mReplacement != (UnityEngine.Object)null)) ? ((Int32)((this.mFont == null) ? 1 : this.mFont.texHeight)) : this.mReplacement.texHeight);
        }
        set
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                this.mReplacement.texHeight = value;
            }
            else if (this.mFont != null)
            {
                this.mFont.texHeight = value;
            }
        }
    }

    public Boolean hasSymbols
    {
        get
        {
            return (!(this.mReplacement != (UnityEngine.Object)null)) ? (this.mSymbols != null && this.mSymbols.Count != 0) : this.mReplacement.hasSymbols;
        }
    }

    public List<BMSymbol> symbols
    {
        get
        {
            return (!(this.mReplacement != (UnityEngine.Object)null)) ? this.mSymbols : this.mReplacement.symbols;
        }
    }

    public UIAtlas atlas
    {
        get
        {
            return (!(this.mReplacement != (UnityEngine.Object)null)) ? this.mAtlas : this.mReplacement.atlas;
        }
        set
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                this.mReplacement.atlas = value;
            }
            else if (this.mAtlas != value)
            {
                this.mPMA = -1;
                this.mAtlas = value;
                if (this.mAtlas != (UnityEngine.Object)null)
                {
                    this.mMat = this.mAtlas.spriteMaterial;
                    if (this.sprite != null)
                    {
                        this.mUVRect = this.uvRect;
                    }
                }
                this.MarkAsChanged();
            }
        }
    }

    public Material material
    {
        get
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                return this.mReplacement.material;
            }
            if (this.mAtlas != (UnityEngine.Object)null)
            {
                return this.mAtlas.spriteMaterial;
            }
            if (this.mMat != (UnityEngine.Object)null)
            {
                if (this.mDynamicFont != (UnityEngine.Object)null && this.mMat != this.mDynamicFont.material)
                {
                    this.mMat.mainTexture = this.mDynamicFont.material.mainTexture;
                }
                return this.mMat;
            }
            if (this.mDynamicFont != (UnityEngine.Object)null)
            {
                return this.mDynamicFont.material;
            }
            return (Material)null;
        }
        set
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                this.mReplacement.material = value;
            }
            else if (this.mMat != value)
            {
                this.mPMA = -1;
                this.mMat = value;
                this.MarkAsChanged();
            }
        }
    }

    [Obsolete("Use UIFont.premultipliedAlphaShader instead")]
    public Boolean premultipliedAlpha
    {
        get
        {
            return this.premultipliedAlphaShader;
        }
    }

    public Boolean premultipliedAlphaShader
    {
        get
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                return this.mReplacement.premultipliedAlphaShader;
            }
            if (this.mAtlas != (UnityEngine.Object)null)
            {
                return this.mAtlas.premultipliedAlpha;
            }
            if (this.mPMA == -1)
            {
                Material material = this.material;
                this.mPMA = (Int32)((!(material != (UnityEngine.Object)null) || !(material.shader != (UnityEngine.Object)null) || !material.shader.name.Contains("Premultiplied")) ? 0 : 1);
            }
            return this.mPMA == 1;
        }
    }

    public Boolean packedFontShader
    {
        get
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                return this.mReplacement.packedFontShader;
            }
            if (this.mAtlas != (UnityEngine.Object)null)
            {
                return false;
            }
            if (this.mPacked == -1)
            {
                Material material = this.material;
                this.mPacked = (Int32)((!(material != (UnityEngine.Object)null) || !(material.shader != (UnityEngine.Object)null) || !material.shader.name.Contains("Packed")) ? 0 : 1);
            }
            return this.mPacked == 1;
        }
    }

    public Texture2D texture
    {
        get
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                return this.mReplacement.texture;
            }
            Material material = this.material;
            return (!(material != (UnityEngine.Object)null)) ? null : (material.mainTexture as Texture2D);
        }
    }

    public Rect uvRect
    {
        get
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                return this.mReplacement.uvRect;
            }
            return (!(this.mAtlas != (UnityEngine.Object)null) || this.sprite == null) ? new Rect(0f, 0f, 1f, 1f) : this.mUVRect;
        }
        set
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                this.mReplacement.uvRect = value;
            }
            else if (this.sprite == null && this.mUVRect != value)
            {
                this.mUVRect = value;
                this.MarkAsChanged();
            }
        }
    }

    public String spriteName
    {
        get
        {
            return (!(this.mReplacement != (UnityEngine.Object)null)) ? this.mFont.spriteName : this.mReplacement.spriteName;
        }
        set
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                this.mReplacement.spriteName = value;
            }
            else if (this.mFont.spriteName != value)
            {
                this.mFont.spriteName = value;
                this.MarkAsChanged();
            }
        }
    }

    public Boolean isValid
    {
        get
        {
            return this.mDynamicFont != (UnityEngine.Object)null || this.mFont.isValid;
        }
    }

    [Obsolete("Use UIFont.defaultSize instead")]
    public Int32 size
    {
        get
        {
            return this.defaultSize;
        }
        set
        {
            this.defaultSize = value;
        }
    }

    public Int32 defaultSize
    {
        get
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                return this.mReplacement.defaultSize;
            }
            if (this.isDynamic || this.mFont == null)
            {
                return this.mDynamicFontSize;
            }
            return this.mFont.charSize;
        }
        set
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                this.mReplacement.defaultSize = value;
            }
            else
            {
                this.mDynamicFontSize = value;
            }
        }
    }

    public UISpriteData sprite
    {
        get
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                return this.mReplacement.sprite;
            }
            if (this.mSprite == null && this.mAtlas != (UnityEngine.Object)null && !String.IsNullOrEmpty(this.mFont.spriteName))
            {
                this.mSprite = this.mAtlas.GetSprite(this.mFont.spriteName);
                if (this.mSprite == null)
                {
                    this.mSprite = this.mAtlas.GetSprite(base.name);
                }
                if (this.mSprite == null)
                {
                    this.mFont.spriteName = (String)null;
                }
                else
                {
                    this.UpdateUVRect();
                }
                Int32 i = 0;
                Int32 count = this.mSymbols.Count;
                while (i < count)
                {
                    this.symbols[i].MarkAsChanged();
                    i++;
                }
            }
            return this.mSprite;
        }
    }

    public UIFont replacement
    {
        get
        {
            return this.mReplacement;
        }
        set
        {
            UIFont uifont = value;
            if (uifont == this)
            {
                uifont = (UIFont)null;
            }
            if (this.mReplacement != uifont)
            {
                if (uifont != (UnityEngine.Object)null && uifont.replacement == this)
                {
                    uifont.replacement = (UIFont)null;
                }
                if (this.mReplacement != (UnityEngine.Object)null)
                {
                    this.MarkAsChanged();
                }
                this.mReplacement = uifont;
                if (uifont != (UnityEngine.Object)null)
                {
                    this.mPMA = -1;
                    this.mMat = (Material)null;
                    this.mFont = (BMFont)null;
                    this.mDynamicFont = (Font)null;
                }
                this.MarkAsChanged();
            }
        }
    }

    public Boolean isDynamic
    {
        get
        {
            return (!(this.mReplacement != (UnityEngine.Object)null)) ? (this.mDynamicFont != (UnityEngine.Object)null) : this.mReplacement.isDynamic;
        }
    }

    public Font dynamicFont
    {
        get
        {
            return (!(this.mReplacement != (UnityEngine.Object)null)) ? this.mDynamicFont : this.mReplacement.dynamicFont;
        }
        set
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                this.mReplacement.dynamicFont = value;
            }
            else if (this.mDynamicFont != value)
            {
                if (this.mDynamicFont != (UnityEngine.Object)null)
                {
                    this.material = (Material)null;
                }
                this.mDynamicFont = value;
                this.MarkAsChanged();
            }
        }
    }

    public FontStyle dynamicFontStyle
    {
        get
        {
            return (FontStyle)((!(this.mReplacement != (UnityEngine.Object)null)) ? this.mDynamicFontStyle : this.mReplacement.dynamicFontStyle);
        }
        set
        {
            if (this.mReplacement != (UnityEngine.Object)null)
            {
                this.mReplacement.dynamicFontStyle = value;
            }
            else if (this.mDynamicFontStyle != value)
            {
                this.mDynamicFontStyle = value;
                this.MarkAsChanged();
            }
        }
    }

    private void Trim()
    {
        Texture texture = this.mAtlas.texture;
        if (texture != (UnityEngine.Object)null && this.mSprite != null)
        {
            Rect rect = NGUIMath.ConvertToPixels(this.mUVRect, this.texture.width, this.texture.height, true);
            Rect rect2 = new Rect((Single)this.mSprite.x, (Single)this.mSprite.y, (Single)this.mSprite.width, (Single)this.mSprite.height);
            Int32 xMin = Mathf.RoundToInt(rect2.xMin - rect.xMin);
            Int32 yMin = Mathf.RoundToInt(rect2.yMin - rect.yMin);
            Int32 xMax = Mathf.RoundToInt(rect2.xMax - rect.xMin);
            Int32 yMax = Mathf.RoundToInt(rect2.yMax - rect.yMin);
            this.mFont.Trim(xMin, yMin, xMax, yMax);
        }
    }

    private Boolean References(UIFont font)
    {
        return !(font == (UnityEngine.Object)null) && (font == this || (this.mReplacement != (UnityEngine.Object)null && this.mReplacement.References(font)));
    }

    public static Boolean CheckIfRelated(UIFont a, UIFont b)
    {
        return !(a == (UnityEngine.Object)null) && !(b == (UnityEngine.Object)null) && ((a.isDynamic && b.isDynamic && a.dynamicFont.fontNames[0] == b.dynamicFont.fontNames[0]) || a == b || a.References(b) || b.References(a));
    }

    private Texture dynamicTexture
    {
        get
        {
            if (this.mReplacement)
            {
                return this.mReplacement.dynamicTexture;
            }
            if (this.isDynamic)
            {
                return this.mDynamicFont.material.mainTexture;
            }
            return (Texture)null;
        }
    }

    public void MarkAsChanged()
    {
        if (this.mReplacement != (UnityEngine.Object)null)
        {
            this.mReplacement.MarkAsChanged();
        }
        this.mSprite = (UISpriteData)null;
        UILabel[] array = NGUITools.FindActive<UILabel>();
        Int32 i = 0;
        Int32 num = (Int32)array.Length;
        while (i < num)
        {
            UILabel uilabel = array[i];
            if (uilabel.enabled && NGUITools.GetActive(uilabel.gameObject) && UIFont.CheckIfRelated(this, uilabel.bitmapFont))
            {
                UIFont bitmapFont = uilabel.bitmapFont;
                uilabel.bitmapFont = (UIFont)null;
                uilabel.bitmapFont = bitmapFont;
            }
            i++;
        }
        Int32 j = 0;
        Int32 count = this.symbols.Count;
        while (j < count)
        {
            this.symbols[j].MarkAsChanged();
            j++;
        }
    }

    public void UpdateUVRect()
    {
        if (this.mAtlas == (UnityEngine.Object)null)
        {
            return;
        }
        Texture texture = this.mAtlas.texture;
        if (texture != (UnityEngine.Object)null)
        {
            this.mUVRect = new Rect((Single)(this.mSprite.x - this.mSprite.paddingLeft), (Single)(this.mSprite.y - this.mSprite.paddingTop), (Single)(this.mSprite.width + this.mSprite.paddingLeft + this.mSprite.paddingRight), (Single)(this.mSprite.height + this.mSprite.paddingTop + this.mSprite.paddingBottom));
            this.mUVRect = NGUIMath.ConvertToTexCoords(this.mUVRect, texture.width, texture.height);
            if (this.mSprite.hasPadding)
            {
                this.Trim();
            }
        }
    }

    private BMSymbol GetSymbol(String sequence, Boolean createIfMissing)
    {
        Int32 i = 0;
        Int32 count = this.mSymbols.Count;
        while (i < count)
        {
            BMSymbol bmsymbol = this.mSymbols[i];
            if (bmsymbol.sequence == sequence)
            {
                return bmsymbol;
            }
            i++;
        }
        if (createIfMissing)
        {
            BMSymbol bmsymbol2 = new BMSymbol();
            bmsymbol2.sequence = sequence;
            this.mSymbols.Add(bmsymbol2);
            return bmsymbol2;
        }
        return (BMSymbol)null;
    }

    public BMSymbol MatchSymbol(String text, Int32 offset, Int32 textLength)
    {
        Int32 count = this.mSymbols.Count;
        if (count == 0)
        {
            return (BMSymbol)null;
        }
        textLength -= offset;
        for (Int32 i = 0; i < count; i++)
        {
            BMSymbol bmsymbol = this.mSymbols[i];
            Int32 length = bmsymbol.length;
            if (length != 0 && textLength >= length)
            {
                Boolean flag = true;
                for (Int32 j = 0; j < length; j++)
                {
                    if (text[offset + j] != bmsymbol.sequence[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag && bmsymbol.Validate(this.atlas))
                {
                    return bmsymbol;
                }
            }
        }
        return (BMSymbol)null;
    }

    public void AddSymbol(String sequence, String spriteName)
    {
        BMSymbol symbol = this.GetSymbol(sequence, true);
        symbol.spriteName = spriteName;
        this.MarkAsChanged();
    }

    public void RemoveSymbol(String sequence)
    {
        BMSymbol symbol = this.GetSymbol(sequence, false);
        if (symbol != null)
        {
            this.symbols.Remove(symbol);
        }
        this.MarkAsChanged();
    }

    public void RenameSymbol(String before, String after)
    {
        BMSymbol symbol = this.GetSymbol(before, false);
        if (symbol != null)
        {
            symbol.sequence = after;
        }
        this.MarkAsChanged();
    }

    public Boolean UsesSprite(String s)
    {
        if (!String.IsNullOrEmpty(s))
        {
            if (s.Equals(this.spriteName))
            {
                return true;
            }
            Int32 i = 0;
            Int32 count = this.symbols.Count;
            while (i < count)
            {
                BMSymbol bmsymbol = this.symbols[i];
                if (s.Equals(bmsymbol.spriteName))
                {
                    return true;
                }
                i++;
            }
        }
        return false;
    }

    [SerializeField]
    [HideInInspector]
    private Material mMat;

    [HideInInspector]
    [SerializeField]
    private Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

    [SerializeField]
    [HideInInspector]
    private BMFont mFont = new BMFont();

    [SerializeField]
    [HideInInspector]
    private UIAtlas mAtlas;

    [HideInInspector]
    [SerializeField]
    private UIFont mReplacement;

    [SerializeField]
    [HideInInspector]
    private List<BMSymbol> mSymbols = new List<BMSymbol>();

    [SerializeField]
    [HideInInspector]
    private Font mDynamicFont;

    [SerializeField]
    [HideInInspector]
    private Int32 mDynamicFontSize = 16;

    [SerializeField]
    [HideInInspector]
    private FontStyle mDynamicFontStyle;

    [NonSerialized]
    private UISpriteData mSprite;

    private Int32 mPMA = -1;

    private Int32 mPacked = -1;
}
