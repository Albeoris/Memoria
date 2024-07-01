using UnityEngine;

namespace Memoria.Scenes
{
    internal class GOWidget : GOBase
    {
        public readonly UIWidget Widget;

        public GOWidget(GameObject obj)
            : base(obj)
        {
            Widget = obj.GetExactComponent<UIWidget>();
        }
    }
}
