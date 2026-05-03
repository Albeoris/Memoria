using Memoria;
using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Sprite")]
public class UISprite : UIBasicSprite
{
    public override Material material
    {
        get => this.mAtlas?.spriteMaterial;
    }

    public override Texture mainTexture
    {
        get
        {
            if (this.mSprite == null)
                this.mSprite = this.atlas.GetSprite(this.spriteName);
            if (this.mSprite != null && this.mSprite.texture != null)
                return this.mSprite.texture;
            return this.material?.mainTexture;
        }
    }

    public UIAtlas atlas
    {
        get => this.mAtlas;
        set
        {
            if (this.mAtlas != value)
            {
                base.RemoveFromPanel();
                this.mAtlas = value;
                this.mSpriteSet = false;
                this.mSprite = null;
                if (String.IsNullOrEmpty(this.mSpriteName) && this.mAtlas != null && this.mAtlas.spriteList.Count > 0)
                {
                    this.SetAtlasSprite(this.mAtlas.spriteList[0]);
                    this.mSpriteName = this.mSprite.name;
                }
                if (!String.IsNullOrEmpty(this.mSpriteName))
                {
                    String spriteName = this.mSpriteName;
                    this.mSpriteName = String.Empty;
                    this.spriteName = spriteName;
                    this.MarkAsChanged();
                }
            }
        }
    }

    public String spriteName
    {
        get => this.mSpriteName;
        set
        {
            if (String.IsNullOrEmpty(value))
            {
                if (String.IsNullOrEmpty(this.mSpriteName))
                    return;
                this.mSpriteName = String.Empty;
                this.mSprite = null;
                this.mChanged = true;
                this.mSpriteSet = false;
            }
            else if (this.mSpriteName != value)
            {
                if (this.drawCall != null)
                    this.panel.RebuildAllDrawCalls();
                this.mSpriteName = value;
                this.mSprite = null;
                this.mChanged = true;
                this.mSpriteSet = false;
            }
        }
    }

    public Boolean isValid
    {
        get => this.GetAtlasSprite() != null;
    }

    [Obsolete("Use 'centerType' instead")]
    public Boolean fillCenter
    {
        get => this.centerType != UIBasicSprite.AdvancedType.Invisible;
        set
        {
            if (value != (this.centerType != UIBasicSprite.AdvancedType.Invisible))
            {
                this.centerType = value ? UIBasicSprite.AdvancedType.Sliced : UIBasicSprite.AdvancedType.Invisible;
                this.MarkAsChanged();
            }
        }
    }

    public override Vector4 border
    {
        get
        {
            UISpriteData atlasSprite = this.GetAtlasSprite();
            if (atlasSprite == null)
                return base.border;
            return new Vector4(atlasSprite.borderLeft, atlasSprite.borderBottom, atlasSprite.borderRight, atlasSprite.borderTop);
        }
    }

    public override Single pixelSize
    {
        get => this.mAtlas == null ? 1f : this.mAtlas.pixelSize;
    }

    public override Int32 minWidth
    {
        get
        {
            // Black square
            if (Configuration.Graphics.WidescreenSupport && (mSpriteName == "menu_bg" || mSpriteName == "dialog_hilight"))
                return mHeight * Screen.width / Screen.height;

            if (this.type == UIBasicSprite.Type.Sliced || this.type == UIBasicSprite.Type.Advanced)
            {
                Vector4 pixelBorder = this.border * this.pixelSize;
                Int32 widthResult = Mathf.RoundToInt(pixelBorder.x + pixelBorder.z);
                UISpriteData atlasSprite = this.GetAtlasSprite();
                if (atlasSprite != null)
                    widthResult += Mathf.RoundToInt(this.pixelSize * (atlasSprite.paddingLeft + atlasSprite.paddingRight));
                if ((widthResult & 1) == 1)
                    widthResult++;
                return Mathf.Max(base.minWidth, widthResult);
            }
            return base.minWidth;
        }
    }

