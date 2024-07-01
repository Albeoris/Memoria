using System;
using System.Diagnostics;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Widget")]
public class UIWidget : UIRect
{
    public UIDrawCall.OnRenderCallback onRender
    {
        get
        {
            return this.mOnRender;
        }
        set
        {
            if (this.mOnRender != value)
            {
                if (this.drawCall != (UnityEngine.Object)null && this.drawCall.onRender != null && this.mOnRender != null)
                {
                    UIDrawCall uidrawCall = this.drawCall;
                    uidrawCall.onRender = (UIDrawCall.OnRenderCallback)Delegate.Remove(uidrawCall.onRender, this.mOnRender);
                }
                this.mOnRender = value;
                if (this.drawCall != (UnityEngine.Object)null)
                {
                    UIDrawCall uidrawCall2 = this.drawCall;
                    uidrawCall2.onRender = (UIDrawCall.OnRenderCallback)Delegate.Combine(uidrawCall2.onRender, value);
                }
            }
        }
    }

    public Vector4 drawRegion
    {
        get
        {
            return this.mDrawRegion;
        }
        set
        {
            if (this.mDrawRegion != value)
            {
                this.mDrawRegion = value;
                if (this.autoResizeBoxCollider)
                {
                    this.ResizeCollider();
                }
                this.MarkAsChanged();
            }
        }
    }

    public Vector2 pivotOffset
    {
        get
        {
            return NGUIMath.GetPivotOffset(this.pivot);
        }
    }

    public Int32 width
    {
        get
        {
            return this.mWidth;
        }
        set
        {
            Int32 minWidth = this.minWidth;
            if (value < minWidth)
            {
                value = minWidth;
            }
            if (this.mWidth != value && this.keepAspectRatio != UIWidget.AspectRatioSource.BasedOnHeight)
            {
                if (this.isAnchoredHorizontally)
                {
                    if (this.leftAnchor.target != (UnityEngine.Object)null && this.rightAnchor.target != (UnityEngine.Object)null)
                    {
                        if (this.mPivot == UIWidget.Pivot.BottomLeft || this.mPivot == UIWidget.Pivot.Left || this.mPivot == UIWidget.Pivot.TopLeft)
                        {
                            NGUIMath.AdjustWidget(this, 0f, 0f, (Single)(value - this.mWidth), 0f);
                        }
                        else if (this.mPivot == UIWidget.Pivot.BottomRight || this.mPivot == UIWidget.Pivot.Right || this.mPivot == UIWidget.Pivot.TopRight)
                        {
                            NGUIMath.AdjustWidget(this, (Single)(this.mWidth - value), 0f, 0f, 0f);
                        }
                        else
                        {
                            Int32 num = value - this.mWidth;
                            num -= (num & 1);
                            if (num != 0)
                            {
                                NGUIMath.AdjustWidget(this, (Single)(-(Single)num) * 0.5f, 0f, (Single)num * 0.5f, 0f);
                            }
                        }
                    }
                    else if (this.leftAnchor.target != (UnityEngine.Object)null)
                    {
                        NGUIMath.AdjustWidget(this, 0f, 0f, (Single)(value - this.mWidth), 0f);
                    }
                    else
                    {
                        NGUIMath.AdjustWidget(this, (Single)(this.mWidth - value), 0f, 0f, 0f);
                    }
                }
                else
                {
                    this.SetDimensions(value, this.mHeight);
                }
            }
        }
    }

    public Int32 height
    {
        get
        {
            return this.mHeight;
        }
        set
        {
            Int32 minHeight = this.minHeight;
            if (value < minHeight)
            {
                value = minHeight;
            }
            if (this.mHeight != value && this.keepAspectRatio != UIWidget.AspectRatioSource.BasedOnWidth)
            {
                if (this.isAnchoredVertically)
                {
                    if (this.bottomAnchor.target != (UnityEngine.Object)null && this.topAnchor.target != (UnityEngine.Object)null)
                    {
                        if (this.mPivot == UIWidget.Pivot.BottomLeft || this.mPivot == UIWidget.Pivot.Bottom || this.mPivot == UIWidget.Pivot.BottomRight)
                        {
                            NGUIMath.AdjustWidget(this, 0f, 0f, 0f, (Single)(value - this.mHeight));
                        }
                        else if (this.mPivot == UIWidget.Pivot.TopLeft || this.mPivot == UIWidget.Pivot.Top || this.mPivot == UIWidget.Pivot.TopRight)
                        {
                            NGUIMath.AdjustWidget(this, 0f, (Single)(this.mHeight - value), 0f, 0f);
                        }
                        else
                        {
                            Int32 num = value - this.mHeight;
                            num -= (num & 1);
                            if (num != 0)
                            {
                                NGUIMath.AdjustWidget(this, 0f, (Single)(-(Single)num) * 0.5f, 0f, (Single)num * 0.5f);
                            }
                        }
                    }
                    else if (this.bottomAnchor.target != (UnityEngine.Object)null)
                    {
                        NGUIMath.AdjustWidget(this, 0f, 0f, 0f, (Single)(value - this.mHeight));
                    }
                    else
                    {
                        NGUIMath.AdjustWidget(this, 0f, (Single)(this.mHeight - value), 0f, 0f);
                    }
                }
                else
                {
                    this.SetDimensions(this.mWidth, value);
                }
            }
        }
    }

