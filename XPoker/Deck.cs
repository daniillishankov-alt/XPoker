namespace XPoker
{
    public class Deck
    {
        private readonly Queue<Card> _cards = new();
        private static readonly Random _random = new();

        public Deck() => Reset();

        public void Reset()
        {
            _cards.Clear();
            var cardList = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    cardList.Add(new Card(suit, rank));
                }
            }
            Shuffle(cardList);
            foreach (var card in cardList)
            {
                _cards.Enqueue(card);
            }
        }

        public void Shuffle()
        {
            var cardList = new List<Card>(_cards);
            _cards.Clear();
            Shuffle(cardList);
            foreach (var card in cardList)
            {
                _cards.Enqueue(card);
            }
        }

        private void Shuffle(List<Card> cards)
        {
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                (cards[k], cards[n]) = (cards[n], cards[k]);
            }
        }

        public Card Draw()
        {
            if (_cards.Count == 0) Reset();
            return _cards.Dequeue();
        }
    }
}