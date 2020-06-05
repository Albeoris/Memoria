using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xD1
    /// Unknown Opcode; ignored in the non-PSX versions.
    /// GLOBALCLEAR = 0x0D1,
    /// </summary>
    internal sealed class GLOBALCLEAR : JsmInstruction
    {
        private GLOBALCLEAR()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new GLOBALCLEAR();
        }
        public override String ToString()
        {
            return $"{nameof(GLOBALCLEAR)}()";
        }
    }
}