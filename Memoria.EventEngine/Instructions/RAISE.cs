using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RaiseWindows
    /// Make all the dialogs and windows display over the filters.
    /// RAISE = 0x08E,
    /// </summary>
    internal sealed class RAISE : JsmInstruction
    {
        private RAISE()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new RAISE();
        }
        public override String ToString()
        {
            return $"{nameof(RAISE)}()";
        }
    }
}