using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x4E
    /// Unknown Opcode.
    /// WATCH = 0x04E,
    /// </summary>
    internal sealed class WATCH : JsmInstruction
    {
        private WATCH()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new WATCH();
        }
        public override String ToString()
        {
            return $"{nameof(WATCH)}()";
        }
    }
}