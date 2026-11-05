using System;
using System.Collections.Generic;
using System.Linq;

namespace XPoker
{
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
                Description = "+10 multiplier for each combo"
            };

            DrawHeader("XPoker: POKER ROGUELIKE");
            Console.WriteLine($"\n  [🃏 Joker]: Грубый Джокер ({activeJoker.Description})");
            Console.WriteLine($"  [🎯 Goal]: Get {targetScore} scores.\n");
            Console.Write("  Press any button to start...");
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
                    DrawHeader("Poker round");

                    Console.WriteLine($"  Scores left: [ {Math.Max(0, targetScore)} ]");
                    Console.WriteLine($"  Moves: {handsLeft}   |   Discards: {discardsLeft}   |   Joker: Грубый Джокер\n");

                    DisplayCardsFancy(hand);

                    var eval = HandEvaluator.Evaluate(hand);
                    var baseScore = eval.GetBaseScore();

                    Console.WriteLine("  ┌────────────────────────────────────────────────────────┐");
                    Console.WriteLine($"  │ Combo: {eval,-42} │");
                    Console.WriteLine($"  │ Base: {baseScore.chips} chips x {baseScore.mult} multiplier                      │");
                    Console.WriteLine("  └────────────────────────────────────────────────────────┘\n");

                    Console.WriteLine("  Control:");
                    Console.WriteLine("    [p] Play hand");
                    Console.WriteLine("    [d] Discard (Discard)");
                    Console.Write("\n  Your choice (p/d): ");

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
                            Console.WriteLine("\n  [!] You don't have any discards! Press Enter...");
                            Console.ReadLine();
                            continue;
                        case "d":
                        {
                            Console.Write("  Input card number with space between (example: 1 3): ");
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
                            DrawHeader("COUNTING HAND SCORE");
                            DisplayCardsFancy(hand);

                            Console.WriteLine($"  Combo: {eval}");
                            Console.WriteLine($"  Card chips ({cardChips}) + Base ({baseScore.chips}) = {totalChips} chips");
                            Console.WriteLine($"  Multiplier (with Joker): x{totalMult}");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("  =========================================");
                            Console.WriteLine($"  GAME HAND SCORE: +{handScore}");
                            Console.ResetColor();
                            Console.WriteLine("  =========================================\n");

                            if (targetScore > 0)
                            {
                                Console.Write("  Press Enter to continue...");
                                Console.ReadLine();
                            }

                            break;
                        }
                    }
                }
            }

            Console.Clear();
            DrawHeader("GAME RESULTS");
            if (targetScore <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  🏆 VICTORY! You crushed the blind, Balatro-style.!\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  💀 DEFEAT! No moves left, goal doesn't achieved.\n");
            }
            Console.ResetColor();
            Console.WriteLine("  Thanks for playing!");
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