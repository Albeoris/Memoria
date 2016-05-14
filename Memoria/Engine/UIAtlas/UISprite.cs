using System;
using UnityEngine;

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

[AddComponentMenu("NGUI/UI/NGUI Sprite"), ExecuteInEditMode]
public class UISprite : UIBasicSprite
{
    [HideInInspector, SerializeField]
    private UIAtlas mAtlas;

    [HideInInspector, SerializeField]
    private string mSpriteName;

    [HideInInspector, SerializeField]
    private bool mFillCenter = true;

    [NonSerialized]
    protected UISpriteData mSprite;

    [NonSerialized]
    private bool mSpriteSet;

    public override Material material => this.mAtlas?.spriteMaterial;

    public UIAtlas atlas
    {
        get { return this.mAtlas; }
        set
        {
            if (this.mAtlas != value)
            {
                RemoveFromPanel();
                this.mAtlas = value;
                this.mSpriteSet = false;
                this.mSprite = null;
                if (string.IsNullOrEmpty(this.mSpriteName) && this.mAtlas != null && this.mAtlas.spriteList.Count > 0)
                {
                    this.SetAtlasSprite(this.mAtlas.spriteList[0]);
                    this.mSpriteName = this.mSprite.name;
                }
                if (!string.IsNullOrEmpty(this.mSpriteName))
                {
                    string sprName = this.mSpriteName;
                    this.mSpriteName = string.Empty;
                    this.spriteName = sprName;
                    this.MarkAsChanged();
                }
            }
        }
    }

