using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using Memoria.Prime;

namespace Memoria.Assets
{
    public static class TranslationExporter
    {
        public static void ExportSafe()
        {
            try
            {
                if (!Configuration.Export.Translation)
                {
                    Log.Message("[TranslationExporter] Pass through {Configuration.Export.Translation = 0}.");
                    return;
                }

                String exportSymbol = Localization.CurrentSymbol;
                String modFolder = $"ExportedTranslation{exportSymbol}/";
                if (Directory.Exists(modFolder))
                {
                    Log.Message($"[TranslationExporter] Translation export was skipped because a directory already exists [{modFolder}].");
                    return;
                }

                InitialiseStaticBatches(exportSymbol);

                ExportFieldTexts(exportSymbol, modFolder);
                ExportBattleTexts(exportSymbol, modFolder);
                ExportCommandTexts(exportSymbol, modFolder);
                ExportAbilityTexts(exportSymbol, modFolder);
                ExportSupportTexts(exportSymbol, modFolder);
                ExportItemTexts(exportSymbol, modFolder);
                ExportKeyItemTexts(exportSymbol, modFolder);
                ExportLocationNames(exportSymbol, modFolder);
                ExportCharacterNames(exportSymbol, modFolder);
                ExportEtcTexts(exportSymbol, modFolder);
                ExportLocalizationTexts(exportSymbol, modFolder);

                ExportPlaceTitles(exportSymbol, modFolder);
                ExportContinentTitles(exportSymbol, modFolder);
                ExportMessageAtlases(exportSymbol, modFolder);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to export translation materials.");
            }
        }

