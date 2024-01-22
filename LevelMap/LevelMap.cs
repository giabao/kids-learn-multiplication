using System;
using Godot;

namespace Kids.LevelMap;

public partial class LevelMap : Control {
    private const int BgWidth = 2222;
    private int _scrollPerLevel; // @onready

    private static readonly Vector2[] ButtonPositions = [
        new(-61, 272),
        new(157, 382),
        new(401, 388),
        new(625, 384),
        new(831, 430),
        new(1001, 327),
        new(1219, 272),
        new(1437, 382),
        new(1681, 400),
        new(1905, 300),
    ];

    private PlayerData _playerData = PlayerData.Load();

    public override void _Ready() {
        _playerData = PlayerData.Load();
        GetNode<TextureButton>("SettingsBtn").WithSound().Pressed +=
            () => Main.ShowModal("res://Settings.tscn");
        GetNode<TextureButton>("StatsBtn").WithSound().Pressed +=
            () => Main.SceneTo("res://LearnStats/LearnStats.tscn");
        var textureRect = GetNode<TextureRect>("%TextureRect");
        var pos = Vector2.Zero;
        foreach (var (rule, i) in MultiplyRule.Rules.WithIndex()) {
            pos = ButtonPositions[i % ButtonPositions.Length];
            pos.X += BgWidth * (i / ButtonPositions.Length);
            var btn = new LevelButton {
                Text = rule.Name,
                Position = pos,
                IsCurrent = i == _playerData.Level,
                Disabled = i > _playerData.Level,
            };
            btn.WithSound().Pressed += () => Main.SceneTo(GameLevel.Load(i));
            textureRect.AddChild(btn);
        }

        _scrollPerLevel = Convert.ToInt32(pos.X / MultiplyRule.Rules.Length);
        LevelScroll(_playerData.Level - 2);
    }

    public void LevelScroll(int level) {
        GetNode<ScrollContainer>("ScrollContainer").ScrollHorizontal += level * _scrollPerLevel;
    }
}