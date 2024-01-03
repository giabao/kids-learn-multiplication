using System;
using System.Collections.Generic;
using System.Linq;

namespace Kids;

class Operands(int left, int right) {
    public readonly int Left = left;
    public readonly int Right = right;
}

class Equation(int left, int right, string op, int result) : Operands(left, right) {
    public readonly string Op = op;
    public readonly int Result = result;
    public string Question => $"{Left} {Op} {Right} = ?";
    public string Answer => $"{Left} {Op} {Right} = {Result}";
}
class MulEquation(int left, int right) : Equation(left, right, "x", left * right) {
    public MulEquation Swap => new(Right, Left);
}

abstract class MathRule<T>(string desc, string audio) where T : Equation {
    public readonly string Desc = desc;
    public readonly string Audio = audio;
    public abstract List<T> Examples();
}
abstract class MultiplyRule(int left, string desc, string audio) : MathRule<MulEquation>(desc, audio) {
    readonly public int Left = left;

    const int OperandMax = 9;
    static readonly public MultiplyRule[] Rules = new MultiplyRule[3 + (OperandMax - 1) * OperandMax / 2];
    static MultiplyRule() {
        Rules[0] = new CompoundRule(0, "Bất kỳ số nào nhân với 0 cũng bằng 0", [0, 6, 99]);
        Rules[1] = new CompoundRule(1, "Bất kỳ số nào nhân với 1 cũng bằng chính số ấy", [3, 15]);
        Rules[2] = new CompoundRule(10, "Muốn nhân một số với 10: Chỉ việc thêm 0 vào sau là được", [1, 10, 12]);
        int i = 3;
        for (int x = 2; x <= OperandMax; x++)
            for (int y = x; y <= OperandMax; y++)
                Rules[i++] = new SimpleRule(x, y);
    }
}
sealed class CompoundRule(int left, string desc, int[] rightExamples) : MultiplyRule(left, desc, $"rule/x{left}") {
    public override List<MulEquation> Examples() => rightExamples.Select(right => new MulEquation(Left, right)).ToList();
    public MulEquation RandomEquation(Random rnd, bool randomSwap = false) {
        var right = Left switch {
            0 or 1 => rnd.Next(121),
            _ => rnd.Next(15)
        };
        MulEquation e = new(Left, right);
        return randomSwap && rnd.NextBool() ? e.Swap : e;
    }
}

sealed class SimpleRule(int left, int right) : MultiplyRule(left, $"{left} x {right} = {left * right}", $"{left}x{right}") {
    public override List<MulEquation> Examples() => [new(Left, right)];
    public MulEquation ToEquation => new(Left, right);
}