        private static void ExportFieldTexts(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting field texts...");
            HashSet<Int32> zones = new HashSet<Int32>(EventEngineUtils.eventIDToMESID.Values);
            String exportDirectoryMes = $"{modFolder}FF9_Data/EmbeddedAsset/Text/{symbol}/Field/";
            String exportDirectoryHW = $"{modFolder}HadesWorkshop/Field/";
            Directory.CreateDirectory(exportDirectoryMes);
            Directory.CreateDirectory(exportDirectoryHW);
            foreach (Int32 zoneId in zones)
            {
                if (!FF9TextTool.UpdateFieldTextNow(zoneId))
                    continue;
                String textAsMes = "";
                String textAsHW = $"#HW filetype TEXT\n#HW language {symbol.ToLower()}\n#HW fileid {zoneId}\n\n";
                Int32 idCounter = -1;
                if (_sharedFieldTexts.TryGetValue(zoneId, out SharedFieldText[] sharedArray))
                {
                    foreach (SharedFieldText shared in sharedArray)
                    {
                        idCounter = shared.EntryIndex - 1;
                        for (Int32 i = shared.EntryIndex; i < shared.EntryIndex + shared.EntryCount; i++)
                        {
                            if (!FF9TextTool.MainBatch.fieldText.TryGetValue(i, out String sentence))
                                sentence = "";
                            textAsMes += ProcessStringForStrtMes(ref idCounter, sentence, i);
                        }
                        File.WriteAllText($"{exportDirectoryMes}{shared.MesName}.mes", textAsMes);
                        textAsMes = "";
                    }
                    idCounter = -1;
                }
                if (_fieldMesSetup.TryGetValue(zoneId, out SharedFieldText sharedPart))
                {
                    textAsMes += sharedPart.MesName;
                    idCounter = sharedPart.EntryCount - 1;
                }
                else
                {
                    sharedPart = null;
                }
                foreach (KeyValuePair<Int32, String> pair in FF9TextTool.MainBatch.fieldText)
                {
                    String sentence = pair.Value;
                    if (sharedPart == null || pair.Key >= sharedPart.EntryCount)
                        textAsMes += ProcessStringForStrtMes(ref idCounter, sentence, pair.Key);
                    if (sentence.EndsWith("[ENDN]"))
                        sentence = sentence.Substring(0, sentence.Length - 6);
                    textAsHW += $"#HW text {pair.Key}\n{sentence}\n\n";
                }
                File.WriteAllText($"{exportDirectoryMes}{zoneId}.mes", textAsMes);
                File.WriteAllText($"{exportDirectoryHW}{FF9DBAll.MesDB[zoneId]}.txt", textAsHW);
            }
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportBattleTexts(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting battle texts...");
            String exportDirectoryMes = $"{modFolder}FF9_Data/EmbeddedAsset/Text/{symbol}/Battle/";
            String exportDirectoryHW = $"{modFolder}HadesWorkshop/Battle/";
            Directory.CreateDirectory(exportDirectoryMes);
            Directory.CreateDirectory(exportDirectoryHW);
            foreach (KeyValuePair<String, Int32> battlePair in FF9BattleDB.SceneData)
            {
                Int32 battleId = battlePair.Value;
                if (!FF9TextTool.UpdateBattleTextNow(battleId))
                    continue;
                BTL_SCENE scene = new BTL_SCENE();
                scene.ReadBattleScene(battlePair.Key.Substring(4));
                Int32 typCount = scene.header.TypCount;
                Int32 typAtkCount = typCount + scene.header.AtkCount;
                if (typAtkCount == 0)
                    continue;
                String textAsMes = "";
                String textAsHW = $"#HW filetype TEXT_BATTLE\n#HW language {symbol.ToLower()}\n#HW fileid {battleId}\n\n";
                Int32 idCounter = -1;
                foreach (KeyValuePair<Int32, String> pair in FF9TextTool.MainBatch.battleText)
                {
                    String sentence = pair.Value;
                    Int32 strIndex = pair.Key;
                    textAsMes += ProcessStringForStrtMes(ref idCounter, sentence, strIndex);
                    if (sentence.EndsWith("[ENDN]"))
                        sentence = sentence.Substring(0, sentence.Length - 6);
                    if (strIndex < typCount)
                        textAsHW += $"#HW enemyname {strIndex}\n{sentence}\n\n";
                    else if (strIndex < typAtkCount)
                        textAsHW += $"#HW attackname {strIndex - typCount}\n{sentence}\n\n";
                    else
                        textAsHW += $"#HW text {strIndex - typAtkCount}\n{sentence}\n\n";
                }
                File.WriteAllText($"{exportDirectoryMes}{battleId}.mes", textAsMes);
                File.WriteAllText($"{exportDirectoryHW}{battleId}.txt", textAsHW);
            }
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportCommandTexts(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting command texts...");
            String nameAsMes = "";
            String helpAsMes = "";
            String textAsHW = $"#HW filetype TEXT_COMMAND\n#HW language {symbol.ToLower()}\n\n";
            Int32 idCounter = -1;
            foreach (KeyValuePair<BattleCommandId, String> pair in FF9TextTool.MainBatch.commandName)
            {
                nameAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, (Int32)pair.Key);
                textAsHW += $"#HW name {(Int32)pair.Key}\n{pair.Value}\n\n";
                if (FF9TextTool.MainBatch.commandHelpDesc.TryGetValue(pair.Key, out String help))
                    textAsHW += $"#HW help {(Int32)pair.Key}\n{help}\n\n";
            }
            idCounter = -1;
            foreach (KeyValuePair<BattleCommandId, String> pair in FF9TextTool.MainBatch.commandHelpDesc)
                helpAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, (Int32)pair.Key);
            String exportDirectoryMes = $"{modFolder}FF9_Data/EmbeddedAsset/Text/{symbol}/Command/";
            String exportDirectoryHW = $"{modFolder}HadesWorkshop/Databases/";
            Directory.CreateDirectory(exportDirectoryMes);
            Directory.CreateDirectory(exportDirectoryHW);
            File.WriteAllText($"{exportDirectoryMes}Com_Name.mes", nameAsMes);
            File.WriteAllText($"{exportDirectoryMes}Com_Help.mes", helpAsMes);
            File.WriteAllText($"{exportDirectoryHW}Commands.txt", textAsHW);
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportAbilityTexts(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting active ability texts...");
            String nameAsMes = "";
            String helpAsMes = "";
            String textAsHW = $"#HW filetype TEXT_ABILITY\n#HW language {symbol.ToLower()}\n\n";
            Int32 idCounter = -1;
            foreach (KeyValuePair<BattleAbilityId, String> pair in FF9TextTool.MainBatch.actionAbilityName)
            {
                nameAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, (Int32)pair.Key);
                textAsHW += $"#HW name {(Int32)pair.Key}\n{pair.Value}\n\n";
                if (FF9TextTool.MainBatch.actionAbilityHelpDesc.TryGetValue(pair.Key, out String help))
                    textAsHW += $"#HW help {(Int32)pair.Key}\n{help}\n\n";
            }
            idCounter = -1;
            foreach (KeyValuePair<BattleAbilityId, String> pair in FF9TextTool.MainBatch.actionAbilityHelpDesc)
                helpAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, (Int32)pair.Key);
            String exportDirectoryMes = $"{modFolder}FF9_Data/EmbeddedAsset/Text/{symbol}/Ability/";
            String exportDirectoryHW = $"{modFolder}HadesWorkshop/Databases/";
            Directory.CreateDirectory(exportDirectoryMes);
            Directory.CreateDirectory(exportDirectoryHW);
            File.WriteAllText($"{exportDirectoryMes}AA_Name.mes", nameAsMes);
            File.WriteAllText($"{exportDirectoryMes}AA_Help.mes", helpAsMes);
            File.WriteAllText($"{exportDirectoryHW}ActiveAbilities.txt", textAsHW);
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportSupportTexts(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting supporting ability texts...");
            String nameAsMes = "";
            String helpAsMes = "";
            String textAsHW = $"#HW filetype TEXT_SUPPORT\n#HW language {symbol.ToLower()}\n\n";
            Int32 idCounter = -1;
            foreach (KeyValuePair<SupportAbility, String> pair in FF9TextTool.MainBatch.supportAbilityName)
            {
                nameAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, (Int32)pair.Key);
                textAsHW += $"#HW name {(Int32)pair.Key}\n{pair.Value}\n\n";
                if (FF9TextTool.MainBatch.supportAbilityHelpDesc.TryGetValue(pair.Key, out String help))
                    textAsHW += $"#HW help {(Int32)pair.Key}\n{help}\n\n";
            }
            idCounter = -1;
            foreach (KeyValuePair<SupportAbility, String> pair in FF9TextTool.MainBatch.supportAbilityHelpDesc)
                helpAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, (Int32)pair.Key);
            String exportDirectoryMes = $"{modFolder}FF9_Data/EmbeddedAsset/Text/{symbol}/Ability/";
            String exportDirectoryHW = $"{modFolder}HadesWorkshop/Databases/";
            Directory.CreateDirectory(exportDirectoryMes);
            Directory.CreateDirectory(exportDirectoryHW);
            File.WriteAllText($"{exportDirectoryMes}SA_Name.mes", nameAsMes);
            File.WriteAllText($"{exportDirectoryMes}SA_Help.mes", helpAsMes);
            File.WriteAllText($"{exportDirectoryHW}SupportAbilities.txt", textAsHW);
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportItemTexts(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting item texts...");
            String nameAsMes = "";
            String helpAsMes = "";
            String btlHelpAsMes = "";
            String textAsHW = $"#HW filetype TEXT_ITEM\n#HW language {symbol.ToLower()}\n\n";
            Int32 idCounter = -1;
            foreach (KeyValuePair<RegularItem, String> pair in FF9TextTool.MainBatch.itemName)
            {
                nameAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, (Int32)pair.Key);
                textAsHW += $"#HW name {(Int32)pair.Key}\n{pair.Value}\n\n";
                if (FF9TextTool.MainBatch.itemHelpDesc.TryGetValue(pair.Key, out String help))
                    textAsHW += $"#HW help {(Int32)pair.Key}\n{help}\n\n";
                if (FF9TextTool.MainBatch.itemBattleDesc.TryGetValue(pair.Key, out help))
                    textAsHW += $"#HW battlehelp {(Int32)pair.Key}\n{help}\n\n";
            }
            idCounter = -1;
            foreach (KeyValuePair<RegularItem, String> pair in FF9TextTool.MainBatch.itemHelpDesc)
                helpAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, (Int32)pair.Key);
            idCounter = -1;
            foreach (KeyValuePair<RegularItem, String> pair in FF9TextTool.MainBatch.itemBattleDesc)
                btlHelpAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, (Int32)pair.Key);
            String exportDirectoryMes = $"{modFolder}FF9_Data/EmbeddedAsset/Text/{symbol}/Item/";
            String exportDirectoryHW = $"{modFolder}HadesWorkshop/Databases/";
            Directory.CreateDirectory(exportDirectoryMes);
            Directory.CreateDirectory(exportDirectoryHW);
            File.WriteAllText($"{exportDirectoryMes}Itm_Name.mes", nameAsMes);
            File.WriteAllText($"{exportDirectoryMes}Itm_Help.mes", helpAsMes);
            File.WriteAllText($"{exportDirectoryMes}Itm_Btl.mes", btlHelpAsMes);
            File.WriteAllText($"{exportDirectoryHW}Items.txt", textAsHW);
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportKeyItemTexts(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting key item texts...");
            String nameAsMes = "";
            String helpAsMes = "";
            String descAsMes = "";
            String textAsHW = $"#HW filetype TEXT_KEY_ITEM\n#HW language {symbol.ToLower()}\n\n";
            Int32 idCounter = -1;
            foreach (KeyValuePair<Int32, String> pair in FF9TextTool.MainBatch.importantItemName)
            {
                nameAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, pair.Key);
                textAsHW += $"#HW name {pair.Key}\n{pair.Value}\n\n";
                if (FF9TextTool.MainBatch.importantItemHelpDesc.TryGetValue(pair.Key, out String help))
                    textAsHW += $"#HW help {pair.Key}\n{help}\n\n";
                if (FF9TextTool.MainBatch.importantSkinDesc.TryGetValue(pair.Key, out help))
                    textAsHW += $"#HW description {pair.Key}\n{help}\n\n";
            }
            idCounter = -1;
            foreach (KeyValuePair<Int32, String> pair in FF9TextTool.MainBatch.importantItemHelpDesc)
                helpAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, pair.Key);
            idCounter = -1;
            foreach (KeyValuePair<Int32, String> pair in FF9TextTool.MainBatch.importantSkinDesc)
                descAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, pair.Key);
            String exportDirectoryMes = $"{modFolder}FF9_Data/EmbeddedAsset/Text/{symbol}/KeyItem/";
            String exportDirectoryHW = $"{modFolder}HadesWorkshop/Databases/";
            Directory.CreateDirectory(exportDirectoryMes);
            Directory.CreateDirectory(exportDirectoryHW);
            File.WriteAllText($"{exportDirectoryMes}Imp_Name.mes", nameAsMes);
            File.WriteAllText($"{exportDirectoryMes}Imp_Help.mes", helpAsMes);
            File.WriteAllText($"{exportDirectoryMes}Imp_Skin.mes", descAsMes);
            File.WriteAllText($"{exportDirectoryHW}KeyItems.txt", textAsHW);
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportLocationNames(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting field location names...");
            String textAsMes = "";
            String textAsHW = $"#HW filetype TEXT_LOCATION\n#HW language {symbol.ToLower()}\n\n";
            foreach (KeyValuePair<Int32, String> pair in FF9TextTool.MainBatch.locationName)
            {
                textAsMes += $"{pair.Key}:{pair.Value}\r\n";
                textAsHW += $"#HW fieldname {pair.Key}\n{pair.Value}\n\n";
            }
            String exportDirectoryMes = $"{modFolder}FF9_Data/EmbeddedAsset/Text/{symbol}/Location/";
            String exportDirectoryHW = $"{modFolder}HadesWorkshop/Databases/";
            Directory.CreateDirectory(exportDirectoryMes);
            Directory.CreateDirectory(exportDirectoryHW);
            File.WriteAllText($"{exportDirectoryMes}Loc_Name.mes", textAsMes);
            File.WriteAllText($"{exportDirectoryHW}Locations.txt", textAsHW);
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportCharacterNames(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting character names...");
            String textAsDP = "";
            foreach (PLAYER player in FF9StateSystem.Common.FF9.PlayerList)
                textAsDP += $"CharacterDefaultName {(Int32)player.Index} {symbol} {FF9TextTool.CharacterDefaultName(player.Index)}\n";
            File.WriteAllText($"{modFolder}DictionaryPatch.txt", textAsDP);
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportEtcTexts(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting various UI texts...");
            String exportDirectoryMes = $"{modFolder}FF9_Data/EmbeddedAsset/Text/{symbol}/Etc/";
            String exportDirectoryHW = $"{modFolder}HadesWorkshop/Interface/";
            Directory.CreateDirectory(exportDirectoryMes);
            Directory.CreateDirectory(exportDirectoryHW);
            foreach (EtcTextBatch block in _etcBatches)
            {
                String textAsMes = "";
                String textAsHW = $"#HW filetype TEXT_INTERFACE\n#HW language {symbol.ToLower()}\n#HW fileid {block.hwId}\n\n";
                Int32 idCounter = -1;
                foreach (KeyValuePair<Int32, String> pair in block.texts)
                {
                    textAsMes += ProcessStringForDatabaseMes(ref idCounter, pair.Value, pair.Key);
                    textAsHW += $"#HW text {pair.Key}\n{pair.Value}\n\n";
                }
                File.WriteAllText($"{exportDirectoryMes}{block.fileName}.mes", textAsMes);
                File.WriteAllText($"{exportDirectoryHW}{block.hwName}.txt", textAsHW);
            }
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportLocalizationTexts(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting localization texts...");
            SortedList<String, String> allEntries = Localization.Provider.ProvideDictionary(symbol);
            String exportDirectoryLoc = $"{modFolder}StreamingAssets/Data/Text/";
            String exportDirectoryHW = $"{modFolder}HadesWorkshop/Interface/";
            Directory.CreateDirectory(exportDirectoryLoc);
            Directory.CreateDirectory(exportDirectoryHW);
            String textAsLoc = "";
            String textAsHW = $"#HW filetype TEXT_LOCALIZATION\n#HW language {symbol.ToLower()}\n\n";
            foreach (String header in new String[] { LanguageMap.LanguageKey, LanguageMap.SymbolKey })
            {
                textAsLoc += $"{header},{Localization.ProcessEntryForCSVWriting(allEntries[header])}\n";
                textAsHW += $"#HW entry {header}\n{allEntries[header]}\n\n";
            }
            foreach (KeyValuePair<String, String> pair in allEntries)
            {
                if (pair.Key == LanguageMap.LanguageKey || pair.Key == LanguageMap.SymbolKey)
                    continue;
                textAsLoc += $"{pair.Key},{Localization.ProcessEntryForCSVWriting(pair.Value)}\n";
                textAsHW += $"#HW entry {pair.Key}\n{pair.Value}\n\n";
            }
            foreach (KeyValuePair<String, Dictionary<String, String>> dict in Localization.DefaultDictionary)
            {
                if (allEntries.ContainsKey(dict.Key))
                    continue;
                if (!dict.Value.TryGetValue(symbol, out String entry))
                    continue;
                textAsLoc += $"{dict.Key},{Localization.ProcessEntryForCSVWriting(entry)}\n";
                textAsHW += $"#HW entry {dict.Key}\n{entry}\n\n";
            }
            File.WriteAllText($"{exportDirectoryLoc}LocalizationPatch.txt", textAsLoc);
            File.WriteAllText($"{exportDirectoryHW}LocalizationEntries.txt", textAsHW);
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportPlaceTitles(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting place titles...");
            foreach (KeyValuePair<String, FieldMapLocalizeAreaTitleInfo> pair in FieldMapInfo.localizeAreaTitle.dict)
            {
                String scenePath = FieldMap.GetMapResourcePath(pair.Key);
                String exportDirectory = $"{modFolder}StreamingAssets/Assets/Resources/{scenePath}";
                Directory.CreateDirectory(exportDirectory);
                BGSCENE_DEF scene = new BGSCENE_DEF(true);
                scene.LoadResources(scenePath, pair.Key);
                scene.LoadLocalizationInfo(pair.Key, scenePath);
                for (Int32 i = pair.Value.startOvrIdx; i <= pair.Value.endOvrIdx; i++)
                {
                    if (!scene.overlayList[i].isMemoria)
                    {
                        scene.atlas = TextureHelper.CopyAsReadable(scene.atlas, true);
                        break;
                    }
                }
                String bgxContent = $"USE_BASE_SCENE\nLANGUAGE: {symbol}\n\n";
                for (Int32 i = pair.Value.startOvrIdx; i <= pair.Value.endOvrIdx; i++)
                {
                    bgxContent += $"OVERLAY\nOverlayId: {i}\n";
                    bgxContent += scene.ExportMemoriaBGXOverlay(scene.overlayList[i], $"{exportDirectory}Title");
                }
                File.WriteAllText(exportDirectory + pair.Key + BGSCENE_DEF.MemoriaBGXExtension, bgxContent);
                UnityEngine.Object.DestroyImmediate(scene.atlas);
            }
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportContinentTitles(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting continent titles...");
            String textureDirectory = $"{modFolder}FF9_Data/EmbeddedAsset/UI/Sprites/{symbol}/";
            String environmentCode = $"# Resize or reposition continent titles here\n# Rect has format (x, y, width, height)\n# where (x, y) is the central position of the title on the screen\n\n";
            Directory.CreateDirectory(textureDirectory);
            for (SByte titleId = 0; titleId < 4; titleId++)
            {
                foreach (Boolean isShadow in new Boolean[] { false, true })
                {
                    Sprite sprite = FF9UIDataTool.LoadWorldTitle(titleId, isShadow);
                    Texture2D fullTexture = TextureHelper.CopyAsReadable(sprite.texture, true);
                    Texture2D texture = TextureHelper.GetFragment(fullTexture, (Int32)sprite.rect.x, (Int32)sprite.rect.y, (Int32)sprite.rect.width, (Int32)sprite.rect.height);
                    File.WriteAllBytes(textureDirectory + FF9UIDataTool.GetWorldTitleSpriteName(titleId, isShadow, symbol), texture.EncodeToPNG());
                    UnityEngine.Object.DestroyImmediate(fullTexture);
                    UnityEngine.Object.DestroyImmediate(texture);
                }
                Rect spriteRect = WorldConfiguration.GetTitleSpriteRect(titleId);
                environmentCode += $"Title {WorldConfiguration.GetContinentName(titleId)} {symbol} [Rect=({(Int32)spriteRect.x}, {(Int32)spriteRect.y}, {(Int32)spriteRect.width}, {(Int32)spriteRect.height})]\n";
            }
            String exportEnvDirectory = $"{modFolder}StreamingAssets/Data/World/";
            Directory.CreateDirectory(exportEnvDirectory);
            File.WriteAllText($"{exportEnvDirectory}Environment.txt", environmentCode);
            Log.Message("[TranslationExporter] Done.");
        }

        private static void ExportMessageAtlases(String symbol, String modFolder)
        {
            Log.Message("[TranslationExporter] Exporting message atlases...");
            String symbolLow = symbol.ToLower();
            foreach (AtlasWithMessage messAtlas in _messageAtlases)
            {
                if (!messAtlas.languages.Contains(symbol))
                    continue;
                String exportPath = $"{modFolder}FF9_Data/{messAtlas.atlasPath}";
                UIAtlas atlas = AssetManager.Load<UIAtlas>(messAtlas.atlasPath, true);
                if (atlas != null)
                {
                    List<UISpriteData> spriteList = atlas.spriteList;
                    spriteList.RemoveAll(sprt => !messAtlas.isTextSprite(symbolLow, sprt.name));
                    atlas.spriteList = spriteList;
                    GraphicResourceExporter.ExportPartialAtlasSafe(exportPath, atlas);
                }
                else
                {
                    Sprite[] spriteArray = Resources.LoadAll<Sprite>(messAtlas.atlasPath);
                    if (spriteArray == null || spriteArray.Length == 0)
                    {
                        Log.Message($"[TranslationExporter] Failed to export '{exportPath}' as an atlas or a sprite list");
                        continue;
                    }
                    List<Sprite> spriteList = new List<Sprite>();
                    foreach (Sprite sprt in spriteArray)
                        if (messAtlas.isTextSprite(symbolLow, sprt.name))
                            spriteList.Add(sprt);
                    GraphicResourceExporter.ExportPartialSpriteListSafe(exportPath, spriteList);
                }
            }
            String titleImageDirectory = $"EmbeddedAsset/UI/Sprites/{symbol}/";
            String exportDirectoryTitle = $"{modFolder}FF9_Data/{titleImageDirectory}";
            Directory.CreateDirectory(exportDirectoryTitle);
            for (Int32 i = 0; i < 8; i++)
            {
                for (Int32 j = 0; j < 2; j++)
                {
                    String texturePath = $"{titleImageDirectory}Title_Image_{i:D2}_Text{j}";
                    Texture2D texture = AssetManager.Load<Texture2D>(texturePath, false);
                    if (texture == null || texture.width <= 1 || texture.height <= 1)
                        continue;
                    texture = TextureHelper.CopyAsReadable(texture, true);
                    TextureHelper.WriteTextureToFile(texture, $"{modFolder}FF9_Data/{texturePath}");
                    UnityEngine.Object.DestroyImmediate(texture);
                }
            }
            Log.Message("[TranslationExporter] Done.");
        }

        private static String ProcessStringForStrtMes(ref Int32 counter, String str, Int32 txtId)
        {
            Boolean addCounterCode = counter + 1 != txtId;
            counter = txtId;
            if (!str.StartsWith("[STRT=")) // Make sure the string starts with a [STRT] opcode...
            {
                Int32 width;
                Int32 lineNo;
                if (str.StartsWith("{W"))
                {
                    Int32 hPos = str.IndexOf('H');
                    Int32 bePos = str.IndexOf('}');
                    Int32.TryParse(str.Substring(2, hPos - 2), out width);
                    Int32.TryParse(str.Substring(hPos + 1, bePos - hPos - 1), out lineNo);
                    str = str.Substring(bePos + 1);
                }
                else
                {
                    width = 0;
                    lineNo = 1;
                    foreach (Char c in str)
                        if (c == '\n')
                            lineNo++;
                }
                str = $"[STRT={width},{lineNo}]" + str;
            }
            if (!new Regex(@"(\[ENDN\]|\[TIME=[\-0-9]+\]|\{TIME [\-0-9]+\})").Match(str).Success) // ...and ends with either [ENDN] or [TIME=XXX] or {Time XXX}
                str += "[ENDN]";
            if (addCounterCode)
                return $"[TXID={txtId}]" + str;
            return str;
        }

        private static String ProcessStringForDatabaseMes(ref Int32 counter, String str, Int32 txtId)
        {
            Boolean addCounterCode = counter + 1 != txtId;
            counter = txtId;
            if (addCounterCode)
                return $"[TXID={txtId}]{str}[ENDN]";
            return str + "[ENDN]";
        }

        private class EtcTextBatch
        {
            public String fileName;
            public String hwName;
            public Int32 hwId;
            public Dictionary<Int32, String> texts;
            public EtcTextBatch(String n, String hn, Int32 id, Dictionary<Int32, String> t)
            {
                fileName = n;
                hwName = hn;
                hwId = id;
                texts = t;
            }
        }

        private class AtlasWithMessage
        {
            public String atlasPath;
            public HashSet<String> languages;
            public Func<String, String, Boolean> isTextSprite;
            public AtlasWithMessage(String p, HashSet<String> l)
            {
                atlasPath = p;
                languages = l;
                if (p.Contains("QuadMist_Text"))
                    isTextSprite = (lang, sprt) => !sprt.Contains("arrow") && !sprt.Contains("digit") && !sprt.Contains("text_combo_");
                else if (p.Contains("General Atlas"))
                    isTextSprite = (lang, sprt) => sprt.EndsWith($"_{lang}") || sprt.Contains($"_{lang}_") || sprt == Localization.Get(TitleUI.GetLanguageSpriteName(lang.ToUpper()))
                                                || sprt == Localization.Get("TitleBack") || sprt == Localization.Get("TitleCloud") || sprt == Localization.Get("TitleContinue") || sprt == Localization.Get("TitleInfo") || sprt == Localization.Get("TitleLanguageHeaderSprite") || sprt == Localization.Get("TitleLoadGame") || sprt == Localization.Get("TitleMenuSqen") || sprt == Localization.Get("TitleMovieGallery") || sprt == Localization.Get("TitleNewGame");
                else
                    isTextSprite = (lang, sprt) => sprt.EndsWith($"_{lang}") || sprt.Contains($"_{lang}_");
            }
        }

        private class SharedFieldText
        {
            public String MesName;
            public Int32 EntryIndex;
            public Int32 EntryCount;
            public SharedFieldText(String name, Int32 index, Int32 count)
            {
                MesName = name;
                EntryIndex = index;
                EntryCount = count;
            }
        }

        private static EtcTextBatch[] _etcBatches;
        private static AtlasWithMessage[] _messageAtlases;
        private static Dictionary<Int32, SharedFieldText[]> _sharedFieldTexts;
        private static Dictionary<Int32, SharedFieldText> _fieldMesSetup;

        private static void InitialiseStaticBatches(String langSymbol)
        {
            foreach (EtcImporter importer in EtcImporter.EnumerateImporters())
                importer.LoadSync();
            new CharacterNamesImporter().LoadSync();
            _etcBatches =
            [
                new EtcTextBatch("WorldLoc", "WorldLocations", 0, FF9TextTool.MainBatch.worldLocationText),
                new EtcTextBatch("Follow",   "BattleMessages", 1, FF9TextTool.MainBatch.followText),
                new EtcTextBatch("Libra",    "ScanTexts",      2, FF9TextTool.MainBatch.libraText),
                new EtcTextBatch("CmdTitle", "CommandTitles",  3, FF9TextTool.MainBatch.cmdTitleText),
                new EtcTextBatch("FF9Choco", "Chocographs",    4, FF9TextTool.MainBatch.chocoUIText),
                new EtcTextBatch("Card",     "CardRanks",      5, FF9TextTool.MainBatch.cardLvName),
                new EtcTextBatch("Minista",  "CardNames",      6, FF9TextTool.MainBatch.cardName)
            ];
            _messageAtlases =
            [
                new AtlasWithMessage("EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_US",     new HashSet<String>{ "US" }),
                new AtlasWithMessage("EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_UK",     new HashSet<String>{ "UK" }),
                new AtlasWithMessage("EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_JP",     new HashSet<String>{ "JP" }),
                new AtlasWithMessage("EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_GR",     new HashSet<String>{ "GR" }),
                new AtlasWithMessage("EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_IT",     new HashSet<String>{ "IT" }),
                new AtlasWithMessage("EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_FR",     new HashSet<String>{ "FR" }),
                new AtlasWithMessage("EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_ES",     new HashSet<String>{ "ES" }),
                new AtlasWithMessage("EmbeddedAsset/UI/Atlas/Ending_Text_FR_IT_ES_Atlas", new HashSet<String>{ "FR", "IT", "ES" }),
                new AtlasWithMessage("EmbeddedAsset/UI/Atlas/Ending_Text_US_JP_GR_Atlas", new HashSet<String>{ "US", "UK", "JP", "GR" }),
                new AtlasWithMessage("EmbeddedAsset/UI/Atlas/General Atlas",              new HashSet<String>{ "US", "UK", "JP", "GR", "FR", "IT", "ES" }),
                new AtlasWithMessage("EmbeddedAsset/UI/Atlas/TutorialUI Atlas",           new HashSet<String>{ "US", "UK", "JP", "GR", "FR", "IT", "ES" }),
            ];
            _sharedFieldTexts = new Dictionary<Int32, SharedFieldText[]>()
            {
                { 22,   [new SharedFieldText("MogNames", 0, 3),
                         new SharedFieldText("MogDialogs", 3, 41),
                         new SharedFieldText("Interface", 59, 32),
                         new SharedFieldText("LindblumTransports", 91, 27),
                         new SharedFieldText("LindblumCastleOrnements", 118, 7),
                         new SharedFieldText("LindblumATE", 125, 6)] },
                { 485,  [new SharedFieldText("LindblumOccupiedATE", 116, 3)] },
                { 595,  [new SharedFieldText("LindblumFinalATE", 116, 11)] },
                { 33,   [new SharedFieldText("JackTutorial", 97, 36),
                         new SharedFieldText("Alexandria", 133, 10),
                         new SharedFieldText("MognetKupo", 44, 21)] }, // The Most Important Thing in Life / Vanity / My Vagabond Life / Superslick
                { 40,   [new SharedFieldText("EvilForestATE", 98, 6)] },
                { 134,  [new SharedFieldText("RamuhTaleTable", 3, 1)] },
                { 71,   [new SharedFieldText("FrogTable", 3, 1)] },
                { 358,  [new SharedFieldText("MadainMogNames", 0, 1)] },
                { 70,   [new SharedFieldText("Treno", 84, 166),
                         new SharedFieldText("MognetMogrich", 44, 8)] }, // Vube Desert / New Champion
                { 7,    [new SharedFieldText("MognetMonevMopliSerino", 44, 16)] }, // No More Pointy Hats! + Tantalus / I Have a Bad Feeling + In Danger
                { 8,    [new SharedFieldText("MognetMois", 44, 6)] }, // Where Is That Item!?
                { 31,   [new SharedFieldText("MognetSuzuna", 44, 10)] }, // null + Rally-Kupo!
                { 47,   [new SharedFieldText("MognetGumo", 44, 4)] }, // Trapped In Ice!
                { 50,   [new SharedFieldText("MognetNone", 44, 3)] }, // null
                { 51,   [new SharedFieldText("MognetMogmiAtla", 44, 15)] }, // It Was So Exciting! + Map of the Entire World in His Bag? / Something Missing
                { 52,   [new SharedFieldText("MognetMozmeNoggy", 44, 18)] }, // null + My First Mognet + It's That Thing
                { 74,   [new SharedFieldText("MognetNazna", 44, 5)] }, // null + Mary's Unrequited Love
                { 124,  [new SharedFieldText("MognetMooel", 44, 6), // null + Very Mad!  Kupo!
                         new SharedFieldText("MognetMogsamKumool", 50, 14)] }, // null + Stiltzkin, On the Move + Where Is Mognet Central? / Rare Item
                { 276,  [new SharedFieldText("MognetMoodon", 44, 13)] }, // Opening a Mini-Theater / Narcissus From Lindblum / Eidolon Odin's Power / Alexandria Destroyed
                { 289,  [new SharedFieldText("MognetMosh", 44, 9)] }, // Jump-rope Champion Appears! / Rumor About Princess Garnet
                { 361,  [new SharedFieldText("MognetMogkiMoonte", 44, 15), // Stiltzkin Visited Me! / Very Bored, Kupo! + Shelter From the Rain / Missing!  Kupo!
                         new SharedFieldText("MognetMoscoMontyMochos", 59, langSymbol == "JP" ? 21 : 22)] }, // How Ya Doin'? + Prince on a White Horse / Stiltzkin on Ice / Escaping Evil Forest / This Might Be the End + Am I Right?
                { 484,  [new SharedFieldText("MognetMogrikaMoolanMogtaka", 44, 12)] }, // Favor + Problem + Where Is Mognet Central?
                { 944,  [new SharedFieldText("MognetMocchi", 44, 7)] }, // [VIVI]'s Eyes / Blessing or Curse?
                { 1073, [new SharedFieldText("MognetMogryo", 44, 8)] }, // To Conde Petie / That Special Something
            };

            // This is a bit of a hacky way to implement the shared text batches in each exported MES
            _fieldMesSetup = new Dictionary<Int32, SharedFieldText>()
            {
                { 121,  new SharedFieldText("[LOADMES=Interface]", 0, 32) },
                { 187,  new SharedFieldText("[LOADMES=Interface]", 0, 32) },
                { 2,    new SharedFieldText("[LOADMES=Interface]", 0, 32) },
                { 223,  new SharedFieldText("[LOADMES=Interface]", 0, 32) },
                { 42,   new SharedFieldText("[LOADMES=Interface]", 0, 32) },
                { 945,  new SharedFieldText("[LOADMES=Interface]", 0, 32) },
                { 358,  new SharedFieldText("[LOADMES=MadainMogNames][LOADMES=Interface]", 0, 33) },
                { 186,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=Interface]", 0, 76) },
                { 189,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=Interface]", 0, 76) },
                { 30,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=Interface]", 0, 76) },
                { 360,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=Interface]", 0, 76) },
                { 38,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=Interface]", 0, 76) },
                { 53,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=Interface]", 0, 76) },
                { 63,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=Interface]", 0, 76) },
                { 694,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=Interface]", 0, 76) },
                { 754,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=Interface]", 0, 76) },
                { 89,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=Interface]", 0, 76) },
                { 91,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=Interface]", 0, 76) },
                { 50,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetNone][LOADMES=Interface]", 0, 79) },
                { 37,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogryo][LOADMES=Interface]", 0, 84) },
                { 1073, new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogryo][LOADMES=Interface]", 0, 84) },
                { 124,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMooel][LOADMES=MognetMogsamKumool][LOADMES=Interface]", 0, 96) },
                { 739,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMooel][LOADMES=MognetMogsamKumool][LOADMES=Interface]", 0, 96) },
                { 740,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMooel][LOADMES=MognetMogsamKumool][LOADMES=Interface][LOADMES=LindblumFinalATE]", 0, 107) },
                { 276,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMoodon][LOADMES=Interface][LOADMES=LindblumTransports][LOADMES=LindblumATE]", 0, 122) },
                { 485,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMoodon][LOADMES=Interface][LOADMES=LindblumTransports][LOADMES=LindblumOccupiedATE]", 0, 119) },
                { 595,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMoodon][LOADMES=Interface][LOADMES=LindblumTransports][LOADMES=LindblumFinalATE]", 0, 127) },
                { 525,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogkiMoonte][LOADMES=Interface][LOADMES=LindblumTransports][LOADMES=LindblumOccupiedATE]", 0, 121) },
                { 943,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogkiMoonte][LOADMES=Interface][LOADMES=LindblumFinalATE][LOADMES=LindblumTransports]", 0, 129) },
                { 22,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogkiMoonte][LOADMES=Interface][LOADMES=LindblumTransports][LOADMES=LindblumCastleOrnements][LOADMES=LindblumATE]", 0, 131) },
                { 361,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogkiMoonte][LOADMES=MognetMoscoMontyMochos][LOADMES=Interface]", 0, langSymbol == "JP" ? 112 : 113) },
                { 134,  new SharedFieldText("[LOADMES=MogNames][LOADMES=RamuhTaleTable][LOADMES=MogDialogs][LOADMES=MognetMoscoMontyMochos][LOADMES=Interface]", 0, langSymbol == "JP" ? 98 : 99) },
                { 23,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMoscoMontyMochos][LOADMES=Interface][LOADMES=LindblumATE]", 0, langSymbol == "JP" ? 103 : 104) },
                { 359,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMoscoMontyMochos][LOADMES=Interface]", 0, langSymbol == "JP" ? 97 : 98) },
                { 4,    new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMoscoMontyMochos][LOADMES=Interface][LOADMES=EvilForestATE]", 0, langSymbol == "JP" ? 103 : 104) },
                { 40,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMoscoMontyMochos][LOADMES=Interface][LOADMES=EvilForestATE]", 0, langSymbol == "JP" ? 103 : 104) },
                { 738,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMocchi][LOADMES=Interface][LOADMES=LindblumOccupiedATE]", 0, 86) },
                { 944,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMocchi][LOADMES=Interface]", 0, 83) },
                { 344,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMozmeNoggy][LOADMES=Interface]", 0, 94) },
                { 52,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMozmeNoggy][LOADMES=Interface]", 0, 94) },
                { 166,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMozmeNoggy][LOADMES=Interface]", 0, 94) },
                { 18,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMonevMopliSerino][LOADMES=Interface]", 0, 92) },
                { 290,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMonevMopliSerino][LOADMES=Interface]", 0, 92) },
                { 44,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMonevMopliSerino][LOADMES=Interface]", 0, 92) },
                { 7,    new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMonevMopliSerino][LOADMES=Interface]", 0, 92) },
                { 289,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMosh][LOADMES=Interface]", 0, 85) },
                { 3,    new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMosh][LOADMES=Interface]", 0, 85) },
                { 88,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMosh][LOADMES=Interface]", 0, 85) },
                { 31,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetSuzuna][LOADMES=Interface]", 0, 86) },
                { 32,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetSuzuna][LOADMES=Interface]", 0, 86) },
                { 33,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetKupo][LOADMES=Interface][LOADMES=JackTutorial][LOADMES=Alexandria]", 0, 143) },
                { 90,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetKupo][LOADMES=Interface][LOADMES=Alexandria]", 0, 107) },
                { 946,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetKupo][LOADMES=Interface]", 0, 97) },
                { 47,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetGumo][LOADMES=Interface]", 0, 80) },
                { 484,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogrikaMoolanMogtaka][LOADMES=Interface]", 0, 88) },
                { 908,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogrikaMoolanMogtaka][LOADMES=Interface]", 0, 88) },
                { 74,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetNazna][LOADMES=Interface]", 0, 81) },
                { 70,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogrich][LOADMES=Interface][LOADMES=Treno]", 0, 250) },
                { 741,  new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogrich][LOADMES=Interface][LOADMES=Treno]", 0, 250) },
                { 51,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogmiAtla][LOADMES=Interface]", 0, 91) },
                { 77,   new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMogmiAtla][LOADMES=Interface]", 0, 91) },
                { 71,   new SharedFieldText("[LOADMES=MogNames][LOADMES=FrogTable][LOADMES=MogDialogs][LOADMES=MognetMois][LOADMES=Interface]", 0, 83) },
                { 8,    new SharedFieldText("[LOADMES=MogNames][LOADMES=MogDialogs][LOADMES=MognetMois][LOADMES=Interface]", 0, 82) },
            };
        }
    }
}
