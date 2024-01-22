using Godot;

namespace Kids.LevelMaps;

public partial class LevelButton : TextureButton {
    private Label _label = new Label { OffsetLeft = -3, OffsetTop = -44 }.AnchorCenter();

    public LevelButton() {
        TextureDisabled = ResourceLoader.Load<Texture2D>("res://assets/LevelMaps/button-disable.png");
        TextureClickMask = ResourceLoader.Load<Bitmap>("res://assets/LevelMaps/button-clickmask.png");
        _label.FontColor(Colors.Black).FontSize(32);
        AddChild(_label);
    }

    private bool _isCurrent;

    public bool IsCurrent {
        get => _isCurrent;
        set {
            _isCurrent = value;
            var suffix = value ? "current" : "normal";
            TextureNormal = ResourceLoader.Load<Texture2D>($"res://assets/LevelMaps/button-{suffix}.png");
        }
    }

    public string Text {
        get => _label.Text;
        set => _label.Text = value;
    }
}