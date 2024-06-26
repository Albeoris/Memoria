using System;
using UnityEngine;

public class NewIconAnimation : MonoBehaviour
{
	private void Update()
	{
		this.timer += Time.deltaTime;
		if (this.timer > this.duration)
		{
			this.ChangeColor();
			this.timer = 0f;
		}
	}

	private void ChangeColor()
	{
		this.isTransparent = !this.isTransparent;
		if (this.isTransparent)
		{
			base.gameObject.GetComponent<UISprite>().alpha = 0.3f;
		}
		else
		{
			base.gameObject.GetComponent<UISprite>().alpha = 1f;
		}
	}

	private Single timer;

	private Boolean isTransparent;

	private Single duration = 0.5f;
}
