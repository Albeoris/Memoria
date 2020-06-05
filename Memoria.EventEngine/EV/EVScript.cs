using System;
using FF8.Core;
using FF8.JSM;
using FF8.JSM.Format;

namespace Memoria.EventEngine.EV
{
    public sealed class EVScript
    {
        public Int32 Id;
        public Jsm.ExecutableSegment Segment { get; }

        public EVScript(UInt16 id, Jsm.ExecutableSegment segment)
        {
            Id = id;
            Segment = segment;
        }
        
        public void FormatMethod(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices executionContext)
        {
            String methodName = GetMethodName(formatterContext);

            sw.AppendLine($"public void {methodName}()");
            {
                sw.AppendLine("{");
                sw.Indent++;

                FormatMethodBody(sw, formatterContext, executionContext);

                sw.Indent--;
                sw.AppendLine("}");
            }
        }

        public void FormatMethodBody(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices executionContext)
        {
            Segment.Format(sw, formatterContext, executionContext);
        }

        private String GetMethodName(IScriptFormatterContext formatterContext)
        {
            formatterContext.GetObjectScriptNamesById(Id, out _, out String methodName);
            if (Char.IsLower(methodName[0]))
                methodName = Char.ToUpperInvariant(methodName[0]) + methodName.Substring(1);
            return methodName;
        }
    }
}