    public override Int32 minHeight
    {
        get
        {
            if (this.type == UIBasicSprite.Type.Sliced || this.type == UIBasicSprite.Type.Advanced)
            {
                Vector4 pixelBorder = this.border * this.pixelSize;
                Int32 heightResult = Mathf.RoundToInt(pixelBorder.y + pixelBorder.w);
                UISpriteData atlasSprite = this.GetAtlasSprite();
                if (atlasSprite != null)
                    heightResult += atlasSprite.paddingTop + atlasSprite.paddingBottom;
                if ((heightResult & 1) == 1)
                    heightResult++;
                return Mathf.Max(base.minHeight, heightResult);
            }
            return base.minHeight;
        }
    }

    public override Vector4 drawingDimensions
    {
        get
        {
            Vector2 pivotOffset = base.pivotOffset;
            Single left = -pivotOffset.x * (Single)this.mWidth;
            Single bottom = -pivotOffset.y * (Single)this.mHeight;
            Single right = left + (Single)this.mWidth;
            Single top = bottom + (Single)this.mHeight;
            if (this.GetAtlasSprite() != null && this.mType != UIBasicSprite.Type.Tiled)
            {
                Int32 paddingLeft = this.mSprite.paddingLeft;
                Int32 paddingBottom = this.mSprite.paddingBottom;
                Int32 paddingRight = this.mSprite.paddingRight;
                Int32 paddingTop = this.mSprite.paddingTop;
                Single pixelSize = this.pixelSize;
                if (pixelSize != 1f)
                {
                    paddingLeft = Mathf.RoundToInt(pixelSize * (Single)paddingLeft);
                    paddingBottom = Mathf.RoundToInt(pixelSize * (Single)paddingBottom);
                    paddingRight = Mathf.RoundToInt(pixelSize * (Single)paddingRight);
                    paddingTop = Mathf.RoundToInt(pixelSize * (Single)paddingTop);
                }
                Int32 spriteWidth = this.mSprite.width + paddingLeft + paddingRight;
                Int32 spriteHeight = this.mSprite.height + paddingBottom + paddingTop;
                Single scaleX = 1f;
                Single scaleY = 1f;
                if (spriteWidth > 0 && spriteHeight > 0 && (this.mType == UIBasicSprite.Type.Simple || this.mType == UIBasicSprite.Type.Filled))
                {
                    if ((spriteWidth & 1) != 0)
                    {
                        paddingRight++;
                    }
                    if ((spriteHeight & 1) != 0)
                    {
                        paddingTop++;
                    }
                    scaleX = 1f / (Single)spriteWidth * (Single)this.mWidth;
                    scaleY = 1f / (Single)spriteHeight * (Single)this.mHeight;
                }
                if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
                {
                    left += (Single)paddingRight * scaleX;
                    right -= (Single)paddingLeft * scaleX;
                }
                else
                {
                    left += (Single)paddingLeft * scaleX;
                    right -= (Single)paddingRight * scaleX;
                }
                if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
                {
                    bottom += (Single)paddingTop * scaleY;
                    top -= (Single)paddingBottom * scaleY;
                }
                else
                {
                    bottom += (Single)paddingBottom * scaleY;
                    top -= (Single)paddingTop * scaleY;
                }
            }
            Vector4 borderVector = (!(this.mAtlas != (UnityEngine.Object)null)) ? Vector4.zero : (this.border * this.pixelSize);
            Single borderX = borderVector.x + borderVector.z;
            Single borderY = borderVector.y + borderVector.w;
            Single x = Mathf.Lerp(left, right - borderX, this.mDrawRegion.x);
            Single y = Mathf.Lerp(bottom, top - borderY, this.mDrawRegion.y);
            Single z = Mathf.Lerp(left + borderX, right, this.mDrawRegion.z);
            Single w = Mathf.Lerp(bottom + borderY, top, this.mDrawRegion.w);
            return new Vector4(x, y, z, w);
        }
    }

    public override Boolean premultipliedAlpha
    {
        get
        {
            return this.mAtlas != (UnityEngine.Object)null && this.mAtlas.premultipliedAlpha;
        }
    }

