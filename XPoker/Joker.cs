namespace XPoker
{
    public class Joker
    {
        public string Description { get; set; } = "+4 to multiplier";
        public int ApplyMultBonus(int currentMult) => currentMult + 4;
    }
}