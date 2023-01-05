using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Database;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;

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

        public static void FF9Abil_SetEnableSA(PLAYER player, SupportAbility saIndex, Boolean enable)
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
            player.pa[index] = (Byte)ap;
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

        public static Boolean FF9Abil_HasAp(Character play)
        {
            if (!_FF9Abil_PaData.ContainsKey(play.PresetId))
                return false;
            return _FF9Abil_PaData[play.PresetId].Any(pa => pa.Ap > 0);
        }

        public static Boolean FF9Abil_HasSA(Character play)
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
                Dictionary<SupportAbility, CharacterAbilityGems> result = new Dictionary<SupportAbility, CharacterAbilityGems>();
                CharacterAbilityGems[] gems;
                String inputPath;
                String[] dir = Configuration.Mod.AllFolderNames;
                for (Int32 i = dir.Length - 1; i >= 0; --i)
                {
                    inputPath = DataResources.Characters.Abilities.ModDirectory(dir[i]) + DataResources.Characters.Abilities.GemsFile;
                    if (File.Exists(inputPath))
                    {
                        gems = CsvReader.Read<CharacterAbilityGems>(inputPath);
                        for (Int32 j = 0; j < gems.Length; j++)
                            result[gems[j].Id] = gems[j];
                    }
                }
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load supporting abilities because a file does not exist: [{DataResources.Characters.Abilities.Directory + DataResources.Characters.Abilities.GemsFile}].", DataResources.Characters.Abilities.Directory + DataResources.Characters.Abilities.GemsFile);
                for (Int32 j = 0; j < 64; j++)
                    if (!result.ContainsKey((SupportAbility)j))
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
                String inputPath;
                for (Int32 i = 0; i < Configuration.Mod.FolderNames.Length; i++)
                {
                    inputPath = DataResources.Characters.Abilities.GetPresetAbilitiesPath(presetId, Configuration.Mod.FolderNames[i]);
                    if (File.Exists(inputPath))
                    {
                        result[presetId] = CsvReader.Read<CharacterAbility>(inputPath);
                        return true;
                    }
                }
                inputPath = DataResources.Characters.Abilities.GetPresetAbilitiesPath(presetId);
                if (!File.Exists(inputPath))
				{
                    if (presetId <= CharacterPresetId.Beatrix2)
                        throw new FileNotFoundException($"File with {presetId}'s abilities not found: [{inputPath}]");
                    return false;
                }
                result[presetId] = CsvReader.Read<CharacterAbility>(inputPath);
                return true;
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
                String inputPath = DataResources.Characters.Abilities.Directory + DataResources.Characters.Abilities.SAFeaturesFile;
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"File with ability features not found: [{inputPath}]");

                Dictionary<SupportAbility, SupportingAbilityFeature> result = new Dictionary<SupportAbility, SupportingAbilityFeature>();
                LoadAbilityFeatureFile(ref result, File.ReadAllText(inputPath));
                for (Int32 i = 0; i < 64; i++)
                    if (!result.ContainsKey((SupportAbility)i))
                        Log.Error($"[ff9abil] Ability features of SA {(SupportAbility)i} ({i}) is missing from [{inputPath}]");
                for (Int32 i = Configuration.Mod.FolderNames.Length - 1; i >= 0; i--)
                {
                    inputPath = DataResources.Characters.Abilities.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.Characters.Abilities.SAFeaturesFile;
                    if (File.Exists(inputPath))
                        LoadAbilityFeatureFile(ref result, File.ReadAllText(inputPath));
                }
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

        private static void LoadAbilityFeatureFile(ref Dictionary<SupportAbility, SupportingAbilityFeature> entries, String input)
		{
            MatchCollection abilMatches = new Regex(@"^(>SA|>AA)\s+(\d+|Global|GlobalLast).*()", RegexOptions.Multiline).Matches(input);
            for (Int32 i = 0; i < abilMatches.Count; i++)
			{
                Int32 abilIndex;
                if (String.Compare(abilMatches[i].Groups[2].Value, "Global") == 0)
                    abilIndex = -1;
                else if (String.Compare(abilMatches[i].Groups[2].Value, "GlobalLast") == 0)
                    abilIndex = -2;
                else if (!Int32.TryParse(abilMatches[i].Groups[2].Value, out abilIndex))
                    continue;
                Int32 endPos, startPos = abilMatches[i].Groups[3].Captures[0].Index+1;
                if (i + 1 == abilMatches.Count)
                    endPos = input.Length;
                else
                    endPos = abilMatches[i + 1].Groups[1].Captures[0].Index;
                if (String.Compare(abilMatches[i].Groups[1].Value, ">SA") == 0)
                {
                    entries[(SupportAbility)abilIndex] = new SupportingAbilityFeature();
                    entries[(SupportAbility)abilIndex].ParseFeatures((SupportAbility)abilIndex, input.Substring(startPos, endPos - startPos));
                }
                else
				{
                    BattleAbilityHelper.ParseAbilityFeature((BattleAbilityId)abilIndex, input.Substring(startPos, endPos - startPos));
                }
            }
		}
    }
}