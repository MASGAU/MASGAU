using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Controls.Ribbon;
namespace MASGAU {
    public class FixedRibbonTextBox : RibbonTextBox {
        protected override bool IsEnabledCore {
            get { return true; }
        }
    }

}
