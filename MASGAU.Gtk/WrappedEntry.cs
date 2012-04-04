using System;

namespace MASGAU.Gtk
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class WrappedEntry : WrappedWidget
	{
		
		public String Text {
			get {
				return entry1.Text;
			}
		}
		
		public WrappedEntry ()
		{
			this.Build ();
			wrapped_object = entry1;
			entry1.Changed += HandleEntry1Changed;
		}

		void HandleEntry1Changed (object sender, EventArgs e)
		{
				widgetChanged("Text");
		}
		
	}
}

