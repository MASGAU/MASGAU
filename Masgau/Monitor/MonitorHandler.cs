using System.IO;
using System;
using MASGAU.Registry;
using MVC;
using Communication;
using Translator;
namespace MASGAU.Monitor
{
    public class MonitorHandler: AModelItem
    {
        private bool monitor_found = false;
        private bool monitor_enabled = false;
        private string monitor_path = null;
        
        
        public MonitorHandler(): base("") {
            if(!Core.all_users_mode) {
                if(File.Exists(Core.programs.monitor)) {
                    monitor_found = true;
                    monitor_path = Core.programs.monitor;
                } else {
                    monitor_found = false;
                }
            } else {
                monitor_found = false;
            }
            RegistryHandler reg = new RegistryHandler(RegRoot.current_user,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",false);
            if (reg.getValue("MASGAUMonitor")!=null)
                monitor_enabled = true;
            else
                monitor_enabled = false;

        }


        public bool available {
            get {
                return monitor_found;
            }
        }

        public bool enabled {
            get {
                return monitor_enabled;
            }
            set{
                RegistryHandler reg = new RegistryHandler(RegRoot.current_user,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
                if(value) {
                    if(monitor_found) {
                        reg.setValue("MASGAUMonitor", monitor_path);
                        monitor_enabled = true;
                    } else {
                        throw new TranslateableException("MonitorNotFound");
                    }
                } else {
                    if (reg.getValue("MASGAUMonitor")!=null) {
                        reg.deleteValue("MASGAUMonitor");
                        monitor_enabled = false;
                    }
                }

                NotifyPropertyChanged("enabled");
            }
        }
    }
}
