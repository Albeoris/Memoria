using System;
using UnityEngine;

[ExecuteInEditMode]
public class AnimatedAlpha : MonoBehaviour
{
	private void OnEnable()
	{
		this.mWidget = base.GetComponent<UIWidget>();
		this.mPanel = base.GetComponent<UIPanel>();
		this.LateUpdate();
	}

	private void LateUpdate()
	{
		if (this.mWidget != (UnityEngine.Object)null)
		{
			this.mWidget.alpha = this.alpha;
		}
		if (this.mPanel != (UnityEngine.Object)null)
		{
			this.mPanel.alpha = this.alpha;
		}
	}

	[Range(0f, 1f)]
	public Single alpha = 1f;

	private UIWidget mWidget;

	private UIPanel mPanel;
}
