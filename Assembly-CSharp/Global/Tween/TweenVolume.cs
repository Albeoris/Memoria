using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("NGUI/Tween/Tween Volume")]
public class TweenVolume : UITweener
{
    public AudioSource audioSource
    {
        get
        {
            if (this.mSource == (UnityEngine.Object)null)
            {
                this.mSource = base.GetComponent<AudioSource>();
                if (this.mSource == (UnityEngine.Object)null)
                {
                    this.mSource = base.GetComponent<AudioSource>();
                    if (this.mSource == (UnityEngine.Object)null)
                    {
                        global::Debug.LogError("TweenVolume needs an AudioSource to work with", this);
                        base.enabled = false;
                    }
                }
            }
            return this.mSource;
        }
    }

    [Obsolete("Use 'value' instead")]
    public Single volume
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

    public Single value
    {
        get
        {
            return (!(this.audioSource != (UnityEngine.Object)null)) ? 0f : this.mSource.volume;
        }
        set
        {
            if (this.audioSource != (UnityEngine.Object)null)
            {
                this.mSource.volume = value;
            }
        }
    }

    protected override void OnUpdate(Single factor, Boolean isFinished)
    {
        this.value = this.from * (1f - factor) + this.to * factor;
        this.mSource.enabled = (this.mSource.volume > 0.01f);
    }

    public static TweenVolume Begin(GameObject go, Single duration, Single targetVolume)
    {
        TweenVolume tweenVolume = UITweener.Begin<TweenVolume>(go, duration);
        tweenVolume.from = tweenVolume.value;
        tweenVolume.to = targetVolume;
        return tweenVolume;
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

    private AudioSource mSource;
}
