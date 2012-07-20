using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
namespace MASGAU.Main {
    public partial class MainWindowNew {
        private void addGame_Click(object sender, RoutedEventArgs e) {
            AddGameGrid.Visibility = System.Windows.Visibility.Visible;

        }

        private void AddGameCancelButton_Click(object sender, RoutedEventArgs e) {
            AddGameGrid.Visibility = System.Windows.Visibility.Collapsed;
        }



    }
}
