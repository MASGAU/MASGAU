using MASGAU.Registry;
using MVC;
namespace MASGAU {
    public abstract class AStartupHelper : ANotifyingObject {

        RegistryHandler reg;
        private string name, program;
        public AStartupHelper(string name, string program) {
            this.name = name;
            this.program = program;
            reg = new RegistryHandler("current_user", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        }

        public bool IsEnabled {
            get {
                return reg.getValue(name) != null;
            }
            set {
                if (value) {
                    if (!reg.setValue(name, program))
                        throw new Translator.TranslateableException("AutoStartEnableError");
                } else {
                    if (reg.getValue(name) != null) {
                        reg.deleteValue(name);
                    }
                }
                NotifyPropertyChanged("IsEnabled");
            }
        }
    }
}
