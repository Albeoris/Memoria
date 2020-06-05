using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x31
    /// Unknown Opcode; ignored in the non-PSX versions.
    /// PRINTF = 0x031,
    /// </summary>
    internal sealed class PRINTF : JsmInstruction
    {
        private PRINTF()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new PRINTF();
        }
        public override String ToString()
        {
            return $"{nameof(PRINTF)}()";
        }
    }
}