    public string spriteName
    {
        get { return this.mSpriteName; }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                if (string.IsNullOrEmpty(this.mSpriteName))
                {
                    return;
                }
                this.mSpriteName = string.Empty;
                this.mSprite = null;
                this.mChanged = true;
                this.mSpriteSet = false;
            }
            else if (this.mSpriteName != value)
            {
                this.mSpriteName = value;
                this.mSprite = null;
                this.mChanged = true;
                this.mSpriteSet = false;
            }
        }
    }

    public bool isValid => this.GetAtlasSprite() != null;

    [Obsolete("Use 'centerType' instead")]
    public bool fillCenter
    {
        get { return this.centerType != AdvancedType.Invisible; }
        set
        {
            if (value != (this.centerType != AdvancedType.Invisible))
            {
                this.centerType = ((!value) ? AdvancedType.Invisible : AdvancedType.Sliced);
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
            {
                return base.border;
            }
            return new Vector4(atlasSprite.borderLeft, atlasSprite.borderBottom, atlasSprite.borderRight, atlasSprite.borderTop);
        }
    }

    public override float pixelSize => this.mAtlas?.pixelSize ?? 1f;

    public override int minWidth
    {
        get
        {
            if (this.type == Type.Sliced || this.type == Type.Advanced)
            {
                float pxSize = this.pixelSize;
                Vector4 vector = this.border * this.pixelSize;
                int num = Mathf.RoundToInt(vector.x + vector.z);
                UISpriteData atlasSprite = this.GetAtlasSprite();
                if (atlasSprite != null)
                {
                    num += Mathf.RoundToInt(pxSize * (atlasSprite.paddingLeft + atlasSprite.paddingRight));
                }
                return Mathf.Max(base.minWidth, ((num & 1) != 1) ? num : (num + 1));
            }
            return base.minWidth;
        }
    }

    public override int minHeight
    {
        get
        {
            if (this.type == Type.Sliced || this.type == Type.Advanced)
            {
                Vector4 vector = this.border * this.pixelSize;
                int num = Mathf.RoundToInt(vector.y + vector.w);
                UISpriteData atlasSprite = this.GetAtlasSprite();
                if (atlasSprite != null)
                {
                    num += atlasSprite.paddingTop + atlasSprite.paddingBottom;
                }
                return Mathf.Max(base.minHeight, ((num & 1) != 1) ? num : (num + 1));
            }
            return base.minHeight;
        }
    }

    public override Vector4 drawingDimensions
    {
        get
        {
            Vector2 pivOffset = this.pivotOffset;
            float num = -pivOffset.x * this.mWidth;
            float num2 = -pivOffset.y * this.mHeight;
            float num3 = num + this.mWidth;
            float num4 = num2 + this.mHeight;
            if (this.GetAtlasSprite() != null && this.mType != Type.Tiled)
            {
                int num5 = this.mSprite.paddingLeft;
                int num6 = this.mSprite.paddingBottom;
                int num7 = this.mSprite.paddingRight;
                int num8 = this.mSprite.paddingTop;
                float pixSize = this.pixelSize;
                if (pixSize != 1f)
                {
                    num5 = Mathf.RoundToInt(pixSize * num5);
                    num6 = Mathf.RoundToInt(pixSize * num6);
                    num7 = Mathf.RoundToInt(pixSize * num7);
                    num8 = Mathf.RoundToInt(pixSize * num8);
                }
                int num9 = this.mSprite.width + num5 + num7;
                int num10 = this.mSprite.height + num6 + num8;
                float num11 = 1f;
                float num12 = 1f;
                if (num9 > 0 && num10 > 0 && (this.mType == Type.Simple || this.mType == Type.Filled))
                {
                    if ((num9 & 1) != 0)
                    {
                        num7++;
                    }
                    if ((num10 & 1) != 0)
                    {
                        num8++;
                    }
                    num11 = 1f / num9 * this.mWidth;
                    num12 = 1f / num10 * this.mHeight;
                }
                if (this.mFlip == Flip.Horizontally || this.mFlip == Flip.Both)
                {
                    num += num7 * num11;
                    num3 -= num5 * num11;
                }
                else
                {
                    num += num5 * num11;
                    num3 -= num7 * num11;
                }
                if (this.mFlip == Flip.Vertically || this.mFlip == Flip.Both)
                {
                    num2 += num8 * num12;
                    num4 -= num6 * num12;
                }
                else
                {
                    num2 += num6 * num12;
                    num4 -= num8 * num12;
                }
            }
            Vector4 vector = (!(this.mAtlas != null)) ? Vector4.zero : (this.border * this.pixelSize);
            float num13 = vector.x + vector.z;
            float num14 = vector.y + vector.w;
            float x = Mathf.Lerp(num, num3 - num13, this.mDrawRegion.x);
            float y = Mathf.Lerp(num2, num4 - num14, this.mDrawRegion.y);
            float z = Mathf.Lerp(num + num13, num3, this.mDrawRegion.z);
            float w = Mathf.Lerp(num2 + num14, num4, this.mDrawRegion.w);
            return new Vector4(x, y, z, w);
        }
    }

    public override bool premultipliedAlpha => this.mAtlas != null && this.mAtlas.premultipliedAlpha;

    public UISpriteData GetAtlasSprite()
    {
        if (!this.mSpriteSet)
        {
            this.mSprite = null;
        }
        if (this.mSprite == null && this.mAtlas != null)
        {
            if (!string.IsNullOrEmpty(this.mSpriteName))
            {
                UISpriteData sprite = this.mAtlas.GetSprite(this.mSpriteName);
                if (sprite == null)
                {
                    return null;
                }
                this.SetAtlasSprite(sprite);
            }
            if (this.mSprite == null && this.mAtlas.spriteList.Count > 0)
            {
                UISpriteData uISpriteData = this.mAtlas.spriteList[0];
                if (uISpriteData == null)
                {
                    return null;
                }
                this.SetAtlasSprite(uISpriteData);
                if (this.mSprite == null)
                {
                    Debug.LogError(this.mAtlas.name + " seems to have a null sprite!");
                    return null;
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
            this.mSpriteName = ((this.mSprite == null) ? string.Empty : this.mSprite.name);
            this.mSprite = null;
        }
    }

    public override void MakePixelPerfect()
    {
        if (!this.isValid)
        {
            return;
        }
        base.MakePixelPerfect();
        if (this.mType == Type.Tiled)
        {
            return;
        }
        UISpriteData atlasSprite = this.GetAtlasSprite();
        if (atlasSprite == null)
        {
            return;
        }
        Texture mainTxture = this.mainTexture;
        if (mainTxture == null)
        {
            return;
        }
        if ((this.mType == Type.Simple || this.mType == Type.Filled || !atlasSprite.hasBorder))
        {
            int num = Mathf.RoundToInt(this.pixelSize * (atlasSprite.width + atlasSprite.paddingLeft + atlasSprite.paddingRight));
            int num2 = Mathf.RoundToInt(this.pixelSize * (atlasSprite.height + atlasSprite.paddingTop + atlasSprite.paddingBottom));
            if ((num & 1) == 1)
            {
                num++;
            }
            if ((num2 & 1) == 1)
            {
                num2++;
            }
            width = num;
            height = num2;
        }
    }

    protected override void OnInit()
    {
        if (!this.mFillCenter)
        {
            this.mFillCenter = true;
            this.centerType = AdvancedType.Invisible;
        }
        base.OnInit();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (this.mChanged || !this.mSpriteSet)
        {
            this.mSpriteSet = true;
            this.mSprite = null;
            this.mChanged = true;
        }
    }

    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        Texture mainTxture = this.mainTexture;
        if (mainTxture == null)
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
        Rect rect = new Rect(this.mSprite.x, this.mSprite.y, this.mSprite.width, this.mSprite.height);
        Rect rect2 = new Rect((this.mSprite.x + this.mSprite.borderLeft), (this.mSprite.y + this.mSprite.borderTop), (this.mSprite.width - this.mSprite.borderLeft - this.mSprite.borderRight), (this.mSprite.height - this.mSprite.borderBottom - this.mSprite.borderTop));
        rect = NGUIMath.ConvertToTexCoords(rect, mainTxture.width, mainTxture.height);
        rect2 = NGUIMath.ConvertToTexCoords(rect2, mainTxture.width, mainTxture.height);
        int size = verts.size;
        Fill(verts, uvs, cols, rect, rect2);
        this.onPostFill?.Invoke(this, size, verts, uvs, cols);
    }
}