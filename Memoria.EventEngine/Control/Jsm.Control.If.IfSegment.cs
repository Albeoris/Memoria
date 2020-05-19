using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FF8.Core;
using FF8.JSM.Format;
using FF8.JSM.Instructions;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Control
        {
            public sealed partial class If
            {
                private sealed class IfSegment : ExecutableSegment
                {
                    private readonly If _aggregator;

                    public IfSegment(Int32 from, Int32 to, If aggregator)
                        : base(from, to)
                    {
                        _aggregator = aggregator;
                    }

                    public override void ToString(StringBuilder sb)
                    {
                        sb.Append("if(");
                        sb.Append((JPF)_list[0]);
                        sb.AppendLine(")");
                        FormatBranch(sb, _list.Skip(1));
                    }

                    public override void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                    {
                        sw.Append("if(");
                        Jpf.Format(sw, formatterContext, services);
                        sw.AppendLine(")");
                        FormatBranch(sw, formatterContext, services, GetBodyInstructions());

                        foreach (var item in _aggregator.EnumerateElseBlocks())
                            item.Format(sw, formatterContext, services);
                    }

                    public override IScriptExecuter GetExecuter()
                    {
                        return new Executer(_aggregator);
                    }

                    public JPF Jpf => ((JPF)_list[0]);

                    public IEnumerable<IJsmInstruction> GetBodyInstructions()
                    {
                        return _list.Skip(1);
                    }
                }
            }
        }
    }
}