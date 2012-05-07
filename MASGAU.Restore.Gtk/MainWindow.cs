using System;
using Gtk;

public partial class MainWindow: MASGAU.Gtk.AProgramWindow
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel, null)
	{
		Build ();
		MASGAU.Gtk.GTKHelpers.translateWindow(this);
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
