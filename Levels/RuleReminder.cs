using Godot;
using System;
using System.Linq;
using Kids.Models;

namespace Kids.Levels;

public partial class RuleReminder : TextureRect {
    [GetNode("%")] private Control _examples = null!;
    private MultiplyRule _rule = MultiplyRule.Rules[0];

    public override async void _Ready() {
        GetNode<TextureButton>("%BackBtn").WithSound().Pressed += Main.Back;
        var desc = GetNode<Label>("%Desc");
        desc.Text = _rule.Desc;
        // position desc at center
        var textWidth = desc.GetStringSize().X;
        if (desc.Size.X > textWidth)
            desc.Position = new Vector2((desc.Size.X - textWidth) / 2, 0);
        desc.TypingEffect(time: 1);
        await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);

        var ex = _rule.Examples()[2]; // TODO
        var box = EquationBox.Load(ex.Text);
        box.PosCenter(_examples);
        _examples.AddChild(box);
        box.RotatingEffect();
        // foreach (var ex in _rule.Examples()) {
        //     var box = EquationBox.Load();
        //     _examples.AddChild(box);
        // }
        // var examples = _rule.Examples();
        //
        // _equationBox.TypingEffect(examples[0].Text);
        // var boxScene = ResourceLoader.Load<PackedScene>("res://Levels/EquationBox.tscn");
        // foreach (var e in rule.Examples()) {
        //     var box = boxScene.Instantiate<EquationBox>();
        //     // box.Left
        // }
    }

    public static RuleReminder Load(int level) {
        var ret = (RuleReminder)ResourceLoader.Load<PackedScene>("res://Levels/RuleReminder.tscn").Instantiate();
        ret._rule = MultiplyRule.Rules[level];
        return ret;
    }
}