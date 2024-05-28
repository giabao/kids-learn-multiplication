using Godot;
using System;
using System.Linq;
using Kids.Models;

namespace Kids.Levels;

public partial class RuleReminder : TextureRect {
    [GetNode("%")] private Control _examples = null!;
    [GetNode("%")] private Label _desc = null!;
    private int _level;

    public override void _Ready() {
        GetNode<TextureButton>("%BackBtn").WithSound().Pressed += Main.Back;
        GetNode<TextureButton>("PlayBtn").WithSound().Pressed +=
            () => Main.SceneTo(GameLevel.Load(_level), replace: true);
        var rule = MultiplyRule.Rules[_level];
        switch (rule) {
            case CompoundRule r:
                _desc.Text = rule.Desc;
                var textWidth = _desc.TextSize().X;
                // position desc at center if single line
                var x = _desc.Size.X < textWidth ? 20 : (_desc.Size.X - textWidth) / 2;
                _desc.Position = new(x, 0);

                var tween = CreateTween();
                tween.TweenMethod(Callable.From((int len) => {
                    if (_desc.Text.Length == len) return;
                    _desc.Text = rule.Desc[..len];
                }), 0, _desc.Text.Length, 1);
                Animate(tween, r);
                break;
            case SimpleRule r:
                _desc.Text = "";
                Animate(r);
                break;
        }
    }

    private void Animate(SimpleRule rule) {
        var tween = CreateTween().SetParallel();
        var r = rule.ToEquation;
        int[] rights = [r.Right - 1, r.Right, r.Right + 1];
        foreach (var right in rights) {
            var box = EquationBox.Load();
            var ex = new MulEquation(r.Left, right);
            box.Text = ex.Text;
            box.ReLayout();
            var d = right - r.Right;
            var focus = d == 0;
            box.Position = (_examples.Size - box.Size) / 2 + d * 130 * Vector2.Down;
            if (focus) box.FontColor(Colors.DarkRed);
            _examples.AddChild(box);
            tween.TweenProperty(box, "modulate:a", focus ? 1f : 0.8f, 1).From(0f);
            TweenScaleAndCenterPos(tween, box, 0.1f, focus ? 1f : 0.7f, 1);

            if (ex.Left == ex.Right) continue;
            tween.TweenMethod(box.Rotate(), 0f, MathF.PI, 1f).SetDelay(1.5);
        }
    }

    private static void TweenScaleAndCenterPos(Tween tween, Control c,
        float scaleFrom, float scaleTo, double duration) {
        var posTo = c.Position + c.Size * (1 - scaleTo) / 2;
        c.Position += c.Size * (1 - scaleFrom) / 2;
        tween.TweenProperty(c, "scale", Vector2.One * scaleTo, duration)
            .From(Vector2.One * scaleFrom)
            .SetTrans(Tween.TransitionType.Elastic);
        tween.TweenProperty(c, "position", posTo, duration)
            .SetTrans(Tween.TransitionType.Elastic);
    }

    private void Animate(Tween tween, CompoundRule rule) {
        var right = rule.Left;
        Vector2[] poses = [
            new(120, 100), new(520, 100),
            new(120, 330), new(520, 330)
        ];
        int[] lefts = right switch {
            0 => [0, 1, 5, 123],
            1 => [3, 5, 15, 321],
            10 => [1, 2, 10, 99],
            _ => [] // never
        };
        var boxes = new EquationBox[lefts.Length];
        foreach (var (left, i) in lefts.WithIndex()) {
            var box = EquationBox.Load();
            boxes[i] = box;
            var ex = new MulEquation(left, right);
            box.Text = ex.Text;
            box.ReLayout();
            box.PosCenter(_examples);
            var resultPos = box.Result.Position;
            var resultBased = right == 0 ? box.Right : box.Left;
            tween.TweenCallback(Callable.From(() => {
                box.Text = "";
                resultBased.FontColor(Colors.DarkRed);
                _examples.AddChild(box);
            }));
            var text = ex.BaseText;
            tween.TweenMethod(Callable.From((int len) => box.Text = text[..len]),
                0, text.Length, 1);
            tween.TweenCallback(Callable.From(() => {
                box.Result.FontColor(Colors.DarkRed);
                box.Result.Position = resultBased.Position; // box._leftSide.Position == ZERO
                box.Result.Text = resultBased.Text;
            }));
            tween.TweenProperty(box.Result, "position", resultPos, 0.6).SetDelay(0.5);
            if (right == 10) tween.TweenCallback(Callable.From(() => box.Result.Text += "0")).SetDelay(0.4);
            tween.TweenMethod(box.Rotate(), 0f, MathF.PI, 1f).SetDelay(0.5);
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
    }

    public static RuleReminder Load(int level) {
        var ret = (RuleReminder)ResourceLoader.Load<PackedScene>("res://src/Levels/RuleReminder.tscn").Instantiate();
        ret._level = level;
        return ret;
    }
}