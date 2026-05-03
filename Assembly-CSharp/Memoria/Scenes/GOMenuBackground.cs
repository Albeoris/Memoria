using System;
using UnityEngine;

namespace Memoria.Scenes
{
    internal class GOMenuBackground : GOBase
    {
        public readonly UISprite Background;
        public readonly UISprite Shadow;

        public GOMenuBackground(GameObject obj, String alternateBackgroundName = null)
            : base(obj)
        {
            Background = obj.GetExactComponent<UISprite>();
            Shadow = obj.GetChild(0).GetExactComponent<UISprite>();

            // Add the possibility to use different backgrounds for different menus
            BetterList<String> spriteList = Background.atlas.GetListOfSprites();
            if (!String.IsNullOrEmpty(alternateBackgroundName) && Background.spriteName == "menu_bg" && spriteList.Contains(alternateBackgroundName))
            {
                Background.spriteName = alternateBackgroundName;
                Background.width = (Int32)UIManager.UIContentSize.x;
                Background.height = (Int32)UIManager.UIContentSize.y;
            }

            // Uncouple selection highlight and background gradient of menus
            spriteList = Shadow.atlas.GetListOfSprites();
            if (Shadow.spriteName == "dialog_hilight" && spriteList.Contains("background_gradient"))
            {
                Shadow.spriteName = "background_gradient";
                Shadow.width = (Int32)UIManager.UIContentSize.x;
                Shadow.height = (Int32)UIManager.UIContentSize.y;
            }
        }
    }
}
