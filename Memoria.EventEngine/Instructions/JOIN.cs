using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetCharacterData
    /// Init a party's member battle and menu datas.
    /// 
    /// 1st argument: character.
    /// 2nd argument: boolean update level/don't update level.
    /// 3rd argument: equipement set to use.
    /// 4th argument: character categories ; doesn't change if all are enabled.
    ///  1: male
    ///  2: female
    ///  3: gaian
    ///  4: terran
    ///  5: temporary character
    /// 5th argument: ability and command set to use.
    /// AT_LCHARACTER Character (1 bytes)
    /// AT_BOOL Update Leve (1 bytes)
    /// AT_EQUIPSET Equipement Set (1 bytes)
    /// AT_BOOLLIST Category (1 bytes)
    /// AT_ABILITYSET Ability Set (1 bytes)
    /// JOIN = 0x0FE,
    /// </summary>
    internal sealed class JOIN : JsmInstruction
    {
        private readonly IJsmExpression _character;

        private readonly IJsmExpression _updateLeve;

        private readonly IJsmExpression _equipementSet;

        private readonly IJsmExpression _category;

        private readonly IJsmExpression _abilitySet;

        private JOIN(IJsmExpression character, IJsmExpression updateLeve, IJsmExpression equipementSet, IJsmExpression category, IJsmExpression abilitySet)
        {
            _character = character;
            _updateLeve = updateLeve;
            _equipementSet = equipementSet;
            _category = category;
            _abilitySet = abilitySet;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression character = reader.ArgumentByte();
            IJsmExpression updateLeve = reader.ArgumentByte();
            IJsmExpression equipementSet = reader.ArgumentByte();
            IJsmExpression category = reader.ArgumentByte();
            IJsmExpression abilitySet = reader.ArgumentByte();
            return new JOIN(character, updateLeve, equipementSet, category, abilitySet);
        }
        public override String ToString()
        {
            return $"{nameof(JOIN)}({nameof(_character)}: {_character}, {nameof(_updateLeve)}: {_updateLeve}, {nameof(_equipementSet)}: {_equipementSet}, {nameof(_category)}: {_category}, {nameof(_abilitySet)}: {_abilitySet})";
        }
    }
}