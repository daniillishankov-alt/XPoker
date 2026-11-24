# XPoker - Code Review & Improvements

## 🔴 Critical Issues

### 1. **Missing namespace in Program.cs**
- **File**: `XPoker\Program.cs` (Line 1)
- **Issue**: Missing `namespace XPoker {` declaration
- **Impact**: Code won't compile
- **Fix**: Add proper namespace wrapper

```csharp
namespace XPoker
{
    class Program
    {
        // ...
    }
}
```

---

## 🟠 High Priority Issues

### 2. **Hardcoded magic numbers in Program.cs**
- **File**: `XPoker\Program.cs`
- **Issue**: Magic numbers scattered throughout code (5 cards, 3 discards, 50 money, etc.)
- **Example**: 
  - Line 45: `for (int i = 0; i < 5; i++)` - hardcoded hand size
  - Line 47: `int discardsLeft = 3;` - hardcoded discard count
  - Line 23: `while (session.CurrentRound <= 3)` - hardcoded max rounds
  - Line 175: `session.Money += 50;` - hardcoded money reward

**Recommendation**: Extract to constants
```csharp
private const int HAND_SIZE = 5;
private const int DISCARDS_PER_HAND = 3;
private const int MAX_ROUNDS = 3;
private const int ROUND_COMPLETION_MONEY = 50;
```

---

### 3. **Duplicate score calculation logic in Program.cs**
- **File**: `XPoker\Program.cs` (Lines 128-139 & 264-291)
- **Issue**: Score calculation is done twice - once in result display, once in `CalculatePredictedScore()`
- **Impact**: Code duplication, maintenance nightmare
- **Fix**: Create a helper method to calculate score breakdown

```csharp
private static ScoreBreakdown CalculateScoreBreakdown(List<Card> hand, HandEvaluation eval, 
    (int chips, int mult) baseScore, GameSession session, List<Joker> jokers)
{
    int cardChips = eval.CardsInCombo.Sum(idx => hand[idx].GetChipValue());
    int totalChips = baseScore.chips + cardChips;
    int totalMult = baseScore.mult + GetHandMultiplierBonus(eval.Rank, session);
    int jokerBonus = jokers.Sum(j => 2); // Current joker value
    totalMult += jokerBonus;
    
    return new ScoreBreakdown
    {
        CardChips = cardChips,
        TotalChips = totalChips,
        TotalMult = totalMult,
        JokerBonus = jokerBonus,
        FinalScore = totalChips * totalMult
    };
}

private class ScoreBreakdown
{
    public int CardChips { get; set; }
    public int TotalChips { get; set; }
    public int TotalMult { get; set; }
    public int JokerBonus { get; set; }
    public int FinalScore { get; set; }
}
```

---

### 4. **GameSession.GetTargetScore() formula issue**
- **File**: `XPoker\GameSession.cs` (Line 17)
- **Current**: `return 300 * CurrentRound;` → Round 1: 300, Round 2: 600, Round 3: 900
- **Issue**: This was changed from `100 * CurrentRound`, creating imbalance
- **Impact**: Game is now too hard
- **Recommendation**: Use balanced scaling
```csharp
return 100 * CurrentRound; // Round 1: 100, Round 2: 200, Round 3: 300
```

---

### 5. **Shop enhancement purchase doesn't update GameSession state**
- **File**: `XPoker\Shop.cs` & `XPoker\GameSession.cs`
- **Issue**: Only `CardChipsBonus` and enhancement name are applied, but `HandChipsBonus` and `HandMultiplierBonus` are never used
- **Impact**: Chip/Multiplier bonuses for specific hands don't work
- **Fix**: Implement the bonus application in `BuyEnhancement()`

```csharp
public void BuyEnhancement(Enhancement enhancement)
{
    if (Money >= enhancement.Cost)
    {
        Money -= enhancement.Cost;
        AppliedEnhancements.Add(enhancement.Name);

        switch (enhancement.Type)
        {
            case EnhancementType.CardChips:
                CardChipsBonus += enhancement.Value;
                break;
            case EnhancementType.ChipsBonus:
                // Need to know which hand this applies to
                // This is a design issue
                break;
            case EnhancementType.MultiplierBonus:
                // Same issue
                break;
        }
    }
}
```

---

## 🟡 Medium Priority Issues

