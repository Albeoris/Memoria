using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class DebugUtil
{
	public static void DebugDrawMarker(Vector3 pos, Single size, Color col)
	{
		global::Debug.DrawLine(pos + new Vector3(-size, -size, 0f), pos + new Vector3(size, size, 0f), col, 2f, true);
		global::Debug.DrawLine(pos + new Vector3(-size, size, 0f), pos + new Vector3(size, -size, 0f), col, 2f, true);
	}

	public static void DrawLine(Vector3 start, Vector3 end, [Optional] Color color, float duration = 0.0f, bool depthTest = true)
	{
		PSXGPU.DrawLineG2(false, color, start, color, end);
	}
}
