using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Masgau
{
    public partial class zipLinker : Form
    {
        public zipLinker()
        {
            InitializeComponent();
        }
        
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WebBrowser browser = new WebBrowser();
            browser.openBrowser("http://www.7-zip.org/");
        }

    }
}