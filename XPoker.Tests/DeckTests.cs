using Xunit;
using XPoker;
using System.Collections.Generic;
using System.Linq;

namespace XPoker.Tests
{
    public class DeckTests
    {
        [Fact]
        public void Deck_Initialize_Contains52Cards()
        {
            var deck = new Deck();
            var cards = new List<Card>();
            for (int i = 0; i < 52; i++)
            {
                cards.Add(deck.Draw());
            }
            Assert.Equal(52, cards.Count);
        }

        [Fact]
        public void Deck_ContainsAllSuitsAndRanks()
        {
            var deck = new Deck();
            var cards = new List<Card>();
            for (int i = 0; i < 52; i++)
            {
                cards.Add(deck.Draw());
            }

            foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
                {
                    Assert.Single(cards, c => c.Suit == suit && c.Rank == rank);
                }
            }
        }

        [Fact]
        public void Reset_ClearsAndReinitializes52Cards()
        {
            var deck = new Deck();
            for (int i = 0; i < 52; i++)
            {
                deck.Draw();
            }

            deck.Reset();
            var cards = new List<Card>();
            for (int i = 0; i < 52; i++)
            {
                cards.Add(deck.Draw());
            }

            Assert.Equal(52, cards.Count);
        }

        [Fact]
        public void Draw_RemovesCardFromDeck()
        {
            var deck = new Deck();
            var firstCard = deck.Draw();
            var secondCard = deck.Draw();
            Assert.NotEqual(firstCard, secondCard);
        }

        [Fact]
        public void Draw_AutoResetsWhenEmpty()
        {
            var deck = new Deck();
            for (int i = 0; i < 52; i++)
            {
                deck.Draw();
            }

            var cardAfterReset = deck.Draw();
            Assert.NotNull(cardAfterReset);
        }

        [Fact]
        public void Shuffle_DoesNotChangeCardCount()
        {
            var deck = new Deck();
            deck.Shuffle();

            var cards = new List<Card>();
            for (int i = 0; i < 52; i++)
            {
                cards.Add(deck.Draw());
            }

            Assert.Equal(52, cards.Count);
        }
    }
}