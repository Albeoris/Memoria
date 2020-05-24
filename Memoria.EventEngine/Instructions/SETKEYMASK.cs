using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// AddControllerMask
    /// Prevent the input to be processed by the game.
    /// 
    /// 1st argument: pad number (should only be 0 or 1).
    /// 2nd argument: button list.
    /// 1: Select
    /// 4: Start
    /// 5: Up
    /// 6: Right
    /// 7: Down
    /// 8: Left
    /// 9: L2
    /// 10: R2
    /// 11: L1
    /// 12: R1
    /// 13: Triangle
    /// 14: Circle
    /// 15: Cross
    /// 16: Square
    /// 17: Cancel
    /// 18: Confirm
    /// 20: Moogle
    /// 21: L1 Ex
    /// 22: R1 Ex
    /// 23: L2 Ex
    /// 24: R2 Ex
    /// 25: Menu
    /// 26: Select Ex
    /// AT_USPIN Pad (1 bytes)
    /// AT_BUTTONLIST Buttons (2 bytes)
    /// SETKEYMASK = 0x0B9,
    /// </summary>
    internal sealed class SETKEYMASK : JsmInstruction
    {
        private readonly IJsmExpression _pad;

        private readonly IJsmExpression _buttons;

        private SETKEYMASK(IJsmExpression pad, IJsmExpression buttons)
        {
            _pad = pad;
            _buttons = buttons;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression pad = reader.ArgumentByte();
            IJsmExpression buttons = reader.ArgumentInt16();
            return new SETKEYMASK(pad, buttons);
        }
        public override String ToString()
        {
            return $"{nameof(SETKEYMASK)}({nameof(_pad)}: {_pad}, {nameof(_buttons)}: {_buttons})";
        }
    }
}