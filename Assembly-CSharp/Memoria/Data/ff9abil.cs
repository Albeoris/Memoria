using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
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

        public static CharacterAbility[][] _FF9Abil_PaData;
        public static EntryCollection<CharacterAbilityGems> _FF9Abil_SaData;
        public static EntryCollection<SupportingAbilityFeature> _FF9Abil_SaFeature;

        static ff9abil()
        {
            _FF9Abil_PaData = LoadCharacterAbilities();
            _FF9Abil_SaData = LoadCharacterAbilityGems();
            _FF9Abil_SaFeature = LoadAllAbilityFeatures();
        }

        public static Boolean FF9Abil_IsEnableSA(UInt32[] sa, Int32 abil_id)
        {
            Int32 num = abil_id - 192;
            return (sa[num >> 5] & 1 << num) != 0L;
        }

        public static List<SupportingAbilityFeature> GetEnabledSA(UInt32[] sa)
        {
            List<SupportingAbilityFeature> result = new List<SupportingAbilityFeature>();
            if (_FF9Abil_SaFeature.Contains(-1)) // Global
                result.Add(_FF9Abil_SaFeature[-1]);
            for (Int32 saIndex = 0; saIndex < 64; saIndex++)
                if ((sa[saIndex >> 5] & 1 << saIndex) != 0L)
                    result.Add(_FF9Abil_SaFeature[saIndex]);
            if (_FF9Abil_SaFeature.Contains(-2)) // GlobalLast
                result.Add(_FF9Abil_SaFeature[-2]);
            return result;
        }

        public static void FF9Abil_SetEnableSA(CharacterIndex characterIndex, Int32 abil_id, Boolean enable)
        {
            UInt32[] numArray = FF9StateSystem.Common.FF9.player[characterIndex].sa;
            Int32 num = abil_id - 192;
            if (enable)
                numArray[num >> 5] |= (UInt32)(1 << num);
            else
                numArray[num >> 5] &= (UInt32)~(1 << num);
        }

        public static Boolean FF9Abil_GetEnableSA(CharacterIndex characterIndex, Int32 abil_id)
        {
            return FF9Abil_IsEnableSA(FF9StateSystem.Common.FF9.player[characterIndex].sa, abil_id);
        }

        public static Int32 FF9Abil_GetAp(CharacterIndex characterIndex, Int32 abil_id)
        {
            Int32 index = FF9Abil_GetIndex(characterIndex, abil_id);
            if (index < 0)
                return -1;
            return FF9StateSystem.Common.FF9.player[characterIndex].pa[index];
        }

        public static Int32 FF9Abil_SetAp(Int32 slot_id, Int32 abil_id, Int32 ap)
        {
            Int32 index = FF9Abil_GetIndex(slot_id, abil_id);
            if (index < 0)
                return -1;
            Int32 num = FF9StateSystem.Common.FF9.player[slot_id].pa[index];
            FF9StateSystem.Common.FF9.player[slot_id].pa[index] = (Byte)ap;
            return num;
        }

        public static Int32 FF9Abil_ClearAp(Int32 slot_id, Int32 abil_id)
        {
            return FF9Abil_SetAp(slot_id, abil_id, 0);
        }

        public static Int32 FF9Abil_GetMax(Int32 slot_id, Int32 abil_id)
        {
            Int32 index = FF9Abil_GetIndex(slot_id, abil_id);
            if (index < 0)
                return 0;

            PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
            return _FF9Abil_PaData[player.info.menu_type][index].Ap;
        }

        public static Boolean FF9Abil_HasAp(Character play)
        {
            if (play.IsMainCharacter && play.PresetId < 8)
                return true;

            if (play.PresetId >= _FF9Abil_PaData.Length)
                return false;

            return _FF9Abil_PaData[play.PresetId].Any(pa => pa.Ap > 0);
        }

        public static Int32 FF9Abil_GetIndex(Int32 slot_id, Int32 abil_id)
        {
            if (slot_id >= 9)
                return -1;

            PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
            if (player.info.menu_type >= 16)
                return -1;

            CharacterAbility[] array = _FF9Abil_PaData[player.info.menu_type];
            for (Int32 index = 0; index < array.Length; ++index)
                if (_FF9Abil_PaData[player.info.menu_type][index].Id == abil_id)
                    return index;

            return -1;
        }

        public static Boolean FF9Abil_IsMaster(CharacterPresetId presetId, Int32 abilityId)
        {
            return FF9Abil_IsMaster(presetId, abilityId);
        }

        public static Boolean FF9Abil_IsMaster(CharacterIndex characterIndex, Int32 abil_id)
        {
            Int32 ap = FF9Abil_GetAp(characterIndex, abil_id);
            Int32 max = FF9Abil_GetMax(characterIndex, abil_id);
            return ap >= 0 && max >= 0 && ap >= max;
        }

        public static Boolean FF9Abil_SetMaster(CharacterPresetId presetId, Int32 abilityId)
        {
            return FF9Abil_SetMaster((Int32)presetId, abilityId);
        }

        public static Boolean FF9Abil_SetMaster(Int32 slot_id, Int32 abil_id)
        {
            Int32 index = FF9Abil_GetIndex(slot_id, abil_id);
            if (index < 0)
                return false;

            PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
            player.pa[index] = _FF9Abil_PaData[player.info.menu_type][index].Ap;
            return true;
        }

        private static CharacterAbility[][] LoadCharacterAbilities()
        {
            try
            {
                if (!Directory.Exists(DataResources.Characters.Abilities.Directory))
                    throw new DirectoryNotFoundException($"[ff9abil] Cannot load character abilities because a directory does not exist: [{DataResources.Characters.Abilities.Directory}].");

                CharacterPresetId[] presetIds = CharacterPresetId.GetWellKnownPresetIds();
                CharacterAbility[][] result = new CharacterAbility[presetIds.Length][];
                for (Int32 id = 0; id < result.Length; id++)
                {
                    CharacterPresetId presetId = presetIds[id];
                    result[id] = LoadCharacterAbilities(presetId);
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9abil] Load character abilities failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        private static EntryCollection<CharacterAbilityGems> LoadCharacterAbilityGems()
        {
            try
            {
                String inputPath = DataResources.Characters.Abilities.Directory + DataResources.Characters.Abilities.GemsFile;
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"File with ability gems not found: [{inputPath}]");

                CharacterAbilityGems[] gems = CsvReader.Read<CharacterAbilityGems>(inputPath);
                if (gems.Length != 64)
                    throw new NotSupportedException($"You must set number of gems for 64 abilities, but there {gems.Length}. Any number of abilities will be available after a game stabilization.");

                EntryCollection<CharacterAbilityGems> result = EntryCollection.CreateWithDefaultElement(gems, g => g.Id);
                for (Int32 i = Configuration.Mod.FolderNames.Length - 1; i >= 0; i--)
                {
                    inputPath = DataResources.Characters.Abilities.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.Characters.Abilities.GemsFile;
                    if (File.Exists(inputPath))
                    {
                        gems = CsvReader.Read<CharacterAbilityGems>(inputPath);
                        foreach (CharacterAbilityGems it in gems)
                            result[it.Id] = it;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9abil] Load abilitiy gems failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        private static CharacterAbility[] LoadCharacterAbilities(CharacterPresetId presetId)
        {
            try
            {
                CharacterAbility[] abilities;
                String inputPath;
                for (Int32 i = 0; i < Configuration.Mod.FolderNames.Length; i++)
                {
                    inputPath = DataResources.Characters.Abilities.GetPresetAbilitiesPath(presetId, Configuration.Mod.FolderNames[i]);
                    if (File.Exists(inputPath))
                    {
                        abilities = CsvReader.Read<CharacterAbility>(inputPath);
                        if (abilities.Length != 48)
                            throw new NotSupportedException($"You must set 48 abilities, but there {abilities.Length}. Any number of abilities will be available after a game stabilization."); // TODO Json, Player, SettingsState, EqupUI, ff9feqp
                        return abilities;
                    }
                }
                inputPath = DataResources.Characters.Abilities.GetPresetAbilitiesPath(presetId);
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"File with {presetId}'s abilities not found: [{inputPath}]");

                abilities = CsvReader.Read<CharacterAbility>(inputPath);
                if (abilities.Length != 48)
                    throw new NotSupportedException($"You must set 48 abilities, but there {abilities.Length}. Any number of abilities will be available after a game stabilization."); // TODO Json, Player, SettingsState, EqupUI, ff9feqp

                return abilities;
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Failed to load {presetId}'s  learnable abilities.", ex);
            }
        }

        private static EntryCollection<SupportingAbilityFeature> LoadAllAbilityFeatures()
        {
            try
            {
                String inputPath = DataResources.Characters.Abilities.Directory + DataResources.Characters.Abilities.SAFeaturesFile;
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"File with ability features not found: [{inputPath}]");

                EntryCollection<SupportingAbilityFeature> result = EntryCollection.CreateWithDefaultElement<SupportingAbilityFeature>(0);
                LoadAbilityFeatureFile(ref result, File.ReadAllText(inputPath));
                for (Int32 i = 0; i < 64; i++)
                    if (!result.Contains(i))
                        Log.Error($"[ff9abil] Ability features of SA {i} is missing from [{inputPath}]");
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

        private static void LoadAbilityFeatureFile(ref EntryCollection<SupportingAbilityFeature> entries, String input)
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
                    entries[abilIndex] = new SupportingAbilityFeature();
                    entries[abilIndex].ParseFeatures(abilIndex, input.Substring(startPos, endPos - startPos));
                }
                else
				{
                    BattleAbilityHelper.ParseAbilityFeature((BattleAbilityId)abilIndex, input.Substring(startPos, endPos - startPos));
                }
            }
		}
    }
}