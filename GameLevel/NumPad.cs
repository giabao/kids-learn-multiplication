using Godot;
using System;
using System.Linq;

namespace Kids;

public partial class NumPad : Control {
    private string value = "?";

    [Signal]
    public delegate void ValueChangedEventHandler(string value);

    [Signal]
    public delegate void SubmitEventHandler(int value);

    public override void _Ready() {
        GetNode<TextureButton>("Del").Pressed += () => {
            if (value != "?") {
                value = value[..^1];
                if (value == "") value = "?";
                EmitSignal(SignalName.ValueChanged, value);
            }
        };
        GetNode<TextureButton>("Enter").Pressed += () => {
            if (value != "?") {
                EmitSignal(SignalName.Submit, value.ToInt());
            }
        };
        foreach (var btn in GetNode("GridContainer").GetChildren().Cast<Button>()) {
            btn.Pressed += () => {
                if (value == "?") value = "";
                value += btn.Text;
                EmitSignal(SignalName.ValueChanged, value);
            };
        }
    }

    public void Reset() => value = "?";
}