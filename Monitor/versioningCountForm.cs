using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MASGAU
{
    public partial class versioningCountForm : Form
    {
        public versioningCountForm(int startingCount)
        {
            InitializeComponent();
            duplicateCount.Value = startingCount;
        }

        public int getCount() {
            return Convert.ToInt32(duplicateCount.Value);
        }
    }
}
