using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/NGUI Scroll Bar")]
[ExecuteInEditMode]
public class UIScrollBar : UISlider
{
    [Obsolete("Use 'value' instead")]
    public Single scrollValue
    {
        get
        {
            return base.value;
        }
        set
        {
            base.value = value;
        }
    }

    public Single barSize
    {
        get
        {
            return this.mSize;
        }
        set
        {
            Single num = Mathf.Clamp01(value);
            if (this.mSize != num)
            {
                this.mSize = num;
                this.mIsDirty = true;
                if (NGUITools.GetActive(this))
                {
                    if (UIProgressBar.current == (UnityEngine.Object)null && this.onChange != null)
                    {
                        UIProgressBar.current = this;
                        EventDelegate.Execute(this.onChange);
                        UIProgressBar.current = (UIProgressBar)null;
                    }
                    this.ForceUpdate();
                }
            }
        }
    }

    protected override void Upgrade()
    {
        if (this.mDir != UIScrollBar.Direction.Upgraded)
        {
            this.mValue = this.mScroll;
            if (this.mDir == UIScrollBar.Direction.Horizontal)
            {
                this.mFill = (UIProgressBar.FillDirection)((!this.mInverted) ? UIProgressBar.FillDirection.LeftToRight : UIProgressBar.FillDirection.RightToLeft);
            }
            else
            {
                this.mFill = (UIProgressBar.FillDirection)((!this.mInverted) ? UIProgressBar.FillDirection.TopToBottom : UIProgressBar.FillDirection.BottomToTop);
            }
            this.mDir = UIScrollBar.Direction.Upgraded;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        if (this.mFG != (UnityEngine.Object)null && this.mFG.gameObject != base.gameObject)
        {
            if (!(this.mFG.GetComponent<Collider>() != (UnityEngine.Object)null) && !(this.mFG.GetComponent<Collider2D>() != (UnityEngine.Object)null))
            {
                return;
            }
            UIEventListener uieventListener = UIEventListener.Get(this.mFG.gameObject);
            UIEventListener uieventListener2 = uieventListener;
            uieventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onPress, new UIEventListener.BoolDelegate(base.OnPressForeground));
            UIEventListener uieventListener3 = uieventListener;
            uieventListener3.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener3.onDrag, new UIEventListener.VectorDelegate(base.OnDragForeground));
            this.mFG.autoResizeBoxCollider = true;
        }
    }

    public override Single LocalToValue(Vector2 localPos)
    {
        if (this.mFG == null)
            return base.LocalToValue(localPos);
        Single halfBarRange = Mathf.Clamp01(this.mSize) * 0.5f;
        Single pageStart = halfBarRange;
        Single pageEnd = 1f - halfBarRange;
        Vector3[] localCorners = this.mFG.localCorners;
        if (base.isHorizontal)
        {
            pageStart = Mathf.Lerp(localCorners[0].x, localCorners[2].x, pageStart);
            pageEnd = Mathf.Lerp(localCorners[0].x, localCorners[2].x, pageEnd);
            Single pageWidth = pageEnd - pageStart;
            if (pageWidth == 0f)
                return base.value;
            return base.isInverted ? ((pageEnd - localPos.x) / pageWidth) : ((localPos.x - pageStart) / pageWidth);
        }
        else
        {
            pageStart = Mathf.Lerp(localCorners[0].y, localCorners[1].y, pageStart);
            pageEnd = Mathf.Lerp(localCorners[3].y, localCorners[2].y, pageEnd);
            Single pageHeight = pageEnd - pageStart;
            if (pageHeight == 0f)
                return base.value;
            return base.isInverted ? ((pageEnd - localPos.y) / pageHeight) : ((localPos.y - pageStart) / pageHeight);
        }
    }

    public override void ForceUpdate()
    {
        if (this.mFG != (UnityEngine.Object)null)
        {
            this.mIsDirty = false;
            Single num = Mathf.Clamp01(this.mSize) * 0.5f;
            Single num2 = Mathf.Lerp(num, 1f - num, base.value);
            Single num3 = num2 - num;
            Single num4 = num2 + num;
            if (base.isHorizontal)
            {
                this.mFG.drawRegion = ((!base.isInverted) ? new Vector4(num3, 0f, num4, 1f) : new Vector4(1f - num4, 0f, 1f - num3, 1f));
            }
            else
            {
                this.mFG.drawRegion = ((!base.isInverted) ? new Vector4(0f, num3, 1f, num4) : new Vector4(0f, 1f - num4, 1f, 1f - num3));
            }
            if (this.thumb != (UnityEngine.Object)null)
            {
                Vector4 drawingDimensions = this.mFG.drawingDimensions;
                Vector3 position = new Vector3(Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, 0.5f), Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, 0.5f));
                base.SetThumbPosition(this.mFG.cachedTransform.TransformPoint(position));
            }
        }
        else
        {
            base.ForceUpdate();
        }
    }

    [SerializeField]
    [HideInInspector]
    protected Single mSize = 1f;

    [SerializeField]
    [HideInInspector]
    private Single mScroll;

    [HideInInspector]
    [SerializeField]
    private UIScrollBar.Direction mDir = UIScrollBar.Direction.Upgraded;

    private enum Direction
    {
        Horizontal,
        Vertical,
        Upgraded
    }
}
