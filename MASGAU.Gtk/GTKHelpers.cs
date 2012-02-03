using System;
using MASGAU.Communication;
using MASGAU.Communication.Message;
using Gtk;
using MASGAU.Communication.Progress;
using Translations;
namespace MASGAU
{
	public class GTKHelpers
	{
		
		public static bool checkEmail(Window parent) {
            if(Core.settings.email==null||Core.settings.email=="") {
				EmailDialog get_email = new EmailDialog(parent);
                if((Gtk.ResponseType)get_email.Run()!= Gtk.ResponseType.Cancel) {
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

