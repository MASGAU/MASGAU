using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace MASGAU
{
    public struct gfw_user {
        public string name, system_user, user_hex, machine_hex, account_path;
    }

    public class gfwLiveHandler
    {
        //HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Software\Microsoft\XLive\DashDir
        //GFWLive.exe
        public string   install_path;
        //private string  user_path;
        public bool     installed = false;
        public Dictionary<string,gfw_user> users = new Dictionary<string,gfw_user>();

        public gfwLiveHandler() {
            RegistryHandler live = new RegistryHandler(RegRoot.local_machine,@"SOFTWARE\Classes\Software\Microsoft\XLive",false);
            if (live.key_found){
                install_path = live.getValue("DashDir").TrimEnd('\\');
                if (Directory.Exists(install_path)){
                    if (File.Exists(Path.Combine(install_path, "GFWLive.exe"))){
                        installed = true;
                        DirectoryInfo data_dir, user_dir;
                        gfw_user new_user;
                        foreach(KeyValuePair<string,user_data> user in WindowsLocationHandler.users) {
                            new_user.system_user = user.Value.name;
                            data_dir = new DirectoryInfo(Path.Combine(user.Value.local_app_data,"Microsoft\\XLive\\Content"));
                            if(data_dir.Exists) {
                                foreach(DirectoryInfo directory in data_dir.GetDirectories()) {
                                    new_user.user_hex = directory.Name.Substring(0,8);
                                    new_user.machine_hex = directory.Name.Substring(8,8);
                                    new_user.account_path = directory.FullName;
                                    new_user.name = "Names not supported";
                                    //users.Add(directory.Name,new_user);
                                    user_dir = new DirectoryInfo(Path.Combine(directory.FullName,"FFFE07D1\\00010000\\" + directory.Name + "_MountPt"));
                                }
                            }
                        }
                    }else{
                        install_path = null;
                    }
                }else{
                    install_path = null;
                }
            }
        }

        public bool backup_account(string place_here) {
            return false;
        }
    }
}
