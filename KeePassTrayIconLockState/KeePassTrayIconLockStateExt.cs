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
    public class KeePassTrayIconLockStateExt: Plugin, IDisposable {

        internal static readonly TimeSpan STARTUP_DURATION = TimeSpan.FromMilliseconds(2000);
        internal static readonly TimeSpan ANIMATION_PERIOD = TimeSpan.FromMilliseconds(700);

        private readonly IDictionary<DatabaseOpenState, Icon> iconsByFileOpenState = new Dictionary<DatabaseOpenState, Icon>();
        private readonly StoredProperty<bool>                 isDatabaseOpen       = new StoredProperty<bool>(false);

        private IPluginHost        keePassHost = null!;
        private Timer?             animationTimer;
        private Property<TrayIcon> trayIcon = null!;

        public KeePassTrayIconLockStateExt() {
            Icon lockedIcon   = loadIcon(false);
            Icon unlockedIcon = loadIcon(true);

            iconsByFileOpenState.Add(DatabaseOpenState.CLOSED, lockedIcon);
            iconsByFileOpenState.Add(DatabaseOpenState.OPENING, lockedIcon);
            iconsByFileOpenState.Add(DatabaseOpenState.OPEN, unlockedIcon);
        }

        public override bool Initialize(IPluginHost host) {
            keePassHost = host;

            keePassHost.MainWindow.FileOpened += delegate { isDatabaseOpen.Value = true; };
            keePassHost.MainWindow.FileClosed += delegate { isDatabaseOpen.Value = false; };

            ToolStripItem    statusBarInfo = keePassHost.MainWindow.Controls.OfType<StatusStrip>().First().Items[1];
            Property<string> statusBarText = new NativeReadableProperty<string>(statusBarInfo, nameof(ToolStripItem.Text), nameof(ToolStripItem.TextChanged));

            Property<DatabaseOpenState> databaseOpenState = DerivedProperty<DatabaseOpenState>.Create(isDatabaseOpen, statusBarText, (isOpen, statusText) => {
                if (statusText == KPRes.OpeningDatabase2) {
                    return DatabaseOpenState.OPENING;
                } else {
                    return isOpen ? DatabaseOpenState.OPEN : DatabaseOpenState.CLOSED;
                }
            });

            trayIcon = DerivedProperty<TrayIcon>.Create(databaseOpenState, isOpen => new TrayIcon(iconsByFileOpenState[isOpen], isOpen != DatabaseOpenState.CLOSED));

            trayIcon.PropertyChanged += (sender, args) => renderTrayIcon();

            animationTimer = new Timer { Enabled = false, Interval = (int) ANIMATION_PERIOD.TotalMilliseconds };
            bool animationShowClosedIcon = false;
            animationTimer.Tick += delegate {
                keePassHost.MainWindow.MainNotifyIcon.Icon =  iconsByFileOpenState[animationShowClosedIcon ? DatabaseOpenState.CLOSED : DatabaseOpenState.OPEN];
                animationShowClosedIcon                    ^= true;
            };

            databaseOpenState.PropertyChanged += (sender, args) => {
                animationShowClosedIcon = false;
                animationTimer.Enabled  = args.NewValue == DatabaseOpenState.OPENING;
            };

            // Give KeePass time to stop setting its own icon
            Timer startupTimer = new Timer { Enabled = true, Interval = 8 };
            startupTimer.Tick += delegate { renderTrayIcon(); };
            Task.Delay(STARTUP_DURATION).ContinueWith(task => startupTimer.Stop());
            renderTrayIcon();

            return true;
        }

        private static Icon loadIcon(bool fileOpen) {
            AppIconType appIconType = fileOpen ? AppIconType.QuadNormal : AppIconType.QuadLocked;
            return AppIcons.Get(appIconType, UIUtil.GetSmallIconSize(), Color.Empty);
        }

        private void renderTrayIcon() {
            TrayIcon icon = trayIcon.Value;
            keePassHost.MainWindow.MainNotifyIcon.Icon    = icon.image;
            keePassHost.MainWindow.MainNotifyIcon.Visible = icon.isVisible;
        }

        public override Image SmallIcon => iconsByFileOpenState[DatabaseOpenState.CLOSED].ToBitmap();

        public override void Terminate() {
            Dispose();
        }

        public void Dispose() {
            animationTimer?.Dispose();
        }

    }

}