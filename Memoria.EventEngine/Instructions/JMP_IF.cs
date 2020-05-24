using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FF8.Core;
using FF8.JSM.Format;
using Memoria.EventEngine.EV;
using Memoria.Prime.Collections;

namespace FF8.JSM.Instructions
{
    internal sealed class JMP_IF : JsmInstruction, IJumpToOpcode, IFormattableScript
    {
        public Int32 Offset { get; set; }
        public IEnumerable<IJsmExpression> Conditions => _conditions;
        private readonly List<IJsmExpression> _conditions = new List<IJsmExpression>();
        private readonly Boolean _isTrue;

        public JMP_IF(Boolean isTrue, Int32 offset, IJsmExpression condition)
        {
            _isTrue = isTrue;
            Offset = offset;
            _conditions.Add(condition);
        }

        public static JsmInstruction Create(Boolean isTrue, JsmInstructionReader reader)
        {
            IJsmExpression condition = reader.Pop();
            UInt16 offset = reader.ReadUInt16();
            return new JMP_IF(isTrue, offset, condition);
        }

        public void Inverse(JMP nextJmp)
        {
            if (_conditions.Count != 1)
                throw new NotSupportedException($"Conditional jump already merged with an other one: {this}");

            IJsmExpression expression = _conditions[0];
            if (!(expression is ILogicalExpression constExpression))
                throw new NotImplementedException();
            //_conditions[0] = new Jsm.Expression.CAL.LogNot(expression);
            else
                _conditions[0] = constExpression.LogicalInverse();

            Int32 jmpIndex = nextJmp.Index;
            Int32 jmpOffset = nextJmp.Offset;
            nextJmp.Index = Index;
            nextJmp.Offset = Offset;
            Index = jmpIndex;
            Offset = jmpOffset;
        }

        public void Union(JMP_IF newJmpIf)
        {
            foreach (var cond in newJmpIf.Conditions)
                _conditions.Add(cond);
        }

        private Int32 _index = -1;

        public Int32 Index
        {
            get
            {
                if (_index == -1)
                    throw new ArgumentException($"{nameof(JMP_IF)} instruction isn't indexed yet.", nameof(Index));
                return _index;
            }
            set
            {
                // if (_index != -1)
                //    throw new ArgumentException($"{nameof(JPF)} instruction has already been indexed: {_index}.", nameof(Index));
                _index = value;
            }
        }

        public override String ToString()
        {
            String[] conditions = Conditions.Select(c => c.ToString()).ToArray();
            String result = _index < 0
                ? $"{nameof(JMP_IF)}[{nameof(Offset)}: {Offset}, {nameof(Conditions)}: ( {String.Join(") && (", conditions)} )]"
                : $"{nameof(JMP_IF)}[{nameof(Index)}: {Index}, {nameof(Conditions)}: ( {String.Join(") && (", conditions)} )]";

            return _isTrue
                ? result
                : $"!({result})";
        }

        public override void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
        {
            if (!_isTrue)
                sw.Append("!(");
            
            ScriptWriter.State state = sw.RememberState();
            
            Boolean isTrue = true;
            foreach (IJsmExpression expression in _conditions)
            {
                if (expression is IConstExpression number)
                {
                    if (IsFalse(number.Value))
                    {
                        // We can ignore the other conditions if one of them is always false
                        state.Cancel();
                        sw.Append("false");
                        return;
                    }

                    // We don't need to add a condition that is always true
                }
                else
                {
                    isTrue = false;

                    if (state.IsChanged)
                        sw.Append(" && ");

                    expression.Format(sw, formatterContext, services);
                }
            }

            if (isTrue)
            {
                // We can ignore conditions if all of them is always true
                state.Cancel();
                sw.Append("true");
            }
            
            if (!_isTrue)
                sw.Append(")");
        }

        private Boolean IsFalse(Int64 value)
        {
            return _isTrue
                ? value == 0
                : value != 0;
        }
    }
}