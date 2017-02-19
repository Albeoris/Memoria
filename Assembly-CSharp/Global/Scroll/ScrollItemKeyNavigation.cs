using System;
using UnityEngine;

internal class ScrollItemKeyNavigation : MonoBehaviour
{
	public RecycleListPopulator ListPopulator
	{
		get
		{
			return this.listPopulator;
		}
	}

	public void OnOtherObjectSelect(GameObject go, Boolean selected)
	{
		this.OnSelect(selected);
	}

	private void OnSelect(Boolean selected)
	{
		if (!selected)
		{
			return;
		}
		if (!base.enabled)
		{
			return;
		}
		if (this.scrollView != (UnityEngine.Object)null)
		{
			this.CheckEmptyLastRow();
			Boolean flag;
			if (this.listItem != (UnityEngine.Object)null)
			{
				flag = !this.listItem.VerifyVisibility();
			}
			else
			{
				flag = !this.ScrollPanel.IsVisible(this.VisionCheckWidget);
			}
			if (flag && !PersistenSingleton<UIManager>.Instance.IsLoading)
			{
				Vector3 pos = -this.ScrollPanel.cachedTransform.InverseTransformPoint(base.transform.position);
				if (!this.scrollView.canMoveHorizontally)
				{
					pos.x = this.ScrollPanel.cachedTransform.localPosition.x;
				}
				if (pos.y > this.ScrollPanel.cachedTransform.localPosition.y)
				{
					pos.y = this.ScrollPanel.cachedTransform.localPosition.y + this.itemHeight;
				}
				else
				{
					pos.y = this.ScrollPanel.cachedTransform.localPosition.y - this.itemHeight;
				}
				ScrollItemKeyNavigation.IsScrollMove = true;
				SpringPanel.Begin(this.ScrollPanel.cachedGameObject, pos, this.Speed).onFinished = new SpringPanel.OnFinished(this.onScrollFinished);
			}
		}
	}

	private void OnEnable()
	{
		if (this.HidePointerOnMoving)
		{
			String groupName = base.gameObject.GetComponent<ButtonGroupState>().GroupName;
			ButtonGroupState.SetPointerVisibilityToGroup(true, groupName);
		}
	}

	private void OnDisable()
	{
		if (this.HidePointerOnMoving)
		{
			String groupName = base.gameObject.GetComponent<ButtonGroupState>().GroupName;
			ButtonGroupState.SetPointerVisibilityToGroup(false, groupName);
		}
	}

	private void onScrollFinished()
	{
		ScrollItemKeyNavigation.IsScrollMove = false;
		this.ScrollButton.CheckScrollPosition();
		if (this.listPopulator != (UnityEngine.Object)null)
		{
			this.listPopulator.CheckAllVisibilty();
		}
		if (this.listItem != (UnityEngine.Object)null)
		{
			this.listItem.CheckVisibilty();
		}
	}

	public void CheckVisibility()
	{
		if (ButtonGroupState.ActiveButton == base.gameObject)
		{
			if (this.listPopulator != (UnityEngine.Object)null)
			{
				RecycleListItem component = base.gameObject.GetComponent<RecycleListItem>();
				if (!component.VerifyVisibility())
				{
					this.listPopulator.SwitchActiveItem();
				}
			}
			else if (!this.ScrollPanel.IsVisible(this.VisionCheckWidget))
			{
				this.ChangeSelectItem();
			}
		}
	}

	private void CheckEmptyLastRow()
	{
		UIKeyNavigation component = base.gameObject.GetComponent<UIKeyNavigation>();
		if (this.listPopulator != (UnityEngine.Object)null)
		{
			if (this.listItem.ItemDataIndex == this.listPopulator.ItemCount - 2)
			{
				GameObject gameObject = this.listPopulator.ItemsPool[this.listPopulator.DataTracker[this.listPopulator.ItemCount - 1]].gameObject;
				component.onDown = gameObject;
			}
			else if (component.onDown != (UnityEngine.Object)null)
			{
				base.gameObject.GetComponent<UIKeyNavigation>().onDown = (GameObject)null;
			}
		}
	}

	private void ChangeSelectItem()
	{
		Int32 childCount = this.ScrollPanel.transform.GetChild(0).childCount;
		Int32 siblingIndex = base.transform.GetSiblingIndex();
		if (siblingIndex > 0)
		{
			Int32 i = siblingIndex - 1;
			GameObject child = this.ScrollPanel.gameObject.GetChild(0).GetChild(i);
			while (i > 0)
			{
				i--;
				if (child.activeSelf)
				{
					child = this.ScrollPanel.gameObject.GetChild(0).GetChild(i);
					if (child)
					{
						ScrollItemKeyNavigation component = child.GetComponent<ScrollItemKeyNavigation>();
						if (this.ScrollPanel.IsVisible(component.VisionCheckWidget))
						{
							ButtonGroupState.ActiveButton = child;
							break;
						}
					}
				}
			}
		}
		if (siblingIndex < childCount - 2)
		{
			Int32 j = siblingIndex + 1;
			GameObject child2 = this.ScrollPanel.gameObject.GetChild(0).GetChild(j);
			while (j < childCount - 1)
			{
				j++;
				if (child2.activeSelf)
				{
					child2 = this.ScrollPanel.gameObject.GetChild(0).GetChild(j);
					if (child2)
					{
						ScrollItemKeyNavigation component2 = child2.GetComponent<ScrollItemKeyNavigation>();
						if (this.ScrollPanel.IsVisible(component2.VisionCheckWidget))
						{
							ButtonGroupState.ActiveButton = child2;
							break;
						}
					}
				}
			}
		}
	}

	private void Start()
	{
		if (this.ID == -1)
		{
			this.ID = base.gameObject.transform.GetSiblingIndex();
		}
		this.listItem = base.gameObject.GetComponent<RecycleListItem>();
		this.scrollView = this.ScrollPanel.GetComponent<UIScrollView>();
		this.listPopulator = this.ScrollPanel.GetComponent<RecycleListPopulator>();
		this.itemHeight = (Single)base.gameObject.GetComponent<UIWidget>().height;
	}

	public Int32 ID = -1;

	public Single Speed = 24f;

	public Boolean HidePointerOnMoving;

	public static Boolean IsScrollMove;

	private Single itemHeight;

	public UIPanel ScrollPanel;

	public ScrollButton ScrollButton;

	public UIWidget VisionCheckWidget;

	private RecycleListPopulator listPopulator;

	private UIScrollView scrollView;

	private RecycleListItem listItem;
}
