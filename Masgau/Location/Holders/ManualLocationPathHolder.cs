
namespace MASGAU.Location.Holders {
    public class ManualLocationPathHolder : LocationPathHolder {
        string manual_path;
        public ManualLocationPathHolder(string manual_path) {
            this.manual_path = manual_path;
            this.override_virtual_store = true;
        }
        public override string ToString() {
            return manual_path;
        }
    }
}
