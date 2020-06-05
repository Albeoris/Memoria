using System;

namespace Memoria.Data
{
    public static class BattleAbilityHelper
    {
        public static BattleAbilityId ApplyEquipment(BattleAbilityId id, Int32 partyIndex)
        {
            if (id == BattleAbilityId.Carbuncle1)
            {
                Character character = FF9StateSystem.Common.FF9.party.GetCharacter(partyIndex);
                GemItem accessory = (GemItem) character.Equipment.Accessory;
                switch (accessory)
                {
                    case GemItem.Emerald:
                        return BattleAbilityId.Carbuncle2;
                    case GemItem.Moonstone:
                        return BattleAbilityId.Carbuncle3;
                    case GemItem.Diamond:
                        return BattleAbilityId.Carbuncle4;
                }
            }
            else if (id == BattleAbilityId.Fenrir1)
            {
                Character character = FF9StateSystem.Common.FF9.party.GetCharacter(partyIndex);
                AccessoryItem accessory = (AccessoryItem) character.Equipment.Accessory;
                if (accessory == AccessoryItem.MaidenPrayer)
                    return BattleAbilityId.Fenrir2;
            }

            return id;
        }
    }
}