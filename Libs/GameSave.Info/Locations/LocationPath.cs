using System.IO;
using System;
using System.Xml;
using XmlData;
namespace GameSaveInfo {
    public class LocationPath : ALocation {
        // Used when dealing with a path
        // ONLY holds the name of the environment variable or wahtever used to figure out the root
        public EnvironmentVariable rel_root;

        public bool IsEnabled { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }

        // Holds only the relative path from the root
        public string Path;

        public override string ElementName {
            get { return "path"; }
        }

        public LocationPath() { }

        public LocationPath(XmlElement element)
            : base(element) {
        }

        public LocationPath(LocationPath copy_me)
            : base(copy_me) {            
            rel_root = copy_me.rel_root;
            Path = copy_me.Path;
            this.IsEnabled = copy_me.IsEnabled;
            this.IsExpanded = copy_me.IsExpanded;
            this.IsSelected = copy_me.IsSelected;
        }

        protected override void LoadMoreData(XmlElement element) {
            foreach (XmlAttribute attrib in element.Attributes) {
                if (attributes.Contains(attrib.Name))
                    continue;

                switch (attrib.Name) {
                    case "ev":
                        this.rel_root = parseEnvironmentVariable(attrib.Value);
                        break;
                    case "path":
                        this.Path = attrib.Value;
                        break;
                    default:
                        throw new NotSupportedException(attrib.Name);
                }
            }
        }

        protected override XmlElement WriteMoreData(XmlElement element) {
            addAtribute(element, "ev", rel_root.ToString().ToLower());
            addAtribute(element, "path", Path);
            return element;
        }

        public override int CompareTo(ALocation comparable) {
            LocationPath location = (LocationPath)comparable;
            int result = compare(this.rel_root, location.rel_root);
            if (result == 0)
                result = compare(this.Path, location.Path);

            return result;
        }


        public override string ToString() {
            if (Path == null)
                return rel_root.ToString();
            else
                return System.IO.Path.Combine(rel_root.ToString(), Path);
        }


    }
}
