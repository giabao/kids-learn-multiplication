namespace Kids.Levels;

using Chickensoft.GoDotTest;
using Godot;
using Kids.Models;
using Shouldly;
using System.Linq;

public class GameLevelTest(Node testScene) : TestClass(testScene) {
    [Test]
    public void Examples() {
        var e = new MulEquation(2, 2);
        (e.Swap == e).ShouldBeTrue();
        for (var _ = 0; _ < 10; _++) {
            var examples = GameLevel.Examples(PlayerData.Load(), 4).Select(x => x.Text);
            examples.ShouldBeUnique();
        }
    }

}
