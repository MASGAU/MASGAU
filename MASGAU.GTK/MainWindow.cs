using System;
using System.ComponentModel;
using Gtk;
using MASGAU;
using MASGAU.Main;
using MVC.GTK;
using MVC.Communication;

namespace MASGAU.GTK {
	public partial class MainWindow : AWindow, IMainWindow {
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


			// Load up translations for etc strings


			this.Destroyed += new EventHandler (this.OnDestroyEvent);


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
	
	
		protected void OnDestroyEvent (object sender, EventArgs a)
		{
			//a.RetVal = true;
			Application.Quit ();
		}
	
		public override void updateProgress (ProgressUpdatedEventArgs e) {
			if (e.max == 0) {
				this.progressbar1.Fraction = 0;
			} else {
				this.progressbar1.Fraction = e.value / e.max;
			}
			if(e.message!=null) {
				setStatusBarText(e.message);
			}
	
		}

		protected void setStatusBarText(string text) {
			statusbar1.Push(1,text);
		}

		private void endOfOperations() {
			ProgressHandler.restoreMessage();
			ProgressHandler.value = 0;
			enableInterface();
		}


		protected void OnBtnBackupGamesActivated (object sender, EventArgs e) {
			if (Core.settings.IsBackupPathSet || changeBackupPath()) {
				beginBackup(null);
			}
		}

		protected void OnBtnRefreshGamesActivated (object sender, EventArgs e) {
			masgau.detectGamesAsync();
		}
	}
}

