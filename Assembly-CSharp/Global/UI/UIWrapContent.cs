using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Wrap Content")]
public class UIWrapContent : MonoBehaviour
{
    protected virtual void Start()
    {
        this.SortBasedOnScrollMovement();
        this.WrapContent();
        if (this.mScroll != (UnityEngine.Object)null)
        {
            this.mScroll.GetComponent<UIPanel>().onClipMove = new UIPanel.OnClippingMoved(this.OnMove);
        }
        this.mFirstTime = false;
    }

    protected virtual void OnMove(UIPanel panel)
    {
        this.WrapContent();
    }

    [ContextMenu("Sort Based on Scroll Movement")]
    public void SortBasedOnScrollMovement()
    {
        if (!this.CacheScrollView())
        {
            return;
        }
        this.mChildren.Clear();
        for (Int32 i = 0; i < this.mTrans.childCount; i++)
        {
            this.mChildren.Add(this.mTrans.GetChild(i));
        }
        if (this.mHorizontal)
        {
            this.mChildren.Sort(new Comparison<Transform>(UIGrid.SortHorizontal));
        }
        else
        {
            this.mChildren.Sort(new Comparison<Transform>(UIGrid.SortVertical));
        }
        this.ResetChildPositions();
    }

    [ContextMenu("Sort Alphabetically")]
    public void SortAlphabetically()
    {
        if (!this.CacheScrollView())
        {
            return;
        }
        this.mChildren.Clear();
        for (Int32 i = 0; i < this.mTrans.childCount; i++)
        {
            this.mChildren.Add(this.mTrans.GetChild(i));
        }
        this.mChildren.Sort(new Comparison<Transform>(UIGrid.SortByName));
        this.ResetChildPositions();
    }

    protected Boolean CacheScrollView()
    {
        this.mTrans = base.transform;
        this.mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
        this.mScroll = this.mPanel.GetComponent<UIScrollView>();
        if (this.mScroll == (UnityEngine.Object)null)
        {
            return false;
        }
        if (this.mScroll.movement == UIScrollView.Movement.Horizontal)
        {
            this.mHorizontal = true;
        }
        else
        {
            if (this.mScroll.movement != UIScrollView.Movement.Vertical)
            {
                return false;
            }
            this.mHorizontal = false;
        }
        return true;
    }

    private void ResetChildPositions()
    {
        Int32 i = 0;
        Int32 count = this.mChildren.Count;
        while (i < count)
        {
            Transform transform = this.mChildren[i];
            transform.localPosition = ((!this.mHorizontal) ? new Vector3(0f, (Single)(-(Single)i * this.itemSize), 0f) : new Vector3((Single)(i * this.itemSize), 0f, 0f));
            this.UpdateItem(transform, i);
            i++;
        }
    }

