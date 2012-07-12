using System;
using System.Windows;
using System.Windows.Input;
using Translator.WPF;
namespace MASGAU.Monitor {
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : AWindow {
        public About(IWindow owner)
            : base(owner) {
            InitializeComponent();
            TranslationHelpers.translateWindow(this);
            this.masgauLbl.Content += " v." + Core.version;
            siteLink.NavigateUri = new Uri(Core.masgau_url);
            siteLink.Inlines.Clear();
            siteLink.Inlines.Add(Core.masgau_url);
        }


        private void urlLbl_MouseUp(object sender, MouseButtonEventArgs e) {
            System.Diagnostics.Process.Start(Core.masgau_url);
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
        }

    }
}
