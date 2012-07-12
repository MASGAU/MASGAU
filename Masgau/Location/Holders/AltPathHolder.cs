using MVC;
namespace MASGAU.Location.Holders {
    public class AltPathHolder : AModelItem<StringID> {
        public string path {
            get {
                return id.ToString();
            }
        }
        public AltPathHolder(string new_path)
            : base(new StringID(new_path)) {

        }
    }
}
