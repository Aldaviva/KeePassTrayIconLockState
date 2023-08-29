using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("KeePassTrayIconLockState")]
[assembly: AssemblyDescription("Replace the default KeePass Windows ME–style tray icon with a wireframe padlock icon to match the style of built-in tray icons in Windows 10 and 11.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Ben Hutchison")]
[assembly: AssemblyProduct("KeePass Plugin")]
[assembly: AssemblyCopyright("© 2023 Ben Hutchison")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("e980abdc-e56d-4e9c-a322-afbe9d9092b4")]

// Remember to also update version.txt when changing these version attributes so that online update checking works.
// https://keepass.info/help/v2_dev/plg_index.html#upd
[assembly: AssemblyVersion("1.2.1.0")]
[assembly: AssemblyFileVersion("1.2.1.0")]

[assembly: InternalsVisibleTo("Test")]
[assembly: InternalsVisibleTo("Test.Elevated")]