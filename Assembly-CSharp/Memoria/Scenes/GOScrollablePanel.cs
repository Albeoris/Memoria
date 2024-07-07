using System;
using UnityEngine;

namespace Memoria.Scenes
{
    internal sealed class GOScrollablePanel : GOWidget
    {
        public readonly GOScrollButton ScrollButton;
        public readonly GOSubPanel SubPanel;
        public readonly CaptionBackground Background;

        public GOScrollablePanel(GameObject obj)
            : base(obj)
        {
            ScrollButton = new GOScrollButton(obj.GetChild(0));
            SubPanel = new GOSubPanel(obj.GetChild(1));
            Background = new CaptionBackground(obj.GetChild(2));
        }

        internal sealed class CaptionBackground : GOWidget
        {
            public readonly GOSprite Body;
            public readonly GOSprite TopBorder;
            public readonly GOSprite Border;
            public readonly GOSprite Shadow = null;
            public readonly CaptionPanel Panel;

            public CaptionBackground(GameObject obj)
                : base(obj)
            {
                TopBorder = new GOSprite(obj.GetChild(0));
                Border = new GOSprite(obj.GetChild(1));
                Body = new GOSprite(obj.GetChild(2));
                if (obj.GetChild(3).GetComponent<UISprite>() != null)
                {
                    Shadow = new GOSprite(obj.GetChild(2));
                    Panel = new CaptionPanel(obj.GetChild(4));
                }
                else
                {
                    Shadow = null;
                    Panel = new CaptionPanel(obj.GetChild(3));
                }
            }
        }

        internal sealed class CaptionPanel : GOPanel
        {
            public readonly GOLocalizableLabel Name;
            public readonly GOLocalizableLabel Info = null;
            public readonly GOLocalizableLabel Name2 = null;
            public readonly GOLocalizableLabel Info2 = null;

            public CaptionPanel(GameObject obj)
                : base(obj)
            {
                Name = new GOLocalizableLabel(obj.GetChild(0));
                if (obj.transform.childCount == 1)
                    return;
                Info = new GOLocalizableLabel(obj.GetChild(1));
                if (obj.transform.childCount != 4)
                    return;
                Name2 = new GOLocalizableLabel(obj.GetChild(2));
                Info2 = new GOLocalizableLabel(obj.GetChild(3));
            }
        }
    }
}
