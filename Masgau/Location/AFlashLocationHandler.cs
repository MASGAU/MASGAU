using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Location {
    public abstract class AFlashLocationHandler: ALocationHandler {
        protected AFlashLocationHandler()
            : base(HandlerType.Flash) {
        }
    }
}
