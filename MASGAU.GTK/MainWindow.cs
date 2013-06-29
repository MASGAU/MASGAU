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
			
			gameTreeView.Selection.Mode = SelectionMode.Multiple;
			
			TreeViewColumn col = new TreeViewColumn();
			CellRendererText text = new CellRendererText();
			col.Title = "Game Title";
			col.PackStart(text,true);
			col.AddAttribute(text,"text",0);
			gameTreeView.AppendColumn(col);
			
			col = new TreeViewColumn();
			text = new CellRendererText();
			col.Title = "Version";
			col.PackStart(text,true);
			col.AddAttribute(text,"text",1);
			gameTreeView.AppendColumn(col);
			
			CellRendererToggle toggle = new CellRendererToggle();
			col = new TreeViewColumn();
			col.Title = "Monitor";
			col.PackStart(toggle,true);
			col.AddAttribute(toggle,"active",2);
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
			model.Clear();
			gameTreeView.Model = null;
		}
		
		ListStore model = new ListStore(typeof(string),typeof (string),typeof(bool));
		public void hookData () {
			model.Clear();
			foreach (GameEntry game in Games.DetectedGames) {
				model.AppendValues (game.Title, game.id.Formatted, game.IsMonitored);				
			}
			gameTreeView.Model = model;
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

