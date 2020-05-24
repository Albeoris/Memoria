using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetRunSpeedLimit
    /// Change the speed at which the character uses his run animation instead of his walk animation (default is 31).
    /// 
    /// 1st argument: speed limit.
    /// AT_USPIN Speed Limit (1 bytes)
    /// SPEEDTH = 0x0A6,
    /// </summary>
    internal sealed class SPEEDTH : JsmInstruction
    {
        private readonly IJsmExpression _speedLimit;

        private SPEEDTH(IJsmExpression speedLimit)
        {
            _speedLimit = speedLimit;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression speedLimit = reader.ArgumentByte();
            return new SPEEDTH(speedLimit);
        }
        public override String ToString()
        {
            return $"{nameof(SPEEDTH)}({nameof(_speedLimit)}: {_speedLimit})";
        }
    }
}