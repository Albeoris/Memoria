using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x6C
    /// Unknown Opcode.
    /// PPRINT = 0x06C,
    /// </summary>
    internal sealed class PPRINT : JsmInstruction
    {
        private PPRINT()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new PPRINT();
        }
        public override String ToString()
        {
            return $"{nameof(PPRINT)}()";
        }
    }
}