using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunAnimationEx
    /// Play an object's animation.
    /// 
    /// 1st argument: object's entry.
    /// 2nd argument: animation ID.
    /// AT_ENTRY Object (1 bytes)
    /// AT_ANIMATION Animation ID (2 bytes)
    /// DANIM = 0x0BD,
    /// </summary>
    internal sealed class DANIM : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _animationId;

        private DANIM(IJsmExpression @object, IJsmExpression animationId)
        {
            _object = @object;
            _animationId = animationId;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression animationId = reader.ArgumentInt16();
            return new DANIM(@object, animationId);
        }
        public override String ToString()
        {
            return $"{nameof(DANIM)}({nameof(_object)}: {_object}, {nameof(_animationId)}: {_animationId})";
        }
    }
}