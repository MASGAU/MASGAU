using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;

namespace Masgau
{
	public struct registry_key_value {
		public string key, value, data;
	}
	public struct shortcut {
		public string path, target, working;
	}

	public partial class searchingForm : Form
	{
		private bool stop = false;
		private string game_path, save_path;
		public string output;
		private Thread worker;
		const int CSIDL_COMMON_STARTMENU = 0x0016;
        private bool playstation_search;
        invokes invokes  = new invokes();

		[DllImport("shell32.dll")]
	    static extern bool SHGetSpecialFolderPath(
		IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);

		
		public searchingForm(string new_game_path, string new_save_path, bool search_playstation){
			InitializeComponent();
			game_path = new_game_path;
			save_path = new_save_path;
            playstation_search = search_playstation;
		}

		private void searchingForm_Shown(object sender, EventArgs e){
			output = "Operating System:" + Environment.NewLine;
			output += Environment.OSVersion.VersionString + Environment.NewLine;

            if (!playstation_search){
    			output += Environment.NewLine + "Install Path:" + Environment.NewLine + game_path + Environment.NewLine; 
            }
            output += Environment.NewLine + "Save Path:" + Environment.NewLine + save_path + Environment.NewLine + Environment.NewLine;
            if (playstation_search)
			    worker = new Thread(parseSaveFolder);
            else
                worker = new Thread(searchRegistry);
            worker.Start();
		}

		private void searchRegistry() {
			output += "Detected Registry Key Values:" + Environment.NewLine;
            invokes.setProgressBarValue(progressBar1,1);
            invokes.setControlText(groupBox1,"Scanning Local Machine Registry...");
			RegistryKey look_here = Registry.LocalMachine.OpenSubKey("SOFTWARE");
			registryTraveller(look_here);
            invokes.setProgressBarValue(progressBar1,2);
            invokes.setControlText(groupBox1,"Scanning Current User Registry...");
			look_here = Registry.CurrentUser.OpenSubKey("Software");
			registryTraveller(look_here);
			worker = new Thread(searchStartMenu);
			worker.Start();
		}

		private void registryTraveller(RegistryKey look_here) {
			registry_key_value value;

			foreach(string check_me in look_here.GetValueNames()) {
				value.key = look_here.Name;
				value.value = check_me;
				if(look_here.GetValue(check_me)!=null) {
					value.data = look_here.GetValue(check_me).ToString();
					if(value.data.Length>=game_path.Length&&game_path==value.data.Substring(0,game_path.Length)) {
						Console.WriteLine(value.key);
						output += value.key + "\\" + value.value + Environment.NewLine + value.data + Environment.NewLine;
					}
				}
			}
			RegistryKey sub_key;
			foreach(string check_me in look_here.GetSubKeyNames()) {
				try {
					sub_key = look_here.OpenSubKey(check_me);
					if(sub_key!=null)
						registryTraveller(sub_key);
				} catch(System.Security.SecurityException) {}
			}
		}

		private void searchStartMenu() {
			output += Environment.NewLine + "Detected Start Menu Shortcuts:" + Environment.NewLine;
            invokes.setProgressBarValue(progressBar1,3);
            invokes.setControlText(groupBox1,"Scanning Start Menu...");

			string start_menu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
			if(start_menu!=null&&Directory.Exists(start_menu))
				startMenuTraveller(start_menu);

	        StringBuilder temp_start_menu = new StringBuilder(260);
			SHGetSpecialFolderPath(IntPtr.Zero,temp_start_menu,CSIDL_COMMON_STARTMENU,false);
            start_menu = temp_start_menu.ToString();
			if(start_menu!=null&&Directory.Exists(start_menu))
				startMenuTraveller(start_menu);


			worker = new Thread(parseSaveFolder);
			worker.Start();
		}

		private void startMenuTraveller(string look_here) {
			IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut link;
            try
            {
                foreach (FileInfo shortcut in new DirectoryInfo(look_here).GetFiles("*.lnk"))
                {
                    link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcut.FullName);
                    if (link.TargetPath.Length >= game_path.Length && game_path == link.TargetPath.Substring(0, game_path.Length))
                    {
                        output += shortcut.FullName + Environment.NewLine + link.TargetPath + Environment.NewLine;
                    }
                }
                foreach (DirectoryInfo now_here in new DirectoryInfo(look_here).GetDirectories())
                {
                    startMenuTraveller(now_here.FullName);
                }
            }
            catch(DirectoryNotFoundException)
            {
                Console.WriteLine("Error while trying to work with " + look_here);
            }
			
		}

		private void parseSaveFolder() {
			output += Environment.NewLine + "Root Drive Information:" + Environment.NewLine;
            foreach(DriveInfo look_here in DriveInfo.GetDrives()) {
                if(look_here.Name==Path.GetPathRoot(save_path)) {
                    output += "Drive Name: " + look_here.Name + Environment.NewLine;
                    output += "Drive Root: " + look_here.RootDirectory + Environment.NewLine;
                    output += "Drive Format: " + look_here.DriveFormat + Environment.NewLine;
                    output += "Drive Type: " + look_here.DriveType + Environment.NewLine;
                    output += "Ready Status: " + look_here.IsReady + Environment.NewLine;
                }
            }
            output += Environment.NewLine + "Save Folder Dump:" + Environment.NewLine;
            invokes.setProgressBarValue(progressBar1,4);
            invokes.setControlText(groupBox1,"Dumping Save Folder...");
			travelSaveFolder(save_path);
            invokes.setProgressBarValue(progressBar1,5);

            if (!playstation_search){
                output += Environment.NewLine + "Install Folder Dump:" + Environment.NewLine;
                invokes.setControlText(groupBox1,"Dumping Install Folder...");
                travelSaveFolder(game_path);
                invokes.setProgressBarValue(progressBar1,6);
			    PathHandler paths = new PathHandler();
			    if(paths.uac) {
                    output += Environment.NewLine + "UAC Enabled" + Environment.NewLine + Environment.NewLine;
				    output += Environment.NewLine + "VirtualStore Folders:" + Environment.NewLine;
                    invokes.setControlText(groupBox1,"Dumping VirtualStore Folders...");
				    string virtual_path;
                    foreach(KeyValuePair<string,user_data> user in paths.users) {
				        Console.WriteLine(user.Value.virtual_store);
				        Console.WriteLine(user.Value.virtual_store);
				        virtual_path = Path.Combine(user.Value.virtual_store,game_path.Substring(3));
				        Console.WriteLine(virtual_path);
				        if(Directory.Exists(virtual_path))
					        travelSaveFolder(virtual_path);
                    }
			    } else {
                    output += Environment.NewLine + "UAC Disabled or not present" + Environment.NewLine + Environment.NewLine;
                }
            }
			this.DialogResult = DialogResult.OK;
		}

		private void travelSaveFolder(string look_here) {
            try {
			    foreach(FileInfo add_me in new DirectoryInfo(look_here).GetFiles()) {
				    output += add_me.FullName + " - " + add_me.Length + Environment.NewLine;
			    }

			    foreach(DirectoryInfo now_here in new DirectoryInfo(look_here).GetDirectories()) {
				    travelSaveFolder(now_here.FullName);
			    }
            } catch(UnauthorizedAccessException) {
                Console.WriteLine("Error while trying to access with " + look_here);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error while trying to work with " + look_here);
            }

		}


		private void searchingForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			worker.Abort();
		}
	}
}