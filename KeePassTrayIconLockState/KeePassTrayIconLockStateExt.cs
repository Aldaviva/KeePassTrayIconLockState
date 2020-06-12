#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using KeePass.App;
using KeePass.Plugins;
using KeePass.UI;
using KoKo.Property;

namespace KeePassTrayIconLockState {

    // ReSharper disable once UnusedType.Global
    public class KeePassTrayIconLockStateExt: Plugin {

        private readonly TimeSpan startupDelay = TimeSpan.FromMilliseconds(20);
        private readonly IDictionary<bool, Icon> iconsByFileOpenState = new Dictionary<bool, Icon>();
        private readonly StoredProperty<bool> isFileOpen = new StoredProperty<bool>(false);
        private readonly Property<Icon> trayIcon;

        private IPluginHost keePassHost = null!;

        public KeePassTrayIconLockStateExt() {
            iconsByFileOpenState.Add(true, loadIcon(true));
            iconsByFileOpenState.Add(false, loadIcon(false));

            trayIcon = DerivedProperty<Icon>.Create(isFileOpen, isOpen => iconsByFileOpenState[isOpen]);
        }

        public override bool Initialize(IPluginHost host) {
            keePassHost = host;

            keePassHost.MainWindow.FileOpened += delegate { isFileOpen.Value = true; };
            keePassHost.MainWindow.FileClosed += delegate { isFileOpen.Value = false; };

            trayIcon.PropertyChanged += (sender, args) => renderTrayIcon(args.NewValue);

            Task.Delay(startupDelay)
                .ContinueWith(_ => renderTrayIcon(trayIcon.Value)); // Give KeePass time to stop setting its own icon

            return true;
        }

        private static Icon loadIcon(bool fileOpen) {
            AppIconType appIconType = fileOpen ? AppIconType.QuadNormal : AppIconType.QuadLocked;
            return AppIcons.Get(appIconType, UIUtil.GetSmallIconSize(), Color.Empty);
        }

        private void renderTrayIcon(Icon icon) {
            keePassHost.MainWindow.MainNotifyIcon.Icon = icon;
        }

        public override Image SmallIcon => iconsByFileOpenState[false].ToBitmap();

    }

}