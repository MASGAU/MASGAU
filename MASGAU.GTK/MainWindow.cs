using System;
using System.ComponentModel;
using Gtk;
using MASGAU;
using MASGAU.Main;
using MVC.GTK;
using MVC.Communication;

public partial class MainWindow: AViewWindow, IMainWindow
{	

	public MainProgramHandler masgau { get; protected set; }

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		masgau = new MainProgramHandler(new MASGAU.Location.LocationsHandler(), this);
		masgau.setupMainProgram ();
	}


	public Config.WindowState StartupState {
		set {
			switch (value) {
			case global::Config.WindowState.Maximized:
				this.Visible = true;
				this.Maximize();
				break;
			case global::Config.WindowState.Iconified:
				this.Unmaximize ();
				break;
			}
		}
	}

	public void unHookData() {}
	public void hookData () {
	}





	protected void OnMapped (object sender, EventArgs e) {
		this.sendMessage ("Mapped");
	}
	protected void OnRealized (object sender, EventArgs e) {
		this.sendMessage ("Realized");
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	public override void updateProgress (ProgressUpdatedEventArgs e) {
		if (e.max == 0) {
			this.progressbar1.Fraction = 0;
		} else {
			this.progressbar1.Fraction = e.value / e.max;
		}
		label2.Text = e.message;

	}
	public void setTranslatedTitle(string name, params string[] vars) {
		this.Title = name;
	}

}
