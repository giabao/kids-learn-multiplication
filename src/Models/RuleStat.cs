namespace Kids.Models;
using System;
using Godot;

public partial class RuleStat : Resource {
    [Export] public int Win;

    [Export] public int Lose;

    // Without a parameterless constructor, Godot will have problems
    // creating and editing your resource via the inspector.
    public RuleStat() : this(0, 0) {
    }

    // disable IDE0290 to fix CS9105: Cannot use primary constructor parameter 'int win' in this context.
    // at Godot.SourceGenerators/Godot.SourceGenerators.ScriptPropertyDefValGenerator/Kids.RuleStat_ScriptPropertyDefVal.generated.cs(17,35)
#pragma warning disable IDE0290
    public RuleStat(int win, int lose) {
#pragma warning restore IDE0290
        Win = win;
        Lose = lose;
    }

    public int Done => Win + Lose;

    public int LosePercent => Done switch {
        0 => 80,
        1 => Lose == 1 ? 90 : 40,
        2 => Lose switch { 2 => 100, 1 => 60, _ => 30 },
        3 => Lose switch { 3 => 100, 2 => 80, 1 => 50, _ => 20 },
        _ => Math.Min(Lose * 100 / Done + 10, 100)
    };
}
