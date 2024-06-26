using System;
using System.Collections.Generic;
using UnityEngine;

public static class NGUIExtension
{
	public static void SetKeyNevigation(List<GameObject> buttonList)
	{
		for (Int32 i = 0; i < buttonList.Count; i++)
		{
			GameObject gameObject = buttonList[i];
			UIKeyNavigation component = gameObject.GetComponent<UIKeyNavigation>();
			if (!(component == (UnityEngine.Object)null))
			{
				if (i + 1 < buttonList.Count)
				{
					component.onDown = buttonList[i + 1];
				}
				else
				{
					component.onDown = buttonList[0];
				}
				if (i - 1 > -1)
				{
					component.onUp = buttonList[i - 1];
				}
				else
				{
					component.onUp = buttonList[buttonList.Count - 1];
				}
			}
		}
	}
}
