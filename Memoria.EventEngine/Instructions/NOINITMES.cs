using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x53
    /// Seems to prevent new windows to close older ones.
    /// NOINITMES = 0x053,
    /// </summary>
    internal sealed class NOINITMES : JsmInstruction
    {
        private NOINITMES()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new NOINITMES();
        }
        public override String ToString()
        {
            return $"{nameof(NOINITMES)}()";
        }
    }
}