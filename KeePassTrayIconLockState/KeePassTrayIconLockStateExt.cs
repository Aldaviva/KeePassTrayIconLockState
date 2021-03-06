﻿#nullable enable

using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePass.Resources;
using KoKo.Property;

namespace KeePassTrayIconLockState {

    // ReSharper disable once UnusedType.Global
    public class KeePassTrayIconLockStateExt: Plugin {

        internal static readonly TimeSpan STARTUP_DURATION = TimeSpan.FromMilliseconds(2000);

        private readonly StoredProperty<DatabaseOpenState> databaseOpenState = new();

        private IPluginHost        keePassHost = null!;
        private Property<TrayIcon> trayIcon    = null!;

        public override Image SmallIcon => Resources.plugin_image;
        public override string UpdateUrl { get; } = @"https://raw.githubusercontent.com/Aldaviva/KeePassTrayIconLockState/master/KeePassTrayIconLockState/version.txt";

        public override bool Initialize(IPluginHost host) {
            keePassHost = host;

            keePassHost.MainWindow.FileOpened += delegate { databaseOpenState.Value = DatabaseOpenState.OPEN; };
            keePassHost.MainWindow.FileClosed += delegate { databaseOpenState.Value = DatabaseOpenState.CLOSED; };

            ToolStripItemCollection statusBarItems       = keePassHost.MainWindow.Controls.OfType<StatusStrip>().First().Items;
            ToolStripItem           statusBarInfo        = statusBarItems[1];
            ToolStripItem           progressBar          = statusBarItems[2];
            Property<string>        statusBarText        = new NativeReadableProperty<string>(statusBarInfo, nameof(ToolStripItem.Text), nameof(ToolStripItem.TextChanged));
            Property<bool>          isProgressBarVisible = new NativeReadableProperty<bool>(progressBar, nameof(ToolStripItem.Visible), nameof(ToolStripItem.VisibleChanged));

            statusBarText.PropertyChanged += (_, args) => {
                if (args.NewValue == KPRes.OpeningDatabase2) {
                    /*
                     * When the status bar text changes to "Opening database...", set the db open state to OPENING.
                     */
                    databaseOpenState.Value = DatabaseOpenState.OPENING;
                }
            };

            isProgressBarVisible.PropertyChanged += (_, args) => {
                if (!args.NewValue && databaseOpenState.Value == DatabaseOpenState.OPENING) {
                    /*
                     * When the database is being opened and the progress bar gets hidden, it means the database was finished being decrypted, but was it successful or unsuccessful?
                     * Sadly there is no good way to tell, so instead we wait 100 ms for the FileOpen event to be fired.
                     * If it is fired, the db open state moves from OPENING to OPEN (above).
                     * Otherwise, assume that the database failed to decrypt and set the db open state to CLOSED.
                     */
                    Task.Delay(100).ContinueWith(_ => {
                        if (databaseOpenState.Value == DatabaseOpenState.OPENING) { // failed to decrypt, otherwise this would have been set to OPEN by the FileOpen event above.
                            databaseOpenState.Value = DatabaseOpenState.CLOSED;
                        }
                    });

                } else if (args.NewValue && databaseOpenState.Value == DatabaseOpenState.CLOSED && statusBarText.Value == KPRes.OpeningDatabase2) {
                    /*
                     * When the database is closed and the status bar says "Opening database..." and the progress bar gets shown, it means the user already failed the previous decryption
                     * attempt and is retrying after submitting another password, so set the db opening state to OPENING.
                     */
                    databaseOpenState.Value = DatabaseOpenState.OPENING;
                }
            };

            trayIcon = DerivedProperty<TrayIcon>.Create(databaseOpenState, openState => new TrayIcon(getIcon(openState), openState != DatabaseOpenState.CLOSED));

            trayIcon.PropertyChanged += (_, _) => renderTrayIcon();

            /*
             * KeePass sets its own icon at some indeterminate time after startup, so repeatedly set our own icon every 8 ms for 2 seconds to make sure our icon isn't overridden.
             */
            Timer startupTimer = new() { Enabled = true, Interval = 8 };
            startupTimer.Tick += delegate { renderTrayIcon(); };
            Task.Delay(STARTUP_DURATION).ContinueWith(_ => startupTimer.Stop());
            renderTrayIcon();

            return true;
        }

        private void renderTrayIcon() {
            TrayIcon   iconToRender = trayIcon.Value;
            NotifyIcon keepassIcon  = keePassHost.MainWindow.MainNotifyIcon;

            keepassIcon.Icon    = iconToRender.image;
            keepassIcon.Visible = iconToRender.isVisible;
        }

        private static Icon getIcon(DatabaseOpenState databaseOpenState) => databaseOpenState switch {
            DatabaseOpenState.CLOSED  => Resources.locked,
            DatabaseOpenState.OPENING => Resources.unlocking,
            DatabaseOpenState.OPEN    => Resources.unlocked,
            _                         => throw new ArgumentOutOfRangeException(nameof(databaseOpenState), databaseOpenState, nameof(getIcon))
        };

    }

}