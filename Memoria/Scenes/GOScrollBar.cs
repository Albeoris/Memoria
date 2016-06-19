using UnityEngine;

namespace Memoria
{
    internal sealed class GOScrollBar : GOBase
    {
        public readonly UIScrollBar Bar;

        public GOScrollBar(GameObject obj)
            : base(obj)
        {
            Bar = obj.GetExactComponent<UIScrollBar>();
        }
    }
}