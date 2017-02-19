using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Scroll View")]
public class UIDragScrollView : MonoBehaviour
{
	private void OnEnable()
	{
		this.mTrans = base.transform;
		if (this.scrollView == (UnityEngine.Object)null && this.draggablePanel != (UnityEngine.Object)null)
		{
			this.scrollView = this.draggablePanel;
			this.draggablePanel = (UIScrollView)null;
		}
		if (this.mStarted && (this.mAutoFind || this.mScroll == (UnityEngine.Object)null))
		{
			this.FindScrollView();
		}
	}

	private void Start()
	{
		this.mStarted = true;
		this.FindScrollView();
	}

	private void FindScrollView()
	{
		UIScrollView uiscrollView = NGUITools.FindInParents<UIScrollView>(this.mTrans);
		if (this.scrollView == (UnityEngine.Object)null || (this.mAutoFind && uiscrollView != this.scrollView))
		{
			this.scrollView = uiscrollView;
			this.mAutoFind = true;
		}
		else if (this.scrollView == uiscrollView)
		{
			this.mAutoFind = true;
		}
		this.mScroll = this.scrollView;
	}

	private void OnPress(Boolean pressed)
	{
		if (this.mAutoFind && this.mScroll != this.scrollView)
		{
			this.mScroll = this.scrollView;
			this.mAutoFind = false;
		}
		if (this.scrollView && base.enabled && NGUITools.GetActive(base.gameObject))
		{
			this.scrollView.Press(pressed);
			if (!pressed && this.mAutoFind)
			{
				this.scrollView = NGUITools.FindInParents<UIScrollView>(this.mTrans);
				this.mScroll = this.scrollView;
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (this.scrollView && NGUITools.GetActive(this))
		{
			this.scrollView.Drag();
		}
	}

	private void OnScroll(Single delta)
	{
		if (this.scrollView && NGUITools.GetActive(this))
		{
			this.scrollView.Scroll(delta);
		}
	}

	public void OnPan(Vector2 delta)
	{
		if (this.scrollView && NGUITools.GetActive(this))
		{
			this.scrollView.OnPan(delta);
		}
	}

	public UIScrollView scrollView;

	[SerializeField]
	[HideInInspector]
	private UIScrollView draggablePanel;

	private Transform mTrans;

	private UIScrollView mScroll;

	private Boolean mAutoFind;

	private Boolean mStarted;
}
