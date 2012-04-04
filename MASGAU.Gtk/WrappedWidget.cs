using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
namespace MASGAU.Gtk
{
	public abstract partial class WrappedWidget : global::Gtk.Bin
	{
		
		private Dictionary<string,string> properties = new Dictionary<string, string>();
		private Dictionary<string,INotifyPropertyChanged> models = new Dictionary<string, INotifyPropertyChanged>();
		
		protected object wrapped_object;
		
		public WrappedWidget() {
		}

		public void attachModelItem (string object_property, INotifyPropertyChanged model, string model_property)
		{
			if(models.ContainsKey(object_property)) {
				if(models[object_property]!=model) {
					detachModelItem(object_property);
					models.Add(object_property,model);
				}
			} else {
				models.Add(object_property,model);
			}
						
			if(properties.ContainsKey(object_property))
				properties[object_property] = model_property;
			else
				properties.Add(object_property,model_property);
			
			propertyChanged(model,model_property);
			
			model.PropertyChanged += HandleSourcePropertyChanged;
		}

		protected void HandleSourcePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			propertyChanged(sender as INotifyPropertyChanged, e.PropertyName);
		}
		
		protected void widgetChanged(string widget_property) {
			if(models.ContainsKey(widget_property)&&properties.ContainsKey(widget_property)) {
				updateProperty(models[widget_property],properties[widget_property],wrapped_object,widget_property);	
			}
		}
		
		protected virtual void propertyChanged(INotifyPropertyChanged model, string object_property) {
			if(models.ContainsValue(model)&&properties.ContainsValue(object_property)) {
				foreach(string key in models.Keys) {
					if(models[key]==model) {
						foreach(string prop in properties.Keys) {
							if(properties[prop]==object_property) {
								updateProperty(wrapped_object, prop,model,object_property);
							}
						}
					}
				}
			} else {
				//throw new MException("Say what", "A model jsut tried to send property update to a widget it wasn't attached too", false);
			}
		}
		
		protected void updateProperty(object dest, string dest_property, object source, string source_property) {
			if(source!=null) {
				Type source_type = source.GetType();
				Type dest_type = dest.GetType(); 
					
				PropertyInfo source_info = source_type.GetProperty(source_property);
				PropertyInfo dest_info = dest_type.GetProperty(dest_property);
				
				object value = source_info.GetValue(source,null);
				
				dest_info.SetValue (dest,value,null);
			}
		}
		
		public void detachModelItem(string object_property) {
			if(models.ContainsKey(object_property)) {
				models[object_property].PropertyChanged-= HandleSourcePropertyChanged;
				models.Remove(object_property);
			}
			if(properties.ContainsKey(object_property))
				properties.Remove(object_property);
		}
		
		protected static void updateValue(INotifyPropertyChanged item, String name, object value) {
			if(item!=null) {
				Type type = item.GetType();
				PropertyInfo info = type.GetProperty(name);
				
				info.SetValue(item,value,null);
			}
		}
		protected static bool getBoolean(INotifyPropertyChanged item, String name) {
			object value = getValue(item,name);
			if(value==null)
				throw new Exception(name + " is not boolable!");
			
			return (bool)value;
		}
		
		protected static string getString(INotifyPropertyChanged item, String name) {
			object value = getValue(item,name);
			if(value==null)
				return null;
			
			return value.ToString();
		}
		
		protected static object getValue(INotifyPropertyChanged item, String name) {
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

