using Assets.Sources.Scripts.UI.Common;
using FF9;
using System;
using System.Collections.Generic;
using System.IO;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;
using UnityEngine;

#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

// ReSharper disable EmptyConstructor
// ReSharper disable RedundantAssignment
// ReSharper disable ArrangeThisQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToConstant.Global
// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming

public static class ff9play
{
    public const Int32 FF9PLAY_DAMAGE_MAX = 9999;
    public const Int32 FF9PLAY_MPDAMAGE_MAX = 999;
    public const Int32 FF9PLAY_HP_MAX = 9999;
    public const Int32 FF9PLAY_MP_MAX = 999;
    public const Int32 FF9PLAY_DEFPARAM_VAL_MAX = 999;
    public const Int32 FF9PLAY_DEF_LEVEL = 1;
    public const Int32 FF9PLAY_DEF_GIL = 500;
    public const Int32 FF9PLAY_DEF_AT = 10;
    public const Int32 FF9PLAY_NAME_MAX = 10;
    public const Int32 FF9PLAY_FLD_STATUS_MAX = 7;
    public const Int32 FF9PLAY_EQUIP_ID_NONE = 255;
    public static readonly Byte[] FF9PLAY_STAT_MAX = { 50, 99, 99, 50 };

    private static Boolean _FF9Play_Face;
    private static Dictionary<EquipmentSetId, CharacterEquipment> DefaultEquipment;
    private static Dictionary<CharacterId, CharacterParameter> CharacterParameterList;

