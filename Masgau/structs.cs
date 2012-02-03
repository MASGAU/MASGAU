using System;
using System.Collections;
using System.IO;

namespace MASGAU {
    // These hold the location detection information loaded straight from the XML file
    public struct location_registry_holder {
        // Used when delaing with a registry key
        public string root, key, value;
        // Used to add or remove path elements
        public string append_path, detract_path;
        // Used to filter user locations by windows versions and language
        public string windows_version, language;
    }
    public struct location_shortcut_holder {
        // Used when dealing with a shortcut
        public string shortcut;
        // Used to add or remove path elements
        public string append_path, detract_path;
        // Used to filter user locations by windows versions and language
        public string windows_version, language;
    }
    public struct location_game_holder {
        // Used when dealing with a game root
        public string name;
        // Used to add or remove path elements
        public string append_path, detract_path;
        // Used to filter user locations by windows versions and language
        public string windows_version, language;
    }
    public struct location_path_holder {
        // Used when dealing with a path
        public string environment_variable, path;
        // Used to filter user locations by windows versions and language
        public string windows_version, language;
    }

    // This holds locations that have been found
    public struct location_holder {
        // ONLY holds the name of the environment variable or wahtever used to figure out the root
        public string rel_root;
        // Holds the actual, interpreted root location
        public string abs_root;
        // Holds only the relative path from the root
        public string path;
        // Holds the associated user for this folder
        public string owner;
        // Gets the full absolute path of the folfer
        public string getFullPath() {
            if(abs_root!=null&&abs_root!="") {
                if(path==null||path=="") {
                    return abs_root;
                } else {
                    return Path.Combine(abs_root,path);
                }
            } else {
                return null;
            }
        }
    }

    // This holds the save detection information loaded straight from the XML file
    public struct save_info_holder {
        public string path, name;
    }
    // This holds the ignore information loaded straight from the XML file
    public struct ignore_info_holder {
        public string path, name;
    }

    // This holds files that have been found
    public struct file_holder {
        // This is the already interpreted root folder
        public string root;
        // This is the path from that root folder to the file
        public string path;
        // This is the name of the file
        public string name;
        // Holds the associated user for the file
        public string owner;
        // Gets the entire folder name
        public string getFullDirPath() {
            if(root!=null&&root!="") {
                if(path!=null&&path!="") {
                    return Path.Combine(root,path);
                } else {
                    return root;
                }
            } else {
                return null;
            }
        }
        // Gets the full path, including file name
        public string getFullPath() {
            if(getFullDirPath()!=null) {
                if(name!=null&&name!="") {
                    return Path.Combine(getFullDirPath(),name);
                } else {
                    return getFullDirPath();
                }
            } else {
                return null;
            }
        }
    }

    public struct backup_holder
    {
        public string game_name, owner, file_name, file_date;
    }

    public struct user_holder {
	    public string name;
    }


    public struct playstation_id {
        public string prefix, suffix;
    }

    public struct monitor_file {
        public string root, path;
    }

    public struct monitor_dir {
	    public string path, game;
    }
}