using System;
using System.IO;
using System.Collections;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

public class PathHandler{
    public ArrayList user_list = new ArrayList();
    
    private ArrayList drives = new ArrayList();
    private ArrayList alt_paths = new ArrayList();
    public string host_name, users, user_profile, all_users, public_user, local_app_data, app_data, program_files, program_files_x86, common_program_files, user_documents, virtual_store=null;
    public bool uac = false, xp = false;
    private bool all_users_mode = false;
	public ArrayList usahs = new ArrayList();
    private SecurityHandler red_shirt = new SecurityHandler();

    public PathHandler() {
        string[] args = Environment.GetCommandLineArgs();
        for(int i = 0;i<args.Length;i++) {
            if(args[i]=="/allusers") {
                all_users_mode = true;
            }
        }

        
        // I would like to replace all this with per-user detection of paths,
		// if anyone knows how to get Environment variables and paths as another user, please let me know

		users = "";
        string[] disect_me = Environment.GetEnvironmentVariable("USERPROFILE").Split(Path.DirectorySeparatorChar);
        for(int i = 1;i<disect_me.Length;i++) {
            users += disect_me[i-1];
            if(i!=disect_me.Length-1)
                users += Path.DirectorySeparatorChar;
        }
        user_profile = Path.Combine(users,"%USERNAME%");

        if(red_shirt.amAdmin()&&all_users_mode) {
            DirectoryInfo[] read_us = new DirectoryInfo(users).GetDirectories();
            foreach(DirectoryInfo read_me in read_us) {
                if(read_me.Name!="Default"&&read_me.Name!="Default User"&&read_me.Name!="Public"&&read_me.Name!="All Users") {
                    user_list.Add(read_me.Name);
                }
            }
        } else {
            user_list.Add(disect_me[disect_me.Length-1]);
        }

        host_name = Environment.GetEnvironmentVariable("COMPUTERNAME");
        all_users = Environment.GetEnvironmentVariable("ALLUSERSPROFILE");
        public_user = Environment.GetEnvironmentVariable("PUBLIC");
        local_app_data = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
//        local_app_data = Environment.GetEnvironmentVariable("LOCALAPPDATA");
        app_data = Environment.GetEnvironmentVariable("APPDATA");
        if (local_app_data != null)
        {
            //Check that these work at home!!!
            disect_me = local_app_data.Split(Path.DirectorySeparatorChar);
            local_app_data = "";
            for(int i = 0;i<disect_me.Length;i++) {
                if(i==disect_me.Length-3)
                    local_app_data += "%USERNAME%";
                else
                    local_app_data += disect_me[i];
                if(i!=disect_me.Length-1)
                    local_app_data += "\\";
            }
            disect_me = app_data.Split(Path.DirectorySeparatorChar);
            app_data = "";
            for(int i = 0;i<disect_me.Length;i++) {
                if(i==disect_me.Length-3)
                    app_data += "%USERNAME%";
                else
                    app_data += disect_me[i];
                if(i!=disect_me.Length-1)
                    app_data += "\\";
            }
        } else {
            xp = true;
//            local_app_data = user_profile + "\\Local Settings\\Application Data";
            disect_me = app_data.Split(Path.DirectorySeparatorChar);
            app_data = "";
            for(int i = 0;i<disect_me.Length;i++) {
                if(i==disect_me.Length-2)
                    app_data += "%USERNAME%";
                else
                    app_data += disect_me[i];
                if(i!=disect_me.Length-1)
                    app_data += Path.DirectorySeparatorChar;
            }
        }
        program_files = Environment.GetEnvironmentVariable("PROGRAMFILES");
        program_files_x86 = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)");
        common_program_files = Environment.GetEnvironmentVariable("COMMONPROGRAMFILES");
        if(!xp) {
            RegistryManager uac_status = new RegistryManager("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
            if(uac_status.getValue("EnableLUA")=="1") {
                uac = true;
                virtual_store = local_app_data + "\\VirtualStore";
            }
            local_app_data = local_app_data.Replace(Environment.GetEnvironmentVariable("USERNAME"), "%USERNAME%");
        }
        user_documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace(Environment.GetEnvironmentVariable("USERNAME"), "%USERNAME%");
        foreach(DriveInfo look_here in DriveInfo.GetDrives()) {
            if(look_here.IsReady&&look_here.DriveType==DriveType.Fixed||look_here.DriveType==DriveType.Removable) {
                drives.Add(look_here.Name);
            }
        }
    }

