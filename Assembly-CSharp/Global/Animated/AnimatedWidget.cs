using System;
using UnityEngine;

[ExecuteInEditMode]
public class AnimatedWidget : MonoBehaviour
{
	private void OnEnable()
	{
		this.mWidget = base.GetComponent<UIWidget>();
		this.LateUpdate();
	}

	private void LateUpdate()
	{
		if (this.mWidget != (UnityEngine.Object)null)
		{
			this.mWidget.width = Mathf.RoundToInt(this.width);
			this.mWidget.height = Mathf.RoundToInt(this.height);
		}
	}

	public Single width = 1f;

	public Single height = 1f;

	private UIWidget mWidget;
}