    public void WrapContent()
    {
        Single num = (Single)(this.itemSize * this.mChildren.Count) * 0.5f;
        Vector3[] worldCorners = this.mPanel.worldCorners;
        for (Int32 i = 0; i < 4; i++)
        {
            Vector3 vector = worldCorners[i];
            vector = this.mTrans.InverseTransformPoint(vector);
            worldCorners[i] = vector;
        }
        Vector3 vector2 = Vector3.Lerp(worldCorners[0], worldCorners[2], 0.5f);
        Boolean flag = true;
        Single num2 = num * 2f;
        if (this.mHorizontal)
        {
            Single num3 = worldCorners[0].x - (Single)this.itemSize;
            Single num4 = worldCorners[2].x + (Single)this.itemSize;
            Int32 j = 0;
            Int32 count = this.mChildren.Count;
            while (j < count)
            {
                Transform transform = this.mChildren[j];
                Single num5 = transform.localPosition.x - vector2.x;
                if (num5 < -num)
                {
                    Vector3 localPosition = transform.localPosition;
                    localPosition.x += num2;
                    num5 = localPosition.x - vector2.x;
                    Int32 num6 = Mathf.RoundToInt(localPosition.x / (Single)this.itemSize);
                    if (this.minIndex == this.maxIndex || (this.minIndex <= num6 && num6 <= this.maxIndex))
                    {
                        transform.localPosition = localPosition;
                        this.UpdateItem(transform, j);
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else if (num5 > num)
                {
                    Vector3 localPosition2 = transform.localPosition;
                    localPosition2.x -= num2;
                    num5 = localPosition2.x - vector2.x;
                    Int32 num7 = Mathf.RoundToInt(localPosition2.x / (Single)this.itemSize);
                    if (this.minIndex == this.maxIndex || (this.minIndex <= num7 && num7 <= this.maxIndex))
                    {
                        transform.localPosition = localPosition2;
                        this.UpdateItem(transform, j);
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else if (this.mFirstTime)
                {
                    this.UpdateItem(transform, j);
                }
                if (this.cullContent)
                {
                    num5 += this.mPanel.clipOffset.x - this.mTrans.localPosition.x;
                    if (!UICamera.IsPressed(transform.gameObject))
                    {
                        NGUITools.SetActive(transform.gameObject, num5 > num3 && num5 < num4, false);
                    }
                }
                j++;
            }
        }
        else
        {
            Single num8 = worldCorners[0].y - (Single)this.itemSize;
            Single num9 = worldCorners[2].y + (Single)this.itemSize;
            Int32 k = 0;
            Int32 count2 = this.mChildren.Count;
            while (k < count2)
            {
                Transform transform2 = this.mChildren[k];
                Single num10 = transform2.localPosition.y - vector2.y;
                if (num10 < -num)
                {
                    Vector3 localPosition3 = transform2.localPosition;
                    localPosition3.y += num2;
                    num10 = localPosition3.y - vector2.y;
                    Int32 num11 = Mathf.RoundToInt(localPosition3.y / (Single)this.itemSize);
                    if (this.minIndex == this.maxIndex || (this.minIndex <= num11 && num11 <= this.maxIndex))
                    {
                        transform2.localPosition = localPosition3;
                        this.UpdateItem(transform2, k);
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else if (num10 > num)
                {
                    Vector3 localPosition4 = transform2.localPosition;
                    localPosition4.y -= num2;
                    num10 = localPosition4.y - vector2.y;
                    Int32 num12 = Mathf.RoundToInt(localPosition4.y / (Single)this.itemSize);
                    if (this.minIndex == this.maxIndex || (this.minIndex <= num12 && num12 <= this.maxIndex))
                    {
                        transform2.localPosition = localPosition4;
                        this.UpdateItem(transform2, k);
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else if (this.mFirstTime)
                {
                    this.UpdateItem(transform2, k);
                }
                if (this.cullContent)
                {
                    num10 += this.mPanel.clipOffset.y - this.mTrans.localPosition.y;
                    if (!UICamera.IsPressed(transform2.gameObject))
                    {
                        NGUITools.SetActive(transform2.gameObject, num10 > num8 && num10 < num9, false);
                    }
                }
                k++;
            }
        }
        this.mScroll.restrictWithinPanel = !flag;
    }

    private void OnValidate()
    {
        if (this.maxIndex < this.minIndex)
        {
            this.maxIndex = this.minIndex;
        }
        if (this.minIndex > this.maxIndex)
        {
            this.maxIndex = this.minIndex;
        }
    }

    protected virtual void UpdateItem(Transform item, Int32 index)
    {
        if (this.onInitializeItem != null)
        {
            Int32 realIndex = (Int32)((this.mScroll.movement != UIScrollView.Movement.Vertical) ? Mathf.RoundToInt(item.localPosition.x / (Single)this.itemSize) : Mathf.RoundToInt(item.localPosition.y / (Single)this.itemSize));
            this.onInitializeItem(item.gameObject, index, realIndex);
        }
    }

    public Int32 itemSize = 100;

    public Boolean cullContent = true;

    public Int32 minIndex;

    public Int32 maxIndex;

    public UIWrapContent.OnInitializeItem onInitializeItem;

    private Transform mTrans;

    private UIPanel mPanel;

    private UIScrollView mScroll;

    private Boolean mHorizontal;

    private Boolean mFirstTime = true;

    private List<Transform> mChildren = new List<Transform>();

    public delegate void OnInitializeItem(GameObject go, Int32 wrapIndex, Int32 realIndex);
}
