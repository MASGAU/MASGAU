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
    public partial class emailForm : Form
    {
        public emailForm()
        {
            InitializeComponent();
        }

        private void emailText_TextChanged(object sender, EventArgs e)
        {
            if(emailText.Text!=""&&emailText.Text.Contains('@')&&emailText.Text.Contains('.')) {
                button2.Enabled = true;
            } else {
                button2.Enabled = false;
            }
        }
    }

}
