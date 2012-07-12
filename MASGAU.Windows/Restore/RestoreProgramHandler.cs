using MASGAU.Location;

namespace MASGAU.Restore {
    public class RestoreProgramHandler : ARestoreProgramHandler<LocationsHandler> {
        public RestoreProgramHandler(Archive archive) : base(Interface.WPF, archive) { }
    }
}
