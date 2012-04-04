using System;

namespace MASGAU.Gtk
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class WrappedButton : WrappedWidget
	{
		public global::Gtk.Button Button {
			get {
				return button29;
			}
		}
		
		public string Label {
			get {
					return button29.Label;
			}
			set {
				button29.Label = value;
			}
			
		}
			
		public WrappedButton ()
		{
			this.Build ();
			wrapped_object = button29;
		}
		
	}
}

