using System;

public class MasterPointer
{
	public MasterPointer()
	{
		this.start = new Obj();
		this.end = new Obj();
	}

	public void copy(MasterPointer mp)
	{
		this.start.copy(mp.start);
		this.end.copy(mp.end);
		if (this.next != null)
		{
			this.next.copy(mp.next);
		}
	}

	public Obj start;

	public Obj end;

	public MasterPointer next;
}
