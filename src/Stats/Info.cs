using Godot;
using Kids.Levels;
using Kids.Models;

namespace Kids.Stats;

public partial class Info : VBoxContainer {
    [GetNode("%")] private Label _title = null!;
    [GetNode("%")] private Label _desc = null!;
    [GetNode("%")] private Label _done = null!;
    [GetNode("%")] private Label _win = null!;
    [GetNode("%")] private Label _lose = null!;
    private int _level;

    public override void _Ready() {
        GetNode<TextureButton>("%Close").WithSound().Pressed += Hide;
        GetNode<Button>("%LearnBtn").WithSound().Pressed +=
            () => Main.SceneTo(GameLevel.Load(_level), replace: true);
    }

    public void Show(int level, RuleStat? stat, Vector2 pos) {
        _level = level;
        Visible = true;
        Position = pos;
        var rule = MultiplyRule.Rules[level];
        _title.Text = $"Rule: {rule.Name}";
        _desc.Text = rule.Desc;
        stat ??= new RuleStat();
        _done.Text = stat.Done.ToString();
        _win.Text = stat.Win.ToString();
        _lose.Text = stat.Lose.ToString();
    }
}