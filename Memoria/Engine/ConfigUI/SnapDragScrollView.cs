using System;
using System.Collections;
using Memoria;
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

[ExportedType("đïĪ7#!!!R.zĖ-!!!ó#Ļĭġį{ĜÑ§ªÝ¿Æĩ$ÞÏ7cľÊ|ÉýŃæß;4ØV=ĩ)ðuùĸËďðįĮ+!!!ĶĠ:Ĳ¡Æóàİä«iĠfÄHygĴf;Ċ²ěĲăk~pOÚ-E_Ódńńńń")]
internal class SnapDragScrollView : MonoBehaviour
{
    public ScrollButton ScrollButton;
    public int Speed;
    public int VisibleItem;
    public int MaxItem;
    public int ItemHeight;
    private float startPosY;
    private bool isScrollMove;
    private bool isStartMove;
    private UIPanel scrollViewPanel;
    private UIScrollView draggablePanel;
    private RecycleListPopulator populator;

    public float StartPostionY
    {
        set
        {
            startPosY = value;
        }
    }

    public SnapDragScrollView()
    {
        Speed = 24;
        VisibleItem = 1;
        MaxItem = 1;
    }

    private void Awake()
    {
        scrollViewPanel = gameObject.GetComponent<UIPanel>();
        draggablePanel = gameObject.GetComponent<UIScrollView>();
        populator = gameObject.GetComponent<RecycleListPopulator>();
        startPosY = scrollViewPanel.transform.localPosition.y;
    }

    private void Start()
    {
        UIScrollView component = scrollViewPanel.GetComponent<UIScrollView>();
        // ISSUE: method pointer
        UIScrollView.OnDragNotification dragNotification = (UIScrollView.OnDragNotification)Delegate.Combine(component.onStoppedMoving, (UIScrollView.OnDragNotification)SnappingScroll);
        component.onStoppedMoving = dragNotification;
    }

    private void Update()
    {
        if (!enabled || Mathf.Abs(draggablePanel.currentMomentum.y) <= 0.0 && Mathf.Abs(draggablePanel.currentMomentum.x) <= 0.0)
            return;
        isStartMove = true;
        if (!isStartMove)
            return;
        IEnumerator enumerator = gameObject.transform.GetChild(0).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                ScrollItemKeyNavigation component = ((Component)enumerator.Current).gameObject.GetComponent<ScrollItemKeyNavigation>();
                component.enabled = false;
                component.CheckVisibility();
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }

    private void OnEnable()
    {
        isScrollMove = false;
    }

    public void ScrollToIndex(int index)
    {
        float num1 = 0.0f;
        if (index > VisibleItem - 1)
        {
            int num2 = MaxItem - VisibleItem;
            num1 = (index + 1f - VisibleItem) / num2;
        }
        draggablePanel.verticalScrollBar.value = num1;
        SnappingScroll();
    }

    private void SnappingScroll()
    {
        if (!enabled)
            return;
        isStartMove = false;
        float f = scrollViewPanel.transform.localPosition.y - startPosY;
        float num1 = scrollViewPanel.baseClipRegion.w;
        if (Mathf.RoundToInt(f) % Mathf.RoundToInt(ItemHeight) != 0)
        {
            if (isScrollMove)
            {
                OnSnapFinish();
                DestroyImmediate(SpringPanel.current);
            }
            isScrollMove = true;
            int num2 = (int)(f / (double)ItemHeight) * ItemHeight;
            if (f % (double)ItemHeight > ItemHeight / 2)
                num2 += ItemHeight;
            // ISSUE: method pointer
            SpringPanel.Begin(scrollViewPanel.cachedGameObject, new Vector3(scrollViewPanel.transform.localPosition.x, startPosY + num2, scrollViewPanel.transform.localPosition.z), Speed).onFinished = OnSnapFinish;
        }
        else
            OnSnapFinish();
    }

    private void OnSnapFinish()
    {
        isScrollMove = false;
        ScrollButton.CheckScrollPosition();
        IEnumerator enumerator = transform.GetChild(0).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                GameObject gameObject = ((Component)enumerator.Current).gameObject;
                if (gameObject.activeSelf)
                {
                    ScrollItemKeyNavigation component = gameObject.GetComponent<ScrollItemKeyNavigation>();
                    component.enabled = true;
                    component.CheckVisibility();
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
        ButtonGroupState.UpdateActiveButton();
    }
}