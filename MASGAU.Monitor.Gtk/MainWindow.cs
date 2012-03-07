using System;
using Gtk;
using Gdk;

public partial class MainWindow: MASGAU.AProgramWindow
{	
	private StatusIcon status;
	private Menu status_menu;
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		status = new StatusIcon(new Pixbuf ("masgau.ico"));
		status.Visible = true;
		
		status_menu = new Menu();
		ImageMenuItem menuItemQuit = new ImageMenuItem ("Quit");
		Gtk.Image appimg = new Gtk.Image(Stock.Quit, IconSize.Menu);
		menuItemQuit.Image = appimg;
		status_menu.Add(menuItemQuit);
		// Quit the application when quit has been clicked.
		menuItemQuit.Activated += delegate { Application.Quit(); };
		status_menu.ShowAll();
		
		status.PopupMenu += HandleStatusPopupMenu;
	}

	void HandleStatusPopupMenu (object o, PopupMenuArgs args)
	{
		status_menu.Popup();
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
