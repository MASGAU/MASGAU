using System;

namespace MASGAU.Helpers
{
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    public class WinMaximizeHelper
    {
        /// <summary>
        /// Attaches the given <see cref="Window"/> to
        /// the handler responsible of adjusting the
        /// the dimensions of the Window when in maximize mode.
        /// </summary>
        /// <param name="window">The window.</param>
        public static void AttachHandler(Window window)
        {
            IntPtr handle = (new WindowInteropHelper(window)).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(windowProc));
        }

        private static IntPtr windowProc(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    wmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }

        private static void wmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            const int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// X coordinate of point.
            /// </summary>
            public int x;

            /// <summary>
            /// Y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Initializes a new instance of the <see cref="POINT" /> struct.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;

            public POINT ptMaxSize;

            public POINT ptMaxPosition;

            public POINT ptMinTrackSize;

            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            public RECT rcMonitor = new RECT();

            public RECT rcWork = new RECT();

            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;

            public int top;

            public int right;

            public int bottom;

            public static readonly RECT Empty = new RECT();

            public int Width
            {
                get
                {
                    return Math.Abs(right - left); // Abs needed for BIDI OS
                }
            }

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(RECT rcSrc)
                : this(rcSrc.left, rcSrc.top, rcSrc.right, rcSrc.bottom)
            {
            }

            public bool IsEmpty
            {
                get
                {
                    // BUG: On BIDI OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                if (this == RECT.Empty)
                {
                    return "RECT {Empty}";
                }

                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">Another object to compare to.</param>
            /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance;
            /// otherwise, <see langword="false" />.</returns>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect))
                {
                    return false;
                }

                return (this == (RECT)obj);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data
            /// structures like a hash table.
            /// </returns>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }

            /// <summary>
            /// Implements the ==.
            /// </summary>
            /// <param name="rect1">The rect1.</param>
            /// <param name="rect2">The rect2.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary>
            /// Implements the !=.
            /// </summary>
            /// <param name="rect1">The rect1.</param>
            /// <param name="rect2">The rect2.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }
        }

        /// <summary>
        /// Gets the monitor information.
        /// </summary>
        /// <param name="hMonitor">The h monitor.</param>
        /// <param name="lpmi">The lpmi.</param>
        /// <returns><see langword="true" /> if XXXX, <see langword="false" /> otherwise.</returns>
        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// Monitors from window.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("user32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
    }
}
