using UnityEngine;

namespace Memoria
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