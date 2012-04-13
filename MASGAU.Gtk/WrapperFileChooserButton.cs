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
			set {
				filechooserbutton1.SelectFilename(value);
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
		
		protected override void widgetChanged (string widget_property)
		{
			if(widget_property=="FileName") {
				this.updateProperty(models["FileName"],properties["FileName"],filechooserbutton1.Filename);
			} else {
				base.widgetChanged (widget_property);
			}
		}
		
		protected override void propertyChanged (System.ComponentModel.INotifyPropertyChanged model, string model_property)
		{
			if(propertyTest(model, model_property,"FileName")) {
				this.FileName = getValue(model,model_property).ToString();
			} else {
				base.propertyChanged (model, model_property);
			}
		}
	}
}

