using UnityEngine;

namespace Memoria
{
    internal class GOWidgetButton : GOWidget
    {
        public readonly UIButton Button;
        public readonly BoxCollider BoxCollider;

        public GOWidgetButton(GameObject obj)
            : base(obj)
        {
            Button = obj.GetExactComponent<UIButton>();
            BoxCollider = obj.GetExactComponent<BoxCollider>();
        }
    }
}