    public Color color
    {
        get
        {
            return this.mColor;
        }
        set
        {
            if (this.mColor != value)
            {
                Boolean includeChildren = this.mColor.a != value.a;
                this.mColor = value;
                this.Invalidate(includeChildren);
            }
        }
    }

    public override Single alpha
    {
        get
        {
            return this.mColor.a;
        }
        set
        {
            if (this.mColor.a != value)
            {
                this.mColor.a = value;
                this.Invalidate(true);
            }
        }
    }

    public Boolean isVisible
    {
        get
        {
            return this.mIsVisibleByPanel && this.mIsVisibleByAlpha && this.mIsInFront && this.finalAlpha > 0.001f && NGUITools.GetActive(this);
        }
    }

    public Boolean hasVertices
    {
        get
        {
            return this.geometry != null && this.geometry.hasVertices;
        }
    }

    public UIWidget.Pivot rawPivot
    {
        get
        {
            return this.mPivot;
        }
        set
        {
            if (this.mPivot != value)
            {
                this.mPivot = value;
                if (this.autoResizeBoxCollider)
                {
                    this.ResizeCollider();
                }
                this.MarkAsChanged();
            }
        }
    }

    public UIWidget.Pivot pivot
    {
        get
        {
            return this.mPivot;
        }
        set
        {
            if (this.mPivot != value)
            {
                Vector3 vector = this.worldCorners[0];
                this.mPivot = value;
                this.mChanged = true;
                Vector3 vector2 = this.worldCorners[0];
                Transform cachedTransform = base.cachedTransform;
                Vector3 vector3 = cachedTransform.position;
                Single z = cachedTransform.localPosition.z;
                vector3.x += vector.x - vector2.x;
                vector3.y += vector.y - vector2.y;
                base.cachedTransform.position = vector3;
                vector3 = base.cachedTransform.localPosition;
                vector3.x = Mathf.Round(vector3.x);
                vector3.y = Mathf.Round(vector3.y);
                vector3.z = z;
                base.cachedTransform.localPosition = vector3;
            }
        }
    }

    public Int32 depth
    {
        get
        {
            return this.mDepth;
        }
        set
        {
            if (this.mDepth != value)
            {
                if (this.panel != (UnityEngine.Object)null)
                {
                    this.panel.RemoveWidget(this);
                }
                this.mDepth = value;
                if (this.panel != (UnityEngine.Object)null)
                {
                    this.panel.AddWidget(this);
                    if (!Application.isPlaying)
                    {
                        this.panel.SortWidgets();
                        this.panel.RebuildAllDrawCalls();
                    }
                }
            }
        }
    }

    public Int32 raycastDepth
    {
        get
        {
            if (this.panel == (UnityEngine.Object)null)
            {
                this.CreatePanel();
            }
            return (Int32)((!(this.panel != (UnityEngine.Object)null)) ? this.mDepth : (this.mDepth + this.panel.depth * 1000));
        }
    }

    public override Vector3[] localCorners
    {
        get
        {
            Vector2 pivotOffset = this.pivotOffset;
            Single num = -pivotOffset.x * (Single)this.mWidth;
            Single num2 = -pivotOffset.y * (Single)this.mHeight;
            Single x = num + (Single)this.mWidth;
            Single y = num2 + (Single)this.mHeight;
            this.mCorners[0] = new Vector3(num, num2);
            this.mCorners[1] = new Vector3(num, y);
            this.mCorners[2] = new Vector3(x, y);
            this.mCorners[3] = new Vector3(x, num2);
            return this.mCorners;
        }
    }

    public virtual Vector2 localSize
    {
        get
        {
            Vector3[] localCorners = this.localCorners;
            return localCorners[2] - localCorners[0];
        }
    }

    public Vector3 localCenter
    {
        get
        {
            Vector3[] localCorners = this.localCorners;
            return Vector3.Lerp(localCorners[0], localCorners[2], 0.5f);
        }
    }

