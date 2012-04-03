using System;
using Gtk;
using MASGAU;
using Translations;
using System.ComponentModel;
using MASGAU.Analyzer;

public partial class AnalyzerWindow : MASGAU.AWindow
{
	private AnalyzerProgramHandler analyzer;
	
	private string window_title;
	
	public AnalyzerWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		
		GTKHelpers.translateWindow(this);

		window_title = this.Title;
		
		psLocation.SelectFilename("/");
		installLocation.SelectFilename(Environment.GetEnvironmentVariable("PROGRAMFILES"));
		saveLocation.SelectFilename(Environment.GetEnvironmentVariable("USERPROFILE"));
		
		
		analyzer = new AnalyzerProgramHandler();
		analyzer.RunWorkerAsync();
	}
	
	public void setup(object sender, RunWorkerCompletedEventArgs e) {
        if(!Core.initialized) {
            this.Destroy();
            return;
        }		
		
		emailEntry.attachModelItem(Core.settings,"email");
	}
	
	public override void disableInterface ()
	{
		this.Sensitive = false;
	}
			
	public override void enableInterface ()
	{
		this.Sensitive = true;
	}
	
	public override void updateProgress (MASGAU.Communication.Progress.ProgressUpdatedEventArgs e)
	{
		if(e.message==null)
			this.Title = window_title;
		else 
			this.Title = window_title + " - " + e.message;
	}	
	
		
	private void updateButton() {
		if(nameEntry.Text=="") {
			scanBtn.Sensitive = false;
			psScanBtn.Sensitive = false;
		} else {
			scanBtn.Sensitive = true;
			if(prefixEntry.Text==""||suffixEntry.Text==""||
			   prefixEntry.Text.Length!=4||suffixEntry.Text.Length!=5) {
				psScanBtn.Sensitive = false;
			} else { 
				psScanBtn.Sensitive = true;
			}
		}
	}

	protected void EntryChanged (object sender, System.EventArgs e)
	{
		updateButton();
	}

	protected void OnScanBtnClicked (object sender, System.EventArgs e)
	{
		this.Hide();
		SearchingDialog search = new SearchingDialog(analyzer,nameEntry.Text,installLocation.Filename,saveLocation.Filename,this);
		switch((Gtk.ResponseType)search.Run()) {
		case Gtk.ResponseType.Ok:
			ReportDialog report = new ReportDialog(search.output,nameEntry.Text,this);
			report.Run();
			break;
		}
		this.Show();
	}
}

