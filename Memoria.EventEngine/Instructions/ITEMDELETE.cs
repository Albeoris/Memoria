using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RemoveItem
    /// Remove item from the player's inventory.
    /// 
    /// 1st argument: item to remove.
    /// 2nd argument: amount to remove.
    /// AT_ITEM Item (2 bytes)
    /// AT_USPIN Amount (1 bytes)
    /// ITEMDELETE = 0x049,
    /// </summary>
    internal sealed class ITEMDELETE : JsmInstruction
    {
        private readonly IJsmExpression _item;

        private readonly IJsmExpression _amount;

        private ITEMDELETE(IJsmExpression item, IJsmExpression amount)
        {
            _item = item;
            _amount = amount;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression item = reader.ArgumentInt16();
            IJsmExpression amount = reader.ArgumentByte();
            return new ITEMDELETE(item, amount);
        }
        public override String ToString()
        {
            return $"{nameof(ITEMDELETE)}({nameof(_item)}: {_item}, {nameof(_amount)}: {_amount})";
        }
    }
}