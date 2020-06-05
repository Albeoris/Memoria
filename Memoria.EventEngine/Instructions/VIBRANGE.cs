using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetVibrationRange
    /// Set the time range of vibration.
    /// 
    /// 1st and 2nd arguments: vibration range in (Start, End) format.
    /// AT_USPIN Start (1 bytes)
    /// AT_USPIN End (1 bytes)
    /// VIBRANGE = 0x0FC,
    /// </summary>
    internal sealed class VIBRANGE : JsmInstruction
    {
        private readonly IJsmExpression _start;

        private readonly IJsmExpression _end;

        private VIBRANGE(IJsmExpression start, IJsmExpression end)
        {
            _start = start;
            _end = end;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression start = reader.ArgumentByte();
            IJsmExpression end = reader.ArgumentByte();
            return new VIBRANGE(start, end);
        }
        public override String ToString()
        {
            return $"{nameof(VIBRANGE)}({nameof(_start)}: {_start}, {nameof(_end)}: {_end})";
        }
    }
}