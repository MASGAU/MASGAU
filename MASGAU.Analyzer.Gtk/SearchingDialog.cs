using System;
using System.ComponentModel;
using Translations;
using MASGAU.Gtk;
namespace MASGAU.Analyzer
{
	public partial class SearchingDialog : MASGAU.Gtk.ADialog
	{
		private AnalyzerProgramHandler analyzer;
		private bool cancelled;
		public string output {get; protected set;}
		public SearchingDialog (AnalyzerProgramHandler analyzer, AWindow parent): base(parent)
		{
            this.analyzer = analyzer;
			this.Build ();
			
			GTKHelpers.translateWindow(this);
			
			analyzer.runAnalyzer(HandleAnalyzerRunWorkerCompleted);
		}
		
		void HandleAnalyzerRunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
		{
			output = analyzer.output.ToString();
            if(cancelled) {
				this.Respond(global::Gtk.ResponseType.Cancel);
			} else {
				this.Respond(global::Gtk.ResponseType.Ok);
			}
		}
		
		public override void updateProgress (MASGAU.Communication.Progress.ProgressUpdatedEventArgs e)
		{
			GTKHelpers.applyProgress(progressbar1,e);
		}
		
		private void cancel() {
            cancelled = true;
			cancelButton.Sensitive = false;
            analyzer.cancelAnalyzer();
		}
		
		protected void OnCancelButtonClicked (object sender, System.EventArgs e)
		{
			cancel();
		}

		protected void OnClose (object sender, System.EventArgs e)
		{
			cancel();
		}
		
		protected void OnDestroyEvent (object o, global::Gtk.DestroyEventArgs args)
		{
			cancel();
		}

	}
}

