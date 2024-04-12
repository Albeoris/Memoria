using Memoria.Scenes;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleHUD : UIScene
{
	private partial class UI
	{
		internal sealed class PanelTarget : GOWidget
		{
			public readonly GOTable<GONavigationButton> Players;
			public readonly GOTable<GONavigationButton> Enemies;
			public readonly GONavigationButton[] AllTargets;
			public readonly ButtonPair Buttons;
			public readonly GOWidgetButton PreventArea;
			public readonly CaptionBackground Captions;

			public PanelTarget(BattleHUD scene, GameObject obj)
				: base(obj)
			{
				Players = new GOTable<GONavigationButton>(obj.GetChild(0));
				Enemies = new GOTable<GONavigationButton>(obj.GetChild(1));
				Buttons = new ButtonPair(obj.GetChild(2));
				PreventArea = new GOWidgetButton(obj.GetChild(3));
				Captions = new CaptionBackground(obj.GetChild(4));

				foreach (GONavigationButton button in Players.Entries)
				{
					button.EventListener.Click += scene.onClick;
					button.EventListener.Navigate += scene.OnTargetNavigate;
				}

				foreach (GONavigationButton button in Enemies.Entries)
				{
					button.EventListener.Click += scene.onClick;
					button.EventListener.Navigate += scene.OnTargetNavigate;
				}

				Int32 index = 0;
				AllTargets = new GONavigationButton[Players.Count + Enemies.Count];
				foreach (GONavigationButton button in Players.Entries)
					AllTargets[index++] = button;
				foreach (GONavigationButton button in Enemies.Entries)
					AllTargets[index++] = button;
			}

			public void ActivateButtons(Boolean value)
			{
				Buttons.Enemy.IsActive = value;
				Buttons.Player.IsActive = value;
			}

			public IEnumerable<GONavigationButton> EnumerateTargets()
			{
				foreach (GONavigationButton target in Players.Entries)
					yield return target;
				foreach (GONavigationButton target in Enemies.Entries)
					yield return target;
			}

			internal sealed class ButtonPair : GOBase
			{
				public readonly GOWidgetButton Player;
				public readonly GOWidgetButton Enemy;

				public ButtonPair(GameObject obj)
					: base(obj)
				{
					Player = new GOWidgetButton(obj.GetChild(0));
					Enemy = new GOWidgetButton(obj.GetChild(1));
				}
			}

			internal sealed class CaptionBackground : GOThinSpriteBackground
			{
				public readonly GOLocalizableLabel Caption1;
				public readonly GOLocalizableLabel Caption2;

				public CaptionBackground(GameObject obj)
					: base(obj)
				{
					Caption1 = new GOLocalizableLabel(obj.GetChild(2));
					Caption2 = new GOLocalizableLabel(obj.GetChild(3));
				}
			}
		}
	}
}
