using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunTileAnimation
    /// Run a field tile animation.
    /// 
    /// 1st argument: background animation.
    /// 2nd argument: starting frame.
    /// AT_TILEANIM Field Animation (1 bytes)
    /// AT_USPIN Frame (1 bytes)
    /// BGAANIME = 0x05F,
    /// </summary>
    internal sealed class BGAANIME : JsmInstruction
    {
        private readonly IJsmExpression _fieldAnimation;

        private readonly IJsmExpression _frame;

        private BGAANIME(IJsmExpression fieldAnimation, IJsmExpression frame)
        {
            _fieldAnimation = fieldAnimation;
            _frame = frame;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression fieldAnimation = reader.ArgumentByte();
            IJsmExpression frame = reader.ArgumentByte();
            return new BGAANIME(fieldAnimation, frame);
        }
        public override String ToString()
        {
            return $"{nameof(BGAANIME)}({nameof(_fieldAnimation)}: {_fieldAnimation}, {nameof(_frame)}: {_frame})";
        }
    }
}