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
        Single posX = 0f;
        Single posY = 0f;
        Int32 rowCount = this.columns <= 0 ? 1 : children.Count / this.columns + 1;
        Int32 colCount = this.columns <= 0 ? children.Count : this.columns;
        Bounds[,] allItemBounds = new Bounds[rowCount, colCount];
        Bounds[] colBounds = new Bounds[colCount];
        Bounds[] rowBounds = new Bounds[rowCount];
        Int32 rowIndex = 0;
        Int32 colIndex = 0;
        for (Int32 i = 0; i < children.Count; i++)
        {
            Transform item = children[i];
            Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(item, !this.hideInactive);
            Vector3 localScale = item.localScale;
            bounds.min = Vector3.Scale(bounds.min, localScale);
            bounds.max = Vector3.Scale(bounds.max, localScale);
            allItemBounds[rowIndex, colIndex] = bounds;
            colBounds[colIndex].Encapsulate(bounds);
            rowBounds[rowIndex].Encapsulate(bounds);
            if (++colIndex >= this.columns && this.columns > 0)
            {
                colIndex = 0;
                rowIndex++;
            }
        }
        colIndex = 0;
        rowIndex = 0;
        Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.cellAlignment);
        for (Int32 i = 0; i < children.Count; i++)
        {
            Transform item = children[i];
            Bounds childBound = allItemBounds[rowIndex, colIndex];
            Bounds colBound = colBounds[colIndex];
            Bounds rowBound = rowBounds[rowIndex];
            Vector3 itemPos = item.localPosition;
            itemPos.x = posX + childBound.extents.x - childBound.center.x;
            itemPos.x -= Mathf.Lerp(0f, childBound.max.x - childBound.min.x - colBound.max.x + colBound.min.x, pivotOffset.x) - this.padding.x;
            if (this.direction == UITable.Direction.Down)
            {
                itemPos.y = -posY - childBound.extents.y - childBound.center.y;
                itemPos.y += Mathf.Lerp(childBound.max.y - childBound.min.y - rowBound.max.y + rowBound.min.y, 0f, pivotOffset.y) - this.padding.y;
            }
            else
            {
                itemPos.y = posY + childBound.extents.y - childBound.center.y;
                itemPos.y -= Mathf.Lerp(0f, childBound.max.y - childBound.min.y - rowBound.max.y + rowBound.min.y, pivotOffset.y) - this.padding.y;
            }
            posX += colBound.size.x + this.padding.x * 2f;
            item.localPosition = itemPos;
            if (++colIndex >= this.columns && this.columns > 0)
            {
                colIndex = 0;
                rowIndex++;
                posX = 0f;
                posY += rowBound.size.y + this.padding.y * 2f;
            }
        }
        if (this.pivot != UIWidget.Pivot.TopLeft)
        {
            pivotOffset = NGUIMath.GetPivotOffset(this.pivot);
            Bounds fullBound = NGUIMath.CalculateRelativeWidgetBounds(base.transform);
            Single pivotOffsetX = Mathf.Lerp(0f, fullBound.size.x, pivotOffset.x);
            Single pivotOffsetY = Mathf.Lerp(-fullBound.size.y, 0f, pivotOffset.y);
            for (Int32 i = 0; i < base.transform.childCount; i++)
            {
                Transform child = base.transform.GetChild(i);
                SpringPosition spring = child.GetComponent<SpringPosition>();
                if (spring != null)
                {
                    spring.target.x -= pivotOffsetX;
                    spring.target.y -= pivotOffsetY;
                }
                else
                {
                    Vector3 childPos = child.localPosition;
                    childPos.x -= pivotOffsetX;
                    childPos.y -= pivotOffsetY;
                    child.localPosition = childPos;
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
