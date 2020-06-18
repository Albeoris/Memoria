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
using Object = System.Object;

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

public class ff9play
{
    public const Int32 FF9PLAY_HP_MAX = 9999;
    public const Int32 FF9PLAY_MP_MAX = 999;
    public const Int32 FF9PLAY_WEAPON_VAL_MAX = 999;
    public const Int32 FF9PLAY_DEF_LEVEL = 1;
    public const Int32 FF9PLAY_DEF_GIL = 500;
    public const Int32 FF9PLAY_DEF_AT = 10;
    public const Int32 FF9PLAY_NAME_MAX = 10;
    public const Int32 FF9PLAY_FLD_STATUS_MAX = 7;
    public const Int32 FF9PLAY_EQUIP_ID_NONE = 255;

    private static Boolean _FF9Play_Face;
    private static EntryCollection<CharacterEquipment> DefaultEquipment;

    public ff9play()
    {
    }

    public static void FF9Play_Init()
    {
        DefaultEquipment = LoadCharacterDefaultEquipment();

        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        FF9Play_SetFaceDirty(false);
        for (Int32 slot_id = 0; slot_id < 9; ++slot_id)
            FF9Play_New(slot_id);
        FF9Play_Add(0);
        FF9Play_Add(1);
        FF9Play_Add(2);
        FF9Play_Add(3);
        FF9Play_Add(4);
        FF9Play_Add(5);
        FF9Play_Add(7);
        FF9Play_Add(6);
        FF9Play_SetParty(0, 0);
        FF9Play_SetParty(1, 1);
        FF9Play_SetParty(2, 2);
        FF9Play_SetParty(3, 3);
        ff9StateGlobal.party.gil = 500U;
        ff9StateGlobal.party.summon_flag = 0;
    }

