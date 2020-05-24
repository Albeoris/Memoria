using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// CureStatus
    /// Cure the status ailments of a party member.
    /// 
    /// 1st argument: character.
    /// 2nd argument: status list.
    ///  1: Petrified
    ///  2: Venom
    ///  3: Virus
    ///  4: Silence
    ///  5: Darkness
    ///  6: Trouble
    ///  7: Zombie
    /// AT_LCHARACTER Character (1 bytes)
    /// AT_BOOLLIST Statuses (1 bytes)
    /// CLEARSTATUS = 0x0D9,
    /// </summary>
    internal sealed class CLEARSTATUS : JsmInstruction
    {
        private readonly IJsmExpression _character;

        private readonly IJsmExpression _statuses;

        private CLEARSTATUS(IJsmExpression character, IJsmExpression statuses)
        {
            _character = character;
            _statuses = statuses;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression character = reader.ArgumentByte();
            IJsmExpression statuses = reader.ArgumentByte();
            return new CLEARSTATUS(character, statuses);
        }
        public override String ToString()
        {
            return $"{nameof(CLEARSTATUS)}({nameof(_character)}: {_character}, {nameof(_statuses)}: {_statuses})";
        }
    }
}