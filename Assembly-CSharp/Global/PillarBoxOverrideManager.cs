using System;
using UnityEngine;
using UnityEngine.UI;

public class PillarBoxOverrideManager : MonoBehaviour
{
	private void Start()
	{
		this.topRect = base.gameObject.FindChild("Top").GetComponent<RectTransform>();
		this.bottomRect = base.gameObject.FindChild("Bottom").GetComponent<RectTransform>();
		this.leftRect = base.gameObject.FindChild("Left").GetComponent<RectTransform>();
		this.rightRect = base.gameObject.FindChild("Right").GetComponent<RectTransform>();
		this.leftImage = base.gameObject.FindChild("Left").GetComponent<Image>();
		this.rightImage = base.gameObject.FindChild("Right").GetChild(0).GetComponent<Image>();
		this.UpdatePillarSize();
	}

	public void Restart()
	{
		this.UpdatePillarSize();
	}

	private void LateUpdate()
	{
	}

	private void UpdatePillarSize()
	{
		this.topRect.sizeDelta = new Vector2(0f, Mathf.Max(UIManager.UIPillarBoxSize.y, 1f));
		this.bottomRect.sizeDelta = new Vector2(0f, Mathf.Max(UIManager.UIPillarBoxSize.y, 1f));
		this.leftRect.sizeDelta = new Vector2(Mathf.Max(UIManager.UIPillarBoxSize.x, 1f), 0f);
		this.rightRect.sizeDelta = new Vector2(Mathf.Max(UIManager.UIPillarBoxSize.x, 1f), 0f);
		if (this.leftRect.sizeDelta.x <= 1f)
		{
			this.leftImage.color = Color.black;
		}
		else
		{
			this.leftImage.color = Color.white;
		}
		if (this.rightRect.sizeDelta.x <= 1f)
		{
			this.rightImage.color = Color.black;
		}
		else
		{
			this.rightImage.color = Color.white;
		}
	}

	private RectTransform topRect;

	private RectTransform bottomRect;

	private RectTransform leftRect;

	private RectTransform rightRect;

	private Image leftImage;

	private Image rightImage;
}
