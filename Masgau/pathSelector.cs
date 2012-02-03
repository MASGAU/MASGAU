using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MASGAU
{
    public partial class pathSelector : Form
    {
        public pathSelector(ArrayList paths)
        {
            InitializeComponent();
            pathCombo.Items.Add("Remove All Manual Paths");
            foreach(string path in paths) {
                if (!pathCombo.Items.Contains(path))
                    pathCombo.Items.Add(path);
            }
            pathCombo.SelectedIndex = 0;
        }
    }
}