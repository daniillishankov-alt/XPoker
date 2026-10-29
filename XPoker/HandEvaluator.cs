using System;
using System.Collections.Generic;
using System.Linq;

namespace XPoker
{
    public class HandEvaluation
    {
        public HandRank Rank { get; set; }
        public List<int> HighCards { get; set; } = new();

        public (int chips, int mult) GetBaseScore()
        {
            return Rank switch
            {
                HandRank.HighCard => (5, 1),
                HandRank.OnePair => (10, 2),
                HandRank.TwoPair => (20, 2),
                HandRank.ThreeOfAKind => (30, 3),
                HandRank.Straight => (30, 4),
                HandRank.Flush => (35, 4),
                HandRank.FullHouse => (40, 4),
                HandRank.FourOfAKind => (60, 7),
                HandRank.StraightFlush => (100, 8),
                HandRank.RoyalFlush => (100, 8),
                _ => (5, 1)
            };
        }

        public override string ToString()
        {
            return Rank switch
            {
                HandRank.HighCard => "Старшая карта",
                HandRank.OnePair => "Пара",
                HandRank.TwoPair => "Две пары",
                HandRank.ThreeOfAKind => "Тройка",
                HandRank.Straight => "Стрит",
                HandRank.Flush => "Флеш",
                HandRank.FullHouse => "Фулл-Хаус",
                HandRank.FourOfAKind => "Каре",
                HandRank.StraightFlush => "Стрит-Флеш",
                HandRank.RoyalFlush => "Роял-Флеш",
                _ => "Неизвестно"
            };
        }
    }
    public static class HandEvaluator
    {
        public static HandEvaluation Evaluate(List<Card> cards)
        {
            var sorted = cards.OrderByDescending(c => c.Rank).ToList();
            bool isFlush = cards.All(c => c.Suit == cards[0].Suit);

            bool isStraight = false;
            var distinctRanks = sorted.Select(c => (int)c.Rank).Distinct().ToList();
            int straightHighRank = 0;

            if (distinctRanks.Count == 5)
            {
                if (distinctRanks[0] - distinctRanks[4] == 4)
                {
                    isStraight = true;
                    straightHighRank = distinctRanks[0];
                }
                else if (distinctRanks[0] == (int)Rank.Ace && distinctRanks[1] == 5 && distinctRanks[4] == 2)
                {
                    isStraight = true;
                    straightHighRank = 5;
                }
            }

            if (isFlush && isStraight)
            {
                return straightHighRank == (int)Rank.Ace ? new HandEvaluation { Rank = HandRank.RoyalFlush } : new HandEvaluation { Rank = HandRank.StraightFlush, HighCards = [straightHighRank] };
            }

            var groups = sorted.GroupBy(c => c.Rank)
                               .OrderByDescending(g => g.Count())
                               .ThenByDescending(g => g.Key)
                               .ToList();

            switch (groups[0].Count())
            {
                case 4:
                    return new HandEvaluation
                    {
                        Rank = HandRank.FourOfAKind,
                        HighCards =
                        [(int)groups[0].Key, (int)groups[1].Key]
                    };
                case 3 when groups[1].Count() == 2:
                    return new HandEvaluation { Rank = HandRank.FullHouse, HighCards = [(int)groups[0].Key, (int)groups[1].Key] };
            }

            if (isFlush)
                return new HandEvaluation { Rank = HandRank.Flush, HighCards = sorted.Select(c => (int)c.Rank).ToList() };

            if (isStraight)
                return new HandEvaluation { Rank = HandRank.Straight, HighCards = [straightHighRank] };

            switch (groups[0].Count())
            {
                case 3:
                    {
                        var high = new List<int> { (int)groups[0].Key };
                        high.AddRange(groups.Skip(1).Select(g => (int)g.Key));
                        return new HandEvaluation { Rank = HandRank.ThreeOfAKind, HighCards = high };
                    }
                case 2 when groups[1].Count() == 2:
                    return new HandEvaluation { Rank = HandRank.TwoPair, HighCards = [(int)groups[0].Key, (int)groups[1].Key, (int)groups[2].Key] };
            }

            if (groups[0].Count() == 2)
            {
                var high = new List<int> { (int)groups[0].Key };
                high.AddRange(groups.Skip(1).Select(g => (int)g.Key));
                return new HandEvaluation { Rank = HandRank.OnePair, HighCards = high };
            }

            return new HandEvaluation { Rank = HandRank.HighCard, HighCards = sorted.Select(c => (int)c.Rank).ToList() };
        }
    }
}
