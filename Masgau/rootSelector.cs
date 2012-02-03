using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;


namespace MASGAU
{
    public partial class rootSelector : Form
    {
        public rootSelector(ArrayList roots)
        {
            InitializeComponent();
            foreach(location_holder root in roots) {
                //if(root.rel_root=="steamuser"||root.rel_root=="") {
                //    if (!rootCombo.Items.Contains(root.relative_path))
                //        rootCombo.Items.Add(root.relative_path);
                //} else {
                    if(!rootCombo.Items.Contains(Path.Combine(root.abs_root,root.path)))
                        rootCombo.Items.Add(Path.Combine(root.abs_root,root.path));
                //}
            }
            rootCombo.SelectedIndex = 0;
        }
        public rootSelector()
        {
            InitializeComponent();
        }
    }
}