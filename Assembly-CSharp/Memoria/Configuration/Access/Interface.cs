using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Interface
        {
            public static Boolean IsEnabled
            {
                get => Instance._interface.Enabled;
                set => Instance._interface.Enabled.Value = value;
            }
            public static Boolean PSXBattleMenu
            {
                get => Instance._interface.PSXBattleMenu;
                set => Instance._interface.PSXBattleMenu.Value = value;
            }
            public static Boolean ScanDisplay
            {
                get => Instance._interface.ScanDisplay;
                set => Instance._interface.ScanDisplay.Value = value;
            }
            public static Boolean SynthIngredientStockDisplayed
            {
                get => Instance._interface.SynthIngredientStockDisplayed;
                set => Instance._interface.SynthIngredientStockDisplayed.Value = value;
            }
            public static Boolean DisplayPSXDiscChanges
            {
                get => Instance._interface.DisplayPSXDiscChanges;
                set => Instance._interface.DisplayPSXDiscChanges.Value = value;
            }
            public static Boolean ThickerATBBar
            {
                get => Instance._interface.ThickerATBBar;
                set => Instance._interface.ThickerATBBar.Value = value;
            }
            public static Int32 BattleRowCount
            {
                get => Math.Max(1, Instance._interface.BattleRowCount);
                set => Instance._interface.BattleRowCount.Value = value;
            }
            public static Int32 BattleColumnCount
            {
                get => Math.Max(1, Instance._interface.BattleColumnCount);
                set => Instance._interface.BattleColumnCount.Value = value;
            }
            public static Vector2 BattleMenuPos
            {
                get => new Vector2(Instance._interface.BattleMenuPosX, Instance._interface.BattleMenuPosY);
                set
                {
                    Instance._interface.BattleMenuPosX.Value = (Int32)value.x;
                    Instance._interface.BattleMenuPosY.Value = (Int32)value.y;
                }
            }
            public static Vector2 BattleMenuSize
            {
                get => new Vector2(Instance._interface.BattleMenuWidth, Instance._interface.BattleMenuHeight);
                set
                {
                    Instance._interface.BattleMenuWidth.Value = (Int32)value.x;
                    Instance._interface.BattleMenuHeight.Value = (Int32)value.y;
                }
            }
            public static Vector2 BattleDetailPos
            {
                get => new Vector2(Instance._interface.BattleDetailPosX, Instance._interface.BattleDetailPosY);
                set
                {
                    Instance._interface.BattleDetailPosX.Value = (Int32)value.x;
                    Instance._interface.BattleDetailPosY.Value = (Int32)value.y;
                }
            }
            public static Vector2 BattleDetailSize
            {
                get => new Vector2(Instance._interface.BattleDetailWidth, Instance._interface.BattleDetailHeight);
                set
                {
                    Instance._interface.BattleDetailWidth.Value = (Int32)value.x;
                    Instance._interface.BattleDetailHeight.Value = (Int32)value.y;
                }
            }
            public static Int32 MinimapPreset
            {
                get => Mathf.Clamp(Instance._interface.MinimapPreset, 0, 1);
                set => Instance._interface.MinimapPreset.Value = value;
            }
            public static Vector2 MinimapOffset
            {
                get => new Vector2(Instance._interface.MinimapOffsetX, Instance._interface.MinimapOffsetY);
                set
                {
                    Instance._interface.MinimapOffsetX.Value = (Int32)value.x;
                    Instance._interface.MinimapOffsetY.Value = (Int32)value.y;
                }
            }
            public static String BattleDamageTextFormat => Instance._interface.BattleDamageTextFormat;
            public static String BattleRestoreTextFormat => Instance._interface.BattleRestoreTextFormat;
            public static String BattleMPDamageTextFormat => Instance._interface.BattleMPDamageTextFormat;
            public static String BattleMPRestoreTextFormat => Instance._interface.BattleMPRestoreTextFormat;
            public static String BattleCommandTitleFormat => Instance._interface.BattleCommandTitleFormat;

            public static Int32 MenuItemRowCount
            {
                get => Math.Max(1, Instance._interface.MenuItemRowCount);
                set => Instance._interface.MenuItemRowCount.Value = value;
            }
            public static Int32 MenuAbilityRowCount
            {
                get => Math.Max(1, Instance._interface.MenuAbilityRowCount);
                set => Instance._interface.MenuAbilityRowCount.Value = value;
            }
            public static Int32 MenuEquipRowCount
            {
                get => Math.Max(1, Instance._interface.MenuEquipRowCount);
                set => Instance._interface.MenuEquipRowCount.Value = value;
            }
            public static Int32 MenuChocographRowCount
            {
                get => Math.Max(1, Instance._interface.MenuChocographRowCount);
                set => Instance._interface.MenuChocographRowCount.Value = value;
            }

            public static Single FadeDuration
            {
                get => Mathf.Clamp(Instance._interface.FadeDuration / 1000f, 0f, 5f);
                set => Instance._interface.FadeDuration.Value = (Int32)(value * 1000f);
            }
            public static Single TextFadeDuration
            {
                get => Mathf.Clamp(Instance._interface.TextFadeDuration / 1000f, 0f, 1f);
                set => Instance._interface.TextFadeDuration.Value = (Int32)(value * 1000f);
            }

            public static void SaveBattleValues()
            {
                //SaveValue(Instance._interface.Name, Instance._interface.Enabled);
                SaveValue(Instance._interface.Name, Instance._interface.PSXBattleMenu);
                SaveValue(Instance._interface.Name, Instance._interface.ScanDisplay);
                SaveValue(Instance._interface.Name, Instance._interface.BattleRowCount);
                SaveValue(Instance._interface.Name, Instance._interface.BattleColumnCount);
                SaveValue(Instance._interface.Name, Instance._interface.BattleMenuPosX);
                SaveValue(Instance._interface.Name, Instance._interface.BattleMenuPosY);
                SaveValue(Instance._interface.Name, Instance._interface.BattleMenuWidth);
                SaveValue(Instance._interface.Name, Instance._interface.BattleMenuHeight);
                SaveValue(Instance._interface.Name, Instance._interface.BattleDetailPosX);
                SaveValue(Instance._interface.Name, Instance._interface.BattleDetailPosY);
                SaveValue(Instance._interface.Name, Instance._interface.BattleDetailWidth);
                SaveValue(Instance._interface.Name, Instance._interface.BattleDetailHeight);
            }

            public static void SaveMenuValues()
            {
                //SaveValue(Instance._interface.Name, Instance._interface.Enabled);
                SaveValue(Instance._interface.Name, Instance._interface.MenuItemRowCount);
                SaveValue(Instance._interface.Name, Instance._interface.MenuAbilityRowCount);
                SaveValue(Instance._interface.Name, Instance._interface.MenuEquipRowCount);
                SaveValue(Instance._interface.Name, Instance._interface.MenuChocographRowCount);
                SaveValue(Instance._interface.Name, Instance._interface.SynthIngredientStockDisplayed);
            }
        }
    }
}
