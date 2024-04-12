using Memoria.Scenes;
using UnityEngine;

public partial class BattleHUD : UIScene
{
	private partial class UI
	{
		internal sealed class ButtonBack : GOWidget
		{
			public readonly UIButton Button;
			public readonly BoxCollider BoxCollider;
			public readonly OnScreenButton OnScreenButton;

			public readonly GOThinSpriteBackground Background;
			public readonly GOSprite Highlight;
			public readonly GOSprite Icon;

			public ButtonBack(GameObject obj)
				: base(obj)
			{
				Button = obj.GetExactComponent<UIButton>();
				BoxCollider = obj.GetExactComponent<BoxCollider>();
				OnScreenButton = obj.GetExactComponent<OnScreenButton>();

				Background = new GOThinSpriteBackground(obj.GetChild(0));
				Highlight = new GOSprite(obj.GetChild(1));
				Icon = new GOSprite(obj.GetChild(2));
			}
		}
	}
}
