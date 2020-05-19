using System;

namespace FF8.JSM.Format
{
    public interface IScriptFormatterContext
    {
        void GetObjectScriptNamesById(Int32 id, out String objectName, out String scriptName);
        String GetObjectNameByIndex(Int32 index);
        String GetMessage(Int32 messageIndex);
    }
}