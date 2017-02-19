using System;
using UnityEngine;

public static class DebugUtil
{
	public static void DebugDrawMarker(Vector3 pos, Single size, Color col)
	{
		global::Debug.DrawLine(pos + new Vector3(-size, -size, 0f), pos + new Vector3(size, size, 0f), col, 2f, true);
		global::Debug.DrawLine(pos + new Vector3(-size, size, 0f), pos + new Vector3(size, -size, 0f), col, 2f, true);
	}
}
