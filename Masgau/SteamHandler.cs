using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace MASGAU {
    public class SteamHandler {
	    public ArrayList users = new ArrayList();
        public ArrayList userdatas = new ArrayList();

        public string path, common_path, source_mods_path;
        public bool installed;

	    public SteamHandler() {
            RegistryManager steam = new RegistryManager(Microsoft.Win32.RegistryHive.LocalMachine,"SOFTWARE\\Valve\\Steam",false);
            path = steam.getValue("InstallPath");

            if (Directory.Exists(path)) {
                if(File.Exists(Path.Combine(path,"Steam.exe"))) {
                    installed  = true;
	                DirectoryInfo read_me = new DirectoryInfo(Path.Combine(path,"steamapps"));
	                DirectoryInfo[] read_us = read_me.GetDirectories();
                    foreach(DirectoryInfo subDir in read_us) {
			            if(subDir.Name.ToLower()!="common"&&subDir.Name.ToLower()!="sourcemods"&&subDir.Name.ToLower()!="media") {
			                users.Add(subDir.Name);
		                }
                    }
                    //if(Directory.Exists(Path.Combine(path,"steamapps"))) {
                    //    read_me = new DirectoryInfo(Path.Combine(path,"steamapps"));
                    //    read_us = read_me.GetDirectories();
                    //    foreach(DirectoryInfo subDir in read_us) {
                    //        if(subDir.Name!="common"&&subDir.Name!="SourceMods"&&subDir.Name!="media") {
                    //            users.Add(subDir.Name);
                    //        }
                    //    }
                    //}
                    if(Directory.Exists(Path.Combine(path,"userdata"))) {
		                read_me = new DirectoryInfo(Path.Combine(path,"userdata"));
		                read_us = read_me.GetDirectories();
                        foreach(DirectoryInfo subDir in read_us) {
				            userdatas.Add(subDir.Name);
                        }
                    }
                    common_path = Path.Combine(path,Path.Combine("steamapps","common"));
                    source_mods_path = Path.Combine(path,Path.Combine("steamapps","SourceMods"));
                } else {
                    path= null;
                }
	        } else{
                path = null;
            }

        }

        public SteamHandler(string force_me) {
            if(Directory.Exists(force_me)) {
                if(File.Exists(force_me + "\\Steam.exe\\")) {
                    installed = true;
                    path = force_me;
                    DirectoryInfo read_me;
                    DirectoryInfo[] read_us;
                    if(Directory.Exists(force_me + "\\steamapps\\")) {
                        read_me = new DirectoryInfo(force_me + "\\steamapps\\");
                        read_us = read_me.GetDirectories();
                        foreach (DirectoryInfo subDir in read_us){
                            if (subDir.Name != "common" && subDir.Name != "SourceMods" && subDir.Name != "media"){
                                users.Add(subDir.Name);
                            }
                        }
                    }
                    if(Directory.Exists(Path.Combine(force_me,"userdata"))) {
		                read_me = new DirectoryInfo(Path.Combine(path,"userdata"));
		                read_us = read_me.GetDirectories();
                        foreach(DirectoryInfo subDir in read_us) {
				            userdatas.Add(subDir.Name);
                        }
                    }
                } else {
                    installed = false;
                    path = null;
                }
            } else {
                installed = false;
                path = null;
            }
        }

        public ArrayList getSteamPaths(location_path_holder get_me){
            location_holder add_me;
            add_me.owner = null;
            add_me.path = get_me.path;
            add_me.abs_root = null;
            add_me.rel_root = get_me.environment_variable;
            ArrayList return_me = new ArrayList();
		    if(installed) {
                switch(get_me.environment_variable) {
                    case "steamuser":
				        foreach(string user in users) {
                            add_me.owner = user;
                            add_me.abs_root = Path.Combine(path, Path.Combine("steamapps", user));
                            if (Directory.Exists(Path.Combine(add_me.abs_root,add_me.path))) {
                                return_me.Add(add_me);
                            }
				        }
                        break;
                    case "steamuserdata":
				        foreach(string userdata in userdatas) {
                            add_me.owner = userdata;
                            add_me.abs_root = Path.Combine(path, Path.Combine("userdata", userdata));
                            if (Directory.Exists(Path.Combine(add_me.abs_root,add_me.path))) {
                                return_me.Add(add_me);
                            }
				        }
                        break;
                    case "steamcommon": 
                        add_me.abs_root = Path.Combine(path,Path.Combine("steamapps","common"));
                        if (Directory.Exists(Path.Combine(add_me.abs_root,add_me.path))) {
                            return_me.Add(add_me);
                        }
                        break;
			        case "steamsourcemods":
                        add_me.abs_root = Path.Combine(path,Path.Combine("steamapps","SourceMods"));
                        if (Directory.Exists(Path.Combine(add_me.abs_root,add_me.path))) {
                            return_me.Add(add_me);
                        }
                        break;
			        default:
			            MessageBox.Show("The specified environment variable " + get_me.environment_variable + " is not recognized." + Environment.NewLine + "You either spelled it wrong or something.","Outta steaaaam",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        break;
                }
            }
            return return_me;
        }

        public string getAbsoluteRoot(location_holder parse_me, string user) {
            string return_me = null;
            if (parse_me.rel_root =="steamuser"&&user!=null&&users.Contains(user)){
                return_me = Path.Combine(path, Path.Combine("steamapps", user));
            } else if (parse_me.rel_root =="steamuserdata"&&user!=null&&userdatas.Contains(user)){
                return_me = Path.Combine(path, Path.Combine("userdata", user));
            } else if (parse_me.rel_root=="steamcommon") {
                return_me = Path.Combine(path, Path.Combine("steamapps", "common"));
            } else if (parse_me.rel_root=="steamsourcemods") {
                return_me = Path.Combine(path, Path.Combine("steamapps", "SourceMods"));
            }
            return return_me;
        }
    }
}