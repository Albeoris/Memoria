using UnityEngine;

#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

// ReSharper disable ArrangeThisQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToConstant.Global
// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming

internal class ScrollItemKeyNavigation : MonoBehaviour
{
    public int ID;
    public float Speed;
    public bool HidePointerOnMoving;
    public static bool IsScrollMove;
    private float itemHeight;
    public UIPanel ScrollPanel;
    public ScrollButton ScrollButton;
    public UIWidget VisionCheckWidget;
    private RecycleListPopulator listPopulator;
    private UIScrollView scrollView;
    private RecycleListItem listItem;

    public RecycleListPopulator ListPopulator => listPopulator;

    public ScrollItemKeyNavigation()
    {
        ID = -1;
        Speed = 24f;
    }

    public void OnOtherObjectSelect(GameObject go, bool selected)
    {
        OnSelect(selected);
    }

    private void OnSelect(bool selected)
    {
        if (!selected || !enabled || !(scrollView != null))
            return;
        CheckEmptyLastRow();
        if (!(!listItem?.VerifyVisibility() ?? !ScrollPanel.IsVisible(VisionCheckWidget)) || PersistenSingleton<UIManager>.Instance.IsLoading)
            return;
        Vector3 pos = -ScrollPanel.cachedTransform.InverseTransformPoint(transform.position);
        if (!scrollView.canMoveHorizontally)
            pos.x = ScrollPanel.cachedTransform.localPosition.x;
        pos.y = (double)pos.y <= (double)ScrollPanel.cachedTransform.localPosition.y ? ScrollPanel.cachedTransform.localPosition.y - itemHeight : ScrollPanel.cachedTransform.localPosition.y + itemHeight;
        IsScrollMove = true;
        // ISSUE: method pointer
        SpringPanel.Begin(ScrollPanel.cachedGameObject, pos, Speed).onFinished = onScrollFinished;
    }

    private void OnEnable()
    {
        if (!HidePointerOnMoving)
            return;
        ButtonGroupState.SetPointerVisibilityToGroup(true, gameObject.GetComponent<ButtonGroupState>().GroupName);
    }

    private void OnDisable()
    {
        if (!HidePointerOnMoving)
            return;
        ButtonGroupState.SetPointerVisibilityToGroup(false, gameObject.GetComponent<ButtonGroupState>().GroupName);
    }

    private void onScrollFinished()
    {
        IsScrollMove = false;
        ScrollButton.CheckScrollPosition();
        listPopulator?.CheckAllVisibilty();
        listItem?.CheckVisibilty();
    }

    public void CheckVisibility()
    {
        if (!(ButtonGroupState.ActiveButton == gameObject))
            return;
        if (listPopulator != null)
        {
            if (gameObject.GetComponent<RecycleListItem>().VerifyVisibility())
                return;
            listPopulator.SwitchActiveItem();
        }
        else
        {
            if (ScrollPanel.IsVisible(VisionCheckWidget))
                return;
            ChangeSelectItem();
        }
    }

    private void CheckEmptyLastRow()
    {
        UIKeyNavigation component = gameObject.GetComponent<UIKeyNavigation>();
        if (!(listPopulator != null))
            return;
        if (listItem.ItemDataIndex == listPopulator.ItemCount - 2)
        {
            component.onDown = listPopulator.ItemsPool[listPopulator.DataTracker[listPopulator.ItemCount - 1]].gameObject;
        }
        else
        {
            if (!(component.onDown != null))
                return;
            gameObject.GetComponent<UIKeyNavigation>().onDown = null;
        }
    }

    private void ChangeSelectItem()
    {
        int childCount = ScrollPanel.transform.GetChild(0).childCount;
        int siblingIndex = transform.GetSiblingIndex();
        if (siblingIndex > 0)
        {
            int childIndex = siblingIndex - 1;
            GameObject child = ScrollPanel.gameObject.GetChild(0).GetChild(childIndex);
            while (childIndex > 0)
            {
                --childIndex;
                if (child.activeSelf)
                {
                    child = ScrollPanel.gameObject.GetChild(0).GetChild(childIndex);
                    if (child && ScrollPanel.IsVisible(child.GetComponent<ScrollItemKeyNavigation>().VisionCheckWidget))
                    {
                        ButtonGroupState.ActiveButton = child;
                        break;
                    }
                }
            }
        }
        if (siblingIndex >= childCount - 2)
            return;
        int childIndex1 = siblingIndex + 1;
        GameObject child1 = ScrollPanel.gameObject.GetChild(0).GetChild(childIndex1);
        while (childIndex1 < childCount - 1)
        {
            ++childIndex1;
            if (child1.activeSelf)
            {
                child1 = ScrollPanel.gameObject.GetChild(0).GetChild(childIndex1);
                if (child1 && ScrollPanel.IsVisible(child1.GetComponent<ScrollItemKeyNavigation>().VisionCheckWidget))
                {
                    ButtonGroupState.ActiveButton = child1;
                    break;
                }
            }
        }
    }

    private void Start()
    {
        if (ID == -1)
            ID = gameObject.transform.GetSiblingIndex();
        listItem = gameObject.GetComponent<RecycleListItem>();
        scrollView = ScrollPanel.GetComponent<UIScrollView>();
        listPopulator = ScrollPanel.GetComponent<RecycleListPopulator>();
        itemHeight = gameObject.GetComponent<UIWidget>().height;
    }
}
