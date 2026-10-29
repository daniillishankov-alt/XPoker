using System;
using System.Collections.Generic;
using System.Linq;

namespace XPoker
{
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    public enum Rank
    {
        Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
        Jack = 11, Queen = 12, King = 13, Ace = 14
    }

    public enum HandRank
    {
        HighCard = 1, OnePair, TwoPair, ThreeOfAKind, Straight,
        Flush, FullHouse, FourOfAKind, StraightFlush, RoyalFlush
    }

    public class Card(Suit suit, Rank rank)
    {
        public Suit Suit { get; } = suit;
        public Rank Rank { get; } = rank;

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
