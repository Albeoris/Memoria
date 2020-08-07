﻿using Assets.Sources.Scripts.UI.Common;
using FF9;
using System;
using System.Collections.Generic;
using System.IO;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Database;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;
using UnityEngine;

public partial class BattleHUD : UIScene
{
    // public const Byte BTLMES_LEVEL_FOLLOW_0 = 0;
    // public const Byte BTLMES_LEVEL_FOLLOW_1 = 1;
    // public const Byte BTLMES_LEVEL_TITLE = 1;
    // public const Byte BTLMES_LEVEL_LIBRA = 2;
    // public const Byte BTLMES_LEVEL_EVENT = 3;
    // public const Byte LIBRA_MES_NO = 10;
    // public const Byte PEEPING_MES_NO = 8;
    // public const Byte BTLMES_ATTRIBUTE_START = 0;

    public const String CommandGroupButton = "Battle.Command";
    public const String TargetGroupButton = "Battle.Target";
    public const String AbilityGroupButton = "Battle.Ability";
    public const String ItemGroupButton = "Battle.Item";

    private static readonly Byte[] BattleMessageTimeTick = new Byte[7] {54, 46, 48, 30, 24, 18, 12};
    private static readonly EntryCollection<IdMap> CmdTitleTable;
    private static readonly Int32 YINFO_ANIM_HPMP_MIN = 4;
    private static readonly Int32 YINFO_ANIM_HPMP_MAX = 16;
    private static readonly Int32 AbilFenril = 66;
    private static readonly Int32 AbilCarbuncle = 68;
    private static readonly Int32 AbilSaMpHalf = 226;
    private static readonly String ATENormal = "battle_bar_atb";
    private static readonly String ATEGray = "battle_bar_slow";
    private static readonly String ATEOrange = "battle_bar_haste";
    // TODO Check Native: #147
    private static readonly Single DefaultPartyPanelPosY = -420f;
    private static readonly Single PartyItemHeight = 60f;
    public static Dictionary<BattleStatus, String> DebuffIconNames;
    public static Dictionary<BattleStatus, String> BuffIconNames;
    private static readonly Color[] TranceTextColor;

    static BattleHUD()
    {
        CmdTitleTable = LoadBattleCommandTitles();

        DebuffIconNames = new Dictionary<BattleStatus, String>
        {
            {BattleStatus.Slow, FF9UIDataTool.IconSpriteName[139]},
            {BattleStatus.Freeze, FF9UIDataTool.IconSpriteName[140]},
            {BattleStatus.Heat, FF9UIDataTool.IconSpriteName[141]},
            {BattleStatus.Mini, FF9UIDataTool.IconSpriteName[142]},
            {BattleStatus.Sleep, FF9UIDataTool.IconSpriteName[143]},
            {BattleStatus.Poison, FF9UIDataTool.IconSpriteName[144]},
            {BattleStatus.Stop, FF9UIDataTool.IconSpriteName[145]},
            {BattleStatus.Berserk, FF9UIDataTool.IconSpriteName[146]},
            {BattleStatus.Confuse, FF9UIDataTool.IconSpriteName[147]},
            {BattleStatus.Zombie, FF9UIDataTool.IconSpriteName[148]},
            {BattleStatus.Trouble, FF9UIDataTool.IconSpriteName[149]},
            {BattleStatus.Blind, FF9UIDataTool.IconSpriteName[150]},
            {BattleStatus.Silence, FF9UIDataTool.IconSpriteName[151]},
            {BattleStatus.Virus, FF9UIDataTool.IconSpriteName[152]},
            {BattleStatus.Venom, FF9UIDataTool.IconSpriteName[153]},
            {BattleStatus.Petrify, FF9UIDataTool.IconSpriteName[154]}
        };

        BuffIconNames = new Dictionary<BattleStatus, String>
        {
            {BattleStatus.AutoLife, FF9UIDataTool.IconSpriteName[131]},
            {BattleStatus.Reflect, FF9UIDataTool.IconSpriteName[132]},
            {BattleStatus.Vanish, FF9UIDataTool.IconSpriteName[133]},
            {BattleStatus.Protect, FF9UIDataTool.IconSpriteName[134]},
            {BattleStatus.Shell, FF9UIDataTool.IconSpriteName[135]},
            {BattleStatus.Float, FF9UIDataTool.IconSpriteName[136]},
            {BattleStatus.Haste, FF9UIDataTool.IconSpriteName[137]},
            {BattleStatus.Regen, FF9UIDataTool.IconSpriteName[138]}
        };

        TranceTextColor = new[]
        {
            // 13
            new Color(1f, 0.2156863f, 0.3176471f),
            new Color(1f, 0.3490196f, 0.3529412f),
            new Color(1f, 0.4862745f, 0.3921569f),
            new Color(1f, 0.6235294f, 0.427451f),
            new Color(1f, 0.7568628f, 0.4666667f),
            new Color(1f, 0.8941177f, 0.5058824f),
            new Color(1f, 0.9647059f, 0.5254902f),
            new Color(1f, 0.8941177f, 0.5058824f),
            new Color(1f, 0.7568628f, 0.4666667f),
            new Color(1f, 0.6235294f, 0.427451f),
            new Color(1f, 0.4862745f, 0.3921569f),
            new Color(1f, 0.3490196f, 0.3529412f),
            new Color(1f, 0.2156863f, 0.3176471f)
        };
    }

