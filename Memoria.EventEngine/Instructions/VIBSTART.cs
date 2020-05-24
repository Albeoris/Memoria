using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// VibrateController
    /// Start the vibration lifespan.
    /// 
    /// 1st argument: frame to begin with.
    /// AT_USPIN Frame (1 bytes)
    /// VIBSTART = 0x0F6,
    /// </summary>
    internal sealed class VIBSTART : JsmInstruction
    {
        private readonly IJsmExpression _frame;

        private VIBSTART(IJsmExpression frame)
        {
            _frame = frame;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression frame = reader.ArgumentByte();
            return new VIBSTART(frame);
        }
        public override String ToString()
        {
            return $"{nameof(VIBSTART)}({nameof(_frame)}: {_frame})";
        }
    }
}