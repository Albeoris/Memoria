using System;
using UnityEngine;
using Object = System.Object;

internal class SnapDragScrollView : MonoBehaviour
{
	public Single StartPostionY
	{
		set => this.startPosY = value;
	}

	private void Awake()
	{
		this.scrollViewPanel = base.gameObject.GetComponent<UIPanel>();
		this.draggablePanel = base.gameObject.GetComponent<UIScrollView>();
		this.populator = base.gameObject.GetComponent<RecycleListPopulator>();
		this.startPosY = this.scrollViewPanel.transform.localPosition.y;
	}

	private void Start()
	{
		this.scrollViewPanel.GetComponent<UIScrollView>().onStoppedMoving += this.SnappingScroll;
		this.startPosY = this.scrollViewPanel.transform.localPosition.y;
	}

	private void Update()
	{
		if (base.enabled && (Mathf.Abs(this.draggablePanel.currentMomentum.y) > 0f || Mathf.Abs(this.draggablePanel.currentMomentum.x) > 0f))
		{
			this.isStartMove = true;
			if (this.isStartMove)
			{
				foreach (Object obj in base.gameObject.transform.GetChild(0))
				{
					Transform transform = (Transform)obj;
					GameObject gameObject = transform.gameObject;
					ScrollItemKeyNavigation component = gameObject.GetComponent<ScrollItemKeyNavigation>();
					component.enabled = false;
					component.CheckVisibility();
				}
			}
		}
	}

	private void OnEnable()
	{
		this.isScrollMove = false;
	}

	public void ScrollToIndex(Int32 index)
	{
		Single value = 0f;
		if (index > this.VisibleItem - 1)
		{
			Int32 num = this.MaxItem - this.VisibleItem;
			value = ((Single)index + 1f - (Single)this.VisibleItem) / (Single)num;
		}
		this.draggablePanel.verticalScrollBar.value = value;
		this.SnappingScroll();
	}

	private void SnappingScroll()
	{
		if (base.enabled)
		{
			this.isStartMove = false;
			Single distY = this.scrollViewPanel.transform.localPosition.y - this.startPosY;
			if (Mathf.RoundToInt(distY) % Mathf.RoundToInt(this.ItemHeight) != 0)
			{
				if (this.isScrollMove)
				{
					this.OnSnapFinish();
					UnityEngine.Object.DestroyImmediate(SpringPanel.current);
				}
				Int32 deltaY = (Int32)Math.Round(distY / this.ItemHeight) * this.ItemHeight;
				this.isScrollMove = true;
				SpringPanel.Begin(this.scrollViewPanel.cachedGameObject, new Vector3(this.scrollViewPanel.transform.localPosition.x, this.startPosY + deltaY, this.scrollViewPanel.transform.localPosition.z), this.Speed, this.OnSnapFinish);
			}
			else
			{
				this.OnSnapFinish();
			}
		}
	}

	private void OnSnapFinish()
	{
		this.isScrollMove = false;
		this.ScrollButton.CheckScrollPosition();
		foreach (Object obj in base.transform.GetChild(0))
		{
			Transform transform = (Transform)obj;
			GameObject gameObject = transform.gameObject;
			if (gameObject.activeSelf)
			{
				ScrollItemKeyNavigation component = gameObject.GetComponent<ScrollItemKeyNavigation>();
				component.enabled = true;
				component.CheckVisibility();
			}
		}
		ButtonGroupState.UpdateActiveButton();
	}

	public ScrollButton ScrollButton;

	public Int32 Speed = 24;

	public Int32 VisibleItem = 1;

	public Int32 MaxItem = 1;

	public Int32 ItemHeight;

	private Single startPosY;

	private Boolean isScrollMove;

	private Boolean isStartMove;

	private UIPanel scrollViewPanel;

	private UIScrollView draggablePanel;

	private RecycleListPopulator populator;
}
