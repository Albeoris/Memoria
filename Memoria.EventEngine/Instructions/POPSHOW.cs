using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xD6
    /// Unknown Opcode.
    /// POPSHOW = 0x0D6,
    /// </summary>
    internal sealed class POPSHOW : JsmInstruction
    {
        private POPSHOW()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new POPSHOW();
        }
        public override String ToString()
        {
            return $"{nameof(POPSHOW)}()";
        }
    }
}