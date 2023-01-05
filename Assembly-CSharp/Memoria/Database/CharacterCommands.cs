using System;
using System.IO;
using System.Collections.Generic;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.CSV;

namespace Memoria.Database
{
    public static class CharacterCommands
    {
        public static readonly Dictionary<BattleCommandId, CharacterCommand> Commands;
        public static readonly Dictionary<CharacterPresetId, CharacterCommandSet> CommandSets;

        static CharacterCommands()
        {
            Commands = LoadBattleCommands();
            CommandSets = LoadBattleCommandSets();
        }

        private static Dictionary<BattleCommandId, CharacterCommand> LoadBattleCommands()
        {
            try
            {
                Dictionary<BattleCommandId, CharacterCommand> result = new Dictionary<BattleCommandId, CharacterCommand>();
                CharacterCommand[] cmds;
                String inputPath;
                String[] dir = Configuration.Mod.AllFolderNames;
                for (Int32 i = dir.Length - 1; i >= 0; --i)
                {
                    inputPath = DataResources.Characters.ModDirectory(dir[i]) + DataResources.Characters.CommandsFile;
                    if (File.Exists(inputPath))
                    {
                        cmds = CsvReader.Read<CharacterCommand>(inputPath);
                        for (Int32 j = 0; j < cmds.Length; j++)
                            if (cmds[j].Id < 0)
                                cmds[j].Id = (BattleCommandId)j;
                        for (Int32 j = 0; j < cmds.Length; j++)
                            result[cmds[j].Id] = cmds[j];
                    }
                }
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load character commands because a file does not exist: [{DataResources.Characters.Directory + DataResources.Characters.CommandsFile}].", DataResources.Characters.Directory + DataResources.Characters.CommandsFile);
                for (Int32 j = 0; j < 45; j++)
                    if (!result.ContainsKey((BattleCommandId)j))
                        throw new NotSupportedException($"You must define at least the 45 character commands, with IDs between 0 and 44.");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[CharacterCommands] Load battle commands failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        private static Dictionary<CharacterPresetId, CharacterCommandSet> LoadBattleCommandSets()
        {
            try
            {
                Dictionary<CharacterPresetId, CharacterCommandSet> result = new Dictionary<CharacterPresetId, CharacterCommandSet>();
                CharacterCommandSet[] sets;
                String inputPath;
                String[] dir = Configuration.Mod.AllFolderNames;
                for (Int32 i = dir.Length - 1; i >= 0; --i)
                {
                    inputPath = DataResources.Characters.ModDirectory(dir[i]) + DataResources.Characters.CommandSetsFile;
                    if (File.Exists(inputPath))
                    {
                        sets = CsvReader.Read<CharacterCommandSet>(inputPath);
                        for (Int32 j = 0; j < sets.Length; j++)
                            if (sets[j].Id < 0)
                                sets[j].Id = (CharacterPresetId)j;
                        for (Int32 j = 0; j < sets.Length; j++)
                            result[sets[j].Id] = sets[j];
                    }
                }
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load command sets because a file does not exist: [{DataResources.Characters.Directory + DataResources.Characters.CommandSetsFile}].", DataResources.Characters.Directory + DataResources.Characters.CommandSetsFile);
                for (Int32 j = 0; j < 20; j++)
                    if (!result.ContainsKey((CharacterPresetId)j))
                        throw new NotSupportedException($"You must define at least the 20 command sets, with IDs between 0 and 19.");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[CharacterCommands] Load character command sets failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }
    }
}
