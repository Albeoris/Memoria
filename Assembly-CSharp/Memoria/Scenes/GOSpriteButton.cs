using UnityEngine;

namespace Memoria.Scenes
{
	internal class GOSpriteButton : GOBase
	{
		public readonly UIButton Button;
		public readonly BoxCollider BoxCollider;
		public readonly UISprite Sprite;

		public GOSpriteButton(GameObject obj)
			: base(obj)
		{
			Button = obj.GetExactComponent<UIButton>();
			BoxCollider = obj.GetExactComponent<BoxCollider>();
			Sprite = obj.GetExactComponent<UISprite>();
		}
	}
}
