#nullable enable

using HarmonyLib;
using KeePass.Plugins;

namespace KeePassTrayIconLockState.Fixes;

public interface Fix {

    public void fix(IPluginHost pluginHost, Harmony harmony);

}