using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x79
    /// Unknown Opcode.
    /// PRINTQUAD = 0x079,
    /// </summary>
    internal sealed class PRINTQUAD : JsmInstruction
    {
        private PRINTQUAD()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new PRINTQUAD();
        }
        public override String ToString()
        {
            return $"{nameof(PRINTQUAD)}()";
        }
    }
}