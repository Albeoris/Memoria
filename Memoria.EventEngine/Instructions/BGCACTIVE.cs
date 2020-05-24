using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnableCameraServices
    /// Enable or disable camera services. When disabling, the 2nd and 3rd arguments are ignored.
    /// 
    /// 1st arguments: boolean activate/deactivate.
    /// 2nd argument: duration of the repositioning when activating (defaulted to 30 if -1 is given).
    /// 3rd argument: scrolling type of the repositioning when activating (8 for sinusoidal, other values for linear interpolation).
    /// AT_BOOL Enable (1 bytes)
    /// AT_SPIN Time (1 bytes)
    /// AT_USPIN Smoothness (1 bytes)
    /// BGCACTIVE = 0x071,
    /// </summary>
    internal sealed class BGCACTIVE : JsmInstruction
    {
        private readonly IJsmExpression _enable;

        private readonly IJsmExpression _time;

        private readonly IJsmExpression _smoothness;

        private BGCACTIVE(IJsmExpression enable, IJsmExpression time, IJsmExpression smoothness)
        {
            _enable = enable;
            _time = time;
            _smoothness = smoothness;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression enable = reader.ArgumentByte();
            IJsmExpression time = reader.ArgumentByte();
            IJsmExpression smoothness = reader.ArgumentByte();
            return new BGCACTIVE(enable, time, smoothness);
        }
        public override String ToString()
        {
            return $"{nameof(BGCACTIVE)}({nameof(_enable)}: {_enable}, {nameof(_time)}: {_time}, {nameof(_smoothness)}: {_smoothness})";
        }
    }
}