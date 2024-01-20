using Godot;

namespace Kids;

public partial class Settings : TextureRect {
    private const int MusicBusIdx = 1;
    private const int SoundBusIdx = 2;

    public override void _Ready() {
        GetNode<TextureButton>("Close").Pressed += Main.HideModal;
        GetNode<TextureButton>("Music").Toggled += toggledOn => AudioServer.SetBusMute(MusicBusIdx, !toggledOn);
        GetNode<TextureButton>("Sound").Toggled += toggledOn => AudioServer.SetBusMute(SoundBusIdx, !toggledOn);
        GetNode<TextureButton>("Vibration").Toggled += toggledOn => GD.PrintErr("TODO impl");
        GetNode<TextureButton>("Notification").Toggled += toggledOn => GD.PrintErr("TODO impl");
    }
}