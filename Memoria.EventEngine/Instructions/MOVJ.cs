using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WalkToExit
    /// Make the entry's object walk to the field exit.
    /// MOVJ = 0x0A0,
    /// </summary>
    internal sealed class MOVJ : JsmInstruction
    {
        private MOVJ()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new MOVJ();
        }
        public override String ToString()
        {
            return $"{nameof(MOVJ)}()";
        }
    }
}