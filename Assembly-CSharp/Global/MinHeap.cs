using System;

public class MinHeap
{
    public Boolean HasNext()
    {
        return this.listHead != (WalkMeshTriangle)null;
    }

    public void Add(WalkMeshTriangle item)
    {
        if (this.listHead == null)
        {
            this.listHead = item;
        }
        else if (this.listHead.next == null && item.cost <= this.listHead.cost)
        {
            item.nextListElem = this.listHead;
            this.listHead = item;
        }
        else
        {
            WalkMeshTriangle nextListElem = this.listHead;
            while (nextListElem.nextListElem != null && nextListElem.nextListElem.cost < item.cost)
            {
                nextListElem = nextListElem.nextListElem;
            }
            item.nextListElem = nextListElem.nextListElem;
            nextListElem.nextListElem = item;
        }
    }

    public WalkMeshTriangle ExtractFirst()
    {
        WalkMeshTriangle result = this.listHead;
        this.listHead = this.listHead.nextListElem;
        return result;
    }

    private WalkMeshTriangle listHead;
}
