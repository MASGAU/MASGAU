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

namespace MASGAU {
    /// <summary>
    /// Interaction logic for ChromeButton.xaml
    /// </summary>
    public partial class ChromeButton : UserControl {
        public ChromeButton() {
            InitializeComponent();
        }
        public ImageSource ImageSource {
            get {
                return image.Source;
            }
            set {
                image.Source = value;
            }
        }

        public event RoutedEventHandler Click {
            add { button.Click += value; }
            remove { button.Click -= value; }
        }
    }
}
