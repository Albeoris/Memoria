using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SlideXZY
    /// Make the character slide to destination (walk without using the walk animation and without changing the facing angle).
    /// 
    /// 1st to 3rd arguments: destination in (X, Z, Y) format.
    /// AT_POSITION_X DestinationX (2 bytes)
    /// AT_POSITION_Z DestinationZ (2 bytes)
    /// AT_POSITION_Y DestinationY (2 bytes)
    /// BGSMOVE = 0x058,
    /// </summary>
    internal sealed class BGSMOVE : JsmInstruction
    {
        private readonly IJsmExpression _destinationX;

        private readonly IJsmExpression _destinationZ;

        private readonly IJsmExpression _destinationY;

        private BGSMOVE(IJsmExpression destinationX, IJsmExpression destinationZ, IJsmExpression destinationY)
        {
            _destinationX = destinationX;
            _destinationZ = destinationZ;
            _destinationY = destinationY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression destinationX = reader.ArgumentInt16();
            IJsmExpression destinationZ = reader.ArgumentInt16();
            IJsmExpression destinationY = reader.ArgumentInt16();
            return new BGSMOVE(destinationX, destinationZ, destinationY);
        }
        public override String ToString()
        {
            return $"{nameof(BGSMOVE)}({nameof(_destinationX)}: {_destinationX}, {nameof(_destinationZ)}: {_destinationZ}, {nameof(_destinationY)}: {_destinationY})";
        }
    }
}