    public override Vector3[] worldCorners
    {
        get
        {
            Vector2 pivotOffset = this.pivotOffset;
            Single num = -pivotOffset.x * (Single)this.mWidth;
            Single num2 = -pivotOffset.y * (Single)this.mHeight;
            Single x = num + (Single)this.mWidth;
            Single y = num2 + (Single)this.mHeight;
            Transform cachedTransform = base.cachedTransform;
            this.mCorners[0] = cachedTransform.TransformPoint(num, num2, 0f);
            this.mCorners[1] = cachedTransform.TransformPoint(num, y, 0f);
            this.mCorners[2] = cachedTransform.TransformPoint(x, y, 0f);
            this.mCorners[3] = cachedTransform.TransformPoint(x, num2, 0f);
            return this.mCorners;
        }
    }

    public Vector3 worldCenter
    {
        get
        {
            return base.cachedTransform.TransformPoint(this.localCenter);
        }
    }

    public virtual Vector4 drawingDimensions
    {
        get
        {
            Vector2 pivotOffset = this.pivotOffset;
            Single num = -pivotOffset.x * (Single)this.mWidth;
            Single num2 = -pivotOffset.y * (Single)this.mHeight;
            Single num3 = num + (Single)this.mWidth;
            Single num4 = num2 + (Single)this.mHeight;
            return new Vector4((this.mDrawRegion.x != 0f) ? Mathf.Lerp(num, num3, this.mDrawRegion.x) : num, (this.mDrawRegion.y != 0f) ? Mathf.Lerp(num2, num4, this.mDrawRegion.y) : num2, (this.mDrawRegion.z != 1f) ? Mathf.Lerp(num, num3, this.mDrawRegion.z) : num3, (this.mDrawRegion.w != 1f) ? Mathf.Lerp(num2, num4, this.mDrawRegion.w) : num4);
        }
    }

    public virtual Material material
    {
        get
        {
            return (Material)null;
        }
        set
        {
            throw new NotImplementedException(base.GetType() + " has no material setter");
        }
    }

    public virtual Texture mainTexture
    {
        get
        {
            Material material = this.material;
            return (!(material != (UnityEngine.Object)null)) ? null : material.mainTexture;
        }
        set
        {
            throw new NotImplementedException(base.GetType() + " has no mainTexture setter");
        }
    }

    public virtual Shader shader
    {
        get
        {
            Material material = this.material;
            return (!(material != (UnityEngine.Object)null)) ? null : material.shader;
        }
        set
        {
            throw new NotImplementedException(base.GetType() + " has no shader setter");
        }
    }

    [Obsolete("There is no relative scale anymore. Widgets now have width and height instead")]
    public Vector2 relativeSize
    {
        get
        {
            return Vector2.one;
        }
    }

    public Boolean hasBoxCollider
    {
        get
        {
            BoxCollider x = base.GetComponent<Collider>() as BoxCollider;
            return x != (UnityEngine.Object)null || base.GetComponent<BoxCollider2D>() != (UnityEngine.Object)null;
        }
    }

    public void SetDimensions(Int32 w, Int32 h)
    {
        if (this.mWidth != w || this.mHeight != h)
        {
            this.mWidth = w;
            this.mHeight = h;
            if (this.keepAspectRatio == UIWidget.AspectRatioSource.BasedOnWidth)
            {
                this.mHeight = Mathf.RoundToInt((Single)this.mWidth / this.aspectRatio);
            }
            else if (this.keepAspectRatio == UIWidget.AspectRatioSource.BasedOnHeight)
            {
                this.mWidth = Mathf.RoundToInt((Single)this.mHeight * this.aspectRatio);
            }
            else if (this.keepAspectRatio == UIWidget.AspectRatioSource.Free)
            {
                this.aspectRatio = (Single)this.mWidth / (Single)this.mHeight;
            }
            this.mMoved = true;
            if (this.autoResizeBoxCollider)
            {
                this.ResizeCollider();
            }
            this.MarkAsChanged();
        }
    }

    public override Vector3[] GetSides(Transform relativeTo)
    {
        Vector2 pivotOffset = this.pivotOffset;
        Single num = -pivotOffset.x * (Single)this.mWidth;
        Single num2 = -pivotOffset.y * (Single)this.mHeight;
        Single num3 = num + (Single)this.mWidth;
        Single num4 = num2 + (Single)this.mHeight;
        Single x = (num + num3) * 0.5f;
        Single y = (num2 + num4) * 0.5f;
        Transform cachedTransform = base.cachedTransform;
        this.mCorners[0] = cachedTransform.TransformPoint(num, y, 0f);
        this.mCorners[1] = cachedTransform.TransformPoint(x, num4, 0f);
        this.mCorners[2] = cachedTransform.TransformPoint(num3, y, 0f);
        this.mCorners[3] = cachedTransform.TransformPoint(x, num2, 0f);
        if (relativeTo != (UnityEngine.Object)null)
        {
            for (Int32 i = 0; i < 4; i++)
            {
                this.mCorners[i] = relativeTo.InverseTransformPoint(this.mCorners[i]);
            }
        }
        return this.mCorners;
    }

