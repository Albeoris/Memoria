using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetObjectLogicalSize
    /// Set different size informations of the object.
    /// 
    /// 1st argument: size for pathing.
    /// 2nd argument: collision radius.
    /// 3rd argument: talk distance.
    /// AT_SPIN Walk Radius (1 bytes)
    /// AT_USPIN Collision Radius (1 bytes)
    /// AT_USPIN Talk Distance (1 bytes)
    /// RADIUS = 0x04B,
    /// </summary>
    internal sealed class RADIUS : JsmInstruction
    {
        private readonly IJsmExpression _walkRadius;

        private readonly IJsmExpression _collisionRadius;

        private readonly IJsmExpression _talkDistance;

        private RADIUS(IJsmExpression walkRadius, IJsmExpression collisionRadius, IJsmExpression talkDistance)
        {
            _walkRadius = walkRadius;
            _collisionRadius = collisionRadius;
            _talkDistance = talkDistance;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression walkRadius = reader.ArgumentByte();
            IJsmExpression collisionRadius = reader.ArgumentByte();
            IJsmExpression talkDistance = reader.ArgumentByte();
            return new RADIUS(walkRadius, collisionRadius, talkDistance);
        }
        public override String ToString()
        {
            return $"{nameof(RADIUS)}({nameof(_walkRadius)}: {_walkRadius}, {nameof(_collisionRadius)}: {_collisionRadius}, {nameof(_talkDistance)}: {_talkDistance})";
        }
    }
}