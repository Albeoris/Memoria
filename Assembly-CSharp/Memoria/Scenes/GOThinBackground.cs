using UnityEngine;

namespace Memoria.Scenes
{
	internal class GOThinBackground : GOWidget
	{
		public readonly GOSprite Border;

		public GOThinBackground(GameObject obj)
			: base(obj)
		{
			Border = new GOSprite(obj.GetChild(0));
		}
	}
}
