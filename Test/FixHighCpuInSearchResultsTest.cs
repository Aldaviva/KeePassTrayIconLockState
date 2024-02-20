using KeePassTrayIconLockState.Fixes;
using System.Windows.Forms;

namespace Test;

public class FixHighCpuInSearchResultsTest {

    [Fact]
    public void returnEarlyOnIsGroupViewEnabledMessage() {
        Message isGroupViewEnabled      = Message.Create(IntPtr.Zero, 0x10AF, IntPtr.Zero, IntPtr.Zero);
        bool    continueMethodExecution = FixHighCpuInSearchResults.ignoreIsGroupViewEnabled(ref isGroupViewEnabled);
        continueMethodExecution.Should().BeFalse();
    }

    [Theory]
    [InlineData(0x7)]
    [InlineData(0x8)]
    [InlineData(0xB)]
    [InlineData(0xF)]
    [InlineData(0x14)]
    [InlineData(0x3D)]
    [InlineData(0x4E)]
    [InlineData(0x7C)]
    [InlineData(0x7D)]
    [InlineData(0x281)]
    [InlineData(0x282)]
    [InlineData(0x1009)]
    [InlineData(0x100C)]
    [InlineData(0x100E)]
    [InlineData(0x101D)]
    [InlineData(0x101F)]
    [InlineData(0x1027)]
    [InlineData(0x102B)]
    [InlineData(0x102C)]
    [InlineData(0x102F)]
    [InlineData(0x1030)]
    [InlineData(0x1032)]
    [InlineData(0x1043)]
    [InlineData(0x104B)]
    [InlineData(0x104D)]
    [InlineData(0x1053)]
    [InlineData(0x1074)]
    [InlineData(0x108F)]
    [InlineData(0x1091)]
    [InlineData(0x109D)]
    [InlineData(0x10A1)]
    [InlineData(0x10C1)]
    [InlineData(0x204E)]
    [InlineData(0xC1AB)]
    public void continueOnOtherMessages(int message) {
        Message isGroupViewEnabled      = Message.Create(IntPtr.Zero, message, IntPtr.Zero, IntPtr.Zero);
        bool    continueMethodExecution = FixHighCpuInSearchResults.ignoreIsGroupViewEnabled(ref isGroupViewEnabled);
        continueMethodExecution.Should().BeTrue("0:{0} is not 0x10AF", message.ToString("X"));
    }

}