using Translator;
namespace MASGAU.Main {
    public partial class MainWindowNew : ITranslateableWindow {

        #region ITranslateableWindow implementations
        public void setTranslatedTitle(string name, params string[] variables) {
            this.Title = Strings.GetLabelString(name, variables);
        }


        #endregion

    }
}
