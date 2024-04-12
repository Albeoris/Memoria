public class ObjList
{
	public void copy(ObjList ol)
	{
		if (ol.obj == null)
		{
			return;
		}
		if (ol.obj.cid != 0)
		{
			if (ol.obj.cid == 1)
			{
				this.obj = new Seq();
			}
			else if (ol.obj.cid == 2)
			{
				this.obj = new Obj();
			}
			else if (ol.obj.cid == 3)
			{
				this.obj = new Quad();
			}
			else if (ol.obj.cid == 4)
			{
				this.obj = new Actor();
			}
		}
		if (ol.obj != null)
		{
			this.obj.copy(ol.obj);
		}
		if (this.next != null)
		{
			this.next = (ObjList)null;
		}
	}

	public ObjList next;

	public Obj obj;
}