    public override Single CalculateFinalAlpha(Int32 frameID)
    {
        if (this.mAlphaFrameID != frameID)
        {
            this.mAlphaFrameID = frameID;
            this.UpdateFinalAlpha(frameID);
        }
        return this.finalAlpha;
    }

    protected void UpdateFinalAlpha(Int32 frameID)
    {
        if (!this.mIsVisibleByAlpha || !this.mIsInFront)
        {
            this.finalAlpha = 0f;
        }
        else
        {
            UIRect parent = base.parent;
            this.finalAlpha = ((!(parent != (UnityEngine.Object)null)) ? this.mColor.a : (parent.CalculateFinalAlpha(frameID) * this.mColor.a));
        }
    }

    public override void Invalidate(Boolean includeChildren)
    {
        this.mChanged = true;
        this.mAlphaFrameID = -1;
        if (this.panel != (UnityEngine.Object)null)
        {
            Boolean visibleByPanel = (!this.hideIfOffScreen && !this.panel.hasCumulativeClipping) || this.panel.IsVisible(this);
            this.UpdateVisibility(this.CalculateCumulativeAlpha(Time.frameCount) > 0.001f, visibleByPanel);
            this.UpdateFinalAlpha(Time.frameCount);
            if (includeChildren)
            {
                base.Invalidate(true);
            }
        }
    }

    public Single CalculateCumulativeAlpha(Int32 frameID)
    {
        UIRect parent = base.parent;
        return (!(parent != (UnityEngine.Object)null)) ? this.mColor.a : (parent.CalculateFinalAlpha(frameID) * this.mColor.a);
    }

    public void SetRawRect(Single x, Single y, Single width, Single height)
    {
        this.transform.SetXY(x, y);
        this.width = (Int32)width;
        this.height = (Int32)height;
    }

    public override void SetRect(Single x, Single y, Single width, Single height)
    {
        Vector2 pivotOffset = this.pivotOffset;
        Single num = Mathf.Lerp(x, x + width, pivotOffset.x);
        Single num2 = Mathf.Lerp(y, y + height, pivotOffset.y);
        Int32 num3 = Mathf.FloorToInt(width + 0.5f);
        Int32 num4 = Mathf.FloorToInt(height + 0.5f);
        if (pivotOffset.x == 0.5f)
        {
            num3 = num3 >> 1 << 1;
        }
        if (pivotOffset.y == 0.5f)
        {
            num4 = num4 >> 1 << 1;
        }
        Transform transform = base.cachedTransform;
        Vector3 localPosition = transform.localPosition;
        localPosition.x = Mathf.Floor(num + 0.5f);
        localPosition.y = Mathf.Floor(num2 + 0.5f);
        if (num3 < this.minWidth)
        {
            num3 = this.minWidth;
        }
        if (num4 < this.minHeight)
        {
            num4 = this.minHeight;
        }
        transform.localPosition = localPosition;
        this.width = num3;
        this.height = num4;
        if (base.isAnchored)
        {
            transform = transform.parent;
            if (this.leftAnchor.target)
            {
                this.leftAnchor.SetHorizontal(transform, x);
            }
            if (this.rightAnchor.target)
            {
                this.rightAnchor.SetHorizontal(transform, x + width);
            }
            if (this.bottomAnchor.target)
            {
                this.bottomAnchor.SetVertical(transform, y);
            }
            if (this.topAnchor.target)
            {
                this.topAnchor.SetVertical(transform, y + height);
            }
        }
    }

