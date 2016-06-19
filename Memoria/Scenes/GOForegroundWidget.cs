using UnityEngine;

namespace Memoria
{
    internal sealed class GOForegroundWidget : GOWidget
    {
        public readonly GOSprite Highlight;
        public readonly GOSprite Foreground;

        public GOForegroundWidget(GameObject obj)
            : base(obj)
        {
            Highlight = new GOSprite(obj.GetChild(0));
            Foreground = new GOSprite(obj.GetChild(1));
        }
    }
}