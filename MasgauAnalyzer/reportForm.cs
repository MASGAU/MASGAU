using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Masgau
{
	public partial class reportForm : Form
	{
		private string report;
		public reportForm(string new_report)
		{
			InitializeComponent();
			report = new_report;
		}

		private void reportForm_Shown(object sender, EventArgs e)
		{
			reportText.Text = report;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			if(saveFileDialog1.ShowDialog(this)!=DialogResult.Cancel) {
				try {
					StreamWriter writer = File.CreateText(saveFileDialog1.FileName);
					writer.Write(report);
					writer.Close();
				} catch {
					MessageBox.Show(this,"Eror while trying to write " + saveFileDialog1.FileName,"Pick somewhere else");
				}
				this.DialogResult =DialogResult.OK;
			}
		}

	}
}