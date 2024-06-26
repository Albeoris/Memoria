using System;
using System.Collections.Generic;
using UnityEngine;

public class FF9StateBattleMap
{
	public Int16 nextMapNo;

	public Byte nextMode;

	public Dictionary<BTL_DATA, GameObject> shadowArray = new Dictionary<BTL_DATA, GameObject>();

	public BBGINFO btlBGInfoPtr;

	public GameObject btlBGPtr;

	public GameObject[] btlBGObjAnim;

	public GEOTEXHEADER btlBGTexAnimPtr;

	public Byte[] evtPtr;

	public GameObject GetShadowFromUID(Int32 uid)
	{
		foreach (KeyValuePair<BTL_DATA, GameObject> pair in shadowArray)
			if (uid == pair.Key.bi.slot_no)
				return pair.Value;
		return null;
	}
}
