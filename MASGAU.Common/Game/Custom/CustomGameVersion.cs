using System.IO;
using System.Xml;
using GameSaveInfo;
using MASGAU.Location;
using MASGAU.Location.Holders;
namespace MASGAU {
    public class CustomGameVersion : GameVersion {
        public CustomGameVersion(GameSaveInfo.Game parent, DirectoryInfo location, string saves, string ignores)
            : base(parent, "Windows", null, "Custom") {

            DetectedLocations locs = Core.locations.interpretPath(location.FullName).DetectedOnly;
            DetectedLocationPathHolder loc = locs.getMostAccurateLocation();

            if (loc.EV == EnvironmentVariable.VirtualStore) {
                string drive = Path.GetPathRoot(loc.FullDirPath);
                string new_path = Path.Combine(drive, loc.Path);
                loc = Core.locations.interpretPath(new_path).DetectedOnly.getMostAccurateLocation();
            }

            switch (loc.EV) {
                case EnvironmentVariable.ProgramFiles:
                case EnvironmentVariable.ProgramFilesX86:
                case EnvironmentVariable.Drive:
                    loc.EV = EnvironmentVariable.InstallLocation;
                    break;
            }


            LocationPath locpath = new LocationPath(this.Locations, loc.EV, loc.Path);

            this.Locations.Paths.Add(locpath);


            FileType type = this.addFileType("Custom");

            string path, file;
            Include save;

            if (saves != null && ignores != "") {
                path = Path.GetDirectoryName(saves);
                file = Path.GetFileName(saves);
                save = type.addSave(path, file);
            } else {
                save = type.addSave(null, null);
            }

            if (ignores != null && ignores != "") {
                path = Path.GetDirectoryName(ignores);
                file = Path.GetFileName(ignores);
                Exclude except = save.addExclusion(path, file);
            }

            if (Core.settings.EmailSender != null)
                this.Contributors.Add(Core.settings.EmailSender);
            else
                this.Contributors.Add("Anonymous");

        }


        public CustomGameVersion(GameSaveInfo.Game parent, XmlElement element)
            : base(parent, element) {

        }

    }

}
