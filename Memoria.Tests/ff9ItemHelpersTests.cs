using FF9;
using Memoria.Data;
using Moq;

namespace Memoria.Tests
{
    public class ff9ItemHelpersTests
    {
        private const int maxItemCount = byte.MaxValue + 1;
        private static readonly Random rnd = new();

        [Fact]
        public void GetPtr_WhenInventoryContainsItem_ReturnsItem()
        {
            FF9ITEM? item = CreateRandomNonZeroCountItem();

            var inventory = new FF9ITEM[maxItemCount];
            inventory[0] = item;

            FF9ITEM? actual = ff9itemHelpers.FF9Item_GetPtr(item.id, inventory);

            Assert.Same(item, actual);
        }

        [Fact]
        public void GetPtr_WhenInventoryNotContainsItem_ReturnsNull()
        {
            FF9ITEM item = CreateRandomNonZeroCountItem();
            var inventory = new FF9ITEM[maxItemCount];

            FF9ITEM? actual = ff9itemHelpers.FF9Item_GetPtr(item.id, inventory);

            Assert.Null(actual);
        }

        [Fact]
        public void GetPtr_WhenInventoryContainsZeroItem_ReturnsNull()
        {
            FF9ITEM? item = CreateRandomZeroCountItem();

            var inventory = new FF9ITEM[maxItemCount];
            inventory[0] = item;

            FF9ITEM? actual = ff9itemHelpers.FF9Item_GetPtr(item.id, inventory);

            Assert.Null(actual);
        }

        private static FF9ITEM_DATA CreateRandomItemData()
        {
            return new FF9ITEM_DATA(name: It.IsAny<ushort>(),
                                    help: It.IsAny<ushort>(),
                                    price: It.IsAny<ushort>(),
                                    equip: It.IsAny<ushort>(),
                                    shape: It.IsAny<byte>(),
                                    color: It.IsAny<byte>(),
                                    eq_lv: It.IsAny<byte>(),
                                    bonus: It.IsAny<byte>(),
                                    ability: It.IsAny<byte[]>(),
                                    type: It.IsAny<byte>(),
                                    sort: It.IsAny<byte>(),
                                    pad: It.IsAny<byte>());
        }

        private static FF9ITEM CreateRandomNonZeroCountItem()
        {
            return new FF9ITEM((byte)rnd.Next(1, byte.MaxValue), (byte)rnd.Next(1, 99));
        }

        private static FF9ITEM CreateRandomZeroCountItem()
        {
            return new FF9ITEM((byte)rnd.Next(1, byte.MaxValue), count: 0);
        }

        [Theory]
        [InlineData(ItemType.Weapon, FF9FEQP_EQUIP.FF9FEQP_EQUIP_WEAPON)]
        [InlineData(ItemType.Armlet, FF9FEQP_EQUIP.FF9FEQP_EQUIP_WRIST)]
        [InlineData(ItemType.Helmet, FF9FEQP_EQUIP.FF9FEQP_EQUIP_HEAD)]
        [InlineData(ItemType.Armor, FF9FEQP_EQUIP.FF9FEQP_EQUIP_BODY)]
        [InlineData(ItemType.Accessory, FF9FEQP_EQUIP.FF9FEQP_EQUIP_ACCESSORY)]
        public void GetEquipPart_WhenItemTypeHasEquipPart_ReturnsEquipPart(ItemType itemType, FF9FEQP_EQUIP expected)
        {
            FF9ITEM_DATA item = CreateRandomItemData();
            item.type = (byte)itemType;

            int actual = ff9itemHelpers.FF9Item_GetEquipPart(item);

            Assert.Equal(expected, (FF9FEQP_EQUIP)actual);
        }

        [Fact]
        // Creates party, where everyone is not in the party. Therefor item count
        // can't be made.
        public void GetEquipCount_WhenPartyIsZero_ReturnsZero()
        {
            PLAYER[] party = new PLAYER[]
            {
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer()
            };

            const int itemId = 1;
            int actual = ff9itemHelpers.FF9Item_GetEquipCount(itemId, party);
            Assert.Equal(0, actual);
        }


        [Fact]
        // Creates party, where everyone, expect one member is in the party, with
        // one item equipped as a weapon.
        public void GetEquipCount_WhenGivenIdExists_ReturnsEquipCount()
        {
            const int itemId = 1;

            PLAYER[] party = new PLAYER[]
            {
                CreateNewPlayer(party:1, itemId: itemId, equipPart:(byte)FF9FEQP_EQUIP.FF9FEQP_EQUIP_WEAPON),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer(),
                CreateNewPlayer()
            };

            int actual = ff9itemHelpers.FF9Item_GetEquipCount(itemId, party);
            Assert.Equal(1, actual);
        }

        private static PLAYER CreateNewPlayer(byte party = 0, byte itemId = 0, byte equipPart = byte.MaxValue)
        {
            var equipment = new CharacterEquipment();
            if (equipPart != byte.MaxValue && itemId != 0)
            {
                equipment[equipPart] = itemId;
            }

            return new PLAYER()
            {
                info = new PLAYER_INFO(It.IsAny<byte>(),
                                       It.IsAny<byte>(),
                                       It.IsAny<byte>(),
                                       It.IsAny<byte>(),
                                       party,
                                       It.IsAny<byte>()),
                equip = equipment
            };
        }
    }
}