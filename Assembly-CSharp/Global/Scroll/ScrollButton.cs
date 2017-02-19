using System;
using UnityEngine;
using Object = System.Object;

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
			if (this.listPopulator != (UnityEngine.Object)null)
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
		Int32 num;
		Int32 num2;
		Int32 num3;
		if (this.listPopulator != (UnityEngine.Object)null)
		{
			if (ButtonGroupState.ActiveButton == (UnityEngine.Object)null)
			{
				return;
			}
			if (ButtonGroupState.ActiveGroup == ItemUI.ItemArrangeGroupButton)
			{
				num = ButtonGroupState.ActiveButton.GetParent().GetComponent<RecycleListItem>().ItemDataIndex;
			}
			else
			{
				num = ButtonGroupState.ActiveButton.GetComponent<RecycleListItem>().ItemDataIndex;
			}
			num2 = this.listPopulator.VisibleItemCount;
			num3 = this.listPopulator.MaxItemCount;
		}
		else
		{
			num = ButtonGroupState.ActiveButton.GetComponent<ScrollItemKeyNavigation>().ID;
			num2 = this.snapScrollView.VisibleItem;
			num3 = this.snapScrollView.MaxItem;
		}
		Int32 num4;
		if (isUp)
		{
			num4 = num - num2;
		}
		else
		{
			num4 = num + num2;
		}
		num4 /= num2;
		num4 = (num4 + 1) * num2;
		num4 = Mathf.Clamp(num4 - 1, 0, num3 - 1);
		if (this.listPopulator != (UnityEngine.Object)null)
		{
			this.listPopulator.JumpToIndex(num4, num4 != num);
		}
		else
		{
			this.snapScrollView.ScrollToIndex(num4);
		}
	}

	private void onScrollFinished()
	{
		if (this.isEnable)
		{
			this.isScrollMove = false;
			ButtonGroupState.MuteActiveSound = true;
			this.CheckScrollPosition();
			foreach (Object obj in this.ScrollViewPanel.transform.GetChild(0))
			{
				Transform transform = (Transform)obj;
				GameObject gameObject = transform.gameObject;
				if (gameObject.activeSelf)
				{
					ScrollItemKeyNavigation component = gameObject.GetComponent<ScrollItemKeyNavigation>();
					component.enabled = true;
					component.CheckVisibility();
					RecycleListItem component2 = gameObject.GetComponent<RecycleListItem>();
					if (component2)
					{
						component2.CheckVisibilty();
					}
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
