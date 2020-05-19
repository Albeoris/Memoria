using System;
using Memoria.Prime.Collections;

namespace FF8.JSM.Instructions
{
    internal sealed class NOP : JsmInstruction
    {
        public NOP()
        {
        }

        public NOP(IStack<IJsmExpression> stack)
            : this()
        {
        }

        public override String ToString()
        {
            return $"{nameof(NOP)}()";
        }
    }
}