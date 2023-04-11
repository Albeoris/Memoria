using System;
using Memoria.Prime;
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

    public static void SetX(this Transform self, Single value)
    {
        self.localPosition = self.localPosition.SetX(value);
    }

    public static void SetY(this Transform self, Single value)
    {
        self.localPosition = self.localPosition.SetY(value);
    }

    public static void SetZ(this Transform self, Single value)
    {
        self.localPosition = self.localPosition.SetZ(value);
	}

	public static void SetXY(this Transform self, Single x, Single y)
	{
		self.localPosition = self.localPosition.SetXY(x, y);
	}

	public static void SetXZ(this Transform self, Single x, Single z)
	{
		self.localPosition = self.localPosition.SetXZ(x, z);
	}

	public static void SetYZ(this Transform self, Single y, Single z)
	{
		self.localPosition = self.localPosition.SetYZ(y, z);
	}

	public static void AddX(this Transform self, Single value)
    {
        Vector3 position = self.localPosition;
        self.localPosition = position.SetX(position.x + value);
    }

    public static void AddY(this Transform self, Single value)
    {
        Vector3 position = self.localPosition;
        self.localPosition = position.SetY(position.y + value);
    }

    public static void AddZ(this Transform self, Single value)
    {
        Vector3 position = self.localPosition;
        self.localPosition = position.SetZ(position.z + value);
	}

	public static void AddXY(this Transform self, Single x, Single y)
	{
		Vector3 position = self.localPosition;
		self.localPosition = new Vector3(position.x + x, position.y + y, position.z);
	}

	public static void AddXZ(this Transform self, Single x, Single z)
	{
		Vector3 position = self.localPosition;
		self.localPosition = new Vector3(position.x + x, position.y, position.z + z);
	}

	public static void AddYZ(this Transform self, Single y, Single z)
	{
		Vector3 position = self.localPosition;
		self.localPosition = new Vector3(position.x, position.y + y, position.z + z);
	}
}
