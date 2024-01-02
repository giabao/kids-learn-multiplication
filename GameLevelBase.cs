using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Kids;

// Map level number to RuleStat
using RuleStats = Dictionary<int, RuleStat>;

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
// public struct Player() {
//     Dictionary<int, RuleStat> Stats = [];
// }

static class Extension {
    public static bool NextBool(this Random r) => r.NextDouble() >= 0.5;
}

class GameLevelBase(int level) {
    public readonly int Level = level; // [0, MultiplyRule.Rules.length)
    public List<MulEquation> Examples(RuleStats stats) {
        if (Level == 0) {
            var r0 = (CompoundRule)MultiplyRule.Rules[0];
            TakeDistinces(() => r0.RandomEquation(rnd, true));
        }
        var (totalLosePercent, level2Percents) = stats
            .Where((p) => p.Key < Level)
            .Aggregate((0, (List<(int, int)>)[]), (acc, p) => {
                var (level, stat) = p;
                var (sum, list) = acc;
                sum += stat.LosePercent;
                list.Add((level, sum));
                return (sum, list);
            });
        var ret = TakeDistinces(() => {
            var percent = rnd.Next(totalLosePercent);
            var level = level2Percents.FirstOrDefault(p => p.Item2 > percent).Item1;
            MultiplyRule r = MultiplyRule.Rules[level];
            var e = GetEquation(r);
            return rnd.NextBool() ? e : e.Swap;
        }, count: 3);

        var e = GetEquation(MultiplyRule.Rules[Level]);
        ret.Insert(Random.Shared.Next(ret.Count), e.Swap);
        ret.Insert(0, e);
        return ret;
    }

    private static readonly Random rnd = Random.Shared;
    private static MulEquation GetEquation(MultiplyRule r) => r switch {
        CompoundRule r1 => r1.RandomEquation(rnd),
        _ => (r as SimpleRule)!.ToEquation
    };

    private static List<T> TakeDistinces<T>(Func<T> rndGen, int count = 5) {
        List<T> ret = [];
        for (var i = 0; i < count; i++) {
            while (true) {
                var t = rndGen();
                if (!ret.Contains(t)) {
                    ret.Add(t);
                    break;
                }
            }
        }
        return ret;
    }

}