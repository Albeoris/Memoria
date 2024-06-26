using System;

public class Seq : Obj
{
	public Seq()
	{
	}

	public Seq(Int32 sid, Int32 uid) : base(sid, uid, EventEngine.sizeOfObj, 0)
	{
		base.cid = 1;
	}

	public void copy(Seq seq)
	{
	}
}
