using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetMode
    /// Set the model of the object and its head's height (used to set the dialog box's height).
    /// 
    /// 1st argument: model.
    /// 2nd argument: head's height.
    /// AT_MODEL Mode (2 bytes)
    /// AT_USPIN Height (1 bytes)
    /// MODEL = 0x02F,
    /// </summary>
    internal sealed class MODEL : JsmInstruction
    {
        private readonly IJsmExpression _mode;

        private readonly IJsmExpression _height;

        private MODEL(IJsmExpression mode, IJsmExpression height)
        {
            _mode = mode;
            _height = height;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression mode = reader.ArgumentInt16();
            IJsmExpression height = reader.ArgumentByte();
            return new MODEL(mode, height);
        }
        public override String ToString()
        {
            return $"{nameof(MODEL)}({nameof(_mode)}: {_mode}, {nameof(_height)}: {_height})";
        }
    }
}