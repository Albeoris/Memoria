using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/NGUI Slider")]
[ExecuteInEditMode]
public class UISlider : UIProgressBar
{
    public Boolean isColliderEnabled
    {
        get
        {
            Collider component = base.GetComponent<Collider>();
            if (component != (UnityEngine.Object)null)
            {
                return component.enabled;
            }
            Collider2D component2 = base.GetComponent<Collider2D>();
            return component2 != (UnityEngine.Object)null && component2.enabled;
        }
    }

    [Obsolete("Use 'value' instead")]
    public Single sliderValue
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

    [Obsolete("Use 'fillDirection' instead")]
    public Boolean inverted
    {
        get
        {
            return base.isInverted;
        }
        set
        {
        }
    }

    protected override void Upgrade()
    {
        if (this.direction != UISlider.Direction.Upgraded)
        {
            this.mValue = this.rawValue;
            if (this.foreground != (UnityEngine.Object)null)
            {
                this.mFG = this.foreground.GetComponent<UIWidget>();
            }
            if (this.direction == UISlider.Direction.Horizontal)
            {
                this.mFill = (UIProgressBar.FillDirection)((!this.mInverted) ? UIProgressBar.FillDirection.LeftToRight : UIProgressBar.FillDirection.RightToLeft);
            }
            else
            {
                this.mFill = (UIProgressBar.FillDirection)((!this.mInverted) ? UIProgressBar.FillDirection.BottomToTop : UIProgressBar.FillDirection.TopToBottom);
            }
            this.direction = UISlider.Direction.Upgraded;
        }
    }

    protected override void OnStart()
    {
        GameObject go = (!(this.mBG != (UnityEngine.Object)null) || (!(this.mBG.GetComponent<Collider>() != (UnityEngine.Object)null) && !(this.mBG.GetComponent<Collider2D>() != (UnityEngine.Object)null))) ? base.gameObject : this.mBG.gameObject;
        UIEventListener uieventListener = UIEventListener.Get(go);
        UIEventListener uieventListener2 = uieventListener;
        uieventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onPress, new UIEventListener.BoolDelegate(this.OnPressBackground));
        UIEventListener uieventListener3 = uieventListener;
        uieventListener3.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener3.onDrag, new UIEventListener.VectorDelegate(this.OnDragBackground));
        if (this.thumb != (UnityEngine.Object)null && (this.thumb.GetComponent<Collider>() != (UnityEngine.Object)null || this.thumb.GetComponent<Collider2D>() != (UnityEngine.Object)null) && (this.mFG == (UnityEngine.Object)null || this.thumb != this.mFG.cachedTransform))
        {
            UIEventListener uieventListener4 = UIEventListener.Get(this.thumb.gameObject);
            UIEventListener uieventListener5 = uieventListener4;
            uieventListener5.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener5.onPress, new UIEventListener.BoolDelegate(this.OnPressForeground));
            UIEventListener uieventListener6 = uieventListener4;
            uieventListener6.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener6.onDrag, new UIEventListener.VectorDelegate(this.OnDragForeground));
        }
    }

    protected void OnPressBackground(GameObject go, Boolean isPressed)
    {
        if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
        {
            return;
        }
        this.mCam = UICamera.currentCamera;
        base.value = base.ScreenToValue(UICamera.lastEventPosition);
        if (!isPressed && this.onDragFinished != null)
        {
            this.onDragFinished();
        }
    }

    protected void OnDragBackground(GameObject go, Vector2 delta)
    {
        if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
        {
            return;
        }
        this.mCam = UICamera.currentCamera;
        base.value = base.ScreenToValue(UICamera.lastEventPosition);
    }

    protected void OnPressForeground(GameObject go, Boolean isPressed)
    {
        if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
        {
            return;
        }
        this.mCam = UICamera.currentCamera;
        if (isPressed)
        {
            this.mOffset = ((!(this.mFG == (UnityEngine.Object)null)) ? (base.value - base.ScreenToValue(UICamera.lastEventPosition)) : 0f);
        }
        else if (this.onDragFinished != null)
        {
            this.onDragFinished();
        }
    }

    protected void OnDragForeground(GameObject go, Vector2 delta)
    {
        if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
        {
            return;
        }
        this.mCam = UICamera.currentCamera;
        base.value = this.mOffset + base.ScreenToValue(UICamera.lastEventPosition);
    }

    public override void OnPan(Vector2 delta)
    {
        if (base.enabled && this.isColliderEnabled)
        {
            base.OnPan(delta);
        }
    }

    [SerializeField]
    [HideInInspector]
    private Transform foreground;

    [HideInInspector]
    [SerializeField]
    private Single rawValue = 1f;

    [SerializeField]
    [HideInInspector]
    private UISlider.Direction direction = UISlider.Direction.Upgraded;

    [SerializeField]
    [HideInInspector]
    protected Boolean mInverted;

    private enum Direction
    {
        Horizontal,
        Vertical,
        Upgraded
    }
}
