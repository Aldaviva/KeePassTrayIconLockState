#nullable enable

using System.Drawing;

namespace KeePassTrayIconLockState {

    internal readonly struct TrayIcon {

        internal readonly Icon image;
        internal readonly bool isVisible;

        public TrayIcon(Icon image, bool isVisible) {
            this.image     = image;
            this.isVisible = isVisible;
        }

        private bool Equals(TrayIcon other) {
            return image.Equals(other.image) && isVisible == other.isVisible;
        }

        public override bool Equals(object? obj) {
            return obj is TrayIcon other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return (image.GetHashCode() * 397) ^ isVisible.GetHashCode();
            }
        }

        public static bool operator ==(TrayIcon left, TrayIcon right) {
            return left.Equals(right);
        }

        public static bool operator !=(TrayIcon left, TrayIcon right) {
            return !left.Equals(right);
        }

    }

}