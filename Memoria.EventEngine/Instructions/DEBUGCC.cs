using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x46
    /// No use.
    /// DEBUGCC = 0x046,
    /// </summary>
    internal sealed class DEBUGCC : JsmInstruction
    {
        private DEBUGCC()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new DEBUGCC();
        }
        public override String ToString()
        {
            return $"{nameof(DEBUGCC)}()";
        }
    }
}