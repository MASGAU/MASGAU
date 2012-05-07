using System;
using MASGAU.Communication;
using MASGAU.Communication.Message;
using Gtk;
using MASGAU.Communication.Progress;
using Translations;
namespace MASGAU.Gtk
{
	public class GTKHelpers
	{
		#region translation helpers
		public static void translateWindow(global::Gtk.Window window) {
			string string_title = window.Title;
			window.Title = Strings.get(string_title);
			
			translateContainer(window);
		}
		public static void translateContainer(global::Gtk.Container container) {
			foreach(global::Gtk.Widget widget in container.Children) {
				translateToolTipText(widget);
				if(widget is global::Gtk.FileChooserButton) {
					continue;
				//} else if(widget is Gtk.Button) {
				//	translateButton(widget as Gtk.Button);
				} else if(widget is global::Gtk.Container) {
					translateContainer(widget as global::Gtk.Container);
				} else if(widget is global::Gtk.Label) {
					translateLabel(widget as global::Gtk.Label);
				}
			}
		}
		public static void translateToolTipText(global::Gtk.Widget widget) {
			string string_title = widget.TooltipText;
			widget.TooltipText = Strings.get(string_title);
		}
		public static void translateButton(global::Gtk.Button button) {
			string string_title = button.Label;
			button.Label = Strings.get(string_title);
			button.UseUnderline = true;
		}
		public static void translateLabel(global::Gtk.Label label) {
			string string_title = label.Text;
			if(string_title.StartsWith("http://"))
			   return;
			label.Text = Strings.get(string_title);
		}
		public static void translateWidget(global::Gtk.Widget widget) {
			string string_title = widget.TooltipText;
			widget.TooltipText = Strings.get(string_title);
		}

		#endregion
		
		public static bool checkEmail(Window parent) {
            if(Core.settings.email==null||Core.settings.email=="") {
				EmailDialog get_email = new EmailDialog(parent);
                if((global::Gtk.ResponseType)get_email.Run()!= global::Gtk.ResponseType.Cancel) {
                    Core.settings.email = get_email.email;
                } else {
                    return false;
                }
            }
			return true;
		}
		
        public static void applyProgress(ProgressBar progress, MASGAU.Communication.Progress.ProgressUpdatedEventArgs e) {
			lock(progress) {
				if(e.message!=null)
					progress.Text = e.message;
				progress.Sensitive = e.state== ProgressState.None;
				//progress.
	            //progress.IsIndeterminate = e.state== ProgressState.Indeterminate;
	            switch(e.state) {
	                case ProgressState.Normal:
	                    //progress.Foreground = default_progress_color;
	                    break;
	                case ProgressState.Error:
	                    //progress.Foreground = Brushes.Red;
	                    break;
	                case ProgressState.Wait:
	                    //progress.Foreground = Brushes.Yellow;
	                    break;
	            }
	
	            progress.Visible = true;
	            if(e.max==0)
	                progress.Fraction = 0;
	            else {
					progress.Fraction = (double)e.value/(double)e.max;
	            }
			}
        }

		
		public static string getHeaderString(string name) {
			return "<b>" + Strings.get(name) + "</b>";
		}

		
		public static void sendMessage (MessageEventArgs e)
		{
			switch(e.type) {
			case MessageTypes.Error:
				//showError(e.title,e.message);
				break;
			case MessageTypes.Info:
				//showInfo(e.title,e.message);
				break;
			case MessageTypes.Warning:
				//showWarning(e.title,e.message);
				break;
			}
			e.response = MASGAU.Communication.ResponseType.OK;
		}

		
		#region MessageBox showing things
        public static bool askQuestion(Window parent, string title, string message) {
			return showMessageDialog(parent,title,message,MessageType.Question);
        }
        public static bool showError(Window parent, string title, string message) {
			return showMessageDialog(parent,title,message,MessageType.Error);
        }
        public static bool showWarning(Window parent, string title, string message) {
			return showMessageDialog(parent,title,message,MessageType.Warning);
        }
        public static bool showInfo(Window parent, string title, string message) {
			return showMessageDialog(parent,title,message,MessageType.Info);
        }
		
		public static bool showMessageDialog(Window parent, string title, string message, MessageType type) {
			MessageDialog dialog;
			switch(type) {
			case MessageType.Error:
				dialog = new MessageDialog(parent,DialogFlags.Modal,type,ButtonsType.Close,true,message);
				return (MASGAU.Communication.ResponseType) dialog.Run()==MASGAU.Communication.ResponseType.Cancel;
			case MessageType.Question:
				dialog = new MessageDialog(parent,DialogFlags.Modal,type,ButtonsType.YesNo,true,message);
				return (MASGAU.Communication.ResponseType) dialog.Run()==MASGAU.Communication.ResponseType.Yes;
			default:
				dialog = new MessageDialog(parent,DialogFlags.Modal,type,ButtonsType.Ok,true,message);
				return (MASGAU.Communication.ResponseType) dialog.Run()==MASGAU.Communication.ResponseType.OK;
			}
		}
		#endregion		
	}
}

