using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.Threading;

namespace MASGAU
{
	public partial class analyzerForm : Form
	{
        PathHandler paths = new PathHandler();
        SteamHandler steam = new SteamHandler();
        bool config_changed = false;

		public analyzerForm()
		{
			InitializeComponent();
		}

        private void analyzerForm_Shown(object sender, EventArgs e)
        {

            if(paths.program_files_x86==null) {
                programFilexX86Button.Enabled = false;
            }
            if(!steam.installed) {
                steamappsButton.Enabled = false;
            }
            if (paths.xp) {
                publicUserButton.Enabled = false;
                virtualstoreButton.Enabled = false;
                savedGamesButton.Enabled = false;
            }
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
			searchingForm searcher = new searchingForm(gamePathText.Text,saveFolderText.Text,gameNameText.Text,false);
            searcher.ShowInTaskbar = true;
            this.Visible = false;
			if(searcher.ShowDialog(this)!=DialogResult.Cancel) {
				reportForm report = new reportForm(searcher.output,gameNameText.Text);
				if(report.ShowDialog(this)!=DialogResult.Cancel) {
				}
			}
            this.Visible = true;
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

            searchingForm searcher = new searchingForm(null, playstationDirTxt.Text, gameNameText.Text, true);
			if(searcher.ShowDialog(this)!=DialogResult.Cancel) {
				reportForm report = new reportForm(searcher.output + Environment.NewLine + "Playstation Code: " + prefixTxt.Text + "-" + suffixTxt.Text,gameNameText.Text);
				if(report.ShowDialog(this)!=DialogResult.Cancel) {
				}
			}

        }

        private string getPath(string path, string description, Environment.SpecialFolder root) {
            FolderBrowserDialog pathBrowser = new FolderBrowserDialog();
            pathBrowser.RootFolder = root;
            pathBrowser.SelectedPath = path;

            if(description==null)
                pathBrowser.Description = "Select Install Folder";
            else
                pathBrowser.Description = description;

            if (pathBrowser.ShowDialog(this) != DialogResult.Cancel) {
                return pathBrowser.SelectedPath;
            } else {
                return null;
            }
        }

        private void getGamePath(string path, Environment.SpecialFolder look_here) {
            string path_result = getPath(path,"Select Game Install Location",look_here);
            if (path_result!=null) {
                gamePathText.Text = path_result;
            }
        }

        private void getSavePath(string path, Environment.SpecialFolder look_here) {
            string path_result = getPath(path,"Select Save Location",look_here);
            if (path_result!=null) {
                saveFolderText.Text = path_result;
            }
        }

        private void gamePathButton_Click_1(object sender, EventArgs e)
        {
            getGamePath("",Environment.SpecialFolder.MyComputer);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(Environment.GetEnvironmentVariable("ProgramW6432")!=null) {
                getGamePath(paths.program_files,Environment.SpecialFolder.MyComputer);
            } else {
                getGamePath(paths.program_files,Environment.SpecialFolder.ProgramFiles);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            getGamePath(paths.program_files_x86,Environment.SpecialFolder.ProgramFilesX86);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            getGamePath(steam.path + "\\steamapps",Environment.SpecialFolder.MyComputer);
        }

        private void installFolderButton_Click(object sender, EventArgs e)
        {
			if(gamePathText.Text=="") {
				MessageBox.Show(this,"This button doesn't do anything if you don't specify an install path.","Come one, man");
			} else {
                getSavePath(gamePathText.Text,Environment.SpecialFolder.MyComputer);
            }
        }

        private void myDocumentsButton_Click(object sender, EventArgs e)
        {
            foreach(KeyValuePair<string,user_data> user in paths.users) {
                getSavePath(user.Value.user_documents,Environment.SpecialFolder.MyDocuments);
            }
        }

        private void savedGamesButton_Click(object sender, EventArgs e)
        {
            foreach(KeyValuePair<string,user_data> user in paths.users) {
                getSavePath(user.Value.saved_games,Environment.SpecialFolder.UserProfile);
            }
        }

        private void virtualstoreButton_Click(object sender, EventArgs e)
        {
            foreach(KeyValuePair<string,user_data> user in paths.users) {
                getSavePath(user.Value.virtual_store,Environment.SpecialFolder.LocalApplicationData);
            }
        }

        private void localAppdataButton_Click(object sender, EventArgs e)
        {
            foreach(KeyValuePair<string,user_data> user in paths.users) {
                getSavePath(user.Value.local_app_data,Environment.SpecialFolder.LocalApplicationData);
            }
        }

        private void roamingAppDataButton_Click(object sender, EventArgs e)
        {
            foreach(KeyValuePair<string,user_data> user in paths.users) {
                getSavePath(user.Value.app_data,Environment.SpecialFolder.ApplicationData);
            }
        }

        private void publicUserButton_Click(object sender, EventArgs e)
        {
            getSavePath(paths.public_profile,Environment.SpecialFolder.MyComputer);
        }

        private void allUsersButton_Click(object sender, EventArgs e)
        {
            getSavePath(paths.all_users_profile,Environment.SpecialFolder.MyComputer);
        }

        private void buttonCheck(object sender, EventArgs e) {
			if(gamePathText.Text=="") {
                installFolderButton.Enabled = false;
                submitButton.Enabled = false;
			} else {
			    if(saveFolderText.Text==""||gameNameText.Text=="") {
                    submitButton.Enabled = false;
			    } else {
                    submitButton.Enabled = true;
                }
                installFolderButton.Enabled = true;
            }

            if(prefixTxt.Text==""||suffixTxt.Text==""||playstationDirTxt.Text==""||gameNameText.Text=="") {
                scanPlaystationButton.Enabled=false;
            } else {
                scanPlaystationButton.Enabled=true;
            }

        }

        private void psSaveButton_Click(object sender, EventArgs e)
        {
            string path_result = getPath("","Select Playstation Save Location",Environment.SpecialFolder.MyComputer);
            if (path_result!=null) {
                playstationDirTxt.Text = path_result;
            }

        }

        private void emailText_TextChanged(object sender, EventArgs e)
        {
            config_changed = true;
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
        }

        private void analyzerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(config_changed) {
                emailSettings email = new emailSettings(this);
                email.email = emailText.Text;
                email.writeConfig();
                config_changed = false;
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            emailSettings email = new emailSettings(this);
            if(tabControl1.SelectedIndex==2) {
                emailText.Text = email.email;
            } else {
                if(config_changed) {
                    email.email = emailText.Text;
                    email.writeConfig();
                    config_changed = false;
                }
            }
        }

	}
}