using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// MoveInstantEx
    /// Instantatly move an object.
    /// 
    /// 1st argument: object's entry.
    /// 2nd and 3rd arguments: destination in (X, Y) format.
    /// AT_ENTRY Object (1 bytes)
    /// AT_POSITION_X DestinationX (2 bytes)
    /// AT_POSITION_Y DestinationY (2 bytes)
    /// DPOS = 0x0BF,
    /// </summary>
    internal sealed class DPOS : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _destinationX;

        private readonly IJsmExpression _destinationY;

        private DPOS(IJsmExpression @object, IJsmExpression destinationX, IJsmExpression destinationY)
        {
            _object = @object;
            _destinationX = destinationX;
            _destinationY = destinationY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression destinationX = reader.ArgumentInt16();
            IJsmExpression destinationY = reader.ArgumentInt16();
            return new DPOS(@object, destinationX, destinationY);
        }
        public override String ToString()
        {
            return $"{nameof(DPOS)}({nameof(_object)}: {_object}, {nameof(_destinationX)}: {_destinationX}, {nameof(_destinationY)}: {_destinationY})";
        }
    }
}