using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x6D
    /// Unknown Opcode.
    /// PPRINTF = 0x06D,
    /// </summary>
    internal sealed class PPRINTF : JsmInstruction
    {
        private PPRINTF()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new PPRINTF();
        }
        public override String ToString()
        {
            return $"{nameof(PPRINTF)}()";
        }
    }
}