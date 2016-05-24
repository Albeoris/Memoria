using Assets.Sources.Scripts.UI.Common;
using FF9;
using System;
using Memoria;
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

[ExportedType("ęďěĖńńńń,!!!ğ¯ïďb'á<čĒ1ÜĪàI}9×ý®µXăĖWÝįh)Ý0{·nĚ®ãåDd=!!!ĄÀļěę5ĦÍÍs±ûb¥ĲêZıÅġĬ=ÕòĂNø°ġ¢Ëâ/®Aĥc÷ð9āYNÚ2ā-ĤîĄ»©ġ?ęgìÊĮ³Oćª[*ĞvåýĮÅ­Ĉđ{ĭ£įÕI¾3ČĶĆãÀľĞ¦8Ò¯éđ×+W>ī<d^@$¯û§#!!!Ì»àÔńńńń&!!!jđS9Ķ8ıÅ0ĝMª,g¹Ò#!!!łĹÙ-ńńńń")]
public class ff9play
{
    public const int FF9PLAY_HP_MAX = 9999;
    public const int FF9PLAY_MP_MAX = 999;
    public const int FF9PLAY_WEAPON_VAL_MAX = 999;
    public const int FF9PLAY_DEF_LEVEL = 1;
    public const int FF9PLAY_DEF_GIL = 500;
    public const int FF9PLAY_DEF_AT = 10;
    public const int FF9PLAY_NAME_MAX = 10;
    public const int FF9PLAY_FLD_STATUS_MAX = 7;
    public const int FF9PLAY_EQUIP_ID_NONE = 255;
    private static bool _FF9Play_Face;

    public ff9play()
    {
    }

