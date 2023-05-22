using FF9;

namespace Memoria.Tests
{
    public class ComnTests
    {
        [Theory]
        [InlineData(UInt64.MinValue, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        [InlineData(627877, 9)]
        [InlineData(UInt64.MaxValue, 64)]
        public void CountBits_Should_Return_NumberOfSetBits(UInt64 bitList, Byte setBits)
        {
            Assert.Equal(Comn.countBits(bitList), setBits);
        }
    }
}