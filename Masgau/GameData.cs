using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MASGAU {
    public class GameData {
	    public string title, identifier_file = null, identifier_path = null;
        public bool override_virtualstore = false, disabled = false, detection_required = false;
        
        // Thse are the location datas loaded from the xml profile
        public ArrayList location_paths = new ArrayList();
        public ArrayList location_shortcuts = new ArrayList();
        public ArrayList location_games = new ArrayList();
        public ArrayList location_registrys = new ArrayList();

        // These are the save and ignore datas loaded from the xml profile
        public ArrayList saves = new ArrayList();
        public ArrayList ignores = new ArrayList();

        // These are the locations that have been found
        public Dictionary<string,location_holder> detected_locations;

        // These are playstation identifiers loaded from the Xml profiles
        public ArrayList psp_ids = new ArrayList();
        public ArrayList ps1_ids = new ArrayList();
        public ArrayList ps2_ids = new ArrayList();
        public ArrayList ps3_ids = new ArrayList();

        // This loads the names of the helpful people who contributed the paths to MASGAU
        public ArrayList contributors;

        // This holds the name of the system the save is for
        public string platform = "Windows";

        // This is something...
        const int CSIDL_COMMON_STARTMENU = 0x0016;

        // These hold data for filtering out something or other.
	    private string windows_version;
        //private string language;

        [DllImport("shell32.dll")]
        static extern bool SHGetSpecialFolderPath(
        IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);

	    public GameData(string load_me, PathHandler paths, SteamHandler steam, playstationHandler playstation, Dictionary<string, string> game_profiles, string new_platform) {
            windows_version = new_platform;
            ArrayList add_us = new ArrayList();
            contributors = new ArrayList();

            // Setting up the XMl reader
            XmlReaderSettings xml_settings = new XmlReaderSettings();
            xml_settings.ConformanceLevel = ConformanceLevel.Fragment;
            xml_settings.IgnoreComments = true;
            xml_settings.IgnoreWhitespace = true;
            XmlReader read_me = XmlReader.Create(new StringReader(load_me),xml_settings);

            // The fresh location holders
            location_game_holder new_game_location;
            location_path_holder new_path_location;
            location_registry_holder new_registry_location;
            location_shortcut_holder new_shortcut_location;

            // A fresh playstation id holder
            playstation_id new_id;
            new_id.prefix = null;
            new_id.suffix = null;

            // Saves and ignores
		    save_info_holder new_save;
            ignore_info_holder new_ignore;

            try {
		        while(read_me.Read()) {
                    // Blanking out the new game location
                    new_game_location.append_path = null;
                    new_game_location.detract_path = null;
                    new_game_location.name = null;
                    new_game_location.language = null;
                    new_game_location.windows_version = null;

                    // Blanking out the new path location
                    new_path_location.environment_variable = null;
                    new_path_location.language = null;
                    new_path_location.path = null;
                    new_path_location.windows_version = null;

                    // Blanking out the new registry location
                    new_registry_location.append_path = null;
                    new_registry_location.detract_path = null;
                    new_registry_location.language = null;
                    new_registry_location.value = null;
                    new_registry_location.key = null;
                    new_registry_location.root = null;
                    new_registry_location.windows_version = null;
                    
                    // Blanking out the new shortcut location
                    new_shortcut_location.append_path = null;
                    new_shortcut_location.detract_path = null;
                    new_shortcut_location.language = null;
                    new_shortcut_location.shortcut = null;
                    new_shortcut_location.windows_version = null;

                    // Blank out the saves and ignores
                    new_save.name = null;
                    new_save.path = null;
                    new_ignore.name = null;
                    new_ignore.path = null;

			        if(read_me.NodeType==XmlNodeType.Element) {
                        switch (read_me.Name) {
					        case "title":
						        title = read_me.ReadString();
						        break;
                            case "contributer":
                                contributors.Add(read_me.ReadString());
                                break;
					        case "require_detection":
						        detection_required = true;
						        break;
                            case "location_registry":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "root":
                                            new_registry_location.root = read_me.Value;
                                            break;
                                        case "key":
                                            new_registry_location.key = read_me.Value;
                                            break;
                                        case "value":
                                            new_registry_location.value = read_me.Value;
                                            break;
                                        case "append":
                                            new_registry_location.append_path= read_me.Value;
                                            break;
                                        case "detract":
                                            new_registry_location.detract_path= read_me.Value;
                                            break;
                                        case "windows_version":
                                            new_registry_location.windows_version= read_me.Value;
                                            break;
                                        case "language":
                                            new_registry_location.language= read_me.Value;
                                            break;
                                    }
                                }
                                location_registrys.Add(new_registry_location);
                                break;
                            case "location_shortcut":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "shortcut":
                                            new_shortcut_location.shortcut=read_me.Value;
                                            break;
                                        case "append":
                                            new_shortcut_location.append_path= read_me.Value;
                                            break;
                                        case "detract":
                                            new_shortcut_location.detract_path= read_me.Value;
                                            break;
                                        case "windows_version":
                                            new_shortcut_location.windows_version= read_me.Value;
                                            break;
                                        case "language":
                                            new_shortcut_location.language= read_me.Value;
                                            break;
                                    }
                                }
                                location_shortcuts.Add(new_shortcut_location);
                                break;
                            case "location_game":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "name":
                                            new_game_location.name = read_me.Value;
                                            break;
                                        case "append":
                                            new_game_location.append_path= read_me.Value;
                                            break;
                                        case "detract":
                                            new_game_location.detract_path= read_me.Value;
                                            break;
                                        case "windows_version":
                                            new_game_location.windows_version= read_me.Value;
                                            break;
                                        case "language":
                                            new_game_location.language= read_me.Value;
                                            break;
                                    }
                                }
                                location_games.Add(new_game_location);
                                break;
                            case "location_path":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "environment_variable":
                                            new_path_location.environment_variable = read_me.Value;
                                            break;
                                        case "path":
                                            new_path_location.path = read_me.Value;
                                            break;
                                        case "windows_version":
                                            new_path_location.windows_version= read_me.Value;
                                            break;
                                        case "language":
                                            new_path_location.windows_version= read_me.Value;
                                            break;
                                    }
                                }
                                location_paths.Add(new_path_location);
                                break;
                            case "save":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "path":
                                            new_save.path = read_me.Value;
                                            break;
                                        case "filename":
                                            new_save.name = read_me.Value;
                                            break;
                                    }
                                }
                                saves.Add(new_save);
                                break;
                            case "ignore":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "path":
                                            new_ignore.path = read_me.Value;
                                            break;
                                        case "filename":
                                            new_ignore.name = read_me.Value;
                                            break;
                                    }
                                }
                                ignores.Add(new_ignore);
                                break;
                            case "virtualstore":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "override":
                                            if(read_me.Value=="yes")
                                                override_virtualstore = true;
                                            break;
                                    }
                                }
                                break;
                            case "identifier":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "path":
                                            identifier_path = read_me.Value;
                                            break;
                                        case "filename":
                                            identifier_file = read_me.Value;
                                            break;
                                    }
                                }
                                break;
                            case "psp":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "prefix":
                                            new_id.prefix = read_me.Value;
                                            break;
                                        case "suffix":
                                            new_id.suffix = read_me.Value;
                                            break;
                                    }
                                }
                                platform = "PSP";
                                psp_ids.Add(new_id);
                                break;
                            case "ps3":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "prefix":
                                            new_id.prefix = read_me.Value;
                                            break;
                                        case "suffix":
                                            new_id.suffix = read_me.Value;
                                            break;
                                    }
                                }
                                platform = "PS3";
                                ps3_ids.Add(new_id);
                                break;
                            case "ps2":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "prefix":
                                            new_id.prefix = read_me.Value;
                                            break;
                                        case "suffix":
                                            new_id.suffix = read_me.Value;
                                            break;
                                    }
                                }
                                platform = "PS2";
                                ps2_ids.Add(new_id);
                                break;
                            case "ps1":
                                while(read_me.MoveToNextAttribute()) {
                                    switch(read_me.Name) {
                                        case "prefix":
                                            new_id.prefix = read_me.Value;
                                            break;
                                        case "suffix":
                                            new_id.suffix = read_me.Value;
                                            break;
                                    }
                                }
                                platform = "PS1";
                                ps1_ids.Add(new_id);
                                break;
				        }
			        }
		        }
            } catch (XmlException) {
                MessageBox.Show("Something went wrong with parsing a game profile. Weird.","Like Al",MessageBoxButtons.OK,MessageBoxIcon.Error);
                location_paths.Clear();
                location_games.Clone();
                location_registrys.Clear();
                location_shortcuts.Clear();
                return;
            }
            read_me.Close();

            detect(paths,steam, playstation,game_profiles);
	    }

        private string modifyPath(string path, string detract, string append) {
            path = path.TrimEnd(Path.DirectorySeparatorChar);
            if (detract!= null) {
                if(path.EndsWith(detract))
                    path = path.Substring(0,path.Length-detract.Length);
            }
            if (append != null)
                path = Path.Combine(path,append);

            return path.Trim('\\');
        }

        public void detect(PathHandler paths, SteamHandler steam, playstationHandler playstation, Dictionary<string, string> game_profiles) {
            ArrayList interim = new ArrayList();
            GameData parent_game;

            RegistryHive hive;
            RegistryManager reg;

            string path = null;
            FileInfo the_shortcut;
            StringBuilder start_menu;

            // This checks all the registry locations
            foreach(location_registry_holder location in location_registrys) {
			    if(location.windows_version==null||location.windows_version==windows_version) {
				    // This handles if the root is a registry key
				    if(location.key!=null&&location.root!=null) {
					    switch(location.root) {
                            case "classes_root":
                                hive = RegistryHive.ClassesRoot;
                                break;
                            case "current_user":
							    hive = RegistryHive.CurrentUser;
							    break;
                            case "current_config":
							    hive = RegistryHive.CurrentConfig;
							    break;
                            case "dyn_data":
							    hive = RegistryHive.DynData;
							    break;
                            case "local_machine":
							    hive = RegistryHive.LocalMachine;
							    break;
                            case "performance_data":
							    hive = RegistryHive.PerformanceData;
							    break;
                            case "users":
							    hive = RegistryHive.Users;
							    break;
                            default:
						        MessageBox.Show("The specified key root in " + location.root + " is not recognized. You either spelled it wrong or something.","The root of all keys",MessageBoxButtons.OK,MessageBoxIcon.Error);
							    continue;
					    } 
                        reg = new RegistryManager(hive,location.key,false);

					    if(reg.key_found) {
						    try {
                                if(location.value==null)
							        path = reg.getValue("");
                                else
							        path = reg.getValue(location.value);

							    if(path!=null){
								    path = modifyPath(path,location.detract_path,location.append_path);
                                    if(Directory.Exists(path)) {
								        interim.AddRange(interpretPath(new DirectoryInfo(path).FullName, steam, paths));
                                    }
							    }
						    } catch {
						        MessageBox.Show("The specified value " + location.value + " is not found in this key:" + Environment.NewLine + location.key + Environment.NewLine + "In this key root:" + Environment.NewLine + location.root + Environment.NewLine + "You either spelled it wrong or something.","I am the keymaster!",MessageBoxButtons.OK,MessageBoxIcon.Error);
						    }
					    }
				    }
                }
            }

            //This checks all the shortcuts
            foreach(location_shortcut_holder location in location_shortcuts) {
			    if(location.windows_version==null||location.windows_version==windows_version) {
				    // This handles if the root is a shortcut
				    if(location.shortcut!=null) {
					    the_shortcut = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),location.shortcut));
					    if(!the_shortcut.Exists) {
						    start_menu = new StringBuilder(260);
						    SHGetSpecialFolderPath(IntPtr.Zero,start_menu,CSIDL_COMMON_STARTMENU,false);
						    the_shortcut = new FileInfo(Path.Combine(start_menu.ToString(),location.shortcut));
					    }

					    if(the_shortcut.Exists) {
						    IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
						    IWshRuntimeLibrary.IWshShortcut link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(the_shortcut.FullName);

						    //split = link.TargetPath.Split(Path.DirectorySeparatorChar);
						    //path = split[0] + Path.DirectorySeparatorChar;
						    //for(int i=1;i<split.Length-1;i++)
						    //    path =  Path.Combine(path,split[i]);
                            try
                            {
                                path = Path.GetDirectoryName(link.TargetPath);
                                path = modifyPath(path, location.detract_path, location.append_path);
                                interim.AddRange(interpretPath(path, steam, paths));
                            }
                            catch { }
					    }
				    }
                }
            }

            // This checks all the locations that are based on other games
            foreach(location_game_holder location in location_games) {
			    if(location.windows_version==null||location.windows_version==windows_version) {
				    // This handles if the root is another game
				    if(location.name!=null) {
					    if(game_profiles.ContainsKey(location.name)){
						    parent_game = new GameData(game_profiles[location.name],paths,steam,playstation,game_profiles,platform);
						    foreach(KeyValuePair<string,location_holder> check_me in parent_game.detected_locations) {
							    path = modifyPath(check_me.Value.getFullPath(),location.detract_path,location.append_path);
							    interim.AddRange(interpretPath(path, steam, paths));
						    }
					    } else {
						    MessageBox.Show("The specified parent game " + location.name + " for " + title + " is not present in the profiles xml. You either spelled it wrong, or this is a chain effect from another error.","Wasting my gorramn time",MessageBoxButtons.OK,MessageBoxIcon.Error);
					    }
				    }
                }
            }

            // This parses each location supplied by the XML file
            foreach (location_path_holder location in location_paths) {
			    // Checks if the path is for a specific windows version, and if we're currently running said version
			    if(location.windows_version==null||location.windows_version==windows_version) {
				    // This handles if the root is a relative path
				    if(location.path!=null&&location.environment_variable!=null) {
					    //path = modifyPath(location.path,location.detract_path,location.append_path);
					    if (location.environment_variable.StartsWith("steam")){
						    interim.AddRange(steam.getSteamPaths(location));
					    } else {
						    interim.AddRange(paths.getPath(location, override_virtualstore));
					    }
				    }


			    }
		    }

		    location_holder playstation_location;
		    playstation_location.owner = null;
		    playstation_location.abs_root= null;
		    playstation_location.rel_root= "drive";
            playstation_location.path = null;
		    string found_id;

		    save_info_holder add_me;
		    add_me.name = null;
		    add_me.path = null;


		    // This parses psp games
		    if(psp_ids.Count>0) {
			    if(playstation.psp_saves!=null) {
				    foreach(playstation_id psp_id in psp_ids) {
					    found_id = playstation.detectPSPGame(psp_id);
					    if(found_id!=null) {
						    add_me.path = Path.Combine(playstation.psp_saves.Substring(3),found_id+"*");
						    saves.Add(add_me);
                        
						    playstation_location.abs_root = Path.GetPathRoot(playstation.psp_saves);
						    interim.Add(playstation_location);
					    }
				    }
			    }
		    }

		    // This parses ps3 games
		    if(ps3_ids.Count>0) {
			    if(playstation.ps3_saves!=null) {
				    foreach(playstation_id ps3_id in ps3_ids) {
					    found_id = playstation.detectPS3Game(ps3_id);
					    if(found_id!=null) {
						    add_me.path= Path.Combine(playstation.ps3_saves.Substring(3),found_id+"*");
						    saves.Add(add_me);

						    playstation_location.abs_root = Path.GetPathRoot(playstation.ps3_saves);
						    interim.Add(playstation_location);
					    }
				    }
			    }
		    }

		    // This parses ps2 games
		    if(ps2_ids.Count>0) {
			    if(playstation.ps3_export!=null) {
				    foreach(playstation_id ps2_id in ps2_ids) {
					    found_id = playstation.detectPS2Game(ps2_id);
					    if(found_id!=null) {
						    add_me.name = "BA" + found_id + "*";
						    add_me.path= playstation.ps3_export.Substring(3);
						    saves.Add(add_me);

						    playstation_location.abs_root = Path.GetPathRoot(playstation.ps3_export);
						    interim.Add(playstation_location);
					    }
				    }
			    }
		    }

        
		    // This parses ps1 games
		    if(ps1_ids.Count>0) {
			    foreach(playstation_id ps1_id in ps1_ids) {
				    if(playstation.ps3_export!=null) {
					    found_id = playstation.detectPS1PS3Game(ps1_id);
					    if(found_id!=null) {
						    add_me.name = "BA" + found_id + "*";
						    add_me.path= playstation.ps3_export.Substring(3);
						    saves.Add(add_me);

						    playstation_location.abs_root = Path.GetPathRoot(playstation.ps3_export);
						    interim.Add(playstation_location);
					    }
				    }
				    if(playstation.psp_saves!=null) {
					    found_id = playstation.detectPS1PSPGame(ps1_id);
					    if(found_id!=null) {
						    add_me.name = null;
						    add_me.path = Path.Combine(playstation.psp_saves.Substring(3),found_id + "*");
						    saves.Add(add_me);

						    playstation_location.abs_root = Path.GetPathRoot(playstation.psp_saves);
						    interim.Add(playstation_location);
					    }
				    }
			    }
		    }

		    detected_locations = new Dictionary<string,location_holder>();
            foreach (location_holder check_me in interim) {
                if(!detected_locations.ContainsKey(check_me.getFullPath().ToLower())) {
                    if(identifier_file!=null&&identifier_path!=null) {
                        foreach(DirectoryInfo directory in getPaths(check_me.getFullPath(),identifier_path)) {
                            if (directory.GetFiles(identifier_file).Length>0) {
                                detected_locations.Add(check_me.getFullPath().ToLower(),check_me);
                                break;
                            }
                        }
                    } else if(identifier_path!=null) {
                        if(getPaths(check_me.getFullPath(),identifier_path).Count>0) {
                            detected_locations.Add(check_me.getFullPath().ToLower(),check_me);
                        }
                    } else if(identifier_file!=null) {
                        if(new DirectoryInfo(check_me.getFullPath()).GetFiles(identifier_file).Length>0) {
                            detected_locations.Add(check_me.getFullPath().ToLower(),check_me);
                        }
                    } else {
                        detected_locations.Add(check_me.getFullPath().ToLower(),check_me);
                    }
                }
		    }
        }

        public ArrayList interpretPath(string interpret_me, SteamHandler steam, PathHandler paths) {
            ArrayList return_me = new ArrayList();
            
            location_path_holder new_location;
            // Only these are used by this
            new_location.environment_variable = null;
            new_location.path = null;
            // Not these, but they need to be nulled out
            new_location.windows_version = null;
            new_location.language = null;


            if(steam.installed&&interpret_me.ToLower().StartsWith(steam.common_path.ToLower())) {
                new_location.environment_variable = "steamcommon";
                new_location.path = interpret_me.Substring(steam.common_path.Length+1);
                return_me.AddRange(steam.getSteamPaths(new_location));
            } else if(paths.program_files_x86!=null&&interpret_me.ToLower().StartsWith(paths.program_files_x86.ToLower())) {
                new_location.environment_variable = "programfiles";
                new_location.path = interpret_me.Substring(paths.program_files_x86.Length+1);
                return_me.AddRange(paths.getPath(new_location,override_virtualstore));
            } else if(paths.program_files!=null&&interpret_me.ToLower().StartsWith(paths.program_files.ToLower())) {
                new_location.environment_variable = "programfiles";
                new_location.path = interpret_me.Substring(paths.program_files.Length+1);
                return_me.AddRange(paths.getPath(new_location,override_virtualstore));
            } else if (Directory.Exists(interpret_me)) {
                foreach(KeyValuePair<string,user_data> check_me in paths.users) {
                    if(check_me.Value.user_dir!=null&&interpret_me.ToLower().StartsWith(check_me.Value.user_dir.ToLower())) {
                        new_location.environment_variable = "userprofile";
                        new_location.path = interpret_me.Substring(check_me.Value.user_dir.Length+1);
                        return_me.AddRange(paths.getPath(new_location,override_virtualstore));
                    }
                }
                if(return_me.Count==0) 
                    new_location.environment_variable = "drive";
                    new_location.path = interpret_me.Substring(3);
                    return_me.AddRange(paths.getPath(new_location,override_virtualstore));
            }
            return return_me;
        }

        private ArrayList getPaths(string root, string path) {
            ArrayList return_me = new ArrayList();
            DirectoryInfo root_directory = new DirectoryInfo(root);

            string[] split = path.Split(Path.DirectorySeparatorChar);
            string forward_me = "";

            for(int i=1;i<split.Length;i++) {
                forward_me = Path.Combine(forward_me,split[i]);
            }

            DirectoryInfo[] directories = root_directory.GetDirectories(split[0]);
            if(split.Length==1) {
                foreach(DirectoryInfo add_me in directories) {
                    return_me.Add(add_me);
                }
            } else {
                foreach(DirectoryInfo add_me in directories) {
                    return_me.AddRange(getPaths(add_me.FullName,forward_me));
                }
            }
            return return_me;
        }

        private ArrayList gatherFiles(string root) {
            ArrayList return_me = new ArrayList();
            try {
                foreach(DirectoryInfo sub_folder in new DirectoryInfo(root).GetDirectories()) {
                    return_me.AddRange(gatherFiles(sub_folder.FullName));
                }
                foreach(FileInfo file in new DirectoryInfo(root).GetFiles()) {
                    return_me.Add(file.FullName);
                }
            } catch {
            }
            return return_me;
        }

        public ArrayList getSaves() {
            ArrayList return_me = new ArrayList();
            ArrayList add_us = new ArrayList();
            ArrayList remove_us = new ArrayList();

            foreach(KeyValuePair<string,location_holder> location in detected_locations) {
                foreach (save_info_holder save in saves) {
                    add_us.AddRange(findTheseFiles(save.path,save.name,location.Value.getFullPath(),location.Value.owner));
                }
                foreach(ignore_info_holder criteria in ignores) {
                    remove_us.AddRange(findTheseFiles(criteria.path,criteria.name,location.Value.getFullPath(),location.Value.owner));
                }
            }
            

            foreach(file_holder add_me in add_us) {
                if(!return_me.Contains(add_me)) {
                    return_me.Add(add_me);
                }
            }


            foreach(file_holder remove_me in remove_us) {
                if(return_me.Contains(remove_me)) {
                    return_me.Remove(remove_me);
                }
            }

            return return_me;
        }

        private ArrayList findTheseFiles(string path, string name, string root, string owner) {
		    ArrayList directories;
            ArrayList files;
            ArrayList return_me = new ArrayList();
            file_holder add_me;
            add_me.root = root;
            add_me.owner = owner;
			directories = new ArrayList();
            files = new ArrayList();
            if(name==null) {  
				if(path==null) {
					if(Directory.Exists(add_me.root)) {
                        files.AddRange(gatherFiles(add_me.root));
                        foreach(string file_name in files) {
                            if(Path.GetDirectoryName(file_name).Length==add_me.root.Length)
                                add_me.path = "";
                            else 
                                add_me.path= Path.GetDirectoryName(file_name).Substring(add_me.root.Trim(Path.DirectorySeparatorChar).Length+1);
                            add_me.name = Path.GetFileName(file_name);
                            return_me.Add(add_me);
                        }
					}
				} else {
					directories.AddRange(getPaths(add_me.root,path));
					foreach(DirectoryInfo directory in directories) {
                        files = new ArrayList();
                        files.AddRange(gatherFiles(directory.FullName));
                        foreach(string file_name in files) {
                            add_me.path = Path.GetDirectoryName(file_name).Substring(add_me.root.Trim(Path.DirectorySeparatorChar).Length + 1);
                            add_me.name = Path.GetFileName(file_name);
                            return_me.Add(add_me);
                        }
					}
				}
            } else if(path==null) {
                if(Directory.Exists(add_me.root)) {
                    foreach(FileInfo read_me in new DirectoryInfo(add_me.root).GetFiles(name)) {
                        if (read_me.DirectoryName.Length == add_me.root.Length)
                            add_me.path = "";
                        else
                            add_me.path = read_me.DirectoryName.Substring(add_me.root.Trim(Path.DirectorySeparatorChar).Length + 1);
                        add_me.name = read_me.Name;
                        return_me.Add(add_me);
                    }
                }
            } else {
				directories.AddRange(getPaths(add_me.root,path));
				foreach(DirectoryInfo directory in directories) {
                    foreach(FileInfo read_me in directory.GetFiles(name)) {
                        add_me.path = read_me.DirectoryName.Substring(add_me.root.Trim(Path.DirectorySeparatorChar).Length + 1);
                        add_me.name = read_me.Name;
                        return_me.Add(add_me);
                    }
                }
            } 
            return return_me;
        }


        //public location_holder getPath() {
        //    location_holder return_me;
        //    return_me.absolute_path = null;
        //    return_me.relative_path = null;
        //    return_me.absolute_root = null;
        //    return_me.file_name = null;
        //    return_me.owner = null;
        //    return_me.relative_root = null;
        //    if (detected_locations.Count != 0)
        //    {
        //        ArrayList from_me = new ArrayList();
        //        from_me.AddRange(getSaves());
        //        if(from_me.Count!=0) {
        //            file_holder example = ((file_holder)from_me[0]);
        //            return_me = example;
        //        } 
        //    }
        //    return return_me;
        //}

        public file_holder checkThis(string this_right_here) {
            file_holder return_me;
            return_me.name = null;
            return_me.owner= null;
            return_me.path= null;
            return_me.root= null;
            string compare;
            foreach(file_holder check_me in getSaves()) {
                compare = check_me.getFullPath();
                if(compare==this_right_here)
                    return_me=check_me;
            }
            return return_me;
        }

							    // Maybe someday...

	    //public void addManualPath(string path, SteamHandler steam, PathHandler paths) {
	    //    ArrayList add_us = interpretPath
	    //}
	    //public void removeManualPath() {

	    //}
    }
}