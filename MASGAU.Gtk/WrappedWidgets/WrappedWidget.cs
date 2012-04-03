using System;
using System.Reflection;

namespace MASGAU
{
	public abstract partial class WrappedWidget : Gtk.Bin, IView
	{
		public WrappedWidget ()
		{
		}
		
		protected AModelItem modelItem = null;
		protected string modelItemProperty = null;
		
		public void attachModelItem(AModelItem source, String name) {
			if(modelItem!=null)
				this.detachModelItem();
			
			
			modelItemProperty = name;
			modelItem = source;
			
			propertyChanged(name);
			
			modelItem.PropertyChanged += HandleSourcePropertyChanged;
		}

		protected void HandleSourcePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			propertyChanged(e.PropertyName);
		}
		protected abstract void propertyChanged(String propertyName);
		
		public void detachModelItem() {
			modelItemProperty = null;
			modelItem.PropertyChanged -= HandleSourcePropertyChanged;
			modelItem = null;
		}
		
		protected static void updateValue(AModelItem item, String name, object value) {
			if(item!=null) {
				Type type = item.GetType();
				PropertyInfo info = type.GetProperty(name);
				
				info.SetValue(item,value,null);
			}
		}
		protected static bool getBoolean(AModelItem item, String name) {
			object value = getValue(item,name);
			if(value==null)
				throw new Exception(name + " is not boolable!");
			
			return (bool)value;
		}
		
		protected static string getString(AModelItem item, String name) {
			object value = getValue(item,name);
			if(value==null)
				return null;
			
			return value.ToString();
		}
		
		protected static object getValue(AModelItem item, String name) {
			if(item!=null) {
				Type type = item.GetType();
				PropertyInfo info = type.GetProperty(name);
				object value = info.GetValue(item,null);
				return value;
			}
			return null;
		}
		
	}
}

