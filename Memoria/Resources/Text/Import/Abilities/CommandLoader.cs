//using System;
//using System.IO;

//namespace Memoria
//{
//    public sealed class CommandLoader : SingleFileExporter
//    {
//        private const String Prefix = "$command";

//        protected override string TypeName => nameof(CommandLoader);
//        protected override string ExportPath => ModTextResources.Export.Commands;

//        protected override TxtEntry[] PrepareEntries()
//        {
//            String[] commandNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.CommandNames);
//            String[] commandHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.CommandHelps);

//            return AbilityFormatter.Build(Prefix, commandNames, commandHelps);
//        }
//    }
//}