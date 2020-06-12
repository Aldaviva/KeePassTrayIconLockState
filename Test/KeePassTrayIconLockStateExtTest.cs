#nullable enable

using System.Drawing;
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

        private const int LOAD_DELAY = 80;

        private readonly KeePassTrayIconLockStateExt extension = new KeePassTrayIconLockStateExt();

        private readonly IPluginHost keePassHost;
        private readonly MainForm mainWindow;
        private readonly NotifyIcon mainNotifyIcon;

        private readonly Icon smallLockedIcon = AppIcons.Get(AppIconType.QuadLocked, UIUtil.GetSmallIconSize(), Color.Empty);
        private readonly Icon smallUnlockedIcon = AppIcons.Get(AppIconType.QuadNormal, UIUtil.GetSmallIconSize(), Color.Empty);

        public KeePassTrayIconLockStateExtTest() {
            Assert.True(Mock.IsProfilerEnabled, "These tests require the JustMock Profiler to be enabled because the " +
                                                "MainForm class's constructor crashes if you just run it");

            keePassHost = Mock.Create<IPluginHost>();
            mainWindow = Mock.Create<MainForm>();
            mainNotifyIcon = Mock.Create<NotifyIcon>();

            Mock.Arrange(() => keePassHost.MainWindow).Returns(mainWindow);
            Mock.Arrange(() => mainWindow.MainNotifyIcon).Returns(mainNotifyIcon);
        }

        [Fact]
        public async void renderBrieflyAfterLoad() {
            extension.Initialize(keePassHost);

            Mock.AssertSet(() => mainNotifyIcon.Icon = smallLockedIcon, Occurs.Never());

            await Task.Delay(LOAD_DELAY);

            Mock.AssertSet(() => mainNotifyIcon.Icon = smallLockedIcon, Occurs.Once());
        }

        [Fact]
        public async void unlockAfterOpeningFile() {
            extension.Initialize(keePassHost);
            await Task.Delay(LOAD_DELAY);

            Mock.Raise(() => mainWindow.FileOpened += null, new FileOpenedEventArgs(null));

            Mock.AssertSet(() => mainNotifyIcon.Icon = smallUnlockedIcon, Occurs.Once());
        }

        [Fact]
        public async void lockAfterClosingFile() {
            extension.Initialize(keePassHost);
            await Task.Delay(LOAD_DELAY);

            Mock.Raise(() => mainWindow.FileOpened += null, new FileOpenedEventArgs(null));
            Mock.Raise(() => mainWindow.FileClosed += null, new FileClosedEventArgs(null, FileEventFlags.Locking));

            Mock.AssertSet(() => mainNotifyIcon.Icon = smallLockedIcon, Occurs.Exactly(2));
        }

    }

}