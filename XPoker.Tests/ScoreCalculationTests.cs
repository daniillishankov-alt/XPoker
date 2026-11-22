using Xunit;
using XPoker;
using System.Collections.Generic;

namespace XPoker.Tests
{
    public class ScoreCalculationTests
    {
        [Fact]
        public void HighCard_OnlyHighestCardCounted()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.King),    // 10 chips (highest)
                new(Suit.Diamonds, Rank.Queen), // 10 chips (not counted)
                new(Suit.Clubs, Rank.Jack),     // 10 chips (not counted)
                new(Suit.Spades, Rank.Ten),     // 10 chips (not counted)
                new(Suit.Hearts, Rank.Nine)     // 9 chips (not counted)
            };

            var eval = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.HighCard, eval.Rank);

            // Only 1 card should be in combo (the King)
            Assert.Equal(1, eval.CardsInCombo.Count);

            var (chips, mult) = eval.GetBaseScore();
            Assert.Equal(5, chips); // Base chips for High Card
            Assert.Equal(1, mult);
        }

        [Fact]
        public void OnePair_OnlyPairCounted()
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

            // Only 2 cards (the pair) should be counted
            Assert.Equal(2, eval.CardsInCombo.Count);

            var (chips, mult) = eval.GetBaseScore();
            Assert.Equal(10, chips);
            Assert.Equal(2, mult);
        }

        [Fact]
        public void CardsInCombo_AreOnlyCountedOnce()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Ace),
                new(Suit.Diamonds, Rank.King),
                new(Suit.Clubs, Rank.Queen),
                new(Suit.Spades, Rank.Jack),
                new(Suit.Hearts, Rank.Ten)
            };

            var eval = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.HighCard, eval.Rank);

            // Only the Ace (highest) should be in combo
            Assert.Single(eval.CardsInCombo);
        }

        [Fact]
        public void Flush_AllCardsCounted()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Two),
                new(Suit.Hearts, Rank.Four),
                new(Suit.Hearts, Rank.Six),
                new(Suit.Hearts, Rank.Eight),
                new(Suit.Hearts, Rank.King)
            };

            var eval = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.Flush, eval.Rank);

            // All 5 cards in flush
            Assert.Equal(5, eval.CardsInCombo.Count);
        }

        [Fact]
        public void Straight_AllCardsCounted()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Five),
                new(Suit.Diamonds, Rank.Six),
                new(Suit.Clubs, Rank.Seven),
                new(Suit.Spades, Rank.Eight),
                new(Suit.Hearts, Rank.Nine)
            };

            var eval = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.Straight, eval.Rank);

            // All 5 cards in straight
            Assert.Equal(5, eval.CardsInCombo.Count);
        }
    }
}