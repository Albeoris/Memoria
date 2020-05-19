using System;
using System.Collections.Generic;
using FF8.Core;
using FF8.JSM.Instructions;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Control
        {
            public sealed partial class If
            {
                private sealed class Executer : IScriptExecuter
                {
                    private readonly If _aggregator;

                    public Executer(If aggregator)
                    {
                        _aggregator = aggregator;
                    }

                    public IEnumerable<IAwaitable> Execute(IServices services)
                    {
                        IEnumerable<IJsmInstruction> executable = GetExecutableInstructions(services);
                        if (executable == null)
                            yield break;

                        IScriptExecuter executer = ExecutableSegment.GetExecuter(executable);
                        foreach (IAwaitable result in executer.Execute(services))
                            yield return result;
                    }

                    public Boolean CanExecute(JPF jpf, IServices services)
                    {
                        foreach (var condition in jpf.Conditions)
                        {
                            if (!condition.Boolean(services))
                                return false;
                        }

                        return true;
                    }

                    private IEnumerable<IJsmInstruction> GetExecutableInstructions(IServices services)
                    {
                        if (CanExecute(_aggregator.IfRange.Jpf, services))
                            return _aggregator.IfRange.GetBodyInstructions();

                        foreach (var elseIf in _aggregator.ElseIfRanges)
                        {
                            if (CanExecute(elseIf.Jpf, services))
                            {
                                return elseIf.GetBodyInstructions();
                            }
                        }

                        return _aggregator.ElseRange?.GetBodyInstructions();
                    }
                }
            }
        }
    }
}