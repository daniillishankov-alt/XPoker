namespace XPoker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            GameSession session = new();
            Shop shop = new();
            Deck deck = new();
            List<Joker> activeJokers = new()
            {
                new Joker { Description = "+10 multiplier for each combo" }
            };

            DrawHeader("🃏 XPoker: ROGUELIKE EDITION 🃏");
            Console.WriteLine("\n  Welcome to the most intense poker experience!");
            Console.WriteLine("  Defeat enemies by scoring points in poker hands.\n");
            Console.Write("  Press any key to start...");
            Console.ReadKey(true);

            // Main game loop - multiple rounds
            while (session.CurrentRound <= 3)
            {
                Console.Clear();
                DrawHeader($"⚔️  ROUND {session.CurrentRound} ⚔️");
                Console.WriteLine($"\n  Target Score: {session.GetTargetScore()} 🎯");
                Console.WriteLine($"  Current Money: {session.Money}💰");
                Console.WriteLine($"  Applied Enhancements: {(session.AppliedEnhancements.Count > 0 ? string.Join(", ", session.AppliedEnhancements) : "None")}");
                Console.Write("\n  Press Enter to start round...");
                Console.ReadLine();

                // Shop before round
                DisplayShop(shop, session);

                // Round gameplay - multiple hands
                session.ResetForNewRound();
                bool roundWon = false;
                int handNumber = 1;

                while (session.HandsLeft > 0 && !roundWon)
                {
                    deck.Reset();
                    List<Card> hand = new();
                    for (int i = 0; i < 5; i++) hand.Add(deck.Draw());

                    int discardsLeft = 3;
                    bool handPlayed = false;

                    while (!handPlayed)
                    {
                        Console.Clear();
                        DrawHeader($"🎰 Hand {handNumber} / 5");

                        int scoreNeeded = session.GetTargetScore() - session.Score;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"  💰 Round Score: {session.Score} / {session.GetTargetScore()}");
                        Console.WriteLine($"  🎯 Score Needed: {scoreNeeded}\n");
                        Console.ResetColor();
                        Console.WriteLine($"  Hands left: {session.HandsLeft} | Discards: {discardsLeft} | Money: {session.Money}💰\n");

                        DisplayCardsFancy(hand);

                        var eval = HandEvaluator.Evaluate(hand);
                        var baseScore = eval.GetBaseScore();
                        int predictedScore = CalculatePredictedScore(hand, eval, baseScore, session, activeJokers);

                        Console.WriteLine("  ┌────────────────────────────────────────────────────────┐");
                        Console.WriteLine($"  │ Combo: {eval,-42} │");
                        Console.WriteLine($"  │ Base: {baseScore.chips} chips x {baseScore.mult} mult                      │");
                        Console.WriteLine($"  │ Predicted Score: {predictedScore}🎯{"",-27} │");
                        Console.WriteLine("  └────────────────────────────────────────────────────────┘\n");

                        Console.WriteLine("  [p] Play hand | [d] Discard | [q] Quit round");
                        Console.Write("\n  Choice (p/d/q): ");

                        string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                        string choice = input switch
                        {
                            "з" => "p",
                            "в" => "d",
                            "й" => "q",
                            _ => input
                        };

                        switch (choice)
                        {
                            case "d" when discardsLeft <= 0:
                                Console.WriteLine("\n  ❌ No discards left! Press Enter...");
                                Console.ReadLine();
                                continue;

                            case "d":
                                Console.Write("  Card numbers to discard (1 2 3): ");
                                string discardInput = Console.ReadLine() ?? "";
                                var indices = ParseIndices(discardInput);

                                if (indices.Count > 0)
                                {
                                    foreach (int idx in indices.OrderByDescending(i => i))
                                    {
                                        hand.RemoveAt(idx);
                                        hand.Add(deck.Draw());
                                    }
                                    discardsLeft--;
                                }
                                break;

                            case "q":
                                Console.WriteLine("\n  Quitting round...");
                                Console.ReadLine();
                                session.HandsLeft = 0;
                                break;

                            case "p":
                                handPlayed = true;
                                int handScore = CalculateHandScore(hand, eval, baseScore, session, activeJokers);
                                session.Score += handScore;
                                session.HandsLeft--;
                                handNumber++;

                                Console.Clear();
                                DrawHeader("✨ HAND RESULT ✨");
                                DisplayCardsFancy(hand);
                                Console.WriteLine($"  Combo: {eval}");
                                Console.WriteLine($"  Hand Score: +{handScore}");
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine($"  Round Total: {session.Score} / {session.GetTargetScore()}");
                                Console.ResetColor();

                                if (session.Score >= session.GetTargetScore())
                                {
                                    Console.WriteLine("\n  ✅ ROUND WON!");
                                    roundWon = true;
                                }
                                else if (session.HandsLeft > 0)
                                {
                                    Console.Write("\n  Press Enter to continue...");
                                    Console.ReadLine();
                                }

                                break;
                        }
                    }
                }

                // Round end
                Console.Clear();
                if (session.Score >= session.GetTargetScore())
                {
                    DrawHeader("🏆 ROUND CLEARED 🏆");
                    Console.WriteLine($"\n  You defeated round {session.CurrentRound}!");
                    Console.WriteLine($"  Final Score: {session.Score}");
                    Console.WriteLine($"  Money Earned: 50💰");
                    session.Money += 50;
                    session.CurrentRound++;
                }
                else
                {
                    DrawHeader("💀 ROUND FAILED 💀");
                    Console.WriteLine($"\n  You couldn't reach the target score.");
                    Console.WriteLine($"  Score: {session.Score} / {session.GetTargetScore()}");
                    break;
                }

                Console.Write("\n  Press Enter to continue...");
                Console.ReadLine();
            }

            // Game end
            Console.Clear();
            DrawHeader("🎮 GAME OVER 🎮");
            if (session.CurrentRound > 3)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  🏆 VICTORY! You defeated all rounds!");
                Console.WriteLine($"  Final Money: {session.Money}💰");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  💀 DEFEAT! You couldn't complete all rounds.");
            }
            Console.ResetColor();
            Console.WriteLine("\n  Thanks for playing!");
            Console.ReadLine();
        }

        private static void DisplayShop(Shop shop, GameSession session)
        {
            var shopItems = shop.GetRandomShop(3);

            while (true)
            {
                Console.Clear();
                DrawHeader("🛒 SHOP 🛒");
                Console.WriteLine($"\n  Your Money: {session.Money}💰\n");

                for (int i = 0; i < shopItems.Count; i++)
                {
                    string affordability = shopItems[i].Cost <= session.Money ? "✅" : "❌";
                    Console.WriteLine($"  [{i + 1}] {shopItems[i]} {affordability}");
                }

                Console.WriteLine($"\n  [0] Exit Shop");
                Console.Write($"  Choose item (0-{shopItems.Count}): ");

                if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > shopItems.Count)
                    continue;

                if (choice == 0)
                    break;

                var selectedItem = shopItems[choice - 1];
                if (session.Money >= selectedItem.Cost)
                {
                    session.BuyEnhancement(selectedItem);
                    Console.WriteLine($"\n  ✅ Bought {selectedItem.Name}!");
                    shopItems.Remove(selectedItem);

                    var newItems = shop.GetRandomShop(1);
                    if (newItems.Count > 0)
                        shopItems.Add(newItems[0]);
                }
                else
                {
                    Console.WriteLine($"\n  ❌ Not enough money!");
                }

                Console.ReadLine();

                if (shopItems.Count == 0)
                    break;
            }
        }

        private static int CalculatePredictedScore(List<Card> hand, HandEvaluation eval,
            (int chips, int mult) baseScore, GameSession session, List<Joker> jokers)
        {
            int cardChips = hand.Sum(c => c.GetChipValue()) + (session.CardChipsBonus * hand.Count);
            int totalChips = baseScore.chips + cardChips;
            int totalMult = baseScore.mult + GetHandMultiplierBonus(eval.Rank, session);

            foreach (var joker in jokers)
                totalMult = joker.ApplyMultBonus(totalMult);

            return totalChips * totalMult;
        }

        private static int CalculateHandScore(List<Card> hand, HandEvaluation eval,
            (int chips, int mult) baseScore, GameSession session, List<Joker> jokers)
        {
            return CalculatePredictedScore(hand, eval, baseScore, session, jokers);
        }

        private static int GetHandMultiplierBonus(HandRank rank, GameSession session)
        {
            return session.HandMultiplierBonus.ContainsKey(rank)
                ? session.HandMultiplierBonus[rank]
                : 0;
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

            string chipsLine = hand.Select(t => $"({t.GetChipValue()}🔵)").Aggregate("  ", (current, chipVal) => current + $"{chipVal.PadLeft(6),-9} ");
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