using System;

namespace MASGAU.GTK {
	public partial class MainWindow {
		protected void OnRefreshGamesBtnActivated (object sender, System.EventArgs e)
		{
			masgau.detectGamesAsync();
		}

	}
}

