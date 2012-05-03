using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
namespace Email
{
    public abstract class AEmailConfig: ConfigFileHandler
    {
        protected AEmailConfig(string new_file_path, string new_file_name, System.Threading.Mutex mutex): 
            base(new_file_path, new_file_name, mutex) {
                shared_settings.Add("email");

        }
        protected AEmailConfig(string new_file_path, string new_file_name)
                    : base(new_file_path, new_file_name)
        {
            shared_settings.Add("email");

        }

        public string email
        {
            get
            {
                return getNodeAttribute("address", "email");
            }
            set
            {
                if (value != null && value.Contains("@"))
                {
                    int loc = value.IndexOf('@');
                    if (value.Substring(loc + 1).Contains("."))
                        setNodeAttribute("address", value, "email");
                }
                NotifyPropertyChanged("email");
            }
        }


    }
}
