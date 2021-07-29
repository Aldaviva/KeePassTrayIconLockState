KeePassTrayIconLockState
===

Replace the KeePass tray icon with a white wireframe style to match Windows 10, with an hourglass shown while the password is being decrypted. When the database is locked or closed, the icon will be hidden.

## Requirements
- [KeePass 2](https://keepass.info/download.html) for Windows
- [.NET Framework 4.8 runtime](https://dotnet.microsoft.com/download/dotnet-framework/net48)

## Installation
1. Download `KeePassTrayIconLockState.dll` from the [latest release](https://github.com/Aldaviva/KeePassTrayIconLockState/releases/latest).
1. Save `KeePassTrayIconLockState.dll` to the `Plugins` directory inside your KeePass installation directory, *e.g.*
    ```text
    C:\Program Files\KeePass Password Safe 2\Plugins\KeePassTrayIconLockState\KeePassTrayIconLockState.dll
    ```
1. Restart KeePass.

## Behavior
<dl>
<dt>Database is open</dt>
<dd>The tray icon is a white wireframe padlock icon that matches the visual style of Windows 10's built-in tray icons.</dd>
<dt>Database is being loaded and decrypted</dt>
<dd>A small hourglass is added to the padlock tray icon until the database is finished opening.</dd>
<dt>Database is locked or closed</dt>
<dd>Tray icon is removed. To interact with KeePass in this state, launch the program again or use a system-wide KeePass hotkey to open its window or perform auto-type (set in Tools → Options → Integrations).</dd>
</dl>
