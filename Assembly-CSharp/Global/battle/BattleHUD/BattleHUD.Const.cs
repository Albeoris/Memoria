using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Database;
using Memoria.Prime;
using Memoria.Prime.CSV;
using System;
using System.Collections.Generic;
using System.IO;
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

    public const String ATENormal = "battle_bar_atb";
    public const String ATEGray = "battle_bar_slow";
    public const String ATEOrange = "battle_bar_haste";

    private static readonly Byte[] BattleMessageTimeTick = [54, 46, 48, 30, 24, 18, 12];
    private static readonly Dictionary<BattleAbilityId, IdMap> CmdTitleTable;
    private static readonly Int32 YINFO_ANIM_HPMP_MIN = 4;
    private static readonly Int32 YINFO_ANIM_HPMP_MAX = 16;
    private static readonly Single DefaultPartyPanelPosY = -420f;
    private static readonly Single PartyItemHeight = 60f;
    public static Dictionary<BattleStatusId, String> DebuffIconNames;
    public static Dictionary<BattleStatusId, String> BuffIconNames;
    private static readonly Color[] TranceTextColor;

    static BattleHUD()
    {
        // Ignore "CommandTitles.csv" for good
        /*
        CmdTitleTable = LoadBattleCommandTitles();
        foreach (IdMap mappingId in CmdTitleTable.Values)
            if (FF9BattleDB.CharacterActions.ContainsKey(mappingId.Id))
                FF9BattleDB.CharacterActions[mappingId.Id].CastingTitleType = mappingId.MappedId;
        */

        DebuffIconNames = new Dictionary<BattleStatusId, String>
        {
            {BattleStatusId.Slow,    FF9UIDataTool.IconSpriteName[139]},
            {BattleStatusId.Freeze,  FF9UIDataTool.IconSpriteName[140]},
            {BattleStatusId.Heat,    FF9UIDataTool.IconSpriteName[141]},
            {BattleStatusId.Mini,    FF9UIDataTool.IconSpriteName[142]},
            {BattleStatusId.Sleep,   FF9UIDataTool.IconSpriteName[143]},
            {BattleStatusId.Poison,  FF9UIDataTool.IconSpriteName[144]},
            {BattleStatusId.Stop,    FF9UIDataTool.IconSpriteName[145]},
            {BattleStatusId.Berserk, FF9UIDataTool.IconSpriteName[146]},
            {BattleStatusId.Confuse, FF9UIDataTool.IconSpriteName[147]},
            {BattleStatusId.Zombie,  FF9UIDataTool.IconSpriteName[148]},
            {BattleStatusId.Trouble, FF9UIDataTool.IconSpriteName[149]},
            {BattleStatusId.Blind,   FF9UIDataTool.IconSpriteName[150]},
            {BattleStatusId.Silence, FF9UIDataTool.IconSpriteName[151]},
            {BattleStatusId.Virus,   FF9UIDataTool.IconSpriteName[152]},
            {BattleStatusId.Venom,   FF9UIDataTool.IconSpriteName[153]},
            {BattleStatusId.Petrify, FF9UIDataTool.IconSpriteName[154]}
        };

        BuffIconNames = new Dictionary<BattleStatusId, String>
        {
            {BattleStatusId.AutoLife, FF9UIDataTool.IconSpriteName[131]},
            {BattleStatusId.Reflect,  FF9UIDataTool.IconSpriteName[132]},
            {BattleStatusId.Vanish,   FF9UIDataTool.IconSpriteName[133]},
            {BattleStatusId.Protect,  FF9UIDataTool.IconSpriteName[134]},
            {BattleStatusId.Shell,    FF9UIDataTool.IconSpriteName[135]},
            {BattleStatusId.Float,    FF9UIDataTool.IconSpriteName[136]},
            {BattleStatusId.Haste,    FF9UIDataTool.IconSpriteName[137]},
            {BattleStatusId.Regen,    FF9UIDataTool.IconSpriteName[138]}
        };

        TranceTextColor = new Color[]
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

    public static String FormatMagicSwordAbility(CMD_DATA pCmd)
    {
        // TODO: Move it to an external file
        String abilityName = FF9TextTool.ActionAbilityName(btl_util.GetCommandMainActionIndex(pCmd));

        if (TryFormatRussianMagicSwordAbility(abilityName, out String result))
            return result;

        String commandTitle = FF9TextTool.BattleCommandTitleText(0);
        Int32 replacePos = commandTitle.IndexOf('%');
        if (replacePos >= 0)
            return commandTitle.Substring(0, replacePos) + abilityName + commandTitle.Substring(replacePos + 1);

        switch (Localization.GetSymbol())
        {
            case "JP":
                return $"{abilityName}{commandTitle}";
            case "FR":
            case "IT":
            case "ES":
                return $"{commandTitle}{abilityName}";
            case "US":
            case "UK":
                if (String.Equals(commandTitle, "Sword"))
                    return $"{abilityName} {commandTitle}";
                return $"{commandTitle}{abilityName}";
            case "GR":
                // Thanks to McPerser for reporting that translation error
                // https://finalfantasy.fandom.com/de/wiki/Magieschwert_(FFIX)
                if (String.Equals(commandTitle, "Aklinge"))
                    return $"{abilityName}klinge";
                return $"{commandTitle}{abilityName}";
            default:
                return $"{commandTitle}{abilityName}";
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

    private static BattleCommandId GetCommandFromCommandIndex(BattleCommandMenu commandIndex, Int32 playerIndex)
    {
        BattleUnit player = FF9StateSystem.Battle.FF9Battle.GetUnit(playerIndex);
        CharacterPresetId presetId = FF9StateSystem.Common.FF9.party.member[player.Position].PresetId;
        if ((Int32)commandIndex >= 0 && (Int32)commandIndex < CharacterCommandSet.SupportedMenus.Count)
        {
            BattleCommandId result = CharacterCommands.CommandSets[presetId].Get(player.IsUnderAnyStatus(BattleStatus.Trance), commandIndex);
            return BattleCommandHelper.Patch(result, commandIndex, player.Player, player);
        }
        else if (commandIndex == BattleCommandMenu.AccessMenu)
        {
            return BattleCommandHelper.Patch(BattleCommandId.AccessMenu, commandIndex, player.Player, player);
        }
        return BattleCommandId.None;
    }

    private static Int32 GetFirstAlivePlayerIndex()
    {
        Int32 index = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (unit.IsPlayer)
            {
                if (unit.CurrentHp > 0)
                    return index;
                index++;
            }
        }
        return -1;
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

            if (unit.IsUnderAnyStatus(BattleStatusConst.RemoveOnPhysicallyAttacked))
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
        Int32 index = 0;
        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            if (!unit.IsPlayer)
            {
                if (unit.CurrentHp > 0)
                    return HonoluluBattleMain.EnemyStartIndex + index;
                index++;
            }
        }
        return -1;
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

    private static Dictionary<BattleAbilityId, IdMap> LoadBattleCommandTitles()
    {
        try
        {
            String inputPath = DataResources.Characters.PureDirectory + DataResources.Characters.CommandTitlesFile;
            Dictionary<BattleAbilityId, IdMap> result = new Dictionary<BattleAbilityId, IdMap>();
            foreach (IdMap[] maps in AssetManager.EnumerateCsvFromLowToHigh<IdMap>(inputPath))
                foreach (IdMap it in maps)
                    result[it.Id] = it;
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[BattleHUD] Load character command titles failed.");
            return new Dictionary<BattleAbilityId, IdMap>();
        }
    }
}
