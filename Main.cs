using System.Collections.Generic;
using Godot;

namespace Kids;

public partial class Main : Node {
    private static Main I;
    private static readonly Stack<Node> Scenes = [];

    public Main() {
        I = this;
    }

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

    private AudioStreamManager _audio = new();
    public static AudioStreamManager Audio => I._audio;

    public static void ShowModal(Control c, bool hideOnPressedOutside = true) {
        var bg = new ModalBg(c, hideOnPressedOutside);
        I.AddChild(bg);
        I.AddChild(c.LayoutCenter());
    }

    public static void ShowModal(string path, bool hideOnPressedOutside = true) =>
        ShowModal((Control)ResourceLoader.Load<PackedScene>(path).Instantiate(), hideOnPressedOutside);

    public static void HideModal() {
        var bg = I.GetChildOrNull<ModalBg>(-2);
        if (bg == null) return;
        I.RemoveChild(I.GetChild(-1));
        I.RemoveChild(bg);
    }

    public override void _Ready() {
        AddChild(_audio);
        SceneTo("res://LevelMap/LevelMap.tscn");
    }
}

internal partial class ModalBg(Control c, bool hideOnPressedOutside) : ColorRect {
    private static readonly Vector2 ScreenSize = new(1280, 960);

    public override void _Ready() {
        Size = ScreenSize;
        Color = new(0, 0, 0, 0.2f);
    }

    public override void _GuiInput(InputEvent @event) {
        if (!hideOnPressedOutside) return;
        AcceptEvent();
        if (@event is not InputEventMouseButton e || e.Pressed || c.GetRect().HasPoint(e.Position)) return;
        Main.HideModal();
    }
}