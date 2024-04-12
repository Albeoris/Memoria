using Memoria.Scripts;
using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Texture")]
public class UITexture : UIBasicSprite
{
	public override Texture mainTexture
	{
		get
		{
			if (this.mTexture != (UnityEngine.Object)null)
			{
				return this.mTexture;
			}
			if (this.mMat != (UnityEngine.Object)null)
			{
				return this.mMat.mainTexture;
			}
			return (Texture)null;
		}
		set
		{
			if (this.mTexture != value)
			{
				if (this.drawCall != (UnityEngine.Object)null && this.drawCall.widgetCount == 1 && this.mMat == (UnityEngine.Object)null)
				{
					this.mTexture = value;
					this.drawCall.mainTexture = value;
				}
				else
				{
					base.RemoveFromPanel();
					this.mTexture = value;
					this.mPMA = -1;
					this.MarkAsChanged();
				}
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
				this.mShader = (Shader)null;
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
				this.mShader = ShadersLoader.Find("Unlit/Transparent Colored");
			}
			return this.mShader;
		}
		set
		{
			if (this.mShader != value)
			{
				if (this.drawCall != (UnityEngine.Object)null && this.drawCall.widgetCount == 1 && this.mMat == (UnityEngine.Object)null)
				{
					this.mShader = value;
					this.drawCall.shader = value;
				}
				else
				{
					base.RemoveFromPanel();
					this.mShader = value;
					this.mPMA = -1;
					this.mMat = (Material)null;
					this.MarkAsChanged();
				}
			}
		}
	}

	public override Boolean premultipliedAlpha
	{
		get
		{
			if (this.mPMA == -1)
			{
				Material material = this.material;
				this.mPMA = (Int32)((!(material != (UnityEngine.Object)null) || !(material.shader != (UnityEngine.Object)null) || !material.shader.name.Contains("Premultiplied")) ? 0 : 1);
			}
			return this.mPMA == 1;
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

	public Rect uvRect
	{
		get
		{
			return this.mRect;
		}
		set
		{
			if (this.mRect != value)
			{
				this.mRect = value;
				this.MarkAsChanged();
			}
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
			if (this.mTexture != (UnityEngine.Object)null && this.mType != UIBasicSprite.Type.Tiled)
			{
				Int32 width = this.mTexture.width;
				Int32 height = this.mTexture.height;
				Int32 num5 = 0;
				Int32 num6 = 0;
				Single num7 = 1f;
				Single num8 = 1f;
				if (width > 0 && height > 0 && (this.mType == UIBasicSprite.Type.Simple || this.mType == UIBasicSprite.Type.Filled))
				{
					if ((width & 1) != 0)
					{
						num5++;
					}
					if ((height & 1) != 0)
					{
						num6++;
					}
					num7 = 1f / (Single)width * (Single)this.mWidth;
					num8 = 1f / (Single)height * (Single)this.mHeight;
				}
				if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
				{
					num += (Single)num5 * num7;
				}
				else
				{
					num3 -= (Single)num5 * num7;
				}
				if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
				{
					num2 += (Single)num6 * num8;
				}
				else
				{
					num4 -= (Single)num6 * num8;
				}
			}
			Single num9;
			Single num10;
			if (this.mFixedAspect)
			{
				num9 = 0f;
				num10 = 0f;
			}
			else
			{
				Vector4 border = this.border;
				num9 = border.x + border.z;
				num10 = border.y + border.w;
			}
			Single x = Mathf.Lerp(num, num3 - num9, this.mDrawRegion.x);
			Single y = Mathf.Lerp(num2, num4 - num10, this.mDrawRegion.y);
			Single z = Mathf.Lerp(num + num9, num3, this.mDrawRegion.z);
			Single w = Mathf.Lerp(num2 + num10, num4, this.mDrawRegion.w);
			return new Vector4(x, y, z, w);
		}
	}

	public Boolean fixedAspect
	{
		get
		{
			return this.mFixedAspect;
		}
		set
		{
			if (this.mFixedAspect != value)
			{
				this.mFixedAspect = value;
				this.mDrawRegion = new Vector4(0f, 0f, 1f, 1f);
				this.MarkAsChanged();
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
			Int32 num = mainTexture.width;
			Int32 num2 = mainTexture.height;
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

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (this.mFixedAspect)
		{
			Texture mainTexture = this.mainTexture;
			if (mainTexture != (UnityEngine.Object)null)
			{
				Int32 num = mainTexture.width;
				Int32 num2 = mainTexture.height;
				if ((num & 1) == 1)
				{
					num++;
				}
				if ((num2 & 1) == 1)
				{
					num2++;
				}
				Single num3 = (Single)this.mWidth;
				Single num4 = (Single)this.mHeight;
				Single num5 = num3 / num4;
				Single num6 = (Single)num / (Single)num2;
				if (num6 < num5)
				{
					Single num7 = (num3 - num4 * num6) / num3 * 0.5f;
					base.drawRegion = new Vector4(num7, 0f, 1f - num7, 1f);
				}
				else
				{
					Single num8 = (num4 - num3 / num6) / num4 * 0.5f;
					base.drawRegion = new Vector4(0f, num8, 1f, 1f - num8);
				}
			}
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture mainTexture = this.mainTexture;
		if (mainTexture == (UnityEngine.Object)null)
		{
			return;
		}
		Rect rect = new Rect(this.mRect.x * (Single)mainTexture.width, this.mRect.y * (Single)mainTexture.height, (Single)mainTexture.width * this.mRect.width, (Single)mainTexture.height * this.mRect.height);
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
	private Rect mRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector]
	[SerializeField]
	private Texture mTexture;

	[HideInInspector]
	[SerializeField]
	private Material mMat;

	[SerializeField]
	[HideInInspector]
	private Shader mShader;

	[HideInInspector]
	[SerializeField]
	private Vector4 mBorder = Vector4.zero;

	[HideInInspector]
	[SerializeField]
	private Boolean mFixedAspect;

	[NonSerialized]
	private Int32 mPMA = -1;
}
