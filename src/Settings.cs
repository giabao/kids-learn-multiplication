namespace Kids;
using Godot;

public partial class Settings : TextureRect {
    private const int MusicBusIdx = 1;
    private const int SoundBusIdx = 2;

    public override void _Ready() {
        GetNode<TextureButton>("Close").WithSound().Pressed += Main.HideModal;
        GetNode<TextureButton>("Music").WithSound().Toggled += toggledOn => {
            AudioServer.SetBusMute(MusicBusIdx, !toggledOn);
            Main.MusicPlayer.Playing = toggledOn;
        };
        GetNode<TextureButton>("Sound").WithSound().Toggled +=
            toggledOn => AudioServer.SetBusMute(SoundBusIdx, !toggledOn);
        GetNode<TextureButton>("Vibration").WithSound().Toggled += toggledOn => GD.PrintErr("TODO impl");
        GetNode<TextureButton>("Notification").WithSound().Toggled += toggledOn => GD.PrintErr("TODO impl");
    }
}
