using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;

namespace Masgau
{
	public partial class analyzerForm : Form
	{
		public analyzerForm()
		{
			InitializeComponent();
		}


		private bool sendData() {
			return true;
		}

		private void submitButton_Click(object sender, EventArgs e)
		{
			if(gamePathText.Text=="") {
				MessageBox.Show(this,"You need to specify the install folder for the game.","I'm not psychic");
				return;
			}
			if(saveFolderText.Text=="") {
				MessageBox.Show(this,"You need to specify the folder that contains the game's saves.","I'm not clairvoyant");
				return;
			}
			searchingForm searcher = new searchingForm(gamePathText.Text,saveFolderText.Text,false);
			if(searcher.ShowDialog(this)!=DialogResult.Cancel) {
				reportForm report = new reportForm(searcher.output);
				if(report.ShowDialog(this)!=DialogResult.Cancel) {
				}
			}
		}

        private void button1_Click(object sender, EventArgs e)
        {
			if(prefixTxt.Text==""||suffixTxt.Text=="") {
				MessageBox.Show(this,"You need to specify but the prefix and suffix of the game's code.","I'm not psychic");
				return;
			}
			if(playstationDirTxt.Text=="") {
				MessageBox.Show(this,"You need to specify the folder that contains the game's saves.","I'm not omniscient");
				return;
			}

            searchingForm searcher = new searchingForm(null, playstationDirTxt.Text, true);
			if(searcher.ShowDialog(this)!=DialogResult.Cancel) {
				reportForm report = new reportForm(searcher.output);
				if(report.ShowDialog(this)!=DialogResult.Cancel) {
				}
			}

        }

		private void gamePathButton_Click(object sender, EventArgs e)
		{
			gamePathBrowser.SelectedPath = gamePathText.Text;
			if(gamePathBrowser.ShowDialog(this)!=DialogResult.Cancel) {
				gamePathText.Text = gamePathBrowser.SelectedPath;
			}
		}

		private void saveFolderButton_Click(object sender, EventArgs e)
		{
			savePathBrowser.SelectedPath = saveFolderText.Text;
			if(savePathBrowser.ShowDialog(this)!=DialogResult.Cancel) {
				saveFolderText.Text = savePathBrowser.SelectedPath;
			}
		}

        private void button2_Click(object sender, EventArgs e)
        {
            savePathBrowser.SelectedPath = saveFolderText.Text;
            if (savePathBrowser.ShowDialog(this) != DialogResult.Cancel)
            {
                playstationDirTxt.Text = savePathBrowser.SelectedPath;
            }
        }

	}
}