using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// MoveInstantXZYEx
    /// Instantatly move an object.
    /// 
    /// 1st argument: object's entry.
    /// 2nd to 4th arguments: destination in (X, Z, Y) format.
    /// AT_ENTRY Object (1 bytes)
    /// AT_POSITION_X DestinationX (2 bytes)
    /// AT_POSITION_Z DestinationY (2 bytes)
    /// AT_POSITION_Y DestinationZ (2 bytes)
    /// DPOS3 = 0x0AD,
    /// </summary>
    internal sealed class DPOS3 : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _destinationX;

        private readonly IJsmExpression _destinationY;

        private readonly IJsmExpression _destinationZ;

        private DPOS3(IJsmExpression @object, IJsmExpression destinationX, IJsmExpression destinationY, IJsmExpression destinationZ)
        {
            _object = @object;
            _destinationX = destinationX;
            _destinationY = destinationY;
            _destinationZ = destinationZ;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression destinationX = reader.ArgumentInt16();
            IJsmExpression destinationY = reader.ArgumentInt16();
            IJsmExpression destinationZ = reader.ArgumentInt16();
            return new DPOS3(@object, destinationX, destinationY, destinationZ);
        }
        public override String ToString()
        {
            return $"{nameof(DPOS3)}({nameof(_object)}: {_object}, {nameof(_destinationX)}: {_destinationX}, {nameof(_destinationY)}: {_destinationY}, {nameof(_destinationZ)}: {_destinationZ})";
        }
    }
}