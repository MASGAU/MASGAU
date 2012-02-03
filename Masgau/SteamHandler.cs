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
            if(File.Exists(Path.Combine(path,"Steam.exe"))) {
                installed  = true;
                if(Directory.Exists(Path.Combine(path,"steamapps"))) {
		            DirectoryInfo read_me = new DirectoryInfo(Path.Combine(path,"steamapps"));
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
                    add_me.relative_path = Path.Combine(Path.Combine(path, Path.Combine("steamapps", "%STEAMUSER%")), get_me.Substring(12));
                    add_me.absolute_root = Path.Combine(path, Path.Combine("steamapps", user));
                    add_me.absolute_path = Path.Combine(Path.Combine(Path.Combine(path, "steamapps"),user), get_me.Substring(12));
                    if (Directory.Exists(add_me.absolute_path)) {
                        return_me.Add(add_me);
                    }
				}
            }
            else if (get_me.StartsWith("%STEAMCOMMON%")) {
                add_me.absolute_path = Path.Combine(Path.Combine(path,Path.Combine("steamapps","common")),get_me.Substring(14));
                add_me.relative_root = "%STEAMCOMMON%";
                add_me.absolute_root = Path.Combine(path,Path.Combine("steamapps","common"));
                if (Directory.Exists(add_me.absolute_path))
                {
                    return_me.Add(add_me);
                }
			}
		}
        return return_me;
    }

	public string parsePath(string parse_me) {
		string return_me = null;
		if (parse_me.StartsWith("%STEAMUSER%")){
            return_me = Path.Combine(Path.Combine(path, Path.Combine("steamapps", "%STEAMUSER%")), parse_me.Substring(12));
        } else if (parse_me.StartsWith("%STEAMCOMMON%")) {
            return_me = Path.Combine(Path.Combine(path, Path.Combine("steamapps", "common")), parse_me.Substring(14));
		}
		return return_me;
	}
}
