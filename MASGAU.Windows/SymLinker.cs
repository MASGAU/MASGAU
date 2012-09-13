using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
namespace MASGAU {
    public class SymLinker : ASymLinker {

        private const string CheckArgs = "";
        private const string CreateArgs = "";
        private const string DeleteArgs = "";

        public SymLinker() {
        }



        protected override bool CreateSymLink(string link_location, string target_location) {
            throw new NotImplementedException();
        }

        protected override bool IsSymLink(string location) {
            throw new NotImplementedException();
        }
    }
}
