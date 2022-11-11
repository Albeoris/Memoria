using System;
using System.IO;
using Assets.SiliconSocial;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.CSV;
using Memoria.Prime.Text;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable RedundantExplicitArraySize

public class ff9item
{
    //public const ushort FF9ITEM_EQUIP_ZIDAN = 2048;
    //public const ushort FF9ITEM_EQUIP_VIVI = 1024;
    //public const ushort FF9ITEM_EQUIP_GARNET = 512;
    //public const ushort FF9ITEM_EQUIP_STEINER = 256;
    //public const ushort FF9ITEM_EQUIP_FREIJA = 128;
    //public const ushort FF9ITEM_EQUIP_KUINA = 64;
    //public const ushort FF9ITEM_EQUIP_EIKO = 32;
    //public const ushort FF9ITEM_EQUIP_SALAMANDER = 16;
    //public const ushort FF9ITEM_EQUIP_CINA = 8;
    //public const ushort FF9ITEM_EQUIP_MARCUS = 4;
    //public const ushort FF9ITEM_EQUIP_BLANK = 2;
    //public const ushort FF9ITEM_EQUIP_BEATRIX = 1;
    //public const byte FF9ITEM_TYPE_FIELD = 1;
    //public const byte FF9ITEM_TYPE_GIL = 2;
    //public const byte FF9ITEM_TYPE_POTION = 4;
    //public const byte FF9ITEM_TYPE_ACCESSORY = 8;
    //public const byte FF9ITEM_TYPE_BODY = 16;
    //public const byte FF9ITEM_TYPE_HEAD = 32;
    //public const byte FF9ITEM_TYPE_WRIST = 64;
    //public const byte FF9ITEM_TYPE_WEAPON = 128;
    //public const byte FF9ITEM_TYPE_EQUIP = 248;
    //public const int FF9ITEM_MAX = 256;
    //public const int FF9ITEM_COUNT_MAX = 99;
    //public const int FF9ITEM_RARE_MAX = 256;
    //public const int FF9ITEM_RARE_SIZE = 64;
    //public const int FF9ITEM_RARE_BIT = 2;
    //public const int FF9ITEM_ABILITY_MAX = 3;
    //public const int FF9ITEM_NONE = 255;
    //public const int FF9ITEM_INFO_START = 224;
    //public const int FF9ITEM_NAME_SIZE = 2048;
    //public const int FF9ITEM_HELP_SIZE = 10240;
    //public const int FF9ITEM_IMP_NAME_SIZE = 3072;

    public static FF9ITEM_DATA[] _FF9Item_Data;
    public static ITEM_DATA[] _FF9Item_Info;

    static ff9item()
    {
        _FF9Item_Data = LoadItems();
        _FF9Item_Info = LoadItemEffects();
        PatchItemEquipability(_FF9Item_Data);
    }

    public static void FF9Item_Init()
    {
        FF9Item_InitNormal();
        FF9Item_InitImportant();
        LoadInitialItems();
    }

