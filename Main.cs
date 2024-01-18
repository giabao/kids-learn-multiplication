using System;
using System.Collections.Generic;
using Godot;

namespace Kids;

public partial class Main : Node {
	private static Main I;
	private static readonly Stack<Node> Scenes = [];

	public Main() { I = this; }
	public static void Back() {
		if (Scenes.Count <= 1) return;
		var current = Scenes.Pop();
		ShowPeek();
		I.RemoveChild(current);
	}
	public static void SceneTo(Node scene) {
		ShowPeek(visible: false);
		Scenes.Push(scene);
		I.AddChild(scene);
	}
	public static void SceneTo(string path) => SceneTo(ResourceLoader.Load<PackedScene>(path).Instantiate());

	private static void ShowPeek(bool visible = true) {
		if (Scenes.IsEmpty()) return;
		if (Scenes.Peek() is Control c) c.Visible = visible;
	}

	private AudioStreamManager _audio = new AudioStreamManager();
	public static AudioStreamManager Audio => I._audio;
	
	public override void _Ready() {
		AddChild(_audio);
		SceneTo("res://LevelMap/LevelMap.tscn");
	}
}
