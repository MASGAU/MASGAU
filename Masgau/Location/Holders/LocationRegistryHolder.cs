using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASGAU.Registry;

namespace MASGAU.Location.Holders {
    public class LocationRegistryHolder : ALocationHolder {
        // Used when delaing with a registry key
        public RegRoot root = RegRoot.local_machine;
        public string key, value = null;

        public override int CompareTo(AModelItem<StringID> comparable) {
            LocationRegistryHolder location = (LocationRegistryHolder)comparable;
            int result = compare(root, location.root);
            if (result == 0)
                result = compare(key, location.key);
            if (result == 0)
                result = compare(value, location.value);

            return result;
        }
    }
}
