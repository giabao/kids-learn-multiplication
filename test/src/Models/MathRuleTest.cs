namespace Kids.Models;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class MathRuleTest(Node testScene) : TestClass(testScene) {
    [Test]
    public void RuleIndex() {
        var i = 3;
        for (var l = 2; l <= 9; l++) {
            for (var r = l; r <= 9; r++) {
                var rule = (MultiplyRule.Rules[i] as SimpleRule)!.ToEquation;
                (l == rule.Left && r == rule.Right).ShouldBeTrue();
                MultiplyRule.RuleIndex(l, r).ShouldBe(MultiplyRule.RuleIndex(r, l));
                MultiplyRule.RuleIndex(l, r).ShouldBe(i);
                // GD.Print($"{i,2}: {l} {r} / {MultiplyRule.RuleIndex(l, r)}");
                i++;
            }
        }
    }

}
