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
		Boolean hasFinished = false;
		Vector3 localPosition = this.mTrans.localPosition;
		Vector3 nextPos = NGUIMath.SpringLerp(this.mTrans.localPosition, this.target, this.strength, deltaTime);
		if ((nextPos - this.target).sqrMagnitude < 0.01f)
		{
			nextPos = this.target;
			base.enabled = false;
			hasFinished = true;
		}
		this.mTrans.localPosition = nextPos;
		Vector3 diffPos = nextPos - localPosition;
		Vector2 clipOffset = this.mPanel.clipOffset;
		clipOffset.x -= diffPos.x;
		clipOffset.y -= diffPos.y;
		this.mPanel.clipOffset = clipOffset;
		if (this.mDrag != null)
			this.mDrag.UpdateScrollbars(false);
		if (hasFinished && this.onFinished != null)
		{
			SpringPanel.current = this;
			this.onFinished();
			SpringPanel.current = null;
		}
	}

	public static SpringPanel Begin(GameObject go, Vector3 pos, Single strength, SpringPanel.OnFinished finish = null)
	{
		SpringPanel springPanel = go.GetComponent<SpringPanel>();
		if (springPanel == null)
			springPanel = go.AddComponent<SpringPanel>();
		springPanel.target = pos;
		springPanel.strength = strength;
		springPanel.onFinished = finish;
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
