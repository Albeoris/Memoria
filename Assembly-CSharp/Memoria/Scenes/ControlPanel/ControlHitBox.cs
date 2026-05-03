using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Memoria.Scenes
{
    public class ControlHitBox
    {
        public readonly UILabel Label;
        public readonly UISprite HitBoxBorder;

        public Boolean IsEnabled
        {
            get => Label.color != FF9TextTool.Gray;
            set => Label.color = value ? FF9TextTool.White : FF9TextTool.Gray;
        }

        public ControlHitBox(ControlPanel control, Int32 panelIndex, Action hitAction)
        {
            UIWidget panel = control.GetPanel(panelIndex);
            Label = control.CreateUIElementForPanel<UILabel>(panel);
            HitBoxBorder = control.InstantiateUIElement<UISprite>(out GameObject borderGo, out _, out _);
            borderGo.transform.parent = panel.transform;
            HitBoxBorder.leftAnchor.Set(Label.transform, 0f, 0);
            HitBoxBorder.rightAnchor.Set(Label.transform, 1f, 0);
            HitBoxBorder.topAnchor.Set(Label.transform, 1f, 0);
            HitBoxBorder.bottomAnchor.Set(Label.transform, 0f, 0);
            HitBoxBorder.atlas = FF9UIDataTool.GeneralAtlas;
            HitBoxBorder.spriteName = "title_button_highlight";
            NGUITools.AddWidgetCollider(Label.gameObject);
            UIEventListener eventListener = UIEventListener.Get(Label.gameObject);
            eventListener.onClick += go =>
            {
                if (IsEnabled)
                    hitAction();
            };
        }
    }
}
