using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xB7
    /// Unknown Opcode.
    /// EYE = 0x0B7,
    /// </summary>
    internal sealed class EYE : JsmInstruction
    {
        private EYE()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new EYE();
        }
        public override String ToString()
        {
            return $"{nameof(EYE)}()";
        }
    }
}