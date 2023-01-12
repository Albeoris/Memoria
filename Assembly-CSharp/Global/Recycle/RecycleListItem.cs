using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria;

public class RecycleListItem : MonoBehaviour
{
	private void Start()
	{
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		foreach (GameObject gameObject in this.TriggerWidget)
		{
			UIEventListener.Get(gameObject).onClick = new UIEventListener.VoidDelegate(this.OnTriggerClick);
			gameObject.GetComponent<UIDragScrollView>().scrollView = this.ListPopulator.draggablePanel;
		}
	}

	private void Update()
	{
		if (Mathf.Abs(this.ListPopulator.draggablePanel.currentMomentum.y) > 0f)
			this.CheckVisibility();
	}

	public Boolean VerifyVisibility()
	{
		return this.Panel != null && this.Panel.IsVisible(this.VisionCheckWidget);
	}

	private void OnClick()
	{
		if (UIKeyTrigger.IsOnlyTouchAndLeftClick())
			this.ListPopulator.itemClicked(this.ItemDataIndex);
	}

	private void OnPress(Boolean isDown)
	{
		if (UIKeyTrigger.IsOnlyTouchAndLeftClick())
			this.ListPopulator.itemIsPressed(this.ItemDataIndex, isDown);
	}

	private void OnTriggerClick(GameObject go)
	{
		if (UIKeyTrigger.IsOnlyTouchAndLeftClick())
			this.ListPopulator.itemClicked(go);
	}

	public void CheckVisibility()
	{
		if (!base.gameObject.activeSelf)
			return;
		Boolean isVisibleNow = this.Panel.IsVisible(this.VisionCheckWidget);
		if (isVisibleNow != this.isVisible)
		{
			this.isVisible = isVisibleNow;
			if (!this.isVisible)
			{
				base.StartCoroutine(this.ListPopulator.ItemIsInvisible(this.ItemNumber));
				if (ButtonGroupState.ActiveButton == base.gameObject)
					this.ListPopulator.SwitchActiveItem();
			}
		}
	}

	public UIWidget VisionCheckWidget;

	public UIPanel Panel;

	public RecycleListPopulator ListPopulator;

	public List<GameObject> TriggerWidget;

	public Int32 ItemNumber;

	public Int32 ItemDataIndex;

	private Boolean isVisible = true;
}
