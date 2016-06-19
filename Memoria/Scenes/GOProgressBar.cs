using UnityEngine;

namespace Memoria
{
    internal sealed class GOProgressBar : GOBase
    {
        public readonly UISprite Sprite;
        public readonly UIProgressBar ProgressBar;

        public readonly GOForegroundWidget Foreground;

        public GOProgressBar(GameObject obj)
            : base(obj)
        {
            Sprite = obj.GetExactComponent<UISprite>();
            ProgressBar = obj.GetExactComponent<UIProgressBar>();

            Foreground = new GOForegroundWidget(obj.GetChild(0));
        }
    }
}