using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
// Ruthlessly stolen from http://blogs.msdn.com/b/adam_nathan/archive/2006/05/04/589686.aspx

public class GlassHelper {
    [DllImport("dwmapi.dll", PreserveSig = false)]
    static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

    [DllImport("dwmapi.dll", PreserveSig = false)]
    static extern bool DwmIsCompositionEnabled();


    public static bool ExtendGlassFrame(Window window, Thickness margin) {
        if (!DwmIsCompositionEnabled())
            return false;

        IntPtr hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
            throw new InvalidOperationException("The Window must be shown before extending glass.");

        // Set the background to transparent from both the WPF and Win32 perspectives
        window.Background = Brushes.Transparent;
        HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;

        MARGINS margins = new MARGINS(margin);
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
        return true;
    }
}
struct MARGINS {
    public MARGINS(Thickness t) {
        Left = (int)t.Left;
        Right = (int)t.Right;
        Top = (int)t.Top;
        Bottom = (int)t.Bottom;
    }
    public int Left;
    public int Right;
    public int Top;
    public int Bottom;
}