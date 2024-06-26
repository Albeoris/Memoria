using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Drag Camera")]
public class UIDragCamera : MonoBehaviour
{
	private void Awake()
	{
		if (this.draggableCamera == (UnityEngine.Object)null)
		{
			this.draggableCamera = NGUITools.FindInParents<UIDraggableCamera>(base.gameObject);
		}
	}

	private void OnPress(Boolean isPressed)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != (UnityEngine.Object)null)
		{
			this.draggableCamera.Press(isPressed);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != (UnityEngine.Object)null)
		{
			this.draggableCamera.Drag(delta);
		}
	}

	private void OnScroll(Single delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != (UnityEngine.Object)null)
		{
			this.draggableCamera.Scroll(delta);
		}
	}

	public UIDraggableCamera draggableCamera;
}