    private static String FormatMagicSwordAbility(CMD_DATA pCmd)
    {
        // TODO: Move it to an external file
        String abilityName = FF9TextTool.ActionAbilityName(pCmd.sub_no);
        
        String result;
        if (TryFormatRussianMagicSwordAbility(abilityName, out result))
            return result;

        String commandTitle = FF9TextTool.BattleCommandTitleText(0);

        switch (Localization.GetSymbol())
        {
            case "JP":
                return $"{abilityName}{commandTitle}";
            case "FR":
            case "IT":
            case "ES":
                return $"{commandTitle}{abilityName}";
            default:
                return $"{abilityName} {commandTitle}";
        }
    }

    private static Boolean TryFormatRussianMagicSwordAbility(String abilityName, out String result)
    {
        switch (abilityName)
        {
            case "Огонь":
            {
                result = "Огненный клинок";
                return true;
            }
            case "Огонь II":
            {
                result = "Огненный клинок II";
                return true;
            }
            case "Огонь III":
            {
                result = "Огненный клинок III";
                return true;
            }
            case "Буран":
            {
                result = "Ледяной клинок";
                return true;
            }
            case "Буран II":
            {
                result = "Ледяной клинок II";
                return true;
            }
            case "Буран III":
            {
                result = "Ледяной клинок III";
                return true;
            }
            case "Молния":
            {
                result = "Электрический клинок";
                return true;
            }
            case "Молния II":
            {
                result = "Электрический клинок II";
                return true;
            }
            case "Молния III":
            {
                result = "Электрический клинок III";
                return true;
            }
            case "Био":
            {
                result = "Ядовитый клинок";
                return true;
            }
            case "Вода":
            {
                result = "Водный клинок";
                return true;
            }
            case "Взрыв":
            {
                result = "Взрывной клинок";
                return true;
            }
            case "Судный день":
            {
                result = "Клинок Судного дня";
                return true;
            }
        }

        result = null;
        return false;
    }

    private static BattleCommandId GetCommandFromCommandIndex(CommandMenu commandIndex, Int32 playerIndex)
    {
        BattleUnit player = FF9StateSystem.Battle.FF9Battle.GetUnit(playerIndex);
        CharacterPresetId presetId = FF9StateSystem.Common.FF9.party.GetCharacter(player.Position).PresetId;
        switch (commandIndex)
        {
            case CommandMenu.Attack:
                return BattleCommandId.Attack;
            case CommandMenu.Defend:
                return BattleCommandId.Defend;
            case CommandMenu.Ability1:
            {
                CharacterCommandSet commandSet = CharacterCommands.CommandSets[presetId];
                Boolean underTrance = player.IsUnderStatus(BattleStatus.Trance);
                return commandSet.Get(underTrance, 0);
            }
            case CommandMenu.Ability2:
            {
                CharacterCommandSet commandSet = CharacterCommands.CommandSets[presetId];
                Boolean underTrance = player.IsUnderStatus(BattleStatus.Trance);
                return commandSet.Get(underTrance, 1);
            }
            case CommandMenu.Item:
                return BattleCommandId.Item;
            case CommandMenu.Change:
                return BattleCommandId.Change;
            default:
                return BattleCommandId.None;
        }
    }

    private static Int32 GetFirstAlivePlayerIndex()
    {
        Int32 index = -1;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (unit.IsPlayer)
            {
                index++;
                if (unit.CurrentHp > 0)
                    return index;
            }
        }
        return index; // The bug is possible
    }

    private static Int32 GetAlivePlayerIndexForHealingAttack()
    {
        UInt32 minHp = Int32.MaxValue;
        Int32 minIndex = -1;

        Int32 index = -1;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
                continue;

            index++;
            if (unit.CurrentHp <= 0)
                continue;

            if (unit.IsUnderStatus(BattleStatus.Confuse | BattleStatus.Sleep))
                return index;

            if (unit.CurrentHp < minHp)
            {
                minHp = unit.CurrentHp;
                minIndex = index;
            }
        }
        return minIndex;
    }

    private static Int32 GetFirstAliveEnemyIndex()
    {
        Int32 index = -1;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
            {
                index++;
                if (unit.CurrentHp > 0)
                    return index;
            }
        }
        return index; // The bug is possible
    }

    private static BattleUnit GetFirstAliveEnemy()
    {
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (unit.IsPlayer)
                continue;

            if (unit.CurrentHp > 0)
                return unit;
        }

        return null;
    }

    private Int32 GetFirstAliveZombieEnemyIndex()
    {
        foreach (KnownUnit knownEnemy in EnumerateKnownEnemies())
        {
            BattleUnit unit = knownEnemy.Unit;
            if (unit.CurrentHp > 0 && unit.IsZombie && !unit.IsUnderAnyStatus(BattleStatus.Vanish))
                return knownEnemy.Index;
        }
        return -1;
    }

    private static EntryCollection<IdMap> LoadBattleCommandTitles()
    {
        try
        {
            String inputPath = DataResources.Characters.Directory + DataResources.Characters.CommandTitlesFile;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"[BattleHUD] Cannot load character command titles because a file does not exist: [{inputPath}].", inputPath);

            IdMap[] maps = CsvReader.Read<IdMap>(inputPath);
            if (maps.Length < 192)
                throw new NotSupportedException($"You must set titles for 192 battle commands, but there {maps.Length}.");

            EntryCollection<IdMap> result = EntryCollection.CreateWithDefaultElement(maps, g => g.Id);
            for (Int32 i = Configuration.Mod.FolderNames.Length - 1; i >= 0; i--)
            {
                inputPath = DataResources.Characters.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.Characters.CommandTitlesFile;
                if (File.Exists(inputPath))
                {
                    maps = CsvReader.Read<IdMap>(inputPath);
                    foreach (IdMap it in maps)
                        result[it.Id] = it;
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[BattleHUD] Load character command titles failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }
}