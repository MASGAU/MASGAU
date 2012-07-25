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
using System.Text.RegularExpressions;
using System.Reflection;
namespace MASGAU {
    /// <summary>
    /// Interaction logic for RibbonSpinner.xaml
    /// </summary>
    public partial class RibbonSpinner : UserControl {
        public static readonly DependencyProperty ValueProperty =
         DependencyProperty.Register("Value", typeof(Int64),
         typeof(RibbonSpinner), new FrameworkPropertyMetadata(Int64.Parse("0"), OnNumberChanged, OnCoerceNumberProperty));

        static RibbonSpinner() {
        }
        public RibbonSpinner() {
            InitializeComponent();
        }

        public string Label {
            get {
                return input.Label;
            }
            set {
                input.Label = value;
            }
        }
        public long Value {
            get {
                return (Int64)GetValue(ValueProperty);
            }
            set {
                SetValue(ValueProperty, value);
            }
        }
        public string InputText {
            set {
                input.Text = value;
            }
        }
        public double TextBoxWidth {
            get {
                return input.TextBoxWidth;
            }
            set {
                input.TextBoxWidth = value;
            }
        }
        private static object OnCoerceNumberProperty(DependencyObject sender, object data) {
            string val = data.ToString();

            long var;
            if (!Int64.TryParse(val, out var)) {
                val = Regex.Replace(val, @"[^0-9]+", "");
            }

            return data;
        }

        private static void OnNumberChanged(DependencyObject source, 
        DependencyPropertyChangedEventArgs e)
        {
            RibbonSpinner control = source as RibbonSpinner;
            control.input.Text = e.NewValue.ToString();
        }

        private void input_TextChanged(object sender, TextChangedEventArgs e) {
            string val = input.Text;
            long var;
            int position = 0;
            foreach (TextChange change in e.Changes) {
                position = change.Offset;
                break;
            }
            if (!Int64.TryParse(val, out var)) {
                input.Text = Regex.Replace(val, @"[^0-9]+", "");
                input.Select(val.Length, position);
            } else {
                if(Value!=var)
                    SetValue(ValueProperty, var);
            }
        }
    }
}
