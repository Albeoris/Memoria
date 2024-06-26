using System;
using UnityEngine;

public class UI2DSpriteAnimation : MonoBehaviour
{
	public Boolean isPlaying
	{
		get
		{
			return base.enabled;
		}
	}

	public Int32 framesPerSecond
	{
		get
		{
			return this.framerate;
		}
		set
		{
			this.framerate = value;
		}
	}

	public void Play()
	{
		if (this.frames != null && (Int32)this.frames.Length > 0)
		{
			if (!base.enabled && !this.loop)
			{
				Int32 num = (Int32)((this.framerate <= 0) ? (this.mIndex - 1) : (this.mIndex + 1));
				if (num < 0 || num >= (Int32)this.frames.Length)
				{
					this.mIndex = (Int32)((this.framerate >= 0) ? 0 : ((Int32)this.frames.Length - 1));
				}
			}
			base.enabled = true;
			this.UpdateSprite();
		}
	}

	public void Pause()
	{
		base.enabled = false;
	}

	public void ResetToBeginning()
	{
		this.mIndex = (Int32)((this.framerate >= 0) ? 0 : ((Int32)this.frames.Length - 1));
		this.UpdateSprite();
	}

	private void Start()
	{
		this.Play();
	}

	private void Update()
	{
		if (this.frames == null || this.frames.Length == 0)
		{
			base.enabled = false;
		}
		else if (this.framerate != 0)
		{
			Single num = (!this.ignoreTimeScale) ? Time.time : RealTime.time;
			if (this.mUpdate < num)
			{
				this.mUpdate = num;
				Int32 num2 = (Int32)((this.framerate <= 0) ? (this.mIndex - 1) : (this.mIndex + 1));
				if (!this.loop && (num2 < 0 || num2 >= (Int32)this.frames.Length))
				{
					base.enabled = false;
					return;
				}
				this.mIndex = NGUIMath.RepeatIndex(num2, (Int32)this.frames.Length);
				this.UpdateSprite();
			}
		}
	}

	private void UpdateSprite()
	{
		if (this.mUnitySprite == (UnityEngine.Object)null && this.mNguiSprite == (UnityEngine.Object)null)
		{
			this.mUnitySprite = base.GetComponent<SpriteRenderer>();
			this.mNguiSprite = base.GetComponent<UI2DSprite>();
			if (this.mUnitySprite == (UnityEngine.Object)null && this.mNguiSprite == (UnityEngine.Object)null)
			{
				base.enabled = false;
				return;
			}
		}
		Single num = (!this.ignoreTimeScale) ? Time.time : RealTime.time;
		if (this.framerate != 0)
		{
			this.mUpdate = num + Mathf.Abs(1f / (Single)this.framerate);
		}
		if (this.mUnitySprite != (UnityEngine.Object)null)
		{
			this.mUnitySprite.sprite = this.frames[this.mIndex];
		}
		else if (this.mNguiSprite != (UnityEngine.Object)null)
		{
			this.mNguiSprite.nextSprite = this.frames[this.mIndex];
		}
	}

	[SerializeField]
	protected Int32 framerate = 20;

	public Boolean ignoreTimeScale = true;

	public Boolean loop = true;

	public Sprite[] frames;

	private SpriteRenderer mUnitySprite;

	private UI2DSprite mNguiSprite;

	private Int32 mIndex;

	private Single mUpdate;
}
