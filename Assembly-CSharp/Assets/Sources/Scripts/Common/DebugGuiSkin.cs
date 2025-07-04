using System;
using UnityEngine;

namespace Assets.Sources.Scripts.Common
{
    internal class DebugGuiSkin
    {
        private DebugGuiSkin()
        {
            this.InitializeGuiSkin();
            this.CalculateGuiAreaRect();
            this.CalculateFullscreenRect();
        }

        public static void ApplySkin()
        {
            GUI.skin = DebugGuiSkin.instance.myskin;
        }

        public static Rect GetUIAreaRect()
        {
            return DebugGuiSkin.instance.guiAreaRect;
        }

        public static Rect GetFullscreenRect()
        {
            return DebugGuiSkin.instance.fullscreenRect;
        }

        private void InitializeGuiSkin()
        {
            String path = "EmbeddedAsset/GUISkins/OrangeGuiSkinWin";
            this.myskin = (Resources.Load(path, typeof(GUISkin)) as GUISkin);
        }

        private void CalculateGuiAreaRect()
        {
            this.guiAreaRect = new Rect((Single)(Screen.width / 20), (Single)(Screen.height / 20), (Single)(Screen.width - Screen.width / 10), (Single)(Screen.height - Screen.height / 10));
        }

        private void CalculateFullscreenRect()
        {
            this.fullscreenRect = new Rect(0f, 0f, (Single)Screen.width, (Single)Screen.height);
        }

        private static DebugGuiSkin instance = new DebugGuiSkin();

        private GUISkin myskin;

        private Rect guiAreaRect;

        private Rect fullscreenRect;

        public static readonly Font font = Font.CreateDynamicFontFromOSFont(["Segoe UI", "Helvetica", " Geneva", "Verdana", "Arial"], 14);
    }
}
