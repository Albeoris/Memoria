using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Memoria.Scenes
{
    public class ControlHelp
    {
        public readonly ControlHitBox HelpOpener;
        public readonly UILabel HelpLabel;

        public ControlHelp(ControlPanel control, Int32 panelIndex, String name, String help)
        {
            UIWidget panel = control.GetPanel(panelIndex);
            Int32 helpPanelIndex = control.CreateSubPanel(name, panelIndex);
            HelpLabel = control.CreateUIElementForPanel<UILabel>(control.GetPanel(helpPanelIndex));
            HelpLabel.leftAnchor.Set(panel.transform, 0f, 50);
            HelpLabel.rightAnchor.Set(panel.transform, 1f, -50);
            HelpLabel.topAnchor.Set(panel.transform, 1f, -50);
            HelpLabel.bottomAnchor.Set(panel.transform, 0f, 50);
            HelpLabel.multiLine = true;
            HelpLabel.rawText = help;
            HelpOpener = control.AddHitBoxOption(name, () => control.SetActivePanel(true, helpPanelIndex), panelIndex);
        }
    }
}
