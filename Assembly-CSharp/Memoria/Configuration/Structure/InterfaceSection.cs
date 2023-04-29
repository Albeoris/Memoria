using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class InterfaceSection : IniSection
        {
            public readonly IniValue<Boolean> PSXBattleMenu;
            public readonly IniValue<Int32> BattleColumnCount;
            public readonly IniValue<Int32> BattleMenuPosX;
            public readonly IniValue<Int32> BattleMenuPosY;
            public readonly IniValue<Int32> BattleMenuWidth;
            public readonly IniValue<Int32> BattleMenuHeight;
            public readonly IniValue<Int32> BattleDetailPosX;
            public readonly IniValue<Int32> BattleDetailPosY;
            public readonly IniValue<Int32> BattleDetailWidth;
            public readonly IniValue<Int32> BattleDetailHeight;
            public readonly IniValue<String> BattleDamageTextFormat;
            public readonly IniValue<String> BattleRestoreTextFormat;
            public readonly IniValue<String> BattleMPDamageTextFormat;
            public readonly IniValue<String> BattleMPRestoreTextFormat;

            public InterfaceSection() : base(nameof(InterfaceSection), false)
            {
                PSXBattleMenu = BindBoolean(nameof(PSXBattleMenu), false);
                BattleColumnCount = BindInt32(nameof(BattleColumnCount), 1);
                BattleMenuPosX = BindInt32(nameof(BattleMenuPosX), -400);
                BattleMenuPosY = BindInt32(nameof(BattleMenuPosY), -362);
                BattleMenuWidth = BindInt32(nameof(BattleMenuWidth), 630);
                BattleMenuHeight = BindInt32(nameof(BattleMenuHeight), 236);
                BattleDetailPosX = BindInt32(nameof(BattleDetailPosX), 345);
                BattleDetailPosY = BindInt32(nameof(BattleDetailPosY), -380);
                BattleDetailWidth = BindInt32(nameof(BattleDetailWidth), 796);
                BattleDetailHeight = BindInt32(nameof(BattleDetailHeight), 230);
                BattleDamageTextFormat = BindString(nameof(BattleDamageTextFormat), String.Empty);
                BattleRestoreTextFormat = BindString(nameof(BattleRestoreTextFormat), String.Empty);
                BattleMPDamageTextFormat = BindString(nameof(BattleMPDamageTextFormat), String.Empty);
                BattleMPRestoreTextFormat = BindString(nameof(BattleMPRestoreTextFormat), String.Empty);
            }
        }
    }
}