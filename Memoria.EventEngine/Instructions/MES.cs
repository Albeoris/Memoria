using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WindowSync
    /// Display a window with text inside and wait until it closes.
    /// 
    /// 1st argument: window ID.
    /// 2nd argument: UI flag list.
    ///  3: disable bubble tail
    ///  4: mognet format
    ///  5: hide window
    ///  7: ATE window
    ///  8: dialog window
    /// 3rd argument: text to display.
    /// AT_USPIN Window ID (1 bytes)
    /// AT_BOOLLIST UI (1 bytes)
    /// AT_TEXT Text (2 bytes)
    /// MES = 0x01F,
    /// </summary>
    internal sealed class MES : JsmInstruction
    {
        private readonly IJsmExpression _windowId;

        private readonly IJsmExpression _ui;

        private readonly IJsmExpression _text;

        private MES(IJsmExpression windowId, IJsmExpression ui, IJsmExpression text)
        {
            _windowId = windowId;
            _ui = ui;
            _text = text;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression windowId = reader.ArgumentByte();
            IJsmExpression ui = reader.ArgumentByte();
            IJsmExpression text = reader.ArgumentInt16();
            return new MES(windowId, ui, text);
        }
        public override String ToString()
        {
            return $"{nameof(MES)}({nameof(_windowId)}: {_windowId}, {nameof(_ui)}: {_ui}, {nameof(_text)}: {_text})";
        }
    }
}