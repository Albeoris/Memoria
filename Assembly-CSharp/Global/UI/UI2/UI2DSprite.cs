using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Unity2D Sprite")]
[ExecuteInEditMode]
public class UI2DSprite : UIBasicSprite
{
	public Sprite sprite2D
	{
		get
		{
			return this.mSprite;
		}
		set
		{
			if (this.mSprite != value)
			{
				base.RemoveFromPanel();
				this.mSprite = value;
				this.nextSprite = (Sprite)null;
				base.CreatePanel();
			}
		}
	}

	public override Material material
	{
		get
		{
			return this.mMat;
		}
		set
		{
			if (this.mMat != value)
			{
				base.RemoveFromPanel();
				this.mMat = value;
				this.mPMA = -1;
				this.MarkAsChanged();
			}
		}
	}

	public override Shader shader
	{
		get
		{
			if (this.mMat != (UnityEngine.Object)null)
			{
				return this.mMat.shader;
			}
			if (this.mShader == (UnityEngine.Object)null)
			{
				this.mShader = Shader.Find("Unlit/Transparent Colored");
			}
			return this.mShader;
		}
		set
		{
			if (this.mShader != value)
			{
				base.RemoveFromPanel();
				this.mShader = value;
				if (this.mMat == (UnityEngine.Object)null)
				{
					this.mPMA = -1;
					this.MarkAsChanged();
				}
			}
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (this.mSprite != (UnityEngine.Object)null)
			{
				return this.mSprite.texture;
			}
			if (this.mMat != (UnityEngine.Object)null)
			{
				return this.mMat.mainTexture;
			}
			return (Texture)null;
		}
	}

	public override Boolean premultipliedAlpha
	{
		get
		{
			if (this.mPMA == -1)
			{
				Shader shader = this.shader;
				this.mPMA = (Int32)((!(shader != (UnityEngine.Object)null) || !shader.name.Contains("Premultiplied")) ? 0 : 1);
			}
			return this.mPMA == 1;
		}
	}

	public override Single pixelSize
	{
		get
		{
			return this.mPixelSize;
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
			if (this.mSprite != (UnityEngine.Object)null && this.mType != UIBasicSprite.Type.Tiled)
			{
				Int32 num5 = Mathf.RoundToInt(this.mSprite.rect.width);
				Int32 num6 = Mathf.RoundToInt(this.mSprite.rect.height);
				Int32 num7 = Mathf.RoundToInt(this.mSprite.textureRectOffset.x);
				Int32 num8 = Mathf.RoundToInt(this.mSprite.textureRectOffset.y);
				Int32 num9 = Mathf.RoundToInt(this.mSprite.rect.width - this.mSprite.textureRect.width - this.mSprite.textureRectOffset.x);
				Int32 num10 = Mathf.RoundToInt(this.mSprite.rect.height - this.mSprite.textureRect.height - this.mSprite.textureRectOffset.y);
				Single num11 = 1f;
				Single num12 = 1f;
				if (num5 > 0 && num6 > 0 && (this.mType == UIBasicSprite.Type.Simple || this.mType == UIBasicSprite.Type.Filled))
				{
					if ((num5 & 1) != 0)
					{
						num9++;
					}
					if ((num6 & 1) != 0)
					{
						num10++;
					}
					num11 = 1f / (Single)num5 * (Single)this.mWidth;
					num12 = 1f / (Single)num6 * (Single)this.mHeight;
				}
				if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
				{
					num += (Single)num9 * num11;
					num3 -= (Single)num7 * num11;
				}
				else
				{
					num += (Single)num7 * num11;
					num3 -= (Single)num9 * num11;
				}
				if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
				{
					num2 += (Single)num10 * num12;
					num4 -= (Single)num8 * num12;
				}
				else
				{
					num2 += (Single)num8 * num12;
					num4 -= (Single)num10 * num12;
				}
			}
			Single num13;
			Single num14;
			if (this.mFixedAspect)
			{
				num13 = 0f;
				num14 = 0f;
			}
			else
			{
				Vector4 vector = this.border * this.pixelSize;
				num13 = vector.x + vector.z;
				num14 = vector.y + vector.w;
			}
			Single x = Mathf.Lerp(num, num3 - num13, this.mDrawRegion.x);
			Single y = Mathf.Lerp(num2, num4 - num14, this.mDrawRegion.y);
			Single z = Mathf.Lerp(num + num13, num3, this.mDrawRegion.z);
			Single w = Mathf.Lerp(num2 + num14, num4, this.mDrawRegion.w);
			return new Vector4(x, y, z, w);
		}
	}

