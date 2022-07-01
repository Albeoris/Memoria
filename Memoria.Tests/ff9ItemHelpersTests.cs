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
    }
}