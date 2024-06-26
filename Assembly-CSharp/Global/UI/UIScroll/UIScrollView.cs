using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Scroll View")]
[ExecuteInEditMode]
[RequireComponent(typeof(UIPanel))]
public class UIScrollView : MonoBehaviour
{
    public UIPanel panel
    {
        get
        {
            return this.mPanel;
        }
    }

    public Boolean isDragging
    {
        get
        {
            return this.mPressed && this.mDragStarted;
        }
    }

    public virtual Bounds bounds
    {
        get
        {
            if (!this.mCalculatedBounds)
            {
                this.mCalculatedBounds = true;
                this.mTrans = base.transform;
                this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.mTrans, this.mTrans);
            }
            return this.mBounds;
        }
    }

    public Boolean canMoveHorizontally
    {
        get
        {
            return this.movement == UIScrollView.Movement.Horizontal || this.movement == UIScrollView.Movement.Unrestricted || (this.movement == UIScrollView.Movement.Custom && this.customMovement.x != 0f);
        }
    }

    public Boolean canMoveVertically
    {
        get
        {
            return this.movement == UIScrollView.Movement.Vertical || this.movement == UIScrollView.Movement.Unrestricted || (this.movement == UIScrollView.Movement.Custom && this.customMovement.y != 0f);
        }
    }

    public virtual Boolean shouldMoveHorizontally
    {
        get
        {
            Single num = this.bounds.size.x;
            if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
            {
                num += this.mPanel.clipSoftness.x * 2f;
            }
            return Mathf.RoundToInt(num - this.mPanel.width) > 0;
        }
    }

    public virtual Boolean shouldMoveVertically
    {
        get
        {
            Single num = this.bounds.size.y;
            if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
            {
                num += this.mPanel.clipSoftness.y * 2f;
            }
            return Mathf.RoundToInt(num - this.mPanel.height) > 0;
        }
    }

    protected virtual Boolean shouldMove
    {
        get
        {
            if (!this.disableDragIfFits)
            {
                return true;
            }
            if (this.mPanel == (UnityEngine.Object)null)
            {
                this.mPanel = base.GetComponent<UIPanel>();
            }
            Vector4 finalClipRegion = this.mPanel.finalClipRegion;
            Bounds bounds = this.bounds;
            Single num = (finalClipRegion.z != 0f) ? (finalClipRegion.z * 0.5f) : ((Single)Screen.width);
            Single num2 = (finalClipRegion.w != 0f) ? (finalClipRegion.w * 0.5f) : ((Single)Screen.height);
            if (this.canMoveHorizontally)
            {
                if (bounds.min.x < finalClipRegion.x - num)
                {
                    return true;
                }
                if (bounds.max.x > finalClipRegion.x + num)
                {
                    return true;
                }
            }
            if (this.canMoveVertically)
            {
                if (bounds.min.y < finalClipRegion.y - num2)
                {
                    return true;
                }
                if (bounds.max.y > finalClipRegion.y + num2)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public Vector3 currentMomentum
    {
        get
        {
            return this.mMomentum;
        }
        set
        {
            this.mMomentum = value;
            this.mShouldMove = true;
        }
    }

    private void Awake()
    {
        this.mTrans = base.transform;
        this.mPanel = base.GetComponent<UIPanel>();
        if (this.mPanel.clipping == UIDrawCall.Clipping.None)
        {
            this.mPanel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
        }
        if (this.movement != UIScrollView.Movement.Custom && this.scale.sqrMagnitude > 0.001f)
        {
            if (this.scale.x == 1f && this.scale.y == 0f)
            {
                this.movement = UIScrollView.Movement.Horizontal;
            }
            else if (this.scale.x == 0f && this.scale.y == 1f)
            {
                this.movement = UIScrollView.Movement.Vertical;
            }
            else if (this.scale.x == 1f && this.scale.y == 1f)
            {
                this.movement = UIScrollView.Movement.Unrestricted;
            }
            else
            {
                this.movement = UIScrollView.Movement.Custom;
                this.customMovement.x = this.scale.x;
                this.customMovement.y = this.scale.y;
            }
            this.scale = Vector3.zero;
        }
        if (this.contentPivot == UIWidget.Pivot.TopLeft && this.relativePositionOnReset != Vector2.zero)
        {
            this.contentPivot = NGUIMath.GetPivot(new Vector2(this.relativePositionOnReset.x, 1f - this.relativePositionOnReset.y));
            this.relativePositionOnReset = Vector2.zero;
        }
    }

    private void OnEnable()
    {
        UIScrollView.list.Add(this);
        if (this.mStarted && Application.isPlaying)
        {
            this.CheckScrollbars();
        }
    }

    private void Start()
    {
        this.mStarted = true;
        if (Application.isPlaying)
        {
            this.CheckScrollbars();
        }
    }

    private void CheckScrollbars()
    {
        if (this.horizontalScrollBar != (UnityEngine.Object)null)
        {
            EventDelegate.Add(this.horizontalScrollBar.onChange, new EventDelegate.Callback(this.OnScrollBar));
            this.horizontalScrollBar.BroadcastMessage("CacheDefaultColor", SendMessageOptions.DontRequireReceiver);
            this.horizontalScrollBar.alpha = ((this.showScrollBars != UIScrollView.ShowCondition.Always && !this.shouldMoveHorizontally) ? 0f : 1f);
        }
        if (this.verticalScrollBar != (UnityEngine.Object)null)
        {
            EventDelegate.Add(this.verticalScrollBar.onChange, new EventDelegate.Callback(this.OnScrollBar));
            this.verticalScrollBar.BroadcastMessage("CacheDefaultColor", SendMessageOptions.DontRequireReceiver);
            this.verticalScrollBar.alpha = ((this.showScrollBars != UIScrollView.ShowCondition.Always && !this.shouldMoveVertically) ? 0f : 1f);
        }
    }

    private void OnDisable()
    {
        UIScrollView.list.Remove(this);
    }

    public Boolean RestrictWithinBounds(Boolean instant)
    {
        return this.RestrictWithinBounds(instant, true, true);
    }

    public Boolean RestrictWithinBounds(Boolean instant, Boolean horizontal, Boolean vertical)
    {
        if (this.mPanel == (UnityEngine.Object)null)
        {
            return false;
        }
        Bounds bounds = this.bounds;
        Vector3 vector = this.mPanel.CalculateConstrainOffset(bounds.min, bounds.max);
        if (!horizontal)
        {
            vector.x = 0f;
        }
        if (!vertical)
        {
            vector.y = 0f;
        }
        if (vector.sqrMagnitude > 0.1f)
        {
            if (!instant && this.dragEffect == UIScrollView.DragEffect.MomentumAndSpring)
            {
                Vector3 pos = this.mTrans.localPosition + vector;
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);
                SpringPanel.Begin(this.mPanel.gameObject, pos, 8f);
            }
            else
            {
                this.MoveRelative(vector);
                if (Mathf.Abs(vector.x) > 0.01f)
                {
                    this.mMomentum.x = 0f;
                }
                if (Mathf.Abs(vector.y) > 0.01f)
                {
                    this.mMomentum.y = 0f;
                }
                if (Mathf.Abs(vector.z) > 0.01f)
                {
                    this.mMomentum.z = 0f;
                }
                this.mScroll = 0f;
            }
            return true;
        }
        return false;
    }

    public void DisableSpring()
    {
        SpringPanel component = base.GetComponent<SpringPanel>();
        if (component != (UnityEngine.Object)null)
        {
            component.enabled = false;
        }
    }

    public void UpdateScrollbars()
    {
        this.UpdateScrollbars(true);
    }

    public virtual void UpdateScrollbars(Boolean recalculateBounds)
    {
        if (this.mPanel == (UnityEngine.Object)null)
        {
            return;
        }
        if (this.horizontalScrollBar != (UnityEngine.Object)null || this.verticalScrollBar != (UnityEngine.Object)null)
        {
            if (recalculateBounds)
            {
                this.mCalculatedBounds = false;
                this.mShouldMove = this.shouldMove;
            }
            Bounds bounds = this.bounds;
            Vector2 vector = bounds.min;
            Vector2 vector2 = bounds.max;
            if (this.horizontalScrollBar != (UnityEngine.Object)null && vector2.x > vector.x)
            {
                Vector4 finalClipRegion = this.mPanel.finalClipRegion;
                Int32 num = Mathf.RoundToInt(finalClipRegion.z);
                if ((num & 1) != 0)
                {
                    num--;
                }
                Single num2 = (Single)num * 0.5f;
                num2 = Mathf.Round(num2);
                if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
                {
                    num2 -= this.mPanel.clipSoftness.x;
                }
                Single contentSize = vector2.x - vector.x;
                Single viewSize = num2 * 2f;
                Single num3 = vector.x;
                Single num4 = vector2.x;
                Single num5 = finalClipRegion.x - num2;
                Single num6 = finalClipRegion.x + num2;
                num3 = num5 - num3;
                num4 -= num6;
                this.UpdateScrollbars(this.horizontalScrollBar, num3, num4, contentSize, viewSize, false);
            }
            if (this.verticalScrollBar != (UnityEngine.Object)null && vector2.y > vector.y)
            {
                Vector4 finalClipRegion2 = this.mPanel.finalClipRegion;
                Int32 num7 = Mathf.RoundToInt(finalClipRegion2.w);
                if ((num7 & 1) != 0)
                {
                    num7--;
                }
                Single num8 = (Single)num7 * 0.5f;
                num8 = Mathf.Round(num8);
                if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
                {
                    num8 -= this.mPanel.clipSoftness.y;
                }
                Single contentSize2 = vector2.y - vector.y;
                Single viewSize2 = num8 * 2f;
                Single num9 = vector.y;
                Single num10 = vector2.y;
                Single num11 = finalClipRegion2.y - num8;
                Single num12 = finalClipRegion2.y + num8;
                num9 = num11 - num9;
                num10 -= num12;
                this.UpdateScrollbars(this.verticalScrollBar, num9, num10, contentSize2, viewSize2, true);
            }
        }
        else if (recalculateBounds)
        {
            this.mCalculatedBounds = false;
        }
    }

    protected void UpdateScrollbars(UIProgressBar slider, Single contentMin, Single contentMax, Single contentSize, Single viewSize, Boolean inverted)
    {
        if (slider == (UnityEngine.Object)null)
        {
            return;
        }
        this.mIgnoreCallbacks = true;
        Single num;
        if (viewSize < contentSize)
        {
            contentMin = Mathf.Clamp01(contentMin / contentSize);
            contentMax = Mathf.Clamp01(contentMax / contentSize);
            num = contentMin + contentMax;
            slider.value = ((!inverted) ? ((num <= 0.001f) ? 1f : (contentMin / num)) : ((num <= 0.001f) ? 0f : (1f - contentMin / num)));
        }
        else
        {
            contentMin = Mathf.Clamp01(-contentMin / contentSize);
            contentMax = Mathf.Clamp01(-contentMax / contentSize);
            num = contentMin + contentMax;
            slider.value = ((!inverted) ? ((num <= 0.001f) ? 1f : (contentMin / num)) : ((num <= 0.001f) ? 0f : (1f - contentMin / num)));
            if (contentSize > 0f)
            {
                contentMin = Mathf.Clamp01(contentMin / contentSize);
                contentMax = Mathf.Clamp01(contentMax / contentSize);
                num = contentMin + contentMax;
            }
        }
        UIScrollBar uiscrollBar = slider as UIScrollBar;
        if (uiscrollBar != (UnityEngine.Object)null)
        {
            uiscrollBar.barSize = 1f - num;
        }
        this.mIgnoreCallbacks = false;
    }

    public virtual void SetDragAmount(Single x, Single y, Boolean updateScrollbars)
    {
        if (this.mPanel == (UnityEngine.Object)null)
        {
            this.mPanel = base.GetComponent<UIPanel>();
        }
        this.DisableSpring();
        Bounds bounds = this.bounds;
        if (bounds.min.x == bounds.max.x || bounds.min.y == bounds.max.y)
        {
            return;
        }
        Vector4 finalClipRegion = this.mPanel.finalClipRegion;
        Single num = finalClipRegion.z * 0.5f;
        Single num2 = finalClipRegion.w * 0.5f;
        Single num3 = bounds.min.x + num;
        Single num4 = bounds.max.x - num;
        Single num5 = bounds.min.y + num2;
        Single num6 = bounds.max.y - num2;
        if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
        {
            num3 -= this.mPanel.clipSoftness.x;
            num4 += this.mPanel.clipSoftness.x;
            num5 -= this.mPanel.clipSoftness.y;
            num6 += this.mPanel.clipSoftness.y;
        }
        Single num7 = Mathf.Lerp(num3, num4, x);
        Single num8 = Mathf.Lerp(num6, num5, y);
        if (!updateScrollbars)
        {
            Vector3 localPosition = this.mTrans.localPosition;
            if (this.canMoveHorizontally)
            {
                localPosition.x += finalClipRegion.x - num7;
            }
            if (this.canMoveVertically)
            {
                localPosition.y += finalClipRegion.y - num8;
            }
            this.mTrans.localPosition = localPosition;
        }
        if (this.canMoveHorizontally)
        {
            finalClipRegion.x = num7;
        }
        if (this.canMoveVertically)
        {
            finalClipRegion.y = num8;
        }
        Vector4 baseClipRegion = this.mPanel.baseClipRegion;
        this.mPanel.clipOffset = new Vector2(finalClipRegion.x - baseClipRegion.x, finalClipRegion.y - baseClipRegion.y);
        if (updateScrollbars)
        {
            this.UpdateScrollbars(this.mDragID == -10);
        }
    }

    public void InvalidateBounds()
    {
        this.mCalculatedBounds = false;
    }

    [ContextMenu("Reset Clipping Position")]
    public void ResetPosition()
    {
        if (NGUITools.GetActive(this))
        {
            this.mCalculatedBounds = false;
            Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.contentPivot);
            this.SetDragAmount(pivotOffset.x, 1f - pivotOffset.y, false);
            this.SetDragAmount(pivotOffset.x, 1f - pivotOffset.y, true);
        }
    }

    public void UpdatePosition()
    {
        if (!this.mIgnoreCallbacks && (this.horizontalScrollBar != (UnityEngine.Object)null || this.verticalScrollBar != (UnityEngine.Object)null))
        {
            this.mIgnoreCallbacks = true;
            this.mCalculatedBounds = false;
            Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.contentPivot);
            Single x = (!(this.horizontalScrollBar != (UnityEngine.Object)null)) ? pivotOffset.x : this.horizontalScrollBar.value;
            Single y = (!(this.verticalScrollBar != (UnityEngine.Object)null)) ? (1f - pivotOffset.y) : this.verticalScrollBar.value;
            this.SetDragAmount(x, y, false);
            this.UpdateScrollbars(true);
            this.mIgnoreCallbacks = false;
        }
    }

    public void OnScrollBar()
    {
        if (!this.mIgnoreCallbacks)
        {
            this.mIgnoreCallbacks = true;
            Single x = (!(this.horizontalScrollBar != (UnityEngine.Object)null)) ? 0f : this.horizontalScrollBar.value;
            Single y = (!(this.verticalScrollBar != (UnityEngine.Object)null)) ? 0f : this.verticalScrollBar.value;
            this.SetDragAmount(x, y, false);
            this.mIgnoreCallbacks = false;
        }
    }

    public virtual void MoveRelative(Vector3 relative)
    {
        this.mTrans.localPosition += relative;
        Vector2 clipOffset = this.mPanel.clipOffset;
        clipOffset.x -= relative.x;
        clipOffset.y -= relative.y;
        this.mPanel.clipOffset = clipOffset;
        this.UpdateScrollbars(false);
    }

    public void MoveAbsolute(Vector3 absolute)
    {
        Vector3 a = this.mTrans.InverseTransformPoint(absolute);
        Vector3 b = this.mTrans.InverseTransformPoint(Vector3.zero);
        this.MoveRelative(a - b);
    }

    public void Press(Boolean pressed)
    {
        if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
        {
            return;
        }
        if (this.smoothDragStart && pressed)
        {
            this.mDragStarted = false;
            this.mDragStartOffset = Vector2.zero;
        }
        if (base.enabled && NGUITools.GetActive(base.gameObject))
        {
            if (!pressed && this.mDragID == UICamera.currentTouchID)
            {
                this.mDragID = -10;
            }
            this.mCalculatedBounds = false;
            this.mShouldMove = this.shouldMove;
            if (!this.mShouldMove)
            {
                return;
            }
            this.mPressed = pressed;
            if (pressed)
            {
                this.mMomentum = Vector3.zero;
                this.mScroll = 0f;
                this.DisableSpring();
                this.mLastPos = UICamera.lastWorldPosition;
                this.mPlane = new Plane(this.mTrans.rotation * Vector3.back, this.mLastPos);
                Vector2 clipOffset = this.mPanel.clipOffset;
                clipOffset.x = Mathf.Round(clipOffset.x);
                clipOffset.y = Mathf.Round(clipOffset.y);
                this.mPanel.clipOffset = clipOffset;
                Vector3 localPosition = this.mTrans.localPosition;
                localPosition.x = Mathf.Round(localPosition.x);
                localPosition.y = Mathf.Round(localPosition.y);
                this.mTrans.localPosition = localPosition;
                if (!this.smoothDragStart)
                {
                    this.mDragStarted = true;
                    this.mDragStartOffset = Vector2.zero;
                    if (this.onDragStarted != null)
                    {
                        this.onDragStarted();
                    }
                }
            }
            else if (this.centerOnChild)
            {
                this.centerOnChild.Recenter();
            }
            else
            {
                if (this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None)
                {
                    this.RestrictWithinBounds(this.dragEffect == UIScrollView.DragEffect.None, this.canMoveHorizontally, this.canMoveVertically);
                }
                if (this.mDragStarted && this.onDragFinished != null)
                {
                    this.onDragFinished();
                }
                if (!this.mShouldMove && this.onStoppedMoving != null)
                {
                    this.onStoppedMoving();
                }
            }
        }
    }

    public void Drag()
    {
        if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
        {
            return;
        }
        if (base.enabled && NGUITools.GetActive(base.gameObject) && this.mShouldMove)
        {
            if (this.mDragID == -10)
            {
                this.mDragID = UICamera.currentTouchID;
            }
            UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
            if (this.smoothDragStart && !this.mDragStarted)
            {
                this.mDragStarted = true;
                this.mDragStartOffset = UICamera.currentTouch.totalDelta;
                if (this.onDragStarted != null)
                {
                    this.onDragStarted();
                }
            }
            Ray ray = (!this.smoothDragStart) ? UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos) : UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos - this.mDragStartOffset);
            Single distance = 0f;
            if (this.mPlane.Raycast(ray, out distance))
            {
                Vector3 point = ray.GetPoint(distance);
                Vector3 vector = point - this.mLastPos;
                this.mLastPos = point;
                if (vector.x != 0f || vector.y != 0f || vector.z != 0f)
                {
                    vector = this.mTrans.InverseTransformDirection(vector);
                    if (this.movement == UIScrollView.Movement.Horizontal)
                    {
                        vector.y = 0f;
                        vector.z = 0f;
                    }
                    else if (this.movement == UIScrollView.Movement.Vertical)
                    {
                        vector.x = 0f;
                        vector.z = 0f;
                    }
                    else if (this.movement == UIScrollView.Movement.Unrestricted)
                    {
                        vector.z = 0f;
                    }
                    else
                    {
                        vector.Scale(this.customMovement);
                    }
                    vector = this.mTrans.TransformDirection(vector);
                }
                if (this.dragEffect == UIScrollView.DragEffect.None)
                {
                    this.mMomentum = Vector3.zero;
                }
                else
                {
                    this.mMomentum = Vector3.Lerp(this.mMomentum, this.mMomentum + vector * (0.01f * this.momentumAmount), 0.67f);
                }
                if (!this.iOSDragEmulation || this.dragEffect != UIScrollView.DragEffect.MomentumAndSpring)
                {
                    this.MoveAbsolute(vector);
                }
                else if (this.mPanel.CalculateConstrainOffset(this.bounds.min, this.bounds.max).magnitude > 1f)
                {
                    this.MoveAbsolute(vector * 0.5f);
                    this.mMomentum *= 0.5f;
                }
                else
                {
                    this.MoveAbsolute(vector);
                }
                if (this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None && this.dragEffect != UIScrollView.DragEffect.MomentumAndSpring)
                {
                    this.RestrictWithinBounds(true, this.canMoveHorizontally, this.canMoveVertically);
                }
            }
        }
    }

    public void Scroll(Single delta)
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject) && this.scrollWheelFactor != 0f)
        {
            this.DisableSpring();
            this.mShouldMove |= this.shouldMove;
            if (Mathf.Sign(this.mScroll) != Mathf.Sign(delta))
            {
                this.mScroll = 0f;
            }
            this.mScroll += delta * this.scrollWheelFactor;
        }
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        Single deltaTime = RealTime.deltaTime;
        if (this.showScrollBars != UIScrollView.ShowCondition.Always && (this.verticalScrollBar || this.horizontalScrollBar))
        {
            Boolean flag = false;
            Boolean flag2 = false;
            if (this.showScrollBars != UIScrollView.ShowCondition.WhenDragging || this.mDragID != -10 || this.mMomentum.magnitude > 0.01f)
            {
                flag = this.shouldMoveVertically;
                flag2 = this.shouldMoveHorizontally;
            }
            if (this.verticalScrollBar)
            {
                Single num = this.verticalScrollBar.alpha;
                num += ((!flag) ? (-deltaTime * 3f) : (deltaTime * 6f));
                num = Mathf.Clamp01(num);
                if (this.verticalScrollBar.alpha != num)
                {
                    this.verticalScrollBar.alpha = num;
                }
            }
            if (this.horizontalScrollBar)
            {
                Single num2 = this.horizontalScrollBar.alpha;
                num2 += ((!flag2) ? (-deltaTime * 3f) : (deltaTime * 6f));
                num2 = Mathf.Clamp01(num2);
                if (this.horizontalScrollBar.alpha != num2)
                {
                    this.horizontalScrollBar.alpha = num2;
                }
            }
        }
        if (!this.mShouldMove)
        {
            return;
        }
        if (!this.mPressed)
        {
            if (this.mMomentum.magnitude > 0.0001f || Mathf.Abs(this.mScroll) > 0.0001f)
            {
                if (this.movement == UIScrollView.Movement.Horizontal)
                {
                    this.mMomentum -= this.mTrans.TransformDirection(new Vector3(this.mScroll * 0.05f, 0f, 0f));
                }
                else if (this.movement == UIScrollView.Movement.Vertical)
                {
                    this.mMomentum -= this.mTrans.TransformDirection(new Vector3(0f, this.mScroll * 0.05f, 0f));
                }
                else if (this.movement == UIScrollView.Movement.Unrestricted)
                {
                    this.mMomentum -= this.mTrans.TransformDirection(new Vector3(this.mScroll * 0.05f, this.mScroll * 0.05f, 0f));
                }
                else
                {
                    this.mMomentum -= this.mTrans.TransformDirection(new Vector3(this.mScroll * this.customMovement.x * 0.05f, this.mScroll * this.customMovement.y * 0.05f, 0f));
                }
                this.mScroll = NGUIMath.SpringLerp(this.mScroll, 0f, 20f, deltaTime);
                Vector3 absolute = NGUIMath.SpringDampen(ref this.mMomentum, this.dampenStrength, deltaTime);
                this.MoveAbsolute(absolute);
                if (this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None)
                {
                    if (NGUITools.GetActive(this.centerOnChild))
                    {
                        if (this.centerOnChild.nextPageThreshold != 0f)
                        {
                            this.mMomentum = Vector3.zero;
                            this.mScroll = 0f;
                        }
                        else
                        {
                            this.centerOnChild.Recenter();
                        }
                    }
                    else
                    {
                        this.RestrictWithinBounds(false, this.canMoveHorizontally, this.canMoveVertically);
                    }
                }
                if (this.onMomentumMove != null)
                {
                    this.onMomentumMove();
                }
            }
            else
            {
                this.mScroll = 0f;
                this.mMomentum = Vector3.zero;
                SpringPanel component = base.GetComponent<SpringPanel>();
                if (component != (UnityEngine.Object)null && component.enabled)
                {
                    return;
                }
                this.mShouldMove = false;
                if (this.onStoppedMoving != null)
                {
                    this.onStoppedMoving();
                }
            }
        }
        else
        {
            this.mScroll = 0f;
            NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
        }
    }

    public void OnPan(Vector2 delta)
    {
        if (this.horizontalScrollBar != (UnityEngine.Object)null)
        {
            this.horizontalScrollBar.OnPan(delta);
        }
        if (this.verticalScrollBar != (UnityEngine.Object)null)
        {
            this.verticalScrollBar.OnPan(delta);
        }
        if (this.horizontalScrollBar == (UnityEngine.Object)null && this.verticalScrollBar == (UnityEngine.Object)null)
        {
            if (this.scale.x != 0f)
            {
                this.Scroll(delta.x);
            }
            else if (this.scale.y != 0f)
            {
                this.Scroll(delta.y);
            }
        }
    }

    public static BetterList<UIScrollView> list = new BetterList<UIScrollView>();

    public UIScrollView.Movement movement;

    public UIScrollView.DragEffect dragEffect = UIScrollView.DragEffect.MomentumAndSpring;

    public Boolean restrictWithinPanel = true;

    public Boolean disableDragIfFits;

    public Boolean smoothDragStart = true;

    public Boolean iOSDragEmulation = true;

    public Single scrollWheelFactor = 0.25f;

    public Single momentumAmount = 35f;

    public Single dampenStrength = 9f;

    public UIProgressBar horizontalScrollBar;

    public UIProgressBar verticalScrollBar;

    public UIScrollView.ShowCondition showScrollBars = UIScrollView.ShowCondition.OnlyIfNeeded;

    public Vector2 customMovement = new Vector2(1f, 0f);

    public UIWidget.Pivot contentPivot;

    public UIScrollView.OnDragNotification onDragStarted;

    public UIScrollView.OnDragNotification onDragFinished;

    public UIScrollView.OnDragNotification onMomentumMove;

    public UIScrollView.OnDragNotification onStoppedMoving;

    [SerializeField]
    [HideInInspector]
    private Vector3 scale = new Vector3(1f, 0f, 0f);

    [SerializeField]
    [HideInInspector]
    private Vector2 relativePositionOnReset = Vector2.zero;

    protected Transform mTrans;

    protected UIPanel mPanel;

    protected Plane mPlane;

    protected Vector3 mLastPos;

    protected Boolean mPressed;

    protected Vector3 mMomentum = Vector3.zero;

    protected Single mScroll;

    protected Bounds mBounds;

    protected Boolean mCalculatedBounds;

    protected Boolean mShouldMove;

    protected Boolean mIgnoreCallbacks;

    protected Int32 mDragID = -10;

    protected Vector2 mDragStartOffset = Vector2.zero;

    protected Boolean mDragStarted;

    [NonSerialized]
    private Boolean mStarted;

    [HideInInspector]
    public UICenterOnChild centerOnChild;

    public enum Movement
    {
        Horizontal,
        Vertical,
        Unrestricted,
        Custom
    }

    public enum DragEffect
    {
        None,
        Momentum,
        MomentumAndSpring
    }

    public enum ShowCondition
    {
        Always,
        OnlyIfNeeded,
        WhenDragging
    }

    public delegate void OnDragNotification();
}
