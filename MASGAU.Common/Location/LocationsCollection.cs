using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameSaveInfo;
using MASGAU.Location.Holders;

namespace MASGAU.Location {
    public class LocationsCollection : Dictionary<string, LocationPath>, IEnumerable<LocationPath> {

        public LocationPath getMostAccurateLocation() {
            LocationPath candidate = null;
            foreach (LocationPath path in this.Values) {
                if (candidate == null || candidate.EV < path.EV || (path is DetectedLocationPathHolder && !(candidate is DetectedLocationPathHolder)))
                    candidate = path;
            }
            return candidate;
        }

        public void Add(LocationPath path) {
            // This compares the environment variables to ensure that the most accurate location gets used when the same path is entered twice
            string key;
            if (path is DetectedLocationPathHolder) {
                key = ((DetectedLocationPathHolder)path).FullDirPath;
            } else {
                key = path.ToString();
            }

            if (this.ContainsKey(key)) {
                LocationPath other = this[key];
                if (path.EV > other.EV)
                    this[key] = path;
            } else {
                base.Add(key, path);
            }
        }

        public new void Add(string name, LocationPath path) {
            throw new NotImplementedException("Use single-value add instead");
        }


        public void AddRange(IEnumerable<LocationPath> items) {
            foreach (LocationPath item in items) {
                this.Add(item);
            }

        }

        public new IEnumerator<LocationPath> GetEnumerator() {
            return this.Values.GetEnumerator();
        }

        public DetectedLocations DetectedOnly {
            get {
                DetectedLocations return_me = new DetectedLocations(this);
                return return_me;
            }
        }

    }
}
