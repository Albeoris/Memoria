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

            public static void SaveValues()
            {
                SaveValue(Instance._interface.Name, Instance._interface.Enabled);
                SaveValue(Instance._interface.Name, Instance._interface.PSXBattleMenu);
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
        }
    }
}