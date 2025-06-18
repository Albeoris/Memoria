using System;
using UnityEngine;

namespace Memoria.Scenes
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

        private Single previous = 2f;
        private Single step = 0f;

        public void SetProgress(Single value)
        {
            if (value < previous || FF9StateSystem.Settings.IsFastForward)
            {
                ProgressBar.value = value;
                step = 0f;
            }
            else if (previous != value)
            {
                // Smooths the progress bar
                step = (value - previous) * (Single)Configuration.Graphics.BattleTPS / FPSManager.GetEstimatedFps();
            }
            ProgressBar.value = Mathf.Min(ProgressBar.value + step, value);
            previous = value;
        }
    }
}
