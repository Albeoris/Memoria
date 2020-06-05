using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// CloseAllWindows
    /// Close all the dialogs and UI windows.
    /// CLOSEALL = 0x0EB,
    /// </summary>
    internal sealed class CLOSEALL : JsmInstruction
    {
        private CLOSEALL()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new CLOSEALL();
        }
        public override String ToString()
        {
            return $"{nameof(CLOSEALL)}()";
        }
    }
}