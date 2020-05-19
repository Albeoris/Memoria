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
            public sealed partial class While : IJsmControl
            {
                private readonly List<JsmInstruction> _instructions;
                private readonly WhileSegment _segment;

                public While(List<JsmInstruction> instructions, Int32 from, Int32 to)
                {
                    _instructions = instructions;
                    _segment = new WhileSegment(from, to);
                    _segment.Add(_instructions[from]);
                }

                public override String ToString()
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("while(");
                    sb.Append((JPF)_instructions[_segment.From]);
                    sb.AppendLine(")");

                    sb.AppendLine("{");
                    for (Int32 i = _segment.From + 1; i < _segment.To; i++)
                    {
                        JsmInstruction instruction = _instructions[i];
                        sb.Append('\t').AppendLine(instruction.ToString());
                    }

                    sb.AppendLine("}");

                    return base.ToString();
                }

                public IEnumerable<Segment> EnumerateSegments()
                {
                    yield return _segment;
                }

                private sealed class WhileSegment : ExecutableSegment
                {
                    public WhileSegment(Int32 from, Int32 to)
                        : base(from, to)
                    {
                    }

                    public override void ToString(StringBuilder sb)
                    {
                        sb.Append("while(");
                        sb.Append((JPF)_list[0]);
                        sb.AppendLine(")");

                        FormatBranch(sb, _list.Skip(1));
                    }

                    public override void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                    {
                        sw.Append("while(");
                        ((JPF)_list[0]).Format(sw, formatterContext, services);
                        sw.AppendLine(")");
                        FormatBranch(sw, formatterContext, services, _list.Skip(1));
                    }

                    public override IScriptExecuter GetExecuter()
                    {
                        return new Executer(this);
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