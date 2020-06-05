using System;
using System.Collections.Generic;
using FF8.Core;
using FF8.JSM.Instructions;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public class ExecutableSegment : Segment, IJsmInstruction
        {
            public ExecutableSegment(Int32 from, Int32 to)
                : base(from, to)
            {
            }

            public virtual IScriptExecuter GetExecuter()
            {
                return GetExecuter(_list);
            }

            internal static IScriptExecuter GetExecuter(IEnumerable<IJsmInstruction> instructions)
            {
                return new Executer(instructions);
            }

            private sealed class Executer : IScriptExecuter
            {
                private readonly IEnumerable<IJsmInstruction> _list;

                public Executer(IEnumerable<IJsmInstruction> list)
                {
                    _list = list;
                }

                public IEnumerable<IAwaitable> Execute(IServices services)
                {
                    foreach (IJsmInstruction instr in _list)
                    {
                        if (instr is JsmInstruction singleInstruction)
                        {
                            yield return singleInstruction.Execute(services);
                        }
                        else if (instr is ExecutableSegment segment)
                        {
                            // TODO: Change recursion to the loop
                            IScriptExecuter nested = segment.GetExecuter();
                            foreach (IAwaitable result in nested.Execute(services))
                                yield return result;
                        }
                        else
                        {
                            throw new NotSupportedException($"Cannot execute instruction [{instr}] of type [{instr.GetType()}].");
                        }
                    }
                }
            }
        }
    }
}