using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// StopSharedScript
    /// Terminate the execution of the ran shared script.
    /// ENDSEQ = 0x045,
    /// </summary>
    internal sealed class ENDSEQ : JsmInstruction
    {
        private ENDSEQ()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new ENDSEQ();
        }
        public override String ToString()
        {
            return $"{nameof(ENDSEQ)}()";
        }
    }
}