	public void addAltPath(string add_me) {
        alt_paths.Add(add_me);
    }
    public void addAltPath(ArrayList add_me) {
        alt_paths.AddRange(add_me);
    }

    public void removeAltPath(string remove_me) {
        for(int i=0;i<alt_paths.Count;i++) {
            if((string)alt_paths[i]==remove_me)
                alt_paths.RemoveAt(i);
        }
    }

    public ArrayList getPath(string get_me, bool override_virtualstore) {
        ArrayList return_me = new ArrayList();
        DirectoryInfo directory;
        file_holder add_me;
        add_me.owner = null;
        add_me.file_name = null;
        add_me.relative_path = get_me;
        add_me.relative_root = null;
        add_me.absolute_root = null;
        if(get_me.StartsWith("%INSTALLLOCATION%")) {
            return_me.AddRange(getPath(get_me.Replace("%INSTALLLOCATION%","%DRIVE%"),override_virtualstore));
            return_me.AddRange(getPath(get_me.Replace("%INSTALLLOCATION%","%PROGRAMFILES%"),override_virtualstore));
            foreach(string drive in alt_paths) {
                add_me.absolute_path = get_me.Replace("%INSTALLLOCATION%",drive);
                add_me.absolute_root = drive;
                if (Directory.Exists(add_me.absolute_path)) 
                    return_me.Add(add_me);
            }
        } else if(get_me.StartsWith("%DRIVE%")) {
            string[] chopped = get_me.Split(Path.DirectorySeparatorChar);
            for(int i = 1;i<chopped.Length;i++) {
                get_me = "%DRIVE%";
                for(int j=i;j<chopped.Length;j++) {
                    get_me = Path.Combine(get_me,chopped[j]);
                }
                add_me.relative_root = "%DRIVE%";
                foreach(string drive in drives) {
                    add_me.absolute_path = get_me.Replace("%DRIVE%\\",drive);
                    add_me.absolute_root = drive;
                    if(Directory.Exists(add_me.absolute_path))
                        return_me.Add(add_me);
                }
            }
        } else if(get_me.StartsWith("%PROGRAMFILES%")) {
            string[] chopped = get_me.Split(Path.DirectorySeparatorChar);
            for(int i = 1;i<chopped.Length-1;i++) {
                get_me = "%PROGRAMFILES%";
                for(int j=i;j<chopped.Length;j++) {
                    get_me = Path.Combine(get_me,chopped[j]);
                }
                add_me.relative_root = "%PROGRAMFILES%";
                // Gotta do this the hard way. If UAC is ebnabled, then only VirtualStore,
                // unles virtualstore override is specified
                if(virtual_store!=null&&!override_virtualstore) {
                    foreach(string user in user_list) {
                        add_me.owner = user;
                        if(program_files_x86!=null) {
                            add_me.absolute_root = virtual_store + program_files_x86.Remove(0, 2);
                            add_me.absolute_path = get_me.Replace("%PROGRAMFILES%", add_me.absolute_root).Replace("%USERNAME%", user);
                            add_me.relative_path = get_me.Replace("%PROGRAMFILES%", add_me.absolute_root);
                            if (Directory.Exists(add_me.absolute_path))
                                return_me.Add(add_me);
                            }
                        add_me.absolute_root = virtual_store + program_files.Remove(0, 2);
                        add_me.absolute_path = get_me.Replace("%PROGRAMFILES%", add_me.absolute_root).Replace("%USERNAME%", user);
                        add_me.relative_path = get_me.Replace("%PROGRAMFILES%", add_me.absolute_root);
                        if (Directory.Exists(add_me.absolute_path))
                            return_me.Add(add_me);
                    }
                } else {
                    add_me.owner = null;
                    if(program_files_x86!=null) {
                        add_me.absolute_root = program_files_x86;
                        add_me.absolute_path = get_me.Replace("%PROGRAMFILES%", program_files_x86);
                        add_me.relative_path = get_me.Replace("%PROGRAMFILES%", add_me.absolute_root);
                        if (Directory.Exists(add_me.absolute_path))
                            return_me.Add(add_me);
                    }
                    add_me.absolute_root = program_files;
                    add_me.absolute_path = get_me.Replace("%PROGRAMFILES%", program_files);
                    add_me.relative_path = get_me.Replace("%PROGRAMFILES%", add_me.absolute_root);
                    if (Directory.Exists(add_me.absolute_path))
                        return_me.Add(add_me);
                }
            }
        } else if(get_me.StartsWith("%USERDOCUMENTS%")) {
            get_me = get_me.Replace("%USERDOCUMENTS%",user_documents);
            add_me.relative_path = get_me;
            foreach (string user in user_list){
                add_me.owner = user;
                add_me.absolute_path = get_me.Replace("%USERNAME%", user);
                if (Directory.Exists(add_me.absolute_path)) {
                    directory = new DirectoryInfo(add_me.absolute_path);
                    //Console.WriteLine(add_me.absolute_path);
                    //Console.WriteLine(add_me.relative_path);
                    return_me.Add(add_me);
                }
            }
        } else if(get_me.StartsWith("%USERPROFILE%")) {
            get_me = get_me.Replace("%USERPROFILE%", user_profile);
            add_me.relative_path = get_me;
            foreach (string user in user_list){
                add_me.owner = user;
                add_me.absolute_path = get_me.Replace("%USERNAME%", user);
                if(Directory.Exists(add_me.absolute_path)) {
                    directory = new DirectoryInfo(add_me.absolute_path);
                    //Console.WriteLine(add_me.absolute_path);
                    //Console.WriteLine(add_me.relative_path);
                    return_me.Add(add_me);
                }
            }
        } else if(get_me.StartsWith("%APPDATA%")) {
            get_me = get_me.Replace("%APPDATA%", app_data);
            add_me.relative_path = get_me;
            foreach (string user in user_list){
                add_me.owner = user;
                add_me.absolute_path = get_me.Replace("%USERNAME%", user);
                if(Directory.Exists(add_me.absolute_path)) {
                    directory = new DirectoryInfo(add_me.absolute_path);
                    //Console.WriteLine(add_me.absolute_path);
                    //Console.WriteLine(add_me.relative_path);
                    return_me.Add(add_me);
                }
            }
        } else if(get_me.StartsWith("%LOCALAPPDATA%")) {
            get_me = get_me.Replace("%LOCALAPPDATA%", local_app_data);
            add_me.relative_path = get_me;
            foreach (string user in user_list){
                add_me.owner = user;
                add_me.absolute_path = get_me.Replace("%USERNAME%", user);
                if(Directory.Exists(add_me.absolute_path)) {
                    directory = new DirectoryInfo(add_me.absolute_path);
                    //Console.WriteLine(add_me.absolute_path);
                    //Console.WriteLine(add_me.relative_path);
                    return_me.Add(add_me);
                }
            }
        } else if(get_me.StartsWith("%ALLUSERSPROFILE%")) {
            add_me.absolute_path = get_me.Replace("%ALLUSERSPROFILE%", all_users);
            if(Directory.Exists(add_me.absolute_path)) {
                directory = new DirectoryInfo(add_me.absolute_path);
                //Console.WriteLine(add_me.absolute_path);
                //Console.WriteLine(add_me.relative_path);
                return_me.Add(add_me);
            }
        } else if(get_me.StartsWith("%PUBLIC%")) {
            if(public_user==null) {
                add_me.absolute_path = get_me.Replace("%PUBLIC%", all_users);
            } else {
                add_me.absolute_path = get_me.Replace("%PUBLIC%", public_user);
            }
            if(Directory.Exists(add_me.absolute_path)) {
                directory = new DirectoryInfo(add_me.absolute_path);
                //Console.WriteLine(add_me.absolute_path);
                //Console.WriteLine(add_me.relative_path);
                return_me.Add(add_me);
            }
        } else {
            add_me.relative_root = null;
            add_me.absolute_path = get_me;
            add_me.absolute_root = null;
            if(Directory.Exists(add_me.absolute_path))
                return_me.Add(add_me);
        }
        return return_me;
    }
}
