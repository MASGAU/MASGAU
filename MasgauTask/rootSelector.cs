using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Masgau
{
    public partial class rootSelector : Form
    {
        public rootSelector(ArrayList roots)
        {
            InitializeComponent();
            foreach(file_holder root in roots) {
                if(root.relative_path.Contains("%STEAMUSER%")||root.relative_path.Contains("%USERNAME%")) {
                    if (!rootCombo.Items.Contains(root.relative_path))
                        rootCombo.Items.Add(root.relative_path);
                } else {
                    if(!rootCombo.Items.Contains(root.absolute_path))
                        rootCombo.Items.Add(root.absolute_path);
                }
            }
            rootCombo.SelectedIndex = 0;
        }
    }
}