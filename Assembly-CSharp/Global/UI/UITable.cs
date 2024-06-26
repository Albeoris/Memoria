using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Table")]
public class UITable : UIWidgetContainer
{
    public Boolean repositionNow
    {
        set
        {
            if (value)
            {
                this.mReposition = true;
                base.enabled = true;
            }
        }
    }

    public List<Transform> GetChildList()
    {
        Transform transform = base.transform;
        List<Transform> list = new List<Transform>();
        for (Int32 i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (!this.hideInactive || (child && NGUITools.GetActive(child.gameObject)))
            {
                list.Add(child);
            }
        }
        if (this.sorting != UITable.Sorting.None)
        {
            if (this.sorting == UITable.Sorting.Alphabetic)
            {
                list.Sort(new Comparison<Transform>(UIGrid.SortByName));
            }
            else if (this.sorting == UITable.Sorting.Horizontal)
            {
                list.Sort(new Comparison<Transform>(UIGrid.SortHorizontal));
            }
            else if (this.sorting == UITable.Sorting.Vertical)
            {
                list.Sort(new Comparison<Transform>(UIGrid.SortVertical));
            }
            else if (this.onCustomSort != null)
            {
                list.Sort(this.onCustomSort);
            }
            else
            {
                this.Sort(list);
            }
        }
        return list;
    }

    protected virtual void Sort(List<Transform> list)
    {
        list.Sort(new Comparison<Transform>(UIGrid.SortByName));
    }

    protected virtual void Start()
    {
        this.Init();
        this.Reposition();
        base.enabled = false;
    }

    protected virtual void Init()
    {
        this.mInitDone = true;
        this.mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
    }

    protected virtual void LateUpdate()
    {
        if (this.mReposition)
        {
            this.Reposition();
        }
        base.enabled = false;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && NGUITools.GetActive(this))
        {
            this.Reposition();
        }
    }

    protected void RepositionVariableSize(List<Transform> children)
    {
        Single num = 0f;
        Single num2 = 0f;
        Int32 num3 = (Int32)((this.columns <= 0) ? 1 : (children.Count / this.columns + 1));
        Int32 num4 = (Int32)((this.columns <= 0) ? children.Count : this.columns);
        Bounds[,] array = new Bounds[num3, num4];
        Bounds[] array2 = new Bounds[num4];
        Bounds[] array3 = new Bounds[num3];
        Int32 num5 = 0;
        Int32 num6 = 0;
        Int32 i = 0;
        Int32 count = children.Count;
        while (i < count)
        {
            Transform transform = children[i];
            Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform, !this.hideInactive);
            Vector3 localScale = transform.localScale;
            bounds.min = Vector3.Scale(bounds.min, localScale);
            bounds.max = Vector3.Scale(bounds.max, localScale);
            array[num6, num5] = bounds;
            array2[num5].Encapsulate(bounds);
            array3[num6].Encapsulate(bounds);
            if (++num5 >= this.columns && this.columns > 0)
            {
                num5 = 0;
                num6++;
            }
            i++;
        }
        num5 = 0;
        num6 = 0;
        Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.cellAlignment);
        Int32 j = 0;
        Int32 count2 = children.Count;
        while (j < count2)
        {
            Transform transform2 = children[j];
            Bounds bounds2 = array[num6, num5];
            Bounds bounds3 = array2[num5];
            Bounds bounds4 = array3[num6];
            Vector3 localPosition = transform2.localPosition;
            localPosition.x = num + bounds2.extents.x - bounds2.center.x;
            localPosition.x -= Mathf.Lerp(0f, bounds2.max.x - bounds2.min.x - bounds3.max.x + bounds3.min.x, pivotOffset.x) - this.padding.x;
            if (this.direction == UITable.Direction.Down)
            {
                localPosition.y = -num2 - bounds2.extents.y - bounds2.center.y;
                localPosition.y += Mathf.Lerp(bounds2.max.y - bounds2.min.y - bounds4.max.y + bounds4.min.y, 0f, pivotOffset.y) - this.padding.y;
            }
            else
            {
                localPosition.y = num2 + bounds2.extents.y - bounds2.center.y;
                localPosition.y -= Mathf.Lerp(0f, bounds2.max.y - bounds2.min.y - bounds4.max.y + bounds4.min.y, pivotOffset.y) - this.padding.y;
            }
            num += bounds3.size.x + this.padding.x * 2f;
            transform2.localPosition = localPosition;
            if (++num5 >= this.columns && this.columns > 0)
            {
                num5 = 0;
                num6++;
                num = 0f;
                num2 += bounds4.size.y + this.padding.y * 2f;
            }
            j++;
        }
        if (this.pivot != UIWidget.Pivot.TopLeft)
        {
            pivotOffset = NGUIMath.GetPivotOffset(this.pivot);
            Bounds bounds5 = NGUIMath.CalculateRelativeWidgetBounds(base.transform);
            Single num7 = Mathf.Lerp(0f, bounds5.size.x, pivotOffset.x);
            Single num8 = Mathf.Lerp(-bounds5.size.y, 0f, pivotOffset.y);
            Transform transform3 = base.transform;
            for (Int32 k = 0; k < transform3.childCount; k++)
            {
                Transform child = transform3.GetChild(k);
                SpringPosition component = child.GetComponent<SpringPosition>();
                if (component != (UnityEngine.Object)null)
                {
                    SpringPosition springPosition = component;
                    springPosition.target.x = springPosition.target.x - num7;
                    SpringPosition springPosition2 = component;
                    springPosition2.target.y = springPosition2.target.y - num8;
                }
                else
                {
                    Vector3 localPosition2 = child.localPosition;
                    localPosition2.x -= num7;
                    localPosition2.y -= num8;
                    child.localPosition = localPosition2;
                }
            }
        }
    }

    [ContextMenu("Execute")]
    public virtual void Reposition()
    {
        if (Application.isPlaying && !this.mInitDone && NGUITools.GetActive(this))
        {
            this.Init();
        }
        this.mReposition = false;
        Transform transform = base.transform;
        List<Transform> childList = this.GetChildList();
        if (childList.Count > 0)
        {
            this.RepositionVariableSize(childList);
        }
        if (this.keepWithinPanel && this.mPanel != (UnityEngine.Object)null)
        {
            this.mPanel.ConstrainTargetToBounds(transform, true);
            UIScrollView component = this.mPanel.GetComponent<UIScrollView>();
            if (component != (UnityEngine.Object)null)
            {
                component.UpdateScrollbars(true);
            }
        }
        if (this.onReposition != null)
        {
            this.onReposition();
        }
    }

    public Int32 columns;

    public UITable.Direction direction;

    public UITable.Sorting sorting;

    public UIWidget.Pivot pivot;

    public UIWidget.Pivot cellAlignment;

    public Boolean hideInactive = true;

    public Boolean keepWithinPanel;

    public Vector2 padding = Vector2.zero;

    public UITable.OnReposition onReposition;

    public Comparison<Transform> onCustomSort;

    protected UIPanel mPanel;

    protected Boolean mInitDone;

    protected Boolean mReposition;

    public enum Direction
    {
        Down,
        Up
    }

    public enum Sorting
    {
        None,
        Alphabetic,
        Horizontal,
        Vertical,
        Custom
    }

    public delegate void OnReposition();
}
