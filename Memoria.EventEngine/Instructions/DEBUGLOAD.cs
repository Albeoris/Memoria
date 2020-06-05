using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xD3
    /// Unknown Opcode; ignored in the non-PSX versions.
    /// DEBUGLOAD = 0x0D3,
    /// </summary>
    internal sealed class DEBUGLOAD : JsmInstruction
    {
        private DEBUGLOAD()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new DEBUGLOAD();
        }
        public override String ToString()
        {
            return $"{nameof(DEBUGLOAD)}()";
        }
    }
}