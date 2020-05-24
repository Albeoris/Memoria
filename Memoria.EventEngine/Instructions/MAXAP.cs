using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// LearnAbility
    /// Make character learn an ability.
    /// 
    /// 1st argument: character.
    /// 2nd argument: ability to learn.
    /// AT_LCHARACTER Character (1 bytes)
    /// AT_ABILITY Ability (1 bytes)
    /// MAXAP = 0x0F4,
    /// </summary>
    internal sealed class MAXAP : JsmInstruction
    {
        private readonly IJsmExpression _character;

        private readonly IJsmExpression _ability;

        private MAXAP(IJsmExpression character, IJsmExpression ability)
        {
            _character = character;
            _ability = ability;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression character = reader.ArgumentByte();
            IJsmExpression ability = reader.ArgumentByte();
            return new MAXAP(character, ability);
        }
        public override String ToString()
        {
            return $"{nameof(MAXAP)}({nameof(_character)}: {_character}, {nameof(_ability)}: {_ability})";
        }
    }
}