using System;

namespace MASGAU.Gtk
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class WrapperFileChooserButton : WrappedWidget
	{
		public string FileName {
			get {
				return filechooserbutton1.Filename;
			}
		}
		
		public global::Gtk.FileChooserAction Action {
			get {
				return filechooserbutton1.Action;
			}
			set {
				filechooserbutton1.Action = value;
			}
		}
		
		public bool SelectFilename(String name) {
			return filechooserbutton1.SelectFilename(name);
		}
		
		public WrapperFileChooserButton ()
		{
			this.Build ();
			wrapped_object = filechooserbutton1;
		
		}

		protected void OnFilechooserbutton1SelectionChanged (object sender, System.EventArgs e)
		{
			widgetChanged("FileName");
		}
		
		protected override void propertyChanged (System.ComponentModel.INotifyPropertyChanged model, string object_property)
		{
			
			
			base.propertyChanged (model, object_property);
		}
	}
}

