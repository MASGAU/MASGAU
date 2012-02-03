using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Location {

    public enum EnvironmentVariable {
        // These are ordered according to priority, do NOT reorganize
        None,
        AltSavePaths,
        InstallLocation,

        Drive,

        SteamUser,
        SteamCommon,
        SteamSourceMods,
        SteamUserData,

        AllUsersProfile,
        Public,

        //The point of the ordering is so that certain relative roots can take precedence
        // For instance, user profile is the most general
        UserProfile,
        // And all these are within that path, but more specific locations
        UserDocuments,
        AppData,
        LocalAppData,
        SavedGames,

        FlashShared,

        VirtualStore,

        // In the anlyzer we prefer the real program files path over the virtualstore one, so we give them higher priority
        ProgramFiles,
        ProgramFilesX86,

        PS3Export,
        PS3Save,
        PSPSave
    }
}
