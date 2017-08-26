using System;
using System.Collections.Generic;
using Memoria;
using Memoria.Prime;
using UnityEngine;
using Object = System.Object;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Root")]
public class UIRoot : MonoBehaviour
{
	public UIRoot.Constraint constraint
	{
		get
		{
			if (this.fitWidth)
			{
				if (this.fitHeight)
				{
					return UIRoot.Constraint.Fit;
				}
				return UIRoot.Constraint.FitWidth;
			}
			else
			{
				if (this.fitHeight)
				{
					return UIRoot.Constraint.FitHeight;
				}
				return UIRoot.Constraint.Fill;
			}
		}
	}

	public UIRoot.Scaling activeScaling
	{
		get
		{
			UIRoot.Scaling scaling = this.scalingStyle;
			if (scaling == UIRoot.Scaling.ConstrainedOnMobiles)
			{
				return UIRoot.Scaling.Flexible;
			}
			return scaling;
		}
	}

	public Int32 activeHeight
	{
		get
		{
		    if (Configuration.Graphics.WidescreenSupport)
		    {
		        this.manualWidth = (Int32)UIManager.UIContentSize.x;
                this.manualHeight = (Int32)UIManager.UIContentSize.y;
            }

		    if (this.activeScaling == UIRoot.Scaling.Flexible)
			{
				Vector2 screenSize = NGUITools.screenSize;
				Single num = screenSize.x / screenSize.y;
				if (screenSize.y < (Single)this.minimumHeight)
				{
					screenSize.y = (Single)this.minimumHeight;
					screenSize.x = screenSize.y * num;
				}
				else if (screenSize.y > (Single)this.maximumHeight)
				{
					screenSize.y = (Single)this.maximumHeight;
					screenSize.x = screenSize.y * num;
				}
				Int32 num2 = Mathf.RoundToInt((!this.shrinkPortraitUI || screenSize.y <= screenSize.x) ? screenSize.y : (screenSize.y / num));
				return (Int32)((!this.adjustByDPI) ? num2 : NGUIMath.AdjustByDPI((Single)num2));
			}
			UIRoot.Constraint constraint = this.constraint;
			if (constraint == UIRoot.Constraint.FitHeight)
			{
				return this.manualHeight;
			}
			Vector2 screenSize2 = NGUITools.screenSize;
			Single num3 = screenSize2.x / screenSize2.y;
			Single num4 = (Single)this.manualWidth / (Single)this.manualHeight;
			switch (constraint)
			{
			case UIRoot.Constraint.Fit:
				return (Int32)((num4 <= num3) ? this.manualHeight : Mathf.RoundToInt((Single)this.manualWidth / num3));
			case UIRoot.Constraint.Fill:
				return (Int32)((num4 >= num3) ? this.manualHeight : Mathf.RoundToInt((Single)this.manualWidth / num3));
			case UIRoot.Constraint.FitWidth:
				return Mathf.RoundToInt((Single)this.manualWidth / num3);
			default:
				return this.manualHeight;
			}
		}
	}

	public Single pixelSizeAdjustment
	{
		get
		{
			Int32 num = Mathf.RoundToInt(NGUITools.screenSize.y);
			return (num != -1) ? this.GetPixelSizeAdjustment(num) : 1f;
		}
	}

	public static Single GetPixelSizeAdjustment(GameObject go)
	{
		UIRoot uiroot = NGUITools.FindInParents<UIRoot>(go);
		return (!(uiroot != (UnityEngine.Object)null)) ? 1f : uiroot.pixelSizeAdjustment;
	}

	public Single GetPixelSizeAdjustment(Int32 height)
	{
		height = Mathf.Max(2, height);
		if (this.activeScaling == UIRoot.Scaling.Constrained)
		{
			return (Single)this.activeHeight / (Single)height;
		}
		if (height < this.minimumHeight)
		{
			return (Single)this.minimumHeight / (Single)height;
		}
		if (height > this.maximumHeight)
		{
			return (Single)this.maximumHeight / (Single)height;
		}
		return 1f;
	}

