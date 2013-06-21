using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using MVC.WPF;
using Translator;
namespace MASGAU {
    public class NewWindow : AViewWindow {


        public NewWindow()
            : this(null) {

        }
		public NewWindow(AViewWindow owner)
            : base(owner, Core.settings) {
            this.Owner = owner;
            //            var uriSource = new Uri(System.IO.Path.Combine(Core.ExecutablePath, "masgau.ico"), UriKind.Relative);
            //          this.Icon = new BitmapImage(uriSource);

            System.Drawing.Icon ico = Properties.Resources.MASGAUIcon;

            this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ico.ToBitmap().GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

        }

        protected bool toggleMinimize() {
            if (this.WindowState == System.Windows.WindowState.Minimized) {
                this.WindowState = System.Windows.WindowState.Normal;
                return false;
            } else {
                this.WindowState = System.Windows.WindowState.Minimized;
                return true;
            }
        }


        public bool addSavePath(AWindow window) {
            string new_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = Strings.GetLabelString("SelectAltPath");
            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(window.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if (PermissionsHelper.isReadable(new_path)) {
                        if (Core.settings.addSavePath(new_path)) {
                            try_again = false;
                            return true;
                        } else {
                            this.showTranslatedError("SelectAltPathDuplicate");
                            try_again = true;
                        }
                    } else {
                        this.showTranslatedError("SelectAltPathDuplicate");
                        try_again = true;
                    }
                } else {
                    try_again = false;
                }
            } while (try_again);
            return false;
        }

        public string promptForPath(string description, Environment.SpecialFolder root, string path) {
            string new_path;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = description;

            if (path != null)
                folderBrowser.SelectedPath = path;


            folderBrowser.RootFolder = root;


            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    try_again = true;
                    return new_path;
                } else {
                    try_again = false;
                }
            } while (try_again);
            return null;
        }


        public bool changeSyncPath() {
            string old_path = Core.settings.sync_path;
            string new_path = null;
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = "Choose where the saves will be synced.";
            folderBrowser.SelectedPath = old_path;
            bool try_again = false;
            do {
                if (folderBrowser.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK) {
                    new_path = folderBrowser.SelectedPath;
                    if (PermissionsHelper.isReadable(new_path)) {
                        if (PermissionsHelper.isWritable(new_path)) {
                            Core.settings.sync_path = new_path;
                            if (new_path != old_path)
                                Core.rebuild_sync = true;
                            return new_path != old_path;
                        } else {
                            this.displayError("Config File Error", "You don't have permission to write to the selected sync folder:" + Environment.NewLine + new_path);
                            try_again = true;
                        }
                    } else {
                        this.displayError("Config File Error", "You don't have permission to read from the selected sync folder:" + Environment.NewLine + new_path);
                        try_again = true;
                    }
                } else {
                    try_again = false;
                }
            } while (try_again);
            return false;
        }



		// Stuff for handling this full-screen taskbar hiding nonsense
		// Gracefully stolen from http://blogs.msdn.com/b/llobo/archive/2006/08/01/maximizing-window-_2800_with-windowstyle_3d00_none_2900_-considering-taskbar.aspx

		//[DllImport("user32")]
		//internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
		//[DllImport("User32")]
		//internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);


		//public override void OnApplyTemplate() {
		//	System.IntPtr handle = (new System.Windows.Interop.WindowInteropHelper(this)).Handle;
		//	System.Windows.Interop.HwndSource.FromHwnd(handle).AddHook(new System.Windows.Interop.HwndSourceHook(WindowProc));
		//}

		//private static System.IntPtr WindowProc(
		//	  System.IntPtr hwnd,
		//	  int msg,
		//	  System.IntPtr wParam,
		//	  System.IntPtr lParam,
		//	  ref bool handled) {
		//	switch (msg) {
		//		case 0x0024:/* WM_GETMINMAXINFO */
		//			WmGetMinMaxInfo(hwnd, lParam);
		//			handled = true;
		//			break;
		//	}
			
		//	return (System.IntPtr)0;
		//}

		//private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam) {
			
		//	MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

		//	// Adjust the maximized size and position to fit the work area of the correct monitor
		//	int MONITOR_DEFAULTTONEAREST = 0x00000002;
		//	System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

		//	if (monitor != System.IntPtr.Zero) {

		//		MONITORINFO monitorInfo = new MONITORINFO();
		//		GetMonitorInfo(monitor, monitorInfo);
		//		Rect rcWorkArea = monitorInfo.rcWork;
		//		Rect rcMonitorArea = monitorInfo.rcMonitor;
		//		mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
		//		mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
		//		mmi.ptMaxSize.x = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
		//		mmi.ptMaxSize.y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
		//	}

		//	Marshal.StructureToPtr(mmi, lParam, true);
		//}
    }
}
