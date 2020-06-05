using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ReleaseCamera
    /// Release camera movement, getting back to its normal behaviour.
    /// 
    /// 1st arguments: duration of the repositioning.
    /// 2nd argument: scrolling type (8 for sinusoidal, other values for linear interpolation).
    /// AT_SPIN Time (1 bytes)
    /// AT_USPIN Smoothness (1 bytes)
    /// BGSRELEASE = 0x070,
    /// </summary>
    internal sealed class BGSRELEASE : JsmInstruction
    {
        private readonly IJsmExpression _time;

        private readonly IJsmExpression _smoothness;

        private BGSRELEASE(IJsmExpression time, IJsmExpression smoothness)
        {
            _time = time;
            _smoothness = smoothness;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression time = reader.ArgumentByte();
            IJsmExpression smoothness = reader.ArgumentByte();
            return new BGSRELEASE(time, smoothness);
        }
        public override String ToString()
        {
            return $"{nameof(BGSRELEASE)}({nameof(_time)}: {_time}, {nameof(_smoothness)}: {_smoothness})";
        }
    }
}