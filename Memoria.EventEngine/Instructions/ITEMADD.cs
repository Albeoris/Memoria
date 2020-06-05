using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// AddItem
    /// Add item to the player's inventory. Only one copy of key items can be in the player's inventory.
    /// 
    /// 1st argument: item to add.
    /// 2nd argument: amount to add.
    /// AT_ITEM Item (2 bytes)
    /// AT_USPIN Amount (1 bytes)
    /// ITEMADD = 0x048,
    /// </summary>
    internal sealed class ITEMADD : JsmInstruction
    {
        private readonly IJsmExpression _item;

        private readonly IJsmExpression _amount;

        private ITEMADD(IJsmExpression item, IJsmExpression amount)
        {
            _item = item;
            _amount = amount;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression item = reader.ArgumentInt16();
            IJsmExpression amount = reader.ArgumentByte();
            return new ITEMADD(item, amount);
        }
        public override String ToString()
        {
            return $"{nameof(ITEMADD)}({nameof(_item)}: {_item}, {nameof(_amount)}: {_amount})";
        }
    }
}