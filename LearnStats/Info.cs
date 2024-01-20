using Godot;

namespace Kids.LearnStats;

public partial class Info : VBoxContainer {
    public override void _Ready() {
        GetNode<TextureButton>("%Close").Pressed += () => Visible = false;
    }

    public void Show(int level, RuleStat? stat, Vector2 pos) {
        Visible = true;
        Position = pos;
        var rule = MultiplyRule.Rules[level];
        GetNode<Label>("%Title").Text = $"Rule: {rule.Name}";
        GetNode<Label>("%Desc").Text = rule.Desc;
        stat = stat ?? new RuleStat();
        GetNode<Label>("%Done").Text = stat.Done.ToString();
        GetNode<Label>("%Win").Text = stat.Win.ToString();
        GetNode<Label>("%Lose").Text = stat.Lose.ToString();
    }
}