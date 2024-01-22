using Godot;
using Kids.Levels;
using Kids.Models;

namespace Kids.Stats;

public partial class Info : VBoxContainer {
    private int _level;

    public override void _Ready() {
        GetNode<TextureButton>("%Close").WithSound().Pressed += Hide;
        GetNode<Button>("%LearnBtn").WithSound().Pressed += () => {
            Main.Back();
            Main.SceneTo(GameLevel.Load(_level));
        };
    }

    public void Show(int level, RuleStat? stat, Vector2 pos) {
        _level = level;
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