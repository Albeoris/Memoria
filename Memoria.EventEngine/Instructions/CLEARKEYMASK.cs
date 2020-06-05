using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RemoveControllerMask
    /// Enable back the controller's inputs.
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
    /// CLEARKEYMASK = 0x0BA,
    /// </summary>
    internal sealed class CLEARKEYMASK : JsmInstruction
    {
        private readonly IJsmExpression _pad;

        private readonly IJsmExpression _buttons;

        private CLEARKEYMASK(IJsmExpression pad, IJsmExpression buttons)
        {
            _pad = pad;
            _buttons = buttons;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression pad = reader.ArgumentByte();
            IJsmExpression buttons = reader.ArgumentInt16();
            return new CLEARKEYMASK(pad, buttons);
        }
        public override String ToString()
        {
            return $"{nameof(CLEARKEYMASK)}({nameof(_pad)}: {_pad}, {nameof(_buttons)}: {_buttons})";
        }
    }
}