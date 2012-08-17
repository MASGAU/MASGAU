using MVC;
namespace MASGAU {
    class SelectFile : AModelItem {
        public SelectFile(string name)
            : base(name) {
            this.name = name;
            this.IsSelected = true;
        }
        public string name {
            get;
            set;
        }

    }
}
