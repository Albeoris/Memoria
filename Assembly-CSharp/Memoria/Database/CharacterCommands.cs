using System;
using System.IO;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.CSV;

namespace Memoria.Database
{
    public static class CharacterCommands
    {
        public static readonly CharacterCommand[] Commands;
        public static readonly CharacterCommandSet[] CommandSets;

        static CharacterCommands()
        {
            Commands = LoadBattleCommands();
            CommandSets = LoadBattleCommandSets();
        }

        private static CharacterCommand[] LoadBattleCommands()
        {
            try
            {
                String inputPath = DataResources.Characters.Directory + DataResources.Characters.CommandsFile;
                String[] dir = Configuration.Mod.AllFolderNames;
                for (Int32 i = 0; i < dir.Length; i++)
                {
                    inputPath = DataResources.Characters.ModDirectory(dir[i]) + DataResources.Characters.CommandsFile;
                    if (File.Exists(inputPath))
                        return CsvReader.Read<CharacterCommand>(inputPath);
                }
                throw new FileNotFoundException($"[rdata] Cannot load character commands because a file does not exist: [{inputPath}].", inputPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[rdata] Load battle commands failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        private static CharacterCommandSet[] LoadBattleCommandSets()
        {
            try
            {
                String inputPath = DataResources.Characters.Directory + DataResources.Characters.CommandSetsFile;
                String[] dir = Configuration.Mod.AllFolderNames;
                for (Int32 i = 0; i < dir.Length; i++)
                {
                    inputPath = DataResources.Characters.ModDirectory(dir[i]) + DataResources.Characters.CommandSetsFile;
                    if (File.Exists(inputPath))
                        return CsvReader.Read<CharacterCommandSet>(inputPath);
                }
                throw new FileNotFoundException($"[rdata] Cannot load character command sets because a file does not exist: [{inputPath}].", inputPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[rdata] Load battle command sets failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }
    }
}