	public override Vector4 border
	{
		get
		{
			return this.mBorder;
		}
		set
		{
			if (this.mBorder != value)
			{
				this.mBorder = value;
				this.MarkAsChanged();
			}
		}
	}

	protected override void OnUpdate()
	{
		if (this.nextSprite != (UnityEngine.Object)null)
		{
			if (this.nextSprite != this.mSprite)
			{
				this.sprite2D = this.nextSprite;
			}
			this.nextSprite = (Sprite)null;
		}
		base.OnUpdate();
		if (this.mFixedAspect)
		{
			Texture mainTexture = this.mainTexture;
			if (mainTexture != (UnityEngine.Object)null)
			{
				Int32 num = Mathf.RoundToInt(this.mSprite.rect.width);
				Int32 num2 = Mathf.RoundToInt(this.mSprite.rect.height);
				Int32 num3 = Mathf.RoundToInt(this.mSprite.textureRectOffset.x);
				Int32 num4 = Mathf.RoundToInt(this.mSprite.textureRectOffset.y);
				Int32 num5 = Mathf.RoundToInt(this.mSprite.rect.width - this.mSprite.textureRect.width - this.mSprite.textureRectOffset.x);
				Int32 num6 = Mathf.RoundToInt(this.mSprite.rect.height - this.mSprite.textureRect.height - this.mSprite.textureRectOffset.y);
				num += num3 + num5;
				num2 += num6 + num4;
				Single num7 = (Single)this.mWidth;
				Single num8 = (Single)this.mHeight;
				Single num9 = num7 / num8;
				Single num10 = (Single)num / (Single)num2;
				if (num10 < num9)
				{
					Single num11 = (num7 - num8 * num10) / num7 * 0.5f;
					base.drawRegion = new Vector4(num11, 0f, 1f - num11, 1f);
				}
				else
				{
					Single num12 = (num8 - num7 / num10) / num8 * 0.5f;
					base.drawRegion = new Vector4(0f, num12, 1f, 1f - num12);
				}
			}
		}
	}

	public override void MakePixelPerfect()
	{
		base.MakePixelPerfect();
		if (this.mType == UIBasicSprite.Type.Tiled)
		{
			return;
		}
		Texture mainTexture = this.mainTexture;
		if (mainTexture == (UnityEngine.Object)null)
		{
			return;
		}
		if ((this.mType == UIBasicSprite.Type.Simple || this.mType == UIBasicSprite.Type.Filled || !base.hasBorder) && mainTexture != (UnityEngine.Object)null)
		{
			Rect rect = this.mSprite.rect;
			Int32 num = Mathf.RoundToInt(rect.width);
			Int32 num2 = Mathf.RoundToInt(rect.height);
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

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture mainTexture = this.mainTexture;
		if (mainTexture == (UnityEngine.Object)null)
		{
			return;
		}
		Rect rect = (!(this.mSprite != (UnityEngine.Object)null)) ? new Rect(0f, 0f, (Single)mainTexture.width, (Single)mainTexture.height) : this.mSprite.textureRect;
		Rect inner = rect;
		Vector4 border = this.border;
		inner.xMin += border.x;
		inner.yMin += border.y;
		inner.xMax -= border.z;
		inner.yMax -= border.w;
		Single num = 1f / (Single)mainTexture.width;
		Single num2 = 1f / (Single)mainTexture.height;
		rect.xMin *= num;
		rect.xMax *= num;
		rect.yMin *= num2;
		rect.yMax *= num2;
		inner.xMin *= num;
		inner.xMax *= num;
		inner.yMin *= num2;
		inner.yMax *= num2;
		Int32 size = verts.size;
		base.Fill(verts, uvs, cols, rect, inner);
		if (this.onPostFill != null)
		{
			this.onPostFill(this, size, verts, uvs, cols);
		}
	}

	[HideInInspector]
	[SerializeField]
	private Sprite mSprite;

	[SerializeField]
	[HideInInspector]
	private Material mMat;

	[HideInInspector]
	[SerializeField]
	private Shader mShader;

	[HideInInspector]
	[SerializeField]
	private Vector4 mBorder = Vector4.zero;

	[HideInInspector]
	[SerializeField]
	private Boolean mFixedAspect;

	[HideInInspector]
	[SerializeField]
	private Single mPixelSize = 1f;

	public Sprite nextSprite;

	[NonSerialized]
	private Int32 mPMA = -1;
}
