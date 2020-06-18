using System;
using UnityEngine;

public class EventState : MonoBehaviour
{
	public Int32 gStepCount;

	public Byte[] gEventGlobal = new Byte[2048];

	// Custom usage of "gEventGlobal":
	// The interest is that this array is saved in Game Saves (no need to touch the save format) and contains a lot of unused ranges
	// For instance, the bytes in the range 1100 to 2047 is completly unused by default
	public Byte GetAAUsageCounter(Byte sub_no)
	{
		return gEventGlobal[1100 + sub_no];
	}
	public void IncreaseAAUsageCounter(Byte sub_no)
	{
		if (gEventGlobal[1100 + sub_no] < 255)
			++gEventGlobal[1100 + sub_no];
	}
}
