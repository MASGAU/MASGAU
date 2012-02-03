using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Windows.Forms;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

public class GameData {
	public string title, identifier_file = null, identifier_path = null;
    public bool override_virtualstore = false, disabled = false;
    public ArrayList roots = new ArrayList();
    public ArrayList saves = new ArrayList();
    public ArrayList detected_roots;
    public ArrayList psp_ids = new ArrayList();
    public ArrayList ps1_ids = new ArrayList();
    public ArrayList ps2_ids = new ArrayList();
    public ArrayList ps3_ids = new ArrayList();
    public string platform = "Windows";
    const int CSIDL_COMMON_STARTMENU = 0x0016; 

    [DllImport("shell32.dll")]
    static extern bool SHGetSpecialFolderPath(
    IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);

	public GameData(string load_me, PathHandler paths, SteamHandler steam, playstationHandler playstation) {
        file_holder add_me;
        ArrayList add_us = new ArrayList();
        XmlReaderSettings xml_settings = new XmlReaderSettings();
        xml_settings.ConformanceLevel = ConformanceLevel.Fragment;
        xml_settings.IgnoreComments = true;
        xml_settings.IgnoreWhitespace = true;
        XmlReader read_me = XmlReader.Create(new StringReader(load_me),xml_settings);
        root_holder new_root;
        playstation_id new_id;
        new_id.prefix = null;
        new_id.suffix = null;

        try {
		    while(read_me.Read()) {
                new_root.registry=null;
                new_root.path=null;
                new_root.shortcut=null;
                new_root.append_path=null;
                new_root.detract_path=null;
                new_root.game=null;

                add_me.absolute_path = null;
                add_me.relative_path = null;
                add_me.owner = null;
                add_me.absolute_root = null;
                add_me.relative_root = null;
                add_me.file_name = null;

			    if(read_me.NodeType==XmlNodeType.Element) {
                    switch (read_me.Name) {
					    case "title":
						    title = read_me.ReadString();
						    break;
                        case "root":
                            while(read_me.MoveToNextAttribute()) {
                                switch(read_me.Name) {
                                    case "path":
                                        new_root.path = read_me.Value;
                                        break;
                                    case "registry":
                                        new_root.registry=read_me.Value;
                                        break;
                                    case "shortcut":
                                        new_root.shortcut=read_me.Value;
                                        break;
                                    case "game":
                                        new_root.game = read_me.Value;
                                        break;
                                    case "append":
                                        new_root.append_path= read_me.Value;
                                        break;
                                    case "detract":
                                        new_root.detract_path= read_me.Value;
                                        break;
                                }
                            }
                            roots.Add(new_root);
                            break;
                        case "save":
                            while(read_me.MoveToNextAttribute()) {
                                switch(read_me.Name) {
                                    case "path":
                                        add_me.relative_path = read_me.Value;
                                        break;
                                    case "filename":
                                        add_me.file_name = read_me.Value;
                                        break;
                                }
                            }
                            saves.Add(add_me);
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
            MessageBox.Show("Something went wrong with parsing a game profile. Weird.");
            roots.Clear();
            return;
        }
        read_me.Close();

        detect(paths,steam, playstation);
//            string check_path = path.Replace("%PROGRAMFILES%",Environment.GetEnvironmentVariable("PROGRAMFILES"));
	}

    private string modifyPath(string path, string detract, string append) {
        path = path.TrimEnd(Path.DirectorySeparatorChar);
        if (detract!= null) {
            if(path.EndsWith(detract))
                path = path.Substring(0,path.Length-detract.Length);
        }
        if (append != null)
            path = Path.Combine(path,append);

        return path;
    }

    public void detect(PathHandler paths, SteamHandler steam, playstationHandler playstation) {
        ArrayList interim = new ArrayList();
        string key_path, key_name;
        string[] split;
        RegistryKey registry = Registry.CurrentUser;
        RegistryKey registry_sub_key;
        GameData parent_game;

        string path = null;
        FileInfo the_shortcut;
        StringBuilder start_menu;

        // This parses each root supplied by the XML file
        foreach (root_holder root in roots) {
            // This handles if the root is a relative path
            if(root.path!=null) {
                path = modifyPath(root.path,root.detract_path,root.append_path);

                if (root.path.StartsWith("%STEAM")){
                    interim.AddRange(steam.getSteamPaths(root.path));
                } else {
                    interim.AddRange(paths.getPath(root.path, override_virtualstore));
                }
            }

            // This handles if the root is a registry key
            if(root.registry!=null) {
                key_path = "";
                split = root.registry.Split('\\');
                for(int i = 1;i<split.Length-1;i++) {
                    key_path += split[i] + "\\";
                }
                key_path = key_path.TrimEnd('\\');
                key_name = split[split.Length-1];
                switch(split[0]) {
                    case "HKEY_CURRENT_USER":
                        registry = Registry.CurrentUser;
                        break;
                    case "HKEY_CLASSES_ROOT":
                        registry = Registry.ClassesRoot;
                        break;
                    case "HKEY_CURRENT_CONFIG":
                        registry = Registry.CurrentConfig;
                        break;
                    case "HKEY_DYN_DATA":
                        registry = Registry.DynData;
                        break;
                    case "HKEY_LOCAL_MACHINE":
                        registry = Registry.LocalMachine;
                        break;
                    case "HKEY_PERFORMANCE_DATA":
                        registry = Registry.PerformanceData;
                        break;
                    case "HKEY_USERS":
                        registry = Registry.Users;
                        break;
                } 
                registry_sub_key = registry.OpenSubKey(key_path);
                if (registry_sub_key == null)
                    registry_sub_key = registry.OpenSubKey(key_path.Replace("SOFTWARE", "Software\\Wow6432Node"));

                if(registry_sub_key!=null) {
                    path = registry_sub_key.GetValue(key_name).ToString();
                    if(path!=null){
                        path = modifyPath(path,root.detract_path,root.append_path);
                        interim.AddRange(interpretPath(path, steam, paths));
                    }
                }
            }
            // This handles if the root is a shortcut
            if(root.shortcut!=null) {
                the_shortcut = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),root.shortcut));
                if(!the_shortcut.Exists) {
                    start_menu = new StringBuilder(260);
                    SHGetSpecialFolderPath(IntPtr.Zero,start_menu,CSIDL_COMMON_STARTMENU,false);
                    the_shortcut = new FileInfo(Path.Combine(start_menu.ToString(),root.shortcut));
                }

                if(the_shortcut.Exists) {
                    IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                    IWshRuntimeLibrary.IWshShortcut link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(the_shortcut.FullName);

                    //split = link.TargetPath.Split(Path.DirectorySeparatorChar);
                    //path = split[0] + Path.DirectorySeparatorChar;
                    //for(int i=1;i<split.Length-1;i++)
                    //    path =  Path.Combine(path,split[i]);

                    path = Path.GetDirectoryName(link.TargetPath);

                    path = modifyPath(path,root.detract_path,root.append_path);


                    interim.AddRange(interpretPath(path, steam, paths));
                }
            }

            // This handles if the root is another game
            if(root.game!=null) {
                path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),"games");
                path = Path.Combine(path,root.game + ".xml");
                if(File.Exists(path)){
                    try {
                        parent_game = new GameData(path,paths,steam,playstation);
                        foreach(file_holder check_me in parent_game.detected_roots) {
                            path = modifyPath(check_me.absolute_path,root.detract_path,root.append_path);
                            interim.AddRange(interpretPath(path, steam, paths));
                        }
                    } catch {
                        MessageBox.Show("There was an error while loading " + title + ",\nwhile trying to load up " + root.game + ".xml","Places, Men");
                    }
                } else {
                    Console.WriteLine("Parent XML Not Found.");
                }
            }
        }


        file_holder playstation_root;
        playstation_root.owner = null;
        playstation_root.file_name = null;
        playstation_root.relative_path = "%DRIVE%";
        playstation_root.relative_root = "%DRIVE%";
        string found_id;

        file_holder add_me;
        add_me.absolute_path = null;
        add_me.relative_path = null;
        add_me.owner = null;
        add_me.absolute_root = null;
        add_me.relative_root = null;
        add_me.file_name = null;


        // This parses psp games
        if(psp_ids.Count>0) {
            if(playstation.psp_saves!=null) {
                foreach(playstation_id psp_id in psp_ids) {
                    found_id = playstation.detectPSPGame(psp_id);
                    if(found_id!=null) {
                        add_me.relative_path = playstation.psp_saves.Substring(3);
                        add_me.relative_root = Path.GetPathRoot(playstation.psp_saves);
                        saves.Add(add_me);

                        playstation_root.absolute_path = Path.GetPathRoot(playstation.psp_saves);
                        playstation_root.absolute_root = Path.GetPathRoot(playstation.psp_saves);
                        interim.Add(playstation_root);
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
                        add_me.file_name = null;
                        add_me.relative_path = playstation.ps3_saves.Substring(3);
                        add_me.relative_root = Path.GetPathRoot(playstation.ps3_saves); 
                        saves.Add(add_me);

                        playstation_root.absolute_path = Path.GetPathRoot(playstation.ps3_saves);
                        playstation_root.absolute_root = Path.GetPathRoot(playstation.ps3_saves);
                        interim.Add(playstation_root);
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
                        add_me.file_name = "BA" + found_id + "*";
                        add_me.relative_path = playstation.ps3_export.Substring(3);
                        add_me.relative_root = Path.GetPathRoot(playstation.ps3_export); 
                        saves.Add(add_me);

                        playstation_root.absolute_path = Path.GetPathRoot(playstation.ps3_export);
                        playstation_root.absolute_root = Path.GetPathRoot(playstation.ps3_export);
                        interim.Add(playstation_root);
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
                        add_me.file_name = "BA" + found_id + "*";
                        add_me.relative_path = playstation.ps3_export.Substring(3);
                        add_me.relative_root = Path.GetPathRoot(playstation.ps3_export); 
                        saves.Add(add_me);

                        playstation_root.absolute_path = Path.GetPathRoot(playstation.ps3_export);
                        playstation_root.absolute_root = Path.GetPathRoot(playstation.ps3_export);
                        interim.Add(playstation_root);
                    }
                }
                if(playstation.psp_saves!=null) {
                    found_id = playstation.detectPS1PSPGame(ps1_id);
                    if(found_id!=null) {
                        add_me.file_name = null;
                        add_me.relative_path = playstation.psp_saves.Substring(3);
                        add_me.relative_root = Path.GetPathRoot(playstation.ps3_export); 
                        saves.Add(add_me);

                        playstation_root.absolute_path = Path.GetPathRoot(playstation.ps3_export);
                        playstation_root.absolute_root = Path.GetPathRoot(playstation.ps3_export);
                        interim.Add(playstation_root);
                    }
                }
            }
        }

        detected_roots = new ArrayList();
        foreach (file_holder check_me in interim) {
            if(!detected_roots.Contains(check_me)) {
                if(identifier_file!=null&&identifier_path!=null) {
                    foreach(DirectoryInfo directory in getPaths(check_me.absolute_path,identifier_path)) {
                        if (directory.GetFiles(identifier_file).Length > 0) {
                            detected_roots.Add(check_me);
                        }
                    }
                } else if(identifier_path!=null) {
                    if(getPaths(check_me.absolute_path,identifier_path).Count>0) {
                        detected_roots.Add(check_me);
                    }
                } else if(identifier_file!=null) {
                    if(new DirectoryInfo(check_me.absolute_path).GetFiles(identifier_file).Length>0) {
                        detected_roots.Add(check_me);
                    }
                } else {
                    detected_roots.Add(check_me);
                }
            }
        }

    }

    public ArrayList interpretPath(string interpret_me, SteamHandler steam, PathHandler paths) {
        ArrayList return_me = new ArrayList();
        if(steam.installed&&interpret_me.ToLower().StartsWith(steam.path.ToLower() + "\\steamapps\\common")) {
            return_me.AddRange(steam.getSteamPaths("%STEAMCOMMON%" + interpret_me.Substring(steam.path.Length + 17)));
        } else if(paths.program_files_x86!=null&&interpret_me.ToLower().StartsWith(paths.program_files_x86.ToLower())) {
            return_me.AddRange(paths.getPath("%PROGRAMFILES%" + interpret_me.Substring(paths.program_files_x86.Length), override_virtualstore));
        } else if(paths.program_files!=null&&interpret_me.ToLower().StartsWith(paths.program_files.ToLower())) {
            return_me.AddRange(paths.getPath("%PROGRAMFILES%" + interpret_me.Substring(paths.program_files.Length), override_virtualstore));
        } else if (Directory.Exists(interpret_me)) {
            return_me.AddRange(paths.getPath("%DRIVE%" + interpret_me.Substring(2), override_virtualstore));
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
				return_me.AddRange(getPaths(Path.Combine(root,split[0]),forward_me));
			}
		}


		return return_me;
	}

    private ArrayList gatherFiles(string root) {
        ArrayList return_me = new ArrayList();
        foreach(DirectoryInfo sub_folder in new DirectoryInfo(root).GetDirectories()) {
            return_me.AddRange(gatherFiles(sub_folder.FullName));
        }
        foreach(FileInfo file in new DirectoryInfo(root).GetFiles()) {
            return_me.Add(file.FullName);
        }
        return return_me;
    }

    public ArrayList getSaves() {
        ArrayList return_me = new ArrayList();
		ArrayList directories;
        ArrayList files;
        file_holder add_me;

        foreach(file_holder root in detected_roots) {
            add_me.owner = root.owner;
            add_me.relative_root = root.relative_path;
            add_me.absolute_root = root.absolute_path;
            foreach (file_holder save in saves) {
			    directories = new ArrayList();
                files = new ArrayList();
                add_me.relative_path = save.relative_path;
                if(save.file_name==null) {  
				    if(save.relative_path==null) {
					      if(Directory.Exists(root.absolute_path)) {
                            files.AddRange(gatherFiles(root.absolute_path));
                            foreach(string file_name in files) {
                                if(Path.GetDirectoryName(file_name).Length==add_me.absolute_root.Length)
                                    add_me.absolute_path = "";
                                else 
                                    add_me.absolute_path = Path.GetDirectoryName(file_name).Substring(add_me.absolute_root.Length+1);
                                add_me.file_name = Path.GetFileName(file_name);
                                return_me.Add(add_me);
                            }
					    }
				    } else {
					    directories.AddRange(getPaths(root.absolute_path,save.relative_path));
					    foreach(DirectoryInfo directory in directories) {
                            files.AddRange(gatherFiles(directory.FullName));
                            foreach(string file_name in files) {
                                add_me.absolute_path = Path.GetDirectoryName(file_name).Substring(add_me.absolute_root.Length+1);
                                add_me.file_name = Path.GetFileName(file_name);
                                return_me.Add(add_me);
                            }
					    }
				    }
                } else if(save.relative_path==null) {
                    if(Directory.Exists(root.absolute_path)) {
                        foreach(FileInfo read_me in new DirectoryInfo(root.absolute_path).GetFiles(save.file_name)) {
                            if (read_me.DirectoryName.Length == add_me.absolute_root.Length)
                                add_me.absolute_path = "";
                            else
                                add_me.absolute_path = read_me.DirectoryName.Substring(add_me.absolute_root.Length + 1);
                            add_me.file_name = read_me.Name;
                            return_me.Add(add_me);
                        }
                    }
                } else {
				    directories.AddRange(getPaths(root.absolute_path,save.relative_path));
				    foreach(DirectoryInfo directory in directories) {
                        foreach(FileInfo read_me in directory.GetFiles(save.file_name)) {
                            add_me.absolute_path = read_me.DirectoryName.Substring(add_me.absolute_root.Length+1);
                            add_me.file_name = read_me.Name;
                            return_me.Add(add_me);
                        }
                    }
                }
            }
        }
        return return_me;
    }
    public file_holder getPath() {
        file_holder return_me;
        return_me.absolute_path = null;
        return_me.relative_path = null;
        return_me.absolute_root = null;
        return_me.file_name = null;
        return_me.owner = null;
        return_me.relative_root = null;
        if (detected_roots.Count != 0)
        {
            ArrayList from_me = new ArrayList();
            from_me.AddRange(getSaves());
            if(from_me.Count!=0) {
                file_holder example = ((file_holder)from_me[0]);
                return_me = example;
            } 
        }
        return return_me;
    }
    public file_holder checkThis(string this_right_here) {
        file_holder return_me;
        return_me.absolute_path = null;
        return_me.relative_path = null;
        return_me.absolute_root = null;
        return_me.file_name = null;
        return_me.owner = null;
        return_me.relative_root = null;
        string compare;
        foreach(file_holder check_me in getSaves()) {
            compare = Path.Combine(Path.Combine(check_me.absolute_root,check_me.absolute_path),check_me.file_name);
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