### 6. **Deck performance issue**
- **File**: `XPoker\Deck.cs` (Line 38)
- **Issue**: `_cards.RemoveAt(0)` is O(n) operation - rebuilds entire list each time
- **Impact**: Removes 52 cards, each removal shifts all remaining items
- **Fix**: Use a queue-based approach
```csharp
private Queue<Card> _cards = new();

public Card Draw()
{
    if (_cards.Count == 0) Reset();
    return _cards.Dequeue();
}
```

---

### 7. **Program.Main() should not be async-void pattern but needs better error handling**
- **File**: `XPoker\Program.cs`
- **Issue**: No try-catch blocks, game crashes on invalid input
- **Example**: Invalid card index parsing doesn't validate properly
- **Fix**: Add error handling throughout

---

### 8. **Inconsistent use of static vs instance for Joker bonus**
- **File**: `XPoker\Program.cs` (Line 139)
- **Issue**: Hardcoded `jokerBonus += 2;` instead of calling `joker.GetBonus()` or similar
- **Impact**: If joker value changes, need to update multiple places
- **Fix**: Create proper joker API
```csharp
int jokerBonus = activeJokers.Sum(j => j.GetBonus());
```

---

### 9. **Console UI rendering is fragile**
- **File**: `XPoker\Program.cs` (Lines 68-72)
- **Issue**: Hardcoded box drawing with limited spacing
- **Example**: Formatting breaks with longer combo names
- **Fix**: Create a proper UI helper class
```csharp
public class ConsoleBox
{
    public static void DrawBox(string content, int width = 60)
    {
        Console.WriteLine("┌" + new string('─', width - 2) + "┐");
        Console.WriteLine($"│ {content.PadRight(width - 3)} │");
        Console.WriteLine("└" + new string('─', width - 2) + "┘");
    }
}
```

---

### 10. **No logging/debugging support**
- **Issue**: Hard to debug game state during play
- **Recommendation**: Add a simple logger
```csharp
public static class GameLogger
{
    private static bool _debugMode = false;
    
    public static void Debug(string message)
    {
        if (_debugMode)
            Console.WriteLine($"[DEBUG] {message}");
    }
}
```

---

## 🟢 Low Priority Issues / Code Style

### 11. **Shop method can be simplified**
- **File**: `XPoker\Shop.cs` (Lines 29-37)
- **Current**: Uses `Random.Next()` for shuffling, which is less reliable
- **Better**: Could use `Random.Shuffle()` from .NET 8
```csharp
public List<Enhancement> GetRandomShop(int count = 3)
{
    var items = _availableEnhancements.ToList();
    Random.Shared.Shuffle(items);
    return items.Take(count).ToList();
}
```

---

### 12. **Unused properties in GameSession**
- **File**: `XPoker\GameSession.cs`
- **Issue**: `JokerSlots` is never used
- **Fix**: Either implement or remove

---

### 13. **Missing null safety in Card comparisons**
- **File**: `XPoker\Card.cs`
- **Issue**: No `Equals()` override for card comparison
- **Impact**: Tests using `Assert.Equal()` on cards might fail
- **Fix**: 
```csharp
public override bool Equals(object? obj)
{
    if (obj is not Card other) return false;
    return Suit == other.Suit && Rank == other.Rank;
}

public override int GetHashCode() => HashCode.Combine(Suit, Rank);
```

---

### 14. **Program should support command-line arguments**
- **File**: `XPoker\Program.cs` (Line 4)
- **Current**: `static void Main()`
- **Better**: `static void Main(string[] args)` for future extensibility

---

## 📋 Summary of Fixes by Priority

| Priority | Issue | File | Effort |
|----------|-------|------|--------|
| 🔴 | Missing namespace | Program.cs | 2 min |
| 🔴 | Incorrect target score formula | GameSession.cs | 1 min |
| 🟠 | Hardcoded magic numbers | Program.cs | 15 min |
| 🟠 | Duplicate score calculation | Program.cs | 20 min |
| 🟠 | Bonus enhancements not applied | GameSession.cs | 15 min |
| 🟡 | Deck O(n) performance issue | Deck.cs | 10 min |
| 🟡 | Missing error handling | Program.cs | 20 min |
| 🟢 | Shop shuffling method | Shop.cs | 5 min |
| 🟢 | Card.Equals() override | Card.cs | 10 min |

**Total estimated time to fix all issues: ~100 minutes**

---

## ✅ What's Good

✓ Excellent test coverage
✓ Proper XML documentation
✓ Good separation of concerns
✓ Proper use of enums
✓ Consistent naming conventions
✓ Modern C# features (records, switch expressions, pattern matching)
✓ Proper async handling potential
