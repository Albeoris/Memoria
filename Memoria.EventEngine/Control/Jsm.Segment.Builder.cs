using System;
using System.Collections.Generic;
using FF8.JSM.Instructions;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public partial class Segment
        {
            public static class Builder
            {
                public static ExecutableSegment Build(List<JsmInstruction> instructions, Jsm.IJsmControl[] controls)
                {
                    Dictionary<Int32, List<Segment>> dic = new Dictionary<Int32, List<Segment>>();
                    foreach (var control in controls)
                    {
                        foreach (var seg in control.EnumerateSegments())
                        {
                            if (!dic.TryGetValue(seg.From, out var list))
                            {
                                list = new List<Segment>();
                                dic.Add(seg.From, list);
                            }

                            list.Add(seg);
                        }
                    }

                    ExecutableSegment rootSegment = new ExecutableSegment(0, instructions.Count);
                    dic.Add(0, new List<Segment> {rootSegment});

                    Stack<Segment> segments = new Stack<Segment>();
                    Segment segment = rootSegment;

                    Int32 instructionsCount = instructions.Count;
                    for (Int32 i = 0; i < instructionsCount; i++)
                    {
                        while (segment.To <= i)
                            segment = segments.Pop();

                        if (i > 0 && dic.TryGetValue(i, out var nestedSegments))
                        {
                            foreach (var seg in nestedSegments)
                            {
                                segments.Push(segment);
                                if (seg is ExecutableSegment executable)
                                    segment.Add(executable);

                                segment = seg;

                                if (!(instructions[i] is IJumpToInstruction))
                                    segment.Add(instructions[i]);
                            }
                        }
                        else
                        {
                            segment.Add(instructions[i]);
                        }
                    }

                    if (segments.Count != 0)
                        throw new InvalidProgramException("Failed to join code segments.");

                    return rootSegment;
                }
            }
        }
    }
}