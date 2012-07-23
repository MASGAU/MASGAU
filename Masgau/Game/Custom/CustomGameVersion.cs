using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using MASGAU.Location;
using MASGAU.Location.Holders;
namespace MASGAU {
    public class CustomGameVersion: MASGAU.GameVersion {


        public CustomGameVersion(Game parent, DirectoryInfo location, string saves, string ignores): base(parent) {
            this.id = new GameID(parent.Name,"Custom");

            DetectedLocations loc = Core.locations.interpretPath(location.FullName);

            this.Locations.Paths.Add(loc.getMostAccurateLocation());

            FileTypeHolder type = new FileTypeHolder("Custom");
            SaveHolder save = new SaveHolder(saves, null, "Custom");
            if (ignores != null && ignores != "") {
                ExceptHolder except = new ExceptHolder(ignores, null, "Custom");
                save.Excepts.Add(except);
            }
            type.Add(save);
            this.FileTypes.Add("Custom",type);

            if (Core.settings.email != null)
                this.Contributors.Add(Core.settings.email);
            else
                this.Contributors.Add("Anonymous");

            this.Detect();
        }


        public CustomGameVersion(Game parent, XmlElement element): base(parent,element) {

        }

    }

}
