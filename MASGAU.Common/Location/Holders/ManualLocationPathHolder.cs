using GameSaveInfo;
namespace MASGAU.Location.Holders {
    public class ManualLocationPathHolder : LocationPath {
        public string ManualPath { get; protected set; }


        public ManualLocationPathHolder(string manual_path) {
            this.ManualPath = manual_path;
            this.override_virtual_store = true;
        }
        public override string ToString() {
            return ManualPath;
        }
    }
}
