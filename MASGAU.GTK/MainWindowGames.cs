using System;

namespace MASGAU.GTK {
	public partial class MainWindow {
		protected void OnRefreshGamesBtnActivated (object sender, System.EventArgs e)
		{
			masgau.detectGamesAsync();
		}

		private void askRefreshGames(string str) {
			if (str == null)
				str = "AskRefreshGames";

			if (this.askTranslatedQuestion(str, false)) {
				masgau.detectGamesAsync ();
			}
		}
	}
}

