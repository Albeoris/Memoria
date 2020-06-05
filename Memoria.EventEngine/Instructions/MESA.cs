using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WindowSyncEx
    /// Display a window with text inside and wait until it closes.
    /// 
    /// 1st argument: talker's entry.
    /// 2nd argument: window ID.
    /// 3rd argument: UI flag list.
    ///  3: disable bubble tail
    ///  4: mognet format
    ///  5: hide window
    ///  7: ATE window
    ///  8: dialog window
    /// 4th argument: text to display.
    /// AT_ENTRY Talker (1 bytes)
    /// AT_USPIN Window ID (1 bytes)
    /// AT_BOOLLIST UI (1 bytes)
    /// AT_TEXT Text (2 bytes)
    /// MESA = 0x095,
    /// </summary>
    internal sealed class MESA : JsmInstruction
    {
        private readonly IJsmExpression _talker;

        private readonly IJsmExpression _windowId;

        private readonly IJsmExpression _ui;

        private readonly IJsmExpression _text;

        private MESA(IJsmExpression talker, IJsmExpression windowId, IJsmExpression ui, IJsmExpression text)
        {
            _talker = talker;
            _windowId = windowId;
            _ui = ui;
            _text = text;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression talker = reader.ArgumentByte();
            IJsmExpression windowId = reader.ArgumentByte();
            IJsmExpression ui = reader.ArgumentByte();
            IJsmExpression text = reader.ArgumentInt16();
            return new MESA(talker, windowId, ui, text);
        }
        public override String ToString()
        {
            return $"{nameof(MESA)}({nameof(_talker)}: {_talker}, {nameof(_windowId)}: {_windowId}, {nameof(_ui)}: {_ui}, {nameof(_text)}: {_text})";
        }
    }
}