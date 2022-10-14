using System;
using System.IO;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.CSV;
using Memoria.Prime.Collections;

namespace Memoria.Database
{
    public static class CharacterCommands
    {
        public static readonly CharacterCommand[] Commands;
        public static readonly EntryCollection<CharacterCommandSet> CommandSets;

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

        private static EntryCollection<CharacterCommandSet> LoadBattleCommandSets()
        {
            try
            {
                String inputPath = DataResources.Characters.Directory + DataResources.Characters.CommandSetsFile;
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"File with character command sets not found: [{inputPath}]");

                CharacterCommandSet[] cmdset = CsvReader.Read<CharacterCommandSet>(inputPath);
                if (cmdset.Length < 20)
                    throw new NotSupportedException($"You must set at least 20 different entries, but there {cmdset.Length}.");

                for (Int32 j = 0; j < cmdset.Length; j++)
                    if (cmdset[j].Id < 0)
                        cmdset[j].Id = j;
                EntryCollection<CharacterCommandSet> result = EntryCollection.CreateWithDefaultElement(cmdset, s => s.Id);
                for (Int32 i = Configuration.Mod.FolderNames.Length - 1; i >= 0; i--)
                {
                    inputPath = DataResources.Characters.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.Characters.CommandSetsFile;
                    if (File.Exists(inputPath))
                    {
                        cmdset = CsvReader.Read<CharacterCommandSet>(inputPath);
                        for (Int32 j = 0; j < cmdset.Length; j++)
                        {
                            if (cmdset[j].Id < 0)
                                cmdset[j].Id = j;
                            result[cmdset[j].Id] = cmdset[j];
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9play] Load character command sets failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        //private static EntryCollection<CharacterCommandSet> LoadBattleCommandSets()
        //{
        //    try
        //    {
        //        String inputPath = DataResources.Characters.Directory + DataResources.Characters.CommandSetsFile;
        //        String[] dir = Configuration.Mod.AllFolderNames;
        //        for (Int32 i = 0; i < dir.Length; i++)
        //        {
        //            inputPath = DataResources.Characters.ModDirectory(dir[i]) + DataResources.Characters.CommandSetsFile;
        //            if (File.Exists(inputPath))
        //                return CsvReader.Read<CharacterCommandSet>(inputPath);
        //        }
        //        throw new FileNotFoundException($"[rdata] Cannot load character command sets because a file does not exist: [{inputPath}].", inputPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "[rdata] Load battle command sets failed.");
        //        UIManager.Input.ConfirmQuit();
        //        return null;
        //    }
        //}
    }
}
