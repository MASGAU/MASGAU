using System;
using System.ComponentModel;
using Gtk;
using MASGAU;
using MASGAU.Main;
using MVC.GTK;
using MVC.Communication;

namespace MASGAU.GTK {
	public partial class MainWindow : AViewWindow, IMainWindow {
		public MainProgramHandler masgau { get; protected set; }
	
		public MainWindow (): base (Gtk.WindowType.Toplevel)
		{
			Build ();
			
			TreeViewColumn col = new TreeViewColumn();
			CellRenderer render = new CellRendererText();
			col.Title = "Game Title";
			col.PackStart(render,true);
			col.AddAttribute(render,"text",0);
			gameTreeView.AppendColumn(col);
			
			col = new TreeViewColumn();
			col.Title = "Version";
			col.PackStart(render,true);
			col.AddAttribute(render,"text",1);
			gameTreeView.AppendColumn(col);
			
			render = new CellRendererToggle();
			col = new TreeViewColumn();
			col.Title = "Monitor";
			col.PackStart(render,true);
			col.AddAttribute(render,"active",2);
			gameTreeView.AppendColumn(col);
			
			
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
	
		public void unHookData() {
			
		}
		
		ListStore model = new ListStore(typeof(string),typeof (string),typeof(bool));
		public void hookData () {
			model.Clear();
			gameTreeView.Model = model;
			foreach (GameEntry game in Games.DetectedGames) {
				model.AppendValues (game.Title, game.id.Formatted, game.IsMonitored);				
			}
		}
	
	
	
		public override void disableInterface ()
		{
			
		}	
		public override void enableInterface ()
		{
			
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
			if(e.message!=null) {
				statusbar1.Push(1,e.message);
			}
	
		}
		public void setTranslatedTitle(string name, params string[] vars) {
			this.Title = name;
		}


	}
}

