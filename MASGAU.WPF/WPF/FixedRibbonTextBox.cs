using Microsoft.Windows.Controls.Ribbon;
namespace MASGAU {
    public class FixedRibbonTextBox : RibbonTextBox {
        protected override bool IsEnabledCore {
            get { return true; }
        }
    }

}
