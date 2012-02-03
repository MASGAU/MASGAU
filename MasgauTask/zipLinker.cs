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
            string key = @"htmlfile\shell\open\command";

            RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(key, false);
            string defaultBrowserPath = ((string) registryKey.GetValue(null, null)).Split('"')[1];
            System.Diagnostics.Process.Start(defaultBrowserPath, "http://www.7-zip.org/");
        }

    }
}