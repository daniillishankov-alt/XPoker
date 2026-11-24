namespace XPoker
{

    public class Card(Suit suit, Rank rank)
    {
        public Suit Suit { get; } = suit;
        public Rank Rank { get; } = rank;

        public override bool Equals(object? obj)
        {
            if (obj is not Card other) return false;
            return Suit == other.Suit && Rank == other.Rank;
        }

        public override int GetHashCode() => HashCode.Combine(Suit, Rank);

        public static bool operator ==(Card? left, Card? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Card? left, Card? right) => !(left == right);

        public int GetChipValue()
        {
            return Rank switch
            {
                Rank.Ace => 11,
                Rank.King or Rank.Queen or Rank.Jack => 10,
                _ => (int)Rank
            };
        }

        public string GetRankStr()
        {
            return Rank switch
            {
                Rank.Jack => "J",
                Rank.Queen => "Q",
                Rank.King => "K",
                Rank.Ace => "A",
                _ => ((int)Rank).ToString()
            };
        }

        public char GetSuitChar()
        {
            return Suit switch
            {
                Suit.Hearts => '♥',
                Suit.Diamonds => '♦',
                Suit.Clubs => '♣',
                Suit.Spades => '♠',
                _ => '?'
            };
        }
    }
}
