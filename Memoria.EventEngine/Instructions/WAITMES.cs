using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WaitWindow
    /// Wait until the window is closed.
    /// 
    /// 1st argument: window ID determined at its creation.
    /// AT_USPIN Window ID (1 bytes)
    /// WAITMES = 0x054,
    /// </summary>
    internal sealed class WAITMES : JsmInstruction
    {
        private readonly IJsmExpression _windowId;

        private WAITMES(IJsmExpression windowId)
        {
            _windowId = windowId;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression windowId = reader.ArgumentByte();
            return new WAITMES(windowId);
        }
        public override String ToString()
        {
            return $"{nameof(WAITMES)}({nameof(_windowId)}: {_windowId})";
        }
    }
}