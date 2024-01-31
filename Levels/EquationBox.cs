using System;
using System.Linq;
using Godot;

namespace Kids.Levels;

[NoAutoGetNodes] public partial class EquationBox : HBoxContainer {
    private const string Op = "x";

    private bool _demoMode;

    [Export] public bool DemoMode {
        get => _demoMode;
        set {
            _demoMode = value;
            var color = value ? Colors.DarkRed : Colors.Black;
            _left.AddThemeColorOverride("font_color", color);
        }
    }

    [GetNode] private HBoxContainer _leftSide = null!;
    [GetNode("_leftSide.")] private Label _left = null!;
    [GetNode("_leftSide.")] private Label _op = null!;
    [GetNode("_leftSide.")] private Label _right = null!;
    [GetNode] private Label _equal = null!;
    [GetNode] public Label Result = null!;

    public static EquationBox Load(string text) {
        var box = ResourceLoader.Load<PackedScene>("res://Levels/EquationBox.tscn").Instantiate<EquationBox>();
        box.GetNodes();
        box.DemoMode = true;
        box.Text = text;
        return box;
    }

    public void RotatingEffect() {
        var sep = _leftSide.GetThemeConstant("separation");
        var parentSize = Vector2.Zero;
        var labelsInfo = _leftSide.GetChildren().Cast<Label>()
            .Select((c, i) => {
                var size = c.GetStringSize();
                parentSize.Y = MathF.Max(parentSize.Y, size.Y);
                var pos = new Vector2(parentSize.X + i * sep, 0);
                parentSize.X += size.X;
                var haftSize = size / 2;
                return (c, pos + haftSize, haftSize);
            }).ToArray();
        parentSize.X += (labelsInfo.Length - 1) * sep;
        var pivot = parentSize / 2;

        var tween = CreateTween();
        tween.TweenMethod(Callable.From((Action<float>)Rotate), 0f, MathF.PI, 2);
        tween.Play();
        return;

        void Rotate(float angle) {
            foreach (var (c, center, haftSize) in labelsInfo) {
                // rotated around pivot
                var p = (center - pivot).Rotated(angle) + pivot;
                // make c's center at p
                c.Position = p - haftSize;
            }
        }
    }

    public void TypingEffect(string? text = null, bool playClick = false) {
        text ??= _text;
        this.TypingEffect(v => Text = v, text, playClick: playClick);
    }

    private string _text = "";

    public override void _Ready() {
        if ((Label?)_left is null) GetNodes();
        _text = $"{_left!.Text}{_op.Text}{_right.Text}{_equal.Text}{Result.Text}";
    }

    public string Text {
        get => _text;
        set {
            if (_text == value) return;
            _text = value;
            var i = value == "" ? -1 : value.IndexOf(Op, 1, StringComparison.Ordinal);
            if (i == -1) {
                _left.Text = value;
                _op.Text = _right.Text = _equal.Text = Result.Text = "";
                return;
            }

            _left.Text = value[..i];
            _op.Text = Op;
            i += Op.Length;
            var j = i >= value.Length ? -1 : value.IndexOf('=', i);
            if (j == -1) {
                _right.Text = value[i..];
                _equal.Text = Result.Text = "";
                return;
            }

            _right.Text = value[i..j];
            _equal.Text = "=";
            Result.Text = value[(j + 1)..];
        }
    }
}