using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Kids;

static class Extension {
    public static bool IsEmpty<T>(this Stack<T> self) => self.Count == 0;
    public static bool IsEmpty<T>(this Queue<T> self) => self.Count == 0;
    public static bool NextBool(this Random r) => r.NextDouble() >= 0.5;

    public static void Swap<T>(this List<T> self, int i, int j) {
        var e = self[i];
        self.RemoveAt(i);
        self.Insert(j, e);
    }

    public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> self) => self.Select((v, i) => (v, i));

    public static void OverrideThemeStylebox(this Control c, StyleBox s, params string[] names) {
        foreach (var name in names) {
            c.AddThemeStyleboxOverride(name, s);
        }
    }

    public static T FontColor<T>(this T self, Color c) where T : Control {
        self.AddThemeColorOverride("font_color", c);
        return self;
    }

    public static T FontSize<T>(this T self, int size) where T : Control {
        self.AddThemeFontSizeOverride("font_size", size);
        return self;
    }

    public static T AnchorCenter<T>(this T self) where T : Control {
        self.AnchorLeft = self.AnchorRight = self.AnchorTop = self.AnchorBottom = 0.5f;
        self.GrowHorizontal = self.GrowVertical = Control.GrowDirection.Both;
        return self;
    }

    public static T LayoutCenter<T>(this T self) where T : Control {
        var (w, h) = self.Size;
        self.OffsetLeft = -w / 2;
        self.OffsetRight = w / 2;
        self.OffsetTop = -h / 2;
        self.OffsetBottom = h / 2;

        return self.AnchorCenter();
    }
}