    private static FF9ITEM_DATA[] LoadItems()
    {
        try
        {
            ItemInfo[] items;
            String inputPath = DataResources.Items.Directory + DataResources.Items.ItemsFile;
            String[] dir = Configuration.Mod.AllFolderNames;
            for (Int32 i = 0; i < dir.Length; i++)
            {
                inputPath = DataResources.Items.ModDirectory(dir[i]) + DataResources.Items.ItemsFile;
                if (File.Exists(inputPath))
                {
                    items = CsvReader.Read<ItemInfo>(inputPath);
                    if (items.Length != 256)
                        throw new NotSupportedException($"You must set 256 items, but there {items.Length}. Any number of items will be available after a game stabilization."); // TODO
                    FF9ITEM_DATA[] result = new FF9ITEM_DATA[items.Length];
                    for (Int32 j = 0; j < result.Length; j++)
                        result[j] = items[j].ToItemData();
                    return result;
                }
            }
            throw new FileNotFoundException($"[ff9item] Cannot load items because a file does not exist: [{inputPath}].", inputPath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ff9item] Load items failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    private static void PatchItemEquipability(FF9ITEM_DATA[] itemDatabase)
    {
        try
        {
            String inputPath;
            for (Int32 i = AssetManager.Folder.Length - 1; i >= 0; --i)
            {
                inputPath = DataResources.Items.ModDirectory(AssetManager.Folder[i].FolderPath) + DataResources.Items.ItemEquipPatchFile;
                if (File.Exists(inputPath))
                    ApplyItemEquipabilityPatchFile(itemDatabase, File.ReadAllLines(inputPath));
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ff9item] Patch item equipability failed.");
        }
    }

    private static ITEM_DATA[] LoadItemEffects()
    {
        try
        {
            ItemEffect[] effects;
            String inputPath = DataResources.Items.Directory + DataResources.Items.ItemEffectsFile;
            String[] dir = Configuration.Mod.AllFolderNames;
            for (Int32 i = 0; i < dir.Length; i++)
            {
                inputPath = DataResources.Items.ModDirectory(dir[i]) + DataResources.Items.ItemEffectsFile;
                if (File.Exists(inputPath))
                {
                    effects = CsvReader.Read<ItemEffect>(inputPath);
                    if (effects.Length != 32)
                        throw new NotSupportedException($"You must set 32 actions, but there {effects.Length}. Any number of actions will be available after a game stabilization."); // TODO
                    ITEM_DATA[] result = new ITEM_DATA[effects.Length];
                    for (Int32 j = 0; j < result.Length; j++)
                        result[j] = effects[j].ToItemData();
                    return result;
                }
            }
            throw new FileNotFoundException($"[ff9item] Cannot load item actions because a file does not exist: [{inputPath}].", inputPath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ff9item] Load item effects failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    private static void LoadInitialItems()
    {
        try
        {
            String[] dir = Configuration.Mod.AllFolderNames;
            for (Int32 i = 0; i < dir.Length; i++)
            {
                String inputPath = DataResources.Items.ModDirectory(dir[i]) + DataResources.Items.InitialItemsFile;
                if (File.Exists(inputPath))
                {
                    FF9ITEM[] items = CsvReader.Read<FF9ITEM>(inputPath);
                    foreach (FF9ITEM item in items)
                        FF9Item_Add(item.id, item.count);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ff9item] Load initial items failed.");
        }
    }

    public static void FF9Item_InitNormal()
    {
        for (Int32 index = 0; index < 256; ++index)
            FF9StateSystem.Common.FF9.item[index] = new FF9ITEM(0, 0);
    }

    public static void FF9Item_InitImportant()
    {
        for (Int32 index = 0; index < 64; ++index)
            FF9StateSystem.Common.FF9.rare_item[index] = 0;
    }

    public static FF9ITEM FF9Item_GetPtr(Int32 id)
    {
        FF9ITEM[] ff9ItemArray = FF9StateSystem.Common.FF9.item;
        for (Int32 index = 0; index < 256; ++index)
        {
            FF9ITEM ff9Item = ff9ItemArray[index];
            if (ff9Item.count != 0 && ff9Item.id == id)
                return ff9Item;
        }
        return null;
    }

    public static Int32 FF9Item_GetCount(Int32 id)
    {
        FF9ITEM ptr = FF9Item_GetPtr(id);
        if (ptr == null)
            return 0;
        return ptr.count;
    }

    public static Int32 FF9Item_Add(Int32 id, Int32 count)
    {
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        if (id == Byte.MaxValue)
            return 0;
        Int32 index1 = 0;
        FF9ITEM[] ff9ItemArray1 = ff9StateGlobal.item;
        for (; index1 < 256; ++index1)
        {
            FF9ITEM ff9Item = ff9ItemArray1[index1];
            if (ff9Item.count != 0 && ff9Item.id == id)
            {
                if (ff9Item.count + count > 99)
                    count = 99 - ff9Item.count;
                ff9Item.count += (Byte)count;
                FF9Item_Achievement(ff9Item.id, ff9Item.count);
                return count;
            }
        }
        Int32 index2 = 0;
        FF9ITEM[] ff9ItemArray2 = ff9StateGlobal.item;
        for (; index2 < 256; ++index2)
        {
            FF9ITEM ff9Item = ff9ItemArray2[index2];
            if (ff9Item.count == 0)
            {
                ff9Item.id = (Byte)id;
                ff9Item.count = (Byte)count;
                FF9Item_Achievement(ff9Item.id, ff9Item.count);
                return count;
            }
        }
        return 0;
    }

    public static Int32 FF9Item_Remove(Int32 id, Int32 count)
    {
        FF9ITEM[] ff9ItemArray = FF9StateSystem.Common.FF9.item;
        for (Int32 index = 0; index < 256; ++index)
        {
            FF9ITEM ff9Item = ff9ItemArray[index];
            if (ff9Item.count != 0 && ff9Item.id == id)
            {
                if (ff9Item.count < count)
                    count = ff9Item.count;
                ff9Item.count -= (Byte)count;
                return count;
            }
        }
        return 0;
    }

    public static Int32 FF9Item_GetEquipPart(Int32 id)
    {
        FF9ITEM_DATA ff9ItemData = _FF9Item_Data[id];
        Byte[] numArray = new Byte[5]
        {
            128,
            32,
            64,
            16,
            8
        };
        for (Int32 index = 0; index < 5; ++index)
        {
            if ((ff9ItemData.type & numArray[index]) != 0)
                return index;
        }
        return -1;
    }

    public static Int32 FF9Item_GetEquipCount(Int32 id)
    {
        Int32 count = 0;
        foreach (PLAYER p in FF9StateSystem.Common.FF9.PlayerList)
            if (p.info.party != 0)
                for (Int32 i = 0; i < 5; ++i)
                    if (id == p.equip[i])
                        ++count;
        return count;
    }

    public static void FF9Item_AddImportant(Int32 id)
    {
        FF9StateSystem.Common.FF9.rare_item[id >> 2] |= (Byte)(1 << (((Byte)id & 3) << 1));
        FF9StateSystem.Common.FF9.rare_item[id >> 2] &= (Byte)~(1 << (((Byte)id & 3) << 1) + 1);
    }

    public static void FF9Item_RemoveImportant(Int32 id)
    {
        FF9StateSystem.Common.FF9.rare_item[id >> 2] &= (Byte)~(1 << (((Byte)id & 3) << 1));
        FF9StateSystem.Common.FF9.rare_item[id >> 2] &= (Byte)~(1 << (((Byte)id & 3) << 1) + 1);
    }

    public static void FF9Item_UseImportant(Int32 id)
    {
        FF9StateSystem.Common.FF9.rare_item[id >> 2] |= (Byte)(1 << (((Byte)id & 3) << 1) + 1);
    }

    public static void FF9Item_UnuseImportant(Int32 id)
    {
        FF9StateSystem.Common.FF9.rare_item[id >> 2] &= (Byte)~(1 << (((Byte)id & 3) << 1) + 1);
    }

    public static Boolean FF9Item_IsExistImportant(Int32 id)
    {
        return (FF9StateSystem.Common.FF9.rare_item[(Byte)id >> 2] & (Byte)(1 << (((Byte)id & 3) << 1))) != 0;
    }

    public static Boolean FF9Item_IsUsedImportant(Int32 id)
    {
        return (FF9StateSystem.Common.FF9.rare_item[(Byte)id >> 2] & (Byte)(1 << (((Byte)id & 3) << 1) + 1)) != 0;
    }

    private static void FF9Item_Achievement(Int32 id, Int32 count)
    {
        Int32 num = id;
        switch (num)
        {
            case 28:
                AchievementManager.ReportAchievement(AcheivementKey.Excalibur, count);
                break;
            case 30:
                AchievementManager.ReportAchievement(AcheivementKey.ExcaliburII, count);
                break;
            default:
                if (num == 14)
                {
                    AchievementManager.ReportAchievement(AcheivementKey.TheTower, count);
                    break;
                }
                if (num == 15)
                {
                    AchievementManager.ReportAchievement(AcheivementKey.UltimaWeapon, count);
                    break;
                }
                if (num == 0)
                {
                    AchievementManager.ReportAchievement(AcheivementKey.Hammer, count);
                    break;
                }
                if (num == 39)
                {
                    AchievementManager.ReportAchievement(AcheivementKey.KainLance, count);
                    break;
                }
                if (num == 50)
                {
                    AchievementManager.ReportAchievement(AcheivementKey.RuneClaws, count);
                    break;
                }
                if (num == 56)
                {
                    AchievementManager.ReportAchievement(AcheivementKey.TigerHands, count);
                    break;
                }
                if (num == 63)
                {
                    AchievementManager.ReportAchievement(AcheivementKey.WhaleWhisker, count);
                    break;
                }
                if (num == 69)
                {
                    AchievementManager.ReportAchievement(AcheivementKey.AngelFlute, count);
                    break;
                }
                if (num == 78)
                {
                    AchievementManager.ReportAchievement(AcheivementKey.MaceOfZeus, count);
                    break;
                }
                if (num == 84)
                {
                    AchievementManager.ReportAchievement(AcheivementKey.GastroFork, count);
                    break;
                }
                if (num == 109 || num == 146 || num == 189)
                {
                    if (FF9Item_GetCount(109) <= 0 && FF9Item_GetEquipCount(109) <= 0 || FF9Item_GetCount(146) <= 0 && FF9Item_GetEquipCount(146) <= 0 || FF9Item_GetCount(189) <= 0 && FF9Item_GetEquipCount(189) <= 0)
                        break;
                    count = FF9Item_GetCount(109) + FF9Item_GetEquipCount(109) + FF9Item_GetCount(146) + FF9Item_GetEquipCount(146) + FF9Item_GetCount(189) + FF9Item_GetEquipCount(189);
                    AchievementManager.ReportAchievement(AcheivementKey.GenjiSet, count);
                    break;
                }
                if (num != 229)
                    break;
                AchievementManager.ReportAchievement(AcheivementKey.Moonstone4, IncreaseMoonStoneCount());
                break;
        }
    }

    public static Int32 IncreaseMoonStoneCount()
    {
        Int32 num = FF9StateSystem.Achievement.EvtReservedArray[0] + 1;
        FF9StateSystem.Achievement.EvtReservedArray[0] = num;
        return num;
    }

    public static Int32 DecreaseMoonStoneCount()
    {
        Int32 num = FF9StateSystem.Achievement.EvtReservedArray[0] - 1;
        FF9StateSystem.Achievement.EvtReservedArray[0] = num;
        return num;
    }

    private static void ApplyItemEquipabilityPatchFile(FF9ITEM_DATA[] itemDatabase, String[] allLines)
	{
        foreach (String line in allLines)
        {
            // eg.: Garnet Add 1 2 Remove 56
            // (allows Garnet to equip Dagger and Mage Masher but disallows her to equip Tiger Racket)
            if (String.IsNullOrEmpty(line) || line.Trim().StartsWith("//"))
                continue;
            String[] allWords = line.Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (allWords.Length < 3)
                continue;
            Int32 charId;
            if (!Int32.TryParse(allWords[0], out charId))
            {
                CharacterId charIdAsStr;
                if (!allWords[0].TryEnumParse(out charIdAsStr))
                    continue;
                charId = (Int32)charIdAsStr;
            }
            Boolean isAdd = false;
            Boolean isRemove = false;
            Int32 itemId;
            UInt64 charMask = ff9feqp.GetCharacterEquipMaskFromId((CharacterId)charId);
            for (Int32 wordIndex = 1; wordIndex < allWords.Length; wordIndex++)
			{
                String word = allWords[wordIndex].Trim();
                if (String.Compare(word, "Add") == 0)
				{
                    isAdd = true;
                    isRemove = false;
                }
                else if (String.Compare(word, "Remove") == 0)
                {
                    isAdd = false;
                    isRemove = true;
                }
                else if (isAdd)
                {
                    if (!Int32.TryParse(word, out itemId) || itemId < 0 || itemId >= itemDatabase.Length)
                        continue;
                    itemDatabase[itemId].equip |= charMask;
                }
                else if (isRemove)
                {
                    if (!Int32.TryParse(word, out itemId) || itemId < 0 || itemId >= itemDatabase.Length)
                        continue;
                    itemDatabase[itemId].equip &= ~charMask;
                }
            }
        }
    }
}