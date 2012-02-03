using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MASGAU
{
    public partial class purgeSelector : Form
    {
        public purgeSelector(Dictionary<string,location_holder> roots)
        {
            InitializeComponent();
            purgeCombo.Items.Add("Purge All Detected Roots");
            foreach(KeyValuePair<string,location_holder> root in roots) {
                if (!purgeCombo.Items.Contains(Path.Combine(root.Value.abs_root,root.Value.path)))
                    purgeCombo.Items.Add(Path.Combine(root.Value.abs_root,root.Value.path));
            }
            purgeCombo.SelectedIndex = 1;
        }
    }
}