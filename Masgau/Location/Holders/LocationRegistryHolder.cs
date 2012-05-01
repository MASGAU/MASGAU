using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MASGAU.Registry;

namespace MASGAU.Location.Holders {
    public class LocationRegistryHolder : ALocationHolder {
        // Used when delaing with a registry key
        public RegRoot root = RegRoot.local_machine;
        public string key, value = null;


        public LocationRegistryHolder(XmlElement element): base(element)
        {

            this.root = parseRegRoot(element.GetAttribute("root"));
            this.key = element.GetAttribute("key");
            if (element.HasAttribute("value"))
                this.value = element.GetAttribute("value");
            else
                this.value = null;


        }
        public override int CompareTo(AModelItem<StringID> comparable) {
            LocationRegistryHolder location = (LocationRegistryHolder)comparable;
            int result = compare(root, location.root);
            if (result == 0)
                result = compare(key, location.key);
            if (result == 0)
                result = compare(value, location.value);

            return result;
        }

        public RegRoot parseRegRoot(string parse_me)
        {
            switch (parse_me.ToLower())
            {
                case "classes_root":
                    return RegRoot.classes_root;
                case "current_user":
                    return RegRoot.current_user;
                case "current_config":
                    return RegRoot.current_config;
                case "dyn_data":
                    return RegRoot.dyn_data;
                case "local_machine":
                    return RegRoot.local_machine;
                case "performance_data":
                    return RegRoot.performace_data;
                case "users":
                    return RegRoot.users;
            }
            throw new NotImplementedException("The specified key root in " + parse_me + " is not recognized. You either spelled it wrong or something.");
        }
    }
}
