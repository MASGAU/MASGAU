using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using GameSaveInfo;
namespace GSMConverter {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        AConverter gsm;
        private void GoButton_Click(object sender, RoutedEventArgs e) {
//            try {
                gsm = new GameSaveManager(input.Text);

                StringBuilder entries = new StringBuilder();
                StringBuilder namesses = new StringBuilder();
                List<string> game_names = new List<string>();

                gsm.output.sortEntries();


                foreach (Game game in gsm.output.Entries) {
                    entries.AppendLine(game.XML.OuterXml);
                    namesses.AppendLine("* " + game.Title);
                }

                output.Text = entries.ToString();
                names.Text = namesses.ToString();

  //         } catch (Exception ex) {
   //             MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
     //       }
        }

        
    }
}
