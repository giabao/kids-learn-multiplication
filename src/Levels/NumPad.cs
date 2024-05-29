namespace Kids.Levels;
using System.Linq;
using Godot;

public partial class NumPad : Control {
    private string _value = "?";

    [Signal] public delegate void ValueChangedEventHandler(string value);

    [Signal] public delegate void SubmitEventHandler(int value);

    public override void _Ready() {
        GetNode<TextureButton>("Del").Pressed += () => {
            if (_value == "?") {
                return;
            }
            _value = _value[..^1];
            if (_value == "") {
                _value = "?";
            }
            EmitSignal(SignalName.ValueChanged, _value);
        };
        GetNode<TextureButton>("Enter").Pressed += () => {
            if (_value != "?") {
                EmitSignal(SignalName.Submit, _value.ToInt());
            }
        };
        foreach (var btn in GetNode("GridContainer").GetChildren().Cast<Button>()) {
            btn.Pressed += () => {
                if (_value == "?") {
                    _value = "";
                }
                _value += btn.Text;
                EmitSignal(SignalName.ValueChanged, _value);
            };
        }
    }

    public void Reset() => _value = "?";
}
