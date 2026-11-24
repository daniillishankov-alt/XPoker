namespace XPoker
{
    
    /// Game constants for easy configuration
    
    internal static class GameConstants
    {
        public const int HAND_SIZE = 5;
        public const int DISCARDS_PER_HAND = 3;
        public const int HANDS_PER_ROUND = 5;
        public const int MAX_ROUNDS = 3;
        public const int ROUND_COMPLETION_MONEY = 50;
        public const int STARTING_MONEY = 100;
        public const int ROUND_BONUS = 25;
    }

    class Program
    {
		static void Main()
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			GameSession session = new();
			Shop shop = new();
			Deck deck = new();
			List<Joker> activeJokers = new()
			{
				new Joker { Description = "+2 multiplier for each combo" }
			};

			DrawHeader("🃏 XPoker: ROGUELIKE EDITION 🃏");
			Console.WriteLine("\n  Welcome to the most intense poker experience!");
			Console.WriteLine("  Defeat enemies by scoring points in poker hands.\n");
			Console.Write("  Press any key to start...");
			Console.ReadKey(true);

            // Main game loop - multiple rounds
            while (session.CurrentRound <= GameConstants.MAX_ROUNDS)
			{
				Console.Clear();
				DrawHeader($"⚔️  ROUND {session.CurrentRound} ⚔️");
				Console.WriteLine($"\n  Target Score: {session.GetTargetScore()} 🎯");
				Console.WriteLine($"  Current Money: {session.Money}💰");
				Console.WriteLine($"  Applied Enhancements: {(session.AppliedEnhancements.Count > 0 ? string.Join(", ", session.AppliedEnhancements) : "None")}");
				Console.Write("\n  Press Enter to start round...");
				Console.ReadLine();

				// Shop before round starts
				DisplayShop(shop, session);

				// Round gameplay - player has 5 hands to reach target score
				session.ResetForNewRound();
				bool roundWon = false;
				int handNumber = 1;

                while (session.HandsLeft > 0 && !roundWon)
                {
                    deck.Reset();
                    List<Card> hand = new();
                    for (int i = 0; i < GameConstants.HAND_SIZE; i++) hand.Add(deck.Draw());

                    int discardsLeft = GameConstants.DISCARDS_PER_HAND;
                    bool handPlayed = false;

					while (!handPlayed)
					{
						Console.Clear();
						DrawHeader($"🎰 Hand {handNumber} / 5");

						int scoreNeeded = session.GetTargetScore() - session.Score;
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine($"  🎯 Score Needed: {scoreNeeded}\n");
						Console.WriteLine($"  💰 Round Score: {session.Score} / {session.GetTargetScore()}");
						Console.ResetColor();
						Console.WriteLine($"  Hands left: {session.HandsLeft} | Discards: {discardsLeft} | Money: {session.Money}💰\n");

						DisplayCardsFancy(hand);

						var eval = HandEvaluator.Evaluate(hand);
						var baseScore = eval.GetBaseScore();
						int predictedScore = CalculatePredictedScore(hand, eval, baseScore, session, activeJokers);

						Console.WriteLine("  ┌────────────────────────────────────────────────────────┐");
						Console.WriteLine($"  │ Combo: {eval,-42}");
						Console.WriteLine($"  │ Base: {baseScore.chips} chips x {baseScore.mult} mult");
						Console.WriteLine($"  │ Predicted Score: {predictedScore}🎯{"",-27}");
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
                                    // Replace cards in-place, keeping positions unchanged
                                    foreach (int idx in indices)
                                    {
                                        hand[idx] = deck.Draw();
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

								// Display detailed score breakdown
								int cardChips = 0;
								foreach (int cardIndex in eval.CardsInCombo)
								{
									cardChips += hand[cardIndex].GetChipValue();
								}

								int totalChips = baseScore.chips + cardChips;
								int totalMult = baseScore.mult + GetHandMultiplierBonus(eval.Rank, session);

								int jokerBonus = 0;
								foreach (var joker in activeJokers)
									jokerBonus += 2; // Current joker bonus

								Console.WriteLine($"  Combo: {eval}");
								Console.WriteLine($"  Base Score: {baseScore.chips} chips × {baseScore.mult} mult");
								Console.WriteLine($"  Cards in Combo: {eval.CardsInCombo.Count} cards = {cardChips} chips");
								Console.WriteLine($"  Total Chips: {baseScore.chips} + {cardChips} = {totalChips}");
								Console.WriteLine($"  Multiplier: {baseScore.mult} (base) + {jokerBonus} (Jokers) = {totalMult}");
								Console.ForegroundColor = ConsoleColor.Cyan;
								Console.WriteLine($"  Calculation: {totalChips} × {totalMult} = {handScore}");
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

				// Round conclusion
				Console.Clear();
				if (session.Score >= session.GetTargetScore())
				{
                    DrawHeader("🏆 ROUND CLEARED 🏆");
                    Console.WriteLine($"\n  You defeated round {session.CurrentRound}!");
                    Console.WriteLine($"  Final Score: {session.Score}");
                    Console.WriteLine($"  Money Earned: {GameConstants.ROUND_COMPLETION_MONEY}💰");
                    session.Money += GameConstants.ROUND_COMPLETION_MONEY;
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

			// Game conclusion
			Console.Clear();
            DrawHeader("🎮 GAME OVER 🎮");
            if (session.CurrentRound > GameConstants.MAX_ROUNDS)
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

		/// Displays the in-game shop where player can buy enhancements
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

        /// Calculates the predicted score for the current hand
        /// Only counts cards that are part of the combo
        private static int CalculatePredictedScore(List<Card> hand, HandEvaluation eval,
            (int chips, int mult) baseScore, GameSession session, List<Joker> jokers)
        {
            // Calculate chips only from cards in the combo
            int cardChips = 0;

            if (eval.CardsInCombo == null || eval.CardsInCombo.Count == 0)
            {
                eval.CardsInCombo = new List<int>();
            }

            foreach (int cardIndex in eval.CardsInCombo)
            {
                if (cardIndex >= 0 && cardIndex < hand.Count)
                {
                    cardChips += hand[cardIndex].GetChipValue();
                }
            }

            int totalChips = baseScore.chips + cardChips;
            int totalMult = baseScore.mult + GetHandMultiplierBonus(eval.Rank, session);

            // Apply joker multipliers
            foreach (var joker in jokers)
                totalMult = joker.ApplyMultBonus(totalMult);

            return totalChips * totalMult;
        }

        /// Calculates detailed score breakdown for display
        private static UI.ScoreBreakdown CalculateScoreBreakdown(List<Card> hand, HandEvaluation eval,
            (int chips, int mult) baseScore, GameSession session, List<Joker> jokers)
        {
            int cardChips = eval.CardsInCombo
                .Where(idx => idx >= 0 && idx < hand.Count)
                .Sum(idx => hand[idx].GetChipValue());

            int totalChips = baseScore.chips + cardChips;
            int jokerBonus = jokers.Count * 2; // Each joker adds +2
            int totalMult = baseScore.mult + GetHandMultiplierBonus(eval.Rank, session) + jokerBonus;

            return new UI.ScoreBreakdown
            {
                BaseChips = baseScore.chips,
                CardChipsInCombo = cardChips,
                TotalChips = totalChips,
                BaseMultiplier = baseScore.mult,
                JokerBonus = jokerBonus,
                TotalMultiplier = totalMult,
                CardsInComboCount = eval.CardsInCombo.Count,
                FinalScore = totalChips * totalMult
            };
        }

        /// Wrapper for calculating final hand score
        private static int CalculateHandScore(List<Card> hand, HandEvaluation eval,
            (int chips, int mult) baseScore, GameSession session, List<Joker> jokers)
        {
            return CalculatePredictedScore(hand, eval, baseScore, session, jokers);
        }

        /// Gets any hand rank multiplier bonuses from purchased enhancements
		private static int GetHandMultiplierBonus(HandRank rank, GameSession session)
		{
			return session.HandMultiplierBonus.ContainsKey(rank)
				? session.HandMultiplierBonus[rank]
				: 0;
		}

		/// Displays a stylized header with the given title
		private static void DrawHeader(string title)
		{
			Console.ForegroundColor = ConsoleColor.DarkMagenta;
			Console.WriteLine("==========================================================");
			Console.WriteLine($"  {title}");
			Console.WriteLine("==========================================================");
			Console.ResetColor();
		}

		/// Displays cards in a fancy visual format with ASCII art
		private static void DisplayCardsFancy(List<Card> hand)
		{
			string line1 = "  ";
			string line2 = "  ";
			string line3 = "  ";
			string line4 = "  ";
			string line5 = "  ";

			foreach (var card in hand)
			{
				string rank = card.GetRankStr();
				char suit = card.GetSuitChar();
				string padding = rank.Length == 1 ? " " : "";

				line1 += "┌───────┐ ";
				line2 += $"│ {rank}{padding}    │ ";
				line3 += $"│   {suit}   │ ";
				line4 += $"│    {padding}{rank} │ ";
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

		/// Parses card indices from user input (e.g., "1 3 5" -> [0, 2, 4])
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