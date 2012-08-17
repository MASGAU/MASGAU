using System;
using System.Windows.Controls;

namespace MASGAU {
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AboutControl : UserControl {
        public AboutControl() {
            InitializeComponent();
            versionLabel.Content += " v." + Core.version;
            siteLink.NavigateUri = new Uri(Core.masgau_url);
            siteLink.Inlines.Clear();
            siteLink.Inlines.Add(Core.masgau_url);
        }


        protected void openHyperlink(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
