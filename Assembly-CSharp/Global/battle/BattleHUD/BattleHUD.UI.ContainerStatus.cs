using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using Memoria;
using Memoria.Data;
using Memoria.Scenes;
using Memoria.Scenes.BattleHUD;
using UnityEngine;
using Object = System.Object;

public partial class BattleHUD : UIScene
{
    private partial class UI
    {
        internal sealed class ContainerStatus : GOBase
        {
            public readonly PanelDetail<ValueWidget> HP;
            public readonly PanelDetail<ValueWidget> MP;
            public readonly PanelDetail<IconsWidget> GoodStatus;
            public readonly PanelDetail<IconsWidget> BadStatus;

            public ContainerStatus(BattleHUD battleHUD, GameObject obj)
                : base(obj)
            {
                // new SOLogger().Do(obj);
                HP = new PanelDetail<ValueWidget>(obj.GetChild(0));
                MP = new PanelDetail<ValueWidget>(obj.GetChild(1));
                GoodStatus = new PanelDetail<IconsWidget>(obj.GetChild(2));
                BadStatus = new PanelDetail<IconsWidget>(obj.GetChild(3));
            }

            internal sealed class PanelDetail<T> : GOWidget where T : GOBase
            {
                public readonly GOArray<T> Array;
                public readonly CaptionBackground<GOLocalizableLabel> Caption;

                public PanelDetail(GameObject obj)
                    : base(obj)
                {
                    Array = new GOArray<T>(obj.GetChild(0));
                    Caption = new CaptionBackground<GOLocalizableLabel>(obj.GetChild(1));
                }

                internal sealed class CaptionBackground<T1> : GOWidget where T1 : GOBase
                {
                    public readonly T1 Content;
                    public readonly GOSprite Body;
                    public readonly GOSprite TopBorder;

                    public CaptionBackground(GameObject obj)
                        : base(obj)
                    {
                        Content = Create<T1>(obj.GetChild(0));
                        Body = new GOSprite(obj.GetChild(1));
                        TopBorder = new GOSprite(obj.GetChild(2));
                    }
                }
            }

            internal sealed class ValueWidget : GOWidget
            {
                public readonly GOLabel Value;
                public readonly GOLabel Slash;
                public readonly GOLabel MaxValue;
                public readonly GOThinBackground Background;

                public ValueWidget(GameObject obj)
                    : base(obj)
                {
                    Value = new GOLabel(obj.GetChild(0));
                    Slash = new GOLabel(obj.GetChild(1));
                    MaxValue = new GOLabel(obj.GetChild(2));
                    Background = new GOThinBackground(obj.GetChild(3));
                }

                public void SetColor(Color value)
                {
                    Value.SetColor(value);
                    Slash.SetColor(value);
                    MaxValue.SetColor(value);
                }
            }

            internal sealed class IconsWidget : GOWidget
            {
                public readonly GOTable<GOSprite> Icons;
                public readonly GOThinBackground Background;

                public IconsWidget(GameObject obj)
                    : base(obj)
                {
                    Icons = new GOTable<GOSprite>(obj.GetChild(0));
                    Background = new GOThinBackground(obj.GetChild(1));
                }
            }

            public void SetActive(Boolean value)
            {
                HP.IsActive = value;
                MP.IsActive = value;
                GoodStatus.IsActive = value;
                BadStatus.IsActive = value;
            }

            public void DisplayStatusRealtime(List<Int32> playerIds)
            {
                if (HP.IsActive)
                    DisplayData(playerIds, DisplayHp);
                else if (MP.IsActive)
                    DisplayData(playerIds, DisplayMp);
                else if (BadStatus.IsActive)
                    DisplayData(playerIds, DisplayBadStatus);
                else if (GoodStatus.IsActive)
                    DisplayData(playerIds, DisplayGoodStatus);
            }

            private static void DisplayData(List<Int32> playerIds, Action<Int32, BattleUnit> action)
            {
                foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
                {
                    if (!unit.IsPlayer)
                        continue;

                    Int32 playerIndex = unit.GetIndex();
                    Int32 playerId = playerIds.IndexOf(playerIndex);
                    if (playerId < 0)
                        continue;

                    action(playerId, unit);
                }
            }

            private void DisplayHp(Int32 playerId, BattleUnit bd)
            {
                ValueWidget hud = HP.Array[playerId];
                hud.IsActive = true;
                hud.Value.SetText(bd.CurrentHp.ToString());
                hud.MaxValue.SetText(bd.MaximumHp.ToString());
                switch (CheckHPState(bd))
                {
                    case ParameterStatus.Empty:
                        hud.SetColor(FF9TextTool.Red);
                        return;
                    case ParameterStatus.Critical:
                        hud.SetColor(FF9TextTool.Yellow);
                        return;
                    default:
                        hud.SetColor(FF9TextTool.White);
                        return;
                }
            }

            private void DisplayMp(Int32 playerId, BattleUnit bd)
            {
                ValueWidget numberSubModeHud = MP.Array[playerId];
                numberSubModeHud.IsActive = true;
                numberSubModeHud.Value.SetText(bd.CurrentMp.ToString());
                numberSubModeHud.MaxValue.SetText(bd.MaximumMp.ToString());
                numberSubModeHud.SetColor(CheckMPState(bd) == ParameterStatus.Empty ? FF9TextTool.Yellow : FF9TextTool.White);
            }

            private void DisplayBadStatus(Int32 playerId, BattleUnit bd)
            {
                IconsWidget statusHud = BadStatus.Array[playerId];
                DisplayStatusHud(bd, statusHud, DebuffIconNames);
            }

            private void DisplayGoodStatus(Int32 playerId, BattleUnit bd)
            {
                IconsWidget statusHud = GoodStatus.Array[playerId];
                DisplayStatusHud(bd, statusHud, BuffIconNames);
            }

            private static void DisplayStatusHud(BattleUnit bd, IconsWidget statusHud, Dictionary<BattleStatus, String> iconDic)
            {
                statusHud.IsActive = true;
                foreach (GOSprite uiWidget in statusHud.Icons.Entries)
                    uiWidget.Sprite.alpha = 0.0f;

                Int32 index = 0;
                foreach (KeyValuePair<BattleStatus, String> current in iconDic)
                {
                    if (!bd.IsUnderAnyStatus(current.Key))
                        continue;

                    GOSprite spriteObj = statusHud.Icons[index];
                    spriteObj.Sprite.alpha = 1f;
                    spriteObj.Sprite.spriteName = current.Value;

                    if (++index > statusHud.Icons.Count)
                        break;
                }
            }
        }
    }
}