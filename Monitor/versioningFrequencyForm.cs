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
    public partial class versioningFrequencyForm : Form
    {
        public versioningFrequencyForm(int start_frequency, string start_unit)
        {
            InitializeComponent();
            duplicateFrequencyNumber.Value = start_frequency;
            duplicateFrequencyCombo.SelectedIndex = duplicateFrequencyCombo.Items.IndexOf(start_unit);
        }

        public int getFrequency() {
            return Convert.ToInt32(duplicateFrequencyNumber.Value);
        }
        public string getUnit() {
            return duplicateFrequencyCombo.SelectedItem.ToString();
        }
    }
}
