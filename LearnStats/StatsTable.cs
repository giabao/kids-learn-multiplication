using Godot;
using System.Collections.Generic;

namespace Kids.LearnStats;

public partial class StatsTable : Control {
    private const string ThemeTypeLabel = "StatsLabel";
    private const string ThemeTypeButton = "StatsButton";

    private Info _info; // @onready
    private PlayerData _playerData; // @onready

    public override void _Ready() {
        GetNode<TextureButton>("BackBtn").WithSound().Pressed += Main.Back;
        _info = GetNode<Info>("Info");
        _info.Visible = false;
        _playerData = PlayerData.Load();
        InitStatsTable();
    }

    private void InitStatsTable() {
        var tbl = GetNode<GridContainer>("StatsTable");
        Theme theme = ResourceLoader.Load<Theme>("res://theme.tres");
        var size = new Vector2(70, 70);

        for (var i = 0; i <= 10; i++) {
            tbl.AddChild(CellLabel(i));
        }

        for (var i = 0; i <= 10; i++) {
            tbl.AddChild(CellLabel(i));
            for (var j = 0; j <= 10; j++) {
                tbl.AddChild(CellButton(i * j, i, j));
            }
        }

        return;

        Label CellLabel(int text) => new Label {
            Text = text.ToString(),
            CustomMinimumSize = size,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        }.FontColor(Colors.Black).FontSize(40);

        Button CellButton(int text, int row, int col) {
            var c = new Button {
                Text = text.ToString(),
                CustomMinimumSize = size,
                Theme = theme,
                ThemeTypeVariation = ThemeTypeButton
            };

            var level = MultiplyRule.RuleIndex(row, col);
            RuleStat? stat = _playerData.Stats.GetValueOrDefault(level);
            StyleBox s = stat == null
                ? theme.GetStylebox("normal", ThemeTypeLabel)
                : new StyleBoxFlat { BgColor = CellColor(stat) };
            c.OverrideThemeStylebox(s, "normal", "pressed", "hover");
            c.MouseDefaultCursorShape = CursorShape.PointingHand;
            c.Pressed += () => {
                var gPos = c.GlobalPosition;
                var pos = gPos - new Vector2(_info.Size.X / 2, _info.Size.Y);
                if (pos.X < 10) pos.X = gPos.X + c.Size.X + 10;
                if (pos.Y < 10) pos.Y = gPos.Y + c.Size.Y + 10;
                _info.Show(level, stat, pos);
            };
            return c;
        }
    }

    /** @return Color from green -> yellow -> red depends on stat.LosePercent */
    private static Color CellColor(RuleStat stat) {
        var win = 1 - stat.LosePercent / 100f;
        // square win so the color turn red faster
        // in HSV model, green -> yellow -> red correspond hue from 0.4 -> 0
        var hue = win * win * 0.4f;
        return Color.FromHsv(hue, 1, 1);
    }
}