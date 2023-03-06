using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria.Scenes
{
	public class ControlToggle
	{
		public readonly UISprite Sprite;
		public readonly UILabel Label;

		public Boolean IsEnabled
		{
			get => Label.color != FF9TextTool.Gray;
			set => Label.color = value ? FF9TextTool.White : FF9TextTool.Gray;
		}

		public Boolean IsToggled
		{
			get => Sprite.spriteName != "skill_stone_gem_slot";
			set
			{
				Sprite.atlas = value ? FF9UIDataTool.IconAtlas : FF9UIDataTool.BlueAtlas;
				Sprite.spriteName = value ? "skill_stone_gem" : "skill_stone_gem_slot";
			}
		}

		public ControlToggle(ControlPanel control, Int32 panelIndex, Action<Boolean> toggleAction)
		{
			UIWidget panel = control.GetPanel(panelIndex);
			Sprite = control.CreateUIElementForPanel<UISprite>(panel);
			Label = control.CreateUIElementForPanel<UILabel>(panel);
			NGUITools.AddWidgetCollider(Sprite.gameObject);
			UIEventListener eventListener = UIEventListener.Get(Sprite.gameObject);
			eventListener.onClick += go =>
			{
				if (!IsEnabled)
					return;
				IsToggled = !IsToggled;
				toggleAction(IsToggled);
			};
		}
	}
}
