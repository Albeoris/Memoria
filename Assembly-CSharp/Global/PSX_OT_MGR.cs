using System;
using System.Collections.Generic;

public static class PSX_OT_MGR
{
	public unsafe static void ClearOTag(UInt32* ot, UInt32 n)
	{
		PSX_OT psx_OT = (PSX_OT)null;
		for (Int32 i = 0; i < PSX_OT_MGR.otList.Count; i++)
		{
			if (PSX_OT_MGR.otList[i].IsMatch(ot, n))
			{
				psx_OT = PSX_OT_MGR.otList[i];
			}
		}
		if (psx_OT == null)
		{
			psx_OT = new PSX_OT();
			PSX_OT_MGR.otList.Add(psx_OT);
		}
		psx_OT.ClearOTag(ot, n);
	}

	public unsafe static void DrawOTagR(UInt32* ot)
	{
		PSX_OT psx_OT = (PSX_OT)null;
		for (Int32 i = 0; i < PSX_OT_MGR.otList.Count; i++)
		{
			if (PSX_OT_MGR.otList[i].IsInOt(ot))
			{
				PSX_OT_MGR.lastAccesOTindex = i;
				psx_OT = PSX_OT_MGR.otList[i];
			}
		}
		if (psx_OT == null)
		{
			Debug.LogError("OT not found");
			Debug.Break();
			return;
		}
		psx_OT.DrawOTagR(ot);
	}

	public unsafe static void AddPrim(void* ot, void* p)
	{
		PSX_OT psx_OT = (PSX_OT)null;
		for (Int32 i = 0; i < PSX_OT_MGR.otList.Count; i++)
		{
			if (PSX_OT_MGR.otList[i].IsInOt((UInt32*)ot))
			{
				psx_OT = PSX_OT_MGR.otList[i];
			}
		}
		if (psx_OT == null)
		{
			for (Int32 j = 0; j < PSX_OT_MGR.otList.Count; j++)
			{
				if (PSX_OT_MGR.otList[j].otSize == 4096u)
				{
					psx_OT = PSX_OT_MGR.otList[j];
				}
			}
		}
		psx_OT.AddPrim(ot, p);
	}

	public unsafe static List<PSX_PrimWithOTZ> GetPrimPtrList(UInt32* ot)
	{
		if (PSX_OT_MGR.lastAccesOTindex < 0 || PSX_OT_MGR.lastAccesOTindex >= PSX_OT_MGR.otList.Count)
		{
			return null;
		}
		PSX_OT psx_OT = PSX_OT_MGR.otList[PSX_OT_MGR.lastAccesOTindex];
		return psx_OT.primPtrList;
	}

	public static List<PSX_OT> otList = new List<PSX_OT>();

	public static Int32 lastAccesOTindex = -1;
}
