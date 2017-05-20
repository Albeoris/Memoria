using Memoria.Scenes;
using UnityEngine;

public partial class BattleHUD : UIScene
{
    private partial class UI
    {
        internal sealed class AllMenuPanel : GOBase
        {
            public readonly ButtonBack BackButton;
            public readonly ToggleButton AllTargetButton;
            public readonly ToggleButton AutoAttackButton;
            public readonly ButtonRun RunButton;
            public readonly ContainerStatus StatusContainer;
            public readonly PanelCommand CommandPanel;
            public readonly PanelTarget TargetPanel;
            public readonly ScrollablePanel AbilityPanel;
            public readonly ScrollablePanel ItemPanel;
            public readonly PanelParty PartyPanel;
            public readonly HUDAutoAttack AutoAttackHUD;

            private readonly BattleHUD _scene;

            public AllMenuPanel(BattleHUD scene, GameObject obj)
                : base(obj)
            {
                _scene = scene;

                BackButton = new ButtonBack(obj.GetChild(0));
                AllTargetButton = new ToggleButton(obj.GetChild(1));
                AutoAttackButton = new ToggleButton(obj.GetChild(2));
                RunButton = new ButtonRun(obj.GetChild(3));
                StatusContainer = new ContainerStatus(_scene, obj.GetChild(4));
                CommandPanel = new PanelCommand(scene, obj.GetChild(5));
                TargetPanel = new PanelTarget(scene, obj.GetChild(6));
                AbilityPanel = new ScrollablePanel(obj.GetChild(7));
                ItemPanel = new ScrollablePanel(obj.GetChild(8));
                PartyPanel = new PanelParty(scene, obj.GetChild(9));
                AutoAttackHUD = new HUDAutoAttack(obj.GetChild(10));
            }
        }
    }
}

