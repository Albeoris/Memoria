using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Alpha")]
public class TweenAlpha : UITweener
{
	[Obsolete("Use 'value' instead")]
	public Single alpha
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	private void Cache()
	{
		this.mCached = true;
		this.mRect = base.GetComponent<UIRect>();
		this.mSr = base.GetComponent<SpriteRenderer>();
		if (this.mRect == (UnityEngine.Object)null && this.mSr == (UnityEngine.Object)null)
		{
			Renderer component = base.GetComponent<Renderer>();
			if (component != (UnityEngine.Object)null)
			{
				this.mMat = component.material;
			}
			if (this.mMat == (UnityEngine.Object)null)
			{
				this.mRect = base.GetComponentInChildren<UIRect>();
			}
		}
	}

	public Single value
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mRect != (UnityEngine.Object)null)
			{
				return this.mRect.alpha;
			}
			if (this.mSr != (UnityEngine.Object)null)
			{
				return this.mSr.color.a;
			}
			return (!(this.mMat != (UnityEngine.Object)null)) ? 1f : this.mMat.color.a;
		}
		set
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mRect != (UnityEngine.Object)null)
			{
				this.mRect.alpha = value;
			}
			else if (this.mSr != (UnityEngine.Object)null)
			{
				Color color = this.mSr.color;
				color.a = value;
				this.mSr.color = color;
			}
			else if (this.mMat != (UnityEngine.Object)null)
			{
				Color color2 = this.mMat.color;
				color2.a = value;
				this.mMat.color = color2;
			}
		}
	}

	protected override void OnUpdate(Single factor, Boolean isFinished)
	{
		this.value = Mathf.Lerp(this.from, this.to, factor);
	}

	public static TweenAlpha Begin(GameObject go, Single duration, Single alpha)
	{
		TweenAlpha tweenAlpha = UITweener.Begin<TweenAlpha>(go, duration);
		tweenAlpha.from = tweenAlpha.value;
		tweenAlpha.to = alpha;
		if (duration <= 0f)
		{
			tweenAlpha.Sample(1f, true);
			tweenAlpha.enabled = false;
		}
		return tweenAlpha;
	}

	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	[Range(0f, 1f)]
	public Single from = 1f;

	[Range(0f, 1f)]
	public Single to = 1f;

	private Boolean mCached;

	private UIRect mRect;

	private Material mMat;

	private SpriteRenderer mSr;
}
