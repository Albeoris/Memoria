using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeThisQualifier
// ReSharper disable PossibleLossOfFraction

namespace Assets.Sources.Scripts.Common
{
    internal class DebugGuiSkin
    {
        private static DebugGuiSkin instance = new DebugGuiSkin();
        private GUISkin myskin;
        private Rect guiAreaRect;
        private Rect fullscreenRect;

        private DebugGuiSkin()
        {
            this.InitializeGuiSkin();
            this.CalculateGuiAreaRect();
            this.CalculateFullscreenRect();
        }

        public static void ApplySkin()
        {
            GUI.skin = instance.myskin;
        }

        public static Rect GetUIAreaRect()
        {
            return instance.guiAreaRect;
        }

        public static Rect GetFullscreenRect()
        {
            return instance.fullscreenRect;
        }

        private void InitializeGuiSkin()
        {
            this.myskin = Resources.Load("EmbeddedAsset/GUISkins/OrangeGuiSkinWin", typeof(GUISkin)) as GUISkin;
        }

        private void CalculateGuiAreaRect()
        {
            this.guiAreaRect = new Rect(Screen.width / 20, Screen.height / 20, Screen.width - Screen.width / 10, Screen.height - Screen.height / 10);
        }

        private void CalculateFullscreenRect()
        {
            this.fullscreenRect = new Rect(0.0f, 0.0f, Screen.width, Screen.height);
        }
    }
}