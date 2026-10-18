using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.IO;

namespace Memoria.Database
{
    public static class CharacterCommands
    {
        public static Dictionary<BattleCommandId, CharacterCommand> Commands;
        public static Dictionary<CharacterPresetId, CharacterCommandSet> CommandSets;

        static CharacterCommands()
        {
            LoadBattleCommands();
            LoadBattleCommandSets();
        }

        private static void LoadBattleCommands()
        {
            try
            {
                Commands = new Dictionary<BattleCommandId, CharacterCommand>();
                String inputPath = DataResources.Characters.PureDirectory + DataResources.Characters.CommandsFile;
                foreach (CharacterCommand[] cmds in AssetManager.EnumerateCsvFromLowToHigh<CharacterCommand>(inputPath))
                {
                    for (Int32 i = 0; i < cmds.Length; i++)
                        if (cmds[i].Id < 0)
                            cmds[i].Id = (BattleCommandId)i;
                    foreach (CharacterCommand cmd in cmds)
                    {
                        if (cmd.Id >= BattleCommandId.BoundaryCheck && cmd.Id <= BattleCommandId.BoundaryUpperCheck)
                            Log.Error($"[CharacterCommands] A command definition with ID {cmd.Id} is invalid in \"{DataResources.Characters.CommandsFile}\": commands must have an ID lower than {(Int32)BattleCommandId.BoundaryCheck} or higher than {(Int32)BattleCommandId.BoundaryUpperCheck}");
                        Commands[cmd.Id] = cmd;
                    }
                }
                if (Commands.Count == 0)
                    throw new FileNotFoundException($"Cannot load character commands because a file does not exist: [{DataResources.Characters.Directory + DataResources.Characters.CommandsFile}].", DataResources.Characters.Directory + DataResources.Characters.CommandsFile);
                for (Int32 i = 0; i < 45; i++)
                    if (!Commands.ContainsKey((BattleCommandId)i))
                        throw new NotSupportedException($"You must define at least the 45 character commands, with IDs between 0 and 44.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[CharacterCommands] Load battle commands failed.");
                UIManager.Input.ConfirmQuit();
            }
        }

        private static void LoadBattleCommandSets()
        {
            try
            {
                CommandSets = new Dictionary<CharacterPresetId, CharacterCommandSet>();
                String inputPath = DataResources.Characters.PureDirectory + DataResources.Characters.CommandSetsFile;
                foreach (CharacterCommandSet[] sets in AssetManager.EnumerateCsvFromLowToHigh<CharacterCommandSet>(inputPath))
                {
                    for (Int32 i = 0; i < sets.Length; i++)
                        if (sets[i].Id < 0)
                            sets[i].Id = (CharacterPresetId)i;
                    foreach (CharacterCommandSet set in sets)
                        CommandSets[set.Id] = set.Data;
                }
                if (CommandSets.Count == 0)
                    throw new FileNotFoundException($"Cannot load command sets because a file does not exist: [{DataResources.Characters.Directory + DataResources.Characters.CommandSetsFile}].", DataResources.Characters.Directory + DataResources.Characters.CommandSetsFile);
                for (Int32 i = 0; i < 20; i++)
                    if (!CommandSets.ContainsKey((CharacterPresetId)i))
                        throw new NotSupportedException($"You must define at least the 20 command sets, with IDs between 0 and 19.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[CharacterCommands] Load character command sets failed.");
                UIManager.Input.ConfirmQuit();
            }
        }
    }
}
