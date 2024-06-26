namespace Kids;
using System.Collections.Generic;
using Godot;

public partial class Main : Node {
    public const string LevelMapName = "LevelMap";
    private static Main I = null!;
    private static readonly Stack<Node> Scenes = [];

    public Main() {
        I = this;
    }

    public static void Back() {
        if (Scenes.Count <= 1) {
            return;
        }
        var current = Scenes.Pop();
        ShowPeek();
        I.RemoveChild(current);
    }

    public static void SceneTo(Node scene, string? name = null, bool replace = false) {
        if (replace) {
            Back();
        }
        ShowPeek(visible: false);
        Scenes.Push(scene);
        I.AddChild(scene);
        if (name != null) {
            scene.Name = name;
        }
    }

    public static void SceneTo(string path, string? name = null) =>
        SceneTo(ResourceLoader.Load<PackedScene>(path).Instantiate(), name);

    public static T Scene<T>(string name) where T : Node => I.GetNode<T>(name);

    private static void ShowPeek(bool visible = true) {
        if (Scenes.IsEmpty()) {
            return;
        }
        if (Scenes.Peek() is Control c) {
            c.Visible = visible;
        }
    }

    private AudioStreamManager _audio = new();
    public static AudioStreamManager Audio => I._audio;
    public static AudioStreamPlayer MusicPlayer => I.GetNode<AudioStreamPlayer>("MusicPlayer");

    public static void ShowModal(Control c, bool hideOnPressedOutside = true) {
        var bg = new ModalBg(c, hideOnPressedOutside);
        I.AddChild(bg);
        I.AddChild(c.LayoutCenter());
    }

    public static void ShowModal(string path, bool hideOnPressedOutside = true) =>
        ShowModal((Control)ResourceLoader.Load<PackedScene>(path).Instantiate(), hideOnPressedOutside);

    public static void HideModal() {
        var bg = I.GetChildOrNull<ModalBg>(-2);
        if (bg == null) {
            return;
        }
        I.RemoveChild(I.GetChild(-1));
        I.RemoveChild(bg);
    }

    public override void _Ready() {
        AddChild(_audio);
        SceneTo("res://src/LevelMaps/LevelMap.tscn", LevelMapName);
    }
}

internal partial class ModalBg(Control c, bool hideOnPressedOutside) : ColorRect {
    public override void _Ready() {
        Size = GetWindow().Size;
        Color = new(0, 0, 0, 0.2f);
    }

    public override void _GuiInput(InputEvent @event) {
        if (!hideOnPressedOutside) {
            return;
        }
        AcceptEvent();
        if (@event is not InputEventMouseButton e || e.Pressed || c.GetRect().HasPoint(e.Position)) {
            return;
        }
        Main.HideModal();
    }
}
