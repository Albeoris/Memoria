using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/UI/Viewport Camera")]
[ExecuteInEditMode]
public class UIViewport : MonoBehaviour
{
	private void Start()
	{
		this.mCam = base.GetComponent<Camera>();
		if (this.sourceCamera == (UnityEngine.Object)null)
		{
			this.sourceCamera = Camera.main;
		}
	}

	private void LateUpdate()
	{
		if (this.topLeft != (UnityEngine.Object)null && this.bottomRight != (UnityEngine.Object)null)
		{
			if (this.topLeft.gameObject.activeInHierarchy)
			{
				Vector3 vector = this.sourceCamera.WorldToScreenPoint(this.topLeft.position);
				Vector3 vector2 = this.sourceCamera.WorldToScreenPoint(this.bottomRight.position);
				Rect rect = new Rect(vector.x / (Single)Screen.width, vector2.y / (Single)Screen.height, (vector2.x - vector.x) / (Single)Screen.width, (vector.y - vector2.y) / (Single)Screen.height);
				Single num = this.fullSize * rect.height;
				if (rect != this.mCam.rect)
				{
					this.mCam.rect = rect;
				}
				if (this.mCam.orthographicSize != num)
				{
					this.mCam.orthographicSize = num;
				}
				this.mCam.enabled = true;
			}
			else
			{
				this.mCam.enabled = false;
			}
		}
	}

	public Camera sourceCamera;

	public Transform topLeft;

	public Transform bottomRight;

	public Single fullSize = 1f;

	private Camera mCam;
}
