using System;
using Gtk;

public partial class MainWindow: MASGAU.AProgramWindow
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		MASGAU.GTKHelpers.translateWindow(this);
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
