using UnityEngine;

namespace Memoria.Scenes
{
	internal sealed class GOBlinkedLabel : GOLabel
	{
		public readonly TweenAlpha TweenAlpha;

		public GOBlinkedLabel(GameObject obj)
			: base(obj)
		{
			TweenAlpha = obj.GetExactComponent<TweenAlpha>();
		}
	}
}
