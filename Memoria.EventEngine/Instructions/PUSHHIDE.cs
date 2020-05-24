using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xD5
    /// Unknown Opcode.
    /// PUSHHIDE = 0x0D5,
    /// </summary>
    internal sealed class PUSHHIDE : JsmInstruction
    {
        private PUSHHIDE()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new PUSHHIDE();
        }
        public override String ToString()
        {
            return $"{nameof(PUSHHIDE)}()";
        }
    }
}