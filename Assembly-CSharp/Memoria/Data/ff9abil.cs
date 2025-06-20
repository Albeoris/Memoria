using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Database;
using Memoria.Prime;
using Memoria.Prime.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantExplicitArraySize
// ReSharper disable EmptyConstructor
// ReSharper disable UnusedMember.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace FF9
{
    public static class ff9abil
    {
        // Each ability (active/support) has 2 IDs:
        // - Active Abilities have a BattleAbilityId (any number) and an integer ID (such that 0 <= (ID % 256) < 192)
        // - Supporting Abilities have a SupportAbility (any number) and an integer ID (such that 192 <= (ID % 256) < 256)
        // Thus that second ID can represent either AA or SA; it is used when something can refer to either one (eg. abilities taught by equipments)
        // Warning: sometimes, an Int32 can actually be a BattleAbilityId or a SupportAbility directly (eg. CMD_DATA.sub_no when applicable)

        //private const Byte FF9ABIL_PLAYER_MAX = 8;
        //private const Byte FF9ABIL_EQUIP_MAX = 3;
        //private const Byte FF9ABIL_NONE = 0;
        //private const Byte FF9ABIL_PA_SLOT_MAX = 16;
        //private const Byte FF9ABIL_AA_START = 0;
        //private const Byte FF9ABIL_AA_MAX = 192;
        //private const Byte FF9ABIL_SA_START = 192;
        //private const Byte FF9ABIL_SA_MAX = 64;
        //private const Byte FF9ABIL_SA_SIZE = 8;
        //private const Byte FF9ABIL_SA_HP10 = 197;
        //private const Byte FF9ABIL_SA_HP20 = 198;
        //private const Byte FF9ABIL_SA_MP10 = 199;
        //private const Byte FF9ABIL_SA_MP20 = 200;
        //private const Byte FF9ABIL_SA_FIELD_MAX = 4;
        //private const Byte FF9ABIL_SA_UP_EXP = 235;
        //private const Byte FF9ABIL_SA_UP_AP = 236;
        //private const Byte FF9ABIL_SA_UP_GIL = 237;
        //private const Byte FF9ABIL_SA_ESCAPE_GIL = 238;
        //private const Byte FF9ABIL_EVENT_SUMMON_GARNET = 18;

        public static Dictionary<CharacterPresetId, CharacterAbility[]> _FF9Abil_PaData;
        public static Dictionary<SupportAbility, CharacterAbilityGems> _FF9Abil_SaData;
        public static Dictionary<SupportAbility, SupportingAbilityFeature> _FF9Abil_SaFeature;

        static ff9abil()
        {
            _FF9Abil_PaData = LoadCharacterAbilities();
            _FF9Abil_SaData = LoadCharacterAbilityGems();
            _FF9Abil_SaFeature = LoadAllAbilityFeatures();
        }

        public static Boolean FF9Abil_IsEnableSA(HashSet<SupportAbility> sa, SupportAbility saIndex)
        {
            return sa.Contains(saIndex);
        }

        public static Boolean FF9Abil_IsEnableSA(PLAYER player, SupportAbility saIndex)
        {
            return FF9Abil_IsEnableSA(player.saExtended, saIndex);
        }

        public static List<SupportingAbilityFeature> GetEnabledSA(BTL_DATA btl)
        {
            List<SupportingAbilityFeature> result = new List<SupportingAbilityFeature>();
            HashSet<SupportAbility> saExtended = btl.saExtended;
            SupportAbility global = (SupportAbility)(btl.bi.player != 0 ? -1 : -3);
            SupportAbility globalLast = (SupportAbility)(btl.bi.player != 0 ? -2 : -4);
            if (_FF9Abil_SaFeature.ContainsKey(global))
                result.Add(_FF9Abil_SaFeature[global]);
            result.AddRange(btl.saMonster);
            foreach (SupportAbility saIndex in saExtended)
                result.Add(_FF9Abil_SaFeature[saIndex]);
            if (_FF9Abil_SaFeature.ContainsKey(globalLast))
                result.Add(_FF9Abil_SaFeature[globalLast]);
            return result;
        }

        public static List<SupportingAbilityFeature> GetEnabledSA(PLAYER player)
        {
            return GetEnabledSA(player.saExtended);
        }

        public static List<SupportingAbilityFeature> GetEnabledSA(HashSet<SupportAbility> saExtended)
        {
            List<SupportingAbilityFeature> result = new List<SupportingAbilityFeature>();
            if (_FF9Abil_SaFeature.ContainsKey((SupportAbility)(-1))) // Global
                result.Add(_FF9Abil_SaFeature[(SupportAbility)(-1)]);
            foreach (SupportAbility saIndex in saExtended)
                result.Add(_FF9Abil_SaFeature[saIndex]);
            if (_FF9Abil_SaFeature.ContainsKey((SupportAbility)(-2))) // GlobalLast
                result.Add(_FF9Abil_SaFeature[(SupportAbility)(-2)]);
            return result;
        }

        public static List<SupportingAbilityFeature> GetEnabledGlobalSA(PLAYER player)
        {
            return GetEnabledGlobalSA(player.saExtended);
        }

        public static List<SupportingAbilityFeature> GetEnabledGlobalSA(HashSet<SupportAbility> saExtended)
        {
            List<SupportingAbilityFeature> result = new List<SupportingAbilityFeature>();
            if (_FF9Abil_SaFeature.ContainsKey((SupportAbility)(-1))) // Global
                result.Add(_FF9Abil_SaFeature[(SupportAbility)(-1)]);
            if (_FF9Abil_SaFeature.ContainsKey((SupportAbility)(-2))) // GlobalLast
                result.Add(_FF9Abil_SaFeature[(SupportAbility)(-2)]);
            return result;
        }

        public static void FF9Abil_SetEnableSA(PLAYER player, SupportAbility saIndex, Boolean enable, Boolean gemCost = false)
        {
            if (enable)
                player.saExtended.Add(saIndex);
            else
                player.saExtended.Remove(saIndex);
            if (saIndex <= SupportAbility.Void)
            {
                if (enable)
                    player.sa[(Int32)saIndex >> 5] |= (UInt32)(1 << (Int32)saIndex);
                else
                    player.sa[(Int32)saIndex >> 5] &= (UInt32)~(1 << (Int32)saIndex);
            }
            if (gemCost && (Configuration.Battle.LockEquippedAbilities == 0 || Configuration.Battle.LockEquippedAbilities == 2))
            {
                CharacterAbilityGems saData = GetSupportAbilityGem(GetAbilityIdFromSupportAbility(saIndex));
                player.cur.capa += (UInt32)(enable ? -saData.GemsCount : saData.GemsCount);
            }
        }

        public static Int32 FF9Abil_GetAp(PLAYER player, Int32 abil_id)
        {
            Int32 index = FF9Abil_GetIndex(player, abil_id);
            if (index < 0)
                return -1;
            return player.pa[index];
        }

        public static Int32 FF9Abil_SetAp(PLAYER player, Int32 abil_id, Int32 ap)
        {
            Int32 index = FF9Abil_GetIndex(player, abil_id);
            if (index < 0)
                return -1;
            Int32 oldAP = player.pa[index];
            player.pa[index] = ap;
            return oldAP;
        }

        public static Int32 FF9Abil_ClearAp(PLAYER player, Int32 abil_id)
        {
            return FF9Abil_SetAp(player, abil_id, 0);
        }

        public static Int32 FF9Abil_GetMax(PLAYER player, Int32 abil_id)
        {
            Int32 index = FF9Abil_GetIndex(player, abil_id);
            if (index < 0)
                return 0;
            return _FF9Abil_PaData[player.PresetId][index].Ap;
        }

        public static Boolean FF9Abil_HasAp(PLAYER play)
        {
            if (!_FF9Abil_PaData.ContainsKey(play.PresetId))
                return false;
            return _FF9Abil_PaData[play.PresetId].Any(pa => pa.Ap > 0);
        }

        public static Boolean FF9Abil_HasSA(PLAYER play)
        {
            if (!_FF9Abil_PaData.ContainsKey(play.PresetId))
                return false;
            return _FF9Abil_PaData[play.PresetId].Any(pa => pa.IsPassive);
        }

        public static Int32 FF9Abil_GetIndex(PLAYER player, Int32 abil_id)
        {
            if (!_FF9Abil_PaData.ContainsKey(player.PresetId))
                return -1;

            CharacterAbility[] paArray = _FF9Abil_PaData[player.PresetId];
            for (Int32 index = 0; index < paArray.Length; ++index)
                if (paArray[index].Id == abil_id)
                    return index;

            return -1;
        }

        public static Boolean FF9Abil_IsMaster(PLAYER player, Int32 abil_id)
        {
            Int32 ap = FF9Abil_GetAp(player, abil_id);
            Int32 max = FF9Abil_GetMax(player, abil_id);
            return ap >= 0 && max >= 0 && ap >= max;
        }

        public static Boolean FF9Abil_SetMaster(PLAYER player, Int32 abil_id)
        {
            Int32 index = FF9Abil_GetIndex(player, abil_id);
            if (index < 0)
                return false;

            player.pa[index] = _FF9Abil_PaData[player.PresetId][index].Ap;
            return true;
        }

        public static List<SupportAbility> GetBoostedAbilityList(SupportAbility baseAbil)
        {
            return _FF9Abil_SaData[baseAbil].Boosted;
        }

        public static SupportAbility GetBaseAbilityFromBoostedAbility(SupportAbility boostedAbil)
        {
            foreach (var kvp in _FF9Abil_SaData)
                if (kvp.Value.Boosted.Contains(boostedAbil))
                    return kvp.Key;
            return boostedAbil;
        }

        public static List<SupportAbility> GetHierarchyFromAnySA(SupportAbility baseAbil)
        {
            SupportAbility baseSA = GetBaseAbilityFromBoostedAbility(baseAbil);
            List<SupportAbility> boostedList = GetBoostedAbilityList(baseSA);
            List<SupportAbility> fullHierarchy = new List<SupportAbility>();
            fullHierarchy.Add(baseSA);
            foreach (SupportAbility boostedSA in boostedList)
                fullHierarchy.Add(boostedSA);
            return fullHierarchy;
        }

        public static void DisableAllHierarchyFromSA(PLAYER player, SupportAbility baseAbil)
        {
            foreach (SupportAbility SAtoReset in GetHierarchyFromAnySA(baseAbil))
                if (FF9Abil_IsEnableSA(player.saExtended, SAtoReset))
                    FF9Abil_SetEnableSA(player, SAtoReset, false);
        }

        public static void DisableHierarchyFromSA(PLAYER player, SupportAbility baseAbil)
        {
            List<SupportAbility> saList = GetHierarchyFromAnySA(baseAbil);
            if (baseAbil != saList[0])
                return;
            foreach (SupportAbility sa in saList)
                if (FF9Abil_IsEnableSA(player.saExtended, sa))
                    FF9Abil_SetEnableSA(player, sa, false, sa != baseAbil);
        }

        public static List<SupportAbility> GetNonForcedSAInHierarchy(PLAYER player, SupportAbility baseAbil)
        {
            return new List<SupportAbility>(GetHierarchyFromAnySA(baseAbil).Where(sa => !player.saForced.Contains(sa)));
        }

        public static void CalculateGemsPlayer(PLAYER player)
        {
            if (Configuration.Battle.LockEquippedAbilities == 1 || Configuration.Battle.LockEquippedAbilities == 3)
                return;

            HashSet<SupportAbility> equippedSA = new HashSet<SupportAbility>(player.saExtended);
            List<String> notEnoughGemsLog = new List<String>();
            Int32 gemsUsed = 0;
            player.cur.capa = player.max.capa;
            foreach (SupportAbility sa in equippedSA)
            {
                if (player.cur.capa >= GetSAGemCostFromPlayer(player, sa) && (gemsUsed + GetSAGemCostFromPlayer(player, sa)) <= player.max.capa)
                {
                    gemsUsed += GetSAGemCostFromPlayer(player, sa); // "gemsUsed" is a check when using [code=MaxGems] features: in some case, currents "cur.capa" isn't enough
                    player.cur.capa -= (UInt32)GetSAGemCostFromPlayer(player, sa);
                }
                else
                {
                    FF9Abil_SetEnableSA(player, sa, false);
                    notEnoughGemsLog.Add($"{sa}");
                    player.cur.capa = (UInt32)Math.Min(player.cur.capa + GetSAGemCostFromPlayer(player, sa), player.max.capa);
                }
            }

            if (notEnoughGemsLog.Count > 0)
                Log.Message($"[CalculateGemsPlayer] Not enough gems for {player.Name}, these SA are removed => {String.Join(", ", notEnoughGemsLog.ToArray())}");
        }

        public static Int32 GetBoostedAbilityMaxLevel(PLAYER player, SupportAbility baseAbil)
        {
            if (!_FF9Abil_PaData.TryGetValue(player.PresetId, out CharacterAbility[] paArray))
                return 0;
            List<SupportAbility> boosted = GetBoostedAbilityList(baseAbil);
            for (Int32 level = 0; level < boosted.Count; level++)
                if (!paArray.Any(charAbil => charAbil.Id == GetAbilityIdFromSupportAbility(boosted[level])))
                    return level;
            return boosted.Count;
        }

        public static Int32 GetBoostedAbilityLevel(PLAYER player, SupportAbility baseAbil)
        {
            // Note: Level might be higher than MaxLevel if a supporting ability was enabled by special means (eg. a save editor)
            List<SupportAbility> boosted = GetBoostedAbilityList(baseAbil);
            for (Int32 level = 0; level < boosted.Count; level++)
                if (!FF9Abil_IsEnableSA(player.saExtended, boosted[level]))
                    return level;
            return boosted.Count;
        }

        public static Int32 GetSAGemCostFromPlayer(PLAYER player, SupportAbility baseAbil)
        {
            if (player.saForced.Contains(baseAbil))
                return 0;
            return _FF9Abil_SaData[baseAbil].GemsCount;
        }

        // The followings are also used by CsvParser and CsvWriter, so any change of behaviour should be reflected there as well
        public static Boolean IsAbilityActive(Int32 abilId)
        {
            Int32 modId = GetAbilityModuledId(abilId);
            return modId < 192;
        }

        public static Boolean IsAbilitySupport(Int32 abilId)
        {
            Int32 modId = GetAbilityModuledId(abilId);
            return modId >= 192;
        }

        public static BattleAbilityId GetActiveAbilityFromAbilityId(Int32 abilId)
        {
            if (!IsAbilityActive(abilId))
                return BattleAbilityId.Void;
            Int32 poolNum = abilId / 256;
            Int32 idInPool = abilId % 256;
            return (BattleAbilityId)(poolNum * 192 + idInPool);
        }

        public static SupportAbility GetSupportAbilityFromAbilityId(Int32 abilId)
        {
            if (!IsAbilitySupport(abilId))
                return SupportAbility.Void;
            Int32 poolNum = abilId / 256;
            Int32 idInPool = abilId % 256 - 192;
            return (SupportAbility)(poolNum * 64 + idInPool);
        }

        public static Int32 GetAbilityIdFromActiveAbility(BattleAbilityId battleAbil)
        {
            Int32 idAsInt = (Int32)battleAbil;
            Int32 poolNum = idAsInt / 192;
            Int32 idInPool = idAsInt % 192;
            return poolNum * 256 + idInPool;
        }

        public static Int32 GetAbilityIdFromSupportAbility(SupportAbility saIndex)
        {
            Int32 idAsInt = (Int32)saIndex;
            Int32 poolNum = idAsInt / 64;
            Int32 idInPool = idAsInt % 64;
            return poolNum * 256 + idInPool + 192;
        }

        public static CharacterAbilityGems GetSupportAbilityGem(Int32 abilId)
        {
            return ff9abil._FF9Abil_SaData[GetSupportAbilityFromAbilityId(abilId)];
        }

        public static SupportingAbilityFeature GetSupportAbilityFeature(Int32 abilId)
        {
            return ff9abil._FF9Abil_SaFeature[GetSupportAbilityFromAbilityId(abilId)];
        }

        public static AA_DATA GetActionAbility(Int32 abilId)
        {
            return FF9StateSystem.Battle.FF9Battle.aa_data[GetActiveAbilityFromAbilityId(abilId)];
        }

        private static Int32 GetAbilityModuledId(Int32 abilId)
        {
            return abilId % 256;
        }

        private static Dictionary<CharacterPresetId, CharacterAbility[]> LoadCharacterAbilities()
        {
            try
            {
                if (!Directory.Exists(DataResources.Characters.Abilities.Directory))
                    throw new DirectoryNotFoundException($"[ff9abil] Cannot load character abilities because a directory does not exist: [{DataResources.Characters.Abilities.Directory}].");

                Dictionary<CharacterPresetId, CharacterAbility[]> result = new Dictionary<CharacterPresetId, CharacterAbility[]>();
                foreach (CharacterCommandSet cmdset in CharacterCommands.CommandSets.Values)
                    LoadCharacterAbilities(result, cmdset.Id);

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9abil] Load character abilities failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        private static Dictionary<SupportAbility, CharacterAbilityGems> LoadCharacterAbilityGems()
        {
            try
            {
                String inputPath = DataResources.Characters.Abilities.PureDirectory + DataResources.Characters.Abilities.GemsFile;
                Dictionary<SupportAbility, CharacterAbilityGems> result = new Dictionary<SupportAbility, CharacterAbilityGems>();
                foreach (CharacterAbilityGems[] gems in AssetManager.EnumerateCsvFromLowToHigh<CharacterAbilityGems>(inputPath))
                    foreach (CharacterAbilityGems gem in gems)
                        result[gem.Id] = gem;
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load supporting abilities because a file does not exist: [{DataResources.Characters.Abilities.Directory + DataResources.Characters.Abilities.GemsFile}].", DataResources.Characters.Abilities.Directory + DataResources.Characters.Abilities.GemsFile);
                for (Int32 i = 0; i < 64; i++)
                    if (!result.ContainsKey((SupportAbility)i))
                        throw new NotSupportedException($"You must define at least the 64 supporting abilities, with IDs between 0 and 63.");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9abil] Load abilitiy gems failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        private static Boolean LoadCharacterAbilities(Dictionary<CharacterPresetId, CharacterAbility[]> result, CharacterPresetId presetId)
        {
            try
            {
                String inputPath = DataResources.Characters.Abilities.GetPresetAbilitiesPath(presetId);
                CharacterAbility[] abilitySet = AssetManager.GetCsvWithHighestPriority<CharacterAbility>(inputPath);
                if (abilitySet != null)
                {
                    result[presetId] = abilitySet;
                    return true;
                }
                if (presetId <= CharacterPresetId.Beatrix2)
                    throw new FileNotFoundException($"File with {presetId}'s abilities not found: [{inputPath}]");
                return false;
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Failed to load {presetId}'s  learnable abilities.", ex);
            }
        }

        private static Dictionary<SupportAbility, SupportingAbilityFeature> LoadAllAbilityFeatures()
        {
            try
            {
                String inputPath = DataResources.Characters.Abilities.PureDirectory + DataResources.Characters.Abilities.SAFeaturesFile;
                Dictionary<SupportAbility, SupportingAbilityFeature> result = new Dictionary<SupportAbility, SupportingAbilityFeature>();
                foreach (AssetManager.AssetFolder folder in AssetManager.FolderLowToHigh)
                    if (folder.TryFindAssetInModOnDisc(inputPath, out String fullPath, AssetManagerUtil.GetStreamingAssetsPath() + "/"))
                        LoadAbilityFeatureFile(ref result, File.ReadAllText(fullPath), fullPath);
                inputPath = DataResources.Characters.Abilities.Directory + DataResources.Characters.Abilities.SAFeaturesFile;
                if (result.Count == 0)
                    throw new FileNotFoundException($"File with ability features not found: [{inputPath}]");
                for (Int32 i = 0; i < 64; i++)
                    if (!result.ContainsKey((SupportAbility)i))
                        Log.Error($"[ff9abil] Ability features of SA {(SupportAbility)i} ({i}) is missing from [{inputPath}]");

                // Apply legacy Memoria.ini configurations
                if (Configuration.Battle.CurseUseWeaponElement)
                {
                    BattleAbilityHelper.ParseAbilityFeature(BattleAbilityId.Curse1, $"[code=Element] CasterWeaponElement != 0 ? CasterWeaponElement : (1 << GetRandom(0, 8)) [/code]");
                    BattleAbilityHelper.ParseAbilityFeature(BattleAbilityId.Curse2, $"[code=Element] CasterWeaponElement != 0 ? CasterWeaponElement : (1 << GetRandom(0, 8)) [/code]");
                }
                if (!String.IsNullOrEmpty(Configuration.Battle.SpareChangeGilSpentFormula))
                {
                    BattleAbilityHelper.ParseAbilityFeature(BattleAbilityId.SpareChange1, $"[code=GilCost] {Configuration.Battle.SpareChangeGilSpentFormula} [/code]");
                    BattleAbilityHelper.ParseAbilityFeature(BattleAbilityId.SpareChange2, $"[code=GilCost] {Configuration.Battle.SpareChangeGilSpentFormula} [/code]");
                }
                if (Configuration.Battle.SummonPriorityCount != 0)
                {
                    BattleAbilityId[] summonAbils = new BattleAbilityId[]
                    {
                        BattleAbilityId.Shiva,
                        BattleAbilityId.Ifrit,
                        BattleAbilityId.Ramuh,
                        BattleAbilityId.Atomos,
                        BattleAbilityId.Odin,
                        BattleAbilityId.Leviathan,
                        BattleAbilityId.Bahamut,
                        BattleAbilityId.Ark,
                        BattleAbilityId.Carbuncle1,
                        BattleAbilityId.Carbuncle2,
                        BattleAbilityId.Carbuncle3,
                        BattleAbilityId.Carbuncle4,
                        BattleAbilityId.Fenrir1,
                        BattleAbilityId.Fenrir2,
                        BattleAbilityId.Phoenix,
                        BattleAbilityId.Madeen
                    };
                    if (Configuration.Battle.SummonPriorityCount < 0)
                        foreach (BattleAbilityId abil in summonAbils)
                            BattleAbilityHelper.ParseAbilityFeature(abil, $"[code=Priority] 1 [/code]");
                    else
                        foreach (BattleAbilityId abil in summonAbils)
                            BattleAbilityHelper.ParseAbilityFeature(abil, $"[code=Priority] CasterSummonCount <= {Configuration.Battle.SummonPriorityCount} ? 1 : -1 [/code]");
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9abil] Load abilitiy features failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        public static void LoadAbilityFeatureFile(ref Dictionary<SupportAbility, SupportingAbilityFeature> entries, String input, String modFilePath)
        {
            MatchCollection abilMatches = new Regex(@"^(>SA|>AA|>CMD)\s+(\d+|GlobalEnemyLast|GlobalEnemy|GlobalLast|Global)(\+?).*()", RegexOptions.Multiline).Matches(input);
            for (Int32 i = 0; i < abilMatches.Count; i++)
            {
                Int32 abilIndex;
                if (String.Equals(abilMatches[i].Groups[2].Value, "Global"))
                    abilIndex = -1;
                else if (String.Equals(abilMatches[i].Groups[2].Value, "GlobalLast"))
                    abilIndex = -2;
                else if (String.Equals(abilMatches[i].Groups[2].Value, "GlobalEnemy"))
                    abilIndex = -3;
                else if (String.Equals(abilMatches[i].Groups[2].Value, "GlobalEnemyLast"))
                    abilIndex = -4;
                else if (!Int32.TryParse(abilMatches[i].Groups[2].Value, out abilIndex))
                    continue;
                Boolean cumulate = String.Equals(abilMatches[i].Groups[3].Value, "+");
                Int32 endPos, startPos = abilMatches[i].Groups[4].Captures[0].Index + 1;
                if (i + 1 == abilMatches.Count)
                    endPos = input.Length;
                else
                    endPos = abilMatches[i + 1].Groups[1].Captures[0].Index;
                Int32 lineNumber = input.Substring(0, abilMatches[i].Index).OccurenceCount("\n") + 1;
                if (String.Equals(abilMatches[i].Groups[1].Value, ">SA"))
                {
                    if (!cumulate || !entries.ContainsKey((SupportAbility)abilIndex))
                        entries[(SupportAbility)abilIndex] = new SupportingAbilityFeature();
                    entries[(SupportAbility)abilIndex].ParseFeatures((SupportAbility)abilIndex, input.Substring(startPos, endPos - startPos), modFilePath, lineNumber);
                }
                else if (String.Equals(abilMatches[i].Groups[1].Value, ">AA"))
                {
                    if (abilIndex > 0)
                    {
                        if (!cumulate)
                            BattleAbilityHelper.ClearAbilityFeature((BattleAbilityId)abilIndex);
                        BattleAbilityHelper.ParseAbilityFeature((BattleAbilityId)abilIndex, input.Substring(startPos, endPos - startPos));
                    }
                    else if (abilIndex == -1)
                    {
                        if (!cumulate)
                            BattleAbilityHelper.ClearFlexibleAbilityFeature();
                        BattleAbilityHelper.ParseAbilityFeature(input.Substring(startPos, endPos - startPos));
                    }
                }
                else if (String.Equals(abilMatches[i].Groups[1].Value, ">CMD"))
                {
                    if (abilIndex > 0)
                    {
                        if (!cumulate)
                            BattleCommandHelper.ClearCommandFeature((BattleCommandId)abilIndex);
                        BattleCommandHelper.ParseCommandFeature((BattleCommandId)abilIndex, input.Substring(startPos, endPos - startPos));
                    }
                    else if (abilIndex == -1)
                    {
                        if (!cumulate)
                            BattleCommandHelper.ClearFlexibleCommandFeature();
                        BattleCommandHelper.ParseCommandFeature(input.Substring(startPos, endPos - startPos));
                    }
                }
                else
                {
                    Log.Warning($"[ff9abil] Failure of regex find: '{abilMatches[i].Value}'");
                }
            }
        }
    }
}
