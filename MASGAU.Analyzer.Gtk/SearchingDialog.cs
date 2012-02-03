using System;
using System.ComponentModel;
using Translations;
namespace MASGAU.Analyzer
{
	public partial class SearchingDialog : MASGAU.ADialog
	{
		private string game_name,game_path,save_path;
		private AnalyzerProgramHandler analyzer;
		private bool cancelled;
		public string output {get; protected set;}
		public SearchingDialog (AnalyzerProgramHandler analyzer, string game_name, string game_path, string save_path, AWindow parent): base(parent)
		{
			this.game_name = game_name;
			this.game_path = game_path;
			this.save_path = save_path;
            this.analyzer = analyzer;
			this.Build ();
			this.Title = Strings.get("SearchingWindowTitle");
			cancelButton.Label = Strings.get("Cancel");
			analyzer.runAnalyzer(HandleAnalyzerRunWorkerCompleted,game_name,game_path,save_path);
		}
		
		void HandleAnalyzerRunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
		{
			output = analyzer.output.ToString();
            if(cancelled) {
				this.Respond(Gtk.ResponseType.Cancel);
			} else {
				this.Respond(Gtk.ResponseType.Ok);
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
		
		protected void OnDestroyEvent (object o, Gtk.DestroyEventArgs args)
		{
			cancel();
		}

	}
}

