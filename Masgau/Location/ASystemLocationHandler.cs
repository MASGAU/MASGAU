using System.Collections.Generic;
using System.IO;
using MASGAU.Location.Holders;

namespace MASGAU.Location {
    public abstract class ASystemLocationHandler : ALocationHandler {
        public bool uac_enabled;

        public string platform_version;

        protected List<string> drives = new List<string>();

        public ASystemLocationHandler()
            : base(HandlerType.System) {

        }

        public override List<DetectedLocationPathHolder> interpretPath(string interpret_me) {
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            LocationPathHolder new_location;
            if (ready) {
                return_me.AddRange(base.interpretPath(interpret_me));
                if (return_me.Count == 0) {
                    foreach (string drive in drives) {
                        if (interpret_me.StartsWith(drive)) {
                            new_location = new LocationPathHolder();
                            new_location.rel_root = EnvironmentVariable.Drive;
                            if (interpret_me.Length == drive.Length)
                                new_location.Path = "";
                            else
                                new_location.Path = interpret_me.Substring(drive.Length);
                            return_me.AddRange(getPaths(new_location));
                        }
                    }
                }
            }
            return return_me;
        }



        protected override DetectedLocations getPaths(LocationPathHolder get_me) {
            DetectedLocations return_me = new DetectedLocations();
            DirectoryInfo test;
            DetectedLocationPathHolder add_me;
            switch (get_me.rel_root) {
                case EnvironmentVariable.InstallLocation:
                    LocationPathHolder temp = new LocationPathHolder(get_me);
                    string[] chopped = temp.Path.Split(Path.DirectorySeparatorChar);
                    for (int i = 0; i < chopped.Length; i++) {
                        temp.Path = chopped[i];
                        for (int j = i + 1; j < chopped.Length; j++) {
                            temp.Path = Path.Combine(temp.Path, chopped[j]);
                        }
                        temp.rel_root = EnvironmentVariable.Drive;
                        return_me.AddRange(getPaths(temp));
                        temp.rel_root = EnvironmentVariable.AltSavePaths;
                        return_me.AddRange(getPaths(temp));
                    }
                    break;
                case EnvironmentVariable.AltSavePaths:
                    foreach (AltPathHolder alt_path in Core.settings.save_paths) {
                        if (PermissionsHelper.isReadable(alt_path.path)) {
                            if (get_me.Path != null && get_me.Path.Length > 0)
                                test = new DirectoryInfo(Path.Combine(alt_path.path, get_me.Path));
                            else
                                test = new DirectoryInfo(alt_path.path);
                            if (test.Exists) {
                                add_me = new DetectedLocationPathHolder(get_me);
                                add_me.AbsoluteRoot = alt_path.path;
                                return_me.Add(add_me);
                            }
                        }
                    }
                    break;
                case EnvironmentVariable.Drive:
                    foreach (string drive in drives) {
                        if (get_me.Path != null && get_me.Path.Length > 0)
                            test = new DirectoryInfo(Path.Combine(drive, get_me.Path));
                        else
                            test = new DirectoryInfo(drive);
                        if (test.Exists) {
                            add_me = new DetectedLocationPathHolder(get_me);
                            add_me.AbsoluteRoot = drive;
                            return_me.Add(add_me);
                        }
                    }
                    break;
                default:
                    return base.getPaths(get_me);
            }
            return return_me;
        }

        public override string getAbsoluteRoot(LocationPathHolder parse_me, string user) {
            switch (parse_me.rel_root) {
                case EnvironmentVariable.Drive:
                    DetectedLocationPathHolder holder = (DetectedLocationPathHolder)parse_me;
                    return holder.AbsoluteRoot;
                default:
                    return base.getAbsoluteRoot(parse_me, user);
            }

        }

    }
}
