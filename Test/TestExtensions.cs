#nullable enable

using System.Drawing;
using System.Windows.Forms;
using FluentAssertions.Primitives;

// ReSharper disable InconsistentNaming - extension methods

namespace Test;

public static class TestExtensions {

    private static readonly ImageConverter IMAGE_CONVERTER = new();

    public static void BeImage(this ObjectAssertions objectAssertions, Icon expected, string because = "", params object[] becauseArgs) {
        BeImage(objectAssertions, expected.ToBitmap(), because, becauseArgs);
    }

    public static void NotBeImage(this ObjectAssertions objectAssertions, Icon expected, string because = "", params object[] becauseArgs) {
        NotBeImage(objectAssertions, expected.ToBitmap(), because, becauseArgs);
    }

    public static void BeImage(this ObjectAssertions objectAssertions, Image expected, string because = "", params object[] becauseArgs) {
        BeImage(objectAssertions, expected, true, because, becauseArgs);
    }

    public static void NotBeImage(this ObjectAssertions objectAssertions, Image expected, string because = "", params object[] becauseArgs) {
        BeImage(objectAssertions, expected, false, because, becauseArgs);
    }

    private static void BeImage(ObjectAssertions objectAssertions, Image expected, bool shouldBeEqual = true, string because = "", params object[] becauseArgs) {
        Image? actual = objectAssertions.Subject as Image
            ?? (objectAssertions.Subject as Icon)?.ToBitmap();

        if (actual is null) {
            objectAssertions.Subject.Should().BeOfType<Image>();
            return;
        }

        IEnumerable<byte>? expectedBytes = getIconBytes(expected);
        IEnumerable<byte>? actualBytes   = getIconBytes(actual);

        if (shouldBeEqual) {
            actualBytes.Should().Equal(expectedBytes, because, becauseArgs);
        } else {
            actualBytes.Should().NotEqual(expectedBytes, because, becauseArgs);
        }
    }

    private static IEnumerable<byte>? getIconBytes(Image image) {
        return (byte[]?) IMAGE_CONVERTER.ConvertTo(image, typeof(byte[]));
    }

    public static void SetItemParent(this ToolStrip toolStrip, ToolStripItem item) {
        ToolStripHelper.setItemParent(item, toolStrip);
    }

    private class ToolStripHelper: ToolStrip {

        public ToolStripHelper() { }
        public ToolStripHelper(params ToolStripItem[] items): base(items) { }

        public static void setItemParent(ToolStripItem item, ToolStrip parent) => SetItemParent(item, parent);

    }

}