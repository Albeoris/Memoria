using UnityEngine;

namespace Memoria.Scenes
{
	internal class GOPanel : GOBase
	{
		public readonly UIPanel Panel;

		public GOPanel(GameObject obj)
			: base(obj)
		{
			Panel = obj.GetExactComponent<UIPanel>();
		}
	}
}
