using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecycleListPopulator : MonoBehaviour
{
	public event RecycleListPopulator.RecycleListItemPress OnRecycleListItemPress;

	public event RecycleListPopulator.RecycleListItemClick OnRecycleListItemClick;

	public Int32 ItemCount => this.dataList.Count;

	public Int32 VisibleItemCount => this.visibleSize;

	public Int32 PageFirstItem
	{
		get
		{
			Int32 it = Int32.MaxValue;
			foreach (Transform itemTransform in this.ItemsPool)
			{
				RecycleListItem itemRecycle = itemTransform.GetComponent<RecycleListItem>();
				if (itemRecycle.ItemDataIndex < it)
				{
					Vector3 dist = this.panel.DistanceToFullVisibleArea(itemRecycle.VisionCheckWidget);
					if (Math.Abs(dist.y) < itemRecycle.VisionCheckWidget.height / 2f)
						it = itemRecycle.ItemDataIndex;
				}
			}
			return it;
		}
	}

	public Int32 MaxItemCount => this.dataList.Count<ListDataTypeBase>();

	public Int32 Column => Math.Max(1, this.table.columns);

	public Dictionary<Int32, Int32> DataTracker => this.dataTracker;

	public List<Transform> ItemsPool => this.itemsPool;

	public Int32 ActiveNumber
	{
		set => this.activeNumber = value;
	}

	private void Awake()
	{
		if (this.itemPrefab == null)
			global::Debug.LogError("InfiniteListPopulator:: itemPrefab is not assigned");
		else if (!this.itemPrefab.tag.Equals(ITEM_TAG))
			global::Debug.LogError("InfiniteListPopulator:: itemPrefab tag should be Item List");
		this.snapDragPanel = base.gameObject.GetComponent<SnapDragScrollView>();
	}

	private void Update()
	{
		if (this.scrollIndicator != null)
		{
			if (Mathf.Abs(this.draggablePanel.currentMomentum.y) > 0f && this.scrollIndicator.GetComponent<TweenAlpha>().from > 0f)
			{
				this.scrollIndicator.GetComponent<TweenAlpha>().from = 0f;
				this.scrollIndicator.GetComponent<TweenAlpha>().to = 0.25f;
				this.scrollIndicator.GetComponent<TweenAlpha>().duration = 0.5f;
				this.scrollIndicator.GetComponent<TweenAlpha>().enabled = true;
				this.scrollIndicator.GetComponent<TweenAlpha>().delay = 0f;
				UITweener.Begin<TweenAlpha>(this.scrollIndicator.gameObject, 0f);
			}
			if (Mathf.Abs(this.draggablePanel.currentMomentum.y) == 0f && this.scrollIndicator.GetComponent<TweenAlpha>().to > 0f)
			{
				this.scrollIndicator.GetComponent<TweenAlpha>().from = 0.25f;
				this.scrollIndicator.GetComponent<TweenAlpha>().to = 0f;
				this.scrollIndicator.GetComponent<TweenAlpha>().duration = 0.5f;
				this.scrollIndicator.GetComponent<TweenAlpha>().enabled = true;
				this.scrollIndicator.GetComponent<TweenAlpha>().delay = 1f;
				UITweener.Begin<TweenAlpha>(this.scrollIndicator.gameObject, 0.25f);
			}
			Single y = (Single)(-this.scrollCursor) / (Single)this.dataList.Count * this.panel.baseClipRegion.w;
			this.scrollIndicator.localPosition = new Vector3(this.scrollIndicator.localPosition.x, y, this.scrollIndicator.localPosition.z);
		}
	}

	public void JumpToIndex(Int32 _activeIndex, Boolean forceReset = false, Boolean preventReset = false)
	{
		if (!preventReset && (this.CheckNeedToJump(_activeIndex) || forceReset))
		{
			this.activeNumber = _activeIndex;
			this.InitTableViewImp(this.dataList, _activeIndex);
		}
		else
		{
			this.activeNumber = _activeIndex;
			this.UpdateTableViewImp();
		}
	}

	public void SetOriginalData(List<ListDataTypeBase> inDataList, Boolean refreshSelection = true)
	{
		if (this.dataList.Count == inDataList.Count)
		{
			this.dataList = new List<ListDataTypeBase>(inDataList);
			this.UpdateTableViewImp(refreshSelection);
		}
		else
		{
			this.dataList = new List<ListDataTypeBase>(inDataList);
			this.RefreshTableView();
		}
	}

	public void RefreshTableView()
	{
		if (this.dataList == null || this.dataList.Count == 0)
			return;
		this.InitTableView(this.dataList, 0);
	}

	public void InitTableView(List<ListDataTypeBase> inDataList, Int32 _activeIndex)
	{
		this.InitTableViewImp(inDataList, _activeIndex);
	}

	public void CheckAllVisibilty()
	{
		foreach (Transform transform in this.itemsPool)
		{
			RecycleListItem component = transform.gameObject.GetComponent<RecycleListItem>();
			component.CheckVisibility();
		}
	}

	public Transform GetItem(Int32 dataId)
	{
		if (this.dataTracker.ContainsKey(dataId))
		{
			Int32 index = this.dataTracker[dataId];
			return this.itemsPool[index];
		}
		return null;
	}

	public void SwitchActiveItem()
	{
		if (this.dataTracker.TryGetValue(this.activeNumber + this.Column, out Int32 poolIndex))
		{
			RecycleListItem item = this.itemsPool[poolIndex].GetComponent<RecycleListItem>();
			if (item.VerifyVisibility())
			{
				ButtonGroupState.ActiveButton = this.itemsPool[poolIndex].gameObject;
				return;
			}
		}
		if (this.dataTracker.TryGetValue(this.activeNumber - this.Column, out poolIndex))
		{
			RecycleListItem item = this.itemsPool[poolIndex].GetComponent<RecycleListItem>();
			if (item.VerifyVisibility())
			{
				ButtonGroupState.ActiveButton = this.itemsPool[poolIndex].gameObject;
				return;
			}
		}
	}

	private void InitTableViewImp(List<ListDataTypeBase> inDataList, Int32 inActiveIndex)
	{
		this.RefreshPool();
		this.activeNumber = inActiveIndex;
		this.dataTracker.Clear();
		this.dataList = new List<ListDataTypeBase>(inDataList);
		if (this.dataList.Count < this.visibleSize || this.activeNumber < this.visibleSize)
			this.startNumber = 0;
		else
			this.startNumber = this.activeNumber - (this.visibleSize - this.Column);
		this.scrollCursor = this.startNumber;
		Int32 visibleCount = this.poolSize - this.extraBuffer;
		Int32 startPoolIndex = this.startNumber - this.startNumber / this.poolSize * this.poolSize;
		Int32 beforeStartCount;
		Int32 afterVisibleMaxCount;
		if (this.dataList.Count <= this.poolSize)
		{
			beforeStartCount = this.startNumber;
			afterVisibleMaxCount = this.dataList.Count - (this.startNumber + this.visibleSize);
		}
		else if (this.startNumber < this.extraBuffer / 2)
		{
			beforeStartCount = this.startNumber;
			afterVisibleMaxCount = this.extraBuffer - beforeStartCount;
		}
		else if (this.dataList.Count > this.poolSize && this.dataList.Count - (this.startNumber + this.visibleSize) < this.extraBuffer / 2)
		{
			afterVisibleMaxCount = this.dataList.Count - (this.startNumber + this.visibleSize);
			beforeStartCount = this.extraBuffer - afterVisibleMaxCount;
		}
		else
		{
			afterVisibleMaxCount = (beforeStartCount = this.extraBuffer / 2);
		}
		Int32 visibleIndex = 0;
		Int32 afterVisibleIndex = 0;
		Int32 dataIndex;
		Int32 poolIndex;
		for (Int32 i = 0; i < this.poolSize; i++)
		{
			if (beforeStartCount > 0)
			{
				dataIndex = this.startNumber - beforeStartCount;
				poolIndex = startPoolIndex - beforeStartCount;
				if (poolIndex < 0)
					poolIndex += this.poolSize;
				if (dataIndex >= this.dataList.Count)
					break;
				this.InitListItemWithIndex(dataIndex, poolIndex);
				beforeStartCount--;
			}
			else if (visibleIndex < visibleCount)
			{
				dataIndex = this.startNumber + visibleIndex;
				poolIndex = startPoolIndex + visibleIndex;
				if (poolIndex >= this.poolSize)
					poolIndex -= this.poolSize;
				if (dataIndex >= this.dataList.Count)
					break;
				this.InitListItemWithIndex(dataIndex, poolIndex);
				visibleIndex++;
			}
			else
			{
				if (afterVisibleIndex >= afterVisibleMaxCount)
					break;
				dataIndex = this.startNumber + visibleCount + afterVisibleIndex;
				poolIndex = startPoolIndex + visibleCount + afterVisibleIndex;
				if (poolIndex >= this.poolSize)
					poolIndex -= this.poolSize;
				if (dataIndex >= this.dataList.Count)
					break;
				this.InitListItemWithIndex(dataIndex, poolIndex);
				afterVisibleIndex++;
			}
		}
		base.Invoke("RepositionList", 0.02f);
	}

	private void UpdateTableViewImp(Boolean refreshSelection = true)
	{
		foreach (KeyValuePair<Int32, Int32> kvp in this.dataTracker)
		{
			Transform poolObj = this.itemsPool[kvp.Value];
			if (refreshSelection && poolObj.GetComponent<UIKeyNavigation>())
				poolObj.GetComponent<UIKeyNavigation>().startsSelected = this.activeNumber == kvp.Key;
			this.PopulateListItemWithData(poolObj, this.dataList[kvp.Key], kvp.Key, false);
		}
	}

	private void RepositionList()
	{
		this.table.Reposition();
		this.draggablePanel.SetDragAmount(0f, 0f, false);
		foreach (KeyValuePair<Int32, Int32> kvp in this.dataTracker)
		{
			Int32 lineNo = kvp.Key / this.Column;
			Transform poolObj = this.itemsPool[kvp.Value];
			if (this.startNumber > 0)
				lineNo -= this.startNumber / this.Column;
			poolObj.localPosition = new Vector3(poolObj.localPosition.x, -(this.cellHeight / 2f + lineNo * this.cellHeight), poolObj.localPosition.z);
		}
		this.draggablePanel.SetDragAmount(0f, 0f, true);
		if (this.ScrollButton)
			this.ScrollButton.CheckScrollPosition();
		if (this.snapDragPanel)
			this.snapDragPanel.StartPostionY = this.panel.transform.localPosition.y;
	}

	private void InitListItemWithIndex(Int32 dataIndex, Int32 poolIndex)
	{
		Transform poolObj = this.GetItemFromPool(poolIndex);
		RecycleListItem recylceItem = poolObj.GetComponent<RecycleListItem>();
		recylceItem.ItemDataIndex = dataIndex;
		recylceItem.ListPopulator = this;
		recylceItem.Panel = this.panel;
		ScrollItemKeyNavigation scrollNavig = poolObj.GetComponent<ScrollItemKeyNavigation>();
		if (scrollNavig)
		{
			scrollNavig.ScrollButton = this.ScrollButton;
			scrollNavig.ScrollPanel = this.panel;
		}
		UIKeyNavigation navig = ButtonGroupState.ActiveGroup == ItemUI.ItemArrangeGroupButton ? poolObj.GetChild(1).GetComponent<UIKeyNavigation>() : poolObj.GetComponent<UIKeyNavigation>();
		if (navig)
			navig.startsSelected = this.activeNumber == dataIndex;
		ButtonGroupState buttonGroup = poolObj.GetComponent<ButtonGroupState>();
		if (buttonGroup)
			ButtonGroupState.RemoveActiveStateOnGroup(poolObj.gameObject, buttonGroup.GroupName);
		poolObj.name = $"Item {dataIndex}";
		this.PopulateListItemWithData(poolObj, this.dataList[dataIndex], dataIndex, true);
		this.dataTracker.Add(dataIndex, poolIndex);
	}

	private void PrepareListItemWithIndex(Transform item, Int32 newIndex, Int32 oldIndex)
	{
		if (newIndex < oldIndex)
			item.localPosition += new Vector3(0f, (this.poolSize / this.Column) * this.cellHeight, 0f);
		else
			item.localPosition -= new Vector3(0f, (this.poolSize / this.Column) * this.cellHeight, 0f);
		item.GetComponent<RecycleListItem>().ItemDataIndex = newIndex;
		UIKeyNavigation navig = item.GetComponent<UIKeyNavigation>();
		if (navig)
			navig.startsSelected = this.activeNumber == newIndex;
		if (oldIndex == this.activeNumber)
			ButtonGroupState.RemoveActiveStateOnGroup(item.gameObject, item.GetComponent<ButtonGroupState>().GroupName);
		if (newIndex == this.activeNumber)
			ButtonGroupState.HoldActiveStateOnGroup(item.gameObject, item.GetComponent<ButtonGroupState>().GroupName);
		item.name = $"Item {newIndex}";
		this.PopulateListItemWithData(item, this.dataList[newIndex], newIndex, false);
		this.dataTracker.Add(newIndex, this.dataTracker[oldIndex]);
		this.dataTracker.Remove(oldIndex);
	}

	private Boolean CheckNeedToJump(Int32 newActiveIndex)
	{
		if (this.dataTracker.ContainsKey(newActiveIndex))
		{
			Transform transform = this.itemsPool[this.dataTracker[newActiveIndex]];
			if (transform)
			{
				RecycleListItem component = transform.GetComponent<RecycleListItem>();
				if (component.VerifyVisibility())
				{
					return false;
				}
			}
		}
		return true;
	}

	private void CheckEndOfList(Int32 itemNumber, Boolean isTop)
	{
		this.draggablePanel.restrictWithinPanel = false;
		if (itemNumber < this.visibleSize + this.Column * 4 && isTop)
		{
			this.draggablePanel.restrictWithinPanel = true;
			this.draggablePanel.UpdateScrollbars(true);
		}
		else if (itemNumber > this.dataList.Count - (this.visibleSize + this.Column * 4) && !isTop)
		{
			this.draggablePanel.restrictWithinPanel = true;
			this.draggablePanel.UpdateScrollbars(true);
		}
	}

	public IEnumerator ItemIsInvisible(Int32 itemNumber)
	{
		if (this.isUpdatingList)
			yield return null;
		Transform item = this.itemsPool[itemNumber];
		Int32 itemDataIndex = 0;
		this.isUpdatingList = true;
		if (this.dataList.Count > this.poolSize)
		{
			if (item.tag.Equals(ITEM_TAG))
				itemDataIndex = item.GetComponent<RecycleListItem>().ItemDataIndex;
			Int32 indexToCheck;
			if (this.dataTracker.ContainsKey(itemDataIndex + 1))
			{
				RecycleListItem nextItem = this.itemsPool[this.dataTracker[itemDataIndex + 1]].GetComponent<RecycleListItem>();
				if (nextItem != null && nextItem.VerifyVisibility())
				{
					indexToCheck = itemDataIndex - this.extraBuffer / 2;
					if (this.dataTracker.ContainsKey(indexToCheck))
					{
						for (Int32 i = indexToCheck; i >= 0; i--)
						{
							if (!this.dataTracker.ContainsKey(i))
							{
								this.scrollCursor = itemDataIndex - this.Column;
								this.CheckEndOfList(this.scrollCursor, false);
								break;
							}
							nextItem = this.itemsPool[this.dataTracker[i]].GetComponent<RecycleListItem>();
							if (nextItem != null && !nextItem.VerifyVisibility())
							{
								item = this.itemsPool[this.dataTracker[i]];
								if (i + this.poolSize < this.dataList.Count)
								{
									this.PrepareListItemWithIndex(item, i + this.poolSize, i);
									this.CheckEndOfList(this.scrollCursor, false);
								}
							}
						}
					}
				}
			}
			if (this.dataTracker.ContainsKey(itemDataIndex - 1))
			{
				RecycleListItem previousItem = this.itemsPool[this.dataTracker[itemDataIndex - 1]].GetComponent<RecycleListItem>();
				if (previousItem != null && previousItem.VerifyVisibility())
				{
					indexToCheck = itemDataIndex + this.extraBuffer / 2;
					if (this.dataTracker.ContainsKey(indexToCheck))
					{
						for (Int32 i = indexToCheck; i < this.dataList.Count; i++)
						{
							if (!this.dataTracker.ContainsKey(i))
							{
								this.scrollCursor = itemDataIndex + this.Column;
								this.CheckEndOfList(this.scrollCursor, true);
								break;
							}
							previousItem = this.itemsPool[this.dataTracker[i]].GetComponent<RecycleListItem>();
							if (previousItem != null && !previousItem.VerifyVisibility())
							{
								item = this.itemsPool[this.dataTracker[i]];
								if (i - this.poolSize > -1 && i < this.dataList.Count)
								{
									this.PrepareListItemWithIndex(item, i - this.poolSize, i);
									this.CheckEndOfList(this.scrollCursor, true);
								}
							}
						}
					}
				}
			}
		}
		else
		{
			this.CheckEndOfList(this.scrollCursor + this.Column, false);
			this.CheckEndOfList(this.scrollCursor - this.Column, true);
		}
		this.isUpdatingList = false;
		yield break;
	}

	public void itemIsPressed(Int32 itemDataIndex, Boolean isDown)
	{
		if (this.OnRecycleListItemPress != null)
			this.OnRecycleListItemPress(this.itemsPool[this.dataTracker[itemDataIndex]].gameObject, isDown);
	}

	public void itemClicked(Int32 itemDataIndex)
	{
		if (this.OnRecycleListItemClick != null)
			this.OnRecycleListItemClick(this.itemsPool[this.dataTracker[itemDataIndex]].gameObject);
	}

	public void itemClicked(GameObject go)
	{
		if (this.OnRecycleListItemClick != null)
			this.OnRecycleListItemClick(go);
	}

	public void itemHasChanged(GameObject go)
	{
		RecycleListItem component = go.GetComponent<RecycleListItem>();
		if (component)
		{
			this.activeNumber = component.ItemDataIndex;
		}
		foreach (Transform transform in this.itemsPool)
		{
			UIKeyNavigation component2 = transform.gameObject.GetComponent<UIKeyNavigation>();
			RecycleListItem component3 = transform.gameObject.GetComponent<RecycleListItem>();
			if (component2)
			{
				component2.startsSelected = (this.activeNumber == component3.ItemDataIndex);
			}
		}
	}

	private Transform GetItemFromPool(Int32 i)
	{
		if (i >= 0 && i < this.poolSize)
		{
			this.itemsPool[i].gameObject.SetActive(true);
			return this.itemsPool[i];
		}
		throw new NullReferenceException();
	}

	private void ScrollToPoolObj(RecycleListItem item)
	{
		Vector3 shiftRequired = item.Panel.DistanceToFullVisibleArea(item.VisionCheckWidget);
	}

	private void RefreshPool()
	{
		this.extraBuffer = this.Column < 3 ? 8 : this.Column * 4;
		this.visibleSize = (Int32)(Math.Round(this.panel.baseClipRegion.w) / this.cellHeight * this.Column);
		this.poolSize = this.visibleSize + this.extraBuffer;
		for (Int32 i = 0; i < this.itemsPool.Count; i++)
			UnityEngine.Object.Destroy(this.itemsPool[i].gameObject);
		this.itemsPool.Clear();
		for (Int32 i = 0; i < this.poolSize; i++)
		{
			Transform transform = UnityEngine.Object.Instantiate<Transform>(this.itemPrefab);
			transform.gameObject.SetActive(false);
			transform.GetComponent<RecycleListItem>().ItemNumber = i;
			transform.name = $"Item {i}";
			transform.parent = this.table.transform;
			this.itemsPool.Add(transform);
		}
	}

	private const String ITEM_TAG = "Item List";

	public Action<Transform, ListDataTypeBase, Int32, Boolean> PopulateListItemWithData;

	public Transform itemPrefab;

	public UITable table;
	public UIScrollView draggablePanel;
	public UIPanel panel;
	public ScrollButton ScrollButton;
	private SnapDragScrollView snapDragPanel;
	public Transform scrollIndicator;

	private Int32 scrollCursor;
	public Single cellHeight = 94f;
	private Int32 poolSize = 6;
	private Int32 visibleSize;

	private List<Transform> itemsPool = new List<Transform>();
	private Int32 extraBuffer = 8;

	private Int32 startNumber;
	private Int32 activeNumber;

	private Dictionary<Int32, Int32> dataTracker = new Dictionary<Int32, Int32>();
	private List<ListDataTypeBase> dataList = new List<ListDataTypeBase>();

	private Boolean isUpdatingList;

	public delegate void RecycleListItemPress(GameObject item, Boolean isDown);
	public delegate void RecycleListItemClick(GameObject item);
}
