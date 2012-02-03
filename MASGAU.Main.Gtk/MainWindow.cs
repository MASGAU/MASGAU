using System;
using System.ComponentModel;
using Gtk;

namespace MASGAU.Main {
	public partial class MainWindow : MASGAU.AWindow
	{
	
	    MASGAU.Main.MainProgramHandler main;
	
		public MainWindow () : base(WindowType.Toplevel)
		{
			Build();
			
            main = new MainProgramHandler(setupDone, Interface.Gtk);
            this.Title = main.program_title;
            disableInterface();
            main.RunWorkerAsync();
	
		}
	
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}
		
        private void setupDone(object sender, RunWorkerCompletedEventArgs e) {
			enableInterface(null,null);
		}
		
		
		protected override void disableInterface ()
		{
			base.disableInterface ();
			notebook1.Sensitive = false;
		}
		protected override void enableInterface (object sender, RunWorkerCompletedEventArgs e)
		{
			base.enableInterface (sender, e);
			notebook1.Sensitive = true;
		}
		
		#region Communication Handler Event handlers
		public override void updateProgress (ProgressChangedEventArgs e)
		{
            applyProgress(progressbar1,e);
		}
		#endregion
		
	}
	
}