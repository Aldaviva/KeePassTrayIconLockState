#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePass.App;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassTrayIconLockState;
using Telerik.JustMock;
using Xunit;

namespace Test {

    public class KeePassTrayIconLockStateExtTest {

        private readonly KeePassTrayIconLockStateExt plugin = new KeePassTrayIconLockStateExt();

        private readonly IPluginHost          keePassHost;
        private readonly MainForm             mainWindow;
        private readonly NotifyIcon           mainNotifyIcon;
        private readonly ToolStripStatusLabel statusPartInfo;

        private readonly TimeSpan loadDelay         = TimeSpan.FromMilliseconds(50 * 2);
        private readonly Icon     smallLockedIcon   = AppIcons.Get(AppIconType.QuadLocked, UIUtil.GetSmallIconSize(), Color.Empty);
        private readonly Icon     smallUnlockedIcon = AppIcons.Get(AppIconType.QuadNormal, UIUtil.GetSmallIconSize(), Color.Empty);

        public KeePassTrayIconLockStateExtTest() {
            Assert.True(Mock.IsProfilerEnabled, "These tests require the JustMock Profiler to be enabled because the " +
                "MainForm class's constructor crashes if you just run it");

            keePassHost    = Mock.Create<IPluginHost>();
            mainWindow     = Mock.Create<MainForm>();
            mainNotifyIcon = Mock.Create<NotifyIcon>();

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
        public async void renderBrieflyAfterLoad() {
            plugin.Initialize(keePassHost);

            Mock.AssertSet(() => mainNotifyIcon.Icon    = smallLockedIcon, Occurs.Never());
            Mock.AssertSet(() => mainNotifyIcon.Visible = false, Occurs.Never());

            await Task.Delay(loadDelay);

            Mock.AssertSet(() => mainNotifyIcon.Icon    = smallLockedIcon, Occurs.Once());
            Mock.AssertSet(() => mainNotifyIcon.Visible = false, Occurs.Once());
        }

        [Fact]
        public async void showWhileDecrypting() {
            plugin.Initialize(keePassHost);
            await Task.Delay(loadDelay);

            statusPartInfo.Text = "Opening database...";

            Mock.AssertSet(() => mainNotifyIcon.Icon    = smallLockedIcon, Occurs.Exactly(2));
            Mock.AssertSet(() => mainNotifyIcon.Visible = true, Occurs.Once());
        }

        [Fact]
        public async void unlockAfterOpeningFile() {
            plugin.Initialize(keePassHost);
            await Task.Delay(loadDelay);

            Mock.Raise(() => mainWindow.FileOpened += null, new FileOpenedEventArgs(null));

            Mock.AssertSet(() => mainNotifyIcon.Icon    = smallUnlockedIcon, Occurs.Once());
            Mock.AssertSet(() => mainNotifyIcon.Visible = true, Occurs.Once());
        }

        [Fact]
        public async void lockAfterClosingFile() {
            plugin.Initialize(keePassHost);
            await Task.Delay(loadDelay);

            Mock.Raise(() => mainWindow.FileOpened += null, new FileOpenedEventArgs(null));
            Mock.Raise(() => mainWindow.FileClosed += null, new FileClosedEventArgs(null, FileEventFlags.Locking));

            Mock.AssertSet(() => mainNotifyIcon.Icon    = smallLockedIcon, Occurs.Exactly(2));
            Mock.AssertSet(() => mainNotifyIcon.Visible = false, Occurs.Exactly(2));
        }

    }

}