using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetVibrationSpeed
    /// Set the vibration frame rate.
    /// 
    /// 1st argument: frame rate.
    /// AT_USPIN Frame Rate (2 bytes)
    /// VIBRATE = 0x0FA,
    /// </summary>
    internal sealed class VIBRATE : JsmInstruction
    {
        private readonly IJsmExpression _frameRate;

        private VIBRATE(IJsmExpression frameRate)
        {
            _frameRate = frameRate;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression frameRate = reader.ArgumentInt16();
            return new VIBRATE(frameRate);
        }
        public override String ToString()
        {
            return $"{nameof(VIBRATE)}({nameof(_frameRate)}: {_frameRate})";
        }
    }
}