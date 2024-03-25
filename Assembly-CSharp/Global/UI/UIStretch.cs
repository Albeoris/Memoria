using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Stretch")]
public class UIStretch : MonoBehaviour
{
	private void Awake()
	{
		this.mAnim = base.GetComponent<Animation>();
		this.mRect = default(Rect);
		this.mTrans = base.transform;
		this.mWidget = base.GetComponent<UIWidget>();
		this.mSprite = base.GetComponent<UISprite>();
		this.mPanel = base.GetComponent<UIPanel>();
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
		if (this.uiCamera == (UnityEngine.Object)null)
		{
			this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		this.mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		this.Update();
		this.mStarted = true;
	}

	private void Update()
	{
		if (this.mAnim != (UnityEngine.Object)null && this.mAnim.isPlaying)
		{
			return;
		}
		if (this.style != UIStretch.Style.None)
		{
			UIWidget uiwidget = (!(this.container == (UnityEngine.Object)null)) ? this.container.GetComponent<UIWidget>() : ((UIWidget)null);
			UIPanel uipanel = (!(this.container == (UnityEngine.Object)null) || !(uiwidget == (UnityEngine.Object)null)) ? this.container.GetComponent<UIPanel>() : ((UIPanel)null);
			Single widgetWidth = 1f;
			if (uiwidget != (UnityEngine.Object)null)
			{
				Bounds bounds = uiwidget.CalculateBounds(base.transform.parent);
				this.mRect.x = bounds.min.x;
				this.mRect.y = bounds.min.y;
				this.mRect.width = bounds.size.x;
				this.mRect.height = bounds.size.y;
			}
			else if (uipanel != (UnityEngine.Object)null)
			{
				if (uipanel.clipping == UIDrawCall.Clipping.None)
				{
					Single widgetHeight = (!(this.mRoot != (UnityEngine.Object)null)) ? 0.5f : ((Single)this.mRoot.activeHeight / (Single)Screen.height * 0.5f);
					this.mRect.xMin = (Single)(-(Single)Screen.width) * widgetHeight;
					this.mRect.yMin = (Single)(-(Single)Screen.height) * widgetHeight;
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
				Transform parent = base.transform.parent;
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
				this.mRect = this.uiCamera.pixelRect;
				if (this.mRoot != (UnityEngine.Object)null)
				{
                    widgetWidth = this.mRoot.pixelSizeAdjustment;
				}
			}
			Single scaleFactorX = this.mRect.width;
			Single scaleFactorY = this.mRect.height;
			if (widgetWidth != 1f && scaleFactorY > 1f)
			{
				Single pixelSizeAdjustment = (Single)this.mRoot.activeHeight / scaleFactorY;
                scaleFactorX *= pixelSizeAdjustment;
                scaleFactorY *= pixelSizeAdjustment;
			}
			Vector3 vector = (!(this.mWidget != (UnityEngine.Object)null)) ? this.mTrans.localScale : new Vector3((Single)this.mWidget.width, (Single)this.mWidget.height);
			if (this.style == UIStretch.Style.BasedOnHeight)
			{
				vector.x = this.relativeSize.x * scaleFactorY;
				vector.y = this.relativeSize.y * scaleFactorY;
			}
			else if (this.style == UIStretch.Style.FillKeepingRatio)
			{
				Single widthHeightRatio = scaleFactorX / scaleFactorY;
				Single initialWidthHeightRatio = this.initialSize.x / this.initialSize.y;
				if (initialWidthHeightRatio < widthHeightRatio)
				{
					Single num8 = scaleFactorX / this.initialSize.x;
					vector.x = scaleFactorX;
					vector.y = this.initialSize.y * num8;
				}
				else
				{
					Single num9 = scaleFactorY / this.initialSize.y;
					vector.x = this.initialSize.x * num9;
					vector.y = scaleFactorY;
				}
			}
			else if (this.style == UIStretch.Style.FitInternalKeepingRatio)
			{
				Single widthHeightRatio = scaleFactorX / scaleFactorY;
				Single initialWidthHeightRatio = this.initialSize.x / this.initialSize.y;
				if (initialWidthHeightRatio > widthHeightRatio)
				{
					Single num12 = scaleFactorX / this.initialSize.x;
					vector.x = scaleFactorX;
					vector.y = this.initialSize.y * num12;
				}
				else
				{
					Single num13 = scaleFactorY / this.initialSize.y;
					vector.x = this.initialSize.x * num13;
					vector.y = scaleFactorY;
				}
			}
			else
			{
				if (this.style != UIStretch.Style.Vertical)
				{
					vector.x = this.relativeSize.x * scaleFactorX;
				}
				if (this.style != UIStretch.Style.Horizontal)
				{
					vector.y = this.relativeSize.y * scaleFactorY;
				}
			}
			if (this.mSprite != (UnityEngine.Object)null)
			{
				Single atlasPixelSize = (!(this.mSprite.atlas != (UnityEngine.Object)null)) ? 1f : this.mSprite.atlas.pixelSize;
				vector.x -= this.borderPadding.x * atlasPixelSize;
				vector.y -= this.borderPadding.y * atlasPixelSize;
				if (this.style != UIStretch.Style.Vertical)
				{
					this.mSprite.width = Mathf.RoundToInt(vector.x);
				}
				if (this.style != UIStretch.Style.Horizontal)
				{
					this.mSprite.height = Mathf.RoundToInt(vector.y);
				}
				vector = Vector3.one;
			}
			else if (this.mWidget != (UnityEngine.Object)null)
			{
				if (this.style != UIStretch.Style.Vertical)
				{
					this.mWidget.width = Mathf.RoundToInt(vector.x - this.borderPadding.x);
				}
				if (this.style != UIStretch.Style.Horizontal)
				{
					this.mWidget.height = Mathf.RoundToInt(vector.y - this.borderPadding.y);
				}
				vector = Vector3.one;
			}
			else if (this.mPanel != (UnityEngine.Object)null)
			{
				Vector4 baseClipRegion = this.mPanel.baseClipRegion;
				if (this.style != UIStretch.Style.Vertical)
				{
					baseClipRegion.z = vector.x - this.borderPadding.x;
				}
				if (this.style != UIStretch.Style.Horizontal)
				{
					baseClipRegion.w = vector.y - this.borderPadding.y;
				}
				this.mPanel.baseClipRegion = baseClipRegion;
				vector = Vector3.one;
			}
			else
			{
				if (this.style != UIStretch.Style.Vertical)
				{
					vector.x -= this.borderPadding.x;
				}
				if (this.style != UIStretch.Style.Horizontal)
				{
					vector.y -= this.borderPadding.y;
				}
			}
			if (this.mTrans.localScale != vector)
			{
				this.mTrans.localScale = vector;
			}
			if (this.runOnlyOnce && Application.isPlaying)
			{
				base.enabled = false;
			}
		}
	}

	public Camera uiCamera;

	public GameObject container;

	public UIStretch.Style style;

	public Boolean runOnlyOnce = true;

	public Vector2 relativeSize = Vector2.one;

	public Vector2 initialSize = Vector2.one;

	public Vector2 borderPadding = Vector2.zero;

	[HideInInspector]
	[SerializeField]
	private UIWidget widgetContainer;

	private Transform mTrans;

	private UIWidget mWidget;

	private UISprite mSprite;

	private UIPanel mPanel;

	private UIRoot mRoot;

	private Animation mAnim;

	private Rect mRect;

	private Boolean mStarted;

	public enum Style
	{
		None,
		Horizontal,
		Vertical,
		Both,
		BasedOnHeight,
		FillKeepingRatio,
		FitInternalKeepingRatio
	}
}