    private static EntryCollection<CharacterEquipment> LoadCharacterDefaultEquipment()
    {
        try
        {
            String inputPath = DataResources.Characters.DefaultEquipmentsFile;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"File with characters default equipments not found: [{inputPath}]");

            CharacterEquipment[] equipment = CsvReader.Read<CharacterEquipment>(inputPath);
            if (equipment.Length < 15)
                throw new NotSupportedException($"You must set at least 15 different entries, but there {equipment.Length}.");

            return EntryCollection.CreateWithDefaultElement(equipment, e => e.Id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ff9play] Load characters default equipments failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    public static void FF9Play_New(Int32 slot_id)
    {
        PLAYER play = FF9StateSystem.Common.FF9.player[slot_id];
        PLAYER_INFO[] playerInfoArray =
        {
            new PLAYER_INFO(CharacterIndex.Zidane, 1, 1, 1, 0, 0),
            new PLAYER_INFO(CharacterIndex.Vivi, 2, 0, 1, 0, 1),
            new PLAYER_INFO(CharacterIndex.Garnet, 3, 0, 1, 0, 2),
            new PLAYER_INFO(CharacterIndex.Steiner, 7, 1, 1, 0, 3),
            new PLAYER_INFO(CharacterIndex.Freya, 12, 0, 1, 0, 4),
            new PLAYER_INFO(CharacterIndex.Quina, 14, 1, 1, 0, 8),
            new PLAYER_INFO(CharacterIndex.Eiko, 15, 1, 1, 0, 10),
            new PLAYER_INFO(CharacterIndex.Amarant, 16, 1, 1, 0, 12),
            new PLAYER_INFO(CharacterIndex.Beatrix, 18, 1, 1, 0, 14)
        };
        play.info = playerInfoArray[slot_id];
        play.status = 0;
        play.category = (Byte)FF9Play_GetCategory(play.info.menu_type);
        play.bonus = new FF9LEVEL_BONUS();
        play.name = FF9TextTool.CharacterDefaultName(play.info.menu_type);
        FF9Play_SetDefEquips(new List<Int32>() {0, 1, 2, 3, 4}, play.equip, play.DefaultEquipmentSetId);
        play.info.serial_no = (Byte)FF9Play_GetSerialID(play.info.slot_no, play.IsSubCharacter, play.equip);
        FF9Play_Build(slot_id, 1, playerInfoArray[slot_id], false);
        play.cur.hp = play.max.hp;
        play.cur.mp = play.max.mp;
        play.cur.capa = play.max.capa;
    }

    public static void FF9Play_SetParty(Int32 party_id, Int32 slot_id)
    {
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        PLAYER player = ff9StateGlobal.party.member[party_id];
        if (slot_id >= 9)
            slot_id = -1;

        if (0 > slot_id)
        {
            ff9StateGlobal.party.member[party_id] = null;
        }
        else
        {
            FF9Play_Add(slot_id);
            ff9StateGlobal.party.member[party_id] = ff9StateGlobal.player[slot_id];
        }

        if (player == ff9StateGlobal.party.member[party_id])
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

    private static Int32 FF9Play_GetAvgLevel(Int32 slot_id)
    {
        Int32 num1 = 0;
        Int32 num2 = 0;
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        for (Int32 index = 0; index < 9; ++index)
        {
            if (slot_id != index && ff9StateGlobal.player[index].info.party != 0)
            {
                num2 += ff9StateGlobal.player[index].level;
                ++num1;
            }
        }

        if (num1 == 0)
            return 1;

        return num2 / num1;
    }

    public static void FF9Play_Add(Int32 slot_id)
    {
        if (slot_id < 0 || slot_id >= 9)
            return;

        FF9StateSystem.Common.FF9.player[slot_id].info.party = 1;
    }

    public static void FF9Play_Delete(Int32 slot_id)
    {
        if (slot_id < 0 || slot_id >= 9)
            return;

        FF9StateSystem.Common.FF9.player[slot_id].info.party = 0;
    }

    public static void FF9Play_Build(Int32 slot_id, Int32 lv, PLAYER_INFO init, Boolean lvup)
    {
        PLAYER play = FF9StateSystem.Common.FF9.player[slot_id];
        if (!lvup && play.level != lv)
            play.exp = ff9level.CharacterLevelUps[lv - 1].ExperienceToLevel;
        play.level = (Byte)lv;
        Int64 num1 = play.max.capa - play.cur.capa;
        Int64 num2 = play.cur.hp;
        Int64 num3 = play.cur.mp;
        Int64 num4 = play.cur.capa;
        Int64 num5 = play.max.capa;
        play.cur = new POINTS();
        play.max = new POINTS();
        play.cur.at_coef = 10;
        play.basis.dex = (Byte)ff9level.FF9Level_GetDex(slot_id, play.level, lvup);
        play.basis.str = (Byte)ff9level.FF9Level_GetStr(slot_id, play.level, lvup);
        play.basis.mgc = (Byte)ff9level.FF9Level_GetMgc(slot_id, play.level, lvup);
        play.basis.wpr = (Byte)ff9level.FF9Level_GetWpr(slot_id, play.level, lvup);
        play.max.hp = ff9level.FF9Level_GetHp(play.level, play.basis.str);
        play.basis.max_hp = play.max.hp;
        play.max.mp = play.basis.max_mp = ff9level.FF9Level_GetMp(play.level, play.basis.mgc);
        play.max.capa = (Byte)ff9level.FF9Level_GetCap(slot_id, play.level, lvup);
        play.cur.hp = ccommon.min((UInt32)num2, play.max.hp);
        play.cur.mp = ccommon.min((UInt32)num3, play.max.mp);
        play.cur.capa = (Byte)(play.max.capa - (UInt64)num1);
        if (init != null)
        {
            play.cur.hp = play.max.hp;
            play.cur.mp = play.max.mp;
            play.cur.capa = play.max.capa;
        }
        FF9Play_Update(play);
        if (lvup)
        {
            play.cur.hp = ccommon.min((UInt32)num2, play.max.hp);
            play.cur.mp = ccommon.min((UInt32)num3, play.max.mp);
        }
        if (num5 == 99L)
        {
            play.cur.capa = (Byte)num4;
            play.max.capa = (Byte)num5;
        }
        if (num1 <= play.max.capa)
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
        for (Int32 index = 0; index < 1; ++index)
            info.sa[index] = play.sa[index];
        info.equip.Absorb(play.equip);
        FF9Play_GetSkill(ref info, ref skill);
        play.elem.dex = skill.Base[0];
        play.elem.str = skill.Base[1];
        play.elem.mgc = skill.Base[2];
        play.elem.wpr = skill.Base[3];
        play.defence.PhisicalDefence = (Byte)skill.weapon[1];
        play.defence.PhisicalEvade = (Byte)skill.weapon[2];
        play.defence.MagicalDefence = (Byte)skill.weapon[3];
        play.defence.MagicalEvade = (Byte)skill.weapon[4];
        play.cur.hp = skill.cur_hp;
        play.cur.mp = skill.cur_mp;
        play.max.hp = skill.max_hp;
        play.max.mp = skill.max_mp;
    }

    public static FF9PLAY_SKILL FF9Play_GetSkill(ref FF9PLAY_INFO info, ref FF9PLAY_SKILL skill)
    {
        Byte[] numArray1 = { 197, 198, 199, 200 };
        Byte[] numArray2 = { 50, 99, 99, 50 };
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
        Int32 index1;
        if ((index1 = info.equip[0]) != Byte.MaxValue)
            skill.weapon[0] = ff9weap.WeaponData[index1].Ref.Power;
        for (Int32 index2 = 0; index2 < 4; ++index2)
        {
            Int32 num;
            if ((num = info.equip[1 + index2]) != Byte.MaxValue && num >= 88 && num < 224)
            {
                ItemDefence defParams = ff9armor.ArmorData[num - 88];
                skill.weapon[1] += defParams.PhisicalDefence;
                skill.weapon[2] += defParams.PhisicalEvade;
                skill.weapon[3] += defParams.MagicalDefence;
                skill.weapon[4] += defParams.MagicalEvade;
            }
        }
        for (Int32 index2 = 0; index2 < 5; ++index2)
        {
            Int32 index3;
            if ((index3 = info.equip[index2]) != Byte.MaxValue)
            {
                FF9ITEM_DATA ff9ItemData = ff9item._FF9Item_Data[index3];
                ItemStats equipPrivilege = ff9equip.ItemStatsData[ff9ItemData.bonus];
                skill.Base[0] += equipPrivilege.dex;
                skill.Base[1] += equipPrivilege.str;
                skill.Base[2] += equipPrivilege.mgc;
                skill.Base[3] += equipPrivilege.wpr;
            }
        }
        for (Int32 index2 = 0; index2 < 4; ++index2)
        {
            if (ff9abil.FF9Abil_IsEnableSA(info.sa, numArray1[index2]))
            {
                switch (numArray1[index2])
                {
                    case 197:
                        skill.max_hp += skill.max_hp / 10U;
                        continue;
                    case 198:
                        skill.max_hp += skill.max_hp / 5U;
                        continue;
                    case 199:
                        skill.max_mp += skill.max_mp / 10U;
                        continue;
                    case 200:
                        skill.max_mp += skill.max_mp / 5U;
                        continue;
                    default:
                        continue;
                }
            }
        }
        for (Int32 index2 = 0; index2 < 4; ++index2)
        {
            if (skill.Base[index2] > numArray2[index2])
                skill.Base[index2] = numArray2[index2];
        }
        for (Int32 index2 = 0; index2 < 5; ++index2)
        {
            if (skill.weapon[index2] > 999)
                skill.weapon[index2] = 999;
        }
        if (skill.max_hp > 9999)
            skill.max_hp = 9999;
        if (skill.max_mp > 999)
            skill.max_mp = 999;
        if (skill.cur_hp > skill.max_hp)
            skill.cur_hp = skill.max_hp;
        if (skill.cur_mp > skill.max_mp)
            skill.cur_mp = skill.max_mp;
        return skill;
    }

    public static CharacterId FF9Play_GetCharID(CharacterPresetId presetId)
    {
        if (presetId >= 0 && presetId < 9)
        {
            Int32 id = presetId;
            return id;
        }

        if (presetId == CharacterPresetId.Marcus1 || presetId == CharacterPresetId.Marcus2 || presetId == CharacterPresetId.StageMarcus)
            return CharacterId.Marcus;
        if (presetId == CharacterPresetId.Blank1 || presetId == CharacterPresetId.Blank2 || presetId == CharacterPresetId.StageBlank)
            return CharacterId.Blank;
        if (presetId == CharacterPresetId.Cinna1 || presetId == CharacterPresetId.Cinna2 || presetId == CharacterPresetId.StageCinna)
            return CharacterId.Cinna;

        if (presetId == CharacterPresetId.Beatrix1 || presetId == CharacterPresetId.Beatrix2)
            return CharacterId.Beatrix;

        if (presetId == CharacterPresetId.StageZidane || presetId == CharacterPresetId.Zidane)
            return CharacterId.Zidane;

        throw new NotSupportedException(presetId.ToString());
    }

    public static CharacterId FF9Play_GetCharID2(CharacterIndex characterIndex, Boolean isSubCharacter)
    {
        Int32 index = characterIndex;
        if (index < CharacterIndex.Quina) // < 5
            return index;

        if (index == CharacterIndex.Beatrix) // == 8
            return CharacterId.Beatrix;

        if (index > CharacterIndex.Beatrix) // >> 8
            throw new NotSupportedException(characterIndex.ToString());

        // 5-7
        return isSubCharacter
            ? index + 3 // Sub characters
            : index; // Main characters
    }

    public static Int32 FF9Play_GetCategory(Byte menu_type)
    {
        return new Byte[] { 9, 5, 6, 5, 6, 5, 6, 5, 21, 21, 21, 22 }[FF9Play_GetCharID(menu_type)];
    }

    public static void FF9Play_SetFaceDirty(Boolean dirty)
    {
        _FF9Play_Face = dirty;
    }

    private Boolean FF9Play_GetFaceDirty()
    {
        return _FF9Play_Face;
    }
    
    public static List<Int32> GetDefaultEquipment(CharacterPresetId presetId)
    {
        CharacterId characterId = presetId.ToCharacterId();
        CharacterEquipment defaultSet = DefaultEquipment[characterId];

        CharacterIndex characterIndex = characterId.ToCharacterIndex();
        PLAYER player = FF9StateSystem.Common.FF9.player[characterIndex];
        CharacterEquipment currentSet = player.equip;

        List<Int32> defaultEquipment = new List<Int32>(5);
        
        for (Int32 i = 0; i < 5; i++)
        {
            if (defaultSet[i] == currentSet[i])
                defaultEquipment.Add(i);
        }

        return defaultEquipment;
    }

    public static void FF9Play_SetDefEquips(List<Int32> defaultEquipment, CharacterEquipment target, EquipmentSetId equipmentId)
    {
        CharacterEquipment newSet = DefaultEquipment[equipmentId];

        for (Int32 i = 0; i < 5; i++)
        {
            if (defaultEquipment.Contains(i))
                target[i] = newSet[i];
            else
                target.Change(i, newSet[i]);    
        }
    }

    public static Int32 FF9Play_GetSerialID(Byte slot_id, Boolean sub_pc, CharacterEquipment equip)
    {
        Int32 charId2 = FF9Play_GetCharID2(slot_id, sub_pc);
        Int32 num = ff9item._FF9Item_Data[equip[0]].shape;
        UInt16 uint16 = BitConverter.ToUInt16(FF9StateSystem.EventState.gEventGlobal, 0);
        switch (charId2)
        {
            case 0:
                return num == 1 ? 0 : 1;
            case 1:
                return 2;
            case 2:
                if (10300 <= uint16)
                    return num == 7 ? 6 : 5;
                return num == 7 ? 4 : 3;
            case 3:
                return 7;
            case 4:
                return 12;
            case 5:
                return 9;
            case 6:
                return num == 7 ? 11 : 10;
            case 7:
                return 13;
            case 8:
                return 14;
            case 9:
                return 15;
            case 10:
                return 1500 <= (Int32)uint16 && 1600 > (Int32)uint16 ? 17 : 16;
            case 11:
                return 18;
            default:
                return -1;
        }
    }

    public static Int32 FF9Play_GrowLevel(Int32 slot_id, Int32 lv)
    {
        Int32 num = FF9StateSystem.Common.FF9.player[slot_id].level;
        FF9Play_Build(slot_id, lv, null, true);
        return num;
    }

    public static Int32 FF9Play_ChangeLevel(Int32 slot_id, Int32 lv, Boolean max_hpmp)
    {
        PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
        Int32 num = player.level;
        player.level = 0;
        FF9Play_Build(slot_id, lv, null, false);
        if (max_hpmp)
        {
            player.cur.hp = player.max.hp;
            player.cur.mp = player.max.mp;
        }
        return num;
    }

    private static void FF9Play_UpdateSA(PLAYER play)
    {
        CharacterIndex characterIndex = play.Index;
        play.cur.capa = play.max.capa;
        for (Int32 index = 0; index < 64; ++index)
        {
            if (ff9abil.FF9Abil_GetEnableSA(characterIndex, 192 + index))
            {
                if (play.cur.capa >= ff9abil._FF9Abil_SaData[index].GemsCount)
                    play.cur.capa -= ff9abil._FF9Abil_SaData[index].GemsCount;
                else
                    ff9abil.FF9Abil_SetEnableSA(characterIndex, 192 + index, false);
            }
        }
    }

    public static void FF9Play_Change(Int32 slot_no, Boolean update_lv, List<Int32> defaultEquipment, EquipmentSetId characterId)
    {
        PLAYER play = FF9StateSystem.Common.FF9.player[slot_no];
        if (characterId != Byte.MaxValue)
            FF9Play_SetDefEquips(defaultEquipment, play.equip, characterId);
        
        play.info.serial_no = (Byte)FF9Play_GetSerialID(play.info.slot_no, play.IsSubCharacter, play.equip);
        if (!update_lv && characterId == Byte.MaxValue)
            return;
        
        if (update_lv)
        {
            Int32 avgLevel = FF9Play_GetAvgLevel(slot_no);
            Int32 lv = (Int32)play.info.serial_no == 10 || (Int32)play.info.serial_no == 11 ? avgLevel : Mathf.Max(play.level, avgLevel);
            FF9Play_Build(slot_no, lv, null, false);
        }
        else
        {
            FF9Play_Update(play);
        }
    }

    public static void FF9Dbg_SetCharacter(Int32 player, Int32 slot)
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
        if (player == Byte.MaxValue)
        {
            ff9StateGlobal.party.member[slot] = null;
        }
        else
        {
            FF9DBG_CHAR ff9DbgChar = ff9DbgCharArray[player];
            PLAYER play = ff9StateGlobal.party.member[slot] = ff9StateGlobal.player[ff9DbgChar.slot_no];
            if (ff9DbgChar.menu_type >= 0)
            {
                if (player != FF9Play_GetCharID(play.info.menu_type))
                    play.info.menu_type = (Byte)ff9DbgChar.menu_type;
                play.category = (Byte)FF9Play_GetCategory(play.info.menu_type);
                if (!FF9Dbg_CheckEquip(play))
                    FF9Play_SetDefEquips(new List<Int32> {0, 1, 2, 3, 4}, play.equip, play.DefaultEquipmentSetId);
                play.info.serial_no = (Byte)FF9Play_GetSerialID(play.info.slot_no, play.IsSubCharacter, play.equip);
            }
            play.category = (Byte)FF9Play_GetCategory(play.info.menu_type);
        }
    }

    private static Boolean FF9Dbg_CheckEquip(PLAYER play)
    {
        UInt16 num = (UInt16)(1 << (11 - (CharacterId)FF9Play_GetCharID2(play.Index, play.IsSubCharacter)));
        for (Int32 index1 = 0; index1 < 5; ++index1)
        {
            Int32 index2;
            if ((index2 = play.equip[index1]) != Byte.MaxValue && (ff9item._FF9Item_Data[index2].equip & num) == 0)
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