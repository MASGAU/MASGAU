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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class IconButton : UserControl {
        public IconButton() {
            InitializeComponent();
            
        }
        public TextBlock Label {
            get {
                return label;
            }
        }

        public event RoutedEventHandler Click {
            add { button.Click += value; }
            remove { button.Click -= value; }
        }

        public new Thickness Padding {
            get {
                return button.Padding;
            }
            set {
                button.Padding = value;
            }
        }

        public new double FontSize {
            get {
                return label.FontSize;
            }
            set {
                label.FontSize = value;
            }
        }

        public string Text {
            get {
                return label.Text;
            }
            set {
                label.Text = value;
            }
        }

        public ImageSource ImageSource {
            get {
                return image.Source;
            }
            set {
                image.Source = value;
            }
        }

    }
}
