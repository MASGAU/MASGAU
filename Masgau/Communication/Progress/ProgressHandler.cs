using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MASGAU.Communication.Progress
{
    #region Progress Definitions
    public delegate void ProgressChangedEventHandler(ProgressUpdatedEventArgs e);
    #endregion

    public class ProgressHandler: CommunicationHandler
    {
        private static string _progress_message = null;
        public static string progress_message {
            get { 
                return _progress_message;
            }
            set {
                _progress_message = value;
                setProgress();
            }
        }
        private static int _progress = 0;
        public static int progress {
            set{
                _progress = value;
                _progress_state = ProgressState.Normal;
                setProgress();
            }
            get {
                return _progress;
            }
        }
        private static int _progress_max = 0; 
        public static int progress_max {
            set{
                _progress_max = value;
                setProgress();
            }
            get {
                return _progress_max;
            }
        }
        private static ProgressState _progress_state = ProgressState.Normal;
        public static ProgressState progress_state {
            set{
                _progress_state = value;
                setProgress();
            }
            get {
                return _progress_state;
            }
        }
        private static int _sub_progress = 0;
        public static int sub_progress {
            set{
                _sub_progress = value;
                setSubProgress();
            }
            get {
                return _sub_progress;
            }
        }
        private static int _sub_progress_max = 0;
        public static int sub_progress_max {
            set{
                _sub_progress_max = value;
                setSubProgress();
            }
            get {
                return _sub_progress_max;
            }
        }
        private static ProgressState _sub_progress_state = ProgressState.Normal;
        public static ProgressState sub_progress_state {
            set{
                _sub_progress_state = value;
                setSubProgress();
            }
            get {
                return _sub_progress_state;
            }
        }


        //protected static event ProgressChangedEventHandler ProgressChanged;
        //protected static event ProgressChangedEventHandler SubProgressChanged;

        private static void setProgress() {
            setProgress(_progress,_progress_max,_progress_message,_progress_state);
        }

        private static void setProgress(int value, int max, string message, ProgressState progstate) {
            ProgressUpdatedEventArgs e = new ProgressUpdatedEventArgs();
            e.max = max;
            e.message = message;
            e.value = value;
            e.state = progstate;
            ICommunicationReceiver receiver = getReceiver();
            if(receiver==null)
                return;

            if(receiver.context!=null) {
                receiver.context.Post(new SendOrPostCallback(delegate(object state) {
                    ProgressChangedEventHandler handler = receiver.updateProgress;
                    if(handler!=null) {
                        handler(e);
                    }
                }),null);
            } else {
                receiver.updateProgress(e);
            }
        }

        private static void setSubProgress() {
            setSubProgress(_sub_progress,_sub_progress_max,_sub_progress_state);
        }

        private static void setSubProgress(int value, int max, ProgressState progstate) {
            ProgressUpdatedEventArgs e = new ProgressUpdatedEventArgs();
            e.max = max;
            e.message = null;
            e.value = value;
            e.state = progstate;

            ICommunicationReceiver receiver = getReceiver();
            if(receiver==null)
                return;

            if(receiver.context!=null) {
                receiver.context.Post(new SendOrPostCallback(delegate(object state) {
                    ProgressChangedEventHandler handler = receiver.updateProgress;
                    if(handler!=null) {
                        handler(e);
                    }
                }),null);
            } else {
                receiver.updateProgress(e);
            }
        }
    }
}
