using System;
using System.ComponentModel;
using MASGAU.Location;
using Translations;
namespace MASGAU.Gtk
{
	public class AProgramWindow: AWindow
	{
		
		protected AProgramHandler<Location.LocationsHandler>  program_handler;
		
		public AProgramWindow(global::Gtk.WindowType window_type, AProgramHandler<Location.LocationsHandler> program_handler): base(window_type)
		{
            this.program_handler = program_handler;
			
            if(program_handler!=null)
                this.Title = program_handler.program_title;
		}
		
        protected virtual void setUpProgramHandler() {
            this.Title = program_handler.program_title;
            disableInterface();
            program_handler.RunWorkerCompleted += new RunWorkerCompletedEventHandler(setup);
            program_handler.RunWorkerAsync();
        }
		
        protected virtual void setup(object sender, RunWorkerCompletedEventArgs e) {
            if(e.Error!=null) {
                this.enableInterface();
                this.Destroy();
				return;
            }

            if(!Core.initialized) {
                this.enableInterface();
                this.Destroy();
                throw new MException(Strings.get("Error"),Strings.get("SettingsLoadError"),false);
            }
            this.Title = program_handler.program_title;
        }

		
	}
}

