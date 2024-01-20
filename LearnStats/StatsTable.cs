using Godot;
using System.Collections.Generic;
using Kids.LearnStats;

namespace Kids;

public partial class StatsTable : GridContainer {
    private Info _info; // lateinit

    private const string ThemeTypeLabel = "StatsLabel";
    private const string ThemeTypeButton = "StatsButton";
    private PlayerData playerData; // lateinit

    public override void _Ready() {
        _info = GetNode<Info>("%Info");
        _info.Visible = false;
        playerData = PlayerData.Load();

        Theme theme = ResourceLoader.Load<Theme>("res://theme.tres");
        var size = new Vector2(70, 70);

        for (var i = 0; i <= 10; i++) {
            AddChild(CellLabel(i));
        }

        for (var i = 0; i <= 10; i++) {
            AddChild(CellLabel(i));
            for (var j = 0; j <= 10; j++) {
                AddChild(CellButton(i * j, i, j));
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
            RuleStat? stat = playerData.Stats.GetValueOrDefault(level);
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

    private static Color CellColor(RuleStat stat) => Color.FromHsv(stat.LosePercent / 100f * 0.4f, 1, 1);
}