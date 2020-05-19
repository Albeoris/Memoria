using System;
using System.Collections.Generic;
using System.IO;
using FF8.Core;
using FF8.JSM.Format;
using Memoria.EventEngine.EV;
using Memoria.EventEngine.Execution;

namespace FF8.ScriptFormatter
{
    public static class FieldScriptFormatter
    {
        public static IEnumerable<FormattedObject> FormatAllObjects(/*Field.ILookupService lookupService*/ String directoryPath)
        {
            // foreach (Field.Info field in lookupService.EnumerateAll())
            foreach (var evPath in Directory.GetFiles(directoryPath, "*.eb.bytes"))
            foreach (FormattedObject formattedObject in FormatFieldObjects(/*field*/evPath))
                yield return formattedObject;
        }

        public static IEnumerable<FormattedObject> FormatFieldObjects(/*Field.Info field*/ String evPath)
        {
            // if (!field.TryReadData(Field.Part.Jsm, out var jsmData))
            //     yield break;

            EVObject[] gameObjects = EVFileReader.Read(evPath);

            if (gameObjects.Length == 0)
                yield break;

            IScriptFormatterContext formatterContext = GetFormatterContext(/*field*/);
            IServices executionContext = StatelessServices.Instance;
            ScriptWriter sw = new ScriptWriter();

            foreach (var obj in gameObjects)
            {
                formatterContext.GetObjectScriptNamesById(obj.Id, out String objectName, out _);
                String formattedScript = FormatObject(obj, sw, formatterContext, executionContext);
                yield return new FormattedObject(/*field,*/ objectName, formattedScript);
            }
        }

        private static ScriptFormatterContext GetFormatterContext(/*Field.Info field*/)
        {
            ScriptFormatterContext context = new ScriptFormatterContext();

            // if (field.TryReadData(Field.Part.Sym, out var symData))
            //     context.SetSymbols(Sym.Reader.FromBytes(symData));
            //
            // if (field.TryReadData(Field.Part.Msd, out var msdData))
            //     context.SetMessages(Msd.Reader.FromBytes(msdData, FF8TextEncoding.Default));
            return context;
        }

        private static String FormatObject(EVObject gameObject, ScriptWriter sw, IScriptFormatterContext formatterContext, IServices executionContext)
        {
            gameObject.FormatType(sw, formatterContext, executionContext);
            return sw.Release();
        }
    }
}