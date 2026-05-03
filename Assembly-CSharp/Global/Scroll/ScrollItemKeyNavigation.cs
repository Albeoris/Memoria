using Memoria;
using System;
using UnityEngine;

internal class ScrollItemKeyNavigation : MonoBehaviour
{
    public RecycleListPopulator ListPopulator => this.listPopulator;

    public Single ItemHeight
    {
        get => this.itemHeight;
        set => this.itemHeight = value;
    }

    public void OnOtherObjectSelect(GameObject go, Boolean selected)
    {
        this.OnSelect(selected);
    }

    private void OnSelect(Boolean selected)
    {
        if (!selected)
            return;
        if (!base.enabled)
            return;
        if (this.scrollView != null)
        {
            this.CheckEmptyLastRow();
            Boolean shouldScroll = this.listItem != null ? !this.listItem.VerifyFullVisibility() : !this.ScrollPanel.IsVisible(this.VisionCheckWidget);
            if (shouldScroll && !PersistenSingleton<UIManager>.Instance.IsLoading)
            {
                Single deltaX;
                Single deltaY;
                if (this.listItem != null)
                {
                    Vector3 targetDist = this.ScrollPanel.DistanceToFullVisibleArea(this.listItem.VisionCheckWidget);
                    deltaX = this.scrollView.canMoveHorizontally ? targetDist.x : 0f;
                    deltaY = (Single)Math.Round(targetDist.y / this.itemHeight) * this.itemHeight;
                }
                else
                {
                    Vector3 targetPos = -this.ScrollPanel.cachedTransform.InverseTransformPoint(base.transform.position);
                    Boolean isUp = targetPos.y <= this.ScrollPanel.cachedTransform.localPosition.y;
                    deltaX = this.scrollView.canMoveHorizontally ? targetPos.x - this.ScrollPanel.cachedTransform.localPosition.x : 0f;
                    deltaY = isUp ? -this.itemHeight : this.itemHeight;
                }
                ScrollItemKeyNavigation.IsScrollMove = true;
                SpringPanel.Begin(this.ScrollPanel.cachedGameObject, this.ScrollPanel.cachedTransform.localPosition + new Vector3(deltaX, deltaY, 0f), this.Speed, this.onScrollFinished);
            }
        }
    }

    private void OnEnable()
    {
        if (this.HidePointerOnMoving)
            ButtonGroupState.SetPointerVisibilityToGroup(true, base.gameObject.GetComponent<ButtonGroupState>().GroupName);
    }

    private void OnDisable()
    {
        if (this.HidePointerOnMoving)
            ButtonGroupState.SetPointerVisibilityToGroup(false, base.gameObject.GetComponent<ButtonGroupState>().GroupName);
    }

    private void onScrollFinished()
    {
        ScrollItemKeyNavigation.IsScrollMove = false;
        this.ScrollButton.CheckScrollPosition();
        if (this.listPopulator != null)
            this.listPopulator.CheckAllVisibilty();
        if (this.listItem != null)
            this.listItem.CheckVisibility();
    }

    public void CheckVisibility()
    {
        if (ButtonGroupState.ActiveButton == base.gameObject)
        {
            if (this.listPopulator != null)
            {
                if (!this.listItem.VerifyVisibility())
                    this.listPopulator.SwitchActiveItem();
            }
            else if (!this.ScrollPanel.IsVisible(this.VisionCheckWidget))
            {
                this.ChangeSelectItem();
            }
        }
    }

    private void CheckEmptyLastRow()
    {
        UIKeyNavigation navig = base.gameObject.GetComponent<UIKeyNavigation>();
        if (this.listPopulator != null)
        {
            Int32 rowIndex = this.listItem.ItemDataIndex / this.listPopulator.Column;
            Int32 lastRowIndex = (this.listPopulator.ItemCount - 1) / this.listPopulator.Column;
            if (rowIndex < lastRowIndex && this.listItem.ItemDataIndex + this.listPopulator.Column >= this.listPopulator.ItemCount)
                navig.onDown = this.listPopulator.ItemsPool[this.listPopulator.DataTracker[this.listPopulator.ItemCount - 1]].gameObject;
            else if (navig.onDown != null)
                navig.onDown = null;
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
            this.ID = base.gameObject.transform.GetSiblingIndex();
        this.listItem = base.gameObject.GetComponent<RecycleListItem>();
        this.scrollView = this.ScrollPanel.GetComponent<UIScrollView>();
        this.listPopulator = this.ScrollPanel.GetComponent<RecycleListPopulator>();
        this.itemHeight = base.gameObject.GetComponent<UIWidget>().height;
    }

    public static Boolean IsScrollMove;

    public Int32 ID = -1;
    public Single Speed = 24f;
    public Boolean HidePointerOnMoving;
    private Single itemHeight;

    public UIPanel ScrollPanel;
    public ScrollButton ScrollButton;
    public UIWidget VisionCheckWidget;
    private RecycleListPopulator listPopulator;
    private UIScrollView scrollView;
    private RecycleListItem listItem;
}
