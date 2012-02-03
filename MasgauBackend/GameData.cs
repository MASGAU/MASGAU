using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Windows.Forms;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

public class GameData {
	public string name, title, identifier_file = null, identifier_path = null;
    public bool root_detected = false, save_detected = false, override_virtualstore = false;
    public ArrayList roots = new ArrayList();
    public ArrayList saves = new ArrayList();
    public ArrayList mods = new ArrayList();
    public ArrayList detected_roots;
    const int CSIDL_COMMON_STARTMENU = 0x0016; 

    [DllImport("shell32.dll")]
    static extern bool SHGetSpecialFolderPath(
    IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);

	public GameData(string load_me, PathHandler paths, SteamHandler steam) {
        FileInfo xml_file = new FileInfo(load_me);
        file_holder add_me;
        ArrayList add_us = new ArrayList();
        name = xml_file.Name.Replace(".xml","");
        XmlTextReader read_me = new XmlTextReader(load_me);
        root_holder new_root;

        try {
		    while(read_me.Read()) {
                new_root.registry=null;
                new_root.path=null;
                new_root.shortcut=null;
                new_root.append_path=null;
                new_root.detract_path=null;

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
                                        new_root.path=read_me.Value;
                                        break;
                                    case "registry":
                                        new_root.registry=read_me.Value;
                                        break;
                                    case "shortcut":
                                        new_root.shortcut=read_me.Value;
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
				    }
			    }
		    }
        } catch (XmlException) {
            MessageBox.Show("The file " + xml_file.Name + " has some errors in it. Go fix it.");
            root_detected = false;
            save_detected = false;
            roots.Clear();
            return;
        }
        read_me.Close();

        detect(paths,steam);
//            string check_path = path.Replace("%PROGRAMFILES%",Environment.GetEnvironmentVariable("PROGRAMFILES"));
	}

    public bool detect(PathHandler paths, SteamHandler steam) {
        ArrayList interim = new ArrayList();
        string key_path, key_name;
        string[] split;
        RegistryKey registry = Registry.CurrentUser;
        RegistryKey registry_sub_key;

        string path = null;
        FileInfo the_shortcut;
        StringBuilder start_menu;

        // This parses each root supplied by the XML file
        foreach (root_holder root in roots) {
            // This handles if the root is a realtive path
            if(root.path!=null) {
                path = root.path;
                if(root.append_path!=null)
                    path = path + "\\" + root.append_path;
                if(root.detract_path!=null)
                    path = path.TrimEnd(root.detract_path.ToCharArray());

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
                        path = path.TrimEnd('\\');
                        if (root.append_path != null)
                            path = path + "\\" + root.append_path;
                        if (root.detract_path != null)
                            path = path.TrimEnd(root.detract_path.ToCharArray());
                        interim.AddRange(interpretPath(path, steam, paths));
                    }
                }
            }
            // This handles if the root is a shortcut
            if(root.shortcut!=null) {
                the_shortcut = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" + root.shortcut);
                if(!the_shortcut.Exists) {
                    start_menu = new StringBuilder(260);
                    SHGetSpecialFolderPath(IntPtr.Zero,start_menu,CSIDL_COMMON_STARTMENU,false);
                    the_shortcut = new FileInfo(start_menu + "\\" + root.shortcut);
                }

                if(the_shortcut.Exists) {
                    IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                    IWshRuntimeLibrary.IWshShortcut link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(the_shortcut.FullName);
                    split = link.TargetPath.Split('\\');

                    path = split[0];
                    for(int i=1;i<split.Length-1;i++)
                        path +=  "\\" + split[i];

                    if (root.append_path != null)
                        path = path + "\\" + root.append_path;
                    if (root.detract_path != null)
                        path = path.TrimEnd(root.detract_path.ToCharArray());
                    path = path.TrimEnd('\\');

                    interim.AddRange(interpretPath(path, steam, paths));
                }
            }
        }

        detected_roots = new ArrayList();
        foreach (file_holder check_me in interim) {
            if(!detected_roots.Contains(check_me)) {
                if(identifier_file!=null&&identifier_path!=null) {
                    if (File.Exists(check_me.absolute_path + "\\" + identifier_path + "\\" + identifier_file))
                        detected_roots.Add(check_me);
                } else if(identifier_path!=null) {
                    if(Directory.Exists(check_me.absolute_path + "\\" + identifier_path))
                        detected_roots.Add(check_me);
                } else if(identifier_file!=null) {
                    if(File.Exists(check_me.absolute_path + "\\" + identifier_file))
                        detected_roots.Add(check_me);
                } else {
                    detected_roots.Add(check_me);
                }
            }
        }

        if(detected_roots.Count!=0) {
            root_detected = true;
            DirectoryInfo mod_dir = new DirectoryInfo(Application.StartupPath + "\\games\\" + name);
            mods = new ArrayList();
            if (mod_dir.Exists) {
                ModData add_me;
                foreach(FileInfo load_me in mod_dir.GetFiles("*.xml")) {
                    add_me = new ModData(load_me.FullName, detected_roots, paths, steam);
                    mods.Add(add_me);
                }
            }
        } else {
            root_detected = false;
        }
        return save_detected;
    }

    private ArrayList interpretPath(string interpret_me, SteamHandler steam, PathHandler paths) {
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

    public ArrayList getSaves() {
        ArrayList return_me = new ArrayList();
        file_holder add_me;
        foreach(file_holder root in detected_roots) {
            add_me.owner = root.owner;
            add_me.relative_root = root.relative_path;
            add_me.absolute_root = root.absolute_path;
            foreach (file_holder save in saves) {
                if(save.file_name==null) {
                    if(Directory.Exists(root.absolute_path + "\\" + save.relative_path)) {
                        add_me.absolute_path = root.absolute_path + "\\" + save.relative_path;
                        add_me.relative_path = save.relative_path;
                        add_me.file_name = null;
                        return_me.Add(add_me);
                    }
                } else if(save.relative_path=="") {
                    if(Directory.Exists(root.absolute_path)) {
                        foreach(FileInfo read_me in new DirectoryInfo(root.absolute_path).GetFiles(save.file_name)) {
                            add_me.absolute_path = read_me.DirectoryName;
                            add_me.relative_path = save.relative_path;
                            add_me.file_name = read_me.Name;
                            return_me.Add(add_me);
                        }
                    }
                } else {
                    if(Directory.Exists(root.absolute_path + "\\" + save.relative_path)) {
                        foreach(FileInfo read_me in new DirectoryInfo(root.absolute_path + "\\" + save.relative_path).GetFiles(save.file_name)) {
                            add_me.absolute_path = read_me.DirectoryName;
                            add_me.relative_path = save.relative_path;
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

    public int findMod(string find_me) {
        for(int i = 0; i<mods.Count;i++) {
            if(((ModData)mods[i]).name==find_me)
                return i;
        }
        return -1;
    }

}
