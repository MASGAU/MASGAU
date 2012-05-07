using System;
using Gtk;
using MASGAU;
using Translations;
using System.ComponentModel;
using MASGAU.Analyzer;
using MASGAU.Gtk;
public partial class AnalyzerWindow : MASGAU.Gtk.AWindow
{
	private AnalyzerProgramHandler analyzer;
	
	private string window_title;
	
	public AnalyzerWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		
		GTKHelpers.translateWindow(this);

		window_title = this.Title;
		
		installLocation.Action = FileChooserAction.SelectFolder;
		psLocation.Action = FileChooserAction.SelectFolder;
		saveLocation.Action = FileChooserAction.SelectFolder;
			
		analyzer = new AnalyzerProgramHandler();
		analyzer.RunWorkerCompleted += setup;
		analyzer.RunWorkerAsync();
	}
	
	public void setup(object sender, RunWorkerCompletedEventArgs e) {
        if(!Core.initialized) {
            this.Destroy();
            return;
        }		
		
		nameEntry.attachModelItem("Text",analyzer,"gameTitle");
		emailEntry.attachModelItem("Text",Core.settings,"email");
		scanBtn.attachModelItem("Sensitive",analyzer,"scanEnabled");
		installLocation.attachModelItem("FileName",analyzer,"gamePath");
		saveLocation.attachModelItem("FileName",analyzer,"savePath");
		
		scanBtn.Button.Clicked += OnScanBtnClicked;;
	}
		
	public override void updateProgress (MASGAU.Communication.Progress.ProgressUpdatedEventArgs e)
	{
		if(e.message==null)
			this.Title = window_title;
		else 
			this.Title = window_title + " - " + e.message;
	}	
	
	protected void OnScanBtnClicked (object sender, System.EventArgs e)
	{
		this.Hide();
		SearchingDialog search = new SearchingDialog(analyzer,this);
		switch((Gtk.ResponseType)search.Run()) {
		case Gtk.ResponseType.Ok:
			ReportDialog report = new ReportDialog(search.output,nameEntry.Text,this);
			report.Run();
			break;
		}
		this.Show();
	}
}

