using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// BattleEx
    /// Start a battle and choose its battle group.
    /// 
    /// 1st argument: rush type (unknown).
    /// 2nd argument: group.
    /// 3rd argument: gathered battle and Steiner's state (highest bit) informations.
    /// AT_SPIN Rush Type (1 bytes)
    /// AT_SPIN Battle Group (1 bytes)
    /// AT_BATTLE Battle (2 bytes)
    /// ENCOUNT2 = 0x08C,
    /// </summary>
    internal sealed class ENCOUNT2 : JsmInstruction
    {
        private readonly IJsmExpression _rushType;

        private readonly IJsmExpression _battleGroup;

        private readonly IJsmExpression _battle;

        private ENCOUNT2(IJsmExpression rushType, IJsmExpression battleGroup, IJsmExpression battle)
        {
            _rushType = rushType;
            _battleGroup = battleGroup;
            _battle = battle;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression rushType = reader.ArgumentByte();
            IJsmExpression battleGroup = reader.ArgumentByte();
            IJsmExpression battle = reader.ArgumentInt16();
            return new ENCOUNT2(rushType, battleGroup, battle);
        }
        public override String ToString()
        {
            return $"{nameof(ENCOUNT2)}({nameof(_rushType)}: {_rushType}, {nameof(_battleGroup)}: {_battleGroup}, {nameof(_battle)}: {_battle})";
        }
    }
}