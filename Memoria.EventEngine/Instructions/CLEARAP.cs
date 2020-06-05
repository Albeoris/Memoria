using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// UnlearnAbility
    /// Set an ability's AP back to 0.
    /// 
    /// 1st argument: character.
    /// 2nd argument: ability to reset.
    /// AT_LCHARACTER Character (1 bytes)
    /// AT_ABILITY Ability (1 bytes)
    /// CLEARAP = 0x0F3,
    /// </summary>
    internal sealed class CLEARAP : JsmInstruction
    {
        private readonly IJsmExpression _character;

        private readonly IJsmExpression _ability;

        private CLEARAP(IJsmExpression character, IJsmExpression ability)
        {
            _character = character;
            _ability = ability;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression character = reader.ArgumentByte();
            IJsmExpression ability = reader.ArgumentByte();
            return new CLEARAP(character, ability);
        }
        public override String ToString()
        {
            return $"{nameof(CLEARAP)}({nameof(_character)}: {_character}, {nameof(_ability)}: {_ability})";
        }
    }
}