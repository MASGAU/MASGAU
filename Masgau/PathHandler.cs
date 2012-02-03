using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MASGAU {
    public struct user_data
    {
        public string name, user_dir, user_documents, app_data, local_app_data, start_menu, virtual_store, saved_games;
    }

    public class PathHandler{
        public Dictionary<string,user_data> users = new Dictionary<string,user_data>();
        private ArrayList drives = new ArrayList();
        private ArrayList alt_paths = new ArrayList();
        public string user_root, host_name, program_files, program_files_x86, common_program_files, all_users_profile, public_profile;
        public bool uac = false, xp = false;
        private bool all_users_mode = false;
        private SecurityHandler red_shirt = new SecurityHandler();

	    [StructLayout(LayoutKind.Sequential)]
	    public struct LUID
	    {
	    public int LowPart;
	    public int HighPart;
	    } 
	    [StructLayout(LayoutKind.Sequential)]
	    public struct TOKEN_PRIVILEGES
	    {
	    public LUID Luid;
	    public int Attributes;
	    public int PrivilegeCount;
	    }

	    [DllImport("advapi32.dll", CharSet=CharSet.Auto)]
	    public static extern int OpenProcessToken(int ProcessHandle, int DesiredAccess, 
	    ref int tokenhandle);

	    [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
	    public static extern int GetCurrentProcess();

	    [DllImport("advapi32.dll", CharSet=CharSet.Auto)]
	    public static extern int LookupPrivilegeValue(string lpsystemname, string lpname, 
	    [MarshalAs(UnmanagedType.Struct)] ref LUID lpLuid);

	    [DllImport("advapi32.dll", CharSet=CharSet.Auto)]
	    public static extern int AdjustTokenPrivileges(int tokenhandle, int disableprivs, 
	    [MarshalAs(UnmanagedType.Struct)]ref TOKEN_PRIVILEGES Newstate, int bufferlength, 
	    int PreivousState, int Returnlength);

	    [DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
	    public static extern int RegLoadKey(uint hKey,string lpSubKey, string lpFile);

	    [DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
	    public static extern int RegUnLoadKey(uint hKey, string lpSubKey);

        public const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
	    public const int TOKEN_QUERY = 0x00000008;
	    public const int SE_PRIVILEGE_ENABLED = 0x00000002;
	    public const string SE_RESTORE_NAME = "SeRestorePrivilege";
	    public const string SE_BACKUP_NAME = "SeBackupPrivilege";
	    public const uint HKEY_USERS = 0x80000003;
	    public string shortname;
	
	
	
	
	
	    public PathHandler() {
            string[] args = Environment.GetCommandLineArgs();
            for(int i = 0;i<args.Length;i++) {
                if(args[i]=="/allusers") {
                    all_users_mode = true;
                }
            }

            // The global variables
            if(Environment.GetEnvironmentVariable("ProgramW6432")!=null) {
                program_files = Environment.GetEnvironmentVariable("ProgramW6432");
            } else {
                program_files = Environment.GetEnvironmentVariable("PROGRAMFILES");
            }
            program_files_x86 = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)");
            common_program_files = Environment.GetEnvironmentVariable("COMMONPROGRAMFILES");
            foreach(DriveInfo look_here in DriveInfo.GetDrives()) {
                if(look_here.IsReady&&(look_here.DriveType==DriveType.Fixed||look_here.DriveType==DriveType.Removable)) {
                    drives.Add(look_here.Name);
                }
            }
            host_name = Environment.GetEnvironmentVariable("COMPUTERNAME");
            all_users_profile = Environment.GetEnvironmentVariable("ALLUSERSPROFILE");
            public_profile = Environment.GetEnvironmentVariable("PUBLIC");
            if (Environment.GetEnvironmentVariable("LOCALAPPDATA") != null){
                xp = false;
            } else {
                xp = true;
            }
		    if(!xp) {
			    RegistryManager uac_status = new RegistryManager(RegistryHive.LocalMachine,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",false);	
			    if(uac_status.getValue("EnableLUA")=="1") {
				    uac = true;
			    }
		    }
            string[] split = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Split(Path.DirectorySeparatorChar);
            user_root = split[0];
		    if(xp) {
			    for(int i = 1;i<split.Length-2;i++) {
				    user_root += Path.DirectorySeparatorChar + split[i];
			    }
		    } else {
			    for(int i = 1;i<split.Length-3;i++) {
				    user_root += Path.DirectorySeparatorChar + split[i];
			    }
		    }



            //Per-user variables
            user_data add_me;

            RegistryManager user_key = new RegistryManager(RegistryHive.CurrentUser,@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders",false);
            try {
                split = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Split(Path.DirectorySeparatorChar);
                if(xp)
				    add_me.name = split[split.Length-2];
			    else 
				    add_me.name = split[split.Length-3];
                add_me.app_data = user_key.getValue("AppData");
                add_me.local_app_data = user_key.getValue("Local AppData");
                //add_me.local_settings = user_key.GetValue("Local Settings").ToString();
                add_me.start_menu = user_key.getValue("Start Menu");
                add_me.user_dir = split[0];
			    if(xp) {
				    for(int i = 1;i<split.Length-1;i++) {
					    add_me.user_dir += Path.DirectorySeparatorChar + split[i];
				    }
			    } else {
				    for(int i = 1;i<split.Length-2;i++) {
					    add_me.user_dir += Path.DirectorySeparatorChar + split[i];
				    }
                }
                add_me.user_documents = user_key.getValue("Personal");
                add_me.virtual_store = Path.Combine(user_key.getValue("Local AppData"),"VirtualStore");

                user_key.close();

                user_key = new RegistryManager(RegistryHive.CurrentUser,@"Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders",false);

                try {
                    add_me.saved_games = user_key.getValue("{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}");
                    if(add_me.saved_games.StartsWith("%USERPROFILE%")) {
                        add_me.saved_games = Path.Combine(add_me.user_dir,add_me.saved_games.Substring(14));
                    }
                } catch {
                    if(Directory.Exists(Path.Combine(add_me.user_dir, "Saved Games"))) {
                        add_me.saved_games = Path.Combine(add_me.user_dir, "Saved Games");
                    } else {
                        add_me.saved_games = null;
                    }
                }
                users.Add(add_me.name,add_me);

                user_key.close();
            } catch {
                Console.WriteLine("Registry read error.");
            }
            if(all_users_mode) {
                // All this crap lets me get data from other user's registries
			    int token=0;
			    int retval=0;

			    TOKEN_PRIVILEGES TP = new TOKEN_PRIVILEGES();
			    TOKEN_PRIVILEGES TP2 = new TOKEN_PRIVILEGES();
			    LUID RestoreLuid = new LUID();
			    LUID BackupLuid = new LUID();

			    retval = OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref token);
			    retval = LookupPrivilegeValue(null, SE_RESTORE_NAME, ref RestoreLuid);
			    retval = LookupPrivilegeValue(null, SE_BACKUP_NAME, ref BackupLuid);
			    TP.PrivilegeCount = 1;
			    TP.Attributes = SE_PRIVILEGE_ENABLED;
			    TP.Luid = RestoreLuid;
			    TP2.PrivilegeCount = 1;
			    TP2.Attributes = SE_PRIVILEGE_ENABLED;
			    TP2.Luid = BackupLuid;

			    retval = AdjustTokenPrivileges(token, 0, ref TP, 1024, 0, 0);
			    retval = AdjustTokenPrivileges(token, 0, ref TP2, 1024, 0, 0);


			
			
			    RegistryManager sub_key;
                foreach(DirectoryInfo user_folder in new DirectoryInfo(user_root).GetDirectories()) {
				    if(user_folder.Name.ToLower()!="default user"&&user_folder.Name.ToLower()!="default"&&user_folder.Name.ToLower()!="all users"&&File.Exists(Path.Combine(user_folder.FullName,"NTUSER.DAT"))) {
					    RegLoadKey(HKEY_USERS,user_folder.Name,Path.Combine(user_folder.FullName,"NTUSER.DAT"));
                        sub_key = new RegistryManager(RegistryHive.Users,user_folder.Name + "\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders",false);
					    try {
                            split = sub_key.getValue("AppData").Split(Path.DirectorySeparatorChar);
						    if(xp)
							    add_me.name = split[split.Length-2];
						    else 
							    add_me.name = split[split.Length-3];
						    add_me.app_data = sub_key.getValue("AppData");
						    add_me.local_app_data = sub_key.getValue("Local AppData");
						    //add_me.local_settings = sub_key.GetValue("Local Settings").ToString();
						    add_me.start_menu = sub_key.getValue("Start Menu");
                            add_me.user_dir = split[0];
						    if(xp){
							    for(int i = 1;i<split.Length-1;i++) {
								    add_me.user_dir += Path.DirectorySeparatorChar + split[i];
							    }
						    } else {
							    for(int i = 1;i<split.Length-2;i++) {
								    add_me.user_dir += Path.DirectorySeparatorChar + split[i];
							    }
						    }
                            if(Directory.Exists(Path.Combine(add_me.user_dir, "Saved Games"))) {
                                add_me.saved_games = Path.Combine(add_me.user_dir, "Saved Games");
                            } else {
                                add_me.saved_games = null;
                            }
						    add_me.user_documents = sub_key.getValue("Personal");
						    add_me.virtual_store = Path.Combine(sub_key.getValue("AppData"),"VirtualStore");
                            sub_key.close();

                            sub_key = new RegistryManager(RegistryHive.Users,user_folder.Name + "\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders",false);
                            try {
                                add_me.saved_games = sub_key.getValue("{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}");
                                if(add_me.saved_games.StartsWith("%USERPROFILE%")) {
                                    add_me.saved_games = Path.Combine(add_me.user_dir,add_me.saved_games.Substring(14));
                                }
                            } catch {
    					        if(sub_key!=null)
                                    sub_key.close();
                                if(Directory.Exists(Path.Combine(add_me.user_dir, "Saved Games"))) {
                                    add_me.saved_games = Path.Combine(add_me.user_dir, "Saved Games");
                                } else {
                                    add_me.saved_games = null;
                                }
                            }
						    users.Add(add_me.name,add_me);
					    } catch {
						    Console.WriteLine("Registry read error.");
					    }
					    if(sub_key!=null)
                            sub_key.close();
				        string result = RegUnLoadKey(HKEY_USERS,user_folder.Name).ToString();	
	                    if(result!="0"&&result!="87")
                            Console.WriteLine("Error unloading " + user_folder.Name + " registry hive");
				    }
                }
			    Console.WriteLine("test");
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

        public ArrayList getPath(location_path_holder get_me, bool override_virtualstore) {
            ArrayList return_me = new ArrayList();
            location_holder add_me;
            add_me.owner = null;
            add_me.path = get_me.path;
            add_me.rel_root = get_me.environment_variable;
            add_me.abs_root= null;
            DirectoryInfo test;
            switch(get_me.environment_variable) {
                case "installlocation":
                    string[] chopped = get_me.path.Split(Path.DirectorySeparatorChar);
                    for(int i = 0;i<chopped.Length;i++) {
                        get_me.path = chopped[i];
                        for(int j=i+1;j<chopped.Length;j++) {
                            get_me.path = Path.Combine(get_me.path,chopped[j]);
                        }

                        get_me.environment_variable = "drive";
                        return_me.AddRange(getPath(get_me,override_virtualstore));
                        get_me.environment_variable = "programfiles";
                        return_me.AddRange(getPath(get_me,override_virtualstore));
                        get_me.environment_variable = "altsavepaths";
                        return_me.AddRange(getPath(get_me,override_virtualstore));
                    }
                    break;
                case "altsavepaths":
                    foreach(string alt_path in alt_paths) {
                        if(get_me.path!=null&&get_me.path.Length>0)
                            test = new DirectoryInfo(Path.Combine(alt_path, get_me.path));
                        else 
                            test = new DirectoryInfo(alt_path);

                        if (test.Exists)  {
                            add_me.abs_root = alt_path;
                            return_me.Add(add_me);
                        }
                    }
                    break;
                case "drive":
                    foreach(string drive in drives) {
                        if(get_me.path!=null&&get_me.path.Length>0)
                            test = new DirectoryInfo(Path.Combine(drive,get_me.path));
                        else
                            test = new DirectoryInfo(drive);
                        if(test.Exists) {
                            add_me.abs_root = drive;
                            return_me.Add(add_me);
                        }
                    }
                    break;
                case "programfiles":
                    // Always checks both the VirtualStore and the real Program Files,
                    // to make sure nothing is missed, especially in the case of old games
                    // that may or may not use the VirtualStore
                    location_path_holder virtualstore_info = get_me;

                    virtualstore_info.environment_variable = "localappdata";
                    if(program_files_x86!=null) {
                        virtualstore_info.path = Path.Combine(Path.Combine("VirtualStore", program_files_x86.Substring(3)), virtualstore_info.path);
                        return_me.AddRange(getPath(virtualstore_info, override_virtualstore));
                        virtualstore_info = get_me;
                        virtualstore_info.environment_variable = "localappdata";
                    }
                    virtualstore_info.path = Path.Combine(Path.Combine("VirtualStore", program_files.Substring(3)), virtualstore_info.path);
                    return_me.AddRange(getPath(virtualstore_info, override_virtualstore));

                    add_me.owner = null;
                    if(program_files_x86!=null) {
                        if(get_me.path!=null&&get_me.path.Length>0)
                            test = new DirectoryInfo(Path.Combine(program_files_x86, get_me.path));
                        else
                            test = new DirectoryInfo(program_files_x86);
                        if (test.Exists) {
                            add_me.abs_root = program_files_x86;
                            return_me.Add(add_me);
                        }
                    }

                    if(get_me.path!=null&&get_me.path.Length>0)
                        test = new DirectoryInfo(Path.Combine(program_files, get_me.path));
                    else
                        test = new DirectoryInfo(program_files);

                    if (test.Exists) {
                        add_me.abs_root= program_files;
                        return_me.Add(add_me);
                    }
                    break;
                case "userdocuments":
                    foreach(KeyValuePair<string,user_data> user in users){
                        add_me.owner = user.Key;

                        if(get_me.path!=null&&get_me.path.Length>0)
                            test = new DirectoryInfo(Path.Combine(user.Value.user_documents, get_me.path));
                        else
                            test = new DirectoryInfo(user.Value.user_documents);

                        if (test.Exists) {
                            add_me.abs_root = user.Value.user_documents;
                            return_me.Add(add_me);
                        }
                    }
                    break;
                case "userprofile":
                    foreach(KeyValuePair<string,user_data> user in users){
                        add_me.owner = user.Key;
                        if(get_me.path!=null&&get_me.path.Length>0)
                            test = new DirectoryInfo(Path.Combine(user.Value.user_dir, get_me.path));
                        else 
                            test = new DirectoryInfo(user.Value.user_dir);

                        if(test.Exists) {
                            add_me.abs_root = user.Value.user_dir;
                            return_me.Add(add_me);
                        }
                    }
                    break;
                case "savedgames":
                    foreach(KeyValuePair<string,user_data> user in users){
				        if(user.Value.saved_games!=null) {
					        add_me.owner = user.Key;
                            if(get_me.path!=null&&get_me.path.Length>0)
                                test = new DirectoryInfo(Path.Combine(user.Value.saved_games, get_me.path));
                            else
                                test = new DirectoryInfo(user.Value.saved_games);

					        if(test.Exists) {
                                add_me.abs_root= user.Value.saved_games;
						        return_me.Add(add_me);
					        }
				        }
                    }
                    break;
                case "appdata":
                    foreach(KeyValuePair<string,user_data> user in users){
                        add_me.owner = user.Key;
                        if(get_me.path!=null&&get_me.path.Length>0)
                            test = new DirectoryInfo(Path.Combine(user.Value.app_data, get_me.path));
                        else
                            test = new DirectoryInfo(user.Value.app_data);

                        if(test.Exists) {
                            add_me.abs_root= user.Value.app_data;
                            return_me.Add(add_me);
                        }
                    }
                    break;
                case "localappdata":
                    foreach(KeyValuePair<string,user_data> user in users){
                        add_me.owner = user.Key;
                        if(get_me.path!=null&&get_me.path.Length>0)
                            test = new DirectoryInfo(Path.Combine(user.Value.local_app_data, get_me.path));
                        else
                            test = new DirectoryInfo(user.Value.local_app_data);

                        if(test.Exists) {
                            add_me.abs_root= user.Value.local_app_data;
                            return_me.Add(add_me);
                        }
                    }
                    break;
                case "allusersprofile":
                    if(get_me.path!=null&&get_me.path.Length>0)
                        test = new DirectoryInfo(Path.Combine(all_users_profile, get_me.path));
                    else
                        test = new DirectoryInfo(all_users_profile);

                    if(test.Exists) {
                        add_me.abs_root = all_users_profile;
                        return_me.Add(add_me);
                    }
                    break;
                case "public":
                    if(public_profile!=null) {
                        if(get_me.path!=null&&get_me.path.Length>0)
                            test = new DirectoryInfo(Path.Combine(public_profile,get_me.path));
                        else
                            test = new DirectoryInfo(public_profile);

                        if(test.Exists) {
                            add_me.abs_root = public_profile;
                            return_me.Add(add_me);
                        }
                    }
                    break;
                default:
                    // This should never happen.

                    //add_me.relative_path = get_me;
                    //add_me.relative_root = null;
                    //test = new DirectoryInfo(get_me);
                    //if(test.Exists) {
                    //    add_me.absolute_path = test.FullName;
                    //    add_me.absolute_root = null;
                    //    return_me.Add(add_me);
                    //}

	    		    MessageBox.Show("The specified environment variable" + get_me.environment_variable + " is not recognized. You either spelled it wrong or something.","It's just this environment",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    break;
            }
            return return_me;
        }

        public string getAbsoluteRoot(location_holder parse_me, string with_user) {
            string return_me = null;
            if(parse_me.rel_root=="allusersprofile") {
                return_me = all_users_profile;
            } else if(parse_me.rel_root=="public") {
                if(public_profile==null) {
                    return_me = all_users_profile;
                } else {
                    return_me = public_profile;
                }
            } 
            if(with_user!=null&&users.ContainsKey(with_user)) {
                if(parse_me.rel_root=="userdocuments") {
                    return_me = users[with_user].user_documents;
                } else if(parse_me.rel_root=="userprofile") {
                    return_me = users[with_user].user_dir;
                } else if(parse_me.rel_root=="savedgames") {
                    return_me = users[with_user].saved_games;
                } else if(parse_me.rel_root=="appdata") {
                    return_me = users[with_user].app_data;
                } else if(parse_me.rel_root=="localappdata") {
                    return_me = users[with_user].local_app_data;
                }
            } 
            return return_me;
        }
    }
}