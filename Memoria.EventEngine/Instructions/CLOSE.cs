using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// CloseWindow
    /// Close a window.
    /// 
    /// 1st argument: window ID determined at its creation.
    /// AT_USPIN Window ID (1 bytes)
    /// CLOSE = 0x021,
    /// </summary>
    internal sealed class CLOSE : JsmInstruction
    {
        private readonly IJsmExpression _windowId;

        private CLOSE(IJsmExpression windowId)
        {
            _windowId = windowId;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression windowId = reader.ArgumentByte();
            return new CLOSE(windowId);
        }
        public override String ToString()
        {
            return $"{nameof(CLOSE)}({nameof(_windowId)}: {_windowId})";
        }
    }
}