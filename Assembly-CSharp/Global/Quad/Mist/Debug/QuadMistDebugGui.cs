using System;
using Assets.Sources.Scripts.Common;
using UnityEngine;

public class QuadMistDebugGui : MonoBehaviour
{
	public static void ShowPreGameDebugMenu()
	{
		QuadMistDebugGui.isShowPreGameDebugMenu = true;
	}

	public static void HidePreGameDebugMenu()
	{
		QuadMistDebugGui.isShowPreGameDebugMenu = false;
	}

	private void OnGUI()
	{
		Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
		DebugGuiSkin.ApplySkin();
		if (QuadMistDebugGui.isShowPreGameDebugMenu)
		{
			GUILayout.BeginArea(fullscreenRect);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical("box", new GUILayoutOption[0]);
			if (GUILayout.Button("Play game with your NOOB cards", new GUILayoutOption[0]))
			{
				QuadMistGame.OnFinishSelectCardUI(null);
				QuadMistDebugGui.isShowPreGameDebugMenu = false;
			}
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}

	private const Boolean isShowDebugGui = false;

	private static Boolean isShowPreGameDebugMenu;
}
