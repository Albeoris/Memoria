using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xB8
    /// Unknown Opcode.
    /// AIM = 0x0B8,
    /// </summary>
    internal sealed class AIM : JsmInstruction
    {
        private AIM()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new AIM();
        }
        public override String ToString()
        {
            return $"{nameof(AIM)}()";
        }
    }
}