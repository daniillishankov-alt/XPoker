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

    public class Joker
    {
        public string Description { get; set; } = "+10 к множителю";
        public int ApplyMultBonus(int currentMult) => currentMult + 10;
    }

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
                    return new HandEvaluation { Rank = HandRank.FourOfAKind, HighCards =
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

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Deck deck = new Deck();
            int targetScore = 300;
            int handsLeft = 4;
            int discardsLeft = 3;

            Joker activeJoker = new Joker
            {
                Description = "+10 к множителю (Mult) за каждую комбинацию"
            };

            DrawHeader("XPoker: POKER ROGUELIKE");
            Console.WriteLine($"\n  [🃏 Джокер]: Грубый Джокер ({activeJoker.Description})");
            Console.WriteLine($"  [🎯 Цель]: Набрать {targetScore} очков за {handsLeft} хода(-ов).\n");
            Console.Write("  Нажмите любую клавишу, чтобы начать раунд...");
            Console.ReadKey(true);

            while (handsLeft > 0 && targetScore > 0)
            {
                deck.Reset();
                List<Card> hand = new();
                for (int i = 0; i < 5; i++) hand.Add(deck.Draw());

                bool played = false;
                while (!played)
                {
                    Console.Clear();
                    DrawHeader("РАУНД ПОКЕРА");

                    Console.WriteLine($"  Осталось набрать очков: [ {Math.Max(0, targetScore)} ]");
                    Console.WriteLine($"  Ходы: {handsLeft}   |   Сбросы: {discardsLeft}   |   Джокер: Грубый Джокер\n");

                    DisplayCardsFancy(hand);

                    var eval = HandEvaluator.Evaluate(hand);
                    var baseScore = eval.GetBaseScore();

                    Console.WriteLine("  ┌────────────────────────────────────────────────────────┐");
                    Console.WriteLine($"  │ Комбинация: {eval,-42} │");
                    Console.WriteLine($"  │ Базово: {baseScore.chips} фишек x {baseScore.mult} множитель                      │");
                    Console.WriteLine("  └────────────────────────────────────────────────────────┘\n");

                    Console.WriteLine("  Управление:");
                    Console.WriteLine("    [p] Сыграть руку (Play)");
                    Console.WriteLine("    [d] Сбросить карты (Discard)");
                    Console.Write("\n  Ваш выбор (p/d): ");

                    string input = Console.ReadLine() ?.Trim().ToLower() ?? "";

                    string choice = input switch
                    {
                        "з" => "p",
                        "в" => "d",
                        _ => input
                    };

                    switch (choice)
                    {
                        case "d" when discardsLeft <= 0:
                            Console.WriteLine("\n  [!] У вас больше нет сбросов! Нажмите Enter...");
                            Console.ReadLine();
                            continue;
                        case "d":
                        {
                            Console.Write("  Введите номера карт для сброса через пробел (например: 1 3): ");
                            string discardInput = Console.ReadLine() ?? "";
                            var indices = ParseIndices(discardInput);

                            if (indices.Count > 0)
                            {
                                foreach (int idx in indices.OrderByDescending(i => i))
                                {
                                    hand.RemoveAt(idx);
                                    hand.Insert(idx, deck.Draw());
                                }
                                discardsLeft--;
                            }

                            break;
                        }
                        case "p":
                        {
                            played = true;
                            int cardChips = hand.Sum(c => c.GetChipValue());
                            int totalChips = baseScore.chips + cardChips;
                            int totalMult = activeJoker.ApplyMultBonus(baseScore.mult);

                            int handScore = totalChips * totalMult;
                            targetScore -= handScore;
                            handsLeft--;

                            Console.Clear();
                            DrawHeader("ПОДСЧЕТ ОЧКОВ РУКИ");
                            DisplayCardsFancy(hand);

                            Console.WriteLine($"  Комбинация: {eval}");
                            Console.WriteLine($"  Фишки карт ({cardChips}) + База ({baseScore.chips}) = {totalChips} фишек");
                            Console.WriteLine($"  Множитель (с Джокером): x{totalMult}");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("  =========================================");
                            Console.WriteLine($"  ИГРОВОЙ СЧЕТ РУКИ: +{handScore} очков!");
                            Console.ResetColor();
                            Console.WriteLine("  =========================================\n");

                            if (targetScore > 0)
                            {
                                Console.Write("  Нажмите Enter для продолжения...");
                                Console.ReadLine();
                            }

                            break;
                        }
                    }
                }
            }

            Console.Clear();
            DrawHeader("ИТОГИ ИГРЫ");
            if (targetScore <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  🏆 ПОБЕДА! Вы разнесли блайнд в стиле Balatro!\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  💀 ПОРАЖЕНИЕ! Закончились ходы, цель не достигнута.\n");
            }
            Console.ResetColor();
            Console.WriteLine("  Спасибо за игру!");
        }

        private static void DrawHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("==========================================================");
            Console.WriteLine($"  {title}");
            Console.WriteLine("==========================================================");
            Console.ResetColor();
        }

        private static void DisplayCardsFancy(List<Card> hand)
        {
            string line1 = "  ";
            string line2 = "  ";
            string line3 = "  ";
            string line4 = "  ";
            string line5 = "  ";

            foreach (var t in hand)
            {
                string r = t.GetRankStr();
                char s = t.GetSuitChar();
                string padR = r.Length == 1 ? " " : "";

                line1 += "┌───────┐ ";
                line2 += $"│ {r}{padR}    │ ";
                line3 += $"│   {s}   │ ";
                line4 += $"│    {padR}{r} │ ";
                line5 += "└───────┘ ";
            }

            Console.WriteLine(line1);
            Console.WriteLine(line2);
            Console.WriteLine(line3);
            Console.WriteLine(line4);
            Console.WriteLine(line5);

            string indicesLine = "  ";
            for (int i = 0; i < hand.Count; i++)
            {
                indicesLine += $"   [{i + 1}]    ";
            }
            Console.WriteLine(indicesLine);

            string chipsLine = hand.Select(t => $"({t.GetChipValue()}ф)").Aggregate("  ", (current, chipVal) => current + $"{chipVal.PadLeft(6),-9} ");
            Console.WriteLine(chipsLine + "\n");
        }

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