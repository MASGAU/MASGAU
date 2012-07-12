using MASGAU.Location;

namespace MASGAU.Main {
    class MainProgramHandler : AMainProgramHandler<LocationsHandler> {
        public MainProgramHandler()
            : base(Interface.WPF) {

        }
    }
}
