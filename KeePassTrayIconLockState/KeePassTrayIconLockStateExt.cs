#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePass.App;
using KeePass.Plugins;
using KeePass.Resources;
using KeePass.UI;
using KoKo.Property;

namespace KeePassTrayIconLockState {

    // ReSharper disable once UnusedType.Global
    public class KeePassTrayIconLockStateExt: Plugin {

        private readonly TimeSpan                             startupDelay         = TimeSpan.FromMilliseconds(50);
        private readonly IDictionary<DatabaseOpenState, Icon> iconsByFileOpenState = new Dictionary<DatabaseOpenState, Icon>();
        private readonly StoredProperty<DatabaseOpenState>    databaseOpenState    = new StoredProperty<DatabaseOpenState>(DatabaseOpenState.CLOSED);
        private readonly Property<TrayIcon>                   trayIcon;

        private IPluginHost keePassHost = null!;

        public KeePassTrayIconLockStateExt() {
            iconsByFileOpenState.Add(DatabaseOpenState.CLOSED, loadIcon(false));
            iconsByFileOpenState.Add(DatabaseOpenState.OPENING, loadIcon(false));
            iconsByFileOpenState.Add(DatabaseOpenState.OPEN, loadIcon(true));

            trayIcon = DerivedProperty<TrayIcon>.Create(databaseOpenState, isOpen => new TrayIcon(iconsByFileOpenState[isOpen], isOpen != DatabaseOpenState.CLOSED));
        }

        public override bool Initialize(IPluginHost host) {
            keePassHost = host;

            keePassHost.MainWindow.FileOpened += delegate { databaseOpenState.Value = DatabaseOpenState.OPEN; };
            keePassHost.MainWindow.FileClosed += delegate { databaseOpenState.Value = DatabaseOpenState.CLOSED; };

            ToolStripItem statusBarInfo = keePassHost.MainWindow.Controls.OfType<StatusStrip>().First().Items[1];
            statusBarInfo.TextChanged += (sender, args) => {
                if (statusBarInfo.Text == KPRes.OpeningDatabase2) {
                    databaseOpenState.Value = DatabaseOpenState.OPENING;
                }
            };

            trayIcon.PropertyChanged += (sender, args) => renderTrayIcon(args.NewValue);

            Task.Delay(startupDelay)
                .ContinueWith(_ => renderTrayIcon(trayIcon.Value)); // Give KeePass time to stop setting its own icon

            return true;
        }

        private static Icon loadIcon(bool fileOpen) {
            AppIconType appIconType = fileOpen ? AppIconType.QuadNormal : AppIconType.QuadLocked;
            return AppIcons.Get(appIconType, UIUtil.GetSmallIconSize(), Color.Empty);
        }

        private void renderTrayIcon(TrayIcon icon) {
            keePassHost.MainWindow.MainNotifyIcon.Icon    = icon.image;
            keePassHost.MainWindow.MainNotifyIcon.Visible = icon.isVisible;
        }

        public override Image SmallIcon => iconsByFileOpenState[DatabaseOpenState.CLOSED].ToBitmap();

        private enum DatabaseOpenState {

            CLOSED,
            OPENING,
            OPEN

        }

    }

}