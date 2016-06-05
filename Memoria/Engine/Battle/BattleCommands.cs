using System;
using System.IO;

namespace Memoria
{
    public static class BattleCommands
    {
        public static readonly CharacterCommand[] Commands;
        public static readonly CharacterCommandSet[] CommandSets;

        static BattleCommands()
        {
            Commands = LoadBattleCommands();
            CommandSets = LoadBattleCommandSets();
        }

        private static CharacterCommand[] LoadBattleCommands()
        {
            try
            {
                String inputPath = DataResources.CharactersDirectory + "Commands.csv";
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"[rdata] Cannot load character commands because a file does not exist: [{inputPath}].", inputPath);

                return CsvReader.Read<CharacterCommand>(inputPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9abil] Load character commands failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        private static CharacterCommandSet[] LoadBattleCommandSets()
        {
            try
            {
                String inputPath = DataResources.CharactersDirectory + "CommandSets.csv";
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"[rdata] Cannot load character command sets because a file does not exist: [{inputPath}].", inputPath);

                return CsvReader.Read<CharacterCommandSet>(inputPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9abil] Load character commands failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }
    }
}