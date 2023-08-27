#nullable enable

using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using FluentAssertions.Primitives;

// ReSharper disable InconsistentNaming - extension methods

namespace Test;

public static class TestExtensions {

    private static readonly ImageConverter IMAGE_CONVERTER = new();

    public static void BeImage(this ObjectAssertions objectAssertions, Icon expected) {
        BeImage(objectAssertions, expected.ToBitmap());
    }

    public static void BeImage(this ObjectAssertions objectAssertions, Image expected) {
        Image? actual = objectAssertions.Subject as Image
            ?? (objectAssertions.Subject as Icon)?.ToBitmap();

        if (actual is null) {
            objectAssertions.Subject.Should().BeOfType<Image>();
            return;
        }

        getIconBytes(actual).Should().Equal(getIconBytes(expected));
    }

    private static IEnumerable<byte>? getIconBytes(Image image) {
        return (byte[]?) IMAGE_CONVERTER.ConvertTo(image, typeof(byte[]));
    }

}