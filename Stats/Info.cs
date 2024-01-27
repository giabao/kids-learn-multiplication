using Godot;
using Kids.Levels;
using Kids.Models;

namespace Kids.Stats;

public partial class Info : VBoxContainer {
    [OnReady("%")] private Label _title;
    [OnReady("%")] private Label _desc;
    [OnReady("%")] private Label _done;
    [OnReady("%")] private Label _win;
    [OnReady("%")] private Label _lose;
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
        _title.Text = $"Rule: {rule.Name}";
        _desc.Text = rule.Desc;
        stat ??= new RuleStat();
        _done.Text = stat.Done.ToString();
        _win.Text = stat.Win.ToString();
        _lose.Text = stat.Lose.ToString();
    }
}