using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MASGAU.Location.Holders;

namespace MASGAU.Location
{
    public abstract class ASystemLocationHandler: ALocationHandler
    {
        public bool             uac_enabled;

        public PlatformVersion platform_version;

        protected List<string> drives = new List<string>();

        public ASystemLocationHandler(): base(HandlerType.System) {

        }

        public override List<DetectedLocationPathHolder> interpretPath(string interpret_me) {
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            LocationPathHolder new_location;
            if(ready) {
                return_me.AddRange(base.interpretPath(interpret_me));
                if(return_me.Count==0) {
                    foreach(string drive in drives) {
                        if(interpret_me.StartsWith(drive)) {
                            new_location = new LocationPathHolder();
                            new_location.rel_root = EnvironmentVariable.Drive;
                            if(interpret_me.Length==drive.Length)
                                new_location.path = "";
                            else
                                new_location.path = interpret_me.Substring(drive.Length);
                            return_me.AddRange(getPaths(new_location));
                        }
                    }
                }
            }
            return return_me;
        }



        protected override List<DetectedLocationPathHolder> getPaths(LocationPathHolder get_me) {
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            DirectoryInfo test;
            DetectedLocationPathHolder add_me;
            switch(get_me.rel_root) {
                case EnvironmentVariable.InstallLocation:
                    LocationPathHolder temp = new LocationPathHolder(get_me);
                    string[] chopped = temp.path.Split(Path.DirectorySeparatorChar);
                    for(int i = 0;i<chopped.Length;i++) {
                        temp.path = chopped[i];
                        for(int j=i+1;j<chopped.Length;j++) {
                            temp.path = Path.Combine(temp.path,chopped[j]);
                        }
                        temp.rel_root = EnvironmentVariable.Drive;
                        return_me.AddRange(getPaths(temp));
                        temp.rel_root = EnvironmentVariable.AltSavePaths;
                        return_me.AddRange(getPaths(temp));
                    }
                    break;
                case EnvironmentVariable.AltSavePaths:
                    foreach(AltPathHolder alt_path in Core.settings.alt_paths) {
                        if(PermissionsHelper.isReadable(alt_path.path)) {
                            if(get_me.path!=null&&get_me.path.Length>0)
                                test = new DirectoryInfo(Path.Combine(alt_path.path, get_me.path));
                            else 
                                test = new DirectoryInfo(alt_path.path);
                            if (test.Exists)  {
                                add_me  = new DetectedLocationPathHolder(get_me);
                                add_me.abs_root = alt_path.path;
                                return_me.Add(add_me);
                            }
                        }
                    }
                    break;
                case EnvironmentVariable.Drive:
                    foreach(string drive in drives) {
                        if(get_me.path!=null&&get_me.path.Length>0)
                            test = new DirectoryInfo(Path.Combine(drive,get_me.path));
                        else
                            test = new DirectoryInfo(drive);
                        if(test.Exists) {
                            add_me  = new DetectedLocationPathHolder(get_me);
                            add_me.abs_root = drive;
                            return_me.Add(add_me);
                        }
                    }
                    break;
                default:
                    return base.getPaths(get_me);
            }
            return return_me;
        }

        public override string getAbsoluteRoot(LocationPathHolder parse_me, string user)
        {
            switch(parse_me.rel_root) {
                case EnvironmentVariable.Drive:
                    DetectedLocationPathHolder holder = (DetectedLocationPathHolder)parse_me;
                    return holder.abs_root;
                default:
                    return base.getAbsoluteRoot(parse_me, user);
            }
            
        }

    }
}
