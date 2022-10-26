KeePassTrayIconLockState
===

Replace the KeePass tray icon with a wireframe padlock to match the style of built-in tray icons in Windows 10 and 11.

- While the database is being opened, an hourglass icon is shown on top of the padlock.
- When the database is locked or closed, the icon will be hidden.
- Supports normal and high-DPI screens (Settings → System → Display → Scale).
- Supports dark and light mode taskbars (Settings → Personalization → Colors → Choose your mode).

## Requirements
- [KeePass 2](https://keepass.info/download.html) for Windows
- [.NET Framework 4.8 runtime](https://dotnet.microsoft.com/download/dotnet-framework/net48)

## Installation
1. Download `KeePassTrayIconLockState.dll` from the [latest release](https://github.com/Aldaviva/KeePassTrayIconLockState/releases/latest).
1. Save `KeePassTrayIconLockState.dll` to the `Plugins` directory inside your KeePass installation directory, *e.g.*
    ```text
    C:\Program Files\KeePass Password Safe 2\Plugins\KeePassTrayIconLockState.dll
    ```
1. Restart KeePass.

## Behavior
<dl>
<dt><img src="https://raw.githubusercontent.com/Aldaviva/KeePassTrayIconLockState/master/KeePassTrayIconLockState/Resources/unlocked.ico" height="16" /> Database is open</dt>
<dd>The tray icon is a wireframe padlock icon.</dd>
<dt><img src="https://raw.githubusercontent.com/Aldaviva/KeePassTrayIconLockState/master/KeePassTrayIconLockState/Resources/unlocking.ico" height="16" /> Database is being loaded and decrypted</dt>
<dd>A small hourglass is added to the padlock tray icon until the database has finished opening.</dd>
<dt>Database is locked or closed</dt>
<dd>Tray icon is removed. To interact with KeePass in this state, launch the program again or use a system-wide KeePass hotkey to open its window or perform auto-type (set in Tools → Options → Integrations).</dd>
</dl>
