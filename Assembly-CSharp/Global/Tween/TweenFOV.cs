using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/Tween/Tween Field of View")]
public class TweenFOV : UITweener
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
    public Single fov
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
            return this.cachedCamera.fieldOfView;
        }
        set
        {
            this.cachedCamera.fieldOfView = value;
        }
    }

    protected override void OnUpdate(Single factor, Boolean isFinished)
    {
        this.value = this.from * (1f - factor) + this.to * factor;
    }

    public static TweenFOV Begin(GameObject go, Single duration, Single to)
    {
        TweenFOV tweenFOV = UITweener.Begin<TweenFOV>(go, duration);
        tweenFOV.from = tweenFOV.value;
        tweenFOV.to = to;
        if (duration <= 0f)
        {
            tweenFOV.Sample(1f, true);
            tweenFOV.enabled = false;
        }
        return tweenFOV;
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

    public Single from = 45f;

    public Single to = 45f;

    private Camera mCam;
}