    public void ResizeCollider()
    {
        if (NGUITools.GetActive(this))
        {
            NGUITools.UpdateWidgetCollider(base.gameObject);
        }
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Int32 FullCompareFunc(UIWidget left, UIWidget right)
    {
        Int32 num = UIPanel.CompareFunc(left.panel, right.panel);
        return (Int32)((num != 0) ? num : UIWidget.PanelCompareFunc(left, right));
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static Int32 PanelCompareFunc(UIWidget left, UIWidget right)
    {
        if (left.mDepth < right.mDepth)
        {
            return -1;
        }
        if (left.mDepth > right.mDepth)
        {
            return 1;
        }
        Material material = left.material;
        Material material2 = right.material;
        if (material == material2)
        {
            return 0;
        }
        if (material != (UnityEngine.Object)null)
        {
            return -1;
        }
        if (material2 != (UnityEngine.Object)null)
        {
            return 1;
        }
        return (Int32)((material.GetInstanceID() >= material2.GetInstanceID()) ? 1 : -1);
    }

    public Bounds CalculateBounds()
    {
        return this.CalculateBounds((Transform)null);
    }

    public Bounds CalculateBounds(Transform relativeParent)
    {
        if (relativeParent == (UnityEngine.Object)null)
        {
            Vector3[] localCorners = this.localCorners;
            Bounds result = new Bounds(localCorners[0], Vector3.zero);
            for (Int32 i = 1; i < 4; i++)
            {
                result.Encapsulate(localCorners[i]);
            }
            return result;
        }
        Matrix4x4 worldToLocalMatrix = relativeParent.worldToLocalMatrix;
        Vector3[] worldCorners = this.worldCorners;
        Bounds result2 = new Bounds(worldToLocalMatrix.MultiplyPoint3x4(worldCorners[0]), Vector3.zero);
        for (Int32 j = 1; j < 4; j++)
        {
            result2.Encapsulate(worldToLocalMatrix.MultiplyPoint3x4(worldCorners[j]));
        }
        return result2;
    }

    public void SetDirty()
    {
        if (this.drawCall != (UnityEngine.Object)null)
        {
            this.drawCall.isDirty = true;
        }
        else if (this.isVisible && this.hasVertices)
        {
            this.CreatePanel();
        }
    }

    public void RemoveFromPanel()
    {
        if (this.panel != (UnityEngine.Object)null)
        {
            this.panel.RemoveWidget(this);
            this.panel = (UIPanel)null;
        }
        this.drawCall = (UIDrawCall)null;
    }

    public virtual void MarkAsChanged()
    {
        if (NGUITools.GetActive(this))
        {
            this.mChanged = true;
            if (this.panel != (UnityEngine.Object)null && base.enabled && NGUITools.GetActive(base.gameObject) && !this.mPlayMode)
            {
                this.SetDirty();
                this.CheckLayer();
            }
        }
    }

    public UIPanel CreatePanel()
    {
        if (this.mStarted && this.panel == (UnityEngine.Object)null && base.enabled && NGUITools.GetActive(base.gameObject))
        {
            this.panel = UIPanel.Find(base.cachedTransform, true, base.cachedGameObject.layer);
            if (this.panel != (UnityEngine.Object)null)
            {
                this.mParentFound = false;
                this.panel.AddWidget(this);
                this.CheckLayer();
                this.Invalidate(true);
            }
        }
        return this.panel;
    }

    public void CheckLayer()
    {
        if (this.panel != (UnityEngine.Object)null && this.panel.gameObject.layer != base.gameObject.layer)
        {
            global::Debug.LogWarning("You can't place widgets on a layer different than the UIPanel that manages them.\nIf you want to move widgets to a different layer, parent them to a new panel instead.", this);
            base.gameObject.layer = this.panel.gameObject.layer;
        }
    }

    public override void ParentHasChanged()
    {
        base.ParentHasChanged();
        if (this.panel != (UnityEngine.Object)null)
        {
            UIPanel y = UIPanel.Find(base.cachedTransform, true, base.cachedGameObject.layer);
            if (this.panel != y)
            {
                this.RemoveFromPanel();
                this.CreatePanel();
            }
        }
    }

    protected virtual void Awake()
    {
        this.mGo = base.gameObject;
        this.mPlayMode = Application.isPlaying;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.RemoveFromPanel();
        this.mMoved = true;
        if (this.mWidth == 100 && this.mHeight == 100 && base.cachedTransform.localScale.magnitude > 8f)
        {
            this.UpgradeFrom265();
            base.cachedTransform.localScale = Vector3.one;
        }
        base.Update();
    }

    protected virtual void UpgradeFrom265()
    {
        Vector3 localScale = base.cachedTransform.localScale;
        this.mWidth = Mathf.Abs(Mathf.RoundToInt(localScale.x));
        this.mHeight = Mathf.Abs(Mathf.RoundToInt(localScale.y));
        NGUITools.UpdateWidgetCollider(base.gameObject, true);
    }

    protected override void OnStart()
    {
        this.CreatePanel();
    }

    protected override void OnAnchor()
    {
        Transform cachedTransform = base.cachedTransform;
        Transform parent = cachedTransform.parent;
        Vector3 localPosition = cachedTransform.localPosition;
        Vector2 pivotOffset = this.pivotOffset;
        Single num;
        Single num2;
        Single num3;
        Single num4;
        if (this.leftAnchor.target == this.bottomAnchor.target && this.leftAnchor.target == this.rightAnchor.target && this.leftAnchor.target == this.topAnchor.target)
        {
            Vector3[] sides = this.leftAnchor.GetSides(parent);
            if (sides != null)
            {
                num = NGUIMath.Lerp(sides[0].x, sides[2].x, this.leftAnchor.relative) + (Single)this.leftAnchor.absolute;
                num2 = NGUIMath.Lerp(sides[0].x, sides[2].x, this.rightAnchor.relative) + (Single)this.rightAnchor.absolute;
                num3 = NGUIMath.Lerp(sides[3].y, sides[1].y, this.bottomAnchor.relative) + (Single)this.bottomAnchor.absolute;
                num4 = NGUIMath.Lerp(sides[3].y, sides[1].y, this.topAnchor.relative) + (Single)this.topAnchor.absolute;
                this.mIsInFront = true;
            }
            else
            {
                Vector3 localPos = base.GetLocalPos(this.leftAnchor, parent);
                num = localPos.x + (Single)this.leftAnchor.absolute;
                num3 = localPos.y + (Single)this.bottomAnchor.absolute;
                num2 = localPos.x + (Single)this.rightAnchor.absolute;
                num4 = localPos.y + (Single)this.topAnchor.absolute;
                this.mIsInFront = (!this.hideIfOffScreen || localPos.z >= 0f);
            }
        }
        else
        {
            this.mIsInFront = true;
            if (this.leftAnchor.target)
            {
                Vector3[] sides2 = this.leftAnchor.GetSides(parent);
                if (sides2 != null)
                {
                    num = NGUIMath.Lerp(sides2[0].x, sides2[2].x, this.leftAnchor.relative) + (Single)this.leftAnchor.absolute;
                }
                else
                {
                    num = base.GetLocalPos(this.leftAnchor, parent).x + (Single)this.leftAnchor.absolute;
                }
            }
            else
            {
                num = localPosition.x - pivotOffset.x * (Single)this.mWidth;
            }
            if (this.rightAnchor.target)
            {
                Vector3[] sides3 = this.rightAnchor.GetSides(parent);
                if (sides3 != null)
                {
                    num2 = NGUIMath.Lerp(sides3[0].x, sides3[2].x, this.rightAnchor.relative) + (Single)this.rightAnchor.absolute;
                }
                else
                {
                    num2 = base.GetLocalPos(this.rightAnchor, parent).x + (Single)this.rightAnchor.absolute;
                }
            }
            else
            {
                num2 = localPosition.x - pivotOffset.x * (Single)this.mWidth + (Single)this.mWidth;
            }
            if (this.bottomAnchor.target)
            {
                Vector3[] sides4 = this.bottomAnchor.GetSides(parent);
                if (sides4 != null)
                {
                    num3 = NGUIMath.Lerp(sides4[3].y, sides4[1].y, this.bottomAnchor.relative) + (Single)this.bottomAnchor.absolute;
                }
                else
                {
                    num3 = base.GetLocalPos(this.bottomAnchor, parent).y + (Single)this.bottomAnchor.absolute;
                }
            }
            else
            {
                num3 = localPosition.y - pivotOffset.y * (Single)this.mHeight;
            }
            if (this.topAnchor.target)
            {
                Vector3[] sides5 = this.topAnchor.GetSides(parent);
                if (sides5 != null)
                {
                    num4 = NGUIMath.Lerp(sides5[3].y, sides5[1].y, this.topAnchor.relative) + (Single)this.topAnchor.absolute;
                }
                else
                {
                    num4 = base.GetLocalPos(this.topAnchor, parent).y + (Single)this.topAnchor.absolute;
                }
            }
            else
            {
                num4 = localPosition.y - pivotOffset.y * (Single)this.mHeight + (Single)this.mHeight;
            }
        }
        Vector3 vector = new Vector3(Mathf.Lerp(num, num2, pivotOffset.x), Mathf.Lerp(num3, num4, pivotOffset.y), localPosition.z);
        vector.x = Mathf.Round(vector.x);
        vector.y = Mathf.Round(vector.y);
        Int32 num5 = Mathf.FloorToInt(num2 - num + 0.5f);
        Int32 num6 = Mathf.FloorToInt(num4 - num3 + 0.5f);
        if (this.keepAspectRatio != UIWidget.AspectRatioSource.Free && this.aspectRatio != 0f)
        {
            if (this.keepAspectRatio == UIWidget.AspectRatioSource.BasedOnHeight)
            {
                num5 = Mathf.RoundToInt((Single)num6 * this.aspectRatio);
            }
            else
            {
                num6 = Mathf.RoundToInt((Single)num5 / this.aspectRatio);
            }
        }
        if (num5 < this.minWidth)
        {
            num5 = this.minWidth;
        }
        if (num6 < this.minHeight)
        {
            num6 = this.minHeight;
        }
        if (Vector3.SqrMagnitude(localPosition - vector) > 0.001f)
        {
            base.cachedTransform.localPosition = vector;
            if (this.mIsInFront)
            {
                this.mChanged = true;
            }
        }
        if (this.mWidth != num5 || this.mHeight != num6)
        {
            this.mWidth = num5;
            this.mHeight = num6;
            if (this.mIsInFront)
            {
                this.mChanged = true;
            }
            if (this.autoResizeBoxCollider)
            {
                this.ResizeCollider();
            }
        }
    }

    protected override void OnUpdate()
    {
        if (this.panel == (UnityEngine.Object)null)
        {
            this.CreatePanel();
        }
    }

    private void OnApplicationPause(Boolean paused)
    {
        if (!paused)
        {
            this.MarkAsChanged();
        }
    }

    protected override void OnDisable()
    {
        this.RemoveFromPanel();
        base.OnDisable();
    }

    private void OnDestroy()
    {
        this.RemoveFromPanel();
    }

    public Boolean UpdateVisibility(Boolean visibleByAlpha, Boolean visibleByPanel)
    {
        if (this.mIsVisibleByAlpha != visibleByAlpha || this.mIsVisibleByPanel != visibleByPanel)
        {
            this.mChanged = true;
            this.mIsVisibleByAlpha = visibleByAlpha;
            this.mIsVisibleByPanel = visibleByPanel;
            return true;
        }
        return false;
    }

    public Boolean UpdateTransform(Int32 frame)
    {
        Transform cachedTransform = base.cachedTransform;
        this.mPlayMode = Application.isPlaying;
        if (this.mMoved)
        {
            this.mMoved = true;
            this.mMatrixFrame = -1;
            cachedTransform.hasChanged = false;
            Vector2 pivotOffset = this.pivotOffset;
            Single num = -pivotOffset.x * (Single)this.mWidth;
            Single num2 = -pivotOffset.y * (Single)this.mHeight;
            Single x = num + (Single)this.mWidth;
            Single y = num2 + (Single)this.mHeight;
            this.mOldV0 = this.panel.worldToLocal.MultiplyPoint3x4(cachedTransform.TransformPoint(num, num2, 0f));
            this.mOldV1 = this.panel.worldToLocal.MultiplyPoint3x4(cachedTransform.TransformPoint(x, y, 0f));
        }
        else if (!this.panel.widgetsAreStatic && cachedTransform.hasChanged)
        {
            this.mMoved = true;
            this.mMatrixFrame = -1;
            cachedTransform.hasChanged = false;
            Vector2 pivotOffset2 = this.pivotOffset;
            Single num3 = -pivotOffset2.x * (Single)this.mWidth;
            Single num4 = -pivotOffset2.y * (Single)this.mHeight;
            Single x2 = num3 + (Single)this.mWidth;
            Single y2 = num4 + (Single)this.mHeight;
            Vector3 b = this.panel.worldToLocal.MultiplyPoint3x4(cachedTransform.TransformPoint(num3, num4, 0f));
            Vector3 b2 = this.panel.worldToLocal.MultiplyPoint3x4(cachedTransform.TransformPoint(x2, y2, 0f));
            if (Vector3.SqrMagnitude(this.mOldV0 - b) > 1E-06f || Vector3.SqrMagnitude(this.mOldV1 - b2) > 1E-06f)
            {
                this.mMoved = true;
                this.mOldV0 = b;
                this.mOldV1 = b2;
            }
        }
        if (this.mMoved && this.onChange != null)
        {
            this.onChange();
        }
        return this.mMoved || this.mChanged;
    }

    public Boolean UpdateGeometry(Int32 frame)
    {
        Single num = this.CalculateFinalAlpha(frame);
        if (this.mIsVisibleByAlpha && this.mLastAlpha != num)
        {
            this.mChanged = true;
        }
        this.mLastAlpha = num;
        if (this.mChanged)
        {
            this.mChanged = false;
            if (this.mIsVisibleByAlpha && num > 0.001f && this.shader != (UnityEngine.Object)null)
            {
                Boolean hasVertices = this.geometry.hasVertices;
                if (this.fillGeometry)
                {
                    this.geometry.Clear();
                    this.OnFill(this.geometry.verts, this.geometry.uvs, this.geometry.cols);
                }
                if (this.geometry.hasVertices)
                {
                    if (this.mMatrixFrame != frame)
                    {
                        this.mLocalToPanel = this.panel.worldToLocal * base.cachedTransform.localToWorldMatrix;
                        this.mMatrixFrame = frame;
                    }
                    this.geometry.ApplyTransform(this.mLocalToPanel, this.panel.generateNormals);
                    this.mMoved = false;
                    return true;
                }
                return hasVertices;
            }
            else if (this.geometry.hasVertices)
            {
                if (this.fillGeometry)
                {
                    this.geometry.Clear();
                }
                this.mMoved = false;
                return true;
            }
        }
        else if (this.mMoved && this.geometry.hasVertices)
        {
            if (this.mMatrixFrame != frame)
            {
                this.mLocalToPanel = this.panel.worldToLocal * base.cachedTransform.localToWorldMatrix;
                this.mMatrixFrame = frame;
            }
            this.geometry.ApplyTransform(this.mLocalToPanel, this.panel.generateNormals);
            this.mMoved = false;
            return true;
        }
        this.mMoved = false;
        return false;
    }

    public void WriteToBuffers(BetterList<Vector3> v, BetterList<Vector2> u, BetterList<Color32> c, BetterList<Vector3> n, BetterList<Vector4> t)
    {
        this.geometry.WriteToBuffers(v, u, c, n, t);
    }

    public virtual void MakePixelPerfect()
    {
        Vector3 localPosition = base.cachedTransform.localPosition;
        localPosition.z = Mathf.Round(localPosition.z);
        localPosition.x = Mathf.Round(localPosition.x);
        localPosition.y = Mathf.Round(localPosition.y);
        base.cachedTransform.localPosition = localPosition;
        Vector3 localScale = base.cachedTransform.localScale;
        base.cachedTransform.localScale = new Vector3(Mathf.Sign(localScale.x), Mathf.Sign(localScale.y), 1f);
    }

    public virtual Int32 minWidth
    {
        get
        {
            return 2;
        }
    }

    public virtual Int32 minHeight
    {
        get
        {
            return 2;
        }
    }

    public virtual Vector4 border
    {
        get
        {
            return Vector4.zero;
        }
        set
        {
        }
    }

    public virtual void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
    }

    [SerializeField]
    [HideInInspector]
    protected Color mColor = Color.white;

    [HideInInspector]
    [SerializeField]
    protected UIWidget.Pivot mPivot = UIWidget.Pivot.Center;

    [HideInInspector]
    [SerializeField]
    protected Int32 mWidth = 100;

    [SerializeField]
    [HideInInspector]
    protected Int32 mHeight = 100;

    [SerializeField]
    [HideInInspector]
    protected Int32 mDepth;

    public UIWidget.OnDimensionsChanged onChange;

    public UIWidget.OnPostFillCallback onPostFill;

    public UIDrawCall.OnRenderCallback mOnRender;

    public Boolean autoResizeBoxCollider;

    public Boolean hideIfOffScreen;

    public UIWidget.AspectRatioSource keepAspectRatio;

    public Single aspectRatio = 1f;

    public UIWidget.HitCheck hitCheck;

    [NonSerialized]
    public UIPanel panel;

    [NonSerialized]
    public UIGeometry geometry = new UIGeometry();

    [NonSerialized]
    public Boolean fillGeometry = true;

    [NonSerialized]
    protected Boolean mPlayMode = true;

    [NonSerialized]
    protected Vector4 mDrawRegion = new Vector4(0f, 0f, 1f, 1f);

    [NonSerialized]
    private Matrix4x4 mLocalToPanel;

    [NonSerialized]
    private Boolean mIsVisibleByAlpha = true;

    [NonSerialized]
    private Boolean mIsVisibleByPanel = true;

    [NonSerialized]
    private Boolean mIsInFront = true;

    [NonSerialized]
    private Single mLastAlpha;

    [NonSerialized]
    private Boolean mMoved;

    [NonSerialized]
    public UIDrawCall drawCall;

    [NonSerialized]
    protected Vector3[] mCorners = new Vector3[4];

    [NonSerialized]
    private Int32 mAlphaFrameID = -1;

    private Int32 mMatrixFrame = -1;

    private Vector3 mOldV0;

    private Vector3 mOldV1;

    public enum Pivot
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    public enum AspectRatioSource
    {
        Free,
        BasedOnWidth,
        BasedOnHeight
    }

    public delegate void OnDimensionsChanged();

    public delegate void OnPostFillCallback(UIWidget widget, Int32 bufferOffset, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols);

    public delegate Boolean HitCheck(Vector3 worldPos);
}
