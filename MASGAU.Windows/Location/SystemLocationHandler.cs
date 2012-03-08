using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using MASGAU.Registry;
using MASGAU.Location.Holders;
using Translations;
using Microsoft.Win32;

namespace MASGAU.Location {
    public class SystemLocationHandler: ASystemLocationHandler {        
        //public string user_root, host_name, program_files, program_files_x86, common_program_files, all_users_profile, public_profile;
        private bool xp = false;

	    [StructLayout(LayoutKind.Sequential)]
	    public struct LUID
	    {
	    public UInt32 LowPart;
	    public Int32 HighPart;
	    } 

	    [StructLayout(LayoutKind.Sequential)]
	    public struct TOKEN_PRIVILEGES
	    {
    	    public uint PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privileges;
            public int Size() {
                return Marshal.SizeOf(this);
            }
	    }

	    [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES {
            public LUID luid;
            public UInt32 attributes;
        }
	    [DllImport("advapi32.dll", CharSet=CharSet.Ansi)]
	    public static extern int OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, 
	    ref IntPtr tokenhandle);

	    [DllImport("kernel32.dll", CharSet=CharSet.Ansi)]
	    public static extern int GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet=CharSet.Ansi, SetLastError=true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

	    [DllImport("advapi32.dll", CharSet=CharSet.Ansi)]
	    public static extern int LookupPrivilegeValue(string lpsystemname, string lpname, 
	    [MarshalAs(UnmanagedType.Struct)] ref LUID lpLuid);

	    [DllImport("advapi32.dll", CharSet=CharSet.Ansi)]
	    public static extern int AdjustTokenPrivileges(IntPtr tokenhandle, int disableprivs, 
	    [MarshalAs(UnmanagedType.Struct)]ref TOKEN_PRIVILEGES Newstate, int bufferlength, 
	    ref TOKEN_PRIVILEGES PreviousState, ref int Returnlength);

	    [DllImport("advapi32.dll", CharSet=CharSet.Ansi, SetLastError=true)]
	    public static extern int RegLoadKey(uint hKey,string lpSubKey, string lpFile);

