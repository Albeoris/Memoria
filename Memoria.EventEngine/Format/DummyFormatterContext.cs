using System;

namespace FF8.JSM.Format
{
    public sealed class DummyFormatterContext : IScriptFormatterContext
    {
        public static IScriptFormatterContext Instance { get; } = new DummyFormatterContext();

        public void GetObjectScriptNamesById(Int32 id, out String objectName, out String scriptName)
        {
            objectName = $"ObjectId_{id:D2}";
            scriptName = $"Script_{id:D2}";
        }

        public String GetObjectNameByIndex(Int32 index)
        {
            return $"ObjectIndex_{index:D2}";
        }

        public String GetMessage(Int32 messageIndex)
        {
            return $"Message_{messageIndex:D3}";
        }
    }
}