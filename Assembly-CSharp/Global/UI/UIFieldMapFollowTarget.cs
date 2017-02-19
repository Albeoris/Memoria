using System;
using UnityEngine;

public class UIFieldMapFollowTarget : MonoBehaviour
{
	public Vector3 TransformOffset
	{
		get
		{
			return this.transformOffset;
		}
		set
		{
			this.transformOffset = value;
		}
	}

	public Vector3 UIOffset
	{
		get
		{
			return this.uiOffset;
		}
		set
		{
			this.uiOffset = value;
		}
	}

	private void Start()
	{
		this.selfPosition = base.transform;
	}

	private Boolean IsNull()
	{
		return this.target == (UnityEngine.Object)null || this.fieldMap == (UnityEngine.Object)null || this.uiCam == (UnityEngine.Object)null;
	}

	private void LateUpdate()
	{
		if (!this.IsNull())
		{
			this.FollowTarget();
		}
	}

	private void FollowTarget()
	{
		if (this.lastPosition != this.target.position || this.updateEveryFrame)
		{
			this.lastPosition = this.target.position;
			Camera mainCamera = this.fieldMap.GetMainCamera();
			BGCAM_DEF currentBgCamera = this.fieldMap.GetCurrentBgCamera();
			Vector2 cameraOffset = this.fieldMap.GetCameraOffset();
			Vector3 position = PSX.CalculateGTE_RTPT(this.lastPosition + this.transformOffset, Matrix4x4.identity, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
			position.x -= cameraOffset.x;
			position.y -= cameraOffset.y;
			position.x *= UIManager.ResourceXMultipier;
			position.y *= UIManager.ResourceYMultipier;
			position.z = 0f;
			this.selfPosition.position = this.uiCam.transform.parent.TransformPoint(position);
			this.selfPosition.localPosition = new Vector3(this.selfPosition.localPosition.x, this.selfPosition.localPosition.y, 0f);
			this.selfPosition.localPosition += this.uiOffset;
		}
	}

	public Transform target;

	public FieldMap fieldMap;

	public Camera uiCam;

	[SerializeField]
	private Vector3 transformOffset = Vector3.zero;

	[SerializeField]
	private Vector3 uiOffset = Vector3.zero;

	public Boolean updateEveryFrame;

	private Transform selfPosition;

	private Vector3 lastPosition;
}
