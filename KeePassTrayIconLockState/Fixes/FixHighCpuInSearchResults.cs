#nullable enable

using HarmonyLib;
using KeePass.Plugins;
using KeePass.UI;
using System.Reflection;
using System.Windows.Forms;

namespace KeePassTrayIconLockState.Fixes;

/// <summary>
/// <para>On Windows 10 but not 11, Windows' Accessibility/UI Automation gets into an infinite loop sending <c>WM_GETOBJECT</c> (0x3D), <c>LVM_ISGROUPVIEWENABLED</c> (0x10AF), and <c>LVM_GETVIEW</c> (0x108F) messages to the KeePass search results view when it has at least 1 result.</para>
/// <para>This results in high CPU usage (roughly 50% of one logical CPU core). It lasts until you hide the search results, for example by navigating to a different folder in your KeePass database.</para>
/// <para>To work around this issue and prevent the high CPU usage, this fix will prevent the window process of the search results view from handling <c>LVM_ISGROUPVIEWENABLED</c> messages by returning early. Blocking <c>WM_GETOBJECT</c> or <c>LVM_GETVIEW</c> messages are not necessary to fix this issue.</para>
/// </summary>
public class FixHighCpuInSearchResults: Fix {

    private const int LVM_ISGROUPVIEWENABLED = 0x10AF;

    public void fix(IPluginHost pluginHost, Harmony harmony) {
        MethodInfo wndProc = AccessTools.Method(typeof(CustomListViewEx), "WndProc", [typeof(Message).MakeByRefType()]);

        HarmonyMethod onBeforeCustomListViewExWndProc = new(AccessTools.Method(typeof(FixHighCpuInSearchResults), nameof(ignoreIsGroupViewEnabled), [typeof(Message).MakeByRefType()]));

        harmony.Patch(wndProc, prefix: onBeforeCustomListViewExWndProc);
    }

    /// <returns><c>true</c> if <paramref name="m"/> will not cause high CPU, which allows WndProc to run; or <c>false</c> if it will cause high CPU, which causes WndProc to return early</returns>
    /// <remarks><see href="https://harmony.pardeike.net/articles/patching-prefix.html#changing-the-result-and-skipping-the-original"/></remarks>
    internal static bool ignoreIsGroupViewEnabled(ref Message m) {
        return m.Msg != LVM_ISGROUPVIEWENABLED;
    }

}