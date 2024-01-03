using System;
using Godot;

namespace Kids;

// Map level number to RuleStat
// Use Godot's Dictionary to support [Export]
// @see https://docs.godotengine.org/en/4.2/tutorials/scripting/c_sharp/diagnostics/GD0102.html
// @see https://docs.godotengine.org/en/stable/tutorials/scripting/resources.html
using RuleStats = Godot.Collections.Dictionary<int, RuleStat>;

public partial class PlayerData : Resource {
	[Export] public RuleStats Stats = [];
	[Export] public int Level = 2; // [0, MultiplyRule.Rules.Length)

	const string SavePath = "user://PlayerData.tres";
	public static PlayerData Load() {
		if (!ResourceLoader.Exists(SavePath)) return new();
		try {
			return ResourceLoader.Load<PlayerData>(SavePath);
		} catch (Exception e) {
			GD.PrintErr(e, "PlayerData.Load");
			return new();
		}
	}
}
