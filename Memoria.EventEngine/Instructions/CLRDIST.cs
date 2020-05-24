using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// InitWalk
    /// Make a further Walk call (or variations of Walk) synchronous.
    /// CLRDIST = 0x025,
    /// </summary>
    internal sealed class CLRDIST : JsmInstruction
    {
        private CLRDIST()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new CLRDIST();
        }
        public override String ToString()
        {
            return $"{nameof(CLRDIST)}()";
        }
    }
}