using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Masgau
{
    public partial class purgeSelector : Form
    {
        public purgeSelector(ArrayList roots)
        {
            InitializeComponent();
            purgeCombo.Items.Add("Purge All Detected Roots");
            foreach(file_holder root in roots) {
                if (!purgeCombo.Items.Contains(root.absolute_path))
                    purgeCombo.Items.Add(root.absolute_path);
            }
            purgeCombo.SelectedIndex = 1;
        }
    }
}