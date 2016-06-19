using UnityEngine;

namespace Memoria
{
    internal sealed class GONavigationButton : GOWidgetButton
    {
        public readonly UIKeyNavigation KeyNavigation;
        public readonly ButtonGroupState GroupState;

        public readonly GOLabel Label;
        public readonly GOSprite Highlight;
        public readonly GOThinBackground Background;

        public GONavigationButton(GameObject obj)
            : base(obj)
        {
            KeyNavigation = obj.GetExactComponent<UIKeyNavigation>();
            GroupState = obj.GetExactComponent<ButtonGroupState>();

            Label = new GOLabel(obj.GetChild(0));
            Highlight = new GOSprite(obj.GetChild(1));
            Background = new GOThinBackground(obj.GetChild(2));
        }
    }
}