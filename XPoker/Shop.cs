namespace XPoker
{
    public class Shop
    {
        private readonly List<Enhancement> _availableEnhancements = new();

        public Shop()
        {
            InitializeEnhancements();
        }

        private void InitializeEnhancements()
        {
            var allEnhancements = new List<Enhancement>
            {
                new() { Name = "Chip Booster", Description = "+20 chips to High Card", Cost = 50, Type = EnhancementType.ChipsBonus, Value = 20 },
                new() { Name = "Pair Power", Description = "+15 chips to One Pair", Cost = 60, Type = EnhancementType.ChipsBonus, Value = 15 },
                new() { Name = "Straight Shot", Description = "+25 chips to Straight", Cost = 80, Type = EnhancementType.ChipsBonus, Value = 25 },
                new() { Name = "Flush Force", Description = "+30 chips to Flush", Cost = 90, Type = EnhancementType.ChipsBonus, Value = 30 },
                new() { Name = "Multiplier X2", Description = "+1 multiplier to One Pair", Cost = 100, Type = EnhancementType.MultiplierBonus, Value = 1 },
                new() { Name = "Triple Threat", Description = "+2 multiplier to Three of a Kind", Cost = 120, Type = EnhancementType.MultiplierBonus, Value = 2 },
                new() { Name = "Card Value +1", Description = "All cards worth +1 chip", Cost = 70, Type = EnhancementType.CardChips, Value = 1 },
                new() { Name = "Card Value +2", Description = "All cards worth +2 chips", Cost = 150, Type = EnhancementType.CardChips, Value = 2 },
            };

            _availableEnhancements.AddRange(allEnhancements);
        }

        public List<Enhancement> GetRandomShop(int count = 3)
        {
            if (_availableEnhancements.Count == 0)
                return new List<Enhancement>();

            var items = _availableEnhancements.ToList();
            // Shuffle using simple Random approach (compatible with .NET 8)
            for (int i = items.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Shared.Next(i + 1);
                (items[i], items[randomIndex]) = (items[randomIndex], items[i]);
            }
            
            return items.Take(Math.Min(count, items.Count)).ToList();
        }

        public Enhancement GetEnhancementByIndex(int index, List<Enhancement> shop)
        {
            if (index >= 0 && index < shop.Count)
                return shop[index];
            return null;
        }
    }
}