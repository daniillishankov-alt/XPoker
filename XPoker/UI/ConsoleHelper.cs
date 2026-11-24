namespace XPoker.UI
{
    /// Helper class for console UI operations
    public static class ConsoleHelper
    {
        private const int BOX_WIDTH = 60;
        private const char TOP_LEFT = '┌';
        private const char TOP_RIGHT = '┐';
        private const char BOTTOM_LEFT = '└';
        private const char BOTTOM_RIGHT = '┘';
        private const char HORIZONTAL = '─';
        private const char VERTICAL = '│';

        /// Draws a styled box with a single line of text
        
        public static void DrawBox(string line1, string? line2 = null, string? line3 = null)
        {
            Console.WriteLine($"  {TOP_LEFT}{new string(HORIZONTAL, BOX_WIDTH - 4)}{TOP_RIGHT}");
            Console.WriteLine($"  {VERTICAL} {line1.PadRight(BOX_WIDTH - 4)} {VERTICAL}");
            
            if (line2 != null)
                Console.WriteLine($"  {VERTICAL} {line2.PadRight(BOX_WIDTH - 4)} {VERTICAL}");
            
            if (line3 != null)
                Console.WriteLine($"  {VERTICAL} {line3.PadRight(BOX_WIDTH - 4)} {VERTICAL}");
            
            Console.WriteLine($"  {BOTTOM_LEFT}{new string(HORIZONTAL, BOX_WIDTH - 4)}{BOTTOM_RIGHT}");
        }

        /// Draws score information with proper formatting
        public static void DisplayScoreInfo(string combo, int baseChips, int baseMult, int predictedScore)
        {
            Console.WriteLine("  ┌────────────────────────────────────────────────────────┐");
            Console.WriteLine($"  │ Combo: {combo,-42}");
            Console.WriteLine($"  │ Base: {baseChips} chips x {baseMult} mult");
            Console.WriteLine($"  │ Predicted Score: {predictedScore}🎯{"",-27}");
            Console.WriteLine("  └────────────────────────────────────────────────────────┘\n");
        }

        /// Displays detailed score breakdown
        public static void DisplayScoreBreakdown(ScoreBreakdown breakdown)
        {
            Console.WriteLine($"  Base Score: {breakdown.BaseChips} chips × {breakdown.BaseMultiplier} mult");
            Console.WriteLine($"  Cards in Combo: {breakdown.CardsInComboCount} cards = {breakdown.CardChipsInCombo} chips");
            Console.WriteLine($"  Total Chips: {breakdown.BaseChips} + {breakdown.CardChipsInCombo} = {breakdown.TotalChips}");
            Console.WriteLine($"  Multiplier: {breakdown.BaseMultiplier} (base) + {breakdown.JokerBonus} (Jokers) = {breakdown.TotalMultiplier}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  Calculation: {breakdown.TotalChips} × {breakdown.TotalMultiplier} = {breakdown.FinalScore}");
            Console.ResetColor();
        }
    }
}
