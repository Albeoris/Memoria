using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Anchor")]
[ExecuteInEditMode]
public class UIAnchor : MonoBehaviour
{
    private void Awake()
    {
        this.mTrans = base.transform;
        this.mAnim = base.GetComponent<Animation>();
        UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Combine(UICamera.onScreenResize, new UICamera.OnScreenResize(this.ScreenSizeChanged));
    }

    private void OnDestroy()
    {
        UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Remove(UICamera.onScreenResize, new UICamera.OnScreenResize(this.ScreenSizeChanged));
    }

    private void ScreenSizeChanged()
    {
        if (this.mStarted && this.runOnlyOnce)
        {
            this.Update();
        }
    }

    private void Start()
    {
        if (this.container == (UnityEngine.Object)null && this.widgetContainer != (UnityEngine.Object)null)
        {
            this.container = this.widgetContainer.gameObject;
            this.widgetContainer = (UIWidget)null;
        }
        this.mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
        if (this.uiCamera == (UnityEngine.Object)null)
        {
            this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
        }
        this.Update();
        this.mStarted = true;
    }

    private void Update()
    {
        if (this.mAnim != (UnityEngine.Object)null && this.mAnim.enabled && this.mAnim.isPlaying)
        {
            return;
        }
        Boolean flag = false;
        UIWidget uiwidget = (!(this.container == (UnityEngine.Object)null)) ? this.container.GetComponent<UIWidget>() : ((UIWidget)null);
        UIPanel uipanel = (!(this.container == (UnityEngine.Object)null) || !(uiwidget == (UnityEngine.Object)null)) ? this.container.GetComponent<UIPanel>() : ((UIPanel)null);
        if (uiwidget != (UnityEngine.Object)null)
        {
            Bounds bounds = uiwidget.CalculateBounds(this.container.transform.parent);
            this.mRect.x = bounds.min.x;
            this.mRect.y = bounds.min.y;
            this.mRect.width = bounds.size.x;
            this.mRect.height = bounds.size.y;
        }
        else if (uipanel != (UnityEngine.Object)null)
        {
            if (uipanel.clipping == UIDrawCall.Clipping.None)
            {
                Single num = (!(this.mRoot != (UnityEngine.Object)null)) ? 0.5f : ((Single)this.mRoot.activeHeight / (Single)Screen.height * 0.5f);
                this.mRect.xMin = (Single)(-(Single)Screen.width) * num;
                this.mRect.yMin = (Single)(-(Single)Screen.height) * num;
                this.mRect.xMax = -this.mRect.xMin;
                this.mRect.yMax = -this.mRect.yMin;
            }
            else
            {
                Vector4 finalClipRegion = uipanel.finalClipRegion;
                this.mRect.x = finalClipRegion.x - finalClipRegion.z * 0.5f;
                this.mRect.y = finalClipRegion.y - finalClipRegion.w * 0.5f;
                this.mRect.width = finalClipRegion.z;
                this.mRect.height = finalClipRegion.w;
            }
        }
        else if (this.container != (UnityEngine.Object)null)
        {
            Transform parent = this.container.transform.parent;
            Bounds bounds2 = (!(parent != (UnityEngine.Object)null)) ? NGUIMath.CalculateRelativeWidgetBounds(this.container.transform) : NGUIMath.CalculateRelativeWidgetBounds(parent, this.container.transform);
            this.mRect.x = bounds2.min.x;
            this.mRect.y = bounds2.min.y;
            this.mRect.width = bounds2.size.x;
            this.mRect.height = bounds2.size.y;
        }
        else
        {
            if (!(this.uiCamera != (UnityEngine.Object)null))
            {
                return;
            }
            flag = true;
            this.mRect = this.uiCamera.pixelRect;
        }
        Single x = (this.mRect.xMin + this.mRect.xMax) * 0.5f;
        Single y = (this.mRect.yMin + this.mRect.yMax) * 0.5f;
        Vector3 vector = new Vector3(x, y, 0f);
        if (this.side != UIAnchor.Side.Center)
        {
            if (this.side == UIAnchor.Side.Right || this.side == UIAnchor.Side.TopRight || this.side == UIAnchor.Side.BottomRight)
            {
                vector.x = this.mRect.xMax;
            }
            else if (this.side == UIAnchor.Side.Top || this.side == UIAnchor.Side.Center || this.side == UIAnchor.Side.Bottom)
            {
                vector.x = x;
            }
            else
            {
                vector.x = this.mRect.xMin;
            }
            if (this.side == UIAnchor.Side.Top || this.side == UIAnchor.Side.TopRight || this.side == UIAnchor.Side.TopLeft)
            {
                vector.y = this.mRect.yMax;
            }
            else if (this.side == UIAnchor.Side.Left || this.side == UIAnchor.Side.Center || this.side == UIAnchor.Side.Right)
            {
                vector.y = y;
            }
            else
            {
                vector.y = this.mRect.yMin;
            }
        }
        Single width = this.mRect.width;
        Single height = this.mRect.height;
        vector.x += this.pixelOffset.x + this.relativeOffset.x * width;
        vector.y += this.pixelOffset.y + this.relativeOffset.y * height;
        if (flag)
        {
            if (this.uiCamera.orthographic)
            {
                vector.x = Mathf.Round(vector.x);
                vector.y = Mathf.Round(vector.y);
            }
            vector.z = this.uiCamera.WorldToScreenPoint(this.mTrans.position).z;
            vector = this.uiCamera.ScreenToWorldPoint(vector);
        }
        else
        {
            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);
            if (uipanel != (UnityEngine.Object)null)
            {
                vector = uipanel.cachedTransform.TransformPoint(vector);
            }
            else if (this.container != (UnityEngine.Object)null)
            {
                Transform parent2 = this.container.transform.parent;
                if (parent2 != (UnityEngine.Object)null)
                {
                    vector = parent2.TransformPoint(vector);
                }
            }
            vector.z = this.mTrans.position.z;
        }
        if (flag && this.uiCamera.orthographic && this.mTrans.parent != (UnityEngine.Object)null)
        {
            vector = this.mTrans.parent.InverseTransformPoint(vector);
            vector.x = (Single)Mathf.RoundToInt(vector.x);
            vector.y = (Single)Mathf.RoundToInt(vector.y);
            if (this.mTrans.localPosition != vector)
            {
                this.mTrans.localPosition = vector;
            }
        }
        else if (this.mTrans.position != vector)
        {
            this.mTrans.position = vector;
        }
        if (this.runOnlyOnce && Application.isPlaying)
        {
            base.enabled = false;
        }
    }

    public Camera uiCamera;

    public GameObject container;

    public UIAnchor.Side side = UIAnchor.Side.Center;

    public Boolean runOnlyOnce = true;

    public Vector2 relativeOffset = Vector2.zero;

    public Vector2 pixelOffset = Vector2.zero;

    [SerializeField]
    [HideInInspector]
    private UIWidget widgetContainer;

    private Transform mTrans;

    private Animation mAnim;

    private Rect mRect = default(Rect);

    private UIRoot mRoot;

    private Boolean mStarted;

    public enum Side
    {
        BottomLeft,
        Left,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        Center
    }
}
