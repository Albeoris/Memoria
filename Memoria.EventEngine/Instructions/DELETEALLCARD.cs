using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xAF
    /// Unknown Opcode.
    /// DELETEALLCARD = 0x0AF,
    /// </summary>
    internal sealed class DELETEALLCARD : JsmInstruction
    {
        private DELETEALLCARD()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new DELETEALLCARD();
        }
        public override String ToString()
        {
            return $"{nameof(DELETEALLCARD)}()";
        }
    }
}