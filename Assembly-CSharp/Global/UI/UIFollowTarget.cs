using System;
using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
	private void Start()
	{
		this.myTrans = base.transform;
		this.mStart = true;
	}

	private void Update()
	{
		this.UpdateUIPosition();
	}

	private Boolean isNull()
	{
		return this.worldCam == (UnityEngine.Object)null || this.uiCam == (UnityEngine.Object)null || this.target == (UnityEngine.Object)null;
	}

	public void UpdateUIPosition()
	{
		if (!this.mStart)
		{
			this.Start();
		}
		if (!this.isNull() && (this.lastPosition != this.target.position || !this.worldCam.worldToCameraMatrix.Equals(this.lastWorldMatrix) || this.updateEveryFrame))
		{
			this.lastPosition = this.target.position;
			this.lastWorldMatrix = this.worldCam.worldToCameraMatrix;
			Vector3 position = this.worldCam.WorldToScreenPoint(this.target.position + this.targetTransformOffset);
			Vector3 vector = this.uiCam.ScreenToWorldPoint(position);
			this.myTrans.position = new Vector3(vector.x, vector.y, 0f);
			this.myTrans.localPosition += this.UIOffset;
		}
	}

	public Camera worldCam;

	public Camera uiCam;

	public Transform target;

	public Vector3 targetTransformOffset;

	public Vector3 UIOffset;

	public Boolean updateEveryFrame;

	private Transform myTrans;

	private Vector3 lastPosition;

	private Matrix4x4 lastWorldMatrix;

	private Boolean mStart;
}
