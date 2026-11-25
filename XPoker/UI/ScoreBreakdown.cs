namespace XPoker.UI
{
    /// Represents a detailed breakdown of score calculation for display purposes
    public class ScoreBreakdown
    {
        public int BaseChips { get; set; }
        public int CardChipsInCombo { get; set; }
        public int TotalChips { get; set; }
        public int BaseMultiplier { get; set; }
        public int JokerBonus { get; set; }
        public int TotalMultiplier { get; set; }
        public int FinalScore { get; set; }
        public int CardsInComboCount { get; set; }
    }
}
