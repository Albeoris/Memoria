using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/Interaction/Draggable Camera")]
public class UIDraggableCamera : MonoBehaviour
{
    public Vector2 currentMomentum
    {
        get
        {
            return this.mMomentum;
        }
        set
        {
            this.mMomentum = value;
        }
    }

    private void Start()
    {
        this.mCam = base.GetComponent<Camera>();
        this.mTrans = base.transform;
        this.mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
        if (this.rootForBounds == (UnityEngine.Object)null)
        {
            global::Debug.LogError(NGUITools.GetHierarchy(base.gameObject) + " needs the 'Root For Bounds' parameter to be set", this);
            base.enabled = false;
        }
    }

    private Vector3 CalculateConstrainOffset()
    {
        if (this.rootForBounds == (UnityEngine.Object)null || this.rootForBounds.childCount == 0)
        {
            return Vector3.zero;
        }
        Vector3 vector = new Vector3(this.mCam.rect.xMin * (Single)Screen.width, this.mCam.rect.yMin * (Single)Screen.height, 0f);
        Vector3 vector2 = new Vector3(this.mCam.rect.xMax * (Single)Screen.width, this.mCam.rect.yMax * (Single)Screen.height, 0f);
        vector = this.mCam.ScreenToWorldPoint(vector);
        vector2 = this.mCam.ScreenToWorldPoint(vector2);
        Vector2 minRect = new Vector2(this.mBounds.min.x, this.mBounds.min.y);
        Vector2 maxRect = new Vector2(this.mBounds.max.x, this.mBounds.max.y);
        return NGUIMath.ConstrainRect(minRect, maxRect, vector, vector2);
    }

    public Boolean ConstrainToBounds(Boolean immediate)
    {
        if (this.mTrans != (UnityEngine.Object)null && this.rootForBounds != (UnityEngine.Object)null)
        {
            Vector3 b = this.CalculateConstrainOffset();
            if (b.sqrMagnitude > 0f)
            {
                if (immediate)
                {
                    this.mTrans.position -= b;
                }
                else
                {
                    SpringPosition springPosition = SpringPosition.Begin(base.gameObject, this.mTrans.position - b, 13f);
                    springPosition.ignoreTimeScale = true;
                    springPosition.worldSpace = true;
                }
                return true;
            }
        }
        return false;
    }

    public void Press(Boolean isPressed)
    {
        if (isPressed)
        {
            this.mDragStarted = false;
        }
        if (this.rootForBounds != (UnityEngine.Object)null)
        {
            this.mPressed = isPressed;
            if (isPressed)
            {
                this.mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(this.rootForBounds);
                this.mMomentum = Vector2.zero;
                this.mScroll = 0f;
                SpringPosition component = base.GetComponent<SpringPosition>();
                if (component != (UnityEngine.Object)null)
                {
                    component.enabled = false;
                }
            }
            else if (this.dragEffect == UIDragObject.DragEffect.MomentumAndSpring)
            {
                this.ConstrainToBounds(false);
            }
        }
    }

    public void Drag(Vector2 delta)
    {
        if (this.smoothDragStart && !this.mDragStarted)
        {
            this.mDragStarted = true;
            return;
        }
        UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
        if (this.mRoot != (UnityEngine.Object)null)
        {
            delta *= this.mRoot.pixelSizeAdjustment;
        }
        Vector2 vector = Vector2.Scale(delta, -this.scale);
        this.mTrans.localPosition += (Vector3)vector;
        this.mMomentum = Vector2.Lerp(this.mMomentum, this.mMomentum + vector * (0.01f * this.momentumAmount), 0.67f);
        if (this.dragEffect != UIDragObject.DragEffect.MomentumAndSpring && this.ConstrainToBounds(true))
        {
            this.mMomentum = Vector2.zero;
            this.mScroll = 0f;
        }
    }

    public void Scroll(Single delta)
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject))
        {
            if (Mathf.Sign(this.mScroll) != Mathf.Sign(delta))
            {
                this.mScroll = 0f;
            }
            this.mScroll += delta * this.scrollWheelFactor;
        }
    }

    private void Update()
    {
        Single deltaTime = RealTime.deltaTime;
        if (this.mPressed)
        {
            SpringPosition component = base.GetComponent<SpringPosition>();
            if (component != (UnityEngine.Object)null)
            {
                component.enabled = false;
            }
            this.mScroll = 0f;
        }
        else
        {
            this.mMomentum += this.scale * (this.mScroll * 20f);
            this.mScroll = NGUIMath.SpringLerp(this.mScroll, 0f, 20f, deltaTime);
            if (this.mMomentum.magnitude > 0.01f)
            {
                this.mTrans.localPosition += (Vector3)NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
                this.mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(this.rootForBounds);
                if (!this.ConstrainToBounds(this.dragEffect == UIDragObject.DragEffect.None))
                {
                    SpringPosition component2 = base.GetComponent<SpringPosition>();
                    if (component2 != (UnityEngine.Object)null)
                    {
                        component2.enabled = false;
                    }
                }
                return;
            }
            this.mScroll = 0f;
        }
        NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
    }

    public Transform rootForBounds;

    public Vector2 scale = Vector2.one;

    public Single scrollWheelFactor;

    public UIDragObject.DragEffect dragEffect = UIDragObject.DragEffect.MomentumAndSpring;

    public Boolean smoothDragStart = true;

    public Single momentumAmount = 35f;

    private Camera mCam;

    private Transform mTrans;

    private Boolean mPressed;

    private Vector2 mMomentum = Vector2.zero;

    private Bounds mBounds;

    private Single mScroll;

    private UIRoot mRoot;

    private Boolean mDragStarted;
}