        [Flags]
        public enum RegSAM
        {
            AllAccess = 0x000f003f
        }
        private const int REG_PROCESS_APPKEY = 0x00000001;
        [DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
	    private static extern int RegLoadAppKey(String hiveFile, out int hKey, RegSAM samDesired, int options, int reserved);

	    [DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
	    public static extern int RegUnLoadKey(uint hKey, string lpSubKey);

        public const uint    TOKEN_ADJUST_PRIVILEGES = 0x00000020;
	    public const uint    TOKEN_QUERY = 0x00000008;
	    public const uint    SE_PRIVILEGE_ENABLED = 0x00000002;
	    public const string SE_RESTORE_NAME = "SeRestorePrivilege";
	    public const string SE_BACKUP_NAME = "SeBackupPrivilege";
	    public const uint   HKEY_USERS = 0x80000003;
	    public string       shortname;

        private bool initialized = false;
	
	    public SystemLocationHandler(): base() {
            // The global variables
            if(Environment.GetEnvironmentVariable("ProgramW6432")!=null) {
                global.setEvFolder(EnvironmentVariable.ProgramFiles,Environment.GetEnvironmentVariable("ProgramW6432"));
                global.setEvFolder(EnvironmentVariable.ProgramFilesX86,Environment.GetEnvironmentVariable("PROGRAMFILES(X86)"));
            } else {
                global.setEvFolder(EnvironmentVariable.ProgramFiles,Environment.GetEnvironmentVariable("PROGRAMFILES"));
                //global.setEvFolder(EnvironmentVariable.ProgramFilesX86,Environment.GetEnvironmentVariable("PROGRAMFILES"));
            }
            if (Environment.GetEnvironmentVariable("LOCALAPPDATA") != null){
                xp = false;
            } else {
                xp = true;
            }
	        if(xp)
                platform_version = PlatformVersion.XP;
	        else
                platform_version = PlatformVersion.Vista;

            global.setEvFolder(EnvironmentVariable.StartMenu, Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));

            // Not really used
            //common_program_files = Environment.GetEnvironmentVariable("COMMONPROGRAMFILES");

            foreach(DriveInfo look_here in DriveInfo.GetDrives()) {
                if(look_here.IsReady&&(look_here.DriveType==DriveType.Fixed||look_here.DriveType==DriveType.Removable)) {
                    drives.Add(look_here.Name);
                }
            }

            
            //host_name = Environment.GetEnvironmentVariable("COMPUTERNAME");
            global.setEvFolder(EnvironmentVariable.AllUsersProfile,Environment.GetEnvironmentVariable("ALLUSERSPROFILE"));
            if(platform_version== PlatformVersion.Vista)
                global.setEvFolder(EnvironmentVariable.Public,Environment.GetEnvironmentVariable("PUBLIC"));

		    if(!xp) {
			    RegistryHandler uac_status = new RegistryHandler(RegRoot.local_machine,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",false);	
			    if(uac_status.getValue("EnableLUA")=="1") {
				    uac_enabled = true;
			    }
		    }
            string[] split = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Split(Path.DirectorySeparatorChar);

            StringBuilder user_root = new StringBuilder(split[0]);
		    if(xp) {
			    for(int i = 1;i<split.Length-2;i++) {
				    user_root.Append(Path.DirectorySeparatorChar + split[i]);
			    }
		    } else {
			    for(int i = 1;i<split.Length-3;i++) {
				    user_root.Append(Path.DirectorySeparatorChar + split[i]);
			    }
		    }
           
            //Per-user variables
            loadUsersData(RegRoot.current_user,null);

            if(Core.all_users_mode) {
                // All this crap lets me get data from other user's registries
			    IntPtr token = new IntPtr(0);
			    int retval=0;

			    TOKEN_PRIVILEGES TP = new TOKEN_PRIVILEGES();
			    TOKEN_PRIVILEGES TP2 = new TOKEN_PRIVILEGES();
			    LUID RestoreLuid = new LUID();
			    LUID BackupLuid = new LUID();

                int return_length = 0;
                TOKEN_PRIVILEGES oldPriveleges = new TOKEN_PRIVILEGES();

                System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
                //IntPtr hndle = GetModuleHandle(null);

			    //retval = OpenProcessToken(hndle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref token);
			    retval = OpenProcessToken(process.Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref token);

                retval = LookupPrivilegeValue(null, SE_RESTORE_NAME, ref RestoreLuid);
			    TP.PrivilegeCount = 1;
			    TP.Privileges.attributes = SE_PRIVILEGE_ENABLED;
			    TP.Privileges.luid = RestoreLuid;
			    retval = AdjustTokenPrivileges(token, 0, ref TP, TP.Size(),ref oldPriveleges,ref return_length);
                if(retval==0)
                    throw new MException("User Load Error","Error while trying to change process restore permission for reading other users", "Error code " + retval, true);

			    retval = LookupPrivilegeValue(null, SE_BACKUP_NAME, ref BackupLuid);
			    TP2.PrivilegeCount = 1;
			    TP2.Privileges.attributes = SE_PRIVILEGE_ENABLED;
			    TP2.Privileges.luid = BackupLuid;
			    retval = AdjustTokenPrivileges(token, 0, ref TP2, TP2.Size(),ref oldPriveleges,ref return_length);
                if(retval==0)
                    throw new MException("User Load Error","Error while trying to change process backup permission for reading other users","Error code " + retval, true);
			
			
                Console.WriteLine(retval);

                foreach(DirectoryInfo user_folder in new DirectoryInfo(user_root.ToString()).GetDirectories()) {
				    if(user_folder.Name.ToLower()=="default user")
                        continue;
                    if(user_folder.Name.ToLower()=="default")
                        continue;
                    if(user_folder.Name.ToLower()=="all users")
                        continue;

                    string hive_file = Path.Combine(user_folder.FullName,"NTUSER.DAT");
                    if(!File.Exists(hive_file))
                        continue;


                    int h = RegLoadKey(HKEY_USERS,user_folder.Name,hive_file);
                    //int h = RegLoadAppKey(hive_file,out hKey, RegSAM.AllAccess,REG_PROCESS_APPKEY,0);

                    if(h==32)
                        continue;

                    if(h!=0)
                        throw new MException("User Load Error","Error while trying to load the registry file for user " + user_folder.Name,"Error code " + h, true);

                    //sub_key = new RegistryHandler(hKey);
                    //sub_key = new RegistryHandler(RegRoot.users,user_folder.Name,false);
                    loadUsersData(RegRoot.users,user_folder.Name);
				    string result = RegUnLoadKey(HKEY_USERS,user_folder.Name).ToString();	
				}
            }

            initialized = true;
        }

        private void loadUsersData(RegRoot reg_root, string path) {
            RegistryHandler user_key;
            if(path==null)
                user_key = new RegistryHandler(reg_root,@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders",false);
            else
                user_key = new RegistryHandler(reg_root,path + @"\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders",false);

            UserData add_me;
            string[] split ;
            try {
                split = user_key.getValue("AppData").Split(Path.DirectorySeparatorChar);
                if(xp)
                    add_me = new UserData(split[split.Length-2]);
			    else 
                    add_me = new UserData(split[split.Length-3]);

                add_me.setEvFolder(EnvironmentVariable.AppData,user_key.getValue("AppData"));

                add_me.setEvFolder(EnvironmentVariable.Desktop, user_key.getValue("Desktop"));
                add_me.setEvFolder(EnvironmentVariable.StartMenu, user_key.getValue("Start Menu"));

                DirectoryInfo flash_share = new DirectoryInfo(Path.Combine(add_me.getFolder(EnvironmentVariable.AppData), @"Macromedia\Flash Player\#SharedObjects"));
                if(flash_share.Exists) {
                    DirectoryInfo[] flash_users = flash_share.GetDirectories();
                    switch(flash_users.Length) {
                        case 0:
                            break;
                        case 1:
                            add_me.setEvFolder(EnvironmentVariable.FlashShared,flash_users[0].FullName);
                            break;
                        default:
                            add_me.setEvFolder(EnvironmentVariable.FlashShared,flash_users[0].FullName);
                            break;
                    }
                }

                add_me.setEvFolder(EnvironmentVariable.LocalAppData,user_key.getValue("Local AppData"));





                //add_me.local_settings = user_key.GetValue("Local Settings").ToString();
                //add_me.start_menu = user_key.getValue("Start Menu");

                StringBuilder user_dir = new StringBuilder(split[0]);
			    if(xp) {
				    for(int i = 1;i<split.Length-1;i++) {
					    user_dir.Append(Path.DirectorySeparatorChar + split[i]);
				    }
			    } else {
				    for(int i = 1;i<split.Length-2;i++) {
					    user_dir.Append(Path.DirectorySeparatorChar + split[i]);
				    }
                }
                add_me.setEvFolder(EnvironmentVariable.UserProfile,user_dir.ToString());
                add_me.setEvFolder(EnvironmentVariable.UserDocuments,user_key.getValue("Personal"));

                if(platform_version== PlatformVersion.Vista)
                    add_me.setEvFolder(EnvironmentVariable.VirtualStore,Path.Combine(user_key.getValue("Local AppData"),"VirtualStore"));

                user_key.close();

                if(path==null)
                    user_key = new RegistryHandler(reg_root,@"Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders",false);
                else
                    user_key = new RegistryHandler(reg_root,path + @"\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders",false);

                string saved_games = user_key.getValue("{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}");
                if(saved_games!=null) {
                    if(saved_games.StartsWith("%USERPROFILE%")) {
                        saved_games = Path.Combine(user_dir.ToString(),saved_games.Substring(14));
                    }
                    add_me.setEvFolder(EnvironmentVariable.SavedGames,saved_games);
                } else {
                    if(Directory.Exists(Path.Combine(user_dir.ToString(), "Saved Games"))) {
                        add_me.setEvFolder(EnvironmentVariable.SavedGames,Path.Combine(user_dir.ToString(), "Saved Games"));
                    }
                }
                this.Add(add_me);
                user_key.close();
            } catch {
                return;
            }
        }



        public bool is_xp {
            get {
                return xp;
            }
        }
        public bool x64 {
            get {
                return global.hasFolderFor(EnvironmentVariable.ProgramFilesX86);
            }
        }
        public override bool ready {
            get {
                return initialized;
            }
        }

        protected override List<DetectedLocationPathHolder> getPaths(LocationPathHolder get_me) {
            //if(get_me.rel_root!= EnvironmentVariable.Public)
               // return new List<DetectedLocationPathHolder>();
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            DetectedLocationPathHolder add_me;
            DirectoryInfo test;
            switch(get_me.rel_root) {
                case EnvironmentVariable.InstallLocation:
                    LocationPathHolder temp = new LocationPathHolder(get_me);
                    string[] chopped = temp.path.Split(Path.DirectorySeparatorChar);
                    for(int i = 0;i<chopped.Length;i++) {
                        temp.path = chopped[i];
                        for(int j=i+1;j<chopped.Length;j++) {
                            temp.path = Path.Combine(temp.path,chopped[j]);
                        }
                        temp.rel_root = EnvironmentVariable.ProgramFiles;
                        return_me.AddRange(getPaths(temp));
                    }
                    return_me.AddRange(base.getPaths(get_me));
                    break;
                case EnvironmentVariable.ProgramFiles:
                case EnvironmentVariable.ProgramFilesX86:
                    // Always checks both the VirtualStore and the real Program Files,
                    // to make sure nothing is missed, especially in the case of old games
                    // that may or may not use the VirtualStore
                    if(!get_me.override_virtual_store&&platform_version== PlatformVersion.Vista) {
                        LocationPathHolder virtualstore_info = new LocationPathHolder(get_me);

                        virtualstore_info.rel_root = EnvironmentVariable.LocalAppData;
                        if(x64) {
                            virtualstore_info.path = Path.Combine("VirtualStore", global.getFolder(EnvironmentVariable.ProgramFilesX86).Substring(3), virtualstore_info.path);
                            return_me.AddRange(getPaths(virtualstore_info));
                            virtualstore_info = new LocationPathHolder(get_me);
                            virtualstore_info.rel_root = EnvironmentVariable.LocalAppData;
                        }
                        virtualstore_info.path = Path.Combine("VirtualStore", global.getFolder(EnvironmentVariable.ProgramFiles).Substring(3), virtualstore_info.path);
                        return_me.AddRange(getPaths(virtualstore_info));
                    }

                    add_me  = new DetectedLocationPathHolder(get_me);
                    if(x64) {
                        if(get_me.path!=null&&get_me.path.Length>0)
                            test = new DirectoryInfo(Path.Combine(global.getFolder(EnvironmentVariable.ProgramFilesX86), get_me.path));
                        else
                            test = new DirectoryInfo(global.getFolder(EnvironmentVariable.ProgramFilesX86));
                        if (test.Exists) {
                            add_me.abs_root = global.getFolder(EnvironmentVariable.ProgramFilesX86);
                            return_me.Add(add_me);
                        }
                    }

                    if(get_me.path!=null&&get_me.path.Length>0)
                        test = new DirectoryInfo(Path.Combine(global.getFolder(EnvironmentVariable.ProgramFiles), get_me.path));
                    else
                        test = new DirectoryInfo(global.getFolder(EnvironmentVariable.ProgramFiles));

                    if (test.Exists) {
                        add_me.abs_root= global.getFolder(EnvironmentVariable.ProgramFiles);
                        return_me.Add(add_me);
                    }
                    break;
                default:
                    return base.getPaths(get_me);
            }
            return return_me;
        }


        protected override List<DetectedLocationPathHolder> getPaths(LocationRegistryHolder get_me) {
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();

            RegistryHandler reg;

			// This handles if the root is a registry key
			if(get_me.key!=null) {
                reg = new RegistryHandler(get_me.root,get_me.key,false);
                
				if(reg.key_found) {
					try {
                        string path;
                        if(get_me.value==null)
							path = reg.getValue("");
                        else
							path = reg.getValue(get_me.value);

						if(path!=null){
                            if(path.Contains("/"))
                                path = path.Split('/')[0].Trim();

                            if(path.Contains("\""))
                                path = path.Trim('\"').Trim();

                            if(Path.HasExtension(path))
                                path = Path.GetDirectoryName(path);

							path = get_me.modifyPath(path);
                            if(Directory.Exists(path)) {
								return_me.AddRange(Core.locations.interpretPath(new DirectoryInfo(path).FullName));
                            }
						}
					} catch (Exception e) {
                        throw new MException(Strings.get("RegistryKeyLoadError"), e.Message, e, false);
					}
				}
			}
            return return_me;
        }

        // This is something...
        const int CSIDL_COMMON_STARTMENU = 0x0016;

        [DllImport("shell32.dll")]
        static extern bool SHGetSpecialFolderPath(
        IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);


        protected override List<DetectedLocationPathHolder> getPaths(LocationShortcutHolder get_me) {
            FileInfo the_shortcut;
            //StringBuilder start_menu;
			List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            String path;

            List<string> paths = this.getPaths(get_me.ev);
            the_shortcut = null;

            foreach (string check_me in paths)
            {
                the_shortcut = new FileInfo(Path.Combine(check_me, get_me.path));
                if (the_shortcut.Exists)
                    break;
            }

            

            if(the_shortcut!=null && the_shortcut.Exists) {
			    IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
			    IWshRuntimeLibrary.IWshShortcut link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(the_shortcut.FullName);
	
	            try
	            {
	                path = Path.GetDirectoryName(link.TargetPath);
	                path = get_me.modifyPath(path);
                    return_me.AddRange(Core.locations.interpretPath(path));
	            }
	            catch { }
		    }
			return return_me;
		}
    }
}