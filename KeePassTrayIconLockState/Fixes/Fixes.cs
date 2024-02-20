#nullable enable

using HarmonyLib;
using KeePass.Plugins;
using System.Collections.Generic;

namespace KeePassTrayIconLockState.Fixes;

public static class Fixes {

    public static IEnumerable<Fix> all { get; } = [
        new FixHighCpuInSearchResults()
    ];

    public static void fixAll(IPluginHost host, Harmony harmony) {
        foreach (Fix fix in all) {
            fix.fix(host, harmony);
        }
    }

}