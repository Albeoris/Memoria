using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// MoveCamera
    /// Move camera over time.
    /// 
    /// 1st and 2nd arguments: destination in (X, Y) format.
    /// 3nd argument: movement duration.
    /// 4th argument: scrolling type (8 for sinusoidal, other values for linear interpolation).
    /// AT_POSITION_X DestinationX (2 bytes)
    /// AT_POSITION_Y DestinationY (2 bytes)
    /// AT_USPIN Time (1 bytes)
    /// AT_USPIN Smoothness (1 bytes)
    /// BGSSCROLL = 0x06F,
    /// </summary>
    internal sealed class BGSSCROLL : JsmInstruction
    {
        private readonly IJsmExpression _destinationX;

        private readonly IJsmExpression _destinationY;

        private readonly IJsmExpression _time;

        private readonly IJsmExpression _smoothness;

        private BGSSCROLL(IJsmExpression destinationX, IJsmExpression destinationY, IJsmExpression time, IJsmExpression smoothness)
        {
            _destinationX = destinationX;
            _destinationY = destinationY;
            _time = time;
            _smoothness = smoothness;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression destinationX = reader.ArgumentInt16();
            IJsmExpression destinationY = reader.ArgumentInt16();
            IJsmExpression time = reader.ArgumentByte();
            IJsmExpression smoothness = reader.ArgumentByte();
            return new BGSSCROLL(destinationX, destinationY, time, smoothness);
        }
        public override String ToString()
        {
            return $"{nameof(BGSSCROLL)}({nameof(_destinationX)}: {_destinationX}, {nameof(_destinationY)}: {_destinationY}, {nameof(_time)}: {_time}, {nameof(_smoothness)}: {_smoothness})";
        }
    }
}