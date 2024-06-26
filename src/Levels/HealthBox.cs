namespace Kids.Levels;
using Godot;

public partial class HealthBox : VBoxContainer {
    [Signal] public delegate void HealthEmptyEventHandler();

    public override void _Ready() => (Owner as GameLevel)!.HealthDown += OnHealthDown;

    private void OnHealthDown() {
        Main.Audio.Play("wrong.mp3");
        var health = GetChildCount();
        if (health == 0) {
            return;
        }
        RemoveChild(GetChild(0));
        if (--health == 0) {
            EmitSignal(SignalName.HealthEmpty);
        }
    }
}
