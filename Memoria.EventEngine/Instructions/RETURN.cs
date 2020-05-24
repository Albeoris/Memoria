using System;

namespace FF8.JSM.Instructions
{
    internal sealed class RETURN : JsmInstruction
    {
        public RETURN()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new RETURN();
        }

        public override String ToString()
        {
            return $"return;";
        }
    }
}