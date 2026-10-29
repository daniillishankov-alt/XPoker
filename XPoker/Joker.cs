using System;
using System.Collections.Generic;
using System.Linq;

namespace XPoker
{
    public class Joker
    {
        public string Description { get; set; } = "+10 к множителю";
        public int ApplyMultBonus(int currentMult) => currentMult + 10;
    }
}