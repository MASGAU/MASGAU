using System;
using Gtk;
using MASGAU;
using Translations;
using System.ComponentModel;
using MASGAU.Analyzer;

public partial class AnalyzerWindow : MASGAU.AWindow
{
	private AnalyzerProgramHandler analyzer;
	
	public AnalyzerWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		this.Title = Strings.get("AnalyzerWindowTitle");
		
		linuxLabel.LabelProp = Strings.get("Linux");
		gameNameLbl.LabelProp = GTKHelpers.getHeaderString("GameNameQuestion");
		gameLocationLbl.LabelProp= GTKHelpers.getHeaderString("GameInstallQuestion");
		installLocation.Title = Strings.get("GameInstallPrompt");
		saveLocationLbl.LabelProp = GTKHelpers.getHeaderString("GameSavesQuestion");
		saveLocation.Title = Strings.get("GameSavesPrompt");
		scanBtn.Label = Strings.get("ScanButton");
		
		
		psLabel.LabelProp = Strings.get("Windows");
		codeLabel.LabelProp = GTKHelpers.getHeaderString("PlayStationCodeQuestion");
		psLocation.Title = Strings.get("PlayStationLocationPrompt");
		psLocationLabel.LabelProp = GTKHelpers.getHeaderString("PlayStationLocationQuestion");
		
		psScanBtn.Label = Strings.get("PlayStationScanButton");
		
		settingsLabel.LabelProp = Strings.get("Settings");
		emailLabel.LabelProp = GTKHelpers.getHeaderString("EmailAddress");
		
		psLocation.SelectFilename("/");
		installLocation.SelectFilename(Environment.GetEnvironmentVariable("PROGRAMFILES"));
		saveLocation.SelectFilename(Environment.GetEnvironmentVariable("USERPROFILE"));
		
		analyzer = new AnalyzerProgramHandler(setup);
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
			this.Title = Strings.get("AnalyzerWindowTitle");
		else 
			this.Title = Strings.get("AnalyzerWindowTitle") + " - " + e.message;
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
		search.Run();
		ReportDialog report = new ReportDialog(search.output,nameEntry.Text,this);
		report.Run();
		this.Show();
	}
}

