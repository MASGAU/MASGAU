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
using System.ComponentModel;
using System.Collections.ObjectModel;
namespace SMJ.WPF {
    /// <summary>
    /// Interaction logic for SuperButton.xaml
    /// </summary>
    public partial class SuperButton : UserControl {

        private ObservableCollection<object> options = new ObservableCollection<object>();
        private Dictionary<object,EventHandler> events = new Dictionary<object,EventHandler>();

        public SuperButton() {
            InitializeComponent();
            buttonOptions.DataContext = options;
            buttonOptions.ItemsSource = options;
        }

        public Thickness ImageMargin {
            get {
                return image.Margin;
            }
            set {
                image.Margin = value;
            }
        }

        public double ImageHeight {
            get {
                return image.Height;
            }
            set {
                image.Height = value;
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

        public string Text {
            get {
                return label.Text;
            }
            set {
                label.Text = value;
            }
        }

        public TextBlock Label {
            get {
                return label;
            }
        }

        public void clearOptions() {
            options.Clear();
            events.Clear();
            button.Margin = new Thickness(0, 0, 0, 0);
        }

        public void addOption(object name, EventHandler click_event) {
            if (options.Contains(name))
                return;

            options.Add(name);
            events.Add(name, click_event);
            button.Margin = new Thickness(0, 0, 15, 0);
        }

        private void buttonOptions_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            object option = buttonOptions.SelectedItem;
            if (option == null)
                return;

            EventHandler evnt = events[option];
            SuperButtonEventArgs ev = new SuperButtonEventArgs(option);

            evnt.Invoke(this, ev);

            buttonOptions.SelectedIndex = -1;

        }

        public event RoutedEventHandler Click {
            add { button.Click += value; }
            remove { button.Click -= value; }
        }


    }
}
