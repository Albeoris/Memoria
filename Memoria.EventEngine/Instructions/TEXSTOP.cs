using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// StopTextureAnimation
    /// Stop playing the model texture animation.
    /// 
    /// 1st argument: model's entry.
    /// 2nd argument: texture animation ID.
    /// AT_ENTRY Object (1 bytes)
    /// AT_USPIN Texture Animation (1 bytes)
    /// TEXSTOP = 0x0C2,
    /// </summary>
    internal sealed class TEXSTOP : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _textureAnimation;

        private TEXSTOP(IJsmExpression @object, IJsmExpression textureAnimation)
        {
            _object = @object;
            _textureAnimation = textureAnimation;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression textureAnimation = reader.ArgumentByte();
            return new TEXSTOP(@object, textureAnimation);
        }
        public override String ToString()
        {
            return $"{nameof(TEXSTOP)}({nameof(_object)}: {_object}, {nameof(_textureAnimation)}: {_textureAnimation})";
        }
    }
}