﻿using System.ComponentModel;
using MVC.WPF;
using Translator;
namespace MASGAU {
    public abstract class AProgramWindow : NewWindow {
        protected AProgramHandler program_handler;

        public AProgramWindow() { }

        protected AProgramWindow(AProgramHandler program_handler, ACommunicationWindow parent)
            : base(parent) {
            this.program_handler = program_handler;
            this.Loaded += new System.Windows.RoutedEventHandler(WindowLoaded);
            if (program_handler != null)
                this.Title = program_handler.ProgramTitle;
        }
        protected AProgramWindow(AProgramHandler program_handler) : this(program_handler, null) { }

        protected virtual void WindowLoaded(object sender, System.Windows.RoutedEventArgs e) {
            setUpProgramHandler();
        }

        protected virtual void setUpProgramHandler() {
            this.Title = program_handler.ProgramTitle;
            disableInterface();
            program_handler.RunWorkerCompleted += new RunWorkerCompletedEventHandler(setup);
            program_handler.RunWorkerAsync();
        }

        protected virtual void setup(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error != null) {
                this.enableInterface();
                this.Close();
            }

            if (!Common.ProgramReady) {
                this.enableInterface();
                this.Close();
                throw new TranslateableException("CriticalSettingsFailure");
            }
            this.Title = program_handler.ProgramTitle;
        }
    }
}
