using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Orthographic Size")]
[RequireComponent(typeof(Camera))]
public class TweenOrthoSize : UITweener
{
    public Camera cachedCamera
    {
        get
        {
            if (this.mCam == (UnityEngine.Object)null)
            {
                this.mCam = base.GetComponent<Camera>();
            }
            return this.mCam;
        }
    }

    [Obsolete("Use 'value' instead")]
    public Single orthoSize
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
            return this.cachedCamera.orthographicSize;
        }
        set
        {
            this.cachedCamera.orthographicSize = value;
        }
    }

    protected override void OnUpdate(Single factor, Boolean isFinished)
    {
        this.value = this.from * (1f - factor) + this.to * factor;
    }

    public static TweenOrthoSize Begin(GameObject go, Single duration, Single to)
    {
        TweenOrthoSize tweenOrthoSize = UITweener.Begin<TweenOrthoSize>(go, duration);
        tweenOrthoSize.from = tweenOrthoSize.value;
        tweenOrthoSize.to = to;
        if (duration <= 0f)
        {
            tweenOrthoSize.Sample(1f, true);
            tweenOrthoSize.enabled = false;
        }
        return tweenOrthoSize;
    }

    public override void SetStartToCurrentValue()
    {
        this.from = this.value;
    }

    public override void SetEndToCurrentValue()
    {
        this.to = this.value;
    }

    public Single from = 1f;

    public Single to = 1f;

    private Camera mCam;
}
