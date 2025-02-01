﻿using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.IO;

namespace FF9
{
    public static class ff9buy
    {
        public const ItemType FF9BUY_TYPE_WEAPON = ItemType.Weapon | ItemType.Armlet | ItemType.Helmet | ItemType.Armor;

        public static readonly Dictionary<Int32, ShopItems> ShopItems;

        static ff9buy()
        {
            ShopItems = LoadShopItems();
            PatchShops();
        }

        private static Dictionary<Int32, ShopItems> LoadShopItems()
        {
            try
            {
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.ShopItems;
                Dictionary<Int32, ShopItems> result = new Dictionary<Int32, ShopItems>();
                foreach (ShopItems[] shops in AssetManager.EnumerateCsvFromLowToHigh<ShopItems>(inputPath))
                    foreach (ShopItems shop in shops)
                        result[shop.Id] = shop;
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load shop items because a file does not exist: [{DataResources.Items.Directory + DataResources.Items.ShopItems}].", DataResources.Items.Directory + DataResources.Items.ShopItems);
                for (Int32 i = 0; i < 32; i++)
                    if (!result.ContainsKey(i))
                        throw new NotSupportedException($"You must define at least 32 shop items, with IDs between 0 and 31.");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9buy] Load shop items failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        private static void PatchShops()
        {
            try
            {
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.ShopPatchFile;
                foreach (AssetManager.AssetFolder folder in AssetManager.FolderLowToHigh)
                    if (folder.TryFindAssetInModOnDisc(inputPath, out String fullPath, AssetManagerUtil.GetStreamingAssetsPath() + "/"))
                        ApplyShopPatchFile(File.ReadAllLines(fullPath));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9buy] Patch shops failed.");
            }
        }

        public static ShopUI.ShopType FF9Buy_GetType(Int32 shopId)
        {
            if (!ShopItems.TryGetValue(shopId, out ShopItems assortiment))
                return ShopUI.ShopType.Synthesis;

            for (Int32 i = 0; i < assortiment.Length; i++)
                if ((ff9item._FF9Item_Data[assortiment[i]].type & FF9BUY_TYPE_WEAPON) == 0)
                    return ShopUI.ShopType.Item;

            return ShopUI.ShopType.Weapon;
        }

        private static void ApplyShopPatchFile(String[] allLines)
        {
            foreach (String line in allLines)
            {
                // eg.: 0 AddItem 31 32 RemoveItem 57
                // (Add to Dali's shop the Javelin and Mythril Spear, and remove Rod from it)
                // or: 32 AddSynth 6 RemoveSynth 0 1
                // (Add to Linblum's first synthesis shop the Masamune recipe, and remove Butterfly Sword and The Ogre recipes from it)
                // Note: items are automatically inserted after the other items of the same type; it might become more customisable in the future
                if (String.IsNullOrEmpty(line) || line.Trim().StartsWith("//"))
                    continue;
                String[] allWords = line.Trim().Split(DataPatchers.SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (allWords.Length < 3)
                    continue;
                if (!Int32.TryParse(allWords[0], out Int32 shopId))
                    continue;
                if (!ShopItems.TryGetValue(shopId, out ShopItems assortiment))
                {
                    assortiment = new ShopItems(shopId);
                    ShopItems[shopId] = assortiment;
                }
                Int32 currentOperation = -1;
                for (Int32 wordIndex = 1; wordIndex < allWords.Length; wordIndex++)
                {
                    String word = allWords[wordIndex].Trim();
                    if (String.Equals(word, "AddItem"))
                        currentOperation = 0;
                    else if (String.Equals(word, "RemoveItem"))
                        currentOperation = 1;
                    else if (String.Equals(word, "AddSynth"))
                        currentOperation = 2;
                    else if (String.Equals(word, "RemoveSynth"))
                        currentOperation = 3;
                    else if (currentOperation == 0 || currentOperation == 1)
                    {
                        if (!Int32.TryParse(word, out Int32 itemId) || !ff9item._FF9Item_Data.TryGetValue((RegularItem)itemId, out FF9ITEM_DATA item))
                            continue;
                        if (currentOperation == 0)
                        {
                            Int32 insertPos = assortiment.ItemIds.FindIndex(id => ff9item._FF9Item_Data[id].type < item.type);
                            assortiment.ItemIds.Insert(insertPos >= 0 ? insertPos : assortiment.ItemIds.Count, (RegularItem)itemId);
                        }
                        else if (currentOperation == 1)
                        {
                            assortiment.ItemIds.Remove((RegularItem)itemId);
                        }
                    }
                    else if (currentOperation == 2 || currentOperation == 3)
                    {
                        if (!Int32.TryParse(word, out Int32 recipeId) || !ff9mix.SynthesisData.TryGetValue(recipeId, out FF9MIX_DATA mix))
                            continue;
                        if (currentOperation == 2)
                            mix.Shops.Add(shopId);
                        else if (currentOperation == 3)
                            mix.Shops.Remove(shopId);
                    }
                }
            }
        }
    }
}
