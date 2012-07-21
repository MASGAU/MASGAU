using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using MASGAU.Effects;
namespace MASGAU.Main {
    public partial class MainWindowNew {
        private void addGame_Click(object sender, RoutedEventArgs e) {
            FadeEffect fade = new FadeInEffect(timing);
            fade.Start(AddGameGrid);
            ribbon.IsEnabled = false;
            GameGrid.IsEnabled = false;
            ArchiveGrid.IsEnabled = false;
        }

        private void AddGameCancelButton_Click(object sender, RoutedEventArgs e) {
            FadeEffect fade = new FadeOutEffect(timing);
            fade.Start(AddGameGrid);
            ribbon.IsEnabled = true;
            GameGrid.IsEnabled = true;
            ArchiveGrid.IsEnabled = true;

        }



    }
}
