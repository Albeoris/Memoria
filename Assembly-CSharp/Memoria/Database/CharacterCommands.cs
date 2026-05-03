using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.CSV;
using System;
using System.Collections.Generic;
using System.IO;

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
                String inputPath = DataResources.Characters.PureDirectory + DataResources.Characters.CommandsFile;
                Dictionary<BattleCommandId, CharacterCommand> result = new Dictionary<BattleCommandId, CharacterCommand>();
                foreach (CharacterCommand[] cmds in AssetManager.EnumerateCsvFromLowToHigh<CharacterCommand>(inputPath))
                {
                    for (Int32 i = 0; i < cmds.Length; i++)
                        if (cmds[i].Id < 0)
                            cmds[i].Id = (BattleCommandId)i;
                    foreach (CharacterCommand cmd in cmds)
                    {
                        if (cmd.Id >= BattleCommandId.BoundaryCheck && cmd.Id <= BattleCommandId.BoundaryUpperCheck)
                            Log.Error($"[CharacterCommands] A command definition with ID {cmd.Id} is invalid in \"{DataResources.Characters.CommandsFile}\": commands must have an ID lower than {(Int32)BattleCommandId.BoundaryCheck} or higher than {(Int32)BattleCommandId.BoundaryUpperCheck}");
                        result[cmd.Id] = cmd;
                    }
                }
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load character commands because a file does not exist: [{DataResources.Characters.Directory + DataResources.Characters.CommandsFile}].", DataResources.Characters.Directory + DataResources.Characters.CommandsFile);
                for (Int32 i = 0; i < 45; i++)
                    if (!result.ContainsKey((BattleCommandId)i))
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
                String inputPath = DataResources.Characters.PureDirectory + DataResources.Characters.CommandSetsFile;
                Dictionary<CharacterPresetId, CharacterCommandSet> result = new Dictionary<CharacterPresetId, CharacterCommandSet>();
                foreach (CharacterCommandSet[] sets in AssetManager.EnumerateCsvFromLowToHigh<CharacterCommandSet>(inputPath))
                {
                    for (Int32 i = 0; i < sets.Length; i++)
                        if (sets[i].Id < 0)
                            sets[i].Id = (CharacterPresetId)i;
                    for (Int32 i = 0; i < sets.Length; i++)
                        result[sets[i].Id] = sets[i];
                }
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load command sets because a file does not exist: [{DataResources.Characters.Directory + DataResources.Characters.CommandSetsFile}].", DataResources.Characters.Directory + DataResources.Characters.CommandSetsFile);
                for (Int32 i = 0; i < 20; i++)
                    if (!result.ContainsKey((CharacterPresetId)i))
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
