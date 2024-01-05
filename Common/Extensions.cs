using System;
using System.Collections.Generic;
using System.Linq;

namespace Kids;

static class Extension {
    public static bool NextBool(this Random r) => r.NextDouble() >= 0.5;
    // public static IEnumerable<(T item, int index)> ZipWithIndex<T>(this IEnumerable<T> self) =>
    //     self.Select((item, index) => (item, index));

    public static void Swap<T>(this List<T> self, int i, int j) {
        var e = self[i];
        self.RemoveAt(i);
        self.Insert(j, e);
    }
}
