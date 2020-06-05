using System;
using System.Collections.Generic;
using FF8.JSM.Instructions;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Control
        {
            public sealed class Builder
            {
                public static IJsmControl[] Build(List<JsmInstruction> instructions)
                {
                    return new Builder(instructions).Make();
                }

                private readonly List<JsmInstruction> _instructions;
                private readonly ProcessedJumps _processed = new ProcessedJumps();
                private readonly List<IJsmControl> _result = new List<IJsmControl>();

                private Builder(List<JsmInstruction> instructions)
                {
                    _instructions = instructions;
                }

                private Int32 _index;
                private JMP_IF _begin;

                public IJsmControl[] Make()
                {
                    for (_index = 0; _index < _instructions.Count; _index++)
                    {
                        if (TryMakeGotoOrSkip())
                            continue;

                        if (TryMakeWhile())
                            continue;

                        if (TryMakeIf())
                            continue;

                        throw new InvalidProgramException($"Cannot recognize the logical block: {_begin}");
                    }

                    return _result.ToArray();
                }

                private Boolean TryMakeGotoOrSkip()
                {
                    var instruction = _instructions[_index];
                    if (instruction is JMP @goto && _processed.TryProcess(@goto))
                    {
                        var control = new Goto(_instructions, _index, @goto.Index);
                        _result.Add(control);
                        return true;
                    }

                    if (!(instruction is JMP_IF jpf))
                        return true;

                    if (!_processed.TryProcess(jpf))
                        return true;

                    _begin = jpf;
                    return false;
                }

                private Boolean TryMakeWhile()
                {
                    if (_instructions[_begin.Index - 1] is JMP jmp && jmp.Index == _index)
                    {
                        _processed.TryProcess(jmp);
                        _result.Add(new While(_instructions, _index, _begin.Index));
                        return true;
                    }

                    return false;
                }

                private Boolean TryMakeIf()
                {
                    JMP_IF jmpIf = _begin;
                    JMP jmp = _instructions[_begin.Index - 1] as JMP;

                    If control = new If(_instructions, _index, _begin.Index);
                    _result.Add(control);

                    if (jmp == null)
                    {
                        // There is no JMP instruction. Simple if {}
                        return true;
                    }

                    if (jmp.Index == jmpIf.Index)
                    {
                        // It isn't our jump, but an nested if. If { nested if{}<-}
                        return true;
                    }

                    if (jmp.Index < jmpIf.Index)
                    {
                        // It isn't our jump, but an nested loop. If { nested while{}<-}
                        return true;
                    }

                    if (jmp.Index < _index)
                    {
                        // It isn't our jump, but an nested goto. If { nested goto l;<-}
                        return true;
                    }

                    _processed.Process(jmp);
                    AddIfElseBranches(control, jmpIf, jmp);
                    return true;
                }

                private void AddIfElseBranches(If control, JMP_IF jmpIf, JMP jmp)
                {
                    Int32 endOfBlock = jmp.Index;

                    while (true)
                    {
                        Int32 newJpfIndex = jmpIf.Index;
                        JMP_IF newJmpIf = _instructions[newJpfIndex] as JMP_IF;
                        if (newJmpIf == null || newJmpIf.Index > endOfBlock)
                        {
                            control.AddElse(jmpIf.Index, endOfBlock);
                            return;
                        }

                        JMP newJmp = _instructions[newJmpIf.Index - 1] as JMP;
                        if (newJmp == null)
                        {
                            if (newJmpIf.Index == endOfBlock)
                            {
                                // if-elseif without jmp
                                _processed.Process(newJmpIf);
                                control.AddIf(newJpfIndex, newJmpIf.Index);
                            }
                            else
                            {
                                // if-else without jmp
                                control.AddElse(jmpIf.Index, endOfBlock);
                            }

                            return;
                        }

                        // Isn't our jump
                        if (newJmp.Index != endOfBlock)
                        {
                            control.AddElse(jmpIf.Index, endOfBlock);
                            return;
                        }

                        jmpIf = newJmpIf;
                        jmp = newJmp;
                        _processed.Process(jmpIf);
                        _processed.TryProcess(jmp);

                        control.AddIf(newJpfIndex, jmpIf.Index);

                        if (jmpIf.Index == endOfBlock)
                            return;
                    }
                }
            }
        }
    }
}