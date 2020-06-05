using System;
using System.Collections.Generic;
using FF8.Core;
using FF8.JSM.Format;
using FF8.JSM.Instructions;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Control
        {
            public sealed class Goto : IJsmControl, IFormattableScript
            {
                private readonly List<JsmInstruction> _instructions;
                private readonly Int32 _label;
                private readonly Segment _segment;

                public Goto(List<JsmInstruction> instructions, Int32 from, Int32 label)
                {
                    _instructions = instructions;
                    _segment = new ExecutableSegment(from, from + 1);
                    _segment.Add(_instructions[from]);
                    _label = label;
                }

                public override String ToString()
                {
                    return $"goto {_segment.From} -> {_label} ({_instructions[_label]})";
                }

                public void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
                {
                    sw.AppendLine($"goto LABEL{_label};");
                }

                public IEnumerable<Segment> EnumerateSegments()
                {
                    yield return _segment;
                }
            }
        }
    }
}