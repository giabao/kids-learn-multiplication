namespace Kids.Levels;
using System;
using Godot;

/** Similar to HBoxContainer but don't auto resize and re-position children */
public partial class HBox : Control {
    private const int Separation = 15;

    public void ReLayout() {
        var s = Vector2.Zero;
        foreach (var (node, i) in GetChildren().WithIndex()) {
            if (node is not Control c) {
                continue;
            }
            if (i == 0) {
                s.X = c.Position.X;
            } else {
                c.Position = new(s.X, c.Position.Y);
            }
            switch (c) {
                case Label lbl:
                    lbl.Size = lbl.TextSize(); // don't consider MinimumSize
                    s.Y = MathF.Max(s.Y, lbl.Size.Y);
                    break;
                case HBox box:
                    box.ReLayout();
                    s.Y = MathF.Max(s.Y, box.Size.Y);
                    break;
                default:
                    break;
            }

            s.X += c.Size.X + Separation;
            s.Y = MathF.Max(s.Y, c.GetCombinedMinimumSize().Y);
        }

        if (s.X > 0) {
            s.X -= Separation;
        }
        Size = s;
    }
}
