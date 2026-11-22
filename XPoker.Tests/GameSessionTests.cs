using Xunit;
using XPoker;

namespace XPoker.Tests
{
    public class GameSessionTests
    {
        [Fact]
        public void GameSession_Initialize_WithDefaultValues()
        {
            var session = new GameSession();
            Assert.Equal(1, session.CurrentRound);
            Assert.Equal(0, session.Score);
            Assert.Equal(5, session.HandsLeft);
            Assert.Equal(100, session.Money);
        }

        [Theory]
        [InlineData(1, 100)]
        [InlineData(2, 200)]
        [InlineData(3, 300)]
        public void GetTargetScore_ReturnsCorrectValue(int round, int expected)
        {
            var session = new GameSession { CurrentRound = round };
            Assert.Equal(expected, session.GetTargetScore());
        }

        [Fact]
        public void ResetForNewRound_ResetsHandsAndScore()
        {
            var session = new GameSession { Score = 100, HandsLeft = 2, Money = 150 };
            session.ResetForNewRound();

            Assert.Equal(5, session.HandsLeft);
            Assert.Equal(0, session.Score);
            Assert.Equal(175, session.Money); // +25 bonus
        }

        [Fact]
        public void BuyEnhancement_DeductsMoneyAndAddsEnhancement()
        {
            var session = new GameSession { Money = 100 };
            var enhancement = new Enhancement
            {
                Name = "Test",
                Cost = 50,
                Type = EnhancementType.CardChips,
                Value = 1
            };

            session.BuyEnhancement(enhancement);

            Assert.Equal(50, session.Money);
            Assert.Contains("Test", session.AppliedEnhancements);
        }

        [Fact]
        public void BuyEnhancement_DoesNotBuyIfInsufficientFunds()
        {
            var session = new GameSession { Money = 30 };
            var enhancement = new Enhancement { Name = "Test", Cost = 50 };

            session.BuyEnhancement(enhancement);

            Assert.Equal(30, session.Money);
            Assert.Empty(session.AppliedEnhancements);
        }

        [Fact]
        public void BuyEnhancement_AppliesCardChipsBonus()
        {
            var session = new GameSession { Money = 100, CardChipsBonus = 0 };
            var enhancement = new Enhancement
            {
                Name = "Card+1",
                Cost = 50,
                Type = EnhancementType.CardChips,
                Value = 2
            };

            session.BuyEnhancement(enhancement);

            Assert.Equal(2, session.CardChipsBonus);
        }
    }
}