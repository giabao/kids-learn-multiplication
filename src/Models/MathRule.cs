namespace Kids.Models;
using System;
using System.Collections.Generic;
using System.Linq;

internal record Operands(int Left, int Right) { }

internal record Equation(int Left, int Right, string Op, int Result) : Operands(Left, Right) {
    public string BaseText => $"{Left}{Op}{Right}=";
    public string Question => $"{BaseText}?";
    public string Text => $"{BaseText}{Result}";
}

internal sealed record MulEquation(int Left, int Right) : Equation(Left, Right, "x", Left * Right) {
    public MulEquation Swap => new(Right, Left);
}

internal abstract class MathRule<T>(string desc) where T : Equation {
    public readonly string Desc = desc;
    public abstract string Audio { get; }
    public abstract List<T> Examples(); // TODO remove
}

internal abstract class MultiplyRule(int left, string desc) : MathRule<MulEquation>(desc) {
    public abstract string Name { get; }
    public override string Audio => $"rule/{Name}";
    public readonly int Left = left;

    private const int OperandMax = 9;
    public static readonly MultiplyRule[] Rules = new MultiplyRule[3 + (OperandMax - 1) * OperandMax / 2];

    static MultiplyRule() {
        Rules[0] = new CompoundRule(0, "Any number multiplied by 0 is equal to 0", [0, 6, 99]);
        Rules[1] = new CompoundRule(1, "Any number multiplied by 1 is the number itself", [3, 15]);
        Rules[2] = new CompoundRule(10, "Multiply any number by 10 = simply add a 0 at the end of the number", [1, 10, 12]);
        var i = 3;
        for (var left = 2; left <= OperandMax; left++) {
            for (var right = left; right <= OperandMax; right++) {
                Rules[i++] = new SimpleRule(left, right);
            }
        }
    }
    public static int RuleIndex(int left, int right) => left <= right ? RuleIndexImpl(left, right) : RuleIndexImpl(right, left);

    private static int RuleIndexImpl(int left, int right) => (left, right) switch {
        (0, _) => 0,
        (1, _) => 1,
        (_, 10) or (10, _) => 2,
        _ => (left - 2) * (OperandMax - 1) - (left - 2) * (left - 3) / 2 + right - left + 3
    };
}

internal sealed class CompoundRule(int left, string desc, int[] rightExamples) : MultiplyRule(left, desc) {
    public override string Name => $"x{Left}";

    public override List<MulEquation> Examples() =>
        rightExamples.Select(right => new MulEquation(Left, right)).ToList();

    public MulEquation RandomEquation(Random rnd, bool randomSwap = false) {
        var right = Left switch {
            0 or 1 => rnd.Next(121),
            _ => rnd.Next(15)
        };
        MulEquation e = new(Left, right);
        return randomSwap && rnd.NextBool() ? e.Swap : e;
    }
}

internal sealed class SimpleRule(int left, int right) : MultiplyRule(left, $"{left} x {right} = {left * right}") {
    public override string Name => $"{Left}x{right}";
    public override List<MulEquation> Examples() => [new(Left, right)];
    public MulEquation ToEquation => new(Left, right);
}
