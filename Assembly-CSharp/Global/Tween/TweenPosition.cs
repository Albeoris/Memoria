using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Position")]
public class TweenPosition : UITweener
{
    public Transform cachedTransform
    {
        get
        {
            if (this.mTrans == (UnityEngine.Object)null)
            {
                this.mTrans = base.transform;
            }
            return this.mTrans;
        }
    }

    [Obsolete("Use 'value' instead")]
    public Vector3 position
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

    public Vector3 value
    {
        get
        {
            return (!this.worldSpace) ? this.cachedTransform.localPosition : this.cachedTransform.position;
        }
        set
        {
            if (this.mRect == (UnityEngine.Object)null || !this.mRect.isAnchored || this.worldSpace)
            {
                if (this.worldSpace)
                {
                    this.cachedTransform.position = value;
                }
                else
                {
                    this.cachedTransform.localPosition = value;
                }
            }
            else
            {
                value -= this.cachedTransform.localPosition;
                NGUIMath.MoveRect(this.mRect, value.x, value.y);
            }
        }
    }

    private void Awake()
    {
        this.mRect = base.GetComponent<UIRect>();
    }

    protected override void OnUpdate(Single factor, Boolean isFinished)
    {
        this.value = this.from * (1f - factor) + this.to * factor;
    }

    public static TweenPosition Begin(GameObject go, Single duration, Vector3 pos)
    {
        TweenPosition tweenPosition = UITweener.Begin<TweenPosition>(go, duration);
        tweenPosition.from = tweenPosition.value;
        tweenPosition.to = pos;
        if (duration <= 0f)
        {
            tweenPosition.Sample(1f, true);
            tweenPosition.enabled = false;
        }
        return tweenPosition;
    }

    public static TweenPosition Begin(GameObject go, Single duration, Vector3 pos, Boolean worldSpace)
    {
        TweenPosition tweenPosition = UITweener.Begin<TweenPosition>(go, duration);
        tweenPosition.worldSpace = worldSpace;
        tweenPosition.from = tweenPosition.value;
        tweenPosition.to = pos;
        if (duration <= 0f)
        {
            tweenPosition.Sample(1f, true);
            tweenPosition.enabled = false;
        }
        return tweenPosition;
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

    public Vector3 from;

    public Vector3 to;

    [HideInInspector]
    public Boolean worldSpace;

    private Transform mTrans;

    private UIRect mRect;
}
