using System;
using MASGAU.Registry;
using MVC;
namespace MASGAU {
    public class StartupHelper : ANotifyingObject {

        RegistryHandler reg;
        private string name, program;
        public StartupHelper(string name, string program) {
            this.name = name;
            this.program = program;
            reg = new RegistryHandler("current_user", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        }

        public bool IsEnabled {
            get {
                return !String.IsNullOrEmpty(reg.getValue(name));
            }
            set {
                if (value) {
                    if (!reg.setValue(name, program))
                        throw new Translator.TranslateableException("AutoStartEnableError");
                } else {
                    if (!String.IsNullOrEmpty(reg.getValue(name))) {
                        reg.deleteValue(name);
                    }
                }
                NotifyPropertyChanged("IsEnabled");
            }
        }
    }
}
