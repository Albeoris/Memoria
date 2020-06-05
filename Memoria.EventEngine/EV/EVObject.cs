using System;
using System.Linq;
using FF8.Core;
using FF8.JSM.Format;

namespace Memoria.EventEngine.EV
{
    public sealed class EVObject
    {
        public Int32 Id { get; } // SID
        public Byte VariableCount { get; }
        public Byte Flags { get; }
        public EVScript[] Scripts { get; }

        public EVObject(Int32 index, Byte variableCount, Byte flags, EVScript[] scripts)
        {
            Id = index;
            VariableCount = variableCount;
            Flags = flags;
            Scripts = scripts;
        }

        public void FormatType(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices executionContext)
        {
            formatterContext.GetObjectScriptNamesById(Id, out String typeName, out _);
            sw.AppendLine($"public sealed class {typeName}");
            {
                sw.AppendLine("{");
                sw.Indent++;

                if (Scripts.Length > 0)
                {
                    FormatConstructor(typeName, sw, formatterContext, executionContext);

                    foreach (var script in Scripts.Skip(1))
                    {
                        sw.AppendLine();
                        script.FormatMethod(sw, formatterContext, executionContext);
                    }
                }

                sw.Indent--;
                sw.AppendLine("}");
            }
        }

        private void FormatConstructor(String typeName, ScriptWriter sw, IScriptFormatterContext formatterContext, IServices executionContext)
        {
            sw.AppendLine($"private readonly {nameof(IServices)} _ctx;");
            sw.AppendLine();

            sw.AppendLine($"public {typeName}({nameof(IServices)} executionContext)");
            {
                sw.AppendLine("{");
                sw.Indent++;

                sw.AppendLine("_ctx = executionContext;");
                Scripts[0].FormatMethodBody(sw, formatterContext, executionContext);

                sw.Indent--;
                sw.AppendLine("}");
            }
        }
    }
}