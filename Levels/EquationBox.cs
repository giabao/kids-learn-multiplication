using System;
using Godot;

namespace Kids.Levels;

public partial class EquationBox : HBoxContainer {
    private const string Op = "x";

    [OnReady] private Label _left;
    [OnReady] private Label _op;
    [OnReady] private Label _right;
    [OnReady] private Label _equal;
    [OnReady] public Label Result;

    public void SetText(string value) {
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