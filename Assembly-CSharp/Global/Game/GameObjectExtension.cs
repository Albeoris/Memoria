using System;
using UnityEngine;

public static class GameObjectExtension
{
	public static GameObject GetChild(this GameObject value, Int32 childIndex)
	{
		if (value == (UnityEngine.Object)null)
		{
			return (GameObject)null;
		}
		Transform child = value.transform.GetChild(childIndex);
		return (!child) ? null : child.gameObject;
	}

	public static GameObject GetParent(this GameObject value)
	{
		return value.transform.parent.gameObject;
	}

	public static GameObject FindChild(this GameObject value, String name)
	{
		return value.transform.FindChild(name).gameObject;
	}
}
