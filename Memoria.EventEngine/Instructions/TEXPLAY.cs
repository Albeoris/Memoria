using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnableTextureAnimation
    /// Run a model texture animation and make it loop.
    /// 
    /// 1st argument: model's entry.
    /// 2nd argument: texture animation ID.
    /// AT_ENTRY Object (1 bytes)
    /// AT_USPIN Texture Animation (1 bytes)
    /// TEXPLAY = 0x0C0,
    /// </summary>
    internal sealed class TEXPLAY : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _textureAnimation;

        private TEXPLAY(IJsmExpression @object, IJsmExpression textureAnimation)
        {
            _object = @object;
            _textureAnimation = textureAnimation;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression textureAnimation = reader.ArgumentByte();
            return new TEXPLAY(@object, textureAnimation);
        }
        public override String ToString()
        {
            return $"{nameof(TEXPLAY)}({nameof(_object)}: {_object}, {nameof(_textureAnimation)}: {_textureAnimation})";
        }
    }
}