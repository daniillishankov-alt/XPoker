using Xunit;
using XPoker;

namespace XPoker.Tests
{
    public class JokerTests
    {
        [Fact]
        public void Joker_DefaultDescription()
        {
            var joker = new Joker();
            Assert.Equal("+2 to multiplier", joker.Description);
        }

        [Fact]
        public void Joker_CanSetDescription()
        {
            var joker = new Joker { Description = "Custom Joker" };
            Assert.Equal("Custom Joker", joker.Description);
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 4)]
        [InlineData(5, 7)]
        [InlineData(10, 12)]
        public void ApplyMultBonus_AddsCorrectAmount(int currentMult, int expected)
        {
            var joker = new Joker();
            Assert.Equal(expected, joker.ApplyMultBonus(currentMult));
        }

        [Fact]
        public void ApplyMultBonus_WithZeroMultiplier()
        {
            var joker = new Joker();
            Assert.Equal(2, joker.ApplyMultBonus(0));
        }
    }
}