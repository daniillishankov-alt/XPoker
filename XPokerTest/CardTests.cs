using Xunit;
using XPoker;

namespace XPokerTests
{
    public class CardTests
    {
        [Theory]
        [InlineData(Rank.Two, 2)]
        [InlineData(Rank.Three, 3)]
        [InlineData(Rank.Four, 4)]
        [InlineData(Rank.Five, 5)]
        [InlineData(Rank.Six, 6)]
        [InlineData(Rank.Seven, 7)]
        [InlineData(Rank.Eight, 8)]
        [InlineData(Rank.Nine, 9)]
        [InlineData(Rank.Ten, 10)]
        [InlineData(Rank.Jack, 10)]
        [InlineData(Rank.Queen, 10)]
        [InlineData(Rank.King, 10)]
        [InlineData(Rank.Ace, 11)]
        public void GetChipValue_ReturnsCorrectValue(Rank rank, int expected)
        {
            var card = new Card(Suit.Hearts, rank);
            Assert.Equal(expected, card.GetChipValue());
        }

        [Theory]
        [InlineData(Rank.Two, "2")]
        [InlineData(Rank.Three, "3")]
        [InlineData(Rank.Ten, "10")]
        [InlineData(Rank.Jack, "J")]
        [InlineData(Rank.Queen, "Q")]
        [InlineData(Rank.King, "K")]
        [InlineData(Rank.Ace, "A")]
        public void GetRankStr_ReturnsCorrectString(Rank rank, string expected)
        {
            var card = new Card(Suit.Hearts, rank);
            Assert.Equal(expected, card.GetRankStr());
        }

        [Theory]
        [InlineData(Suit.Hearts, '♥')]
        [InlineData(Suit.Diamonds, '♦')]
        [InlineData(Suit.Clubs, '♣')]
        [InlineData(Suit.Spades, '♠')]
        public void GetSuitChar_ReturnsCorrectSymbol(Suit suit, char expected)
        {
            var card = new Card(suit, Rank.Ace);
            Assert.Equal(expected, card.GetSuitChar());
        }

        [Fact]
        public void Card_PropertiesSet_Correctly()
        {
            var card = new Card(Suit.Diamonds, Rank.King);
            Assert.Equal(Suit.Diamonds, card.Suit);
            Assert.Equal(Rank.King, card.Rank);
        }
    }
}