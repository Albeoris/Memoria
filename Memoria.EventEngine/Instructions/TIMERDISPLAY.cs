using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ShowTimer
    /// Activate the timer window.
    /// 
    /// 1st argument: boolean show/hide.
    /// AT_SPIN Enable (1 bytes)
    /// TIMERDISPLAY = 0x08D,
    /// </summary>
    internal sealed class TIMERDISPLAY : JsmInstruction
    {
        private readonly IJsmExpression _enable;

        private TIMERDISPLAY(IJsmExpression enable)
        {
            _enable = enable;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression enable = reader.ArgumentByte();
            return new TIMERDISPLAY(enable);
        }
        public override String ToString()
        {
            return $"{nameof(TIMERDISPLAY)}({nameof(_enable)}: {_enable})";
        }
    }
}