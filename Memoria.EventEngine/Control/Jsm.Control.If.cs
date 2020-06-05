using System;
using System.Collections.Generic;
using System.Text;
using FF8.JSM.Instructions;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Control
        {
            public sealed partial class If : IJsmControl
            {
                private readonly List<JsmInstruction> _instructions;
                private readonly IfSegment IfRange;
                private readonly List<ElseIfSegment> ElseIfRanges = new List<ElseIfSegment>();
                private ElseSegment ElseRange;

                public If(List<JsmInstruction> instructions, Int32 from, Int32 to)
                {
                    _instructions = instructions;
                    IfRange = new IfSegment(from, to, this);
                    IfRange.Add(_instructions[from]);
                }

                public void AddIf(Int32 from, Int32 to)
                {
                    ElseIfSegment elseIf = new ElseIfSegment(from, to);
                    elseIf.Add(_instructions[from]);
                    ElseIfRanges.Add(elseIf);
                }

                public void AddElse(Int32 from, Int32 to)
                {
                    if (ElseRange != null)
                        throw new InvalidOperationException($"Cannot replace existing else ({ElseRange}) by a new one.");

                    ElseRange = new ElseSegment(from, to);
                }

                public override String ToString()
                {
                    StringBuilder sb = new StringBuilder();
                    FormatIf(sb, IfRange);
                    FormatBranch(sb, IfRange);
                    foreach (var item in ElseIfRanges)
                    {
                        FormatElseIf(sb, item);
                        FormatBranch(sb, item);
                    }

                    if (ElseRange != null)
                    {
                        FormatElse(sb, ElseRange);
                        FormatBranch(sb, ElseRange);
                    }

                    return sb.ToString();
                }

                public IEnumerable<Segment> EnumerateSegments()
                {
                    yield return IfRange;

                    foreach (var block in EnumerateElseBlocks())
                        yield return block;
                }

                private IEnumerable<Segment> EnumerateElseBlocks()
                {
                    foreach (var item in ElseIfRanges)
                        yield return item;
                    
                    if (ElseRange != null)
                        yield return ElseRange;
                }

                private void FormatIf(StringBuilder sb, Segment ifRange)
                {
                    sb.Append("if(");
                    sb.Append((JMP_IF)_instructions[ifRange.From]);
                    sb.AppendLine(")");
                }

                private void FormatElseIf(StringBuilder sb, Segment elseIfRange)
                {
                    sb.Append("else if(");
                    sb.Append((JMP_IF)_instructions[elseIfRange.From]);
                    sb.AppendLine(")");
                }

                private void FormatElse(StringBuilder sb, Segment elseRange)
                {
                    sb.AppendLine("else");
                }

                private void FormatBranch(StringBuilder sb, Segment range)
                {
                    sb.AppendLine("{");
                    for (Int32 i = range.From + 1; i < range.To; i++)
                    {
                        JsmInstruction instruction = _instructions[i];
                        sb.Append('\t').AppendLine(instruction.ToString());
                    }

                    sb.AppendLine("}");
                }
            }
        }
    }
}