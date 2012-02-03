using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;

namespace MASGAU.Registry {
    public class RegistryHandler {
        private RegistryKey the_key;
        private RegistryKey root_key;
        public bool key_found;

        public RegistryHandler(int hKey) {
            key_found = false;
            SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle(new IntPtr(hKey), true);
            root_key = RegistryKey.FromHandle(safeRegistryHandle);
        }
        public RegistryHandler(RegRoot look_here, string register_me, bool writable) {
            key_found = false;
            switch(look_here) {
                case RegRoot.classes_root:
                    root_key = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64);
                    break;
                case RegRoot.current_config:
                    root_key = RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry64);
                    break;
                case RegRoot.current_user:
                    root_key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                    break;
                case RegRoot.dyn_data:
                    root_key = RegistryKey.OpenBaseKey(RegistryHive.DynData, RegistryView.Registry64);
                    break;
                case RegRoot.local_machine:
                    root_key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    break;
                case RegRoot.performace_data:
                    root_key = RegistryKey.OpenBaseKey(RegistryHive.PerformanceData, RegistryView.Registry64);
                    break;
                case RegRoot.users:
                    root_key = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64);
                    break;
                default:
                    root_key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    break;
            }
            //root_key = RegistryKey.OpenRemoteBaseKey(look_here, RegistryView.Registry64);

			if(register_me!=null) {
                the_key = root_key.OpenSubKey(register_me,writable);        

                if (the_key==null) {
                    the_key = root_key.OpenSubKey(register_me.Replace("SOFTWARE","Software\\Wow6432Node"),writable);
                }

                if (the_key!=null) {
                    key_found=true;
                }
            } else {
                key_found = false;
            }
        }

        ~RegistryHandler() {
            if(the_key!=null)
                the_key.Close();
            if(root_key!=null)
                root_key.Close();
        }


        public void close() {
            if(the_key!=null)
                the_key.Close();
            if(root_key!=null)
                root_key.Close();
        }
    
    
        public string getValue(string get_me) {
            if (the_key!=null) {
                if(get_me==null&&the_key.GetValue("")!=null) {
                    return the_key.GetValue("").ToString();
                } else if(the_key.GetValue(get_me)!=null) {
                    return the_key.GetValue(get_me).ToString();
                } else {
                    return null;
                }
            } else{
                return null;
            }
        }

        public bool setValue(string set_me, object to_me) {
            try {
                if(the_key!=null) {
                    the_key.SetValue(set_me,to_me);
                    return true;
                } else {
                    return false;
                }
            } catch {
                return false;
            }
        }

        public bool deleteValue(string delete_me) {
            try {
                if(the_key!=null) {
                    the_key.DeleteValue(delete_me,false);
                    return true;
                } else {
                    return false;
                }
            } catch {
                return false;
            }
        }

        public bool deleteKey() {
            try {
                if(the_key!=null) {
                    return false;
                } else {
                    return false;
                }
            } catch {
                return false;
            }
        }

    }
}