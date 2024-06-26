using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite Animation")]
[RequireComponent(typeof(UISprite))]
[ExecuteInEditMode]
public class UISpriteAnimation : MonoBehaviour
{
	public Int32 frames
	{
		get
		{
			return this.mSpriteNames.Count;
		}
	}

	public Int32 framesPerSecond
	{
		get
		{
			return this.mFPS;
		}
		set
		{
			this.mFPS = value;
		}
	}

	public String namePrefix
	{
		get
		{
			return this.mPrefix;
		}
		set
		{
			if (this.mPrefix != value)
			{
				this.mPrefix = value;
				this.RebuildSpriteList();
			}
		}
	}

	public Boolean loop
	{
		get
		{
			return this.mLoop;
		}
		set
		{
			this.mLoop = value;
		}
	}

	public Boolean isPlaying
	{
		get
		{
			return this.mActive;
		}
	}

	protected virtual void Start()
	{
		this.RebuildSpriteList();
	}

	protected virtual void Update()
	{
		if (this.mActive && this.mSpriteNames.Count > 1 && Application.isPlaying && this.mFPS > 0)
		{
			this.mDelta += RealTime.deltaTime;
			Single num = 1f / (Single)this.mFPS;
			if (num < this.mDelta)
			{
				this.mDelta = ((num <= 0f) ? 0f : (this.mDelta - num));
				if (++this.mIndex >= this.mSpriteNames.Count)
				{
					this.mIndex = 0;
					this.mActive = this.mLoop;
				}
				if (this.mActive)
				{
					this.mSprite.spriteName = this.mSpriteNames[this.mIndex];
					if (this.mSnap)
					{
						this.mSprite.MakePixelPerfect();
					}
				}
			}
		}
	}

	public void RebuildSpriteList()
	{
		if (this.mSprite == (UnityEngine.Object)null)
		{
			this.mSprite = base.GetComponent<UISprite>();
		}
		this.mSpriteNames.Clear();
		if (this.mSprite != (UnityEngine.Object)null && this.mSprite.atlas != (UnityEngine.Object)null)
		{
			List<UISpriteData> spriteList = this.mSprite.atlas.spriteList;
			Int32 i = 0;
			Int32 count = spriteList.Count;
			while (i < count)
			{
				UISpriteData uispriteData = spriteList[i];
				if (String.IsNullOrEmpty(this.mPrefix) || uispriteData.name.StartsWith(this.mPrefix))
				{
					this.mSpriteNames.Add(uispriteData.name);
				}
				i++;
			}
			this.mSpriteNames.Sort();
		}
	}

	public void Play()
	{
		this.mActive = true;
	}

	public void Pause()
	{
		this.mActive = false;
	}

	public void ResetToBeginning()
	{
		this.mActive = true;
		this.mIndex = 0;
		if (this.mSprite != (UnityEngine.Object)null && this.mSpriteNames.Count > 0)
		{
			this.mSprite.spriteName = this.mSpriteNames[this.mIndex];
			if (this.mSnap)
			{
				this.mSprite.MakePixelPerfect();
			}
		}
	}

	[SerializeField]
	[HideInInspector]
	protected Int32 mFPS = 30;

	[HideInInspector]
	[SerializeField]
	protected String mPrefix = String.Empty;

	[SerializeField]
	[HideInInspector]
	protected Boolean mLoop = true;

	[HideInInspector]
	[SerializeField]
	protected Boolean mSnap = true;

	protected UISprite mSprite;

	protected Single mDelta;

	protected Int32 mIndex;

	protected Boolean mActive = true;

	protected List<String> mSpriteNames = new List<String>();
}
