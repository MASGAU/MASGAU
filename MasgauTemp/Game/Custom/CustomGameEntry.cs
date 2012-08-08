using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU {
    public class CustomGameEntry: GameEntry {
        public bool Submitted {
            get {
                return ((CustomGame)Game).Submitted;
            }
            set {
                ((CustomGame)Game).Submitted = value;
            }
        }

        public CustomGameEntry(CustomGameVersion ver)
            : base(ver) {
                this.Detect();

        }
    }
}
