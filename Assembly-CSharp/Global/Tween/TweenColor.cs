using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Color")]
public class TweenColor : UITweener
{
	private void Cache()
	{
		this.mCached = true;
		this.mWidget = base.GetComponent<UIWidget>();
		if (this.mWidget != (UnityEngine.Object)null)
		{
			return;
		}
		this.mSr = base.GetComponent<SpriteRenderer>();
		if (this.mSr != (UnityEngine.Object)null)
		{
			return;
		}
		Renderer component = base.GetComponent<Renderer>();
		if (component != (UnityEngine.Object)null)
		{
			this.mMat = component.material;
			return;
		}
		this.mLight = base.GetComponent<Light>();
		if (this.mLight == (UnityEngine.Object)null)
		{
			this.mWidget = base.GetComponentInChildren<UIWidget>();
		}
	}

	[Obsolete("Use 'value' instead")]
	public Color color
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

	public Color value
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mWidget != (UnityEngine.Object)null)
			{
				return this.mWidget.color;
			}
			if (this.mMat != (UnityEngine.Object)null)
			{
				return this.mMat.color;
			}
			if (this.mSr != (UnityEngine.Object)null)
			{
				return this.mSr.color;
			}
			if (this.mLight != (UnityEngine.Object)null)
			{
				return this.mLight.color;
			}
			return Color.black;
		}
		set
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mWidget != (UnityEngine.Object)null)
			{
				this.mWidget.color = value;
			}
			else if (this.mMat != (UnityEngine.Object)null)
			{
				this.mMat.color = value;
			}
			else if (this.mSr != (UnityEngine.Object)null)
			{
				this.mSr.color = value;
			}
			else if (this.mLight != (UnityEngine.Object)null)
			{
				this.mLight.color = value;
				this.mLight.enabled = (value.r + value.g + value.b > 0.01f);
			}
		}
	}

	protected override void OnUpdate(Single factor, Boolean isFinished)
	{
		this.value = Color.Lerp(this.from, this.to, factor);
	}

	public static TweenColor Begin(GameObject go, Single duration, Color color)
	{
		TweenColor tweenColor = UITweener.Begin<TweenColor>(go, duration);
		tweenColor.from = tweenColor.value;
		tweenColor.to = color;
		if (duration <= 0f)
		{
			tweenColor.Sample(1f, true);
			tweenColor.enabled = false;
		}
		return tweenColor;
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	public Color from = Color.white;

	public Color to = Color.white;

	private Boolean mCached;

	private UIWidget mWidget;

	private Material mMat;

	private Light mLight;

	private SpriteRenderer mSr;
}
