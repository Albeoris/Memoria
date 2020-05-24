using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x30
    /// Unknown Opcode; ignored in the non-PSX versions.
    /// PRINT1 = 0x030,
    /// </summary>
    internal sealed class PRINT1 : JsmInstruction
    {
        private PRINT1()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new PRINT1();
        }
        public override String ToString()
        {
            return $"{nameof(PRINT1)}()";
        }
    }
}