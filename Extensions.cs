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

    private static T OffsetCenter<T>(this T self) where T : Control {
        var (w, h) = self.Size;
        self.OffsetLeft = -w / 2;
        self.OffsetRight = w / 2;
        self.OffsetTop = -h / 2;
        self.OffsetBottom = h / 2;
        return self;
    }

    public static T LayoutCenter<T>(this T self) where T : Control => self.OffsetCenter().AnchorCenter();

    public static void PosCenter<T>(this T self, Control parent) where T : Control =>
        self.Position = (parent.Size - self.Size) / 2;

    public static T WithSound<T>(this T self) where T : BaseButton {
        self.Pressed += Main.Audio.PlayClick;
        return self;
    }

    // TODO use Tween
    public static void TypingEffect<TC>(this TC c, Action<string> setText, string text, double time = 0.7,
        bool playClick = false) where TC : Control {
        setText("");
        if (text == "") return;
        var i = 0;
        var timer = new Timer { WaitTime = time / text.Length, Autostart = true };
        c.AddChild(timer);
        timer.Timeout += () => {
            if (playClick) Main.Audio.PlayClick();
            setText(text[..++i]);
            if (i < text.Length) return;
            timer.Stop();
            c.RemoveChild(timer);
        };
    }

    public static void TypingEffect(this Label c, string? text = null, double time = 0.7, bool playClick = false) =>
        TypingEffect(c, txt => c.Text = txt, text ?? c.Text, time, playClick);

    public static Vector2 GetStringSize(this Label c) =>
        c.GetThemeDefaultFont().GetStringSize(c.Text, fontSize: c.GetThemeFontSize("font_size"));
}