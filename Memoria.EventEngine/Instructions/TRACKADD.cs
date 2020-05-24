using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x78
    /// Unknown Opcode.
    /// TRACKADD = 0x078,
    /// </summary>
    internal sealed class TRACKADD : JsmInstruction
    {
        private TRACKADD()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new TRACKADD();
        }
        public override String ToString()
        {
            return $"{nameof(TRACKADD)}()";
        }
    }
}