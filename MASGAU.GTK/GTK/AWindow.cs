using System;
using System.ComponentModel;
using MVC.Communication;
using MVC.Translator;
using MVC.GTK;

namespace MASGAU {
	public abstract class AWindow: MVC.GTK.AViewWindow, IWindow {

		public AWindow ()
			: this (null, Gtk.WindowType.Toplevel) {
		}

		public AWindow (Gtk.WindowType type)
			: this (null, type) {
		}

		public AWindow (IWindow parent, Gtk.WindowType type) : 
		base (type) {

			if (parent != null) {
				this.Parent = parent as Gtk.Widget;
				this.WindowPosition = Gtk.WindowPosition.CenterOnParent;
			} else {
				this.WindowPosition = Gtk.WindowPosition.Center;
			}
		}
	}
}

