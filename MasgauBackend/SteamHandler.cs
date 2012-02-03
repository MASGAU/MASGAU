using System;
using System.IO;
using System.Collections;

public class SteamHandler {
	public ArrayList users = new ArrayList();

    public string path;
    public bool installed;

	public SteamHandler() {
        RegistryManager steam = new RegistryManager("SOFTWARE\\Valve\\Steam");
        path = steam.getValue("InstallPath");

        if (Directory.Exists(path)) {
            if(File.Exists(path + "\\Steam.exe")) {
                installed  = true;
                if(Directory.Exists(path + "\\steamapps\\")) {
		            DirectoryInfo read_me = new DirectoryInfo(path + "\\steamapps\\");
		            DirectoryInfo[] read_us = read_me.GetDirectories();
                    foreach(DirectoryInfo subDir in read_us) {
    			        if(subDir.Name!="common"&&subDir.Name!="SourceMods"&&subDir.Name!="media") {
				            users.Add(subDir.Name);
			            }
                    }
                }
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
                if(Directory.Exists(force_me + "\\steamapps\\")) {
                    DirectoryInfo read_me = new DirectoryInfo(force_me + "\\steamapps\\");
                    DirectoryInfo[] read_us = read_me.GetDirectories();
                    foreach (DirectoryInfo subDir in read_us){
                        if (subDir.Name != "common" && subDir.Name != "SourceMods" && subDir.Name != "media"){
                            users.Add(subDir.Name);
                        }
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

    public ArrayList getSteamPaths(string get_me){
        file_holder add_me;
        add_me.file_name = null;
        add_me.owner = null;
        add_me.relative_path = get_me;
        ArrayList return_me = new ArrayList();
		if(installed) {
            if (get_me.StartsWith("%STEAMUSER%")){
				foreach(string user in users) {
                    add_me.owner = user;
                    add_me.relative_root = "%STEAMUSER%";
                    add_me.relative_path = get_me.Replace("%STEAMUSER%", path + "\\steamapps\\%STEAMUSER%");
                    add_me.absolute_root = path + "\\steamapps\\" + user;
                    add_me.absolute_path = get_me.Replace("%STEAMUSER%", path + "\\steamapps\\" + user);
                    if (Directory.Exists(add_me.absolute_path)) {
                        return_me.Add(add_me);
                    }
				}
            }
            else if (get_me.StartsWith("%STEAMCOMMON%")) {
                add_me.absolute_path = get_me.Replace("%STEAMCOMMON%", path + "\\steamapps\\common");
                add_me.relative_root = "%STEAMCOMMON%";
                add_me.absolute_root = path + "\\steamapps\\common";
                if (Directory.Exists(add_me.absolute_path))
                {
                    return_me.Add(add_me);
                }
			}
		}
        return return_me;
    }
}
