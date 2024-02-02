using Godot;
using System;
using System.Linq;
using Kids.Models;

namespace Kids.Levels;

public partial class RuleReminder : TextureRect {
    [GetNode("%")] private Control _examples = null!;
    [GetNode("%")] private Label _desc = null!;
    private MultiplyRule _rule = MultiplyRule.Rules[0];

    public override void _Ready() {
        GetNode<TextureButton>("%BackBtn").WithSound().Pressed += Main.Back;
        _desc.Text = _rule.Desc;
        var textWidth = _desc.TextSize().X;
        if (_desc.Size.X > textWidth) // position desc at center
            _desc.Position = new Vector2((_desc.Size.X - textWidth) / 2, 0);

        var tween = CreateTween();
        tween.TweenMethod(Callable.From((int len) => {
            if (_desc.Text.Length == len) return;
            _desc.Text = _rule.Desc[..len];
        }), 0, _desc.Text.Length, 1);

        switch (_rule) {
            case CompoundRule { Left: 0 } r:
                int[] rights = [0, 1, 5, 123];
                Vector2[] poses = [
                    new(150, 100), new(550, 100),
                    new(150, 300), new(550, 300)
                ];
                var boxes = new EquationBox[rights.Length];
                foreach (var (right, i) in rights.WithIndex()) {
                    var box = EquationBox.Load();
                    boxes[i] = box;
                    var ex = new MulEquation(r.Left, right);
                    box.Text = ex.Text;
                    box.ReLayout();
                    box.PosCenter(_examples);
                    var resultPos = box.Result.Position;
                    tween.TweenCallback(Callable.From(() => {
                        box.Text = "";
                        box.Left.FontColor(Colors.DarkRed);
                        _examples.AddChild(box);
                    }));
                    var text = ex.BaseText;
                    tween.TweenMethod(Callable.From((int len) => box.Text = text[..len]),
                        0, text.Length, 1);
                    tween.TweenCallback(Callable.From(() => {
                        box.Result.FontColor(Colors.DarkRed);
                        box.Result.Position = box.Left.Position;
                        box.Result.Text = box.Left.Text;
                    }));
                    tween.TweenProperty(box.Result, "position", resultPos, 0.6).SetDelay(0.5);
                    box.RotatingEffect(tween).SetDelay(0.5);
                    tween.TweenProperty(box, "modulate:a", 0f, 0.5);
                }

                tween.TweenCallback(Callable.From(() => { }));
                foreach (var (box, pos) in boxes.Zip(poses)) {
                    tween.Parallel().TweenProperty(box, "modulate:a", 1f, 0.3);
                    tween.Parallel().TweenProperty(box, "scale", new Vector2(0.6f, 0.6f), 0.5);
                    // rotate random angle from -0.5 to 0.5 radian == ~-28 to 28 degree
                    tween.Parallel().TweenProperty(box, "rotation", GD.Randf() - 0.5f, 0.5);
                    tween.Parallel().TweenProperty(box, "position", pos, 0.5);
                }

                break;
            default: // TODO impl
                break;
        }
    }

    public static RuleReminder Load(int level) {
        var ret = (RuleReminder)ResourceLoader.Load<PackedScene>("res://Levels/RuleReminder.tscn").Instantiate();
        ret._rule = MultiplyRule.Rules[level];
        return ret;
    }
}