using System;
using System.Collections.Generic;
using System.Linq;

public class PSX_OT
{
	public unsafe Boolean IsMatch(UInt32* ot, UInt32 n)
	{
		return this.otAddr == ot & n == this.otSize;
	}

	public unsafe Boolean IsInOt(UInt32* ot)
	{
		return ot >= this.otAddr && ot < this.otAddr + this.otSize;
	}

	public unsafe void ClearOTag(UInt32* ot, UInt32 n)
	{
		this.otAddr = ot;
		this.otSize = n;
		this.OT_array.Clear();
		this.primPtrList.Clear();
	}

	public unsafe void DrawOTag(UInt32* ot)
	{
	}

    public unsafe void DrawOTagR(UInt32* ot)
    {
        if (this.OT_array.Count == 0)
        {
            return;
        }
        this.OT_array.Reverse<KeyValuePair<UInt64, LinkedList<UInt64>>>();
        foreach (KeyValuePair<UInt64, LinkedList<UInt64>> keyValuePair in this.OT_array)
        {
            Int32 otz = (Int32)((Int64)keyValuePair.Key - (Int64)ot);
            LinkedList<UInt64> value = keyValuePair.Value;
            foreach (UInt64 num in value)
            {
                PSX_PrimWithOTZ psx_PrimWithOTZ = new PSX_PrimWithOTZ();
                psx_PrimWithOTZ.otz = otz;
                psx_PrimWithOTZ.primAddr = (UInt64*)num;
                this.primPtrList.Add(psx_PrimWithOTZ);
            }
        }
    }

    public unsafe void AddPrim(void* ot, void* p)
	{
		if (ot < (void*)this.otAddr || ot >= (void*)(this.otAddr + this.otSize))
		{
		}
        UInt64 key = (UInt64)ot;
        LinkedList<UInt64> linkedList;
		if (this.OT_array.ContainsKey(key))
		{
			linkedList = this.OT_array[key];
		}
		else
		{
			linkedList = new LinkedList<UInt64>();
			this.OT_array[key] = linkedList;
		}
        linkedList.AddFirst((UInt64)p);
    }

	public unsafe UInt32* otAddr;

	public UInt32 otSize;

	public List<PSX_PrimWithOTZ> primPtrList = new List<PSX_PrimWithOTZ>();

	private SortedDictionary<UInt64, LinkedList<UInt64>> OT_array = new SortedDictionary<UInt64, LinkedList<UInt64>>();
}
