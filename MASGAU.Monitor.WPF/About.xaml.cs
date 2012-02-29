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
using System.Windows.Shapes;

namespace MASGAU.Monitor
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : AWindow
    {
        public About(AWindow owner): base(owner)
        {
            InitializeComponent();
            WPFHelpers.translateWindow(this);
            this.masgauLbl.Content += Core.version;
            siteLink.NavigateUri = new Uri(Core.site_url);
            siteLink.Inlines.Clear();
            siteLink.Inlines.Add(Core.site_url);
        }


        private void urlLbl_MouseUp(object sender, MouseButtonEventArgs e)
        {
             System.Diagnostics.Process.Start(Core.site_url);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
