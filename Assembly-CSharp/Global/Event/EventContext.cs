using System;

public class EventContext
{
	public EventContext()
	{
		this.mapvar = new Byte[80];
		this.objbuf = new Int32[1280];
		this.objlist = new ObjList[32];
		for (Int32 i = 0; i < 32; i++)
		{
			this.objlist[i] = new ObjList();
		}
		this.partyUID = new Byte[EventContext.partyUIDSize];
		this.watch = new Watch[16];
		for (Int32 j = 0; j < 16; j++)
		{
			this.watch[j] = new Watch();
		}
	}

	public void copy(EventContext ec)
	{
		for (Int32 i = 0; i < 80; i++)
		{
			this.mapvar[i] = ec.mapvar[i];
		}
		for (Int32 j = 0; j < 1280; j++)
		{
			this.objbuf[j] = ec.objbuf[j];
		}
		for (Int32 k = 0; k < 32; k++)
		{
		}
		for (Int32 l = 0; l < 32; l++)
		{
			this.objlist[l] = (ObjList)null;
			if (ec.objlist[l] != null)
			{
				this.objlist[l] = new ObjList();
				this.objlist[l].copy(ec.objlist[l]);
				if (ec.objlist[l].obj != null)
				{
				}
			}
		}
		for (Int32 m = 0; m < 32; m++)
		{
			ObjList next = ec.objlist[m].next;
			if (next != null && next.obj != null)
			{
				for (Int32 n = 0; n < 32; n++)
				{
					if (ec.objlist[n].obj != null && next.obj.uid == ec.objlist[n].obj.uid)
					{
						this.objlist[m].next = this.objlist[n];
					}
				}
			}
		}
		this.activeObj = (ObjList)null;
		if (ec.activeObj != null)
		{
			for (Int32 num = 0; num < 32; num++)
			{
				if (this.objlist[num].obj != null && ec.activeObj.obj != null && this.objlist[num].obj.uid == ec.activeObj.obj.uid)
				{
					this.activeObj = this.objlist[num];
				}
			}
			PersistenSingleton<EventEngine>.Instance.printObjsInObjList(this.activeObj);
		}
		this.activeObjTail = (ObjList)null;
		if (ec.activeObjTail != null)
		{
			for (Int32 num2 = 0; num2 < 32; num2++)
			{
				if (this.objlist[num2].obj != null && ec.activeObjTail.obj != null && this.objlist[num2].obj.uid == ec.activeObjTail.obj.uid)
				{
					this.activeObjTail = this.objlist[num2];
				}
			}
		}
		this.freeObj = (ObjList)null;
		if (ec.freeObj != null)
		{
			for (Int32 num3 = 0; num3 < 32; num3++)
			{
				if (this.objlist[num3].obj == null)
				{
					this.objlist[num3].next = this.freeObj;
					this.freeObj = this.objlist[num3];
				}
			}
		}
		for (Int32 num4 = 0; num4 < EventContext.partyUIDSize; num4++)
		{
			this.partyUID[num4] = ec.partyUID[num4];
		}
		this.twist_a = ec.twist_a;
		this.twist_d = ec.twist_d;
		this.usercontrol = ec.usercontrol;
		this.controlUID = ec.controlUID;
		this.inited = ec.inited;
		this.encratio = ec.encratio;
		this.start = ec.start;
		this.lastmap = ec.lastmap;
		this.pad0 = ec.pad0;
		this.dashinh = ec.dashinh;
		this.pad1 = ec.pad1;
		this.pad2 = ec.pad2;
		this.pad3 = ec.pad3;
		this.idletimer = ec.idletimer;
		this.pad4 = ec.pad4;
		this.partyObjTail = (ObjList)null;
		if (ec.partyObjTail != null)
		{
			for (Int32 num5 = 0; num5 < 32; num5++)
			{
				if (this.objlist[num5].obj != null && ec.partyObjTail.obj != null && this.objlist[num5].obj.uid == ec.partyObjTail.obj.uid)
				{
					this.partyObjTail = this.objlist[num5];
				}
			}
		}
	}

	public void printObjList()
	{
		for (Int32 i = 0; i < 32; i++)
		{
			ObjList objList = this.objlist[i];
		}
	}

	public Byte[] mapvar;

	public Int32[] objbuf;

	public ObjList[] objlist;

	public ObjList activeObj;

	public ObjList activeObjTail;

	public ObjList freeObj;

	public Byte[] partyUID;

	public Int16 twist_a;

	public Int16 twist_d;

	public Byte usercontrol;

	public Byte controlUID;

	public Byte inited;

	public Byte encratio;

	public Int32 start;

	public UInt16 lastmap;

	public UInt16 pad0;

	public Byte dashinh;

	public Byte pad1;

	public Byte pad2;

	public Byte pad3;

	public ObjList partyObjTail;

	public Int16 idletimer;

	public Int16 pad4;

	public Watch[] watch;

	public Watch watchP;

	private static Int32 partyUIDSize = 4;
}
