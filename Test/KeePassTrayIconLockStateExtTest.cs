#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentAssertions;
using KeePass.Forms;
using KeePass.Plugins;
using KeePassTrayIconLockState;
using Telerik.JustMock;
using Xunit;

namespace Test {

    public class KeePassTrayIconLockStateExtTest {

        private static readonly ImageConverter IMAGE_CONVERTER = new ImageConverter();

        private readonly KeePassTrayIconLockStateExt plugin = new KeePassTrayIconLockStateExt();

        private readonly IPluginHost          keePassHost;
        private readonly MainForm             mainWindow;
        private readonly NotifyIcon           mainNotifyIcon;
        private readonly ToolStripStatusLabel statusPartInfo;

        private readonly TimeSpan loadDelay = TimeSpan.FromTicks(KeePassTrayIconLockStateExt.STARTUP_DURATION.Ticks * 2);

        public KeePassTrayIconLockStateExtTest() {
            Assert.True(Mock.IsProfilerEnabled, "These tests require the JustMock Profiler to be enabled because the " +
                "MainForm class's constructor crashes if you just run it");

            keePassHost    = Mock.Create<IPluginHost>();
            mainWindow     = Mock.Create<MainForm>();
            mainNotifyIcon = new NotifyIcon();

            Control.ControlCollection mainWindowControls = Mock.Create<Control.ControlCollection>();
            StatusStrip               statusStrip        = new StatusStrip();
            statusPartInfo = new ToolStripStatusLabel();
            statusStrip.Items.Add(new ToolStripStatusLabel());
            statusStrip.Items.Add(statusPartInfo);
            Mock.Arrange(() => mainWindowControls.OfType<StatusStrip>()).Returns(new[] { statusStrip });

            Mock.Arrange(() => keePassHost.MainWindow).Returns(mainWindow);
            Mock.Arrange(() => mainWindow.MainNotifyIcon).Returns(mainNotifyIcon);
            Mock.Arrange(() => mainWindow.Controls).Returns(mainWindowControls);
        }

        [Fact]
        public void startup() {
            plugin.Initialize(keePassHost);

            mainNotifyIcon.Visible.Should().BeFalse();
            assertIconsEqual(mainNotifyIcon.Icon, Resources.locked);
        }

        [Fact]
        public async void decrypting() {
            plugin.Initialize(keePassHost);
            await Task.Delay(loadDelay);

            statusPartInfo.Text = "Opening database...";

            mainNotifyIcon.Visible.Should().BeTrue();
            assertIconsEqual(mainNotifyIcon.Icon, Resources.unlocking);
        }

        [Fact]
        public async void unlocked() {
            plugin.Initialize(keePassHost);
            await Task.Delay(loadDelay);

            Mock.Raise(() => mainWindow.FileOpened += null, new FileOpenedEventArgs(null));

            mainNotifyIcon.Visible.Should().BeTrue();
            assertIconsEqual(mainNotifyIcon.Icon, Resources.unlocked);
        }

        [Fact]
        public async void locked() {
            plugin.Initialize(keePassHost);
            await Task.Delay(loadDelay);

            Mock.Raise(() => mainWindow.FileOpened += null, new FileOpenedEventArgs(null));
            Mock.Raise(() => mainWindow.FileClosed += null, new FileClosedEventArgs(null, FileEventFlags.Locking));

            mainNotifyIcon.Visible.Should().BeFalse();
            assertIconsEqual(mainNotifyIcon.Icon, Resources.locked);
        }

        internal static void assertIconsEqual(Icon actual, Icon expected) {
            getIconBytes(actual.ToBitmap()).Should().Equal(getIconBytes(expected.ToBitmap()));
        }

        internal static void assertIconsEqual(Image actual, Icon expected) {
            getIconBytes(actual).Should().Equal(getIconBytes(expected.ToBitmap()));
        }

        private static IEnumerable<byte>? getIconBytes(Image image) {
            return (byte[]?) IMAGE_CONVERTER.ConvertTo(image, typeof(byte[]));
        }

    }

}