using System;
using UnityEngine;

[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/Tween/Tween Width")]
public class TweenWidth : UITweener
{
    public UIWidget cachedWidget
    {
        get
        {
            if (this.mWidget == (UnityEngine.Object)null)
            {
                this.mWidget = base.GetComponent<UIWidget>();
            }
            return this.mWidget;
        }
    }

    [Obsolete("Use 'value' instead")]
    public Int32 width
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

    public Int32 value
    {
        get
        {
            return this.cachedWidget.width;
        }
        set
        {
            this.cachedWidget.width = value;
        }
    }

    protected override void OnUpdate(Single factor, Boolean isFinished)
    {
        this.value = Mathf.RoundToInt((Single)this.from * (1f - factor) + (Single)this.to * factor);
        if (this.updateTable)
        {
            if (this.mTable == (UnityEngine.Object)null)
            {
                this.mTable = NGUITools.FindInParents<UITable>(base.gameObject);
                if (this.mTable == (UnityEngine.Object)null)
                {
                    this.updateTable = false;
                    return;
                }
            }
            this.mTable.repositionNow = true;
        }
    }

    public static TweenWidth Begin(UIWidget widget, Single duration, Int32 width)
    {
        TweenWidth tweenWidth = UITweener.Begin<TweenWidth>(widget.gameObject, duration);
        tweenWidth.from = widget.width;
        tweenWidth.to = width;
        if (duration <= 0f)
        {
            tweenWidth.Sample(1f, true);
            tweenWidth.enabled = false;
        }
        return tweenWidth;
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

    public Int32 from = 100;

    public Int32 to = 100;

    public Boolean updateTable;

    private UIWidget mWidget;

    private UITable mTable;
}
