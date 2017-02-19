using System;
using UnityEngine;
using Object = System.Object;

public static class TransformExtension
{
	public static Int32 GetSiblingIndex(this Transform value)
	{
		Int32 num = 0;
		foreach (Object obj in value.parent)
		{
			Transform x = (Transform)obj;
			if (x == value)
			{
				break;
			}
			num++;
		}
		return num;
	}

	public static Transform GetChildByName(this Transform root, String name)
	{
		foreach (Object obj in root)
		{
			Transform transform = (Transform)obj;
			if (transform.name == name)
			{
				return transform;
			}
			Transform childByName = transform.GetChildByName(name);
			if (childByName != (UnityEngine.Object)null)
			{
				return childByName;
			}
		}
		return (Transform)null;
	}

	public static Vector3 LocalToUIRootPoint(this UIRoot root, Transform trans)
	{
		return root.transform.InverseTransformPoint(trans.position);
	}
}
