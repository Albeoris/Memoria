using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ResetFieldName
    /// Reset the name of the field.
    /// RESETMAPNAME = 0x0B1,
    /// </summary>
    internal sealed class RESETMAPNAME : JsmInstruction
    {
        private RESETMAPNAME()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new RESETMAPNAME();
        }
        public override String ToString()
        {
            return $"{nameof(RESETMAPNAME)}()";
        }
    }
}