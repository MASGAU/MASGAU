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
using MASGAU.Communication.Progress;
using Translations;
namespace MASGAU
{
    /// <summary>
    /// Interaction logic for EmailWindow.xaml
    /// </summary>
    public partial class EmailWindow : AWindow
    {
        public EmailWindow(AWindow owner): base(owner)
        {
            InitializeComponent();
            WPFHelpers.translateWindow(this);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        public override void updateProgress(ProgressUpdatedEventArgs e)
        {
            // Do nothing!
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

        }
        public string email {
            get {
                return emailTxt.Text;
            }
        }

        private void emailTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            saveBtn.IsEnabled = EmailHandler.validateEmailAddress(emailTxt.Text);
        }
    }
}
