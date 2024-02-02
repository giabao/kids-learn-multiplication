using System;
using System.Linq;
using Godot;

namespace Kids.Levels;

[NoAutoGetNodes] public partial class EquationBox : HBox {
    private const string Op = "x";

    [GetNode] private HBox _leftSide = null!;
    [GetNode("_leftSide.")] public Label Left = null!;
    [GetNode("_leftSide.")] private Label _op = null!;
    [GetNode("_leftSide.")] public Label Right = null!;
    [GetNode] private Label _equal = null!;
    [GetNode] public Label Result = null!;

    public static EquationBox Load() {
        var box = ResourceLoader.Load<PackedScene>("res://Levels/EquationBox.tscn").Instantiate<EquationBox>();
        box.GetNodes();
        return box;
    }

    public Callable Rotate() {
        var labelsInfo = _leftSide.GetChildren().Cast<Label>().Select(c => (c, c.Position)).ToArray();
        return Callable.From((float angle) => {
            foreach (var (c, pos) in labelsInfo) {
                var half = c.Size / 2;
                var pivot = _leftSide.Size / 2;
                // rotate c's center around _leftSide's center 
                var p = (pos + half - pivot).Rotated(angle) + pivot;
                // make c's center at p
                c.Position = p - half;
            }
        });
    }

    public void TypingEffect(string text) {
        Text = "";
        CreateTween().TweenMethod(
            Callable.From((int i) => {
                if (i == Text.Length) return;
                Main.Audio.PlayClick();
                Text = text[..i];
            }),
            0, text.Length, 1);
    }

    private string _text = "";

    public override void _Ready() {
        if ((Label?)Left is null) GetNodes();
        _text = $"{Left!.Text}{_op.Text}{Right.Text}{_equal.Text}{Result.Text}";
    }

    public string Text {
        get => _text;
        set {
            if (_text == value) return;
            _text = value;
            var i = value == "" ? -1 : value.IndexOf(Op, 1, StringComparison.Ordinal);
            if (i == -1) {
                Left.Text = value;
                _op.Text = Right.Text = _equal.Text = Result.Text = "";
                return;
            }

            Left.Text = value[..i];
            _op.Text = Op;
            i += Op.Length;
            var j = i >= value.Length ? -1 : value.IndexOf('=', i);
            if (j == -1) {
                Right.Text = value[i..];
                _equal.Text = Result.Text = "";
                return;
            }

            Right.Text = value[i..j];
            _equal.Text = "=";
            Result.Text = value[(j + 1)..];
        }
    }
}