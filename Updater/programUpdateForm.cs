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
    public partial class programUpdateForm : Form
    {
        private string target_url;
        public programUpdateForm(string new_target_url)
        {
            InitializeComponent();
            target_url = new_target_url;
            label1.Text = target_url;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(target_url);
            DialogResult = DialogResult.OK;
        }
    }
}
