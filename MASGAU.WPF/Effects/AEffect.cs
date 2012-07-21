using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Reflection;
// Rutlessly stolen from http://www.codeproject.com/Articles/24543/Creating-and-Reusing-Dynamic-Animations-in-Silverl
// Modified to hell afterward
namespace MASGAU.Effects {
    public abstract class AEffect {
        protected FrameworkElement target;
        Storyboard storyboard;

        protected abstract Storyboard CreateStoryboard(FrameworkElement target);
        protected virtual void Add(FrameworkElement target) {
            this.target = target;
            storyboard = CreateStoryboard(target);
            storyboard.Completed += new EventHandler(OnCompleted);
            if (target.Resources.Contains("animation")) {
                Storyboard story = target.Resources["animation"] as Storyboard;
                story.Remove();
                target.Resources.Remove("animation");
            }
            target.Resources.Add("animation",storyboard);
        }

        protected virtual void Remove() {
            if (target == null)
                return;
            if (storyboard == null)
                return;
            target.Resources.Remove("animation");
            target = null;
            storyboard = null;
        }
        protected virtual void OnCompleted(object sender, EventArgs e) {
            Remove();
            if (Completed != null)
                Completed(sender, e);
        }

        public virtual void Start(FrameworkElement target) {
            Add(target);
            storyboard.Begin();
        }
        public virtual void Stop() {
            if (target == null || storyboard == null)
                return;
            storyboard.Stop();
            Remove();
        }

        public event EventHandler Completed;
    }
}
