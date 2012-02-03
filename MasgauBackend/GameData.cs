using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Windows.Forms;


public class GameData {
	public string name, title;
    public ArrayList detected_roots;
    public bool root_detected = false, save_detected = false, override_virtualstore = false;
    public ArrayList roots = new ArrayList();
    public ArrayList saves = new ArrayList();

	public GameData(string load_me, PathHandler paths, SteamHandler steam) {
        FileInfo xml_file = new FileInfo(load_me);
        file_holder add_me;
        add_me.absolute_path = null;
        add_me.owner = null;
        add_me.absolute_root = null;
        add_me.relative_root = null;
        ArrayList add_us = new ArrayList();
        name = xml_file.Name.Replace(".xml","");
        XmlTextReader read_me = new XmlTextReader(load_me);

        try {
		    while(read_me.Read()) {
			    if(read_me.NodeType==XmlNodeType.Element) {
				    switch (read_me.Name) {
					    case "title":
						    title = read_me.ReadString();
						    break;
                        case "root":
                            read_me.MoveToAttribute("path");
                            roots.Add(read_me.Value);
                            break;
                        case "save":
                            read_me.MoveToAttribute("path");
                            add_me.relative_path = read_me.Value;
                            add_me.file_name = null;
                            read_me.MoveToAttribute("filename");
                            add_me.file_name = read_me.Value;
                            saves.Add(add_me);
                            break;
                        case "virtualstore":
                            read_me.MoveToAttribute("override");
                            if(read_me.Value=="yes") {
                                override_virtualstore = true;
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

        detect(paths,steam);
//            string check_path = path.Replace("%PROGRAMFILES%",Environment.GetEnvironmentVariable("PROGRAMFILES"));
        read_me.Close();
	}

    public bool detect(PathHandler paths, SteamHandler steam) {
        detected_roots = new ArrayList();
        foreach (string root in roots) {
            if (root.StartsWith("%STEAM")){
                detected_roots.AddRange(steam.getSteamPaths(root));
            } else {
                detected_roots.AddRange(paths.getPath(root,override_virtualstore));
            }
        }

        if(detected_roots.Count!=0) {
            root_detected = true;
        } else {
            root_detected = false;
        }
        return save_detected;
    }

    public ArrayList getSaves() {
        ArrayList return_me = new ArrayList();
        file_holder add_me;
        foreach(file_holder root in detected_roots) {
            add_me.owner = root.owner;
            add_me.relative_root = root.relative_path;
            add_me.absolute_root = root.absolute_path;
            foreach (file_holder save in saves) {
                if(save.file_name==save.relative_path) {
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
    public file_holder getPath(PathHandler paths, string steam_path) {
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

}
