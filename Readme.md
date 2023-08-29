KeePassTrayIconLockState
===

[![Build status](https://img.shields.io/github/actions/workflow/status/Aldaviva/KeePassTrayIconLockState/dotnetframework.yml?branch=master&logo=github)](https://github.com/Aldaviva/KeePassTrayIconLockState/actions/workflows/dotnetframework.yml) [![Test status](https://img.shields.io/testspace/tests/Aldaviva/Aldaviva:KeePassTrayIconLockState/master?passed_label=passing&failed_label=failing&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA4NTkgODYxIj48cGF0aCBkPSJtNTk4IDUxMy05NCA5NCAyOCAyNyA5NC05NC0yOC0yN3pNMzA2IDIyNmwtOTQgOTQgMjggMjggOTQtOTQtMjgtMjh6bS00NiAyODctMjcgMjcgOTQgOTQgMjctMjctOTQtOTR6bTI5My0yODctMjcgMjggOTQgOTQgMjctMjgtOTQtOTR6TTQzMiA4NjFjNDEuMzMgMCA3Ni44My0xNC42NyAxMDYuNS00NFM1ODMgNzUyIDU4MyA3MTBjMC00MS4zMy0xNC44My03Ni44My00NC41LTEwNi41UzQ3My4zMyA1NTkgNDMyIDU1OWMtNDIgMC03Ny42NyAxNC44My0xMDcgNDQuNXMtNDQgNjUuMTctNDQgMTA2LjVjMCA0MiAxNC42NyA3Ny42NyA0NCAxMDdzNjUgNDQgMTA3IDQ0em0wLTU1OWM0MS4zMyAwIDc2LjgzLTE0LjgzIDEwNi41LTQ0LjVTNTgzIDE5Mi4zMyA1ODMgMTUxYzAtNDItMTQuODMtNzcuNjctNDQuNS0xMDdTNDczLjMzIDAgNDMyIDBjLTQyIDAtNzcuNjcgMTQuNjctMTA3IDQ0cy00NCA2NS00NCAxMDdjMCA0MS4zMyAxNC42NyA3Ni44MyA0NCAxMDYuNVMzOTAgMzAyIDQzMiAzMDJ6bTI3NiAyODJjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjY3IDE0LjY3LTEwNiA0NHMtNDQgNjUtNDQgMTA3YzAgNDEuMzMgMTQuNjcgNzYuODMgNDQgMTA2LjVTNjY2LjY3IDU4NCA3MDggNTg0em0tNTU3IDBjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjgzIDE0LjY3LTEwNi41IDQ0UzAgMzkxIDAgNDMzYzAgNDEuMzMgMTQuODMgNzYuODMgNDQuNSAxMDYuNVMxMDkuNjcgNTg0IDE1MSA1ODR6IiBmaWxsPSIjZmZmIi8%2BPC9zdmc%2B)](https://aldaviva.testspace.com/spaces/234282) [![Coverage status](https://img.shields.io/coveralls/github/Aldaviva/KeePassTrayIconLockState?logo=coveralls)](https://coveralls.io/github/Aldaviva/KeePassTrayIconLockState?branch=master)

Replace the default KeePass Windows ME–style tray icon with a wireframe padlock icon to match the style of built-in tray icons in Windows 10 and 11.

- While the database is being opened, an hourglass icon is shown on top of the padlock, so you can tell when it's done opening and ready to use.
- When the database is locked or closed, the icon will be hidden, because it's distracting and not useful in this state that is effectively identical to exiting KeePass.
- Supports normal and high-DPI screens (Settings → System → Display → Scale).
- Supports dark and light mode taskbars (Settings → Personalization → Colors → Choose your mode).

## Requirements
- [KeePass 2](https://keepass.info/download.html) for Windows
- [.NET Framework 4.8 runtime](https://dotnet.microsoft.com/download/dotnet-framework/net48) (included in Windows 10 version 1903 and later)

## Installation
1. Download [**`KeePassTrayIconLockState.dll`**](https://github.com/Aldaviva/KeePassTrayIconLockState/releases/latest/download/KeePassTrayIconLockState.dll) from the [latest release](https://github.com/Aldaviva/KeePassTrayIconLockState/releases/latest).
1. Save `KeePassTrayIconLockState.dll` to the `Plugins` directory inside your KeePass installation directory, or a subfolder, *e.g.*
    ```text
    C:\Program Files\KeePass Password Safe 2\Plugins\KeePassTrayIconLockState\KeePassTrayIconLockState.dll
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

## Configuration
### Custom icons
To change the icons that this plugin renders from the default ones, you can create your own icon files in same directory as this plugin's `KeePassTrayIconLockState.dll` file.

The icons can each contain a 16×16px image for normal DPI (100%) screens and a 32×32px image for high DPI (≈200%) screens. If your scaling factor is not represented, such as 150%, you can supply other dimensions as well, otherwise, Windows will scale down the larger image so that it doesn't look blurry.

You can use any icon editing program you want to edit or convert images to ICO files, such as [Iconaton](https://sourceforge.net/projects/iconaton/files/iconaton/Iconaton%200.1%20Beta%201/).

Icon files with the following filenames will be used by this plugin.

|filename|database state|OS theme|default plugin icon|
|---|---|---|---|
|`opening-lighttaskbar.ico`|loading|light|<img src="https://raw.githubusercontent.com/Aldaviva/KeePassTrayIconLockState/master/KeePassTrayIconLockState/Resources/unlocking-light.ico" height="16" /> black padlock and hourglass|
|`opening-darktaskbar.ico`|loading|dark|<img src="https://raw.githubusercontent.com/Aldaviva/KeePassTrayIconLockState/master/KeePassTrayIconLockState/Resources/unlocking.ico" height="16" /> white padlock and hourglass|
|`open-lighttaskbar.ico`|loaded|light|<img src="https://raw.githubusercontent.com/Aldaviva/KeePassTrayIconLockState/master/KeePassTrayIconLockState/Resources/unlocked-light.ico" height="16" /> black padlock|
|`open-darktaskbar.ico`|loaded|dark|<img src="https://raw.githubusercontent.com/Aldaviva/KeePassTrayIconLockState/master/KeePassTrayIconLockState/Resources/unlocked.ico" height="16" /> white padlock|

Any states for which you don't supply an icon file will be rendered with this plugin's built-in icons, so you don't have to provide all four files if you don't want to.

Changes to these icon files take effect after restarting KeePass.