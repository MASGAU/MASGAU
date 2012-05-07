using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVC;
namespace MASGAU.Location.Holders {
    public class AltPathHolder : AModelItem<StringID> {
        public string path {
            get {
                return id.ToString();
            } 
        }
        public AltPathHolder(string new_path): base(new StringID(new_path)) {

        }
    }
}
