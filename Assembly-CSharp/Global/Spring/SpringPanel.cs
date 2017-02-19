using System;
using UnityEngine;

[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Internal/Spring Panel")]
public class SpringPanel : MonoBehaviour
{
	private void Start()
	{
		this.mPanel = base.GetComponent<UIPanel>();
		this.mDrag = base.GetComponent<UIScrollView>();
		this.mTrans = base.transform;
	}

	private void Update()
	{
		this.AdvanceTowardsPosition();
	}

	protected virtual void AdvanceTowardsPosition()
	{
		Single deltaTime = RealTime.deltaTime;
		Boolean flag = false;
		Vector3 localPosition = this.mTrans.localPosition;
		Vector3 vector = NGUIMath.SpringLerp(this.mTrans.localPosition, this.target, this.strength, deltaTime);
		if ((vector - this.target).sqrMagnitude < 0.01f)
		{
			vector = this.target;
			base.enabled = false;
			flag = true;
		}
		this.mTrans.localPosition = vector;
		Vector3 vector2 = vector - localPosition;
		Vector2 clipOffset = this.mPanel.clipOffset;
		clipOffset.x -= vector2.x;
		clipOffset.y -= vector2.y;
		this.mPanel.clipOffset = clipOffset;
		if (this.mDrag != (UnityEngine.Object)null)
		{
			this.mDrag.UpdateScrollbars(false);
		}
		if (flag && this.onFinished != null)
		{
			SpringPanel.current = this;
			this.onFinished();
			SpringPanel.current = (SpringPanel)null;
		}
	}

	public static SpringPanel Begin(GameObject go, Vector3 pos, Single strength)
	{
		SpringPanel springPanel = go.GetComponent<SpringPanel>();
		if (springPanel == (UnityEngine.Object)null)
		{
			springPanel = go.AddComponent<SpringPanel>();
		}
		springPanel.target = pos;
		springPanel.strength = strength;
		springPanel.onFinished = (SpringPanel.OnFinished)null;
		springPanel.enabled = true;
		return springPanel;
	}

	public static SpringPanel current;

	public Vector3 target = Vector3.zero;

	public Single strength = 10f;

	public SpringPanel.OnFinished onFinished;

	private UIPanel mPanel;

	private Transform mTrans;

	private UIScrollView mDrag;

	public delegate void OnFinished();
}
