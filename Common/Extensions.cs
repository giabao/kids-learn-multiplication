using System;
using System.Collections.Generic;
using Godot;

namespace Kids;

static class Extension {
    public static bool NextBool(this Random r) => r.NextDouble() >= 0.5;

    public static void Swap<T>(this List<T> self, int i, int j) {
        var e = self[i];
        self.RemoveAt(i);
        self.Insert(j, e);
    }

    public static void OverrideThemeStylebox(this Control c, StyleBox s, params string[] names) {
        foreach (var name in names) {
            c.AddThemeStyleboxOverride(name, s);
        }
    }
}
