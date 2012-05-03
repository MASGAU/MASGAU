using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication.Progress {
    public enum ProgressState {
        Normal,
        Wait,
        Error,
        Indeterminate,
        None
    }
}
