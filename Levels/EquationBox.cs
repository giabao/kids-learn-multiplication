using System;
using Godot;

namespace Kids.Levels;

public partial class EquationBox : HBoxContainer {
    private const string Op = "x"; // @onready

    private Label _left; // @onready
    private Label _op; // @onready
    private Label _right; // @onready
    private Label _equal; // @onready
    public Label Result; // @onready

    public override void _Ready() {
        _left = GetNode<Label>("Left");
        _op = GetNode<Label>("Op");
        _right = GetNode<Label>("Right");
        _equal = GetNode<Label>("Equal");
        Result = GetNode<Label>("Result");
    }

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