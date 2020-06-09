#nullable enable

using System.Drawing;
using System.Threading.Tasks;
using KeePass.App;
using KeePass.Plugins;
using KeePass.UI;
using KoKo.Property;

namespace KeePassTrayIconLockState {

    // ReSharper disable once UnusedType.Global
    public class KeePassTrayIconLockStateExt: Plugin {

        private IPluginHost keePassHost = null!;

        private readonly StoredProperty<bool> isFileOpen = new StoredProperty<bool>(false);

        public override bool Initialize(IPluginHost host) {
            keePassHost = host;

            keePassHost.MainWindow.FileOpened += delegate { isFileOpen.Value = true; };
            keePassHost.MainWindow.FileClosed += delegate { isFileOpen.Value = false; };

            isFileOpen.PropertyChanged += delegate { renderTrayIcon(); };

            Task.Delay(20).ContinueWith(task => renderTrayIcon()); // Give KeePass time to stop setting its own icon

            return true;
        }

        private void renderTrayIcon() {
            AppIconType appIconType = isFileOpen.Value ? AppIconType.QuadNormal : AppIconType.QuadLocked;
            Icon appIcon = AppIcons.Get(appIconType, UIUtil.GetSmallIconSize(), Color.Blue);
            keePassHost.MainWindow.MainNotifyIcon.Icon = appIcon;
        }

    }

}