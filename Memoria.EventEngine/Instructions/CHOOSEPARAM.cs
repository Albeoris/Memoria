using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnableDialogChoices
    /// Define choices availability in dialogs using the [INIT_MULTICHOICE] text opcode.
    /// 
    /// 1st argument: boolean list for the different choices.
    /// 2nd argument: default choice selected.
    /// AT_BOOLLIST Choice List (2 bytes)
    /// AT_USPIN Default Choice (1 bytes)
    /// CHOOSEPARAM = 0x07C,
    /// </summary>
    internal sealed class CHOOSEPARAM : JsmInstruction
    {
        private readonly IJsmExpression _choiceList;

        private readonly IJsmExpression _defaultChoice;

        private CHOOSEPARAM(IJsmExpression choiceList, IJsmExpression defaultChoice)
        {
            _choiceList = choiceList;
            _defaultChoice = defaultChoice;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression choiceList = reader.ArgumentInt16();
            IJsmExpression defaultChoice = reader.ArgumentByte();
            return new CHOOSEPARAM(choiceList, defaultChoice);
        }
        public override String ToString()
        {
            return $"{nameof(CHOOSEPARAM)}({nameof(_choiceList)}: {_choiceList}, {nameof(_defaultChoice)}: {_defaultChoice})";
        }
    }
}