using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASGAU.Location.Holders;
namespace MASGAU.Location {
    public class DetectedLocations: List<DetectedLocationPathHolder> {

        public DetectedLocationPathHolder getMostAccurateLocation() {
            if(this.Count==1)
                return this[0];


            DetectedLocationPathHolder candidate = null;
            foreach (DetectedLocationPathHolder path in this) {
                if (candidate == null || candidate.rel_root < path.rel_root)
                    candidate = path;
            }
            return candidate;

        }

    }
}
