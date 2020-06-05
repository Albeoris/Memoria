using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xEA
    /// Unknown Opcode.
    /// VRP = 0x0EA,
    /// </summary>
    internal sealed class VRP : JsmInstruction
    {
        private VRP()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new VRP();
        }
        public override String ToString()
        {
            return $"{nameof(VRP)}()";
        }
    }
}