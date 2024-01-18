using Godot;
using System;
using System.Linq;

namespace Kids.LevelMap;
public partial class LevelMap : Control {
	private const int BgWidth = 2222;
	private static Vector2[] ButtonPositions = [
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
	private PlayerData playerData; // lateinit
	public override void _Ready() {
		playerData = PlayerData.Load();
		var textureRect = GetNode<TextureRect>("%TextureRect");
		foreach (var (rule, i) in MultiplyRule.Rules.WithIndex()) {
			var dx = BgWidth * (i / ButtonPositions.Length);
			var btn = new LevelButton() {
				Text = rule.Name,
				Position = ButtonPositions[i % ButtonPositions.Length] + new Vector2(dx, 0),
				IsCurrent = i == playerData.Level,
				Disabled = i > playerData.Level,
			};
			btn.Pressed += () => _loadLevel(i);
			textureRect.AddChild(btn);
		}
	}

	private void _loadLevel(int level) {
		var scene = ResourceLoader.Load<PackedScene>("res://GameLevel/GameLevel.tscn");
		var gl = (GameLevel)scene.Instantiate();
		gl.Level = level;
		GetTree().Root.AddChild(gl);
		Visible = false;
	}
}
