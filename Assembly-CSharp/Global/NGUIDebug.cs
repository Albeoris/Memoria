using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("NGUI/Internal/Debug")]
public class NGUIDebug : MonoBehaviour
{
	public static Boolean debugRaycast
	{
		get
		{
			return NGUIDebug.mRayDebug;
		}
		set
		{
			NGUIDebug.mRayDebug = value;
			if (value && Application.isPlaying)
			{
				NGUIDebug.CreateInstance();
			}
		}
	}

	public static void CreateInstance()
	{
		if (NGUIDebug.mInstance == (UnityEngine.Object)null)
		{
			GameObject gameObject = new GameObject("_NGUI Debug");
			NGUIDebug.mInstance = gameObject.AddComponent<NGUIDebug>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	[Conditional("NGUI_DEBUG")]
	private static void LogString(String text)
	{
		if (Application.isPlaying)
		{
			if (NGUIDebug.mLines.Count > 20)
			{
				NGUIDebug.mLines.RemoveAt(0);
			}
			NGUIDebug.mLines.Add(text);
			NGUIDebug.CreateInstance();
		}
		else
		{
			global::Debug.Log(text);
		}
	}

	[Conditional("NGUI_DEBUG")]
	public static void Log(params Object[] objs)
	{
		String str = String.Empty;
		for (Int32 i = 0; i < (Int32)objs.Length; i++)
		{
			if (i == 0)
			{
				str += objs[i].ToString();
			}
			else
			{
				str = str + ", " + objs[i].ToString();
			}
		}
	}

	public static void Clear()
	{
		NGUIDebug.mLines.Clear();
	}

	public static void DrawBounds(Bounds b)
	{
		Vector3 center = b.center;
		Vector3 vector = b.center - b.extents;
		Vector3 vector2 = b.center + b.extents;
		global::Debug.DrawLine(new Vector3(vector.x, vector.y, center.z), new Vector3(vector2.x, vector.y, center.z), Color.red, 0f, true);
		global::Debug.DrawLine(new Vector3(vector.x, vector.y, center.z), new Vector3(vector.x, vector2.y, center.z), Color.red, 0f, true);
		global::Debug.DrawLine(new Vector3(vector2.x, vector.y, center.z), new Vector3(vector2.x, vector2.y, center.z), Color.red, 0f, true);
		global::Debug.DrawLine(new Vector3(vector.x, vector2.y, center.z), new Vector3(vector2.x, vector2.y, center.z), Color.red, 0f, true);
	}

	private void OnGUI()
	{
		Rect position = new Rect(5f, 5f, 1000f, 18f);
		if (NGUIDebug.mRayDebug)
		{
			UICamera.ControlScheme currentScheme = UICamera.currentScheme;
			String text = "Scheme: " + currentScheme;
			GUI.color = Color.black;
			GUI.Label(position, text);
			position.y -= 1f;
			position.x -= 1f;
			GUI.color = Color.white;
			GUI.Label(position, text);
			position.y += 18f;
			position.x += 1f;
			text = "Hover: " + NGUITools.GetHierarchy(UICamera.hoveredObject).Replace("\"", String.Empty);
			GUI.color = Color.black;
			GUI.Label(position, text);
			position.y -= 1f;
			position.x -= 1f;
			GUI.color = Color.white;
			GUI.Label(position, text);
			position.y += 18f;
			position.x += 1f;
			text = "Selection: " + NGUITools.GetHierarchy(UICamera.selectedObject).Replace("\"", String.Empty);
			GUI.color = Color.black;
			GUI.Label(position, text);
			position.y -= 1f;
			position.x -= 1f;
			GUI.color = Color.white;
			GUI.Label(position, text);
			position.y += 18f;
			position.x += 1f;
			text = "Controller: " + NGUITools.GetHierarchy(UICamera.controllerNavigationObject).Replace("\"", String.Empty);
			GUI.color = Color.black;
			GUI.Label(position, text);
			position.y -= 1f;
			position.x -= 1f;
			GUI.color = Color.white;
			GUI.Label(position, text);
			position.y += 18f;
			position.x += 1f;
			text = "Active events: " + UICamera.CountInputSources();
			if (UICamera.disableController)
			{
				text += ", disabled controller";
			}
			if (UICamera.inputHasFocus)
			{
				text += ", input focus";
			}
			GUI.color = Color.black;
			GUI.Label(position, text);
			position.y -= 1f;
			position.x -= 1f;
			GUI.color = Color.white;
			GUI.Label(position, text);
			position.y += 18f;
			position.x += 1f;
		}
		Int32 i = 0;
		Int32 count = NGUIDebug.mLines.Count;
		while (i < count)
		{
			GUI.color = Color.black;
			GUI.Label(position, NGUIDebug.mLines[i]);
			position.y -= 1f;
			position.x -= 1f;
			GUI.color = Color.white;
			GUI.Label(position, NGUIDebug.mLines[i]);
			position.y += 18f;
			position.x += 1f;
			i++;
		}
	}

	private static Boolean mRayDebug = false;

	private static List<String> mLines = new List<String>();

	private static NGUIDebug mInstance = (NGUIDebug)null;
}
