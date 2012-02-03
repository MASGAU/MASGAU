using System;
using Microsoft.Win32;


class RegistryManager {
    private RegistryKey the_key;

    public RegistryManager(string register_me) {
        the_key = Registry.LocalMachine.OpenSubKey(register_me);
        if (the_key==null) {
            the_key = Registry.LocalMachine.OpenSubKey(register_me.Replace("SOFTWARE","Software\\Wow6432Node"));
        }
    }

    public string getValue(string get_me) {
        if (the_key != null && the_key.GetValue(get_me)!=null)
            return the_key.GetValue(get_me).ToString();
        return null;
    }
}
