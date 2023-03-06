using Memoria;
using System;
using UnityEngine;

public class ScrollButton : MonoBehaviour
{
	private void Start()
	{
		UIEventListener uieventListener = UIEventListener.Get(this.UpButton);
		uieventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener.onPress, new UIEventListener.BoolDelegate(this.OnUpButtonPress));
		UIEventListener uieventListener2 = UIEventListener.Get(this.DownButton);
		uieventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onPress, new UIEventListener.BoolDelegate(this.OnDownButtonPress));
		this.isScrollMove = false;
		this.listPopulator = this.ScrollViewPanel.GetComponent<RecycleListPopulator>();
		this.snapScrollView = this.ScrollViewPanel.GetComponent<SnapDragScrollView>();
		if (this.Offset == 0)
		{
			this.Offset = this.ScrollViewPanel.GetComponent<SnapDragScrollView>().ItemHeight;
		}
		this.CheckScrollPosition();
	}

	public void SetScrollButtonEnable(Boolean _isEnable)
	{
		this.isEnable = _isEnable;
		if (_isEnable)
		{
			this.CheckScrollPosition();
		}
		else
		{
			this.DisplayScrollButton(false, false);
		}
	}

	public void CheckScrollPosition()
	{
		if (this.isEnable)
		{
			this.isScrollMove = false;
			if (this.listPopulator != null)
			{
				if (this.listPopulator.ItemCount > this.listPopulator.VisibleItemCount)
				{
					Boolean isUp = true;
					Boolean isDown = true;
					if (this.listPopulator.DataTracker.ContainsKey(0))
					{
						Transform transform = this.listPopulator.ItemsPool[this.listPopulator.DataTracker[0]];
						RecycleListItem component = transform.gameObject.GetComponent<RecycleListItem>();
						if (component.VerifyVisibility())
						{
							isUp = false;
						}
					}
					Int32 key = this.listPopulator.ItemCount - 1;
					if (this.listPopulator.DataTracker.ContainsKey(key))
					{
						Transform transform2 = this.listPopulator.ItemsPool[this.listPopulator.DataTracker[key]];
						RecycleListItem component2 = transform2.gameObject.GetComponent<RecycleListItem>();
						if (component2.VerifyVisibility())
						{
							isDown = false;
						}
					}
					this.DisplayScrollButton(isUp, isDown);
				}
				else
				{
					this.DisplayScrollButton(false, false);
				}
			}
			else
			{
				Boolean isUp2;
				Boolean isDown2;
				if (this.Scrollbar.barSize < 1f)
				{
					isUp2 = (this.Scrollbar.value > 0.02f);
					isDown2 = (this.Scrollbar.value < 0.98f);
				}
				else
				{
					isUp2 = false;
					isDown2 = false;
				}
				this.DisplayScrollButton(isUp2, isDown2);
			}
		}
	}

	public void DisplayScrollButton(Boolean isUp, Boolean isDown)
	{
		if (isUp)
		{
			this.UpButton.GetComponent<BoxCollider>().enabled = true;
			this.UpButton.GetComponent<UIButton>().enabled = true;
			this.UpButton.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, true);
		}
		else
		{
			this.pressTimer = -1f;
			this.UpButton.GetComponent<BoxCollider>().enabled = false;
			this.UpButton.GetComponent<UIButton>().enabled = false;
			this.UpButton.GetComponent<UIButton>().SetState(UIButtonColor.State.Disabled, true);
		}
		if (isDown)
		{
			this.DownButton.GetComponent<BoxCollider>().enabled = true;
			this.DownButton.GetComponent<UIButton>().enabled = true;
			this.DownButton.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, true);
		}
		else
		{
			this.pressTimer = -1f;
			this.DownButton.GetComponent<BoxCollider>().enabled = false;
			this.DownButton.GetComponent<UIButton>().enabled = false;
			this.DownButton.GetComponent<UIButton>().SetState(UIButtonColor.State.Disabled, true);
		}
	}

	private void OnDownButtonPress(GameObject go, Boolean isPress)
	{
		if (isPress)
		{
			if (!this.isScrollMove)
			{
				this.MoveScroll(1f, false);
				this.pressDirectionUp = false;
				this.pressTimer = 0f;
				this.totalPressTimer = 0f;
			}
		}
		else
		{
			this.pressTimer = -1f;
		}
	}

	public void OnPageDownButtonClick()
	{
		this.JumpScroll(false);
	}

	private void OnUpButtonPress(GameObject go, Boolean isPress)
	{
		if (isPress)
		{
			if (!this.isScrollMove)
			{
				this.MoveScroll(1f, true);
				this.pressDirectionUp = true;
				this.pressTimer = 0f;
				this.totalPressTimer = 0f;
			}
		}
		else
		{
			this.pressTimer = -1f;
		}
	}

	public void OnPageUpButtonClick()
	{
		this.JumpScroll(true);
	}

	private void MoveScroll(Single multiple, Boolean isUp)
	{
		FF9Sfx.FF9SFX_Play(103);
		Single y = (!isUp) ? (this.ScrollViewPanel.transform.localPosition.y + (Single)this.Offset) : (this.ScrollViewPanel.transform.localPosition.y - (Single)this.Offset);
		this.isScrollMove = true;
		SpringPanel.Begin(this.ScrollViewPanel.cachedGameObject, new Vector3(this.ScrollViewPanel.transform.localPosition.x, y, this.ScrollViewPanel.transform.localPosition.z), this.Speed * multiple).onFinished = new SpringPanel.OnFinished(this.onScrollFinished);
	}

	private void JumpScroll(Boolean isUp)
	{
		Int32 currentSelection;
		Int32 newSelection;
		Int32 countPerPage;
		Int32 maxCount;
		if (this.listPopulator != null)
		{
			if (ButtonGroupState.ActiveButton == null)
				return;
			if (ButtonGroupState.ActiveGroup == ItemUI.ItemArrangeGroupButton)
				currentSelection = ButtonGroupState.ActiveButton.GetParent().GetComponent<RecycleListItem>().ItemDataIndex;
			else
				currentSelection = ButtonGroupState.ActiveButton.GetComponent<RecycleListItem>().ItemDataIndex;
			countPerPage = this.listPopulator.VisibleItemCount;
			maxCount = this.listPopulator.MaxItemCount;
		}
		else
		{
			currentSelection = ButtonGroupState.ActiveButton.GetComponent<ScrollItemKeyNavigation>().ID;
			countPerPage = this.snapScrollView.VisibleItem;
			maxCount = this.snapScrollView.MaxItem;
		}
		if (Configuration.Control.PSXScrollingMethod)
		{
			if (maxCount <= countPerPage)
				return;
			Int32 firstItemInPage = currentSelection;
			Int32 columnCount = this.listPopulator != null ? this.listPopulator.Column : 1;
			if (this.listPopulator != null)
			{
				foreach (Transform itemTransform in this.listPopulator.ItemsPool)
				{
					GameObject itemObj = itemTransform.gameObject;
					RecycleListItem itemRecycle = itemObj.GetComponent<RecycleListItem>();
					if (itemRecycle.ItemDataIndex < firstItemInPage && itemRecycle.VerifyVisibility())
						firstItemInPage = itemRecycle.ItemDataIndex;
				}
			}
			else
			{
				foreach (Transform itemTransform in this.snapScrollView.transform.GetChild(0))
				{
					GameObject itemObj = itemTransform.gameObject;
					ScrollItemKeyNavigation itemNavig = itemObj.GetComponent<ScrollItemKeyNavigation>();
					if (itemNavig.ID < firstItemInPage && itemNavig.ScrollPanel.IsVisible(itemNavig.VisionCheckWidget))
						firstItemInPage = itemNavig.ID;
				}
			}
			if (firstItemInPage == 0 && isUp)
				return;
			Int32 currentSelectionInPage = currentSelection - firstItemInPage;
			Int32 newPageLastItem = isUp ? Math.Max(0, firstItemInPage - columnCount) : Math.Min(maxCount - 1, firstItemInPage + 2 * countPerPage - columnCount);
			for (Int32 i = 1; i <= columnCount; i++)
				if (firstItemInPage + countPerPage == newPageLastItem + i)
					return;
			if (this.listPopulator != null)
				this.listPopulator.JumpToIndex(newPageLastItem, true);
			else
				this.snapScrollView.ScrollToIndex(newPageLastItem);
			newSelection = Math.Max(0, newPageLastItem - countPerPage + columnCount);
			newSelection -= newSelection % columnCount;
			newSelection = Math.Min(maxCount - 1, newSelection + currentSelectionInPage);
			if (this.listPopulator != null)
				this.listPopulator.JumpToIndex(newSelection, false, true);
			else
				ButtonGroupState.ActiveButton = this.snapScrollView.transform.GetChild(0).GetChild(newSelection).gameObject;
		}
		else
		{
			if (isUp)
				newSelection = currentSelection - countPerPage;
			else
				newSelection = currentSelection + countPerPage;
			newSelection /= countPerPage;
			newSelection = (newSelection + 1) * countPerPage;
			newSelection = Mathf.Clamp(newSelection - 1, 0, maxCount - 1);
			if (this.listPopulator != null)
				this.listPopulator.JumpToIndex(newSelection, newSelection != currentSelection);
			else
				this.snapScrollView.ScrollToIndex(newSelection);
		}
	}

	private void onScrollFinished()
	{
		if (this.isEnable)
		{
			this.isScrollMove = false;
			ButtonGroupState.MuteActiveSound = true;
			this.CheckScrollPosition();
			foreach (Transform itemTransform in this.ScrollViewPanel.transform.GetChild(0))
			{
				GameObject itemObj = itemTransform.gameObject;
				if (itemObj.activeSelf)
				{
					ScrollItemKeyNavigation navig = itemObj.GetComponent<ScrollItemKeyNavigation>();
					navig.enabled = true;
					navig.CheckVisibility();
					RecycleListItem recycleList = itemObj.GetComponent<RecycleListItem>();
					if (recycleList)
						recycleList.CheckVisibility();
				}
			}
			ButtonGroupState.MuteActiveSound = false;
		}
	}

	private void Update()
	{
		if (this.pressTimer > -1f)
		{
			Single num = 0.2f;
			Single multiple = 1f;
			this.pressTimer += Time.deltaTime;
			this.totalPressTimer += Time.deltaTime;
			if (this.totalPressTimer > 0.5f)
			{
				Single num2 = (this.totalPressTimer - 0.5f) / 2f;
				num2 = Mathf.Min(num2, 1f);
				multiple = Mathf.Lerp(1f, 5f, num2);
				num = Mathf.Lerp(0.2f, 0.04f, num2);
			}
			if (this.pressTimer > num && !this.isScrollMove)
			{
				this.pressTimer = 0f;
				this.MoveScroll(multiple, this.pressDirectionUp);
			}
		}
	}

	private const Single MinTriggerPressTime = 0.2f;

	private const Single MaxTriggerPressTime = 0.04f;

	private const Single MaxSpeed = 5f;

	private const Single IncreaseAmountTime = 0.5f;

	private const Single MaxSpeedAmountTime = 2.5f;

	public GameObject UpButton;

	public GameObject DownButton;

	public UIScrollBar Scrollbar;

	public UIPanel ScrollViewPanel;

	public Int32 Offset;

	public Single Speed = 48f;

	private Boolean isEnable;

	private Boolean isScrollMove;

	private Single pressTimer = -1f;

	private Single totalPressTimer;

	private Boolean pressDirectionUp;

	private RecycleListPopulator listPopulator;

	private SnapDragScrollView snapScrollView;
}
