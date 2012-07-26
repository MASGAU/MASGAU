using System.Windows.Controls;
using System.Windows.Media;
using MVC.Communication;

namespace MVC.WPF {
    public class WPFCommunicationHelpers {
        public static Brush default_progress_color;
        public static void ApplyProgress(ProgressBar progress, ProgressUpdatedEventArgs e) {
            progress.IsEnabled = e.state != ProgressState.None;
            progress.IsIndeterminate = e.state == ProgressState.Indeterminate;
            switch (e.state) {
                case ProgressState.Normal:
                    progress.Foreground = default_progress_color;
                    break;
                case ProgressState.Error:
                    progress.Foreground = Brushes.Red;
                    break;
                case ProgressState.Wait:
                    progress.Foreground = Brushes.Yellow;
                    break;
            }

            progress.Visibility = System.Windows.Visibility.Visible;
            if (e.max == 0)
                progress.Value = 0;
            else {
                progress.Maximum = e.max;
                progress.Value = e.value;
            }
        }

    }
}
