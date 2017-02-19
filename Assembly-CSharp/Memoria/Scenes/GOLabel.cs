using UnityEngine;

namespace Memoria.Scenes
{
    internal class GOLabel : GOBase
    {
        public readonly UILabel Label;

        public GOLabel(GameObject obj)
            : base(obj)
        {
            Label = obj.GetExactComponent<UILabel>();
        }
    }
}