using Godot;
using System;
using Range = System.Range;

namespace Kids;

public partial class StatsTable : GridContainer {
	public override void _Ready() {
		var theme = ResourceLoader.Load<Theme>("res://theme.tres");;
		var size = new Vector2(70, 70);

		for (var i = 0; i <= 10; i++) {
			AddLabel(i);
		}
		for (var i = 0; i <= 10; i++) {
			AddLabel(i);
			for (var j = 0; j <= 10; j++) {
				AddButton(i * j, j*0.04f);
			}
		}

		return;

		void AddLabel(int text) {
			var c = new Label();
			c.Text = text.ToString();
			c.CustomMinimumSize = size;
			c.HorizontalAlignment = HorizontalAlignment.Center;
			c.VerticalAlignment = VerticalAlignment.Center;
			c.Theme= theme;
			c.ThemeTypeVariation = "StatsLabel";
			AddChild(c);
		}

		void AddButton(int text, float hue) {
			var c = new Button();
			c.Text = text.ToString();
			c.CustomMinimumSize = size;
			c.Theme= theme;
			c.ThemeTypeVariation = "StatsButton";
			var s = new StyleBoxFlat();
			s.BgColor = Color.FromHsv(hue, 1, 1);
			c.AddThemeStyleboxOverride("normal", s);
			c.AddThemeStyleboxOverride("pressed", s);
			AddChild(c);
		}
	}

}
