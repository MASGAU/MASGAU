using System;
using System.ComponentModel;
using Gtk;
using MASGAU.Gtk;
namespace MASGAU.Main {
	public partial class MainWindow : MASGAU.Gtk.AProgramWindow
	{
		
		public MainWindow () : base(WindowType.Toplevel, new MainProgramHandler())
		{
			Build();
			GTKHelpers.translateWindow(this);
			siteUrlLabel.Text = Core.site_url;
			setUpProgramHandler();
		}
	
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}
		
		protected override void setup (object sender, RunWorkerCompletedEventArgs e)
		{
			base.setup (sender, e);
			
			versionLabel.Text += " v." + Core.version;
			
			taskUserEntry.Text = Environment.UserName;
			
			siteUrlLabel.Text = Core.site_url;
			
			enableInterface();
		}
		
		
		public override void disableInterface ()
		{
			base.disableInterface ();
			notebook1.Sensitive = false;
		}
		public override void enableInterface ()
		{
			base.enableInterface ();
			notebook1.Sensitive = true;
		}
		
		
		
		#region Communication Handler Event handlers
		public override void updateProgress (MASGAU.Communication.Progress.ProgressUpdatedEventArgs e)
		{
			//statusbar3.Push(0,e.message);
			progressbar1.Text = e.message;
		}
		#endregion
		
	}
	
}