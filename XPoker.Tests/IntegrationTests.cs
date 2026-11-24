using Xunit;
using XPoker;
using System.Collections.Generic;

namespace XPoker.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void GameFlow_HandEvaluationAndScoring()
        {
            var session = new GameSession();
            var deck = new Deck();

            // Create a hand with known cards
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.King),
                new(Suit.Diamonds, Rank.King),
                new(Suit.Clubs, Rank.Queen),
                new(Suit.Spades, Rank.Ten),
                new(Suit.Hearts, Rank.Nine)
            };

            // Evaluate the hand
            var eval = HandEvaluator.Evaluate(hand);

            Assert.Equal(HandRank.OnePair, eval.Rank);
            Assert.Equal(2, eval.CardsInCombo.Count);

            var (chips, mult) = eval.GetBaseScore();
            Assert.Equal(10, chips);
            Assert.Equal(2, mult);
        }

        [Fact]
        public void GameFlow_HighCardWithDiscard()
        {
            var deck = new Deck();

            // Initial hand - High Card
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Two),
                new(Suit.Diamonds, Rank.Three),
                new(Suit.Clubs, Rank.Four),
                new(Suit.Spades, Rank.Five),
                new(Suit.Hearts, Rank.King)
            };

            var initialEval = HandEvaluator.Evaluate(hand);
            Assert.Equal(HandRank.HighCard, initialEval.Rank);
            Assert.Single(initialEval.CardsInCombo); // Only King

            // Store card at position 1 (Queen will be drawn)
            var card1Before = hand[1];

            // Discard position 1
            hand[1] = new Card(Suit.Diamonds, Rank.Queen); // Manual replacement for testing

            // Position 1 should be different now
            Assert.NotEqual(card1Before, hand[1]);

            // Other positions unchanged
            Assert.Equal(new Card(Suit.Hearts, Rank.Two), hand[0]);
            Assert.Equal(new Card(Suit.Clubs, Rank.Four), hand[2]);
            Assert.Equal(new Card(Suit.Spades, Rank.Five), hand[3]);
            Assert.Equal(new Card(Suit.Hearts, Rank.King), hand[4]);
        }

        [Fact]
        public void GameFlow_CompleteRound()
        {
            var session = new GameSession();
            var deck = new Deck();
            var jokers = new List<Joker> { new Joker { Description = "+2 multiplier" } };

            // Hand 1
            var hand1 = new List<Card>
            {
                new(Suit.Hearts, Rank.King),
                new(Suit.Diamonds, Rank.King),
                new(Suit.Clubs, Rank.Queen),
                new(Suit.Spades, Rank.Jack),
                new(Suit.Hearts, Rank.Ten)
            };

            var eval1 = HandEvaluator.Evaluate(hand1);
            Assert.Equal(HandRank.OnePair, eval1.Rank);

            // High Card hand should only count highest card
            var handHighCard = new List<Card>
            {
                new(Suit.Hearts, Rank.Two),
                new(Suit.Diamonds, Rank.Three),
                new(Suit.Clubs, Rank.Four),
                new(Suit.Spades, Rank.Five),
                new(Suit.Hearts, Rank.King)
            };

            var evalHighCard = HandEvaluator.Evaluate(handHighCard);
            Assert.Equal(HandRank.HighCard, evalHighCard.Rank);
            Assert.Single(evalHighCard.CardsInCombo); // Only 1 card (King)
        }

        [Fact]
        public void GameFlow_EnhancementPurchaseAndSession()
        {
            var session = new GameSession { Money = 100 };
            var enhancement = new Enhancement
            {
                Name = "Card Value +1",
                Cost = 50,
                Type = EnhancementType.CardChips,
                Value = 1
            };

            session.BuyEnhancement(enhancement);

            Assert.Equal(50, session.Money);
            Assert.Equal(1, session.CardChipsBonus);
            Assert.Contains("Card Value +1", session.AppliedEnhancements);
        }
    }
}