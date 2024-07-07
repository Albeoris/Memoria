using System;
using UnityEngine;

namespace Memoria.Scenes
{
    internal sealed class GONavigationButton : GOWidgetButton
    {
        public readonly UIKeyNavigation KeyNavigation;
        public readonly ButtonGroupState ButtonGroup;

        public readonly GOLabel Name;
        public readonly GOSprite Highlight;
        public readonly GOThinBackground Background;

        public GONavigationButton(GameObject obj)
            : base(obj)
        {
            KeyNavigation = obj.GetExactComponent<UIKeyNavigation>();
            ButtonGroup = obj.GetExactComponent<ButtonGroupState>();

            Name = new GOLabel(obj.GetChild(0));
            Highlight = new GOSprite(obj.GetChild(1));
            Background = new GOThinBackground(obj.GetChild(2));
            Name.Label.overflowMethod = UILabel.Overflow.ResizeFreely;
        }

        public void SetLabelText(String value)
        {
            Name.Label.text = value;
        }

        public void SetLabelColor(Color value)
        {
            Name.Label.color = value;
        }
    }
}
