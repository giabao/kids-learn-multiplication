using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Kids;

struct RuleStat(int win, int lose) {
    public int Win = win;
    public int Lose = lose;
    public int Done => Win + Lose;
    public int LosePercent => Done switch {
        0 => 80,
        1 => Lose == 1 ? 90 : 40,
        2 => Lose switch { 2 => 100, 1 => 60, _ => 30 },
        3 => Lose switch { 3 => 100, 2 => 80, 1 => 50, _ => 20 },
        _ => Math.Min(Lose * 100 / Done + 10, 100)
    };
}

