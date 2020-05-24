using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WaitSharedScript
    /// Wait until the ran shared script has ended.
    /// WAITSEQ = 0x044,
    /// </summary>
    internal sealed class WAITSEQ : JsmInstruction
    {
        private WAITSEQ()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new WAITSEQ();
        }
        public override String ToString()
        {
            return $"{nameof(WAITSEQ)}()";
        }
    }
}