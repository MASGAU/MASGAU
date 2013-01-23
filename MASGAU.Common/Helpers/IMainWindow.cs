using System;
using MASGAU.Main;
using MASGAU.Location;
namespace MASGAU {
	public interface IMainWindow: IWindow {
		string Title { get; set; }
		MainProgramHandler masgau { get; }

		void unHookData();
		void hookData();

		Config.WindowState StartupState { set; }

	}
}

