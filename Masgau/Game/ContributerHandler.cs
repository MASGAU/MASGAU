using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Game {
    public class ContributorHandler : AModelItem {
        private string _name;
        public string name {
            get { return _name; }
            set {
                _name = value;
                NotifyPropertyChanged("name");
            }
        }
        private int _count = 1;
        public int count {
            get { return _count; }
            set {
                _count = value;
                NotifyPropertyChanged("count");
            }
        }

        public ContributorHandler(string name, int count): base(name) {
            this.name = name;
            this.count = count;
        }

        
        public static int Compare(ContributorHandler a, ContributorHandler b) {
            // This goes a little weird due to sorting by count first
            int result = compare(b.count, a.count);
            if (result == 0)
                result = compare(a.name, b.name);
            return result;
        }

        public override int CompareTo(AModelItem<StringID> comparable) {
            return Compare(this, comparable as ContributorHandler);
        }
    }
}
