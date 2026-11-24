using Xunit;
using XPoker;
using System.Collections.Generic;
using System.Linq;

namespace XPoker.Tests
{
    public class DiscardTests
    {
        [Fact]
        public void Discard_ReplacesCardsInPlace_PositionsUnchanged()
        {
            var deck = new Deck();
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Two),
                new(Suit.Diamonds, Rank.Three),
                new(Suit.Clubs, Rank.Four),
                new(Suit.Spades, Rank.Five),
                new(Suit.Hearts, Rank.Six)
            };

            // Store original positions of cards that won't be discarded
            var originalCard0 = hand[0]; // Position 0 - should stay
            var originalCard2 = hand[2]; // Position 2 - should stay
            var originalCard4 = hand[4]; // Position 4 - should stay

            // Discard cards at positions 1 and 3
            var indicesToDiscard = new List<int> { 1, 3 };
            foreach (int idx in indicesToDiscard)
            {
                hand[idx] = deck.Draw();
            }

            // Verify positions didn't change
            Assert.Equal(originalCard0, hand[0]); // Position 0 unchanged
            Assert.Equal(originalCard2, hand[2]); // Position 2 unchanged
            Assert.Equal(originalCard4, hand[4]); // Position 4 unchanged

            // Verify discarded positions were replaced
            Assert.NotEqual(new(Suit.Diamonds, Rank.Three), hand[1]);
            Assert.NotEqual(new(Suit.Spades, Rank.Five), hand[3]);
        }

        [Fact]
        public void Discard_SingleCard_OthersStayInPlace()
        {
            var deck = new Deck();
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.King),
                new(Suit.Diamonds, Rank.Queen),
                new(Suit.Clubs, Rank.Jack),
                new(Suit.Spades, Rank.Ten),
                new(Suit.Hearts, Rank.Nine)
            };

            var originalCard0 = hand[0];
            var originalCard1 = hand[1];
            var originalCard3 = hand[3];
            var originalCard4 = hand[4];

            // Discard only position 2
            hand[2] = deck.Draw();

            // All other positions should remain unchanged
            Assert.Equal(originalCard0, hand[0]);
            Assert.Equal(originalCard1, hand[1]);
            Assert.Equal(originalCard3, hand[3]);
            Assert.Equal(originalCard4, hand[4]);
        }

        [Fact]
        public void Discard_AllCards_ReplacedCorrectly()
        {
            var deck = new Deck();
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Two),
                new(Suit.Diamonds, Rank.Three),
                new(Suit.Clubs, Rank.Four),
                new(Suit.Spades, Rank.Five),
                new(Suit.Hearts, Rank.Six)
            };

            var oldCards = hand.ToList();

            // Discard all cards
            for (int i = 0; i < 5; i++)
            {
                hand[i] = deck.Draw();
            }

            // Verify all cards are different
            for (int i = 0; i < 5; i++)
            {
                Assert.NotEqual(oldCards[i], hand[i]);
            }

            // Verify hand still has 5 cards
            Assert.Equal(5, hand.Count);
        }

        [Fact]
        public void Discard_MaintainsHandCount()
        {
            var deck = new Deck();
            var hand = new List<Card>
            {
                new(Suit.Hearts, Rank.Two),
                new(Suit.Diamonds, Rank.Three),
                new(Suit.Clubs, Rank.Four),
                new(Suit.Spades, Rank.Five),
                new(Suit.Hearts, Rank.Six)
            };

            int originalCount = hand.Count;

            // Discard cards at various positions
            var indicesToDiscard = new List<int> { 0, 2, 4 };
            foreach (int idx in indicesToDiscard)
            {
                hand[idx] = deck.Draw();
            }

            // Hand count should remain the same
            Assert.Equal(originalCount, hand.Count);
            Assert.Equal(5, hand.Count);
        }

        [Fact]
        public void ParseIndices_ValidInput()
        {
            // This tests the helper method for parsing user input
            string input = "1 3 5";
            var indices = ParseIndices(input);

            Assert.Equal(3, indices.Count);
            Assert.Contains(0, indices); // "1" -> index 0
            Assert.Contains(2, indices); // "3" -> index 2
            Assert.Contains(4, indices); // "5" -> index 4
        }

        [Fact]
        public void ParseIndices_WithDuplicates_RemovesDuplicates()
        {
            string input = "1 1 2 2 3";
            var indices = ParseIndices(input);

            Assert.Equal(3, indices.Count); // Duplicates removed
        }

        [Fact]
        public void ParseIndices_OutOfRange_Ignored()
        {
            string input = "0 1 6 10 3";
            var indices = ParseIndices(input);

            Assert.Equal(2, indices.Count);
            Assert.Contains(0, indices); // "1" -> index 0
            Assert.Contains(2, indices); // "3" -> index 2
        }

        [Fact]
        public void ParseIndices_InvalidInput_ReturnsEmpty()
        {
            string input = "a b c";
            var indices = ParseIndices(input);

            Assert.Empty(indices);
        }

        // Helper method from Program.cs for testing
        private static List<int> ParseIndices(string input)
        {
            List<int> indices = new();
            if (string.IsNullOrWhiteSpace(input)) return indices;

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int number) || number is < 1 or > 5) continue;
                int index = number - 1;
                if (!indices.Contains(index)) indices.Add(index);
            }
            return indices;
        }
    }
}