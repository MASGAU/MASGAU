using System;

namespace MASGAU
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class Entry : Gtk.Bin, IView
	{
		public Entry ()
		{
			this.Build ();
			entry1.Changed += HandleEntry1Changed;
		}

		void HandleEntry1Changed (object sender, EventArgs e)
		{
			WrapperHelper.updateValue(modelItem,modelItemProperty,entry1.Text);
		}
		
		private AModelItem modelItem = null;
		private string modelItemProperty = null;
		public void attachModelItem(AModelItem source, String name) {
			if(modelItem!=null)
				this.detachModelItem();
			
			entry1.Text = WrapperHelper.getString(source,name);
			
			modelItemProperty = name;
			modelItem = source;
			source.PropertyChanged += HandleSourcePropertyChanged;
		}

		void HandleSourcePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName==modelItemProperty) {
				
			}
		}
		public void detachModelItem() {
			modelItemProperty = null;
			modelItem = null;
		}
	}
}

