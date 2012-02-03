using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Masgau
{
    class WebBrowser
    {
        object temp;
        string browser_path;
        bool browser_found = false;

        public WebBrowser() {
            //try {
                RegistryKey browserKey;
                if (Environment.GetEnvironmentVariable("LOCALAPPDATA") != null){
                    browserKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http", false);
                    temp = browserKey.GetValue(null);
                    if(temp==null) {
                        browserKey = Registry.CurrentUser.OpenSubKey(@"Software\Classes\http\shell\open\command");
                        temp = browserKey.GetValue(null);
                    }
                } else {
                    browserKey = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command", false);
                    temp = browserKey.GetValue(null);
                }
                if(temp!=null) {
                    browser_path = temp.ToString().Split('\"')[1];
                    browser_found = true;
                }
            //} catch {}
        }

        public bool openBrowser(string url) {
            if(browser_found) {
                System.Diagnostics.Process.Start(browser_path, url);
                return true;
            } else {
                return false;
            }
        }

    }

}
