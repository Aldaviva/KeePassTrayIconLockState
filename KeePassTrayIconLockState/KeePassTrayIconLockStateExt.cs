#nullable enable

using Dark.Net;
using HarmonyLib;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.Resources;
using KoKo.Property;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeePassTrayIconLockState;

// ReSharper disable once UnusedType.Global
public class KeePassTrayIconLockStateExt: Plugin {

    private static readonly IDictionary<string, Icon?> EXTERNAL_ICON_CACHE           = new Dictionary<string, Icon?>(6);
    private static readonly string                     PLUGIN_INSTALLATION_DIRECTORY = getPluginInstallationDirectory();

    private static event Action? keepassRenderedTrayIcon;

    private readonly StoredProperty<DatabaseOpenState> databaseOpenState = new();
    private readonly IDarkNet                          darkNet           = new DarkNet();

    private IPluginHost        keePassHost        = null!;
    private Property<TrayIcon> trayIcon           = null!;
    private Property<bool>     taskbarIsDarkTheme = null!;

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
                 * Sadly there is no good way to tell, so instead we wait 100 ms for the FileOpened event to be fired.
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

        taskbarIsDarkTheme = new NativeReadableProperty<bool>(darkNet, nameof(IDarkNet.UserTaskbarThemeIsDark), nameof(IDarkNet.UserTaskbarThemeIsDarkChanged));

        trayIcon = DerivedProperty<TrayIcon>.Create(databaseOpenState, taskbarIsDarkTheme,
            (openState, isDarkTheme) => new TrayIcon(getIcon(openState, isDarkTheme), openState != DatabaseOpenState.CLOSED));

        trayIcon.PropertyChanged += delegate { renderTrayIcon(); };

        renderTrayIcon();

        hookKeepassTrayIconUpdates();
        keepassRenderedTrayIcon += renderTrayIcon;

        return true;
    }

    private void renderTrayIcon() {
        TrayIcon   iconToRender = trayIcon.Value;
        NotifyIcon keepassIcon  = keePassHost.MainWindow.MainNotifyIcon;

        keepassIcon.Icon    = iconToRender.image;
        keepassIcon.Visible = iconToRender.isVisible;
    }

    internal static Icon getIcon(DatabaseOpenState databaseOpenState, bool isDarkTheme) =>
        new(databaseOpenState switch {
            DatabaseOpenState.CLOSED when isDarkTheme  => getExternalCustomIcon("closed-darktaskbar.ico") ?? Resources.locked,
            DatabaseOpenState.CLOSED                   => getExternalCustomIcon("closed-lighttaskbar.ico") ?? Resources.locked,
            DatabaseOpenState.OPENING when isDarkTheme => getExternalCustomIcon("opening-darktaskbar.ico") ?? Resources.unlocking,
            DatabaseOpenState.OPENING                  => getExternalCustomIcon("opening-lighttaskbar.ico") ?? Resources.unlocking_light,
            DatabaseOpenState.OPEN when isDarkTheme    => getExternalCustomIcon("open-darktaskbar.ico") ?? Resources.unlocked,
            DatabaseOpenState.OPEN                     => getExternalCustomIcon("open-lighttaskbar.ico") ?? Resources.unlocked_light
        }, SystemInformation.SmallIconSize);

    private static Icon? getExternalCustomIcon(string filename) {
        if (!EXTERNAL_ICON_CACHE.TryGetValue(filename, out Icon? icon)) {
            try {
                icon = new Icon(Path.Combine(PLUGIN_INSTALLATION_DIRECTORY, filename), SystemInformation.SmallIconSize);
            } catch (FileNotFoundException) {
                // add a null value to the map, so that missing files don't result in disk I/O every time
                icon = null;
            }

            EXTERNAL_ICON_CACHE.Add(filename, icon);
        }

        return icon;
    }

    internal static void invalidateExternalIconCache() {
        EXTERNAL_ICON_CACHE.Clear();
    }

    /// <summary>
    /// <para>There are 3 different directory structures this plugin may be run within:</para>
    /// <para>1. in KeePass: DLL is in <c>plugins</c> subdirectory, CWD is KeePass installation directory, ICOs are in same directory as DLL</para>
    /// <para>2. in ReSharper unit tests: DLL is in <c>bin</c> subdirectory, CWD is the same, ICOs are in same directory as DLL</para>
    /// <para>3. in VSTest (Test Explorer, dotnet test): DLL is copied to <c>temp</c> directory, CWD is in <c>bin</c> subdirectory, ICOs are in CWD instead of the directory of loaded DLL</para>
    /// </summary>
    /// <returns>the absolute path of the directory containing the ICO files</returns>
    private static string getPluginInstallationDirectory() {
        string   executingDllPath = Assembly.GetExecutingAssembly().Location;
        string   cwdDllPath       = Path.Combine(Environment.CurrentDirectory, Path.GetFileName(executingDllPath));
        FileInfo executingDllInfo = new(executingDllPath);
        FileInfo cwdDllInfo       = new(cwdDllPath);

        return cwdDllInfo.Exists && executingDllInfo.Length == cwdDllInfo.Length
            ? Environment.CurrentDirectory              // test scenario
            : Path.GetDirectoryName(executingDllPath)!; // KeePass scenario
    }

    /// <summary>
    /// Use Harmony to modify the bytecode of KeePass' MainForm.UpdateTrayIcon(bool) method to call our own tray icon rendering method.
    /// This prevents KeePass' built-in icons from wrongly replacing my icons.
    /// I do this because KeePass sometimes renders its tray icon without exposing any events to indicate that it's doing so, therefore I can't receive any notifications to trigger my own icon to render. Two examples of this are clicking OK in the Options dialog box and canceling the Unlock Database dialog box when minimize to tray, minimize after locking, and minimize after opening are all enabled.
    /// Also, all of the types in KeePass are static, private, sealed, and don't implement interfaces, so there is no other way to receive notifications that it has rendered its tray icon.
    /// </summary>
    private static void hookKeepassTrayIconUpdates() {
        Harmony harmony = new("com.aldaviva.keepasstrayiconlockstate");

        MethodInfo originalUpdateTrayIconMethod = AccessTools.Method(typeof(MainForm), "UpdateTrayIcon", new[] { typeof(bool) }) ??
            throw new MissingMethodException("Cannot find KeePass.Forms.MainForm.UpdateTrayIcon(bool) method");

        HarmonyMethod onAfterUpdateTrayIcon = new(AccessTools.Method(typeof(KeePassTrayIconLockStateExt), nameof(onKeepassRenderedTrayIcon)));

        harmony.Patch(originalUpdateTrayIconMethod, postfix: onAfterUpdateTrayIcon);
    }

    internal static void onKeepassRenderedTrayIcon() {
        keepassRenderedTrayIcon?.Invoke();
    }

    public override void Terminate() {
        keepassRenderedTrayIcon -= renderTrayIcon;
        darkNet.Dispose();
        base.Terminate();
    }

}