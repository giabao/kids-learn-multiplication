using Godot;
using System;
using System.Linq;

namespace Kids.LevelMap;
public partial class LevelMap : Control {
	private const int BgWidth = 2222;
	private static readonly Vector2[] ButtonPositions = [
		new(-61 , 272 ),
		new(157 , 382 ),
		new(401,	388 ),
		new(625,	384 ),
		new(831,	430 ),
		new(1001,	327),
		new(1219,	272),
		new(1437, 382),
		new(1681, 400),
		new(1905, 300),
	];
	private PlayerData _playerData = PlayerData.Load();

	public override void _Ready() {
		_playerData = PlayerData.Load();
		var textureRect = GetNode<TextureRect>("%TextureRect");
		foreach (var (rule, i) in MultiplyRule.Rules.WithIndex()) {
			var dx = BgWidth * (i / ButtonPositions.Length);
			var btn = new LevelButton {
				Text = rule.Name,
				Position = ButtonPositions[i % ButtonPositions.Length] + new Vector2(dx, 0),
				IsCurrent = i == _playerData.Level,
				Disabled = i > _playerData.Level,
			};
			btn.Pressed += Main.Audio.PlayClick;
			btn.Pressed += () => LoadLevel(i);
			textureRect.AddChild(btn);
		}
	}

	private static void LoadLevel(int level) {
		var gl = (GameLevel)ResourceLoader.Load<PackedScene>("res://GameLevel/GameLevel.tscn").Instantiate();
		gl.Level = level;
		Main.SceneTo(gl);
	}
}
