using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using UnityEngine;

public class QuadMistDebugMenu : MonoBehaviour
{
	private void Start()
	{
		FF9StateSystem.Common.FF9.miniGameArg = 0;
		this.stageStr = FF9StateSystem.Common.FF9.miniGameArg.ToString();
		this.cardStr = "0";
		QuadMistDatabase.LoadData();
	}

	private void OnGUI()
	{
		Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
		DebugGuiSkin.ApplySkin();
		GUILayout.BeginArea(fullscreenRect);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		if (GUILayout.Button("Back", new GUILayoutOption[0]))
		{
			SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[]
		{
			GUILayout.Width(fullscreenRect.width * 2f / 3f),
			GUILayout.Height(fullscreenRect.height * 3f / 4f)
		});
		GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
		GUILayout.Label("Please select game stage", new GUILayoutOption[0]);
		GUILayout.Label("( Valid stage are between 0 AND 127 )", new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("-10", new GUILayoutOption[0]))
		{
			Int32 num = Convert.ToInt32(this.stageStr);
			if (num >= 10)
			{
				num -= 10;
			}
			else
			{
				num = 0;
			}
			this.stageStr = String.Empty + num;
		}
		if (GUILayout.Button("-1", new GUILayoutOption[0]))
		{
			Int32 num2 = Convert.ToInt32(this.stageStr);
			if (num2 > 0)
			{
				num2--;
			}
			this.stageStr = String.Empty + num2;
		}
		this.stageStr = GUILayout.TextField(this.stageStr, new GUILayoutOption[0]);
		if (GUILayout.Button("+1", new GUILayoutOption[0]))
		{
			Int32 num3 = Convert.ToInt32(this.stageStr);
			if (num3 < 127)
			{
				num3++;
			}
			this.stageStr = String.Empty + num3;
		}
		if (GUILayout.Button("+10", new GUILayoutOption[0]))
		{
			Int32 num4 = Convert.ToInt32(this.stageStr);
			if (num4 < 118)
			{
				num4 += 10;
			}
			else
			{
				num4 = 127;
			}
			this.stageStr = String.Empty + num4;
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("PLAY!", new GUILayoutOption[0]))
		{
			Int32 num5 = Convert.ToInt32(this.stageStr);
			FF9StateSystem.Common.FF9.miniGameArg = (UInt16)num5;
			SceneDirector.Replace("QuadMist", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.Label("----------------------------------------", new GUILayoutOption[0]);
		if (GUILayout.Button("Clear all cards", new GUILayoutOption[0]))
		{
			QuadMistDatabase.LoadData();
			List<QuadMistCard> cardList = QuadMistDatabase.GetCardList();
			cardList.Clear();
			QuadMistDatabase.SetCardList(cardList);
			QuadMistDatabase.SaveData();
		}
		if (GUILayout.Button("Reset all stat", new GUILayoutOption[0]))
		{
			QuadMistDatabase.LoadData();
			QuadMistDatabase.WinCount = 0;
			QuadMistDatabase.LoseCount = 0;
			QuadMistDatabase.DrawCount = 0;
			QuadMistDatabase.SaveData();
		}
		GUILayout.Label("Game generates new 8 cards", new GUILayoutOption[0]);
		GUILayout.Label("----------------------------------------", new GUILayoutOption[0]);
		GUILayout.Label("Create/Remove card", new GUILayoutOption[0]);
		GUILayout.Label("Select card ID (Between 0-99)", new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("-10", new GUILayoutOption[0]))
		{
			Int32 num6 = Convert.ToInt32(this.cardStr);
			if (num6 >= 10)
			{
				num6 -= 10;
			}
			else
			{
				num6 = 0;
			}
			this.cardStr = String.Empty + num6;
		}
		if (GUILayout.Button("-1", new GUILayoutOption[0]))
		{
			Int32 num7 = Convert.ToInt32(this.cardStr);
			if (num7 > 0)
			{
				num7--;
			}
			this.cardStr = String.Empty + num7;
		}
		this.cardStr = GUILayout.TextField(this.cardStr, new GUILayoutOption[0]);
		if (GUILayout.Button("+1", new GUILayoutOption[0]))
		{
			Int32 num8 = Convert.ToInt32(this.cardStr);
			if (num8 < 99)
			{
				num8++;
			}
			this.cardStr = String.Empty + num8;
		}
		if (GUILayout.Button("+10", new GUILayoutOption[0]))
		{
			Int32 num9 = Convert.ToInt32(this.cardStr);
			if (num9 < 90)
			{
				num9 += 10;
			}
			else
			{
				num9 = 99;
			}
			this.cardStr = String.Empty + num9;
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Create card", new GUILayoutOption[0]))
		{
			QuadMistDatabase.LoadData();
			Int32 id = Convert.ToInt32(this.cardStr);
			List<QuadMistCard> cardList2 = QuadMistDatabase.GetCardList();
			QuadMistCard item = CardPool.CreateQuadMistCard(id);
			cardList2.Add(item);
			QuadMistDatabase.SetCardList(cardList2);
			QuadMistDatabase.SaveData();
		}
		if (GUILayout.Button("Remove card", new GUILayoutOption[0]))
		{
			QuadMistDatabase.LoadData();
			Int32 num10 = Convert.ToInt32(this.cardStr);
			List<QuadMistCard> cardList3 = QuadMistDatabase.GetCardList();
			List<QuadMistCard> list = new List<QuadMistCard>();
			foreach (QuadMistCard quadMistCard in cardList3)
			{
				if ((Int32)quadMistCard.id == num10)
				{
					list.Add(quadMistCard);
				}
			}
			foreach (QuadMistCard item2 in list)
			{
				cardList3.Remove(item2);
			}
			QuadMistDatabase.SetCardList(cardList3);
			QuadMistDatabase.SaveData();
		}
		GUILayout.Label("(Remove all All card at filled ID)", new GUILayoutOption[0]);
		GUILayout.Label("----------------------------------------", new GUILayoutOption[0]);
		GUILayout.Label("Templates", new GUILayoutOption[0]);
		if (GUILayout.Button("Create 100", new GUILayoutOption[0]))
		{
			QuadMistDatabase.LoadData();
			List<QuadMistCard> cardList4 = QuadMistDatabase.GetCardList();
			cardList4.Clear();
			for (Int32 i = 0; i < 100; i++)
			{
				QuadMistCard item3 = CardPool.CreateQuadMistCard(i);
				cardList4.Add(item3);
			}
			QuadMistDatabase.SetCardList(cardList4);
			QuadMistDatabase.SaveData();
		}
		GUILayout.Label("Create 100 cards, 1 ID 1 card", new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private String stageStr;

	private String cardStr;

	private Vector2 scrollPosition = new Vector2(0f, 0f);
}
