using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
namespace MASGAU {
    public class ASymLinker {
        protected Process linker;
        protected ProcessStartInfo start_info = new ProcessStartInfo();

        public static bool CanLink {
            get {
                return Common.AllUsersMode;
            }
        }

        protected ASymLinker() {
            linker = new Process();
            linker.StartInfo = start_info;

        }
        public bool EstablishLink(string link_location, string target_location) {
            DirectoryInfo Target = new DirectoryInfo(target_location);
            DirectoryInfo Source = new DirectoryInfo(link_location);

            if (Target.Exists) {
                if (IsSymLink(link_location)) {
                    // Already linked
                } else {
                    // Need to merge?
                }
            } else {
                if (IsSymLink(link_location)) {
                    // What the hell is it linked to?
                } else {
                    Source.MoveTo(target_location);

                    // Needs to copy to destination
                }
            }

            return true;
        }

        protected abstract bool IsSymLink(string location);
        protected abstract bool CreateSymLink(string link_location, string target_location);

    }
}
