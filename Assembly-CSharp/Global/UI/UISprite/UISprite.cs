using System;
using Memoria;
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
				if (!String.IsNullOrEmpty(this.mSpriteName) && this.drawCall != null)
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
			Single num = -pivotOffset.x * (Single)this.mWidth;
			Single num2 = -pivotOffset.y * (Single)this.mHeight;
			Single num3 = num + (Single)this.mWidth;
			Single num4 = num2 + (Single)this.mHeight;
			if (this.GetAtlasSprite() != null && this.mType != UIBasicSprite.Type.Tiled)
			{
				Int32 num5 = this.mSprite.paddingLeft;
				Int32 num6 = this.mSprite.paddingBottom;
				Int32 num7 = this.mSprite.paddingRight;
				Int32 num8 = this.mSprite.paddingTop;
				Single pixelSize = this.pixelSize;
				if (pixelSize != 1f)
				{
					num5 = Mathf.RoundToInt(pixelSize * (Single)num5);
					num6 = Mathf.RoundToInt(pixelSize * (Single)num6);
					num7 = Mathf.RoundToInt(pixelSize * (Single)num7);
					num8 = Mathf.RoundToInt(pixelSize * (Single)num8);
				}
				Int32 num9 = this.mSprite.width + num5 + num7;
				Int32 num10 = this.mSprite.height + num6 + num8;
				Single num11 = 1f;
				Single num12 = 1f;
				if (num9 > 0 && num10 > 0 && (this.mType == UIBasicSprite.Type.Simple || this.mType == UIBasicSprite.Type.Filled))
				{
					if ((num9 & 1) != 0)
					{
						num7++;
					}
					if ((num10 & 1) != 0)
					{
						num8++;
					}
					num11 = 1f / (Single)num9 * (Single)this.mWidth;
					num12 = 1f / (Single)num10 * (Single)this.mHeight;
				}
				if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
				{
					num += (Single)num7 * num11;
					num3 -= (Single)num5 * num11;
				}
				else
				{
					num += (Single)num5 * num11;
					num3 -= (Single)num7 * num11;
				}
				if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
				{
					num2 += (Single)num8 * num12;
					num4 -= (Single)num6 * num12;
				}
				else
				{
					num2 += (Single)num6 * num12;
					num4 -= (Single)num8 * num12;
				}
			}
			Vector4 vector = (!(this.mAtlas != (UnityEngine.Object)null)) ? Vector4.zero : (this.border * this.pixelSize);
			Single num13 = vector.x + vector.z;
			Single num14 = vector.y + vector.w;
			Single x = Mathf.Lerp(num, num3 - num13, this.mDrawRegion.x);
			Single y = Mathf.Lerp(num2, num4 - num14, this.mDrawRegion.y);
			Single z = Mathf.Lerp(num + num13, num3, this.mDrawRegion.z);
			Single w = Mathf.Lerp(num2 + num14, num4, this.mDrawRegion.w);
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
		{
			return;
		}
		base.MakePixelPerfect();
		if (this.mType == UIBasicSprite.Type.Tiled)
		{
			return;
		}
		UISpriteData atlasSprite = this.GetAtlasSprite();
		if (atlasSprite == null)
		{
			return;
		}
		Texture mainTexture = this.mainTexture;
		if (mainTexture == (UnityEngine.Object)null)
		{
			return;
		}
		if ((this.mType == UIBasicSprite.Type.Simple || this.mType == UIBasicSprite.Type.Filled || !atlasSprite.hasBorder) && mainTexture != (UnityEngine.Object)null)
		{
			Int32 num = Mathf.RoundToInt(this.pixelSize * (Single)(atlasSprite.width + atlasSprite.paddingLeft + atlasSprite.paddingRight));
			Int32 num2 = Mathf.RoundToInt(this.pixelSize * (Single)(atlasSprite.height + atlasSprite.paddingTop + atlasSprite.paddingBottom));
			if ((num & 1) == 1)
			{
				num++;
			}
			if ((num2 & 1) == 1)
			{
				num2++;
			}
			base.width = num;
			base.height = num2;
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
}
