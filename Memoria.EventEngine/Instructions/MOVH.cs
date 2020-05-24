using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Slide
    /// Make the character slide to destination (walk without using the walk animation and without changing the facing angle).
    /// 
    /// 1st and 2nd arguments: destination in (X, Y) format.
    /// AT_POSITION_X DestinationX (2 bytes)
    /// AT_POSITION_Y DestinationY (2 bytes)
    /// MOVH = 0x0A5,
    /// </summary>
    internal sealed class MOVH : JsmInstruction
    {
        private readonly IJsmExpression _destinationX;

        private readonly IJsmExpression _destinationY;

        private MOVH(IJsmExpression destinationX, IJsmExpression destinationY)
        {
            _destinationX = destinationX;
            _destinationY = destinationY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression destinationX = reader.ArgumentInt16();
            IJsmExpression destinationY = reader.ArgumentInt16();
            return new MOVH(destinationX, destinationY);
        }
        public override String ToString()
        {
            return $"{nameof(MOVH)}({nameof(_destinationX)}: {_destinationX}, {nameof(_destinationY)}: {_destinationY})";
        }
    }
}