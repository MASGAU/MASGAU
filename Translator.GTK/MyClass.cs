using System;
using Gtk;

namespace Translator.GTK {
	public class TranslationHelpers {


		private static bool objectIsOfType(object obj, Type type) {
			Type check = obj.GetType();
			return check.Equals(type) || check.IsSubclassOf(type);
		}

		private static void translateRecursively(Widget obj) {
			if (obj == null ) {
				return;
			} else {
				throw new Exception("Can't translate object " + obj.GetType().ToString());
			}
		}

		public static void translate(Widget obj, string name, params string[] variables) {
			name = "$" + name;
			if (obj == null) {
				return;

			} else {
				throw new Exception("Can't translate object " + obj.GetType().ToString());
			}
		}

	}
}
