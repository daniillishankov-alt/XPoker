using Xunit;
using XPoker;
using System.Collections.Generic;

namespace XPoker.Tests
{
    public class HandEvaluatorTests
    {
        [Fact]
        public void Evaluate_HighCard()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Two),
                new(Suit.Diamonds, Rank.Four),
                new(Suit.Clubs, Rank.Six),
                new(Suit.Spades, Rank.Eight),
                new(Suit.Hearts, Rank.Ten)
            };

            var result = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.HighCard, result.Rank);
            Assert.Equal(5, result.CardsInCombo.Count);
        }

        [Fact]
        public void Evaluate_OnePair()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.King),
                new(Suit.Diamonds, Rank.King),
                new(Suit.Clubs, Rank.Two),
                new(Suit.Spades, Rank.Three),
                new(Suit.Hearts, Rank.Four)
            };

            var result = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.OnePair, result.Rank);
            Assert.Equal(2, result.CardsInCombo.Count);
        }

        [Fact]
        public void Evaluate_TwoPair()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.King),
                new(Suit.Diamonds, Rank.King),
                new(Suit.Clubs, Rank.Queen),
                new(Suit.Spades, Rank.Queen),
                new(Suit.Hearts, Rank.Two)
            };

            var result = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.TwoPair, result.Rank);
            Assert.Equal(4, result.CardsInCombo.Count);
        }

        [Fact]
        public void Evaluate_ThreeOfAKind()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Jack),
                new(Suit.Diamonds, Rank.Jack),
                new(Suit.Clubs, Rank.Jack),
                new(Suit.Spades, Rank.Two),
                new(Suit.Hearts, Rank.Three)
            };

            var result = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.ThreeOfAKind, result.Rank);
            Assert.Equal(3, result.CardsInCombo.Count);
        }

        [Fact]
        public void Evaluate_Straight()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Five),
                new(Suit.Diamonds, Rank.Six),
                new(Suit.Clubs, Rank.Seven),
                new(Suit.Spades, Rank.Eight),
                new(Suit.Hearts, Rank.Nine)
            };

            var result = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.Straight, result.Rank);
            Assert.Equal(5, result.CardsInCombo.Count);
        }

        [Fact]
        public void Evaluate_Straight_WheelAce()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Ace),
                new(Suit.Diamonds, Rank.Two),
                new(Suit.Clubs, Rank.Three),
                new(Suit.Spades, Rank.Four),
                new(Suit.Hearts, Rank.Five)
            };

            var result = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.Straight, result.Rank);
        }

        [Fact]
        public void Evaluate_Flush()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Two),
                new(Suit.Hearts, Rank.Four),
                new(Suit.Hearts, Rank.Six),
                new(Suit.Hearts, Rank.Eight),
                new(Suit.Hearts, Rank.Ten)
            };

            var result = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.Flush, result.Rank);
            Assert.Equal(5, result.CardsInCombo.Count);
        }

        [Fact]
        public void Evaluate_FullHouse()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.King),
                new(Suit.Diamonds, Rank.King),
                new(Suit.Clubs, Rank.King),
                new(Suit.Spades, Rank.Queen),
                new(Suit.Hearts, Rank.Queen)
            };

            var result = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.FullHouse, result.Rank);
            Assert.Equal(5, result.CardsInCombo.Count);
        }

        [Fact]
        public void Evaluate_FourOfAKind()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Ace),
                new(Suit.Diamonds, Rank.Ace),
                new(Suit.Clubs, Rank.Ace),
                new(Suit.Spades, Rank.Ace),
                new(Suit.Hearts, Rank.Two)
            };

            var result = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.FourOfAKind, result.Rank);
            Assert.Equal(4, result.CardsInCombo.Count);
        }

        [Fact]
        public void Evaluate_StraightFlush()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Five),
                new(Suit.Hearts, Rank.Six),
                new(Suit.Hearts, Rank.Seven),
                new(Suit.Hearts, Rank.Eight),
                new(Suit.Hearts, Rank.Nine)
            };

            var result = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.StraightFlush, result.Rank);
            Assert.Equal(5, result.CardsInCombo.Count);
        }

        [Fact]
        public void Evaluate_RoyalFlush()
        {
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Ten),
                new(Suit.Hearts, Rank.Jack),
                new(Suit.Hearts, Rank.Queen),
                new(Suit.Hearts, Rank.King),
                new(Suit.Hearts, Rank.Ace)
            };

            var result = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.RoyalFlush, result.Rank);
        }

        [Theory]
        [InlineData(HandRank.HighCard, 5, 1)]
        [InlineData(HandRank.OnePair, 10, 2)]
        [InlineData(HandRank.TwoPair, 20, 2)]
        [InlineData(HandRank.ThreeOfAKind, 30, 3)]
        [InlineData(HandRank.Straight, 30, 4)]
        [InlineData(HandRank.Flush, 35, 4)]
        [InlineData(HandRank.FullHouse, 40, 4)]
        [InlineData(HandRank.FourOfAKind, 60, 7)]
        [InlineData(HandRank.StraightFlush, 100, 8)]
        [InlineData(HandRank.RoyalFlush, 100, 8)]
        public void GetBaseScore_ReturnsCorrectValues(HandRank rank, int expectedChips, int expectedMult)
        {
            var eval = new HandEvaluation { Rank = rank };
            var (chips, mult) = eval.GetBaseScore();
            Assert.Equal(expectedChips, chips);
            Assert.Equal(expectedMult, mult);
        }

        [Theory]
        [InlineData(HandRank.HighCard, "High Card")]
        [InlineData(HandRank.OnePair, "One Pair")]
        [InlineData(HandRank.TwoPair, "Two Pair")]
        [InlineData(HandRank.ThreeOfAKind, "Three of a Kind")]
        [InlineData(HandRank.Straight, "Straight")]
        [InlineData(HandRank.Flush, "Flush")]
        [InlineData(HandRank.FullHouse, "Full House")]
        [InlineData(HandRank.FourOfAKind, "Four of a Kind")]
        [InlineData(HandRank.StraightFlush, "Straight Flush")]
        [InlineData(HandRank.RoyalFlush, "Royal flush")]
        public void ToString_ReturnsCorrectHandName(HandRank rank, string expected)
        {
            var eval = new HandEvaluation { Rank = rank };
            Assert.Equal(expected, eval.ToString());
        }
    }
}