    public UISpriteData GetAtlasSprite()
    {
        if (!this.mSpriteSet)
        {
            this.mSprite = (UISpriteData)null;
        }
        if (this.mSprite == null && this.mAtlas != (UnityEngine.Object)null)
        {
            if (!String.IsNullOrEmpty(this.mSpriteName))
            {
                UISpriteData sprite = this.mAtlas.GetSprite(this.mSpriteName);
                if (sprite == null)
                {
                    return (UISpriteData)null;
                }
                this.SetAtlasSprite(sprite);
            }
            if (this.mSprite == null && this.mAtlas.spriteList.Count > 0)
            {
                UISpriteData uispriteData = this.mAtlas.spriteList[0];
                if (uispriteData == null)
                {
                    return (UISpriteData)null;
                }
                this.SetAtlasSprite(uispriteData);
                if (this.mSprite == null)
                {
                    global::Debug.LogError(this.mAtlas.name + " seems to have a null sprite!");
                    return (UISpriteData)null;
                }
                this.mSpriteName = this.mSprite.name;
            }
        }
        return this.mSprite;
    }

    protected void SetAtlasSprite(UISpriteData sp)
    {
        this.mChanged = true;
        this.mSpriteSet = true;
        if (sp != null)
        {
            this.mSprite = sp;
            this.mSpriteName = this.mSprite.name;
        }
        else
        {
            this.mSpriteName = ((this.mSprite == null) ? String.Empty : this.mSprite.name);
            this.mSprite = sp;
        }
    }

    public override void MakePixelPerfect()
    {
        if (!this.isValid)
            return;
        base.MakePixelPerfect();
        if (this.preventPixelPerfect || this.mType == UIBasicSprite.Type.Tiled)
            return;
        UISpriteData atlasSprite = this.GetAtlasSprite();
        if (atlasSprite == null)
            return;
        Texture mainTexture = this.mainTexture;
        if (mainTexture == null)
            return;
        if (this.mType == UIBasicSprite.Type.Simple || this.mType == UIBasicSprite.Type.Filled || !atlasSprite.hasBorder)
        {
            Int32 w = Mathf.RoundToInt(this.pixelSize * (Single)(atlasSprite.width + atlasSprite.paddingLeft + atlasSprite.paddingRight));
            Int32 h = Mathf.RoundToInt(this.pixelSize * (Single)(atlasSprite.height + atlasSprite.paddingTop + atlasSprite.paddingBottom));
            if ((w & 1) == 1)
                w++;
            if ((h & 1) == 1)
                h++;
            base.width = w;
            base.height = h;
        }
    }

    protected override void OnInit()
    {
        if (!this.mFillCenter)
        {
            this.mFillCenter = true;
            this.centerType = UIBasicSprite.AdvancedType.Invisible;
        }
        base.OnInit();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (this.mChanged || !this.mSpriteSet)
        {
            this.mSpriteSet = true;
            this.mSprite = (UISpriteData)null;
            this.mChanged = true;
        }
    }

    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        Texture mainTexture = this.mainTexture;
        if (mainTexture == (UnityEngine.Object)null)
        {
            return;
        }
        if (this.mSprite == null)
        {
            this.mSprite = this.atlas.GetSprite(this.spriteName);
        }
        if (this.mSprite == null)
        {
            return;
        }
        Rect rect = new Rect((Single)this.mSprite.x, (Single)this.mSprite.y, (Single)this.mSprite.width, (Single)this.mSprite.height);
        Rect rect2 = new Rect((Single)(this.mSprite.x + this.mSprite.borderLeft), (Single)(this.mSprite.y + this.mSprite.borderTop), (Single)(this.mSprite.width - this.mSprite.borderLeft - this.mSprite.borderRight), (Single)(this.mSprite.height - this.mSprite.borderBottom - this.mSprite.borderTop));
        rect = NGUIMath.ConvertToTexCoords(rect, mainTexture.width, mainTexture.height);
        rect2 = NGUIMath.ConvertToTexCoords(rect2, mainTexture.width, mainTexture.height);
        Int32 size = verts.size;
        base.Fill(verts, uvs, cols, rect, rect2);
        if (this.onPostFill != null)
        {
            this.onPostFill(this, size, verts, uvs, cols);
        }
    }

    [SerializeField]
    [HideInInspector]
    private UIAtlas mAtlas;

    [SerializeField]
    [HideInInspector]
    private String mSpriteName;

    [HideInInspector]
    [SerializeField]
    private Boolean mFillCenter = true;

    [NonSerialized]
    protected UISpriteData mSprite;

    [NonSerialized]
    private Boolean mSpriteSet;

    [NonSerialized]
    public Boolean preventPixelPerfect = false;
}
