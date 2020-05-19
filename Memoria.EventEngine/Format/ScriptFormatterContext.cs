using System;
using System.Collections.Generic;
using FF8.JSM.Format;

namespace FF8.ScriptFormatter
{
    public sealed class ScriptFormatterContext : IScriptFormatterContext
    {
        // private Sym.GameObjects _symbols;
        // private IReadOnlyList<String> _messages;

        public void GetObjectScriptNamesById(Int32 id, out String objectName, out String scriptName)
        {
            // if (_symbols != null && _symbols.FindByLabel(id, out var obj, out scriptName))
            // {
            //     objectName = obj.Name;
            //     return;
            // }

            objectName = $"ObjectId_{id:D2}";
            scriptName = $"Script_{id:D2}";
        }

        public String GetObjectNameByIndex(Int32 index)
        {
            String objectName = String.Empty;
            // if (_symbols != null)
            //     objectName = _symbols.GetObjectOrDefault(index, defaultValue: String.Empty).Name;

            if (String.IsNullOrEmpty(objectName))
                return $"ObjectIndex_{index:D2}";

            return objectName;
        }

        public String GetMessage(Int32 messageIndex)
        {
            // if (messageIndex < _messages.Count)
            //     return _messages[messageIndex];

            return $"INVALID MESSAGE {messageIndex:D3}";
        }

        public void SetSymbols(/*Sym.GameObjects symbols*/)
        {
            // _symbols = symbols;
        }

        public void SetMessages(/*IReadOnlyList<String> messages*/)
        {
            // _messages = messages;
        }
    }
}