using Godot;

namespace Kids.LevelMap;

[Tool]
public partial class LevelButton : TextureButton {
	private Label _label = new Label {
		AnchorLeft = 0.5f,
		AnchorRight = 0.5f,
		AnchorTop = 0.5f,
		AnchorBottom = 0.5f,
		GrowHorizontal = GrowDirection.Both,
		GrowVertical = GrowDirection.Both,
		OffsetLeft = -3,
		OffsetTop = -44,
	};
	public LevelButton() {
		TextureDisabled = ResourceLoader.Load<Texture2D>("res://assets/LevelMap/button-disable.png");
		TextureClickMask = ResourceLoader.Load<Bitmap>("res://assets/LevelMap/button-clickmask.png");
		_label.FontColor(Colors.Black).FontSize(32);
		AddChild(_label);
	}
	
	private bool _isCurrent;
	[Export] public bool IsCurrent {
		get => _isCurrent;
		set {
			_isCurrent = value;
			var suffix = value ? "current" : "normal";
			TextureNormal = ResourceLoader.Load<Texture2D>($"res://assets/LevelMap/button-{suffix}.png");
		}
	}
	
	[Export] public string Text {
		get => _label.Text;
		set => _label.Text = value;
	}
}
