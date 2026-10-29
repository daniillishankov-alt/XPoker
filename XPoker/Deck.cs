using System;
using System.Collections.Generic;
using System.Linq;

namespace XPoker
{
    public class Deck
    {
        private readonly List<Card> _cards = new();
        private static readonly Random _random = new();

        public Deck() => Reset();

        public void Reset()
        {
            _cards.Clear();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    _cards.Add(new Card(suit, rank));
                }
            }
            Shuffle();
        }

        public void Shuffle()
        {
            int n = _cards.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                (_cards[k], _cards[n]) = (_cards[n], _cards[k]);
            }
        }

        public Card Draw()
        {
            if (_cards.Count == 0) Reset();
            Card card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }
    }
}