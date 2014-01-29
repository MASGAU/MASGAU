using System;
using System.ComponentModel;
using MVC.Communication;
using MVC.Translator;
using MVC.GTK;

namespace MASGAU {
	public abstract class AWindow: MVC.GTK.AViewWindow, IWindow {

		public AWindow ()
			: this (null) {
		}

		public AWindow (IWindow parent) : 
			base (parent) {

			if (parent != null) {
				this.Parent = parent as Gtk.Widget;
				this.WindowPosition = Gtk.WindowPosition.CenterOnParent;
			} else {
				this.WindowPosition = Gtk.WindowPosition.Center;
			}
		}
	}
}

