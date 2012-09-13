using System;
using System.Windows.Controls;

namespace MASGAU {
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AboutControl : UserControl {
        public AboutControl() {
            InitializeComponent();
            versionLabel.Content += " v." + Common.VersionString;
            siteLink.NavigateUri = new Uri(Common.MasgauUrl);
            siteLink.Inlines.Clear();
            siteLink.Inlines.Add(Common.MasgauUrl);
        }


        protected void openHyperlink(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
