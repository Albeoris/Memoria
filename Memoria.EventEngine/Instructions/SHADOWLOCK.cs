using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// LockShadowRotation
    /// Stop updating the shadow rotation by the object's rotation.
    /// 
    /// 1st argument: locked rotation.
    /// AT_SPIN Locked Rotation (1 bytes)
    /// SHADOWLOCK = 0x083,
    /// </summary>
    internal sealed class SHADOWLOCK : JsmInstruction
    {
        private readonly IJsmExpression _lockedRotation;

        private SHADOWLOCK(IJsmExpression lockedRotation)
        {
            _lockedRotation = lockedRotation;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression lockedRotation = reader.ArgumentByte();
            return new SHADOWLOCK(lockedRotation);
        }
        public override String ToString()
        {
            return $"{nameof(SHADOWLOCK)}({nameof(_lockedRotation)}: {_lockedRotation})";
        }
    }
}