	protected virtual void Awake()
	{
		this.mTrans = base.transform;
	    this.mPanel = this.gameObject.GetComponent<UIPanel>();

	}

	protected virtual void OnEnable()
	{
		UIRoot.list.Add(this);
	}

	protected virtual void OnDisable()
	{
		UIRoot.list.Remove(this);
	}

	protected virtual void Start()
	{
		UIOrthoCamera componentInChildren = base.GetComponentInChildren<UIOrthoCamera>();
		if (componentInChildren != (UnityEngine.Object)null)
		{
			global::Debug.LogWarning("UIRoot should not be active at the same time as UIOrthoCamera. Disabling UIOrthoCamera.", componentInChildren);
			Camera component = componentInChildren.gameObject.GetComponent<Camera>();
			componentInChildren.enabled = false;
			if (component != (UnityEngine.Object)null)
			{
				component.orthographicSize = 1f;
			}
		}
		else
		{
			this.UpdateScale(false);
		}
	}

	private void Update()
	{
		this.UpdateScale(true);
	}

	public void UpdateScale(Boolean updateAnchors = true)
	{
	    if (this.mTrans == null)
            return;

	    if (Configuration.Graphics.WidescreenSupport)
	    {
            if (mPanel != null)
            {
                Vector4 clipRegion = mPanel.baseClipRegion;
                if (clipRegion.z < manualWidth || clipRegion.w < manualHeight)
                {
                    Log.Message("[UIRoot] Changing a clip region of the UI root.");
                    clipRegion.z = manualWidth + 2;
                    clipRegion.w = manualHeight + 2;
                    mPanel.baseClipRegion = clipRegion;
                }
            }
	    }

	    Single num = (Single)this.activeHeight;
	    if (num > 0f)
	    {
	        Single num2 = 2f / num;
	        Vector3 localScale = this.mTrans.localScale;
	        if (Mathf.Abs(localScale.x - num2) > 1.401298E-45f || Mathf.Abs(localScale.y - num2) > 1.401298E-45f || Mathf.Abs(localScale.z - num2) > 1.401298E-45f)
	        {
	            this.mTrans.localScale = new Vector3(num2, num2, num2);
	            if (updateAnchors)
	            {
	                base.BroadcastMessage("UpdateAnchors");
	            }
	        }
	    }
	}

	public static void Broadcast(String funcName)
	{
		Int32 i = 0;
		Int32 count = UIRoot.list.Count;
		while (i < count)
		{
			UIRoot uiroot = UIRoot.list[i];
			if (uiroot != (UnityEngine.Object)null)
			{
				uiroot.BroadcastMessage(funcName, SendMessageOptions.DontRequireReceiver);
			}
			i++;
		}
	}

	public static void Broadcast(String funcName, Object param)
	{
		if (param == null)
		{
			global::Debug.LogError("SendMessage is bugged when you try to pass 'null' in the parameter field. It behaves as if no parameter was specified.");
		}
		else
		{
			Int32 i = 0;
			Int32 count = UIRoot.list.Count;
			while (i < count)
			{
				UIRoot uiroot = UIRoot.list[i];
				if (uiroot != (UnityEngine.Object)null)
				{
					uiroot.BroadcastMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
				}
				i++;
			}
		}
	}

	public static List<UIRoot> list = new List<UIRoot>();

	public UIRoot.Scaling scalingStyle;

	public Int32 manualWidth = 1280;

	public Int32 manualHeight = 720;

	public Int32 minimumHeight = 320;

	public Int32 maximumHeight = 1536;

	public Boolean fitWidth;

	public Boolean fitHeight = true;

	public Boolean adjustByDPI;

	public Boolean shrinkPortraitUI;

	private Transform mTrans;

    private UIPanel mPanel;

	public enum Scaling
	{
		Flexible,
		Constrained,
		ConstrainedOnMobiles
	}

	public enum Constraint
	{
		Fit,
		Fill,
		FitWidth,
		FitHeight
	}
}
