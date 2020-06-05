using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Walk
    /// Make the character walk to destination. Make it synchronous if InitWalk is called before.
    /// 
    /// 1st and 2nd arguments: destination in (X, Y) format.
    /// AT_POSITION_X DestinationX (2 bytes)
    /// AT_POSITION_Y DestinationY (2 bytes)
    /// MOVE = 0x023,
    /// </summary>
    internal sealed class MOVE : JsmInstruction
    {
        private readonly IJsmExpression _destinationX;

        private readonly IJsmExpression _destinationY;

        private MOVE(IJsmExpression destinationX, IJsmExpression destinationY)
        {
            _destinationX = destinationX;
            _destinationY = destinationY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression destinationX = reader.ArgumentInt16();
            IJsmExpression destinationY = reader.ArgumentInt16();
            return new MOVE(destinationX, destinationY);
        }
        public override String ToString()
        {
            return $"{nameof(MOVE)}({nameof(_destinationX)}: {_destinationX}, {nameof(_destinationY)}: {_destinationY})";
        }
    }
}