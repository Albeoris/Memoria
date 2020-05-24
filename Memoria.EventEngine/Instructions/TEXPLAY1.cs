using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunTextureAnimation
    /// Run once a model texture animation.
    /// 
    /// 1st argument: model's entry.
    /// 2nd argument: texture animation ID.
    /// AT_ENTRY Object (1 bytes)
    /// AT_USPIN Texture Animation (1 bytes)
    /// TEXPLAY1 = 0x0C1,
    /// </summary>
    internal sealed class TEXPLAY1 : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _textureAnimation;

        private TEXPLAY1(IJsmExpression @object, IJsmExpression textureAnimation)
        {
            _object = @object;
            _textureAnimation = textureAnimation;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression textureAnimation = reader.ArgumentByte();
            return new TEXPLAY1(@object, textureAnimation);
        }
        public override String ToString()
        {
            return $"{nameof(TEXPLAY1)}({nameof(_object)}: {_object}, {nameof(_textureAnimation)}: {_textureAnimation})";
        }
    }
}