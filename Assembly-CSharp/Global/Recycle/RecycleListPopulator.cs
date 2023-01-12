using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecycleListPopulator : MonoBehaviour
{
	public event RecycleListPopulator.RecycleListItemPress OnRecycleListItemPress;

	public event RecycleListPopulator.RecycleListItemClick OnRecycleListItemClick;

	public Int32 ItemCount
	{
		get
		{
			return this.dataList.Count;
		}
	}

	public Int32 VisibleItemCount
	{
		get
		{
			return this.visibleSize;
		}
	}

	public Int32 MaxItemCount
	{
		get
		{
			return this.dataList.Count<ListDataTypeBase>();
		}
	}

	public Int32 Column
	{
		get
		{
			return (Int32)((this.table.columns <= 0) ? 1 : this.table.columns);
		}
	}

	public Dictionary<Int32, Int32> DataTracker
	{
		get
		{
			return this.dataTracker;
		}
	}

	public List<Transform> ItemsPool
	{
		get
		{
			return this.itemsPool;
		}
	}

	public Int32 ActiveNumber
	{
		set
		{
			this.activeNumber = value;
		}
	}

	private void Awake()
	{
		if (this.itemPrefab == (UnityEngine.Object)null)
		{
			global::Debug.LogError("InfiniteListPopulator:: itemPrefab is not assigned");
		}
		else if (!this.itemPrefab.tag.Equals("Item List"))
		{
			global::Debug.LogError("InfiniteListPopulator:: itemPrefab tag should be Item List");
		}
		this.snapDragPanel = base.gameObject.GetComponent<SnapDragScrollView>();
	}

	private void Update()
	{
		if (this.scrollIndicator != (UnityEngine.Object)null)
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
			Single y = (Single)(-(Single)this.scrollCursor) / (Single)this.dataList.Count * this.panel.baseClipRegion.w;
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

	public void SetOriginalData(List<ListDataTypeBase> inDataList)
	{
		if (this.dataList.Count == inDataList.Count)
		{
			this.dataList = new List<ListDataTypeBase>(inDataList);
			this.UpdateTableViewImp();
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
		{
		}
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
		Int32 num = this.dataTracker[this.activeNumber];
		if (this.dataTracker.ContainsKey(this.activeNumber + this.Column))
		{
			Int32 index = this.dataTracker[this.activeNumber + this.Column];
			Transform transform = this.itemsPool[index];
			RecycleListItem component = this.itemsPool[index].GetComponent<RecycleListItem>();
			if (component.VerifyVisibility())
			{
				ButtonGroupState.ActiveButton = transform.gameObject;
				return;
			}
		}
		if (this.dataTracker.ContainsKey(this.activeNumber - this.Column))
		{
			Int32 index2 = this.dataTracker[this.activeNumber - this.Column];
			Transform transform2 = this.itemsPool[index2];
			RecycleListItem component2 = this.itemsPool[index2].GetComponent<RecycleListItem>();
			if (component2.VerifyVisibility())
			{
				ButtonGroupState.ActiveButton = transform2.gameObject;
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
		if (this.dataList.Count < this.visibleSize)
		{
			this.startNumber = 0;
		}
		else if (this.activeNumber < this.visibleSize)
		{
			this.startNumber = 0;
		}
		else
		{
			this.startNumber = this.activeNumber - (this.visibleSize - this.Column);
		}
		this.scrollCursor = this.startNumber;
		Int32 num = this.poolSize - this.extraBuffer;
		Int32 num2 = this.startNumber - this.startNumber / this.poolSize * this.poolSize;
		Int32 num3;
		Int32 num4;
		if (this.dataList.Count <= this.poolSize)
		{
			num3 = this.startNumber;
			num4 = this.dataList.Count - (this.startNumber + this.visibleSize);
		}
		else if (this.startNumber < this.extraBuffer / 2)
		{
			num3 = this.startNumber;
			num4 = this.extraBuffer - num3;
		}
		else if (this.dataList.Count > this.poolSize && this.dataList.Count - (this.startNumber + this.visibleSize) < this.extraBuffer / 2)
		{
			num4 = this.dataList.Count - (this.startNumber + this.visibleSize);
			num3 = this.extraBuffer - num4;
		}
		else
		{
			num4 = (num3 = this.extraBuffer / 2);
		}
		Int32 i = 0;
		Int32 num5 = 0;
		Int32 num6 = 0;
		Int32 num7 = this.startNumber;
		while (i < this.poolSize)
		{
			if (num3 > 0)
			{
				num7 = this.startNumber - num3;
				Int32 num8 = num2 - num3;
				if (num8 < 0)
				{
					num8 += this.poolSize;
				}
				if (num7 >= this.dataList.Count)
				{
					break;
				}
				Transform itemFromPool = this.GetItemFromPool(num8);
				this.InitListItemWithIndex(itemFromPool, num7, num8);
				num3--;
			}
			else if (num5 < num)
			{
				num7 = this.startNumber + num5;
				Int32 num8 = num2 + num5;
				if (num8 >= this.poolSize)
				{
					num8 -= this.poolSize;
				}
				if (num7 >= this.dataList.Count)
				{
					break;
				}
				Transform itemFromPool2 = this.GetItemFromPool(num8);
				this.InitListItemWithIndex(itemFromPool2, num7, num8);
				num5++;
			}
			else
			{
				if (num6 >= num4)
				{
					break;
				}
				num7 = this.startNumber + num + num6;
				Int32 num8 = num2 + num + num6;
				if (num8 >= this.poolSize)
				{
					num8 -= this.poolSize;
				}
				if (num7 >= this.dataList.Count)
				{
					break;
				}
				Transform itemFromPool3 = this.GetItemFromPool(num8);
				this.InitListItemWithIndex(itemFromPool3, num7, num8);
				num6++;
			}
			i++;
		}
		base.Invoke("RepositionList", 0.02f);
	}

	private void UpdateTableViewImp()
	{
		foreach (KeyValuePair<Int32, Int32> kvp in this.dataTracker)
		{
			Transform poolObj = this.itemsPool[kvp.Value];
			if (poolObj.GetComponent<UIKeyNavigation>())
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

	private void InitListItemWithIndex(Transform item, Int32 dataIndex, Int32 poolIndex)
	{
		RecycleListItem component = item.GetComponent<RecycleListItem>();
		component.ItemDataIndex = dataIndex;
		component.ListPopulator = this;
		component.Panel = this.panel;
		ScrollItemKeyNavigation component2 = item.GetComponent<ScrollItemKeyNavigation>();
		if (component2)
		{
			component2.ScrollButton = this.ScrollButton;
			component2.ScrollPanel = this.panel;
		}
		UIKeyNavigation component3;
		if (ButtonGroupState.ActiveGroup == ItemUI.ItemArrangeGroupButton)
		{
			component3 = item.GetChild(1).GetComponent<UIKeyNavigation>();
		}
		else
		{
			component3 = item.GetComponent<UIKeyNavigation>();
		}
		if (component3)
		{
			component3.startsSelected = (this.activeNumber == dataIndex);
		}
		ButtonGroupState component4 = item.GetComponent<ButtonGroupState>();
		if (component4)
		{
			ButtonGroupState.RemoveActiveStateOnGroup(item.gameObject, component4.GroupName);
		}
		item.name = "Item " + dataIndex;
		this.PopulateListItemWithData(item, this.dataList[dataIndex], dataIndex, true);
		this.dataTracker.Add(this.itemsPool[poolIndex].GetComponent<RecycleListItem>().ItemDataIndex, this.itemsPool[poolIndex].GetComponent<RecycleListItem>().ItemNumber);
	}

	private void PrepareListItemWithIndex(Transform item, Int32 newIndex, Int32 oldIndex)
	{
		if (newIndex < oldIndex)
		{
			item.localPosition += new Vector3(0f, (Single)(this.poolSize / this.Column) * this.cellHeight, 0f);
		}
		else
		{
			item.localPosition -= new Vector3(0f, (Single)(this.poolSize / this.Column) * this.cellHeight, 0f);
		}
		item.GetComponent<RecycleListItem>().ItemDataIndex = newIndex;
		UIKeyNavigation component = item.GetComponent<UIKeyNavigation>();
		if (component)
		{
			component.startsSelected = (this.activeNumber == newIndex);
		}
		if (oldIndex == this.activeNumber)
		{
			ButtonGroupState component2 = item.GetComponent<ButtonGroupState>();
			ButtonGroupState.RemoveActiveStateOnGroup(item.gameObject, component2.GroupName);
		}
		if (newIndex == this.activeNumber)
		{
			ButtonGroupState component3 = item.GetComponent<ButtonGroupState>();
			ButtonGroupState.HoldActiveStateOnGroup(item.gameObject, component3.GroupName);
		}
		item.name = "Item " + newIndex;
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
			return;
		}
		if (itemNumber > this.dataList.Count - (this.visibleSize + this.Column * 4) && !isTop)
		{
			this.draggablePanel.restrictWithinPanel = true;
			this.draggablePanel.UpdateScrollbars(true);
			return;
		}
	}

	public IEnumerator ItemIsInvisible(Int32 itemNumber)
	{
		if (this.isUpdatingList)
		{
			yield return null;
		}
		Int32 itemDataIndex = 0;
		Transform item = this.itemsPool[itemNumber];
		RecycleListItem infItem = (RecycleListItem)null;
		this.isUpdatingList = true;
		if (this.dataList.Count > this.poolSize)
		{
			if (item.tag.Equals("Item List"))
			{
				itemDataIndex = item.GetComponent<RecycleListItem>().ItemDataIndex;
			}
			Int32 indexToCheck = 0;
			if (this.dataTracker.ContainsKey(itemDataIndex + 1))
			{
				infItem = this.itemsPool[this.dataTracker[itemDataIndex + 1]].GetComponent<RecycleListItem>();
				if (infItem != (UnityEngine.Object)null && infItem.VerifyVisibility())
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
							infItem = this.itemsPool[this.dataTracker[i]].GetComponent<RecycleListItem>();
							if (infItem != (UnityEngine.Object)null && !infItem.VerifyVisibility())
							{
								item = this.itemsPool[this.dataTracker[i]];
								if (i + this.poolSize < this.dataList.Count && i > -1)
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
				infItem = this.itemsPool[this.dataTracker[itemDataIndex - 1]].GetComponent<RecycleListItem>();
				if (infItem != (UnityEngine.Object)null && infItem.VerifyVisibility())
				{
					indexToCheck = itemDataIndex + this.extraBuffer / 2;
					if (this.dataTracker.ContainsKey(indexToCheck))
					{
						for (Int32 j = indexToCheck; j < this.dataList.Count; j++)
						{
							if (!this.dataTracker.ContainsKey(j))
							{
								this.scrollCursor = itemDataIndex + this.Column;
								this.CheckEndOfList(this.scrollCursor, true);
								break;
							}
							infItem = this.itemsPool[this.dataTracker[j]].GetComponent<RecycleListItem>();
							if (infItem != (UnityEngine.Object)null && !infItem.VerifyVisibility())
							{
								item = this.itemsPool[this.dataTracker[j]];
								if (j - this.poolSize > -1 && j < this.dataList.Count)
								{
									this.PrepareListItemWithIndex(item, j - this.poolSize, j);
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
		{
			this.OnRecycleListItemPress(this.itemsPool[this.dataTracker[itemDataIndex]].gameObject, isDown);
		}
	}

	public void itemClicked(Int32 itemDataIndex)
	{
		if (this.OnRecycleListItemClick != null)
		{
			this.OnRecycleListItemClick(this.itemsPool[this.dataTracker[itemDataIndex]].gameObject);
		}
	}

	public void itemClicked(GameObject go)
	{
		if (this.OnRecycleListItemClick != null)
		{
			this.OnRecycleListItemClick(go);
		}
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
		return (Transform)null;
	}

	private void RefreshPool()
	{
		this.visibleSize = (Int32)((Single)Mathf.RoundToInt(this.panel.baseClipRegion.w) / this.cellHeight * (Single)this.Column);
		this.poolSize = this.visibleSize + this.extraBuffer;
		for (Int32 i = 0; i < this.itemsPool.Count; i++)
		{
			UnityEngine.Object.Destroy(this.itemsPool[i].gameObject);
		}
		this.itemsPool.Clear();
		for (Int32 j = 0; j < this.poolSize; j++)
		{
			Transform transform = UnityEngine.Object.Instantiate<Transform>(this.itemPrefab);
			transform.gameObject.SetActive(false);
			transform.GetComponent<RecycleListItem>().ItemNumber = j;
			transform.name = "Item " + j;
			transform.parent = this.table.transform;
			this.itemsPool.Add(transform);
		}
	}

	private const String listItemTag = "Item List";

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
