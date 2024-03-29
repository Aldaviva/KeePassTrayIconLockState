﻿using KeePass.App;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassTrayIconLockState;
using System.Drawing;
using System.Windows.Forms;

namespace Test.Elevated;

public class KeePassTrayIconLockStateExtElevatedTest {

    private readonly KeePassTrayIconLockStateExt plugin = new();
    private readonly IPluginHost                 keePassHost;
    private readonly MainForm                    mainWindow;
    private readonly NotifyIcon                  mainNotifyIcon;
    private readonly ToolStripStatusLabel        statusPartInfo;
    private readonly ToolStripProgressBar        progressBar;

    public KeePassTrayIconLockStateExtElevatedTest() {
        Assert.True(Mock.IsProfilerEnabled, "These tests require the JustMock Profiler to be enabled because the " +
            "MainForm class's constructor crashes if you just run it");

        keePassHost    = Mock.Create<IPluginHost>();
        mainWindow     = Mock.Create<MainForm>(Constructor.Mocked, Behavior.Strict);
        mainNotifyIcon = new NotifyIcon();

        Control.ControlCollection mainWindowControls = Mock.Create<Control.ControlCollection>();
        StatusStrip               statusStrip        = new();
        statusStrip.Items.Add(new ToolStripStatusLabel());
        statusPartInfo = new ToolStripStatusLabel();
        statusStrip.Items.Add(statusPartInfo);
        statusStrip.SetItemParent(statusPartInfo);
        progressBar = new ToolStripProgressBar();
        statusStrip.Items.Add(progressBar);
        statusStrip.SetItemParent(progressBar);
        Mock.Arrange(() => mainWindowControls.OfType<StatusStrip>()).Returns(new[] { statusStrip });

        Mock.Arrange(() => keePassHost.MainWindow).Returns(mainWindow);
        Mock.Arrange(() => mainWindow.MainNotifyIcon).Returns(mainNotifyIcon);
        Mock.Arrange(() => mainWindow.Controls).Returns(mainWindowControls);
    }

    [Fact]
    public void startup() {
        plugin.Initialize(keePassHost);

        mainNotifyIcon.Visible.Should().BeFalse();
        mainNotifyIcon.Icon.Should().BeImage(Resources.locked);
    }

    [Fact]
    public void decrypting() {
        plugin.Initialize(keePassHost);
        // Thread.Sleep(loadDelay); // using Thread.Sleep() instead of await Task.Delay(), which apparently never halts when run in a mocking profiler chained to a coverage profiler

        statusPartInfo.Text = "Opening database...";

        mainNotifyIcon.Visible.Should().BeTrue();
        mainNotifyIcon.Icon.Should().BeImage(new Icon(Resources.unlocking, SystemInformation.SmallIconSize));
    }

    [Fact]
    public void unlocked() {
        plugin.Initialize(keePassHost);
        // Thread.Sleep(loadDelay);

        Mock.Raise(() => mainWindow.FileOpened += null, new FileOpenedEventArgs(null));

        mainNotifyIcon.Visible.Should().BeTrue();
        mainNotifyIcon.Icon.Should().BeImage(new Icon(Resources.unlocked, SystemInformation.SmallIconSize));
    }

    [Fact]
    public void locked() {
        plugin.Initialize(keePassHost);
        // Thread.Sleep(loadDelay);

        Mock.Raise(() => mainWindow.FileOpened += null, new FileOpenedEventArgs(null));
        Mock.Raise(() => mainWindow.FileClosed += null, new FileClosedEventArgs(null, FileEventFlags.Locking));

        mainNotifyIcon.Visible.Should().BeFalse();
        mainNotifyIcon.Icon.Should().BeImage(Resources.locked);
    }

    [Fact]
    public void wrongPassword() {
        plugin.Initialize(keePassHost);

        progressBar.Visible = true;
        statusPartInfo.Text = "Opening database...";

        progressBar.Visible = false;
        Thread.Sleep(200);

        mainNotifyIcon.Visible.Should().BeFalse();
        mainNotifyIcon.Icon.Should().BeImage(Resources.locked);
    }

    [Fact]
    public void retryDecrypting() {
        statusPartInfo.Text = "Opening database...";
        progressBar.Visible = false;

        plugin.Initialize(keePassHost);

        mainNotifyIcon.Visible.Should().BeFalse();
        mainNotifyIcon.Icon.Should().BeImage(Resources.locked);

        progressBar.Visible = true;

        mainNotifyIcon.Visible.Should().BeTrue();
        mainNotifyIcon.Icon.Should().BeImage(new Icon(Resources.unlocking, SystemInformation.SmallIconSize));
    }

    [Fact]
    public void keepassUpdateTrayIconHookedCallback() {
        plugin.Initialize(keePassHost);
        mainNotifyIcon.Icon    = AppIcons.Get(AppIconType.QuadNormal, UIUtil.GetSmallIconSize(), Color.Empty);
        mainNotifyIcon.Visible = true;

        KeePassTrayIconLockStateExt.onKeepassRenderedTrayIcon();

        mainNotifyIcon.Visible.Should().BeFalse();
        mainNotifyIcon.Icon.Should().BeImage(Resources.locked);
    }

}