using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
