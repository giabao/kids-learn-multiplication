using Godot;
using System;
using System.Collections.Generic;
using Range = System.Range;

namespace Kids;

public partial class StatsTable : GridContainer {
	private const string ThemeTypeLabel = "StatsLabel";
	private const string ThemeTypeButton = "StatsButton";
	private PlayerData playerData; // lateinit

	public override void _Ready() {
		playerData = PlayerData.Load();

		Theme theme = ResourceLoader.Load<Theme>("res://theme.tres");
		var size = new Vector2(70, 70);

		for (var i = 0; i <= 10; i++) {
			AddChild(CellLabel(i));
		}
		for (var i = 0; i <= 10; i++) {
			AddChild(CellLabel(i));
			for (var j = 0; j <= 10; j++) {
				var level = MultiplyRule.RuleIndex(i, j);
				RuleStat? stat = playerData.Stats.GetValueOrDefault(level);
				if (stat != null) AddButton(i * j, CellColor(stat));
				else AddLabel(i * j);
			}
		}

		return;

		Label CellLabel(int text) => new Label {
			Text = text.ToString(),
			CustomMinimumSize = size,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Theme = theme,
			ThemeTypeVariation = ThemeTypeLabel
		};

		Button CellButton(int text, int row, int col) {
			var c = new Button {
				Text = text.ToString(),
				CustomMinimumSize = size,
				Theme = theme,
				ThemeTypeVariation = ThemeTypeButton
			};
			
			var level = MultiplyRule.RuleIndex(row, col);
			RuleStat? stat = playerData.Stats.GetValueOrDefault(level);
			StyleBox s = stat == null?
				theme.GetStylebox("normal", ThemeTypeLabel) :
				new StyleBoxFlat { BgColor = bgColor };
			c.OverrideThemeStylebox(s, "normal", "pressed", "hover");
			c.MouseDefaultCursorShape = CursorShape.PointingHand;
			return c;
		}
	}

	private static Color CellColor(RuleStat stat) => Color.FromHsv(stat.LosePercent / 100f * 0.4f, 1, 1);
}
