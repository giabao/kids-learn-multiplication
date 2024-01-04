using Godot;
using System;
namespace Kids;

public partial class HealthBox : VBoxContainer {
	[Signal] public delegate void HealthEmptyEventHandler();
	private int Health => GetChildCount();

	public override void _Ready() {
		GetNode<GameLevel>("%GameLevel").HealthDown += OnHealthDown;
	}
	private void OnHealthDown() {
		if (Health == 1) {
			EmitSignal(SignalName.HealthEmpty);
		} else {
			RemoveChild(GetChild(0));
		}
	}
}
