using System.Windows.Shell;
using Translator;
namespace MASGAU.Main {
    public partial class MainWindowNew {
        private static void setupJumpList() {
            #region Jumplist stuff
            // Jumplist setup
            JumpList masgau_jump_list = JumpList.GetJumpList(Application.Current);
            if (masgau_jump_list == null) {
                masgau_jump_list = new JumpList();
                JumpList.SetJumpList(Application.Current, masgau_jump_list);
            } else {
                masgau_jump_list.JumpItems.Clear();
                masgau_jump_list.ShowFrequentCategory = false;
                masgau_jump_list.ShowRecentCategory = false;

            }

            JumpTask masgau_jump = new JumpTask();
            masgau_jump.ApplicationPath = Core.ExecutableName;
            masgau_jump.IconResourcePath = Core.ExecutableName;
            masgau_jump.IconResourceIndex = 0;
            masgau_jump.WorkingDirectory = Core.ExecutablePath;
            masgau_jump.Title = Strings.GetLabelString("JumpMainProgram");
            masgau_jump.Description = Strings.GetToolTipString("JumpMainProgram");
            masgau_jump.CustomCategory = "MASGAU";
            masgau_jump_list.JumpItems.Add(masgau_jump);

            masgau_jump = new JumpTask();
            masgau_jump.ApplicationPath = Core.ExecutableName;
            masgau_jump.IconResourcePath = Core.ExecutableName;
            masgau_jump.IconResourceIndex = 0;
            masgau_jump.WorkingDirectory = Core.ExecutablePath;
            masgau_jump.Title = Strings.GetLabelString("JumpMainProgramAllUsers");
            masgau_jump.Description = Strings.GetToolTipString("JumpMainProgramAllUsers");
            masgau_jump.Arguments = "-allusers";
            masgau_jump.CustomCategory = "MASGAU";
            masgau_jump_list.JumpItems.Add(masgau_jump);

            masgau_jump_list.Apply();
            #endregion

        }

    }
}
