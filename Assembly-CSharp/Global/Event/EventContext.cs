using Memoria.Data;
using System;
using System.Collections.Generic;

public class EventContext
{
	public EventContext()
	{
		this.mapvar = new Byte[80]; // variables of type "global" in Hades Workshop, not shared between fields but shared between entries
		this.objbuf = new Int32[1280];
		this.objlist = new List<ObjList>(32); // Start with 32 possible entries to be used simultaneously
		for (Int32 i = 0; i < 32; i++)
			this.objlist.Add(new ObjList());
		this.partyUID = new Byte[EventContext.partyUIDSize];
		this.eventPartyMember = new CharacterId[EventContext.partyUIDSize];
		this.watch = new Watch[16];
		for (Int32 i = 0; i < 16; i++)
			this.watch[i] = new Watch();
	}

	public void copy(EventContext ec)
	{
		for (Int32 i = 0; i < 80; i++)
			this.mapvar[i] = ec.mapvar[i];
		for (Int32 i = 0; i < 1280; i++)
			this.objbuf[i] = ec.objbuf[i];
		for (Int32 i = 0; i < Math.Max(this.objlist.Count, ec.objlist.Count); i++)
		{
			if (i >= this.objlist.Count)
				this.objlist.Add(new ObjList());
			this.objlist[i] = null;
			if (i < ec.objlist.Count && ec.objlist[i] != null)
			{
				this.objlist[i] = new ObjList();
				this.objlist[i].copy(ec.objlist[i]);
			}
		}
		for (Int32 i = 0; i < ec.objlist.Count; i++)
		{
			ObjList next = ec.objlist[i].next;
			if (next != null && next.obj != null)
				for (Int32 j = 0; j < ec.objlist.Count; j++)
					if (ec.objlist[j].obj != null && next.obj.uid == ec.objlist[j].obj.uid)
						this.objlist[i].next = this.objlist[j];
		}
		this.activeObj = null;
		if (ec.activeObj != null)
		{
			for (Int32 i = 0; i < this.objlist.Count; i++)
				if (this.objlist[i].obj != null && ec.activeObj.obj != null && this.objlist[i].obj.uid == ec.activeObj.obj.uid)
					this.activeObj = this.objlist[i];
			PersistenSingleton<EventEngine>.Instance.printObjsInObjList(this.activeObj);
		}
		this.activeObjTail = null;
		if (ec.activeObjTail != null)
			for (Int32 i = 0; i < this.objlist.Count; i++)
				if (this.objlist[i].obj != null && ec.activeObjTail.obj != null && this.objlist[i].obj.uid == ec.activeObjTail.obj.uid)
					this.activeObjTail = this.objlist[i];
		this.freeObj = null;
		if (ec.freeObj != null)
		{
			for (Int32 i = 0; i < this.objlist.Count; i++)
			{
				if (this.objlist[i].obj == null)
				{
					this.objlist[i].next = this.freeObj;
					this.freeObj = this.objlist[i];
				}
			}
		}
		for (Int32 i = 0; i < EventContext.partyUIDSize; i++)
			this.partyUID[i] = ec.partyUID[i];
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
		this.partyObjTail = null;
		if (ec.partyObjTail != null)
			for (Int32 i = 0; i < this.objlist.Count; i++)
				if (this.objlist[i].obj != null && ec.partyObjTail.obj != null && this.objlist[i].obj.uid == ec.partyObjTail.obj.uid)
					this.partyObjTail = this.objlist[i];
	}

	public ObjList AddObjList()
	{
		ObjList objl = new ObjList();
		objl.obj = null;
		objl.next = null;
		this.objlist[this.objlist.Count - 1].next = objl;
		this.objlist.Add(objl);
		return objl;
	}

	public void printObjList()
	{
		for (Int32 i = 0; i < this.objlist.Count; i++)
		{
			ObjList objList = this.objlist[i];
		}
	}

	public Byte[] mapvar;

	public Int32[] objbuf;

	public List<ObjList> objlist;

	public ObjList activeObj;

	public ObjList activeObjTail;

	public ObjList freeObj;

	public Byte[] partyUID;
	public CharacterId[] eventPartyMember;

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
