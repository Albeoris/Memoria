using UnityEngine;

namespace Memoria.Scenes
{
    internal sealed class GOScrollButton : GOBase
    {
        public readonly UIPanel Panel;
        public readonly ScrollButton ScrollButton;

        public readonly GOSpriteButton Up;
        public readonly GOSpriteButton Down;
        public readonly GOScrollBar ScrollBar;
        public readonly GOWidgetButton IgnoreAreaUp;
        public readonly GOWidgetButton IgnoreAreaDown;

        public GOScrollButton(GameObject obj)
            : base(obj)
        {
            Panel = obj.GetExactComponent<UIPanel>();
            ScrollButton = obj.GetExactComponent<ScrollButton>();

            Up = new GOSpriteButton(obj.GetChild(0));
            Down = new GOSpriteButton(obj.GetChild(1));
            ScrollBar = new GOScrollBar(obj.GetChild(2));
            IgnoreAreaUp = new GOWidgetButton(obj.GetChild(3));
            IgnoreAreaDown = new GOWidgetButton(obj.GetChild(4));
        }
    }
}
