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
                private sealed class ElseIfSegment : Segment
                {
                    public ElseIfSegment(Int32 from, Int32 to)
                        : base(from, to)
                    {
                    }

                    public override void ToString(StringBuilder sb)
                    {
                        sb.Append("else if(");
                        sb.Append(Jpf);
                        sb.AppendLine(")");
                        FormatBranch(sb, GetBodyInstructions());
                    }

                    public override void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                    {
                        sw.Append("else if(");
                        ((JPF)_list[0]).Format(sw, formatterContext, services);
                        sw.AppendLine(")");
                        FormatBranch(sw, formatterContext, services, GetBodyInstructions());
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