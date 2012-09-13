using System;
using System.Collections.Generic;
using System.IO;
using GameSaveInfo;
using MASGAU.Location.Holders;
namespace MASGAU.Location {

    public abstract class APlaystationLocationHandler : ALocationHandler {
        public APlaystationLocationHandler()
            : base(HandlerType.PlayStation) {
        }

        public abstract List<string> GetDriveCandidates();

        protected bool psp_found {
            get {
                return getUsers(EnvironmentVariable.PSPSave).Count > 0;
            }
        }
        protected bool ps3_found {
            get {
                return getUsers(EnvironmentVariable.PS3Save).Count > 0;
            }
        }
        public override bool ready {
            get { return psp_found || ps3_found; }
        }

        protected override DetectedLocations getPaths(APlayStationID get_me) {
            DetectedLocations return_me = new DetectedLocations();
            if (get_me.suffix == null || get_me.prefix == null)
                return return_me;

            Type check = get_me.GetType();
            if (check.Equals(typeof(PlayStation1ID))) {
                return_me.AddRange(detectPS3Export(get_me as APlayStationID));
            } else if (check.Equals(typeof(PlayStation2ID))) {
                return_me.AddRange(detectPS3Export(get_me as APlayStationID));
            } else if (check.Equals(typeof(PlayStation3ID))) {
                return_me.AddRange(detectPSGame(get_me as PlayStation3ID, EnvironmentVariable.PS3Save));
            } else if (check.Equals(typeof(PlayStationPortableID))) {
                return_me.AddRange(detectPSGame(get_me as PlayStationPortableID, EnvironmentVariable.PSPSave));
            }
            return return_me;
        }



        private List<DetectedLocationPathHolder> detectPSGame(APlayStationID id, EnvironmentVariable ev) {
            id.clearPath();
            id.EV = ev;
            DetectedLocations interim = getPaths(id as LocationPath);
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            foreach (DetectedLocationPathHolder path in interim) {
                DirectoryInfo test = new DirectoryInfo(path.full_dir_path);

                if (test.GetDirectories(id.ToString()).Length > 0) {
                    path.owner = null;
                    return_me.Add(path);
                }

            }
            return return_me;
        }

        private List<DetectedLocationPathHolder> detectPS3Export(APlayStationID id) {
            id.clearPath();
            id.EV = EnvironmentVariable.PS3Export;
            DetectedLocations interim = getPaths(id as LocationPath);
            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            foreach (DetectedLocationPathHolder path in interim) {
                DirectoryInfo test = new DirectoryInfo(path.full_dir_path);
                if (test.GetFiles(id.ToString()).Length > 0) {
                    path.owner = null;
                    return_me.Add(path);
                }

            }
            return return_me;
        }
    }
}