    public static void FF9Play_Init()
    {
        DefaultEquipment = LoadCharacterDefaultEquipment();
        CharacterParameterList = LoadCharacterParameters();
        btl_mot.Init();

        foreach (CharacterParameter param in CharacterParameterList.Values)
        {
            NGUIText.RegisterCustomNameKeywork(param.NameKeyword, param.Id);
            FF9StateSystem.Common.FF9.player[param.Id] = new PLAYER();
        }
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        FF9Play_SetFaceDirty(false);
        foreach (CharacterParameter param in CharacterParameterList.Values)
            FF9Play_New(param.Id);
        FF9Play_Add(FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Zidane));
        FF9Play_SetParty(0, CharacterId.Zidane);
        FF9Play_SetParty(1, CharacterId.NONE);
        FF9Play_SetParty(2, CharacterId.NONE);
        FF9Play_SetParty(3, CharacterId.NONE);
        ff9StateGlobal.party.gil = FF9PLAY_DEF_GIL;
        ff9StateGlobal.party.summon_flag = 0;
    }

    private static Dictionary<EquipmentSetId, CharacterEquipment> LoadCharacterDefaultEquipment()
    {
        try
        {
            String inputPath = DataResources.Characters.PureDirectory + DataResources.Characters.DefaultEquipmentsFile;
            Dictionary<EquipmentSetId, CharacterEquipment> result = new Dictionary<EquipmentSetId, CharacterEquipment>();
            foreach (CharacterEquipment[] equips in AssetManager.EnumerateCsvFromLowToHigh<CharacterEquipment>(inputPath))
                foreach (CharacterEquipment equip in equips)
                    result[equip.Id] = equip;
            if (result.Count == 0)
                throw new FileNotFoundException($"Cannot load equipment sets because a file does not exist: [{DataResources.Characters.Directory + DataResources.Characters.DefaultEquipmentsFile}].", DataResources.Characters.Directory + DataResources.Characters.DefaultEquipmentsFile);
            for (Int32 i = 0; i < 15; i++)
                if (!result.ContainsKey((EquipmentSetId)i))
                    throw new NotSupportedException($"You must define at least the 15 equipment sets, with IDs between 0 and 14.");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ff9play] Load characters default equipments failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    private static Dictionary<CharacterId, CharacterParameter> LoadCharacterParameters()
    {
        try
        {
            String inputPath = DataResources.Characters.PureDirectory + DataResources.Characters.CharacterParametersFile;
            Dictionary<CharacterId, CharacterParameter> result = new Dictionary<CharacterId, CharacterParameter>();
            foreach (CharacterParameter[] characters in AssetManager.EnumerateCsvFromLowToHigh<CharacterParameter>(inputPath))
                foreach (CharacterParameter character in characters)
                    result[character.Id] = character;
            if (result.Count == 0)
                throw new FileNotFoundException($"Cannot load character parameters because a file does not exist: [{DataResources.Characters.Directory + DataResources.Characters.CharacterParametersFile}].", DataResources.Characters.Directory + DataResources.Characters.CharacterParametersFile);
            for (Int32 i = 0; i < 12; i++)
                if (!result.ContainsKey((CharacterId)i))
                    throw new NotSupportedException($"You must define at least 12 character parameters, with IDs between 0 and 11.");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ff9play] Load character parameters failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    public static void FF9Play_New(CharacterId slotId)
    {
        PLAYER play = FF9StateSystem.Common.FF9.player[slotId];
        CharacterParameter parameter = CharacterParameterList[slotId];
        play.info = new PLAYER_INFO(parameter.Id, CharacterSerialNumber.ZIDANE_DAGGER, parameter.DefaultRow, parameter.DefaultWinPose, 0, parameter.DefaultMenuType);
        play.pa = new Byte[ff9abil._FF9Abil_PaData.ContainsKey(play.info.menu_type) ? ff9abil._FF9Abil_PaData[play.info.menu_type].Length : 0];
        play.status = 0;
        play.permanent_status = 0;
        play.category = parameter.DefaultCategory;
        play.bonus = new FF9LEVEL_BONUS();
        play.Name = FF9TextTool.CharacterDefaultName(play.info.slot_no);
        FF9Play_SetDefEquips(play.equip, parameter.DefaultEquipmentSet, true);
        play.info.serial_no = parameter.GetSerialNumber();
        FF9Play_Build(play, FF9PLAY_DEF_LEVEL, true, false);
        play.cur.hp = play.max.hp;
        play.cur.mp = play.max.mp;
        play.cur.capa = play.max.capa;
    }

    public static void FF9Play_UpdateSerialNumber(PLAYER player)
    {
        CharacterParameter parameter = CharacterParameterList[player.info.slot_no];
        player.info.serial_no = parameter.GetSerialNumber();
    }

    public static void FF9Play_SetParty(Int32 partySlot, CharacterId charId)
    {
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        PLAYER player = ff9StateGlobal.party.member[partySlot];

        if (charId == CharacterId.NONE)
        {
            ff9StateGlobal.party.member[partySlot] = null;
        }
        else
        {
            PLAYER newMember = ff9StateGlobal.GetPlayer(charId);
            FF9Play_Add(newMember);
            ff9StateGlobal.party.member[partySlot] = newMember;
        }

        if (player == ff9StateGlobal.party.member[partySlot])
            return;

        FF9Play_SetFaceDirty(true);
    }

    public static Int32 FF9Play_GetPrev(Int32 id)
    {
        do
        {
            id = id == 0 ? 3 : id - 1;
        } while (FF9StateSystem.Common.FF9.party.member[id] == null);
        return id;
    }

    public static Int32 FF9Play_GetNext(Int32 id)
    {
        do
        {
            id = id >= 3 ? 0 : id + 1;
        } while (FF9StateSystem.Common.FF9.party.member[id] == null);
        return id;
    }

    private static Int32 FF9Play_GetAvgLevel(CharacterId exceptionId)
    {
        Int32 playCount = 0;
        Int32 lvlSum = 0;
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        foreach (PLAYER player in ff9StateGlobal.PlayerList)
        {
            if (player.Index != exceptionId && player.info.party != 0)
            {
                lvlSum += player.level;
                ++playCount;
            }
        }

        if (playCount == 0)
            return 1;

        return lvlSum / playCount;
    }

    public static void FF9Play_Add(PLAYER player)
    {
        player.info.party = 1;
    }

    public static void FF9Play_Delete(PLAYER player)
    {
        player.info.party = 0;
    }

    public static void FF9Play_Build(PLAYER play, Int32 lv, Boolean init, Boolean lvup)
    {
        if (!lvup && play.level != lv)
            play.exp = ff9level.CharacterLevelUps[lv - 1].ExperienceToLevel;
        play.level = (Byte)lv;
        Int64 oldStoneUse = play.max.capa - play.cur.capa;
        Int64 oldHP = play.cur.hp;
        Int64 oldMP = play.cur.mp;
        Int64 oldStoneCapa = play.cur.capa;
        Int64 oldStoneMax = play.max.capa;
        play.cur = new POINTS();
        play.max = new POINTS();
        play.cur.at_coef = FF9PLAY_DEF_AT;
        play.basis.dex = (Byte)ff9level.FF9Level_GetDex(play, play.level, lvup);
        play.basis.str = (Byte)ff9level.FF9Level_GetStr(play, play.level, lvup);
        play.basis.mgc = (Byte)ff9level.FF9Level_GetMgc(play, play.level, lvup);
        play.basis.wpr = (Byte)ff9level.FF9Level_GetWpr(play, play.level, lvup);
        play.max.hp = ff9level.FF9Level_GetHp(play, play.level, play.basis.str);
        play.basis.max_hp = play.max.hp;
        play.max.mp = play.basis.max_mp = ff9level.FF9Level_GetMp(play, play.level, play.basis.mgc);
        play.max.capa = (Byte)ff9level.FF9Level_GetCap(play, play.level, lvup);
        play.cur.hp = ccommon.min((UInt32)oldHP, play.max.hp);
        play.cur.mp = ccommon.min((UInt32)oldMP, play.max.mp);
        play.cur.capa = (Byte)(play.max.capa - (UInt64)oldStoneUse);
        if (init)
        {
            play.cur.hp = play.max.hp;
            play.cur.mp = play.max.mp;
            play.cur.capa = play.max.capa;
        }
        FF9Play_Update(play);
        if (lvup)
        {
            play.cur.hp = ccommon.min((UInt32)oldHP, play.max.hp);
            play.cur.mp = ccommon.min((UInt32)oldMP, play.max.mp);
        }
        if (oldStoneMax == 99L)
        {
            play.cur.capa = (Byte)oldStoneCapa;
            play.max.capa = (Byte)oldStoneMax;
        }
        if (oldStoneUse <= play.max.capa)
            return;
        FF9Play_UpdateSA(play);
    }

    public static void FF9Play_Update(PLAYER play)
    {
        FF9PLAY_INFO info = new FF9PLAY_INFO();
        FF9PLAY_SKILL skill = new FF9PLAY_SKILL();
        info.Base = play.basis;
        info.cur_hp = play.cur.hp;
        info.cur_mp = play.cur.mp;
        info.equip.Absorb(play.equip);
        FF9Play_GetSkill(ref info, ref skill);
        play.elem.dex = skill.Base[0];
        play.elem.str = skill.Base[1];
        play.elem.mgc = skill.Base[2];
        play.elem.wpr = skill.Base[3];
        play.defence.PhysicalDefence = skill.defParam[1];
        play.defence.PhysicalEvade = skill.defParam[2];
        play.defence.MagicalDefence = skill.defParam[3];
        play.defence.MagicalEvade = skill.defParam[4];
        play.max.hp = skill.max_hp;
        play.max.mp = skill.max_mp;
        play.mpCostFactor = 100;
        play.mpCostFactorSkill1 = 100;
        play.mpCostFactorSkill2 = 100;
        play.maxHpLimit = ff9play.FF9PLAY_HP_MAX;
        play.maxMpLimit = ff9play.FF9PLAY_MP_MAX;
        play.maxDamageLimit = ff9play.FF9PLAY_DAMAGE_MAX;
        play.maxMpDamageLimit = ff9play.FF9PLAY_MPDAMAGE_MAX;
        foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(play))
            saFeature.TriggerOnEnable(play);
        if (play.max.hp > play.maxHpLimit)
            play.max.hp = play.maxHpLimit;
        if (play.max.mp > play.maxMpLimit)
            play.max.mp = play.maxMpLimit;
        if (play.cur.hp > play.max.hp)
            play.cur.hp = play.max.hp;
        if (play.cur.mp > play.max.mp)
            play.cur.mp = play.max.mp;
    }

    public static FF9PLAY_SKILL FF9Play_GetSkill(ref FF9PLAY_INFO info, ref FF9PLAY_SKILL skill)
    {
        skill = new FF9PLAY_SKILL
        {
            cur_hp = info.cur_hp,
            cur_mp = info.cur_mp,
            max_hp = info.Base.max_hp,
            max_mp = info.Base.max_mp,
            Base =
            {
                [0] = info.Base.dex,
                [1] = info.Base.str,
                [2] = info.Base.mgc,
                [3] = info.Base.wpr
            }
        };
        if (info.equip[0] != RegularItem.NoItem)
            skill.defParam[0] = ff9item.GetItemWeapon(info.equip[0]).Ref.Power;
        for (Int32 i = 0; i < 5; ++i)
        {
            RegularItem itemId = info.equip[i];
            if (itemId != RegularItem.NoItem)
            {
                if (ff9item.HasItemArmor(itemId))
                {
                    ItemDefence defParams = ff9item.GetItemArmor(itemId);
                    skill.defParam[1] += defParams.PhysicalDefence;
                    skill.defParam[2] += defParams.PhysicalEvade;
                    skill.defParam[3] += defParams.MagicalDefence;
                    skill.defParam[4] += defParams.MagicalEvade;
                }
                ItemStats equipPrivilege = ff9equip.ItemStatsData[ff9item._FF9Item_Data[itemId].bonus];
                skill.Base[0] += equipPrivilege.dex;
                skill.Base[1] += equipPrivilege.str;
                skill.Base[2] += equipPrivilege.mgc;
                skill.Base[3] += equipPrivilege.wpr;
            }
        }
        for (Int32 i = 0; i < 4; ++i)
            if (skill.Base[i] > FF9PLAY_STAT_MAX[i])
                skill.Base[i] = FF9PLAY_STAT_MAX[i];
        for (Int32 i = 0; i < 5; ++i)
            if (skill.defParam[i] > FF9PLAY_DEFPARAM_VAL_MAX)
                skill.defParam[i] = FF9PLAY_DEFPARAM_VAL_MAX;
        if (skill.cur_hp > skill.max_hp)
            skill.cur_hp = skill.max_hp;
        if (skill.cur_mp > skill.max_mp)
            skill.cur_mp = skill.max_mp;
        return skill;
    }

    public static CharacterId CharacterOldIndexToID(CharacterOldIndex characterIndex)
    {
        if (characterIndex <= CharacterOldIndex.Amarant)
            return (CharacterId)characterIndex;
        if (characterIndex == CharacterOldIndex.Beatrix)
            return CharacterId.Beatrix;
        if (characterIndex == CharacterOldIndex.Cinna)
            return CharacterId.Cinna;
        if (characterIndex == CharacterOldIndex.Marcus)
            return CharacterId.Marcus;
        if (characterIndex == CharacterOldIndex.Blank)
            return CharacterId.Blank;
        if (characterIndex == CharacterOldIndex.NONE)
            return CharacterId.NONE;
        return (CharacterId)characterIndex;
    }

    public static CharacterOldIndex CharacterIDToOldIndex(CharacterId charId)
    {
        if (charId <= CharacterId.Amarant)
            return (CharacterOldIndex)charId;
        if (charId == CharacterId.Beatrix)
            return CharacterOldIndex.Beatrix;
        if (charId == CharacterId.Cinna)
            return CharacterOldIndex.Cinna;
        if (charId == CharacterId.Marcus)
            return CharacterOldIndex.Marcus;
        if (charId == CharacterId.Blank)
            return CharacterOldIndex.Blank;
        if (charId == CharacterId.NONE)
            return CharacterOldIndex.NONE;
        return (CharacterOldIndex)charId;
    }

    public static Int32 CharacterIDToEventId(CharacterId characterId)
    {
        if (characterId <= CharacterId.Amarant)
            return (Int32)characterId;
        if (characterId == CharacterId.Beatrix)
            return (Int32)CharacterOldIndex.Beatrix;
        if (characterId == CharacterId.Cinna)
            return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Quina).info.sub_replaced ? -1 : 5;
        if (characterId == CharacterId.Marcus)
            return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Eiko).info.sub_replaced ? -1 : 6;
        if (characterId == CharacterId.Blank)
            return FF9StateSystem.Common.FF9.GetPlayer(CharacterId.Amarant).info.sub_replaced ? -1 : 7;
        return -1;
    }

    public static void FF9Play_SetFaceDirty(Boolean dirty)
    {
        _FF9Play_Face = dirty;
    }

    private static Boolean FF9Play_GetFaceDirty()
    {
        return _FF9Play_Face;
    }

    public static void FF9Play_SetDefEquips(CharacterEquipment target, EquipmentSetId equipmentId, Boolean isNewPlayer)
    {
        CharacterEquipment newSet = DefaultEquipment[equipmentId];
        CharacterEquipment characterInitialSet = null;
        if (!isNewPlayer)
		{
            if (equipmentId == EquipmentSetId.Blank2)
                characterInitialSet = DefaultEquipment[EquipmentSetId.Blank];
            else if (equipmentId == EquipmentSetId.Marcus2)
                characterInitialSet = DefaultEquipment[EquipmentSetId.Marcus];
            else if (equipmentId == EquipmentSetId.Beatrix2)
                characterInitialSet = DefaultEquipment[EquipmentSetId.Beatrix];
        }
        Boolean isCharacterReequip = characterInitialSet != null;
        if (isCharacterReequip)
            for (Int32 i = 0; i < 5; i++)
                if (characterInitialSet[i] != target[i])
                    isCharacterReequip = false;
        for (Int32 i = 0; i < 5; i++)
        {
            if (!isNewPlayer && !isCharacterReequip && target[i] != RegularItem.NoItem && target[i] != newSet[i])
            {
                if (target[i] == RegularItem.Moonstone)
                    ff9item.DecreaseMoonStoneCount();
                ff9item.FF9Item_Add(target[i], 1);
            }
            target[i] = newSet[i];
        }
    }

    public static void FF9Play_GrowLevel(PLAYER player, Int32 lv)
    {
        BattleAchievement.GetReachLv99Achievement(lv);
        FF9Play_Build(player, lv, false, true);
    }

    public static Int32 FF9Play_ChangeLevel(PLAYER player, Int32 lv, Boolean max_hpmp)
    {
        Int32 num = player.level;
        player.level = 0;
        FF9Play_Build(player, lv, false, false);
        if (max_hpmp)
        {
            player.cur.hp = player.max.hp;
            player.cur.mp = player.max.mp;
        }
        return num;
    }

    private static void FF9Play_UpdateSA(PLAYER play)
    {
        play.cur.capa = play.max.capa;
        if (Configuration.Battle.LockEquippedAbilities == 1 || Configuration.Battle.LockEquippedAbilities == 3)
            return;
        HashSet<SupportAbility> disableSet = new HashSet<SupportAbility>();
        foreach (SupportAbility saIndex in play.saExtended)
        {
            if (play.cur.capa >= ff9abil._FF9Abil_SaData[saIndex].GemsCount)
                play.cur.capa -= ff9abil._FF9Abil_SaData[saIndex].GemsCount;
            else
                disableSet.Add(saIndex);
        }
        foreach (SupportAbility saIndex in disableSet)
            ff9abil.FF9Abil_SetEnableSA(play, saIndex, false);
    }

    public static void FF9Play_Change(PLAYER play, Boolean update_lv, EquipmentSetId equipSetId)
    {
        if (equipSetId != EquipmentSetId.NONE)
            FF9Play_SetDefEquips(play.equip, equipSetId, false);

        ff9play.FF9Play_UpdateSerialNumber(play);
        if (!update_lv && equipSetId == EquipmentSetId.NONE)
            return;
        
        if (update_lv)
        {
            Int32 lv = Mathf.Max(play.level, FF9Play_GetAvgLevel(play.Index));
            FF9Play_Build(play, lv, false, false);
        }
        else
        {
            FF9Play_Update(play);
        }
    }

    public static void FF9Dbg_SetCharacter(CharacterId player, Int32 slot)
    {
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        FF9DBG_CHAR[] ff9DbgCharArray =
        {
            new FF9DBG_CHAR(0, false, -1),
            new FF9DBG_CHAR(1, false, -1),
            new FF9DBG_CHAR(2, false, -1),
            new FF9DBG_CHAR(3, false, -1),
            new FF9DBG_CHAR(4, false, -1),
            new FF9DBG_CHAR(5, false, 5),
            new FF9DBG_CHAR(6, false, 6),
            new FF9DBG_CHAR(7, false, 7),
            new FF9DBG_CHAR(5, true, 8),
            new FF9DBG_CHAR(6, true, 10),
            new FF9DBG_CHAR(7, true, 12),
            new FF9DBG_CHAR(8, true, 14)
        };
        if (player == CharacterId.NONE)
        {
            ff9StateGlobal.party.member[slot] = null;
        }
        else
        {
            FF9DBG_CHAR ff9DbgChar = ff9DbgCharArray[(Byte)player];
            PLAYER play = ff9StateGlobal.party.member[slot] = ff9StateGlobal.player[(CharacterId)ff9DbgChar.slot_no];
            CharacterParameter parameter = CharacterParameterList[player];
            if (ff9DbgChar.menu_type >= 0)
            {
                if (player != parameter.Id)
                    play.info.menu_type = (CharacterPresetId)ff9DbgChar.menu_type;
                play.category = parameter.DefaultCategory;
                if (!FF9Dbg_CheckEquip(play))
                    FF9Play_SetDefEquips(play.equip, parameter.DefaultEquipmentSet, true);
                play.info.serial_no = parameter.GetSerialNumber();
            }
            play.category = parameter.DefaultCategory;
        }
    }

    private static Boolean FF9Dbg_CheckEquip(PLAYER play)
    {
        UInt64 characterMask = ff9feqp.GetCharacterEquipMask(play);
        for (Int32 i = 0; i < 5; ++i)
        {
            RegularItem itemIndex;
            if ((itemIndex = play.equip[i]) != RegularItem.NoItem && (ff9item._FF9Item_Data[itemIndex].equip & characterMask) == 0)
                return false;
        }
        return true;
    }

    public class FF9DBG_CHAR
    {
        public Byte slot_no;
        public Boolean sub;
        public SByte menu_type;
        public Byte pad;

        public FF9DBG_CHAR(Byte slot_no, Boolean sub, SByte menu_type)
        {
            this.slot_no = slot_no;
            this.sub = sub;
            this.menu_type = menu_type;
        }
    }
}
