using System;
using Microsoft.Win32;



class RegistryManager {
    private RegistryKey the_key;
    private RegistryKey root_key;
    public bool key_found;

    public RegistryManager(RegistryHive look_here, string register_me, bool writable) {
        key_found = false;

        root_key = RegistryKey.OpenBaseKey(look_here, RegistryView.Registry64);
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

    ~RegistryManager() {
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
