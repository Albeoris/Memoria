using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xD2
    /// Unknown Opcode; ignored in the non-PSX versions.
    /// DEBUGSAVE = 0x0D2,
    /// </summary>
    internal sealed class DEBUGSAVE : JsmInstruction
    {
        private DEBUGSAVE()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new DEBUGSAVE();
        }
        public override String ToString()
        {
            return $"{nameof(DEBUGSAVE)}()";
        }
    }
}