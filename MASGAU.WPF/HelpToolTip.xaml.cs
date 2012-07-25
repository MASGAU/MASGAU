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
using Translator;
namespace MASGAU {
    /// <summary>
    /// Interaction logic for HelpToolTip.xaml
    /// </summary>
    public partial class HelpToolTip : UserControl {
        public object ToolTip {
            get {
                return image.ToolTip;
            }
            set {
                image.ToolTip = value;
            }
        }
        public string TranslatedToolTip {
            get {
                if(ToolTip!=null)
                    return ToolTip.ToString();
                return "";
            }
            set {
                ToolTip = Strings.GetToolTipString(value);
            }
        }

        public HelpToolTip() {
            InitializeComponent();
        }

        private void image_MouseEnter(object sender, MouseEventArgs e) {
        }
    }
}
