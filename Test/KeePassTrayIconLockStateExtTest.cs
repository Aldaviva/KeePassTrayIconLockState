#nullable enable

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FluentAssertions;
using KeePassTrayIconLockState;
using Xunit;

namespace Test;

public class KeePassTrayIconLockStateExtTest: IDisposable {

    public void Dispose() {
        KeePassTrayIconLockStateExt.invalidateExternalIconCache();
    }

    [Theory]
    [MemberData(nameof(getBuiltInIconData))]
    internal void getBuiltInIcon(DatabaseOpenState databaseOpenState, bool isDarkTheme, Icon expected) {
        Icon actual = KeePassTrayIconLockStateExt.getIcon(databaseOpenState, isDarkTheme);
        actual.Should().BeImage(new Icon(expected, SystemInformation.SmallIconSize));
    }

    public static object[][] getBuiltInIconData => new[] {
        new object[] { DatabaseOpenState.OPEN, true, Resources.unlocked },
        new object[] { DatabaseOpenState.OPEN, false, Resources.unlocked_light },
        new object[] { DatabaseOpenState.OPENING, true, Resources.unlocking },
        new object[] { DatabaseOpenState.OPENING, false, Resources.unlocking_light },
    };

    [Theory]
    [MemberData(nameof(getCustomIconData))]
    internal void getCustomIcon(DatabaseOpenState databaseOpenState, bool isDarkTheme, string externalFilename) {
        File.Copy("red.ico", externalFilename);
        try {
            Icon actual = KeePassTrayIconLockStateExt.getIcon(databaseOpenState, isDarkTheme);
            actual.Should().BeImage(new Icon("red.ico", SystemInformation.SmallIconSize));
        } finally {
            File.Delete(externalFilename);
        }
    }

    public static object[][] getCustomIconData => new[] {
        new object[] { DatabaseOpenState.OPEN, true, "open-darktaskbar.ico" },
        new object[] { DatabaseOpenState.OPEN, false, "open-lighttaskbar.ico" },
        new object[] { DatabaseOpenState.OPENING, true, "opening-darktaskbar.ico" },
        new object[] { DatabaseOpenState.OPENING, false, "opening-lighttaskbar.ico" }
    };

}