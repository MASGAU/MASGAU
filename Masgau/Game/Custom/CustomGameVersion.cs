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

            DetectedLocations locs = Core.locations.interpretPath(location.FullName);
            DetectedLocationPathHolder loc = locs.getMostAccurateLocation();

            if (loc.rel_root == Location.EnvironmentVariable.VirtualStore) {
                string drive = Path.GetPathRoot(loc.full_dir_path);
                string new_path = Path.Combine(drive, loc.Path);
                loc = Core.locations.interpretPath(new_path).getMostAccurateLocation();
            }

            switch (loc.rel_root) {
                case EnvironmentVariable.ProgramFiles:
                case EnvironmentVariable.ProgramFilesX86:
                case EnvironmentVariable.Drive:
                    loc.rel_root = EnvironmentVariable.InstallLocation;
                    break;
            }

            this.Locations.Paths.Add(loc);

            FileTypeHolder type = new FileTypeHolder("Custom");
            SaveHolder save = new SaveHolder(saves, null, "Custom");
            if (ignores != null && ignores != "") {
                ExceptHolder except = new ExceptHolder(ignores, null, "Custom");
                save.Excepts.Add(except);
            }
            type.Add(save);
            this.FileTypes.Add("Custom",type);

            if (Core.settings.EmailSender != null)
                this.Contributors.Add(Core.settings.EmailSender);
            else
                this.Contributors.Add("Anonymous");

            this.Detect();
        }


        public CustomGameVersion(Game parent, XmlElement element): base(parent,element) {

        }

    }

}
