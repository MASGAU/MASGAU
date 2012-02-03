using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MASGAU.Location.Holders {
    public class LocationPathHolder : ALocationHolder {
        // Used when dealing with a path
        // ONLY holds the name of the environment variable or wahtever used to figure out the root
        public EnvironmentVariable rel_root;

        // Holds only the relative path from the root
        public string path;

        public LocationPathHolder()
        { }

        public LocationPathHolder(LocationPathHolder copy_me)
            : base(copy_me) {
            rel_root = copy_me.rel_root;
            path = copy_me.path;
        }

        public override int CompareTo(AModelItem<StringID> comparable) {
            LocationPathHolder location = (LocationPathHolder)comparable;
            int result = compare(this.rel_root, location.rel_root);
            if (result == 0)
                result = compare(this.path, location.path);

            return result;
        }


        public override string ToString() {
            return Path.Combine(rel_root.ToString(), path);
        }


    }
}
