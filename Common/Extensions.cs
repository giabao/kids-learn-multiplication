using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Kids;

static class Extension {
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
}
