namespace XPoker
{
    public class GameSession
    {
        public int CurrentRound { get; set; } = 1;
        public int Score { get; set; } = 0;
        public int HandsLeft { get; set; } = 5;
        public int Money { get; set; } = 100;
        public List<string> AppliedEnhancements { get; set; } = new();
        public Dictionary<HandRank, int> HandChipsBonus { get; set; } = new();
        public Dictionary<HandRank, int> HandMultiplierBonus { get; set; } = new();
        public int CardChipsBonus { get; set; } = 0;
        public int JokerSlots { get; set; } = 1;

        public int GetTargetScore()
        {
            // Each round requires more chips
            return 300 * CurrentRound;
        }

        public void ResetForNewRound()
        {
            HandsLeft = 5;
            Score = 0;
            Money += 25; // Round completion bonus
        }

        public void BuyEnhancement(Enhancement enhancement)
        {
            if (Money >= enhancement.Cost)
            {
                Money -= enhancement.Cost;
                AppliedEnhancements.Add(enhancement.Name);

                if (enhancement.Type == EnhancementType.CardChips)
                    CardChipsBonus += enhancement.Value;
            }
        }
    }
}