    public static void FF9Play_Init()
    {
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        FF9Play_SetFaceDirty(false);
        for (int slot_id = 0; slot_id < 9; ++slot_id)
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

    public static void FF9Play_New(int slot_id)
    {
        PLAYER play = FF9StateSystem.Common.FF9.player[slot_id];
        PLAYER_INFO[] playerInfoArray =
        {
            new PLAYER_INFO(0, 1, 1, 1, 0, 0),
            new PLAYER_INFO(1, 2, 0, 1, 0, 1),
            new PLAYER_INFO(2, 3, 0, 1, 0, 2),
            new PLAYER_INFO(3, 7, 1, 1, 0, 3),
            new PLAYER_INFO(4, 12, 0, 1, 0, 4),
            new PLAYER_INFO(5, 14, 1, 1, 0, 8),
            new PLAYER_INFO(6, 15, 1, 1, 0, 10),
            new PLAYER_INFO(7, 16, 1, 1, 0, 12),
            new PLAYER_INFO(8, 18, 1, 1, 0, 14)
        };
        play.info = playerInfoArray[slot_id];
        play.status = 0;
        play.category = (byte)FF9Play_GetCategory(play.info.menu_type);
        play.bonus = new FF9LEVEL_BONUS();
        int charId3 = FF9Play_GetCharID3(play);
        play.name = FF9TextTool.CharacterDefaultName(play.info.menu_type);
        FF9Play_SetDefEquips(play.equip, charId3);
        play.info.serial_no = (byte)FF9Play_GetSerialID(play.info.slot_no, (play.category & 16) != 0, play.equip);
        FF9Play_Build(slot_id, 1, playerInfoArray[slot_id], false);
        play.cur.hp = play.max.hp;
        play.cur.mp = play.max.mp;
        play.cur.capa = play.max.capa;
    }

    public static void FF9Play_SetParty(int party_id, int slot_id)
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

    public static int FF9Play_GetPrev(int id)
    {
        do
        {
            id = id == 0 ? 3 : id - 1;
        } while (FF9StateSystem.Common.FF9.party.member[id] == null);
        return id;
    }

    public static int FF9Play_GetNext(int id)
    {
        do
        {
            id = id >= 3 ? 0 : id + 1;
        } while (FF9StateSystem.Common.FF9.party.member[id] == null);
        return id;
    }

    private static int FF9Play_GetAvgLevel(int slot_id)
    {
        int num1 = 0;
        int num2 = 0;
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        for (int index = 0; index < 9; ++index)
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

    public static void FF9Play_Add(int slot_id)
    {
        if (TryHackCharacterAvailability())
            return;

        if (slot_id < 0 || slot_id >= 9)
            return;
        FF9StateSystem.Common.FF9.player[slot_id].info.party = 1;
    }

    public static void FF9Play_Delete(int slot_id)
    {
        if (TryHackCharacterAvailability())
            return;

        if (slot_id < 0 || slot_id >= 9)
            return;
        FF9StateSystem.Common.FF9.player[slot_id].info.party = 0;
    }

    internal static bool TryHackCharacterAvailability()
    {
        if (!Configuration.Hacks.IsAllCharactersAvailable)
            return false;

        for (byte i = 0; i < 9; i++)
            FF9StateSystem.Common.FF9.player[i].info.party = 1;

        return true;
    }

    public static void FF9Play_Build(int slot_id, int lv, PLAYER_INFO init, bool lvup)
    {
        PLAYER play = FF9StateSystem.Common.FF9.player[slot_id];
        if (!lvup && play.level != lv)
            play.exp = (uint)ff9level._FF9Level_Exp[lv - 1];
        play.level = (byte)lv;
        long num1 = play.max.capa - play.cur.capa;
        long num2 = play.cur.hp;
        long num3 = play.cur.mp;
        long num4 = play.cur.capa;
        long num5 = play.max.capa;
        play.cur = new POINTS();
        play.max = new POINTS();
        play.cur.at_coef = 10;
        play.basis.dex = (byte)ff9level.FF9Level_GetDex(slot_id, play.level, lvup);
        play.basis.str = (byte)ff9level.FF9Level_GetStr(slot_id, play.level, lvup);
        play.basis.mgc = (byte)ff9level.FF9Level_GetMgc(slot_id, play.level, lvup);
        play.basis.wpr = (byte)ff9level.FF9Level_GetWpr(slot_id, play.level, lvup);
        play.max.hp = (ushort)ff9level.FF9Level_GetHp(play.level, play.basis.str);
        play.basis.max_hp = (short)play.max.hp;
        play.max.mp = play.basis.max_mp = (short)ff9level.FF9Level_GetMp(play.level, play.basis.mgc);
        play.max.capa = (byte)ff9level.FF9Level_GetCap(slot_id, play.level, lvup);
        play.cur.hp = (ushort)ccommon.min((int)num2, play.max.hp);
        play.cur.mp = (short)ccommon.min((int)num3, play.max.mp);
        play.cur.capa = (byte)(play.max.capa - (ulong)num1);
        if (init != null)
        {
            play.cur.hp = play.max.hp;
            play.cur.mp = play.max.mp;
            play.cur.capa = play.max.capa;
        }
        FF9Play_Update(play);
        if (lvup)
        {
            play.cur.hp = (ushort)ccommon.min((int)num2, play.max.hp);
            play.cur.mp = (short)ccommon.min((int)num3, play.max.mp);
        }
        if (num5 == 99L)
        {
            play.cur.capa = (byte)num4;
            play.max.capa = (byte)num5;
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
        info.cur_mp = (ushort)play.cur.mp;
        for (int index = 0; index < 1; ++index)
            info.sa[index] = play.sa[index];
        for (int index = 0; index < 5; ++index)
            info.equip[index] = play.equip[index];
        FF9Play_GetSkill(ref info, ref skill);
        play.elem.dex = skill.Base[0];
        play.elem.str = skill.Base[1];
        play.elem.mgc = skill.Base[2];
        play.elem.wpr = skill.Base[3];
        play.defence.p_def = (byte)skill.weapon[1];
        play.defence.p_ev = (byte)skill.weapon[2];
        play.defence.m_def = (byte)skill.weapon[3];
        play.defence.m_ev = (byte)skill.weapon[4];
        play.cur.hp = skill.cur_hp;
        play.cur.mp = (short)skill.cur_mp;
        play.max.hp = skill.max_hp;
        play.max.mp = (short)skill.max_mp;
    }

    public static FF9PLAY_SKILL FF9Play_GetSkill(ref FF9PLAY_INFO info, ref FF9PLAY_SKILL skill)
    {
        byte[] numArray1 = {197, 198, 199, 200};
        byte[] numArray2 = {50, 99, 99, 50};
        skill = new FF9PLAY_SKILL
        {
            cur_hp = info.cur_hp,
            cur_mp = info.cur_mp,
            max_hp = (ushort)info.Base.max_hp,
            max_mp = (ushort)info.Base.max_mp,
            Base =
            {
                [0] = info.Base.dex,
                [1] = info.Base.str,
                [2] = info.Base.mgc,
                [3] = info.Base.wpr
            }
        };
        int index1;
        if ((index1 = info.equip[0]) != byte.MaxValue)
            skill.weapon[0] = ff9weap._FF9Weapon_Data[index1].Ref.power;
        for (int index2 = 0; index2 < 4; ++index2)
        {
            int num;
            if ((num = info.equip[1 + index2]) != byte.MaxValue && num >= 88 && num < 224)
            {
                DEF_PARAMS defParams = ff9armor._FF9Armor_Data[num - 88];
                skill.weapon[1] += defParams.p_def;
                skill.weapon[2] += defParams.p_ev;
                skill.weapon[3] += defParams.m_def;
                skill.weapon[4] += defParams.m_ev;
            }
        }
        for (int index2 = 0; index2 < 5; ++index2)
        {
            int index3;
            if ((index3 = info.equip[index2]) != byte.MaxValue)
            {
                FF9ITEM_DATA ff9ItemData = ff9item._FF9Item_Data[index3];
                EQUIP_PRIVILEGE equipPrivilege = ff9equip._FF9EquipBonus_Data[ff9ItemData.bonus];
                skill.Base[0] += equipPrivilege.dex;
                skill.Base[1] += equipPrivilege.str;
                skill.Base[2] += equipPrivilege.mgc;
                skill.Base[3] += equipPrivilege.wpr;
            }
        }
        for (int index2 = 0; index2 < 4; ++index2)
        {
            if (ff9abil.FF9Abil_IsEnableSA(info.sa, numArray1[index2]))
            {
                switch (numArray1[index2])
                {
                    case 197:
                        skill.max_hp = (ushort)(skill.max_hp + skill.max_hp / 10U);
                        continue;
                    case 198:
                        skill.max_hp = (ushort)(skill.max_hp + skill.max_hp / 5U);
                        continue;
                    case 199:
                        skill.max_mp = (ushort)(skill.max_mp + skill.max_mp / 10U);
                        continue;
                    case 200:
                        skill.max_mp = (ushort)(skill.max_mp + skill.max_mp / 5U);
                        continue;
                    default:
                        continue;
                }
            }
        }
        for (int index2 = 0; index2 < 4; ++index2)
        {
            if (skill.Base[index2] > numArray2[index2])
                skill.Base[index2] = numArray2[index2];
        }
        for (int index2 = 0; index2 < 5; ++index2)
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

    public static int FF9Play_GetCharID(int menu_type)
    {
        switch (menu_type)
        {
            case 0:
            case 16:
                return 0;
            case 1:
                return 1;
            case 2:
                return 2;
            case 3:
                return 3;
            case 4:
                return 4;
            case 5:
                return 5;
            case 6:
                return 6;
            case 7:
                return 7;
            case 8:
            case 9:
            case 17:
                return 8;
            case 10:
            case 11:
            case 18:
                return 9;
            case 12:
            case 13:
            case 19:
                return 10;
            case 14:
            case 15:
                return 11;
            default:
                return -1;
        }
    }

    public static int FF9Play_GetCharID2(int slot_id, bool sub_pc)
    {
        byte[][] numArray1 = new byte[9][];
        numArray1[0] = new byte[2];
        int index1 = 1;
        byte[] numArray2 = {1, 1};
        numArray1[index1] = numArray2;
        int index2 = 2;
        byte[] numArray3 = {2, 2};
        numArray1[index2] = numArray3;
        int index3 = 3;
        byte[] numArray4 =
        {
            3, 3
        };
        numArray1[index3] = numArray4;
        int index4 = 4;
        byte[] numArray5 = {4, 4};
        numArray1[index4] = numArray5;
        int index5 = 5;
        byte[] numArray6 = {5, 8};
        numArray1[index5] = numArray6;
        int index6 = 6;
        byte[] numArray7 = {6, 9};
        numArray1[index6] = numArray7;
        int index7 = 7;
        byte[] numArray8 = {7, 10};
        numArray1[index7] = numArray8;
        int index8 = 8;
        byte[] numArray9 = {11, 11};
        numArray1[index8] = numArray9;
        return numArray1[slot_id][!sub_pc ? 0 : 1];
    }

    public static int FF9Play_GetCharID3(PLAYER play)
    {
        byte[][] numArray1 = new byte[9][];
        numArray1[0] = new byte[2];
        int index1 = 1;
        byte[] numArray2 = {1, 1};
        numArray1[index1] = numArray2;
        int index2 = 2;
        byte[] numArray3 = {2, 2};
        numArray1[index2] = numArray3;
        int index3 = 3;
        byte[] numArray4 = {3, 3};
        numArray1[index3] = numArray4;
        int index4 = 4;
        byte[] numArray5 = {4, 4};
        numArray1[index4] = numArray5;
        int index5 = 5;
        byte[] numArray6 = {5, 8};
        numArray1[index5] = numArray6;
        int index6 = 6;
        byte[] numArray7 = {6, 9};
        numArray1[index6] = numArray7;
        int index7 = 7;
        byte[] numArray8 =
        {
            7, 10
        };
        numArray1[index7] = numArray8;
        int index8 = 8;
        byte[] numArray9 = {11, 11};
        numArray1[index8] = numArray9;
        return numArray1[play.info.slot_no][((int)play.category & 16) == 0 ? 0 : 1];
    }

    public static int FF9Play_GetCategory(int menu_type)
    {
        return new byte[] {9, 5, 6, 5, 6, 5, 6, 5, 21, 21, 21, 22}[FF9Play_GetCharID(menu_type)];
    }

    public static void FF9Play_SetFaceDirty(bool dirty)
    {
        _FF9Play_Face = dirty;
    }

    private bool FF9Play_GetFaceDirty()
    {
        return _FF9Play_Face;
    }

    public static void FF9Play_SetDefEquips(byte[] equip, int eqp_id)
    {
        byte[][] numArray =
        {
            new byte[] {1, 112, 88, 149, byte.MaxValue},
            new byte[] {70, 112, byte.MaxValue, 149, byte.MaxValue},
            new byte[] {57, byte.MaxValue, byte.MaxValue, 150, byte.MaxValue},
            new byte[] {16, 137, byte.MaxValue, 177, byte.MaxValue},
            new byte[] {31, 136, 102, 178, byte.MaxValue},
            new byte[] {79, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue},
            new byte[] {64, 114, 90, 150, 232},
            new byte[] {41, byte.MaxValue, 89, 155, 194},
            new byte[] {0, 112, byte.MaxValue, byte.MaxValue, byte.MaxValue},
            new byte[] {16, 112, 88, 149, byte.MaxValue},
            new byte[] {16, byte.MaxValue, byte.MaxValue, 150, byte.MaxValue},
            new byte[] {26, 138, 104, 179, 192},
            new byte[] {17, 114, 88, 151, byte.MaxValue},
            new byte[] {26, 142, 105, 181, 212},
            new byte[] {17, 112, byte.MaxValue, 150, byte.MaxValue},
            new[] {byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue}
        };
        for (int index = 0; index < 5; ++index)
            equip[index] = numArray[eqp_id][index];
    }

    public static int FF9Play_GetSerialID(int slot_id, bool sub_pc, byte[] equip)
    {
        int charId2 = FF9Play_GetCharID2(slot_id, sub_pc);
        int num = ff9item._FF9Item_Data[equip[0]].shape;
        ushort uint16 = BitConverter.ToUInt16(FF9StateSystem.EventState.gEventGlobal, 0);
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
                return 1500 <= (int)uint16 && 1600 > (int)uint16 ? 17 : 16;
            case 11:
                return 18;
            default:
                return -1;
        }
    }

    public static int FF9Play_GrowLevel(int slot_id, int lv)
    {
        int num = FF9StateSystem.Common.FF9.player[slot_id].level;
        FF9Play_Build(slot_id, lv, null, true);
        return num;
    }

    public static int FF9Play_ChangeLevel(int slot_id, int lv, bool max_hpmp)
    {
        PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
        int num = player.level;
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
        int slot_id = play.info.slot_no;
        play.cur.capa = play.max.capa;
        for (int index = 0; index < 64; ++index)
        {
            if (ff9abil.FF9Abil_GetEnableSA(slot_id, 192 + index))
            {
                if (play.cur.capa >= ff9abil._FF9Abil_SaData[index].capa_val)
                    play.cur.capa -= ff9abil._FF9Abil_SaData[index].capa_val;
                else
                    ff9abil.FF9Abil_SetEnableSA(slot_id, 192 + index, false);
            }
        }
    }

    public static void FF9Play_Change(int slot_no, bool update_lv, int eqp_id)
    {
        PLAYER play = FF9StateSystem.Common.FF9.player[slot_no];
        if (eqp_id != byte.MaxValue)
            FF9Play_SetDefEquips(play.equip, eqp_id);
        play.info.serial_no = (byte)FF9Play_GetSerialID(play.info.slot_no, (play.category & 16) != 0, play.equip);
        if (!update_lv && eqp_id == byte.MaxValue)
            return;
        if (update_lv)
        {
            int avgLevel = FF9Play_GetAvgLevel(slot_no);
            int lv = (int)play.info.serial_no == 10 || (int)play.info.serial_no == 11 ? avgLevel : Mathf.Max(play.level, avgLevel);
            FF9Play_Build(slot_no, lv, null, false);
        }
        else
            FF9Play_Update(play);
    }

    private static void FF9Play_DebugEquip(int slot_no)
    {
        PLAYER play = FF9StateSystem.Common.FF9.player[slot_no];
        byte[][] numArray =
        {
            new byte[] {15, 135, 101, 167, 192},
            new byte[] {78, 135, 101, 175, 192},
            new byte[] {63, 135, 101, 175, 192},
            new byte[] {30, 147, 111, 191, 192},
            new byte[] {40, 147, 111, 191, 192},
            new byte[] {84, 135, 101, 175, 192},
            new byte[] {69, 135, 101, 175, 232},
            new byte[] {50, 135, 101, 167, 194},
            new byte[] {0, 135, 101, 167, 192},
            new byte[] {25, 135, 101, 167, 192},
            new byte[] {25, 135, 101, 167, 192},
            new byte[] {26, 147, 111, 191, 192}
        };
        int charId3 = FF9Play_GetCharID3(play);
        for (int index = 0; index < 5; ++index)
            play.equip[index] = numArray[charId3][index];
        play.info.serial_no = (byte)FF9Play_GetSerialID(play.info.slot_no, (play.category & 16) != 0, play.equip);
        FF9Play_Update(play);
        FF9Play_UpdateSA(play);
    }

    public static void FF9Dbg_SetCharacter(int player, int slot)
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
        if (player == byte.MaxValue)
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
                    play.info.menu_type = (byte)ff9DbgChar.menu_type;
                play.category = (byte)FF9Play_GetCategory(play.info.menu_type);
                if (!FF9Dbg_CheckEquip(play))
                    FF9Play_SetDefEquips(play.equip, FF9Play_GetCharID3(play));
                play.info.serial_no = (byte)FF9Play_GetSerialID(play.info.slot_no, (play.category & 16) != 0, play.equip);
            }
            play.category = (byte)FF9Play_GetCategory(play.info.menu_type);
        }
    }

    private static bool FF9Dbg_CheckEquip(PLAYER play)
    {
        ushort num = (ushort)(1 << (11 - FF9Play_GetCharID3(play)));
        for (int index1 = 0; index1 < 5; ++index1)
        {
            int index2;
            if ((index2 = play.equip[index1]) != byte.MaxValue && (ff9item._FF9Item_Data[index2].equip & num) == 0)
                return false;
        }
        return true;
    }

    public class FF9DBG_CHAR
    {
        public byte slot_no;
        public bool sub;
        public sbyte menu_type;
        public byte pad;

        public FF9DBG_CHAR(byte slot_no, bool sub, sbyte menu_type)
        {
            this.slot_no = slot_no;
            this.sub = sub;
            this.menu_type = menu_type;
        }
    }
}