﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using Communication.Message;

namespace MASGAU
{
    public class BackgroundWorker: System.ComponentModel.BackgroundWorker, INotifyPropertyChanged
    {

        public BackgroundWorker(): base() {
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
            this.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(forwardExceptions);
        }

        protected void forwardExceptions(object sender, RunWorkerCompletedEventArgs e) {
            if(e.Error!=null) {
                MessageHandler.SendException(e.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string prop)
        {
           if( PropertyChanged != null )
           {
              PropertyChanged(this, new PropertyChangedEventArgs(prop));
           }
        }
    }
}
