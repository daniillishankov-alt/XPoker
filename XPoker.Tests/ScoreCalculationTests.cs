using Xunit;
using XPoker;
using System.Collections.Generic;
using System.Reflection;

namespace XPoker.Tests
{
    public class ScoreCalculationTests
    {
        [Fact]
        public void HighCard_CalculatesScoreCorrectly()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.King),
                new(Suit.Diamonds, Rank.Queen),
                new(Suit.Clubs, Rank.Jack),
                new(Suit.Spades, Rank.Ten),
                new(Suit.Hearts, Rank.Nine)
            };

            var eval = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.HighCard, eval.Rank);

            var (chips, mult) = eval.GetBaseScore();
            Assert.Equal(5, chips); // Base chips
            Assert.Equal(1, mult);
        }

        [Fact]
        public void OnePair_CalculatesScoreCorrectly()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.King),
                new(Suit.Diamonds, Rank.King),
                new(Suit.Clubs, Rank.Two),
                new(Suit.Spades, Rank.Three),
                new(Suit.Hearts, Rank.Four)
            };

            var eval = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.OnePair, eval.Rank);

            var (chips, mult) = eval.GetBaseScore();
            Assert.Equal(10, chips);
            Assert.Equal(2, mult);
            Assert.Equal(2, eval.CardsInCombo.Count);
        }

        [Fact]
        public void CardsInCombo_AreOnlyCountedOnce()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.King),
                new(Suit.Diamonds, Rank.King),
                new(Suit.Clubs, Rank.Queen),
                new(Suit.Spades, Rank.Ten),
                new(Suit.Hearts, Rank.Nine)
            };

            var eval = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.OnePair, eval.Rank);

            // Only 2 cards (the pair) are in combo, not all 5
            Assert.Equal(2, eval.CardsInCombo.Count);
        }
    }
}