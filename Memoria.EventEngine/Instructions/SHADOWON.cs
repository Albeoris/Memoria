using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnableShadow
    /// Enable the shadow for the entry's object.
    /// SHADOWON = 0x07F,
    /// </summary>
    internal sealed class SHADOWON : JsmInstruction
    {
        private SHADOWON()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new SHADOWON();
        }
        public override String ToString()
        {
            return $"{nameof(SHADOWON)}()";
        }
    }
}