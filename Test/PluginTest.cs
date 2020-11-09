#nullable enable

using System.Diagnostics;
using System.IO;
using FluentAssertions;
using KeePass.Plugins;
using KeePassTrayIconLockState;
using Xunit;

namespace Test {

    public class PluginTest {

        private readonly KeePassTrayIconLockStateExt plugin = new KeePassTrayIconLockStateExt();


        [Fact]
        public void derivesFromPluginSuperclass() {
            plugin.GetType().IsSubclassOf(typeof(Plugin)).Should().Be(true, "The main plugin class must be derived " +
                                                                            "from the KeePass.Plugins.Plugin base class.");
        }

        [Fact]
        public void isNamespaceNamedCorrectly() {
            string expected = Path.GetFileNameWithoutExtension(plugin.GetType().Assembly.Location);
            string actual = plugin.GetType().Namespace;
            actual.Should().Be(expected, "The namespace must be named like the DLL file without extension. Our DLL file is named " +
                                         "SimplePlugin.dll, therefore the namespace must be called SimplePlugin.");
        }

        [Fact]
        public void isClassNamedCorrectly() {
            string expected = Path.GetFileNameWithoutExtension(plugin.GetType().Assembly.Location) + "Ext";
            string actual = plugin.GetType().Name;
            actual.Should().Be(expected, "The main plugin class (which KeePass will instantiate when it loads your plugin) must be " +
                                         "called exactly the same as the namespace plus \"Ext\". In this case: \"SimplePlugin\" + " +
                                         "\"Ext\" = \"SimplePluginExt\".");
        }

        [Fact]
        public void isProductNameSetCorrectly() {
            string actual = FileVersionInfo.GetVersionInfo(plugin.GetType().Assembly.Location).ProductName;
            actual.Should().Be("KeePass Plugin", "[the Product Name field] must be set to \"KeePass Plugin\" (without the quotes).");
        }

        [Fact]
        public void pluginIcon() {
            KeePassTrayIconLockStateExtTest.assertIconsEqual(plugin.SmallIcon, Resources.locked);
        }

    }

}