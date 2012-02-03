using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using Microsoft.Win32;

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
        program_files = Environment.GetEnvironmentVariable("PROGRAMFILES");
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
			RegistryManager uac_status = new RegistryManager("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");	
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
        try {
            RegistryKey user_key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders");
            split = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Split(Path.DirectorySeparatorChar);
            if(xp)
				add_me.name = split[split.Length-2];
			else 
				add_me.name = split[split.Length-3];
            add_me.app_data = user_key.GetValue("AppData").ToString();
            add_me.local_app_data = user_key.GetValue("Local AppData").ToString();
            //add_me.local_settings = user_key.GetValue("Local Settings").ToString();
            add_me.start_menu = user_key.GetValue("Start Menu").ToString();
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
            add_me.user_documents = user_key.GetValue("Personal").ToString();
            add_me.virtual_store = Path.Combine(user_key.GetValue("AppData").ToString(),"VirtualStore");

            try {
                user_key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders");
                add_me.saved_games = user_key.GetValue("{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}").ToString();
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


			
			
			RegistryKey sub_key;
            foreach(DirectoryInfo user_folder in new DirectoryInfo(user_root).GetDirectories()) {
				if(user_folder.Name!="Default User"&&user_folder.Name!="Default"&&user_folder.Name!="All Users"&&File.Exists(Path.Combine(user_folder.FullName,"NTUSER.DAT"))) {
					RegLoadKey(HKEY_USERS,user_folder.Name,Path.Combine(user_folder.FullName,"NTUSER.DAT"));
                    sub_key = Registry.Users.OpenSubKey(user_folder.Name);
					try {
                        sub_key = sub_key.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer");
                        sub_key = sub_key.OpenSubKey("Shell Folders");
                        split = sub_key.GetValue("AppData").ToString().Split(Path.DirectorySeparatorChar);
						if(xp)
							add_me.name = split[split.Length-2];
						else 
							add_me.name = split[split.Length-3];
						add_me.app_data = sub_key.GetValue("AppData").ToString();
						add_me.local_app_data = sub_key.GetValue("Local AppData").ToString();
						//add_me.local_settings = sub_key.GetValue("Local Settings").ToString();
						add_me.start_menu = sub_key.GetValue("Start Menu").ToString();
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
						add_me.user_documents = sub_key.GetValue("Personal").ToString();
						add_me.virtual_store = Path.Combine(sub_key.GetValue("AppData").ToString(),"VirtualStore");
                        try {
                            sub_key = Registry.Users.OpenSubKey(user_folder.Name + "\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders");
                            add_me.saved_games = sub_key.GetValue("{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}").ToString();
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
						if(sub_key!=null)
							sub_key.Close();
						RegUnLoadKey(HKEY_USERS,user_folder.Name);		
					} catch {
						if(sub_key!=null)
							sub_key.Close();
						RegUnLoadKey(HKEY_USERS,user_folder.Name);		
						Console.WriteLine("Registry read error.");
					}
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

    public ArrayList getPath(string get_me, bool override_virtualstore) {
        ArrayList return_me = new ArrayList();
        file_holder add_me;
        add_me.owner = null;
        add_me.file_name = null;
        add_me.relative_path = get_me;
        add_me.relative_root = null;
        add_me.absolute_root = null;
        if(get_me.StartsWith("%INSTALLLOCATION%")) {
            string[] chopped = get_me.Split(Path.DirectorySeparatorChar);
            for(int i = 0;i<chopped.Length-1;i++) {
                get_me = chopped[0];
                for(int j=i+1;j<chopped.Length;j++) {
                    get_me = Path.Combine(get_me,chopped[j]);
                }
                return_me.AddRange(getPath("%DRIVE%"+get_me.Substring(17),override_virtualstore));
                return_me.AddRange(getPath("%PROGRAMFILES%"+get_me.Substring(17),override_virtualstore));
                foreach(string drive in alt_paths) {
                    add_me.absolute_path = Path.Combine(drive, get_me.Substring(18));
                    add_me.absolute_root = drive;
                    if (Directory.Exists(add_me.absolute_path)) 
                        return_me.Add(add_me);
                }
            }
        } else if(get_me.StartsWith("%DRIVE%")) {
            add_me.relative_root = "%DRIVE%";
            add_me.relative_path = get_me;
            foreach(string drive in drives) {
                add_me.absolute_path = Path.Combine(drive,get_me.Substring(8));
                add_me.absolute_root = drive;
                if(Directory.Exists(add_me.absolute_path))
                    return_me.Add(add_me);
            }
        } else if(get_me.StartsWith("%PROGRAMFILES%")) {
            add_me.relative_root = "%PROGRAMFILES%";
            // Gotta do this the hard way. If UAC is ebnabled, then only VirtualStore,
            // unless virtualstore override is specified
            if(uac&&!override_virtualstore) {
                if(program_files_x86!=null) {
                    return_me.AddRange(getPath(Path.Combine(Path.Combine("%LOCALAPPDATA%\\VirtualStore", program_files_x86.Substring(3)), get_me.Substring(15)), override_virtualstore));
                }
                return_me.AddRange(getPath(Path.Combine(Path.Combine("%LOCALAPPDATA%\\VirtualStore", program_files.Substring(3)), get_me.Substring(15)), override_virtualstore));
            } else {
                add_me.owner = null;
                if(program_files_x86!=null) {
                    add_me.absolute_root = program_files_x86;
                    add_me.absolute_path = Path.Combine(program_files_x86, get_me.Substring(15));
                    add_me.relative_path = get_me;
                    if (Directory.Exists(add_me.absolute_path))
                        return_me.Add(add_me);
                }
                add_me.absolute_root = program_files;
                add_me.absolute_path = Path.Combine(program_files, get_me.Substring(15));
                add_me.relative_path = get_me;
                if (Directory.Exists(add_me.absolute_path))
                    return_me.Add(add_me);
            }
        } else if(get_me.StartsWith("%USERDOCUMENTS%")) {
            add_me.relative_path = get_me;
            add_me.relative_root = "%USERDOCUMENTS%";
            foreach(KeyValuePair<string,user_data> user in users){
                add_me.owner = user.Key;
                add_me.absolute_root = user.Value.user_documents;
                add_me.absolute_path = Path.Combine(user.Value.user_documents, get_me.Substring(16));
                if (Directory.Exists(add_me.absolute_path))
                {
                    return_me.Add(add_me);
                }
            }
        } else if(get_me.StartsWith("%USERPROFILE%")) {
            add_me.relative_root = "%USERPROFILE%";
            add_me.relative_path = get_me;
            foreach(KeyValuePair<string,user_data> user in users){
                add_me.owner = user.Key;
                add_me.absolute_path = Path.Combine(user.Value.user_dir, get_me.Substring(14));
                add_me.relative_root = user.Value.user_dir;
                if(Directory.Exists(add_me.absolute_path)) {
                    return_me.Add(add_me);
                }
            }
        } else if(get_me.StartsWith("%SAVEDGAMES%")) {
            add_me.relative_root = "%SAVEDGAMES%";
            add_me.relative_path = get_me;
            foreach(KeyValuePair<string,user_data> user in users){
				if(user.Value.saved_games!=null) {
					add_me.owner = user.Key;
                    add_me.absolute_path = Path.Combine(user.Value.saved_games, get_me.Substring(13));
                    add_me.absolute_root = user.Value.app_data;
					if(Directory.Exists(add_me.absolute_path)) {
						return_me.Add(add_me);
					}
				}
            }
        } else if(get_me.StartsWith("%APPDATA%")) {
            add_me.relative_root = "%APPDATA%";
            add_me.relative_path = get_me;
            foreach(KeyValuePair<string,user_data> user in users){
                add_me.owner = user.Key;
                add_me.absolute_path = Path.Combine(user.Value.app_data, get_me.Substring(10));
                add_me.absolute_root = user.Value.app_data;
                if(Directory.Exists(add_me.absolute_path)) {
                    return_me.Add(add_me);
                }
            }
        } else if(get_me.StartsWith("%LOCALAPPDATA%")) {
            add_me.relative_root = "%LOCALAPPDATA%";
            add_me.relative_path = get_me;
            foreach(KeyValuePair<string,user_data> user in users){
                add_me.owner = user.Key;
                add_me.absolute_path = Path.Combine(user.Value.local_app_data, get_me.Substring(15));
                add_me.absolute_root = user.Value.local_app_data;
                if(Directory.Exists(add_me.absolute_path)) {
                    return_me.Add(add_me);
                }
            }
        } else if(get_me.StartsWith("%ALLUSERSPROFILE%")) {
            add_me.relative_root = "%ALLUSERSPROFILE%";
            add_me.relative_path = get_me;
            add_me.absolute_root = all_users_profile;
            add_me.absolute_path = Path.Combine(all_users_profile, get_me.Substring(18));
            if(Directory.Exists(add_me.absolute_path)) {
                return_me.Add(add_me);
            }
        } else if(get_me.StartsWith("%PUBLIC%")) {
            add_me.relative_root = "%PUBLIC%";
            add_me.relative_path = get_me;
            add_me.absolute_root = public_profile;
            add_me.absolute_path = Path.Combine(public_profile,get_me.Substring(9));
            if(Directory.Exists(add_me.absolute_path)) {
                return_me.Add(add_me);
            }
        } else {
            add_me.relative_path = get_me;
            add_me.relative_root = null;
            add_me.absolute_path = get_me;
            add_me.absolute_root = null;
            if(Directory.Exists(add_me.absolute_path))
                return_me.Add(add_me);
        }
        return return_me;
    }

public string parsePath(string parse_me, string with_user) {
		string return_me = parse_me;
        if(parse_me.StartsWith("%ALLUSERSPROFILE%")) {
            return_me = Path.Combine(all_users_profile, parse_me.Substring(18));
        } else if(parse_me.StartsWith("%PUBLIC%")) {
            if(public_profile==null) {
                return_me = Path.Combine(all_users_profile, parse_me.Substring(9));
            } else {
                return_me = Path.Combine(public_profile, parse_me.Substring(9));
            }
        } 
        if(with_user!=null&&users.ContainsKey(with_user)) {
            if(parse_me.StartsWith("%USERDOCUMENTS%")) {
                return_me = Path.Combine(users[with_user].user_documents, parse_me.Substring(16));
            } else if(parse_me.StartsWith("%USERPROFILE%")) {
                return_me = Path.Combine(users[with_user].user_dir, parse_me.Substring(14));
            } else if(parse_me.StartsWith("%SAVEDGAMES%")) {
                return_me = Path.Combine(users[with_user].saved_games, parse_me.Substring(13));
            } else if(parse_me.StartsWith("%APPDATA%")) {
                return_me = Path.Combine(users[with_user].app_data, parse_me.Substring(10));
            } else if(parse_me.StartsWith("%LOCALAPPDATA%")) {
                return_me = Path.Combine(users[with_user].local_app_data, parse_me.Substring(15));
            }
        }
		return return_me;
	}
}
