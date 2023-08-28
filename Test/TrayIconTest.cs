using System.Drawing;
using KeePassTrayIconLockState;

namespace Test;

public class TrayIconTest {

    [Fact]
    public void equality() {
        Icon     image = new("red.ico");
        TrayIcon a     = new(image, true);
        TrayIcon b     = new(image, true);
        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
    }

    [Fact]
    public void inequality() {
        Icon     image = new("red.ico");
        TrayIcon a     = new(image, true);
        TrayIcon b     = new(image, false);
        a.Should().NotBe(b);
        a.GetHashCode().Should().NotBe(b.GetHashCode());
        (a == b).Should().BeFalse();
        (a != b).Should().BeTrue();
    }

}