using System;
using System.Reflection;

namespace MASGAU
{
	class WrapperHelper
	{
		public static void updateValue(AModelItem item, String name, object value) {
			if(item!=null) {
				Type type = item.GetType();
				PropertyInfo info = type.GetProperty(name);
				
				info.SetValue(item,value,null);
			}
		}
		public static bool getBoolean(AModelItem item, String name) {
			object value = getValue(item,name);
			if(value==null)
				throw new Exception(name + " is not boolable!");
			
			return (bool)value;
		}
		public static string getString(AModelItem item, String name) {
			object value = getValue(item,name);
			if(value==null)
				return null;
			
			return value.ToString();
		}
		
		public static object getValue(AModelItem item, String name) {
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

