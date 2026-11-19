namespace XPoker
{
    public class Enhancement
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public EnhancementType Type { get; set; }
        public int Value { get; set; }

        public override string ToString() => $"{Name} ({Cost}💰) - {Description}";
    }

    public enum EnhancementType
    {
        ChipsBonus,      // +X chips to specific hand
        MultiplierBonus, // +X multiplier to specific hand
        CardChips,       // +X chips to cards
        JokerSlot        // Add extra joker slot
    }
}