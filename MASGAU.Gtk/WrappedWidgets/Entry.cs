using System;

namespace MASGAU
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class Entry : WrappedWidget
	{
		public Entry ()
		{
			this.Build ();
			entry1.Changed += HandleEntry1Changed;
		}

		void HandleEntry1Changed (object sender, EventArgs e)
		{
			updateValue(modelItem,modelItemProperty,entry1.Text);
		}
		
		protected override void propertyChanged (string propertyName)
		{
			entry1.Text = getString(modelItem,modelItemProperty);
		}

	}
}

